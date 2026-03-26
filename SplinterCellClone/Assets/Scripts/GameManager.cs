using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject interactPrompt;
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text intelText;
    [SerializeField] TMP_Text ammoCurr;
    [SerializeField] TMP_Text ammoReserve;

    public ShootingComponent playerShootingScript;

    [SerializeField] public GameObject lightEnemy;
    [SerializeField] public GameObject guardEnemy;
    [SerializeField] public GameObject playerShootingRoot;

    public List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    public List<GameObject> patrolWaypoints = new List<GameObject>();



    [SerializeField] public GameObject checkpointPopup;
    public GameObject playerSpawner;
    
    public Image playerHPBar;
    public Image playerStamBar;

    public GameObject playerDamageFlash;



    public GameObject player;
    public bool isPaused;
    public int briefcaseCount;
    public int enemyCount;

    bool isPlayerAtExit;


    float timeScaleOriginal;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOriginal = Time.timeScale;

        GameManager.instance.playerSpawner = GameObject.FindWithTag("PlayerSpawner");
        foreach (EnemySpawner spawner in enemySpawners)
        {
            spawner.SpawnEnemy();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();

                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
            }
        }
    }



    public void StatePause()
    {
        isPaused = true;
        Time.timeScale = 0;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void StateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOriginal;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void OptionsScreen()
    {
        StatePause();
        menuActive = menuOptions;
        menuActive.SetActive(true);
    }

    public void QuitToMenu()
    { 

    }

    public void LoseScreen()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }


    public void UpdateEnemyCount(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");
    }

    public void UpdateBCount(int amount)
    {
        briefcaseCount += amount;
        intelText.text = briefcaseCount.ToString("F0");
    }

    public void CheckWin()
    {
        if (enemyCount <= 0 && briefcaseCount <= 0)
        {
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }


    public void RespawnPlayer()
    {
        GameManager.instance.player.transform.position = GameManager.instance.playerSpawner.transform.position;
    }



    public void showInteractPrompt(bool show)
    {
       /* menuActive = interactPrompt;
        interactPrompt.SetActive(show);
       */
    }


    public void updateAmmoCurr()
    {
        var inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.playerGuns.Count > 0)
        {
            var gun = inventory.playerGuns[inventory.gunListPos];
            ammoCurr.text = gun.currentAmmo.ToString();
        }
        else
        {
            ammoCurr.text = "0";
        }
    }

    public void updateAmmoReserve()
    {
        if (playerShootingRoot != null)
        {
            ammoReserve.text = playerShootingRoot.GetComponent<ShootingComponent>().GetReserveAmmoCount().ToString(); 
        }
        else
        {
            ammoReserve.text = "0";
        }
    }
}

