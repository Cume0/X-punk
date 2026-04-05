using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileCursour : MonoBehaviour
{
    void Update()
    {
        Ray CursourRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit CursourHit;

        bool hasHit = Physics.Raycast(CursourRay, out CursourHit, Mathf.Infinity, ~LayerMask.NameToLayer("Terrain"));

        if (hasHit)
        {
            if (CursourHit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                transform.position = CursourHit.point;
            }
        }
    }
}