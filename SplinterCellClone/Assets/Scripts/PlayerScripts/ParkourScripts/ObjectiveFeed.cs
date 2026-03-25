using UnityEngine;
using TMPro;

public class objectiveFeed : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI feedText;

    [SerializeField] float showTime = 2f;
    [SerializeField] float fadeSpeed = 2f;

    int totalEnemies;
    int enemiesKilled;

    float timer;
    float currentAlpha;

    bool fadingOut;


    void Update()
    {
        if (currentAlpha > 0 && fadingOut)
        {
            currentAlpha -= fadeSpeed * Time.deltaTime;

            if (currentAlpha < 0)
                currentAlpha = 0;

            Color color = feedText.color;
            color.a = currentAlpha;
            feedText.color = color;
        }

        if (!fadingOut && currentAlpha > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
                fadingOut = true;
        }
    }


    public void setTotalEnemies(int amount)
    {
        totalEnemies = amount;
    }


    public void enemyKilled()
    {
        enemiesKilled++;

        int quarter = totalEnemies / 4;

        if (enemiesKilled == quarter)
            showMessage("1/4 enemies eliminated");

        else if (enemiesKilled == quarter * 2)
            showMessage("halfway there");

        else if (enemiesKilled == quarter * 3)
            showMessage("3/4 enemies eliminated");

        else if (enemiesKilled >= totalEnemies)
            showMessage("all enemies eliminated. grab the intel");
    }


    public void intelGrabbed()
    {
        showMessage("intel secured. head to the exit");
    }


    void showMessage(string message)
    {
        feedText.text = message;

        currentAlpha = 1f;
        timer = showTime;
        fadingOut = false;

        Color color = feedText.color;
        color.a = currentAlpha;
        feedText.color = color;
    }

}
