using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NavigationManager : MonoBehaviour
{
    GameEvents gameEvents;

    Pawn selectedUnitData;
    NavMeshPath calculatedPath;

    Ray CameraRay; //raycastontainer
    RaycastHit hit; //raycast target container for default raycast
    RaycastHit pathHit; //raycast target container for path raycast

    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;

        gameEvents.onUnitSelection += ActivateNavigationAfterSelection;
        gameEvents.onUnitDeselection += DeactivateNavigationAfterDeselection;
        gameEvents.onFreezePath += FreezePath;

        gameEvents.onShootingModeActivated += DeactivateNavigationAfterShooting;
        gameEvents.onShootingModeDeactivated += ActivateNavigationAfterShooting;

        calculatedPath = new NavMeshPath();

        enabled = false;
    }

    public void Update()
    {
        UpdatePath();
    }

    public void ActivateNavigationAfterSelection(GameObject selectedUnit)
    {
        selectedUnitData = selectedUnit.GetComponent<Pawn>();
        enabled = true;
    }

    public void DeactivateNavigationAfterDeselection(GameObject deselectedUnit)
    {
        enabled = false;
            
        if (selectedUnitData == null) return;

        print("Hiding path for " + selectedUnitData.id);
        GameManager.gameEvents.HidePath(selectedUnitData.id);
        selectedUnitData = null;
    }

    public void DeactivateNavigationAfterShooting()
    {
        enabled = false;
    }

    public void ActivateNavigationAfterShooting()
    {
        enabled = true;
    }

    public void UpdatePath()
    {
        if (selectedUnitData == null || selectedUnitData.path == null) return;

        CameraRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool hasHit = Physics.Raycast(CameraRay, out hit);
        if (!hasHit 
        || !Physics.Raycast(CameraRay, out pathHit, Mathf.Infinity, LayerMask.GetMask("Floor"))
        || selectedUnitData.state == StatesEnum.Busy 
        || selectedUnitData.state == StatesEnum.Inactive
        || selectedUnitData.CurrentMovePoints <= 0.1f)
        return;


        // Edge case handling: If the raycast hit point is not on the NavMesh, find the closest point on the NavMesh within a small radius. If no point is found, try a larger radius. If still no point is found, exit the method.
        float instantCheckRadius = 0.2f;
        if (!NavMesh.SamplePosition(pathHit.point, out NavMeshHit closeHit, instantCheckRadius, NavMesh.AllAreas))
        {
            float maxSearchRadius = 5.0f; 
            if (NavMesh.SamplePosition(pathHit.point, out NavMeshHit edgeHit, maxSearchRadius, NavMesh.AllAreas))
            {
                pathHit.point = edgeHit.position;
            }
            else
            {
                return;
            }
        }
        else
        {
            pathHit.point = closeHit.position;
        }

        selectedUnitData.pathCorners = GetReachablePath();
        selectedUnitData.path = calculatedPath;
        selectedUnitData.finalPathPoint = selectedUnitData.pathCorners.Last();
        gameEvents.ShowPath(selectedUnitData.id, pathHit.point, selectedUnitData.pathCorners, selectedUnitData.transform);
    }

    public void FreezePath(Guid ID)
    {
        if (selectedUnitData == null || selectedUnitData.id != ID) return;

        enabled = false;
    }

    public List<Vector3> GetReachablePath()
    {   
        Physics.Raycast(CameraRay, out hit);

        float availableMovePoints = selectedUnitData.CurrentMovePoints;
        float remainingMovePoints = availableMovePoints;
        float spentMovePoints = 0f;
        
        if (!NavMesh.CalculatePath(selectedUnitData.transform.position, pathHit.point, 1, calculatedPath))
        {
            print("invalid path to " + pathHit.point);
            selectedUnitData.previewMovePointsRemaining = availableMovePoints;
            selectedUnitData.previewMovePointsSpent = 0f;
            return new List<Vector3>();
        }
        
        Vector3[] corners = calculatedPath.corners;
        
        if (corners.Length == 0)
        {
            print("path is empty");
            selectedUnitData.previewMovePointsRemaining = availableMovePoints;
            selectedUnitData.previewMovePointsSpent = 0f;
            return new List<Vector3>();
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
                
                return path;
            }

            path.Add(corners[i + 1]);
            remainingMovePoints -= segmentLength;
            spentMovePoints += segmentLength;

            if (remainingMovePoints <= 0f)
            {
                selectedUnitData.previewMovePointsRemaining = remainingMovePoints;
                selectedUnitData.previewMovePointsSpent = spentMovePoints;
                return path;
            }
        }

        selectedUnitData.previewMovePointsRemaining = remainingMovePoints;
        selectedUnitData.previewMovePointsSpent = spentMovePoints;
        return path;
    }
}
