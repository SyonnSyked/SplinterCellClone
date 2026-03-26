using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static GuardVision;

public enum AIState { Patrol, Investigate, SearchForMissing, HighAlert }

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyGuard : MonoBehaviour, iDamage
{
    [Header("--- Health & Death ---")]
    public int HP = 25;
    public bool isDead = false;

    [Header("--- State Management ---")]
    public AIState currentState = AIState.Patrol;
    public Transform[] patrolWaypoints;
    public float waypointBuffer = 1.5f;
    public float bodySearchDistance = 3.0f; // Keeps them from stepping on bodies
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    [Header("--- Dialogue UI ---")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueCanvas;
    public float textDuration = 2.5f;
    private Coroutine dialogueCoroutine;
    private string lastSaidMessage = "";

    [Header("--- Combat (Staff) ---")]
    public Transform staffPivot;
    public int staffDamage = 20;
    public float attackRange = 2.5f;
    public float attackCooldown = 2.0f;
    public Vector3 swingRotation = new Vector3(0, 90, 0);
    public LayerMask playerLayer;

    private float lastAttackTime;
    private bool isSwinging = false;
    private Quaternion originalStaffRotation;
    private Transform playerTransform;

    [Header("--- Buddy System & Memory ---")]
    public EnemyGuard buddyAI;
    private List<Vector3> distractionHistory = new List<Vector3>();
    private static List<EnemyGuard> allGuards = new List<EnemyGuard>();

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        allGuards.Add(this);

        if (staffPivot != null) originalStaffRotation = staffPivot.localRotation;
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
    }

    void Start()
    {
        try { gameObject.CompareTag("Body"); }
        catch { Debug.LogError($"{gameObject.name}: Tag 'Body' not found"); }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (isDead || isSwinging) return;

        switch (currentState)
        {
            case AIState.Patrol:
                UpdatePatrol();
                CheckOnBuddy();
                break;

            case AIState.HighAlert:
                HandleCombat();
                break;

            case AIState.Investigate:
                break;

            case AIState.SearchForMissing:
                break;
        }
    }


    // --- DIALOGUE SYSTEM ---

    public void Say(string message)
    {
        if (isDead || dialogueText == null || dialogueCanvas == null) return;
        if (message == lastSaidMessage && dialogueCanvas.activeSelf) return;

        if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);
        dialogueCoroutine = StartCoroutine(ShowText(message));
    }

    IEnumerator ShowText(string message)
    {
        lastSaidMessage = message;
        dialogueCanvas.SetActive(true);
        dialogueText.text = message;
        yield return new WaitForSeconds(textDuration);
        dialogueCanvas.SetActive(false);
        lastSaidMessage = "";
    }

    // --- DETECTION TRIGGERS ---

    public void OnSeenObject(GameObject obj)
    {
        if (isDead || isSwinging) return;

        if (obj.CompareTag("Body") && currentState != AIState.HighAlert)
        {
            Say("Man down! I found a body!");
            currentState = AIState.HighAlert;

            // Set stopping distance so they don't step on the body
            agent.stoppingDistance = bodySearchDistance;
            agent.SetDestination(obj.transform.position);

            RadioAllies("We have a casualty!");
        }
        else if (obj.CompareTag("Player"))
        {
            if (currentState != AIState.HighAlert)
            {
                Say("Found you, intruder!");
                currentState = AIState.HighAlert;

                // Reset stopping distance for combat
                agent.stoppingDistance = attackRange - 0.5f;
                RadioAllies("Intruder spotted!");
            }
        }
    }

    public void OnHearNoise(Vector3 location)
    {
        if (isDead || isSwinging || currentState == AIState.HighAlert) return;

        int repeatedCount = distractionHistory.FindAll(pos => Vector3.Distance(pos, location) < 2f).Count;
        if (repeatedCount < 3)
        {
            Say("What was that...?");
            distractionHistory.Add(location);
            StopAllCoroutines();
            StartCoroutine(InvestigateLocation(location));
        }
    }

    // --- COMBAT & DAMAGE ---

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        HP -= amount;
        Say("I'm hit!");
        if (currentState != AIState.HighAlert) currentState = AIState.HighAlert;
        if (HP <= 0) Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // --- DETACHES THE DIALOGUE ---
        if (dialogueCanvas != null)
        {
            // This un-sticks the UI from the guard so it stays floating
            dialogueCanvas.transform.SetParent(null);

            // Keep it slightly above the death point
            dialogueCanvas.transform.position += Vector3.up * 0.5f;
        }

        Say("...Backup... needed...");

        gameObject.tag = "Body";
        agent.isStopped = true;
        agent.enabled = false;

        // Body falls over
        transform.rotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y, 0);

        // Hide the floating text after a few seconds so it doesn't stay forever
        Invoke("HideUI", 3f);

        this.enabled = false;
    }


    void HideUI() { if (dialogueCanvas != null) dialogueCanvas.SetActive(false); }

    void HandleCombat()
    {
        if (playerTransform == null) return;

        agent.stoppingDistance = attackRange - 0.5f;
        agent.SetDestination(playerTransform.position);
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            StartCoroutine(StaffSwing());
            lastAttackTime = Time.time;
        }
    }

    IEnumerator StaffSwing()
    {
        isSwinging = true;
        agent.isStopped = true;
        Say("Hyaah!");

        Quaternion windUpRot = originalStaffRotation * Quaternion.Euler(0, -30, 0);
        float t = 0;
        while (t < 0.2f)
        {
            staffPivot.localRotation = Quaternion.Slerp(staffPivot.localRotation, windUpRot, t / 0.2f);
            t += Time.deltaTime;
            yield return null;
        }

        Quaternion targetRot = originalStaffRotation * Quaternion.Euler(swingRotation);
        t = 0;
        bool damageDone = false;
        while (t < 0.15f)
        {
            staffPivot.localRotation = Quaternion.Slerp(staffPivot.localRotation, targetRot, t / 0.15f);
            if (t > 0.07f && !damageDone)
            {
                CheckForImpact();
                damageDone = true;
            }
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        t = 0;
        while (t < 0.3f)
        {
            staffPivot.localRotation = Quaternion.Slerp(staffPivot.localRotation, originalStaffRotation, t / 0.3f);
            t += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        isSwinging = false;
    }

    void CheckForImpact()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, attackRange, playerLayer);
        foreach (Collider h in hits) h.GetComponent<iDamage>()?.TakeDamage(staffDamage);
    }

    // --- MOVEMENT & RADIO ---

    void UpdatePatrol()
    {
        if (patrolWaypoints.Length == 0 || agent.isStopped) return;

        agent.stoppingDistance = 0; // Waypoints need exact arrival
        agent.SetDestination(patrolWaypoints[currentWaypointIndex].position);

        if (!agent.pathPending && agent.remainingDistance < waypointBuffer)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }

    void CheckOnBuddy()
    {
        if (buddyAI != null && (buddyAI.isDead || buddyAI.gameObject.CompareTag("Body")))
        {
            if (currentState != AIState.HighAlert && currentState != AIState.SearchForMissing)
            {
                Say("Partner? Where are you?");
                currentState = AIState.SearchForMissing;

                // FIX: Apply the distance here too
                agent.stoppingDistance = bodySearchDistance;
                agent.SetDestination(buddyAI.transform.position);
            }
        }
    }

    IEnumerator InvestigateLocation(Vector3 location)
    {
        currentState = AIState.Investigate;
        agent.isStopped = false;
        agent.stoppingDistance = 1f; // Stop slightly away from noise source
        agent.SetDestination(location);

        while (agent.pathPending || agent.remainingDistance > 1.2f) yield return null;

        yield return new WaitForSeconds(3f);
        if (currentState != AIState.HighAlert) currentState = AIState.Patrol;
    }

    public void RadioAllies(string message)
    {
        foreach (var guard in allGuards)
        {
            if (guard != this && !guard.isDead && guard.currentState != AIState.HighAlert)
                guard.ReceiveRadioAlert(transform.position);
        }
    }

    public void ReceiveRadioAlert(Vector3 pos)
    {
        if (isDead || currentState == AIState.HighAlert) return;
        StopAllCoroutines();
        StartCoroutine(InvestigateLocation(pos));
    }

    void OnDestroy() => allGuards.Remove(this);

    public void takeDamage(int amount, PlayerState Instigator, bool Headshot = false)
    {
       
    }
}

