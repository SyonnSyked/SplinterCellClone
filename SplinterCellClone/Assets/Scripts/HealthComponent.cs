using UnityEditor.Build.Content;
using UnityEngine;

public class HealthComponent : MonoBehaviour, iDamage
{
    

    [Header("----Stats----")]
    [Range(0, 100)][SerializeField] int HP;


    


    int HPOriginal;

   

    void Start()
    {
        HPOriginal = HP;
        UpdatePlayerHealth();
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;

        UpdatePlayerHealth();

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
