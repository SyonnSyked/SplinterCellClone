using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour, iPickup
{
    public List<GameObject> playerInv = new List<GameObject>();
    public List<GunStats> playerGuns = new List<GunStats>();

    public int gunListPos;
    public int itemListPos;

    [Range(0, 1000)] private int bigAmmo;
    [Range(0, 1000)] private int medAmmo;
    [Range(0, 1000)] private int smallAmmo;


    public int GetSmallAmmoCount()
    {
        return smallAmmo;
    }

    public int GetMediumAmmoCount()
    {
        return medAmmo;
    }
    public int GetHeavyAmmoCount()
    {
        return bigAmmo;
    }

   

    public void AddItemToBag(GameObject item)
    { 
        playerInv.Add(item);
        itemListPos++;
    }

    public void AddGunToList(GunStats gun)
    { 
        playerGuns.Add(gun);
        gunListPos++;
    }


    public void AddAmmoToBag(AmmoStats ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoStats.AmmoType.light:
                smallAmmo += ammo.ammoStackCount;
                break;
            case AmmoStats.AmmoType.medium:
                medAmmo += ammo.ammoStackCount;
                break;
            case AmmoStats.AmmoType.heavy:
                bigAmmo += ammo.ammoStackCount;
                break;
        }
    }
}
