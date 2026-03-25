using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour, iPickup
{
    public List<GameObject> playerInv = new List<GameObject>();
    public List<GunStats> playerGuns = new List<GunStats>();

    public int gunListPos;
    public int itemListPos;

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
}
