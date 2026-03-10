using UnityEngine;

public class ShootingComponent : MonoBehaviour
{
    [Header("----Components----")]
    [SerializeField] LayerMask ignoreLayer;

    [Header("----Guns----")]
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    float shootTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && shootTimer >= shootRate)
        {
            Shoot();
        }
    }


    void Shoot()
    {
        shootTimer = 0;

        Instantiate(bullet, shootPos.position, transform.rotation);
    }

}
