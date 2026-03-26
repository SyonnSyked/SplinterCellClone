using UnityEngine;
using System.Collections;

public class EnemyTurret : MonoBehaviour
{
    [Header("----Components----")]
    [SerializeField] Renderer model;
    [SerializeField] ParticleSystem hitEffect;


    [Header("----Guns----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] GameObject gunModel;
    [SerializeField] GunStats equippedGun;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform gunPivot;


    float shootTimer;
    

    [Header("----AI Behavior Variables----")]
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int gunRotateSpeed;
    [SerializeField] int FOV;


    Color colorOriginal;
    bool playerInTrigger;
    float angleToPlayer;
    Vector3 playerDir;

    private void Start()
    {
        InitializeGun(); 
    }

    private void Update()
    {
        if (playerInTrigger && !CanSeePlayer())
            return;
        if (!playerInTrigger && !CanSeePlayer())
            return;

        if (playerInTrigger)
            CanSeePlayer();
    }

    private void InitializeGun()
    {
        shootDamage = equippedGun.damage;
        shootDistance = equippedGun.range;
        shootRate = equippedGun.rateOfFire;
    }



    bool CanSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {

                FaceTarget();

                GunRotate();

                shootTimer += Time.deltaTime;

                if (shootTimer >= shootRate)
                {
                    Shoot();
                }

                return true;
            }
        }

        return false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }


    void GunRotate()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, rot, Time.deltaTime * gunRotateSpeed);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

    void Shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, gunPivot.transform.rotation);
    }
    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (hitEffect != null)
        {
            Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);
        }


        AmDead(HP);
    }


    bool AmDead(int hp)
    {
        if (hp <= 0)
        {
            Destroy(gameObject);
            GameManager.instance.UpdateEnemyCount(-1);
            return true;
        }
        else
        {
            StartCoroutine(FlashRed());
            return false;
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        model.material.color = colorOriginal;
    }
}
