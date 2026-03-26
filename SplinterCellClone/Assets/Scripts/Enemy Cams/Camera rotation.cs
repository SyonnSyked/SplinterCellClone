using UnityEngine;

public class SecurityCameraRotation : MonoBehaviour
{
    public float rotationSpeed = 2f;
    public float maxAngle = 45f;

    private float currentAngle = 0f;
    private int direction = 1;

    // Update is called once per frame
    void Update()
    {
        float rotation = rotationSpeed * direction * Time.deltaTime;
        transform.Rotate(0, rotation, 0);

        currentAngle += rotation;

        if (Mathf.Abs(currentAngle) >= maxAngle)
        {
            direction *= -1;
        }
    }
}
