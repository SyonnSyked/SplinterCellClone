using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform camHolder;

    [SerializeField] float camAdjustSpeed;

    float xRotation;
    float yRotation;

    float camSpeed;

    float camFOV;


    [SerializeField] Quaternion tiltTargetRotation;

    

    // Coroutine to smoothly rotate the camera
    IEnumerator RotateCamera(Quaternion target, float time)
    {
        Quaternion startRotation = transform.rotation;
        float elapsed = 0f;

        while (elapsed < time)
        {
            // Slerp from the start rotation to the target rotation over time
            transform.rotation = Quaternion.Slerp(startRotation, target, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure the final rotation is exactly the target rotation
        transform.rotation = target;
    }



    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    private void FixedUpdate()
    {
        
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        camSpeed = (Vector3.Distance(orientation.position, camHolder.position));
    }

    public float GetCameraSpeed()
    {
        return camSpeed;
    }

    public void DoFov(float endValue)
    {
        camFOV = GetComponent<Camera>().fieldOfView;

        camFOV = Mathf.Lerp(camFOV, endValue, Time.deltaTime * camAdjustSpeed);
    }

    public void DoTilt()
    {
        StartCoroutine(RotateCamera(tiltTargetRotation, camAdjustSpeed));
    }
}