using UnityEngine;

public interface iPickup  
{
    public void AddItemToBag(GameObject obj);

    public void AddGunToList(GunStats gunStats);

    public void AddAmmoToBag(AmmoStats ammo);
}
