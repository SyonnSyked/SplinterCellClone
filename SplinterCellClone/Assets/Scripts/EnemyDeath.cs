using UnityEngine;
using UnityEngine.AI;

public class EnemyDeath : MonoBehaviour
{
    public bool isDead = false;

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

        // 4. Physics/Ragdoll
        HandleDeathPhysics();

        Debug.Log(gameObject.name + " is now a BODY. Other guards can now detect it.");
    }

    void HandleDeathPhysics()
    {
        // Simple "Fall over"
        transform.Rotate(Vector3.right * 90);

    }
}

