using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class IsometricCameraController : MonoBehaviour
{
    GameEvents gameEvents;
    UnitManager unitManager;

    private Vector3 trueMoveDirection;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _verticalOffset;
    [SerializeField] private InputActionReference _move;
    [SerializeField] private InputActionReference _scrollY;

    [SerializeField] private float rotationOffset;

    private Quaternion savedRotation;

    private bool isInitialized = false;

    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
        unitManager = GameManager.unitManager;

        gameEvents.onPawnsTurn += OnPawnsTurn;

        savedRotation = transform.rotation;
        isInitialized = true;
    }

    void Update()
    {
        HandleInput();
    }

    private void OnPawnsTurn(Guid id)
    {
        if (unitManager.getPawnByID(id, out GameObject pawn))
        {
            transform.parent.position = pawn.transform.position + new Vector3(0, 6, -3);
        }
        else
        {
            print("Pawn with ID " + id + " not found in UnitManager. (by IsometricCameraController)");
        }
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

        transform.parent.Translate(trueMoveDirection * _moveSpeed * Time.deltaTime, Space.World);
    }

    private void HandleVerticalMovement()
    {
        Vector2 scroll = Mouse.current.scroll.ReadValue();
        if (scroll.y > 0)
        {
            transform.position += new Vector3(0, _verticalOffset, 0);
        }
        else if (scroll.y < 0 && transform.position.y - _verticalOffset > 0)
        {
            transform.position -= new Vector3(0, _verticalOffset, 0);
        }
    }
    
    private void OnEnable()
    {
        if(isInitialized)
        {
            transform.rotation = savedRotation;
        }
    }

    private void OnDisable()
    {
        savedRotation = transform.rotation;
    }
}