using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    [SerializeField] InputActionReference selectAction;
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference menuAction;

    Pathfinding pathfinding;
    GameEvents gameEvents;
    MapManager mapManager;

    Ray ray; //where raycast is contained
    RaycastHit hit; //where raycast target is contained

    StatesEnum states;
    bool unitSelected = false;
    Transform previousSelectedUnit;
    Transform selectedUnit;
    Pawn selectedUnitData;

    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
        mapManager = GameManager.mapManager;
        pathfinding = GameManager.pathfinding;
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
                //
            }

            if (hit.transform.TryGetComponent(out Pawn pawn))
            {
                previousSelectedUnit = selectedUnit;
                selectedUnit = hit.transform;
                selectedUnitData = selectedUnit.GetComponent<Pawn>();

                if (selectedUnitData.state == StatesEnum.Busy || selectedUnitData.state == StatesEnum.InActive ||selectedUnitData.team == TeamsEnum.Enemy)
                {
                    return;
                }

                if (selectedUnit.Equals(previousSelectedUnit) && unitSelected)
                {
                    unitSelected = false;
                    gameEvents.UnitSelected(unitSelected, selectedUnit);
                    return;
                }

                unitSelected = true;
                gameEvents.UnitSelected(unitSelected, selectedUnit);
            }
        }
    }

    public void unitMover()
    {  
        Physics.Raycast(ray, out hit); // raycast shot

        Vector3 pos = selectedUnit.position / mapManager.UnityMapSize;
        Vector3Int startCords = Vector3Int.RoundToInt(pos);
        Tile startTile = mapManager.GetTile(startCords);

        Vector3Int targetCords = hit.transform.GetComponent<Tile>().Cords;
        Tile endTile = mapManager.GetTile(targetCords);
                  
        selectedUnitData.path = pathfinding.FindPath(startTile, endTile, selectedUnitData.movePoints);

        StartCoroutine(selectedUnitData.MoveAlongPath());
    }
}
