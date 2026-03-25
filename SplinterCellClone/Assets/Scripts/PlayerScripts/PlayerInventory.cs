using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour, iPickup
{
    [SerializeField] InventoryComponent inventoryComponent;

    public int gunListPos;
    public int itemListPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public InventoryComponent GetPlayerInventory()
    {
        return inventoryComponent;
    }

    private void InitializeInventory()
    {
        inventoryComponent.Inventory.Add("ItemList", new List<GameObject>());
        itemListPos = 0;

        inventoryComponent.EquippedGuns.Add("GunStats", new List<GunStats>());
        gunListPos = 0;
    }

    public void AddItemToBag(GameObject obj)
    {
        if (obj != null)
        {
            inventoryComponent.Inventory["ItemList"].Add(obj);
            itemListPos = 1;
        }
    }

    public void AddGunToList(GunStats gunStats)
    {
        inventoryComponent.EquippedGuns["GunStats"].Add(gunStats);
        gunListPos = 1;
    }
}
