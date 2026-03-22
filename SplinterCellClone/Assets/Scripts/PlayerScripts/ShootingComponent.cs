using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class ShootingComponent : MonoBehaviour 
{
    [Header("----Components----")]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] PlayerInputComponent playerInput;
    [SerializeField] PlayerInventory playerInventory;

    [Header("----Guns----")]
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] GameObject gunModel;
    [SerializeField] GunStats equippedGun;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform gunPivot;

    [Header("----CameraData----")]
    [SerializeField] Transform cameraPosition;

    [Header("----Audio----")]
    [SerializeField] AudioSource audioPlayer;

    float shootTimer;

    bool isAutomatic;

    int gunListPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       playerInventory.GetPlayerInventory().EquippedGuns.TryGetValue("GunStats", out gunList);
    }

    // Update is called once per frame
    void Update()
    {
        gunPivot.transform.localRotation = cameraPosition.localRotation;



        if (!GameManager.instance.isPaused)
        { 
            SelectGun();

            if (isAutomatic)
            {

                if (Input.GetButton("Fire1") && shootTimer >= shootRate)
                {
                    Shoot();
                }
            }

            if (Input.GetButtonDown("Fire1") && shootTimer >= shootRate)
            {
                Shoot();
            }

            shootTimer += Time.deltaTime;
        }
    }


    void Shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.forward, gunPivot.transform.rotation);
        //audioPlayer.PlayOneShot();
    }



    void ChangeGun()
    {
        gunListPos = playerInventory.gunListPos;

        equippedGun = gunList[gunListPos];
        shootDamage = equippedGun.damage;
        shootRate = equippedGun.rateOfFire;
        shootDistance = equippedGun.range;
        isAutomatic = equippedGun.isAutomatic;

        
        GameObject _gun = Instantiate(equippedGun.gunModel, gunModel.transform.position, Quaternion.identity);


        shootPos.position = _gun.GetComponentInChildren<Transform>().position;

    }

    void SelectGun()
    {
        ChangeGun();

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            ChangeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            ChangeGun();
        }

        if (playerInput.weapon1.action.IsPressed())
        {
            gunListPos = 0;
            ChangeGun();
        }
        else if (playerInput.weapon2.action.IsPressed())
        {
            gunListPos = 1;
            ChangeGun();
        }
        else if (playerInput.weapon3.action.IsPressed())
        {
            gunListPos = 2;
            ChangeGun();
        }
    }

}
