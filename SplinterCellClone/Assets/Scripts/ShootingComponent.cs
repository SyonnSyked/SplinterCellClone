using UnityEngine;

public class ShootingComponent : MonoBehaviour
{
    [Header("----Components----")]
    [SerializeField] LayerMask ignoreLayer;

    [Header("----Guns----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    float shootTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && shootTimer >= shootRate)
        {
            Shoot();
        }
    }


    void Shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer)) ;
        {
            Debug.Log(hit.collider.name);

            iDamage dmg = hit.collider.GetComponent<iDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }
    }

}
