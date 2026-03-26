using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
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

    int reserveAmmo;

    [Header("----CameraData----")]
    [SerializeField] Transform cameraPosition;

    [Header("----Audio----")]
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] List<AudioClip> shootClips = new List<AudioClip>();
    [SerializeField] float audioShotVol;

    float shootTimer;

    float reloadTimer;

    bool isAutomatic;

    bool isPlayingShooting;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInventory.gunListPos = 0;
        ChangeGun();
        GameManager.instance.updateAmmoCurr();
        GameManager.instance.updateAmmoReserve();
        InitializeGunSounds();
    }

    // Update is called once per frame
    void Update()
    {
        gunPivot.transform.localRotation = cameraPosition.localRotation;
        Reload();


        if (!GameManager.instance.isPaused)
        { 
            SelectGun();

            if (isAutomatic)
            {

                if (Input.GetButton("Fire1") && reloadTimer <= 0 && equippedGun.currentAmmo > 0 && shootTimer >= shootRate)
                {
                    Shoot();
                }
            }

            if (Input.GetButtonDown("Fire1") && reloadTimer <= 0 && shootTimer >= shootRate && equippedGun.currentAmmo > 0 && playerInventory.playerGuns.Count > 0)
            {
                Shoot();
            }

            shootTimer += Time.deltaTime;
            if (reloadTimer > 0)
            {
                reloadTimer -= Time.deltaTime;
            }
        }
    }

    private void InitializeGunSounds()
    {
        foreach (AudioClip clip in equippedGun.audioClips)
        {
            shootClips.Add(clip);
        }
    }

    public int GetReserveAmmoCount()
    {
        switch (equippedGun.ammoType)
        {
            case AmmoStats.AmmoType.light:
                reserveAmmo = playerInventory.GetSmallAmmoCount();
                break;
            case AmmoStats.AmmoType.medium:
                reserveAmmo = playerInventory.GetMediumAmmoCount();
                break;
            case AmmoStats.AmmoType.heavy:
                reserveAmmo = playerInventory.GetHeavyAmmoCount();
                break;
        }

        return reserveAmmo;
    }


    public GunStats GetGunStats()
    {
        return equippedGun;
    }
    void Shoot()
    {
        StartCoroutine(PlayShootSound(shootClips, shootTimer + 0.5f));

        shootTimer = 0;
        Instantiate(bullet, shootPos.position, gunPivot.transform.rotation);
        equippedGun.currentAmmo--;
        GameManager.instance.updateAmmoCurr();
    }


    private IEnumerator PlayShootSound(List<AudioClip> clips, float interval)
    {
        isPlayingShooting = true;

        // Play a random step sound
        if (clips != null && clips.Count > 0)
            audioPlayer.PlayOneShot(shootClips[Random.Range(0, shootClips.Count)], audioShotVol);

        yield return new WaitForSeconds(interval);

        isPlayingShooting = false;
    }

    void ChangeGun()
    {
        equippedGun = playerInventory.playerGuns[playerInventory.gunListPos];
        shootDamage = equippedGun.damage;
        shootRate = equippedGun.rateOfFire;
        shootDistance = equippedGun.range;
        isAutomatic = equippedGun.isAutomatic;

        GameManager.instance.updateAmmoReserve();
        GameManager.instance.updateAmmoCurr();

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


    void Reload()
    {
        if(playerInput.reload.action.IsPressed())
        {
            int reloadAmount = equippedGun.maxAmmo - equippedGun.currentAmmo;
            if (reloadAmount > 0)
            {
                equippedGun.currentAmmo += reloadAmount;
                reloadTimer = 1;
            }
        }
    }
}
