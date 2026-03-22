using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { 

        }
    }
}
