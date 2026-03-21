using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] GunStats gun;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pickup Collider detected");
        iPickup pickup = other.GetComponent<iPickup>();
        if (pickup != null)

        {

            gun.currentAmmo = gun.maxAmmo;

            pickup.GetGunStats(gun);
            Destroy(gameObject);
        }
    }
}
