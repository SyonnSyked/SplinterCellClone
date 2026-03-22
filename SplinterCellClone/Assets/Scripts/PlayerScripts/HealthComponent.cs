using UnityEngine;

public class HealthComponent : MonoBehaviour, iDamage
{

    [Header("----Components----")]
    [SerializeField] ParticleSystem hitEffect;

    [Header("----Stats----")]
    [Range(0, 100)][SerializeField] int HP;
    [Range(0, 100)][SerializeField] int maxHP;

    int HPOriginal;

   

    void Start()
    {
        HPOriginal = maxHP;
        UpdatePlayerHealth();
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;

        UpdatePlayerHealth();

        if (hitEffect != null)
        {
            Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);
        }

        if (HP <= 0)
        {
            GameManager.instance.LoseScreen();
        }

    }

    public void UpdatePlayerHealth()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
    }
}
