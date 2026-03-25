using UnityEngine;
using UnityEngine.UI;

public class damageFeedback : MonoBehaviour
{

    [SerializeField] Image damageImage;

    [SerializeField] float fadeSpeed = 2f;
    [SerializeField] float maxAlpha = 0.6f;

    [SerializeField] float hitScale = 1.1f;
    [SerializeField] float scaleSpeed = 5f;

    float currentAlpha;
    float currentScale = 1f;


    // Update is called once per frame
    void Update()
    {
        // fade out
        if (currentAlpha > 0)
        {
            currentAlpha -= fadeSpeed * Time.deltaTime;

            if (currentAlpha < 0)
                currentAlpha = 0;

            Color color = damageImage.color;
            color.a = currentAlpha;
            damageImage.color = color;
        }

        // scale back to normal
        if (currentScale > 1f)
        {
            currentScale -= scaleSpeed * Time.deltaTime;

            if (currentScale < 1f)
                currentScale = 1f;

            damageImage.rectTransform.localScale = new Vector3(currentScale, currentScale, 1f);
        }
    }


    public void showDamage(float amount)
    {
        currentAlpha += amount / 100f;

        if (currentAlpha > maxAlpha)
            currentAlpha = maxAlpha;

        // scale effect
        currentScale = hitScale;

        Color color = damageImage.color;
        color.a = currentAlpha;
        damageImage.color = color;

        damageImage.rectTransform.localScale = new Vector3(currentScale, currentScale, 1f);
    }

}