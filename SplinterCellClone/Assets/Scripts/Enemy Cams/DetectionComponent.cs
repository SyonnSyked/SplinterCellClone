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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            warningBanner.SetActive(true);

            //AlertCounter.instance.AddAlert();

            StartCoroutine(FlashScreen());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            warningBanner.SetActive(false);
        }
    }

    System.Collections.IEnumerator FlashScreen()
    {
        redFlash.alpha = 1f;
        yield return new WaitForSeconds(0.2f);
        redFlash.alpha = 0f;
    }
}
