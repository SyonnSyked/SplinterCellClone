using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform cameraPosition;

    private void Update()
    {
        transform.position = cameraPosition.position;
        //transform.localRotation = cameraPosition.localRotation;
    }
}

