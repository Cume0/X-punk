using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TileOverlay : MonoBehaviour
{
    GameObject highlight;

    public void Initialize()
    {
        highlight = transform.GetChild(0).GameObject();
    }

    public void Show() => highlight.SetActive(true);
    public void Hide() => highlight.SetActive(false);
}
