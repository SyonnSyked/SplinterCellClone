using UnityEngine;

public class SecurityCameraDetection : MonoBehaviour
{
    public GameObject warningBanner;
    public CanvasGroup redFlash;

    void start()
    {
        warningBanner.SetActive(false);
        redFlash.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
