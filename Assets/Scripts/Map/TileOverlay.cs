using UnityEngine;
using UnityEngine.InputSystem;

public class TileOverlay : MonoBehaviour
{
    void Update()
    {
        Ray CursourRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit CursourHit;

        bool hasHit = Physics.Raycast(CursourRay, out CursourHit);

        if (hasHit)
        {
            if (CursourHit.transform.tag == "Tile")
            {
                transform.position = CursourHit.transform.position;
            }
        }
    }
}
