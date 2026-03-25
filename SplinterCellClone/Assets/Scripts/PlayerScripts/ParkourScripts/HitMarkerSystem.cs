using UnityEngine;
using UnityEngine.UI;

public class hitMarkerSystem : MonoBehaviour
{

    public static hitMarkerSystem instance;

    [SerializeField] Image hitMarker;

    [SerializeField] float hitMarkerTime = 0.2f;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip headshotSound;

    float timer;


    void Awake()
    {
        instance = this;
    }


    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            hitMarker.enabled = false;
        }
    }


    public void showHitMarker(bool isHeadshot)
    {
        hitMarker.enabled = true;
        timer = hitMarkerTime;

        // color change
        if (isHeadshot)
            hitMarker.color = Color.red;
        else
            hitMarker.color = Color.white;

        // sound
        if (isHeadshot && headshotSound != null)
            audioSource.PlayOneShot(headshotSound);
        else if (hitSound != null)
            audioSource.PlayOneShot(hitSound);
    }

}