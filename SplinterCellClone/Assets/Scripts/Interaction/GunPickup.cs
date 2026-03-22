using UnityEngine;

public class GunPickup : MonoBehaviour, iGun
{
    [SerializeField] GunStats gun;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pickup Collider detected");
        iPickup pickup = other.GetComponent<iPickup>();
        if (pickup != null)

        {

            gun.currentAmmo = gun.maxAmmo;

            pickup.AddItemToBag(gun.gunModel);
            pickup.AddGunToList(gun);
            Destroy(gameObject);
        }
    }
}
