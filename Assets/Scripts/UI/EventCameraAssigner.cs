using UnityEngine;

public class EventCameraAssigner : MonoBehaviour
{
    private Canvas canvas;
    public void Initialize()
    {
        canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
        }
    }
}
