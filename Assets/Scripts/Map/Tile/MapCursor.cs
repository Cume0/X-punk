using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapCursor : MonoBehaviour
{
    public LayerMask targetLayerMask;
    GameEvents gameEvents;
    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
    }
    void Update()
    {
        Ray CursourRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit CursourHit;

        bool hasHit = Physics.Raycast(CursourRay, out CursourHit, Mathf.Infinity, targetLayerMask);

        if (hasHit)
        {
            transform.position = CursourHit.point;
        }
    }
}