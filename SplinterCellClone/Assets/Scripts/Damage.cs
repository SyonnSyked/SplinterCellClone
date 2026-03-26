using System.Collections;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("----ScriptReferences----")]
    [SerializeField] EnemyAI lightEnemyScript;
    [SerializeField] EnemyGuard guardEnemyScript;
    [SerializeField] ShootingComponent playerShootingScript;
    enum damageType { bullet, stationary, DOT }
    enum entityType { light, guard, camera, player, environmental}

    [SerializeField] damageType dmgType;
    [SerializeField] entityType enType;
    [SerializeField] Rigidbody rb;

    [SerializeField] GunStats gunStats;
    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] ParticleSystem hitEffect;

    bool isDamaging;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightEnemyScript = GameManager.instance.lightEnemy.GetComponent<EnemyAI>();
        guardEnemyScript = GameManager.instance.guardEnemy.GetComponent<EnemyGuard>();

        if (dmgType == damageType.bullet)
        {
            switch (enType)
            {
                case entityType.light:
                    gunStats = lightEnemyScript.GetGunStats();
                    break;

                case entityType.guard:
                    gunStats = guardEnemyScript.GetGunStats();
                    break;
                case entityType.player:
                    playerShootingScript = GameManager.instance.playerShootingScript;
                    gunStats = playerShootingScript.GetGunStats();
                    break;
                case entityType.environmental:
                    gunStats = null;
                    break;
            }
            SetBulletStats();
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
        else if (dmgType == damageType.DOT)
        {
            if (enType == entityType.environmental)
            {
                lightEnemyScript = null;
                guardEnemyScript = null;
                playerShootingScript = null;
                return;
            }
        }
    }


    private void SetBulletStats()
    {
        damageAmount = gunStats.damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        iDamage dmg = other.GetComponent<iDamage>();
        if (dmg != null && dmgType == damageType.DOT)
        {
            dmg.TakeDamage(damageAmount);
        }

        if (dmg != null && dmgType == damageType.bullet)
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            dmg.TakeDamage(damageAmount);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        iDamage dmg = other.GetComponent<iDamage>();
        if (dmg != null && dmgType == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }


    IEnumerator damageOther(iDamage d)
    {
        isDamaging = true;

        d.TakeDamage(damageAmount);

        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
