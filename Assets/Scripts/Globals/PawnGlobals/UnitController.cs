using System;
using System.Collections.Generic;
using System.Linq;
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
    RaycastHit pathHit; //raycast target container for path raycast
    RaycastHit moveHit; //raycast target container for move raycast

    StatesEnum states;
    bool isUnitSelected = false;
    bool isUnitReadyToGo = false;
    GameObject previousSelectedUnit;
    GameObject selectedUnit;
    Pawn selectedUnitData;
    NavMeshPath unitsPath;
    List<Vector3> pathCorners;

    public void Initialize()
    {
        current = this;
        
        gameEvents = GameManager.gameEvents;

        gameEvents.onEndTurnButtonPressed += EndTurnClear;
        gameEvents.onShootingModeActivated += ActivateShootingMode;
        gameEvents.onShootingModeDeactivated += DeactivateShootingMode;

        unitsPath = new NavMeshPath();
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

            selector();
        }
        else if (moveAction.action.WasPerformedThisFrame() && isUnitSelected)
        {
            CameraRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //create new raycast

            unitMover();
        }
    }

    public void selector()
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

    public void unitMover()
    {
        bool hasHit = Physics.Raycast(CameraRay, out hit);
        if (!hasHit || !Physics.Raycast(CameraRay, out pathHit, Mathf.Infinity, LayerMask.GetMask("Terrain"))
        || selectedUnitData.state == StatesEnum.Busy || selectedUnitData.state == StatesEnum.InActive
        || selectedUnitData.movePoints <= 0.1f)
        {
            return;
        }

        if (isUnitReadyToGo &&
            Physics.Raycast(CameraRay, out moveHit, Mathf.Infinity, LayerMask.GetMask("PlannedPosition")))
        {
            StartCoroutine(selectedUnitData.MoveAlongPath());
            isUnitReadyToGo = false;
            return;   
        }

        isUnitReadyToGo = true;
        selectedUnitData.finalPathPoint = GetReachablePath();
        selectedUnitData.path = unitsPath;
        selectedUnitData.pathCorners = pathCorners;
        gameEvents.ShowPath(selectedUnitData.id, hit.point, selectedUnitData.pathCorners);
    }

    public Vector3 GetReachablePath()
    {   
        Physics.Raycast(CameraRay, out hit);

        float availableMovePoints = selectedUnitData.movePoints;
        float remainingMovePoints = availableMovePoints;
        float spentMovePoints = 0f;
        
        if (!NavMesh.CalculatePath(selectedUnit.transform.position, hit.point, 1, unitsPath))
        {
            print("invalid path to " + hit.point);
            selectedUnitData.previewMovePointsRemaining = availableMovePoints;
            selectedUnitData.previewMovePointsSpent = 0f;
            pathCorners = new List<Vector3>();
            return new Vector3();
        }
        
        Vector3[] corners = unitsPath.corners;
        
        if (corners.Length == 0)
        {
            print("path is empty");
            selectedUnitData.previewMovePointsRemaining = availableMovePoints;
            selectedUnitData.previewMovePointsSpent = 0f;
            pathCorners = new List<Vector3>();
            return new Vector3();
        }
        
        List<Vector3> path = new List<Vector3>();

        path.Add(corners[0]);
        for (int i = 0; i < corners.Length - 1; i++)
        {
            float segmentLength = Vector3.Distance(corners[i], corners[i + 1]);

            if (remainingMovePoints < segmentLength)
            {
                float t = remainingMovePoints / segmentLength;
                path.Add(Vector3.Lerp(corners[i], corners[i + 1], t));
                spentMovePoints += remainingMovePoints;
                remainingMovePoints = 0f;
                selectedUnitData.previewMovePointsRemaining = remainingMovePoints;
                selectedUnitData.previewMovePointsSpent = spentMovePoints;
                print("Too far, preview remaining movePoints: " + remainingMovePoints);
                pathCorners = path;
                
                return path.Last();
            }

            path.Add(corners[i + 1]);
            remainingMovePoints -= segmentLength;
            spentMovePoints += segmentLength;

            if (remainingMovePoints <= 0f)
            {
                selectedUnitData.previewMovePointsRemaining = remainingMovePoints;
                selectedUnitData.previewMovePointsSpent = spentMovePoints;
                pathCorners = path;
                return path.Last();
            }
        }

        selectedUnitData.previewMovePointsRemaining = remainingMovePoints;
        selectedUnitData.previewMovePointsSpent = spentMovePoints;
        print("Going to " + hit.point + ", preview remaining movePoints: " + remainingMovePoints);
        pathCorners = path;
        return path.Last();
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
        isUnitReadyToGo = false;
        selectedUnit = null;
        selectedUnitData = null;       
    }
}