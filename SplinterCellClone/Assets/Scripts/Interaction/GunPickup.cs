using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] GunStats gun;
    private void OnTriggerEnter(Collider other)
    {
        iPickup pickup = other.GetComponent<iPickup>();

        if (pickup != null )
        {
            gun.currentAmmo = gun.maxAmmo;
            pickup.GetGunStats(gun);
        }
    }
}
