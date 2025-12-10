using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Vector3 trueMoveDirection;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _verticalOffset;
    [SerializeField] private InputActionReference _move;
    [SerializeField] private InputActionReference _scrollY;

    [SerializeField] private float rotationOffset;

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        HandleHorizontalMovement();

        HandleVerticalMovement();
    }

    private void HandleHorizontalMovement()
    {
        var moveDirection = _move.action.ReadValue<Vector2>();

        trueMoveDirection = new Vector3(moveDirection.x, 0, moveDirection.y);
        trueMoveDirection = Quaternion.Euler(0, rotationOffset, 0) * trueMoveDirection;

        transform.Translate(trueMoveDirection * _moveSpeed * Time.deltaTime, Space.World);
    }

    private void HandleVerticalMovement()
    {
        Vector2 scroll = Mouse.current.scroll.ReadValue();
        if (scroll.y > 0)
        {
            transform.position += new Vector3(0, _verticalOffset, 0);
            print("Going up");
        }
        else if (scroll.y < 0 && transform.position.y - _verticalOffset > 0)
        {
            transform.position -= new Vector3(0, _verticalOffset, 0);
            print("Going down");
        }
    }
    
}
