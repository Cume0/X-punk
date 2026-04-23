using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathLineVisual : MonoBehaviour
{
    Guid assignedPawnGuid;
    LineRenderer lineRenderer;
    Transform futurePosition;
    public void Initialize(Guid pawnGuid)
    {
        assignedPawnGuid = pawnGuid;
        lineRenderer = GetComponentInChildren<LineRenderer>();
        futurePosition = transform.GetChild(0).Find("FuturePosition");
        HidePath();
    }

    public void ShowPath(Vector3 GivenFuturePositionCords, List<Vector3> path)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        
        futurePosition.position = GivenFuturePositionCords;
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