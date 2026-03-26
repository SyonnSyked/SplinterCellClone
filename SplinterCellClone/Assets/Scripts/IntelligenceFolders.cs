using UnityEngine;

public class IntelligenceFolders : MonoBehaviour
{

    private bool playerNear;


    void Start()
    {
        GameManager.instance.UpdateBCount(1);
    }



    void Update()
    {
        if (playerNear == true && Input.GetButtonDown("Interact"))
        {
            Collect();
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerNear = true;
            GameManager.instance.showInteractPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerNear = false;
            GameManager.instance.showInteractPrompt(false);
        }
    }

    private void Collect()
    {
        GameManager.instance.UpdateBCount(-1);
        GameManager.instance.showInteractPrompt(false);
        Destroy(gameObject);

    }
}
