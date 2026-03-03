using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("       Initialization        ")]
    [SerializeField] String PlayerTag;
    [SerializeField] String PlayerSpawnTag;
    



    [Header("       Menus        ")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;



    [Header("       Game State      ")]
    public bool isPaused;
    public TMP_Text gameGoalCountText;

    [Header("       Player Variable      ")]
    public GameObject player;
    public PlayerController playerScript;


    [Header("       Player UI      ")]
    [SerializeField] public GameObject FFASCORE;
    public GameObject DamageScreen;
    public GameObject HealScreen;
    public GameObject playerCompass;
    public Image playerCompassNeedle;
    public Image playerCrossHair;
    [SerializeField] Sprite DefaultCrossHairSprite;
    [SerializeField] Sprite DefaultWeaponSprite;

    public Image playerHitmarker;

    [Header("SFX")]

    [Header("       Player Weapon UI Elements      ")]
    public Image playerHPBar;
    public Image playerGun;
    public TMP_Text playerGunName;
    public TMP_Text playerAmmoCur;
    public TMP_Text playerAmmoReserve;



    int gameGoalCount;

    public static GameManager instance;
    void toggleCrosshair(bool val)
    {
        playerCrossHair.enabled = val;
    }
    
    public void playHitmarker()
    {
        StartCoroutine(HitmarkerRoutine());
        //Debug.Log("HitMarker");
    }

    IEnumerator HitmarkerRoutine()
    {
        audioSource2D.PlayOneShot(hitmarkerSound2D, hitmarkerSoundVolume);
        playerHitmarker.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        playerHitmarker.gameObject.SetActive(false);
    }

    public void updateGunUI(Sprite gunsprite, Sprite crosshairsprite, int ammo_reserve, int ammo_cur, string GunName)
    {

        // change crosshair
        playerGun.sprite = gunsprite;
        // change gun sprite
        playerCrossHair.sprite = crosshairsprite;
        playerCrossHair.rectTransform.sizeDelta = new Vector2(crosshairsprite.rect.width, crosshairsprite.rect.height);
        // change ammo reserve count
        updateAmmoUI(ammo_reserve, ammo_cur);
        // change gun name
        playerGunName.text = GunName;

    }
    
    public void ClearGunUI()
    {
      
        // change crosshair
        playerGun.sprite = DefaultWeaponSprite;
        // change gun sprite
        playerCrossHair.sprite = DefaultCrossHairSprite;
        // change ammo reserve count
        updateAmmoUI(0, 0);
        // change gun name
        playerGunName.text = "";

    }
    public void updateAmmoUI(int ammo_reserve, int ammo_cur)
    {   // change ammo reserve count
        playerAmmoReserve.text = ammo_reserve.ToString();
        // change ammo current
        playerAmmoCur.text = ammo_cur.ToString();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void Start()
    {

    }
    public void statePause()
    {
        toggleCrosshair(false);
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void stateUnPause()
    {
        toggleCrosshair(true);
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);

    }

    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);

    }
    public void updateGameGoal(int amount)
    {
        gameGoalCount = amount;
        updateScore();
       
    }
}

