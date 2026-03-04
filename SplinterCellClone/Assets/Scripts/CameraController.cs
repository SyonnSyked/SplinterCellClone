using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensitivity = 3;
    [SerializeField] int lockRotNegative = -90, lockRotPositive = 90;
    [SerializeField] bool invertY;

    public Transform orientation;

    float camRotX;
    float camRotY;

    [Header("Camera Shake")]
    [SerializeField] float shakeDamping = 1f;

    Coroutine shakeRoutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Time.timeScale == 0f)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;         
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime; 

        if (invertY)
        {
            camRotX += mouseY;
        }
        else
        {
            camRotX -= mouseY;
        }

        camRotY += mouseX;
        camRotX -= mouseY;

        camRotX = Mathf.Clamp(camRotX, lockRotNegative, lockRotPositive);
       // camRotY = Mathf.Clamp(camRotY, lockRotNegative, lockRotPositive);
        transform.rotation = Quaternion.Euler(camRotX, camRotY, 0);
        //orientation.rotation = Quaternion.Euler(0, camRotY, 0);
    }

    private void FixedUpdate()
    {
       // transform.rotation = Quaternion.Euler(camRotX, camRotY, 0);
        orientation.rotation = Quaternion.Euler(0, camRotY, 0);
    }

}
