using UnityEngine;
using UnityEngine.UI;

public class HitMarkerSystem : MonoBehaviour
{

    public static HitMarkerSystem instance;

    [SerializeField] Image hitMarker;
    [SerializeField] float hitMarkerTime = 0.2f;

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


    public void ShowHitMarker()
    {
        hitMarker.enabled = true;
        timer = hitMarkerTime;
    }

}