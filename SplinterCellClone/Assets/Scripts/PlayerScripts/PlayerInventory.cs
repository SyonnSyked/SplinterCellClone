using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour, iPickup
{
    [SerializeField] InventoryComponent inventoryComponent;

    int gunListPos;
    int itemListPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeInventory()
    {
        inventoryComponent.Inventory.Add("GunList", new List<GameObject>());
        inventoryComponent.Inventory.Add("ItemList", new List<GameObject>());
    }

    public void AddItemToBag(GameObject obj)
    {
        if (obj != null)
        {
            if (obj.CompareTag("Gun"))
            {
                inventoryComponent.Inventory["GunList"].Add(obj);
                gunListPos++;
            }
            else if (obj.CompareTag("Item"))
            {
                inventoryComponent.Inventory["ItemList"].Add(obj);
                itemListPos++; 
            }
        }
    }


}
