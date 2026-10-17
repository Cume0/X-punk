using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class FirstPersonCameraController : MonoBehaviour
{
    [SerializeField] InputActionReference LookAction;
    [SerializeField] InputActionReference ShootAction;
    [SerializeField] InputActionReference AimAction;
    
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float maxPitch = 90f;

    GameEvents gameEvents;

    public Transform playerBody;
    float xRotation = 0f;

    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
    }

    void LateUpdate()
    {
        HandleMouseLook();
    }

    
    private void HandleMouseLook()
    {
        Vector2 lookVector = LookAction.action.ReadValue<Vector2>();

        float mouseX = lookVector.x * mouseSensitivity;
        float mouseY = lookVector.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity; 

        LookAction.action?.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        if (playerBody != null && playerBody.TryGetComponent(out NavMeshAgent agent))
        {
            agent.updateRotation = false;
        }
    }

    private void OnDisable()
    {
        LookAction.action?.Disable();
        Cursor.lockState = CursorLockMode.None;
        if (playerBody != null && playerBody.TryGetComponent(out NavMeshAgent agent))
        {
            agent.updateRotation = true;
        }
    }
}
