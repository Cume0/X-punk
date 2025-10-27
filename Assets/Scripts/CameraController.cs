using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Vector3 trueMoveDirection;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private InputActionReference _move;

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        var moveDirection = _move.action.ReadValue<Vector2>();

        trueMoveDirection = new Vector3(moveDirection.x, 0, moveDirection.y);

        transform.Translate(trueMoveDirection * _moveSpeed * Time.deltaTime, Space.World);
    } 
}
