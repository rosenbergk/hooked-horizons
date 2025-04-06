using UnityEngine;

public class RodAiming : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float verticalLimit = 45f;

    private float verticalRotation = 0f;

    void Update()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        // Horizontal rotation
        transform.Rotate(0f, horizontal, 0f);

        // Vertical rotation
        verticalRotation = Mathf.Clamp(verticalRotation + vertical, -verticalLimit, verticalLimit);
        Vector3 currentRotation = transform.localEulerAngles;
        currentRotation.x = verticalRotation;
        transform.localEulerAngles = currentRotation;
    }
}
