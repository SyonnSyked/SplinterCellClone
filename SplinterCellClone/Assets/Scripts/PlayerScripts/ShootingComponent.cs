using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class ShootingComponent : MonoBehaviour, iPickup
{
    [Header("----Components----")]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] PlayerInputComponent playerInput;

    [Header("----Guns----")]
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform gunPivot;

    [Header("----CameraData----")]
    [SerializeField] Transform cameraPosition;

    float shootTimer;

    bool isAutomatic;

    int gunListPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        gunList[gunListPos].currentAmmo--;
        Instantiate(bullet, shootPos.forward, gunPivot.transform.rotation);
    }


    public void GetGunStats(GunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        ChangeGun();
    }

    void ChangeGun()
    { 
        shootDamage = gunList[gunListPos].damage;
        shootRate = gunList[gunListPos].rateOfFire;
        shootDistance = gunList[gunListPos].range;
        isAutomatic = gunList[gunListPos].isAutomatic;

        


        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        shootPos.position = gunList[gunListPos].gunModel.GetComponentInChildren<Transform>().position;

    }

    void SelectGun()
    {
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
