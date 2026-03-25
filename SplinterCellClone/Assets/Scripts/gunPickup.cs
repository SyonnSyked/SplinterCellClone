using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [SerializeField] gunStats gun;


    private void OnTriggerEnter(Collider other)
    {
        iPickup pik = other.GetComponent<iPickup>();

        if (pik != null)
        {
            gun.ammoCur = gun.ammoMax;
            pik.getGunStats(gun);
            Destroy(gameObject);
        }
    }


}
