using UnityEngine;
using UnityEngine.PlayerLoop;

public class MovementAreaOverlay : MonoBehaviour
{
    bool isOverlayShown = false;
    public void Initialize()
    {
        toogleOverlay();
    }

    public void toogleOverlay()
    {
        isOverlayShown = !isOverlayShown;
        transform.GetChild(0).gameObject.SetActive(isOverlayShown);
    }
}
