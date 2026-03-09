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
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            GameManager.instance.LoseScreen();
        }
    }
}
