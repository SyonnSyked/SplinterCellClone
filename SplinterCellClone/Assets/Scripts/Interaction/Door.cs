using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;

    bool playerInTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger && Input.GetButtonDown("Interact") && CheckAnimationState("Closed"))
            doorAnimator.SetTrigger("DoorOpen");
        else if (playerInTrigger && Input.GetButtonDown("Interact") && CheckAnimationState("Open"))
            doorAnimator.SetTrigger("DoorClose");
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
