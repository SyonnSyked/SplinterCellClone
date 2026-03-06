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
    [SerializeField] TMP_Text gameGoalText;
    [SerializeField] TMP_Text intelText;

    public GameObject playerDamageFlash;



    public GameObject player;
    public bool isPaused;
    public int briefcaseCount;


    float timeScaleOriginal;


    int gameGoalCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOriginal = Time.timeScale;

        player = GameObject.FindWithTag("Player");
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


    public void UpdateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalText.text = gameGoalCount.ToString("F0");

        CheckWin();
    }

    public void UpdateBCount(int amount)
    {
        briefcaseCount += amount;
        intelText.text = briefcaseCount.ToString("F0");
        CheckWin();
    }

    public void CheckWin()
    {
        if (gameGoalCount <= 0 && briefcaseCount <= 0)
        {
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }
}

