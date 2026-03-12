using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum AIState { Patrol, Investigate, SearchForMissing, HighAlert }

public class EnemyGuard : MonoBehaviour
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
                // Logic handled by OnHearNoise coroutine
                break;
            case AIState.HighAlert:
                agent.speed = 5.0f; // Run to player or body
                break;
        }
    }

    public void OnHearNoise(Vector3 location)
    {
        // Check memory: Is this the 3rd time we heard a noise at this exact spot?
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
}
