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
        if (playerInTrigger)
        {
            if (!isDoorOpen && Input.GetButtonDown("Interact") && CheckAnimationState("Closed"))
            {
                doorAnimator.SetTrigger("DoorOpen");
                isDoorOpen = true;
            }
            else if (isDoorOpen && Input.GetButtonDown("Interact") && CheckAnimationState("Open"))
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
    private bool CheckAnimationState(string animation)
    {
        AnimatorStateInfo stateInfo = doorAnimator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.IsName(animation);
    }
}
