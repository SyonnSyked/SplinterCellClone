using UnityEngine;

public class HealthComponent : MonoBehaviour, iDamage
{

    [Header("----Components----")]
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] AudioSource aud;

    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;

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
        PlayHurtSound();

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


    public void ResetHealth()
    {
        HPOriginal = maxHP;
        HP = HPOriginal;
        UpdatePlayerHealth();
    }



    public void PlayHurtSound()
    {
        if (audHurt != null && audHurt.Length > 0)
        {
            aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        }
    }

    public void UpdatePlayerHealth()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
    }

}
