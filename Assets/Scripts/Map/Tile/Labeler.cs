using UnityEngine;
using TMPro;
using System;

[ExecuteAlways]
public class Labeler : MonoBehaviour
{
    TextMeshPro label;
    Vector3Int cords = new Vector3Int();
    public Vector3Int Cords {get{ return cords; }}
    MapManager mapManager;

    void Awake()
    {
        mapManager = FindFirstObjectByType<MapManager>();
        label = GetComponentInChildren<TextMeshPro>();

        DisolayCords();
    }

    void Update()
    {
        DisolayCords();

        transform.name = cords.ToString();
    }
    void DisolayCords()
    {
        if (!mapManager) { return; }
        cords.x = Mathf.RoundToInt(transform.position.x / mapManager.UnityMapSize);
        cords.y = Mathf.RoundToInt(transform.position.y / mapManager.UnityMapSize);
        cords.z = Mathf.RoundToInt(transform.position.z / mapManager.UnityMapSize);

        label.text = $"{cords.x}, {cords.y}, {cords.z}";
    }
}
