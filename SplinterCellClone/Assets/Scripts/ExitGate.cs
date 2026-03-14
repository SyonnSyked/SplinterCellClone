using UnityEngine;

public class ExitGate : MonoBehaviour
{
    bool playerInExit;

    bool enemyKillsMet;
    bool briefcaseMet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GameManager.instance.CheckWin();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInExit = false;
    }


    bool CheckWinCondition()
    {
        if (GameManager.instance.enemyCount <= 0)
            enemyKillsMet = true;

        if (GameManager.instance.briefcaseCount <= 0)
            briefcaseMet = true;

        if (enemyKillsMet && briefcaseMet)
            return true;

        return false;
    }
} 