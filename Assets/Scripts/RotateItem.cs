using UnityEngine;

public class RotateItem : MonoBehaviour
{
    public float rotationSpeed = 20f; // Speed of rotation

    void Update()
    {
        // Rotate the object around the X-axis
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}