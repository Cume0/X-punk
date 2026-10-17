using System;
using System.Collections.Generic;
using UnityEngine;

public class PathLineVisual : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform futurePositionTransform;
    private Pawn trackedPawn;

    bool isPathFrozen = false;
    private Vector3[] renderBuffer = new Vector3[0];

    public void Initialize(Guid pawnGuid)
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        futurePositionTransform = transform.GetChild(0).Find("FuturePosition");
        HidePath();
    }

    private void Update()
    {
        UpdatePathVisual();
    }

    public void ShowPath(Transform givenPawnTransform)
    {
        if (isPathFrozen) return;
        
        if (trackedPawn == null || trackedPawn.transform != givenPawnTransform)
        {
            trackedPawn = givenPawnTransform.GetComponent<Pawn>();
        }

        transform.GetChild(0).gameObject.SetActive(true);
        enabled = true;
    }

    private void UpdatePathVisual()
    {
        if (isPathFrozen) return;

        // Если пешки нет или у неё пустой путь — просто обнуляем линию
        if (trackedPawn == null || trackedPawn.pathCorners == null || trackedPawn.pathCorners.Count == 0)
        {
            lineRenderer.positionCount = 0;
            HidePath();
            return;
        }

        List<Vector3> currentCorners = trackedPawn.pathCorners;
        int pointsCount = currentCorners.Count;

        if (renderBuffer.Length != pointsCount)
        {
            renderBuffer = new Vector3[pointsCount];
        }

        currentCorners.CopyTo(renderBuffer);

        renderBuffer[0] = trackedPawn.transform.position;

        lineRenderer.positionCount = pointsCount;
        lineRenderer.SetPositions(renderBuffer);

        futurePositionTransform.position = renderBuffer[renderBuffer.Length - 1];
    }

    public void HidePath()
    {       
        if (transform.GetChild(0).gameObject.activeSelf)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            lineRenderer.positionCount = 0;
        }
        trackedPawn = null;
        isPathFrozen = false;
        enabled = false;
    }

    public void FreezePath()
    {
        isPathFrozen = true;
        enabled = false;
    }

    public void UnfreezePath()
    {
        if (trackedPawn != null)
        {
            isPathFrozen = false;
            enabled = true;
        }
    }
}
