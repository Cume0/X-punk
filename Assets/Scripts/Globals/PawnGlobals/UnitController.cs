using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    UnitController current;
    [SerializeField] InputActionReference selectAction;
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference menuAction;

    GameEvents gameEvents;

    Ray CameraRay; //raycastontainer
    RaycastHit hit; //raycast target container for default raycast
    RaycastHit moveHit; //raycast target container for move raycast

    StatesEnum states;
    bool isUnitSelected = false;
    bool isUnitReadyToGo = false;
    GameObject previousSelectedUnit;
    GameObject selectedUnit;
    Pawn selectedUnitData;

    public void Initialize()
    {
        current = this;
        
        gameEvents = GameManager.gameEvents;

        gameEvents.onEndTurnButtonPressed += EndTurnClear;
        gameEvents.onShootingModeActivated += ActivateShootingMode;
        gameEvents.onShootingModeDeactivated += DeactivateShootingMode;
    }

    void Update()
    {
        Debug.DrawRay(CameraRay.origin, CameraRay.direction * 10, Color.yellow);

        UnitActionHandler();
    }

    public void UnitActionHandler()
    {
        if (selectAction.action.WasPerformedThisFrame())
        {
            CameraRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //create new raycast

            Selector();
        }
        else if (moveAction.action.WasPerformedThisFrame() && isUnitSelected)
        {
            CameraRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //create new raycast

            UnitMover();
        }
    }

    public void Selector()
    {
        bool hasHit = Physics.Raycast(CameraRay, out hit); //shoot raycast
        if (!hasHit)
        {
            return;
        }

        if (hit.transform.TryGetComponent(out Pawn pawn))
        {
            previousSelectedUnit = selectedUnit;
            selectedUnit = hit.transform.gameObject;
            selectedUnitData = hit.transform.GetComponent<Pawn>();

            if (selectedUnitData.state == StatesEnum.Busy || selectedUnitData.state == StatesEnum.InActive ||selectedUnitData.pawnInfo.team == TeamsEnum.Enemy)
            {
                return;
            }
            if (selectedUnit.Equals(previousSelectedUnit) && isUnitSelected)
            {
                isUnitSelected = false;
                gameEvents.UnitSelected(isUnitSelected, selectedUnit);
                return;
            }

            print(selectedUnitData.name + ": Orders?");
            isUnitSelected = true;
            gameEvents.UnitSelected(isUnitSelected, selectedUnit);
        }
    }

    public void UnitMover()
    {
        if (Physics.Raycast(CameraRay, out moveHit, Mathf.Infinity, LayerMask.GetMask("PlannedPosition")))
        {
            StartCoroutine(selectedUnitData.MoveAlongPath());
            return;   
        }
    }

    private void ActivateShootingMode()
    {
        enabled = false;
    }

    private void DeactivateShootingMode()
    {
        enabled = true;
    }

    private void EndTurnClear()
    {
        isUnitSelected = false;
        gameEvents.UnitSelected(isUnitSelected, selectedUnit);
        selectedUnit = null;
        selectedUnitData = null;       
    }
}