using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    [SerializeField] InputActionReference selectAction;
    [SerializeField] InputActionReference moveAction;
    GameManager gameManager;
    Transform selectedUnit;
    Transform previousSelectedUnit;

    Ray ray; //where raycast is contained
    RaycastHit hit; //where raycast target is contained

    bool unitSelected = false;

    StatesEnum states;
    MapManager mapManager;
    Pathfinding pathfinding;
    List<Tile> highlightedTiles = new List<Tile>();

    public void Initialize()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        mapManager = gameManager.mapManager;
        pathfinding = GetComponent<Pathfinding>();

        selectedUnit = new GameObject("Null").GetComponent<Transform>();
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

            selector();

            showMovementArea(unitSelected);
        }
        else if (moveAction.action.WasPerformedThisFrame() && unitSelected)
        {
            ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //raycast created

            unitMover();
        }
    }

    public void selector()
    {
        bool hasHit = Physics.Raycast(ray, out hit); // raycast shot

        if (hasHit)
        { 
            if (hit.transform.TryGetComponent(out Tile tile))
            {
                print("Tile selected");
            }

            if (hit.transform.TryGetComponent(out Pawn pawn))
            {
                previousSelectedUnit = selectedUnit;
                selectedUnit = hit.transform;

                if (selectedUnit.Equals(previousSelectedUnit))
                {
                    unitSelected = !unitSelected;
                    return;
                }

                

                if (selectedUnit.GetComponent<Pawn>().state == StatesEnum.Moving)
                {
                    return;
                }

                unitSelected = true;
            }
        }
    }

    public void unitMover()
    {  
        Physics.Raycast(ray, out hit); // raycast shot

        Vector3 pos = selectedUnit.position / mapManager.UnityMapSize;
        Vector3Int startCords = Vector3Int.RoundToInt(pos);
        Tile startTile = mapManager.GetTile(startCords);

        Vector3Int targetCords = hit.transform.GetComponent<Labeler>().Cords;
        Tile endTile = mapManager.GetTile(targetCords);
                  
        selectedUnit.GetComponent<Pawn>().path = pathfinding.FindPath(startTile, endTile);

        if (selectedUnit.GetComponent<Pawn>().path.Count > selectedUnit.GetComponent<Pawn>().movePoints)
        {
            print("Too far");
            return;
        }

        StartCoroutine(selectedUnit.GetComponent<Pawn>().MoveAlongPath());

        unitSelected = false;

        ClearOverlay();

        print("Unit is moving");
    }

    public void showMovementArea(bool Activate)
    {
        ClearOverlay();

        if (Activate == false)
        {
            return;
        }
        
        Vector3Int startCords = Vector3Int.RoundToInt(selectedUnit.transform.position / mapManager.UnityMapSize);

        Tile startTile = mapManager.GetTile(startCords);

        var tiles = pathfinding.GetReachableTiles(
            startTile,
            selectedUnit.GetComponent<Pawn>().movePoints
        );

        foreach (var Tile in tiles)
        {
            if (Tile.tileOverlay != null)
            {
                Tile.tileOverlay.Show();
                highlightedTiles.Add(Tile);
            }
        }
    }

    void ClearOverlay()
    {
        foreach (var Tile in highlightedTiles)
            Tile.tileOverlay?.Hide();

        highlightedTiles.Clear();
    }
}
