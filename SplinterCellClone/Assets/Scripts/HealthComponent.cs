using UnityEngine;

public class HealthComponent : MonoBehavior 
{
   public int currentHP;
   public int maxHP;
   public float headshotModifier;


   public void TakeDamage();
   public void TakeDamage(float headshotMod);
}
