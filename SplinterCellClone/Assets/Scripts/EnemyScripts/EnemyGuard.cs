using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum AIState { Patrol, Investigate, SearchForMissing, HighAlert }

public class EnemyGuard : MonoBehaviour, iDamage
{
    [Header("Movement Settings")]
    [SerializeField] float detectionRange;
    [SerializeField] float patrolSpeed;
    [SerializeField] float patrolStopDistance;
    [SerializeField] float alertSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float investigateWaitTime;

    public List<GameObject> patrolWaypoints = new List<GameObject>();
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private bool hasLastKnownTargetPosition;
    private Vector3 lastKnownTargetPosition;



    [Header("Stealth Logic")]
    public AIState currentState = AIState.Patrol;
    public float suspicionLevel = 0f;
    public float buddyCheckTimer = 0f;
    public const float BUDDY_MISSING_THRESHOLD = 30f;

    [Header("Combat Settings")]
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] GunStats equippedGun;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform gunPivot;
    float shootTimer;

    [SerializeField] int HP;
    public float AttackRange;
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

        if (agent != null)
        {
            agent.speed = patrolSpeed;
            agent.stoppingDistance = AttackRange * 0.9f;
        }

        if (model != null)
        {
            colorOrg = model.material.color;
        }

        if (!allGuards.Contains(this))
        { 
            allGuards.Add(this);
        }
    }

    void Start()
    {
        GameManager.instance.UpdateEnemyCount(1);

        if (!InitializePatrolPoints())
        {
            Debug.LogWarning($"{name}: was unable to initialize patrol points list");
        }

        InitializePatrolPoints();
        
        FindPlayer();
    }

    void OnDisable()
    {
        allGuards.Remove(this);
    }

    void OnDestroy()
    {
        allGuards.Remove(this);
    }

    void Update()
    {
        if (isDead || agent == null || !agent.enabled)
            return;

        if (PlayerTransform == null)
        {
            FindPlayer();
        }

        switch (currentState)
        {
            case AIState.Patrol:
                UpdatePatrol();
                CheckForMissingPatrols();
                CheckForMissingPatrols();
                break;

            case AIState.Investigate:
                UpdateInvestigate();
                if (TryDetectPlayer())
                    AttackPlayer();
                break;

            case AIState.SearchForMissing:
                UpdateSearchForMissing();
                break;

            case AIState.HighAlert:
                UpdateHighAlert();
                break;
        }
    }

    bool InitializePatrolPoints()
    {
        patrolWaypoints.Clear();

        if (GameManager.instance == null || GameManager.instance.patrolWaypoints == null)
        {
            return false;
        }

        foreach (GameObject patrolPoint in GameManager.instance.patrolWaypoints)
        {
            if (patrolPoint != null)
            {
                patrolWaypoints.Add(patrolPoint);
            }

        }

        return patrolWaypoints.Count > 0;
    }


    void FindPlayer()
    {
        if (GameManager.instance.player != null && GameManager.instance.player != null)
        {
            PlayerTransform = GameManager.instance.player.transform;
            return;
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        { 
            PlayerTransform = playerObj.transform;
        }
    }


    bool TryDetectPlayer()
    {
        if (PlayerTransform == null)
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, PlayerTransform.position);

        if (distance <= detectionRange && currentState == AIState.HighAlert)
        {
            hasLastKnownTargetPosition = true;
            lastKnownTargetPosition = PlayerTransform.position;
            return true;
        }
        else
            return false;
    }

    public GunStats GetGunStats()
    {
        return equippedGun;
    }

    public void OnHearNoise(Vector3 location)
    {
        if (isDead) return;

        // Check memory: Is this the 3rd time we heard a noise at this exact spot
        int repeatedCount = distractionHistory.FindAll(pos => Vector3.Distance(pos, location) < 1.0f).Count;

        if (repeatedCount < 3)
        {
            distractionHistory.Add(location);
            hasLastKnownTargetPosition = true;
            currentState = AIState.Investigate;
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
        if (isDead) return;

        if (body != null)
        {
            currentState = AIState.HighAlert;
            hasLastKnownTargetPosition = true;
            lastKnownTargetPosition = body.transform.position;
            RadioAllies("Man down at- ", body.transform.position);
        }
    }

    void UpdatePatrol()
    {
        if (patrolWaypoints == null || patrolWaypoints.Count == 0) return;

        agent.speed = patrolSpeed;
        agent.isStopped = false;
        agent.SetDestination(patrolWaypoints[currentWaypointIndex].transform.position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Count;
            buddyCheckTimer = 0f; // Reset timer when reaching a waypoint (active patrol)
        }
    }

    void CheckForMissingPatrols()
    {
        buddyCheckTimer += Time.deltaTime;
        if (buddyCheckTimer > BUDDY_MISSING_THRESHOLD)
        {
            currentState = AIState.SearchForMissing;
            hasLastKnownTargetPosition = true;
            lastKnownTargetPosition = transform.position;
            RadioAllies("There's a post not reporting in. I'm checking it out.", transform.position);
        }
    }

    void UpdateSearchForMissing()
    {
        agent.speed = patrolSpeed;
        agent.isStopped = false;

        if (hasLastKnownTargetPosition)
        {
            agent.SetDestination(lastKnownTargetPosition);

            if (!agent.pathPending && agent.remainingDistance <= patrolStopDistance)
            {
                hasLastKnownTargetPosition = false;
                currentState = AIState.Patrol;
                buddyCheckTimer = 0f;
            }
        }
        else 
        {
            currentState = AIState.Patrol;
            buddyCheckTimer = 0f;
        }
    }

    void UpdateHighAlert()
    {
        if (PlayerTransform == null)
        {
            currentState = AIState.Patrol;
            return;
        }

        agent.speed = alertSpeed;
        agent.isStopped = false;

        float distance = Vector3.Distance(transform.position, PlayerTransform.position);

        hasLastKnownTargetPosition = true;
        lastKnownTargetPosition = PlayerTransform.position;

        if (distance > detectionRange * 1.5f)
        {
            currentState = AIState.Investigate;
            return;
        }

        agent.SetDestination(PlayerTransform.position);
        FaceTarget(PlayerTransform.transform.position);

        if (distance <= AttackRange && Time.time >= LastAttackTime + AttackCooldown)
        {
            AttackPlayer();
            LastAttackTime = Time.time;
        }
    }

    void FaceTarget(Vector3 targetPosition)
    { 
        Vector3 lookPos = targetPosition - transform.position;
        lookPos.y = 0f;

        if (lookPos.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    IEnumerator InvestigateLocation(Vector3 location)
    {
        currentState = AIState.Investigate;
        agent.speed = patrolSpeed;
        agent.isStopped = false;
        agent.SetDestination(location);

        while (!isDead && agent.enabled && (agent.pathPending || agent.remainingDistance > patrolStopDistance))
        {
            TryDetectPlayer();
            if (currentState == AIState.HighAlert)
                yield break;

            yield return null;
        }

        float timer = 0f;
        while (timer < investigateWaitTime)
        {
            TryDetectPlayer();
            if (currentState == AIState.HighAlert)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        currentState = AIState.Patrol;
    }

    void UpdateInvestigate()
    { 
        agent.speed = patrolSpeed;
        agent.isStopped = false;

        if (!hasLastKnownTargetPosition)
        {
            currentState = AIState.Patrol;
            return;
        }


        if (!agent.pathPending && agent.remainingDistance <= patrolStopDistance)
        {
            hasLastKnownTargetPosition = false;
            StartCoroutine(InvestigateLocation(lastKnownTargetPosition));
        }

        
    }

    public void OnSeePlayer(Transform player)
    {
        if (isDead || player == null)
            return;

        PlayerTransform = player;
        currentState = AIState.HighAlert;
        hasLastKnownTargetPosition = true;
        lastKnownTargetPosition = player.position;
    }

    public void RadioAllies(string message, Vector3 alertPos)
    {
        Debug.Log($"[RADIO] {message}");
        foreach (EnemyGuard guard in allGuards)
        {
            if (guard != null && guard != this && !guard.isDead)
            {
                // Alert nearby guards to the same location
                guard.ReceiveRadioAlert(alertPos);
            }
        }
    }

    public void ReceiveRadioAlert(Vector3 alertPos)
    {
        if (isDead) return;

        if (currentState != AIState.HighAlert)
        {
            currentState = AIState.Investigate;
            hasLastKnownTargetPosition = true;
            lastKnownTargetPosition = alertPos;
            agent.isStopped = false;
            agent.SetDestination(alertPos);
        }
    }

    void AttackPlayer()
    {
        if (PlayerTransform == null)
        {
            PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

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

    void Shoot()
    {
        if (bullet == null || shootPos == null)
        {
            Debug.LogWarning($"{name}: Missing bullet prefab or shootPos");
            return;
        }

        Instantiate(bullet, shootPos.position, gunPivot.transform.rotation);
    }

    void Attack()
    {
        Debug.Log($"{gameObject.name} attack!");
        Shoot();
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        HP -= amount;

        if (hitEffect != null)
        {
            Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);
        }


        if (PlayerTransform == null)
        {
            FindPlayer();
        }

        if (PlayerTransform != null)
        {
            currentState = AIState.HighAlert;
            hasLastKnownTargetPosition = true;
            lastKnownTargetPosition = PlayerTransform.position;

            if (agent != null && agent.enabled)
            {
                agent.isStopped = false;
                agent.SetDestination(PlayerTransform.position);
            }
        }

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
        if (model == null)
            yield break;

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

        StopAllCoroutines();

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        enabled = false;

        // will make Ragdoll function more indepth later
        HandleDeathPhysics();

    }

    void HandleDeathPhysics()
    {
        // Simple "Fall over"
        transform.Rotate(Vector3.right * 90);

    }

}