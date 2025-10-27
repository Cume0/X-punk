using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] InputActionReference selectAction;
    Transform selectedUnit;

    bool unitSelected = false;

    MapManager mapManager;
    void Start()
    {
        mapManager = FindFirstObjectByType<MapManager>();
    }

    void Update()
    {
        UnitMovementHandler();
    }
    
    void UnitMovementHandler()
    {
        if (selectAction.action.WasPerformedThisFrame())
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
                        Vector3Int startCords = new Vector3Int((int)selectedUnit.position.x, (int)selectedUnit.position.y, (int)selectedUnit.position.z) / mapManager.UnityMapSize;
                        Vector3Int targetCords = hit.transform.GetComponent<Labeler>().Cords;

                        selectedUnit.transform.position = new Vector3(targetCords.x, targetCords.y + 1, targetCords.z);

                        unitSelected = false;
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
}
