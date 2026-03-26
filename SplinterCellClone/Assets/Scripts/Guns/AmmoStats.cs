using UnityEngine;

[CreateAssetMenu]
public class AmmoStats : ScriptableObject
{

    public enum AmmoType { light, medium, heavy};

    [Header("----Stats----")]
    public int ammoStackCount;
    public AmmoType ammoType;

    [Header("----Visual----")]
    public GameObject ammoModel;
}
