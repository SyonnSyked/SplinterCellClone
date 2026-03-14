using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum AIState { Patrol, Investigate, SearchForMissing, HighAlert }

public class EnemyGuard : MonoBehaviour, iDamage
{
    [Header("Movement Settings")]
    public Transform[] patrolWaypoints;
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    [Header("Stealth Logic")]
    public AIState currentState = AIState.Patrol;
    public float suspicionLevel = 0f;
    public float buddyCheckTimer = 0f;
    public const float BUDDY_MISSING_THRESHOLD = 30f;

    [Header("Combat Settings")]
    [SerializeField] int HP;
    public float AttackRange = 2.0f;
    public float AttackCooldown = 1.5f;
    [SerializeField] Renderer model;
    private float LastAttackTime;
    private Transform PlayerTransform;
    public bool isDead = false;
    Color colorOrg;

    [Header("Memory")]
    private List<Vector3> distractionHistory = new List<Vector3>();
    private static List<EnemyGuard> allGuards = new List<EnemyGuard>();

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        allGuards.Add(this); // Registration for "Radio" communication
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                UpdatePatrol();
                CheckForMissingPatrols();
                break;
            case AIState.Investigate:
                break;
            case AIState.HighAlert:
                agent.speed = 5.0f; // Run to player or body
                AttackPlayer();
                break;
        }
    }

    public void OnHearNoise(Vector3 location)
    {
        // Check memory: Is this the 3rd time we heard a noise at this exact spot
        int repeatedCount = distractionHistory.FindAll(pos => Vector3.Distance(pos, location) < 1.0f).Count;

        if (repeatedCount < 3)
        {
            distractionHistory.Add(location);
            StopAllCoroutines();
            StartCoroutine(InvestigateLocation(location));
        }
        else
        {
            Debug.Log($"{gameObject.name}: Ignoring repeated distraction.");
        }
    }

    public void OnSeeBody(GameObject body)
    {
        if (currentState != AIState.HighAlert)
        {
            currentState = AIState.HighAlert;
            RadioAllies("Man down! There's a body at " + body.transform.position);
        }
    }

    void UpdatePatrol()
    {
        if (patrolWaypoints.Length == 0) return;

        agent.destination = patrolWaypoints[currentWaypointIndex].position;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
            buddyCheckTimer = 0f; // Reset timer when reaching a waypoint (active patrol)
        }
    }

    void CheckForMissingPatrols()
    {
        buddyCheckTimer += Time.deltaTime;
        if (buddyCheckTimer > BUDDY_MISSING_THRESHOLD)
        {
            currentState = AIState.SearchForMissing;
            RadioAllies("There's a post not reporting in. I'm checking it out.");
        }
    }

    IEnumerator InvestigateLocation(Vector3 location)
    {
        currentState = AIState.Investigate;
        agent.destination = location;

        // Wait until we arrive
        while (agent.pathPending || agent.remainingDistance > 0.5f)
            yield return null;

        // Look around for 3 seconds
        yield return new WaitForSeconds(3f);

        currentState = AIState.Patrol;
    }

    public void RadioAllies(string message)
    {
        Debug.Log($"[RADIO] {message}");
        foreach (var guard in allGuards)
        {
            if (guard != this)
            {
                // Alert nearby guards to the same location
                guard.ReceiveRadioAlert(transform.position);
            }
        }
    }

    public void ReceiveRadioAlert(Vector3 alertPos)
    {
        if (currentState != AIState.HighAlert)
        {
            currentState = AIState.Investigate;
            agent.destination = alertPos;
        }
    }

    void AttackPlayer()
    {
        if (PlayerTransform == null)
        {
            PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            float distance = Vector3.Distance(transform.position, PlayerTransform.position);

            agent.SetDestination(PlayerTransform.position);

            if (distance <= AttackRange)
            {
                Vector3 lookPos = PlayerTransform.position - transform.position;
                lookPos.y = 0f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), 10 * Time.deltaTime);

                if (Time.time > LastAttackTime + AttackCooldown)
                {
                    Attack();
                    LastAttackTime = Time.time;
                }
            }
        }
    }

    void Attack()
    {
        Debug.Log($"{gameObject.name} attack!");
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            GameManager.instance.UpdateEnemyCount(-1);
            Die();
         
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrg;
    }

    

    public void Die()
    {
        if (isDead) return; // Prevent double-triggering

        isDead = true;

        gameObject.tag = "Body";

        gameObject.layer = LayerMask.NameToLayer("Body");

        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agent.isStopped = true;
            agent.enabled = false; // Stops them from moving
        }

        if (TryGetComponent<EnemyGuard>(out EnemyGuard ai))
        {
            ai.enabled = false; // Stops the AI brain
        }

        // will make Ragdoll function more indepth later
        HandleDeathPhysics();

    }

    void HandleDeathPhysics()
    {
        // Simple "Fall over"
        transform.Rotate(Vector3.right * 90);

    }

}