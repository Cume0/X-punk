using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileCursour : MonoBehaviour
{
    public LayerMask targetLayerMask;
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