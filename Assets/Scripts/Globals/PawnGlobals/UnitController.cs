using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    [SerializeField] InputActionReference selectAction;
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference menuAction;

    GameEvents gameEvents;

    Ray ray; //where raycast is contained
    RaycastHit hit; //where raycast target is contained

    StatesEnum states;
    bool isUnitSelected = false;
    GameObject previousSelectedUnit;
    GameObject selectedUnit;
    Pawn selectedUnitData;

    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
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
        else if (moveAction.action.WasPerformedThisFrame() && isUnitSelected)
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
            if (hit.transform.TryGetComponent(out Terrain terrain))
            {
                if (selectedUnit == null)
                {
                    return;
                }

                unitMover();
            }

            if (hit.transform.TryGetComponent(out Pawn pawn))
            {
                previousSelectedUnit = selectedUnit;
                selectedUnit = hit.transform.gameObject;
                selectedUnitData = hit.transform.GetComponent<Pawn>();
                print("got it");

                if (selectedUnitData.state == StatesEnum.Busy || selectedUnitData.state == StatesEnum.InActive ||selectedUnitData.team == TeamsEnum.Enemy)
                {
                    return;
                }

                if (selectedUnit.Equals(previousSelectedUnit) && isUnitSelected)
                {
                    isUnitSelected = false;
                    gameEvents.UnitSelected(isUnitSelected, selectedUnit);
                    return;
                }

                isUnitSelected = true;
                gameEvents.UnitSelected(isUnitSelected, selectedUnit);
            }
        }
    }
    
    public void unitMover()
    {  
        Physics.Raycast(ray, out hit); // raycast shot

        selectedUnit.GetComponent<NavMeshAgent>().SetDestination(hit.point);
    }
}
