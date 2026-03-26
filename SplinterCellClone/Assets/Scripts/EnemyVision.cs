using UnityEngine;

public class GuardVision : MonoBehaviour
{
    public float viewDistance = 15f;
    [Range(0, 180)] public float viewAngle = 90f;
    public LayerMask targetMask;      // Should be "Player" and "Body"
    public LayerMask obstructionMask; // Should be "Default" or "Level"

    private EnemyGuard AIController;

    void Start()
    {
        AIController = GetComponentInParent<EnemyGuard>();
    }

    void Update()
    {
        if (AIController == null || AIController.isDead) return;
        FindVisibleTargets();
    }

    void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        foreach (var targetCollider in targetsInViewRadius)
        {
            Transform target = targetCollider.transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                // Raycast check
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstructionMask))
                {
                    // If we hit this line, the guard DEFINITELY sees the object
                    AIController.OnSeenObject(target.gameObject);
                }
            }
        }
    }



void IdentifyTarget(GameObject obj)
    {
        if (obj.CompareTag("Body"))
        {
            AIController.OnSeenObject(obj);
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
