using UnityEngine;
using UnityEngine.InputSystem;

public class MenuCameraController : MonoBehaviour
{
    [SerializeField] InputActionReference LookAction;

    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float maxPitchX = 90f;
    [SerializeField] private float maxPitchY = 90f;

    float xRotation = 0f;
    float yRotation = 0f;
    void Start()
    {
        
    }

    void Update()
    {
        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        Vector2 lookVector = LookAction.action.ReadValue<Vector2>();

        float mouseX = lookVector.x * mouseSensitivity;
        float mouseY = lookVector.y * mouseSensitivity;

        xRotation -= Mathf.Clamp(xRotation, -maxPitchX, maxPitchX);
        yRotation = Mathf.Clamp(yRotation, -maxPitchY, maxPitchY);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
//