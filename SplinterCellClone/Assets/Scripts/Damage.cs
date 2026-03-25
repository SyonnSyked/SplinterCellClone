using System.Collections;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("----ScriptReferences----")]
    [SerializeField] EnemyAI lightEnemyScript;
    [SerializeField] EnemyGuard guardEnemyScript;
    enum damageType { bullet, stationary, DOT }
    enum enemyType { light, guard, camera}

    [SerializeField] damageType dmgType;
    [SerializeField] enemyType enType;
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
        if (dmgType == damageType.bullet)
        {
            if (enType == enemyType.light)
            {
                gunStats = lightEnemyScript.GetGunStats();
            }
            else if (enType == enemyType.guard)
            {
                gunStats = guardEnemyScript.GetGunStats();
            }
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
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
