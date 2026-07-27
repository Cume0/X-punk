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

    List<Vector3> pathCorners;

    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;

        gameEvents.onUnitSelection += ActivateNavigationAfterSelection;

        gameEvents.onShootingModeActivated += DeactivateNavigationAfterShooting;
        gameEvents.onShootingModeDeactivated += ActivateNavigationAfterShooting;

        

        calculatedPath = new NavMeshPath();

        enabled = false;
    }

    public void Update()
    {
        UpdatePath();
    }

    public void ActivateNavigationAfterSelection(bool isUnitSelected, GameObject selectedUnit)
    {
        if (isUnitSelected == false)
        {
            enabled = false;

            GameManager.gameEvents.HidePath(selectedUnitData.id);
            selectedUnitData = null;
            return;
        }

        selectedUnitData = selectedUnit.GetComponent<Pawn>();
        enabled = true;
    }

    public void DeactivateNavigationAfterShooting()
    {
        enabled = false;
        gameEvents.HidePath(selectedUnitData.id);
    }

    public void ActivateNavigationAfterShooting()
    {
        enabled = true;
    }

    public void UpdatePath()
    {
        if (selectedUnitData == null)
        {
            return;
        }

        CameraRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool hasHit = Physics.Raycast(CameraRay, out hit);
        if (!hasHit 
        || !Physics.Raycast(CameraRay, out pathHit, Mathf.Infinity, LayerMask.GetMask("Terrain"))
        || selectedUnitData.state == StatesEnum.Busy 
        || selectedUnitData.state == StatesEnum.InActive
        || selectedUnitData.movePoints <= 0.1f)
        {
            return;
        }

        selectedUnitData.finalPathPoint = GetReachablePath();
        selectedUnitData.path = calculatedPath;
        selectedUnitData.pathCorners = pathCorners;
        gameEvents.ShowPath(selectedUnitData.id, hit.point, selectedUnitData.pathCorners);
    }

    public Vector3 GetReachablePath()
    {   
        Physics.Raycast(CameraRay, out hit);

        float availableMovePoints = selectedUnitData.movePoints;
        float remainingMovePoints = availableMovePoints;
        float spentMovePoints = 0f;
        
        if (!NavMesh.CalculatePath(selectedUnitData.transform.position, hit.point, 1, calculatedPath))
        {
            print("invalid path to " + hit.point);
            selectedUnitData.previewMovePointsRemaining = availableMovePoints;
            selectedUnitData.previewMovePointsSpent = 0f;
            pathCorners = new List<Vector3>();
            return new Vector3();
        }
        
        Vector3[] corners = calculatedPath.corners;
        
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
}
