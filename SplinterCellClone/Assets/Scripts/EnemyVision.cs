using UnityEngine;

public class GuardVision : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewDistance = 15f;
    [Range(0, 180)] public float viewAngle = 90f;
    public LayerMask targetMask; // Set to "Player" and "Body" layers
    public LayerMask obstructionMask; // Set to "Default/Level" layers

    private EnemyGuard AIController;

    void Start()
    {
        AIController = GetComponentInParent<EnemyGuard>();
    }

    void Update()
    {
        FindVisibleTargets();
    }

    void FindVisibleTargets()
    {
        // Find potential targets in range
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // Check if target is within the FOV angle
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                // Raycast to check for wall obstructions
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstructionMask))
                {
                    IdentifyTarget(target.gameObject);
                }
            }
        }
    }

    void IdentifyTarget(GameObject obj)
    {
        if (obj.CompareTag("Body"))
        {
            AIController.OnSeeBody(obj);
        }
        else if (obj.CompareTag("Player"))
        {
            AIController.currentState = AIState.HighAlert;
            AIController.RadioAllies("Intruder spotted!");
        }
    }

    // the vision cone in the Unity Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 leftBoundary = Quaternion.AngleAxis(-viewAngle / 2, transform.up) * transform.forward;
        Vector3 rightBoundary = Quaternion.AngleAxis(viewAngle / 2, transform.up) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftBoundary * viewDistance);
        Gizmos.DrawRay(transform.position, rightBoundary * viewDistance);
    }
}
