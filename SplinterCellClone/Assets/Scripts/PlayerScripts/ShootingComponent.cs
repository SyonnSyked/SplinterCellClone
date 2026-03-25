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



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInventory.gunListPos = 0;
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
        equippedGun = playerInventory.playerGuns[playerInventory.gunListPos];
        shootDamage = equippedGun.damage;
        shootRate = equippedGun.rateOfFire;
        shootDistance = equippedGun.range;
        isAutomatic = equippedGun.isAutomatic;

        gunModel.GetComponent<MeshFilter>().sharedMesh = playerInventory.playerInv[playerInventory.gunListPos].GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = playerInventory.playerInv[playerInventory.gunListPos].GetComponent<MeshRenderer>().sharedMaterial;
    }

    void SelectGun()
    {
        if (playerInventory.playerGuns.Count == 0 || playerInventory.playerInv.Count == 0)
            return;

        playerInventory.gunListPos = Mathf.Clamp(playerInventory.gunListPos, 0, playerInventory.playerGuns.Count - 1);

        ChangeGun();

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && playerInventory.gunListPos < playerInventory.playerGuns.Count - 1)
        {
            playerInventory.gunListPos++;
            ChangeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && playerInventory.gunListPos > 0)
        {
            playerInventory.gunListPos--;
            ChangeGun();
        }

        if (playerInput.weapon1.action.IsPressed())
        {
            playerInventory.gunListPos = 0;
            ChangeGun();
        }
        else if (playerInput.weapon2.action.IsPressed())
        {
            playerInventory.gunListPos = 1;
            ChangeGun();
        }
        else if (playerInput.weapon3.action.IsPressed())
        {
            playerInventory.gunListPos = 2;
            ChangeGun();
        }
    }

}
