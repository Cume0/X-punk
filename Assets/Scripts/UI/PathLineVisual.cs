using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathLineVisual : MonoBehaviour
{
    GameEvents gameEvents;
    LineRenderer lineRenderer;
    Transform futurePosition;

    Guid assignedPawnGuid;

    Vector3 FuturePositionCords;
    List<Vector3> path;
    public void Initialize(Guid pawnGuid)
    {
        gameEvents = GameManager.gameEvents;

        assignedPawnGuid = pawnGuid;
        lineRenderer = GetComponentInChildren<LineRenderer>();
        futurePosition = transform.GetChild(0).Find("FuturePosition");
        HidePath();

        enabled = false;
    }

    public void Update()
    {
        UpdatePathVisual();
    }

    public void ShowPath(Vector3 GivenFuturePositionCords, List<Vector3> GivenPath)
    {
        FuturePositionCords = GivenFuturePositionCords;
        path = GivenPath;
        
        enabled = true;
    }

    public void UpdatePathVisual()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        
        futurePosition.position = FuturePositionCords;
        renderLine(path);
    }

    public void HidePath()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void renderLine(List<Vector3> path)
    {
        futurePosition.position = path.Last();
        lineRenderer.positionCount = path.Count;
        lineRenderer.SetPositions(path.ToArray());
    }
}