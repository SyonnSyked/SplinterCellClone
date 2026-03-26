using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] public AmmoStats ammoStats;

    private void OnTriggerEnter(Collider other)
    {
        iPickup pickup = other.GetComponent<iPickup>();

        if (pickup != null)
        {
            pickup.AddAmmoToBag(ammoStats);
            Destroy(gameObject);
        }
    }
}
