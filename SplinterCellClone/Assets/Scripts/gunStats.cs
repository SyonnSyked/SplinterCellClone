using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;

    [Range (1, 3)]public int bulletType;

    [Range(1, 10)] public int shootDamage;

    [Range (5, 300)] public int shootDist;

    [Range(0.1f, 2f)] public float shootRate;

    public int ammoCur;

    [Range(6, 24)] public int ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;

    [Range(0, 5)] public float shootSoundVol;

}
