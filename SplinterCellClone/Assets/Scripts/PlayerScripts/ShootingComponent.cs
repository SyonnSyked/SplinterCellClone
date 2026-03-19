using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class ShootingComponent : MonoBehaviour, iPickup
{
    [Header("----Components----")]
    [SerializeField] LayerMask ignoreLayer;

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

        Instantiate(bullet, shootPos.position, gunPivot.transform.rotation);
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
    }

}
