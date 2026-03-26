using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    [Header("----Stats----")]
    public int damage;
    public int range;
    public float rateOfFire;
    public bool isAutomatic;

    public AmmoStats.AmmoType ammoType;
    public int currentAmmo;
    public int maxAmmo;

    

    [Header("----Visual----")]
    public GameObject gunModel;
    public Transform shootPos;
    public AudioClip[] audioClips;
    [Range(0, 1)] public float audioVolume;


    public bool IsAutomatic()
    {
        return isAutomatic;
    }
}
