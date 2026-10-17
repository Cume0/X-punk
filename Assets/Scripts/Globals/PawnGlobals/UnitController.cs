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

        gameEvents.onEndTurn += EndTurnClear;
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
        else if (menuAction.action.WasPerformedThisFrame())
        {
            UnitCancler();
        }
    }

    public void Selector()
    {
        int maskExceptMarker = ~LayerMask.GetMask("PlannedPosition");

        bool hasHit = Physics.Raycast(CameraRay, out hit, Mathf.Infinity, maskExceptMarker); //shoot raycast
        if (!hasHit) return;

        if (hit.transform.TryGetComponent(out Pawn pawn))
        {
            if (previousSelectedUnit == null)
            {
                previousSelectedUnit = hit.transform.gameObject;
            }

            if (pawn.state == StatesEnum.Busy || pawn.state == StatesEnum.Inactive || pawn.pawnInfo.team == TeamsEnum.Enemy)
            {
                selectedUnit = previousSelectedUnit;
                selectedUnitData = previousSelectedUnit.GetComponent<Pawn>();
                print("Unit " + pawn.name + " is not selectable.");
                return;
            }

            if (pawn.gameObject.Equals(previousSelectedUnit) && isUnitSelected)
            {
                isUnitSelected = false;
                gameEvents.UnitDeselected(pawn.gameObject);
                print("Deselected " + pawn.name);
                return;
            }

            selectedUnit = hit.transform.gameObject;
            selectedUnitData = pawn;
            previousSelectedUnit = selectedUnit;

            print(selectedUnitData.name + ": Orders?");
            isUnitSelected = true;
            gameEvents.UnitSelected(selectedUnit);
        }
    }

    public void UnitMover()
    {
        if (selectedUnitData.state == StatesEnum.Active)
        {
            selectedUnitData.state = StatesEnum.Planning;
            gameEvents.FreezePath(selectedUnitData.id);
            return;
        }

        if (!Physics.Raycast(CameraRay, out moveHit, Mathf.Infinity, LayerMask.GetMask("PlannedPosition"))) return;

        
        gameEvents.UnfreezePath(selectedUnitData.id);
        StartCoroutine(selectedUnitData.MoveAlongPath());
    }

    public void UnitCancler()
    {
        if (!isUnitSelected || selectedUnitData == null) return;

        
        if (selectedUnitData.state == StatesEnum.Planning)
        {
            selectedUnitData.state = StatesEnum.Active;
            gameEvents.UnfreezePath(selectedUnitData.id);
            return;
        }

        isUnitSelected = false;
        gameEvents.UnitDeselected(selectedUnit);
        print("Deselected " + selectedUnitData.name);
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
        gameEvents.UnitDeselected(selectedUnit);
        selectedUnit = null;
        selectedUnitData = null;       
    }
}