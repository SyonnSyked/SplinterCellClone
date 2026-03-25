using UnityEngine;

public class GunPickup : MonoBehaviour, iGun
{
    [SerializeField] public GunStats gun;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pickup Collider detected");
        Debug.Log(other);
        Debug.Log(other.gameObject);
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
