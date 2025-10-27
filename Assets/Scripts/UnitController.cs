using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 1f;
    Transform selectedUnit;

    bool unitSelected = false;

    MapManager mapManager;
    void Start()
    {
        mapManager = FindFirstObjectByType<MapManager>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        bool hasHit = Physics.Raycast(ray, out hit);

        if (hasHit)
        {
            if (hit.transform.tag == "Tile")
            {
                if (unitSelected)
                {
                    Vector2Int startCords = new Vector2Int((int)selectedUnit.position.x, (int)selectedUnit.position.y) / mapManager.UnityMapSize;
                    Vector2Int targetCords = hit.transform.GetComponent<Labeler>().Cords;

                    selectedUnit.transform.position = new Vector3(targetCords.x, selectedUnit.position.y, targetCords.y);
                }
            }

            if (hit.transform.tag == "Unit")
            {
                selectedUnit = hit.transform;
                unitSelected = true;
            }
        }
    }
}
