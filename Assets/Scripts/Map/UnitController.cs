using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    [SerializeField] public float jumpHight = 1f;
    [SerializeField] InputActionReference selectAction;
    [SerializeField] InputActionReference moveAction;
    Transform selectedUnit;
    Ray ray; //where raycast is contained
    RaycastHit hit; //where raycast target is contained

    bool unitSelected = false;

    MapManager mapManager;
    Pathfinding pathfinding;
    void Start()
    {
        mapManager = FindFirstObjectByType<MapManager>();

        pathfinding = GetComponent<Pathfinding>();
    }

    void Update()
    {
        UnitActionHandler();
    }

    public void UnitActionHandler()
    {
        if (selectAction.action.WasPerformedThisFrame())
        {
            ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //raycast created

            unitSelector();
        }
        else if (moveAction.action.WasPerformedThisFrame() && unitSelected)
        {
            ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //raycast created

            unitMover();
        }
    }

    public void unitSelector()
    {
        bool hasHit = Physics.Raycast(ray, out hit); // raycast shot

        if (hasHit)
        { 
            if (hit.transform.TryGetComponent(out Labeler tileLabeler))
            {
                print("Tile selected");
            }

            if (hit.transform.TryGetComponent(out PawnData pawnData))
            {
                selectedUnit = hit.transform;
                unitSelected = !unitSelected;
                print("Unit selected =" + unitSelected);
            }
        }
    }

    public void unitMover()
    {  
        Physics.Raycast(ray, out hit); // raycast shot

        Vector3 pos = selectedUnit.position / mapManager.UnityMapSize;
        Vector3Int startCords = Vector3Int.RoundToInt(pos);
        Node startNode = mapManager.GetNode(startCords);

        Vector3Int targetCords = hit.transform.GetComponent<Labeler>().Cords;
        Node endNode = mapManager.GetNode(targetCords);
                        
        selectedUnit.GetComponent<PlayerPawnScript>().path = pathfinding.FindPath(startNode, endNode);
        StartCoroutine(selectedUnit.GetComponent<PlayerPawnScript>().MoveAlongPath());

        unitSelected = false;

        print("Unit is moving");
    }
}
