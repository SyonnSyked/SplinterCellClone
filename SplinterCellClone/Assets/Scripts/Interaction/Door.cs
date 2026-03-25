using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator doorAnimator;

    bool playerInTrigger;
    bool isDoorOpen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorAnimator = GetComponentInChildren<Animator>();
        isDoorOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger && Input.GetButtonDown("Interact"))
        {
            if (!isDoorOpen)
            {
                doorAnimator.SetTrigger("DoorOpen");
                isDoorOpen = true;
            }
            else
            {
                doorAnimator.SetTrigger("DoorClose");
                isDoorOpen = false;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

}
