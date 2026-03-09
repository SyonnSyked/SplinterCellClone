using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour, iDamage
{

    [Header("----Components----")]
    [SerializeField] Renderer model;

    Color colorOriginal;

    [Header("----Stats----")]
    [SerializeField] int HP;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOriginal = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        model.material.color = colorOriginal;
    }
}
