using UnityEngine;
using UnityEngine.UI;

public class damageFeedback : MonoBehaviour
{

    [SerializeField] Image damageImage;

    [SerializeField] float fadeSpeed = 2f;
    [SerializeField] float maxAlpha = 0.6f;

    float currentAlpha;


    // Update is called once per frame
    void Update()
    {
        if (currentAlpha > 0)
        {
            currentAlpha -= fadeSpeed * Time.deltaTime;

            if (currentAlpha < 0)
                currentAlpha = 0;

            Color color = damageImage.color;
            color.a = currentAlpha;
            damageImage.color = color;
        }
    }

    //
    public void showDamage(float amount)
    {
        currentAlpha += amount / 100f;

        if (currentAlpha > maxAlpha)
            currentAlpha = maxAlpha;

        Color color = damageImage.color;
        color.a = currentAlpha;
        damageImage.color = color;
    }

}