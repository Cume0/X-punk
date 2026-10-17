using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] EnemyButton enemyButton;
    [SerializeField] Transform enemyBar;
    [SerializeField] Transform menu;
    [SerializeField] Transform volume;
    [SerializeField] PathLineVisual pathLineVisual;
    [SerializeField] GameObject mainCameraObject;

    GameEvents gameEvents;

    IsometricCameraController isometricCameraController;
    FirstPersonCameraController firstPersonCameraController;

    Transform isometricCameraPosition;
    GameObject TacticUI;
    GameObject ShootingUI;
    GameObject MapCursor;

    GameObject thisUnit;
    Pawn thisUnitData;

    Dictionary<Guid, PathLineVisual> pathLineVisuals = new Dictionary<Guid, PathLineVisual>();
    Dictionary<Guid, PathLineVisual> WorldSpaceCanvas = new Dictionary<Guid, PathLineVisual>();
    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;

        gameEvents.onUnitSelection += UnitSelected;
        gameEvents.onUnitDeselection += UnitDeselected;
        gameEvents.onEndTurn += EndTurnClear;          

        gameEvents.onShowTacticUI += ShowTacticUI;
        gameEvents.onHideTacticUI += HideTacticUI;

        gameEvents.onShowPath += ShowPath;
        gameEvents.onHidePath += HidePath;

        gameEvents.onFreezePath += FreezePath;
        gameEvents.onUnfreezePath += UnfreezePath;

        GameObject.Find("EndTurnButton").GetComponent<EndTurnButton>().Initialize();

        foreach (GameObject pawn in GameManager.unitManager.pawns.Values)
        {
            var pathLine = Instantiate(pathLineVisual, transform.Find("WorldSpaceUI").Find("PathsLineVisuals"));
            Pawn pawnData = pawn.GetComponent<Pawn>();
            pathLine.Initialize(pawnData.id);
            pathLineVisuals.Add(pawnData.id, pathLine);
        }

        TacticUI = GameObject.Find("TacticUI");
        TacticUI.SetActive(false);
        ShootingUI = GameObject.Find("ShootingUI");
        ShootingUI.SetActive(false);

        MapCursor = GameObject.Find("MapCursor");

        isometricCameraController = mainCameraObject.GetComponent<IsometricCameraController>();
        firstPersonCameraController = mainCameraObject.GetComponent<FirstPersonCameraController>();
        isometricCameraPosition = GameObject.Find("IsometricCameraPosition").transform;

        isometricCameraController.Initialize();
        firstPersonCameraController.Initialize();
    }

    public void UnitSelected(GameObject selectedUnit)
    {
        thisUnit = selectedUnit;

        thisUnitData = selectedUnit.GetComponent<Pawn>();
        thisUnitData.selectedUnitSelectionSign.SetActive(true);

        ShowTacticUI();
    }

    public void UnitDeselected(GameObject deselectedUnit)
    {
        HideTacticUI();

        thisUnitData.selectedUnitSelectionSign.SetActive(false);
        thisUnitData = null;

        thisUnit = null;
    }

    public void EnterShootingUI()
    {
        ShowShootingUI();
        HideTacticUI();

        gameEvents.ShootingModeActivated();

        mainCameraObject.transform.SetParent(thisUnit.transform.Find("FirstPersonCameraPosition"));

        isometricCameraController.enabled = false;

        firstPersonCameraController.playerBody = thisUnit.transform;
        firstPersonCameraController.enabled = true;

        MapCursor.SetActive(false);
    }

    public void ExitShootingUI()
    {
        HideShootingUI();
        ShowTacticUI();
        
        gameEvents.ShootingModeDeactivated();

        mainCameraObject.transform.SetParent(isometricCameraPosition);
        mainCameraObject.transform.localPosition = Vector3.zero;

        isometricCameraController.enabled = true;

        firstPersonCameraController.enabled = false;

        MapCursor.SetActive(true);
    }

    private void ShowPath(Guid guid, Vector3 FuturePositionCords, List<Vector3> path, Transform pawnTransform)
    {
        if (pathLineVisuals.TryGetValue(guid, out PathLineVisual pathLine))
        {
            MapCursor.SetActive(false);
            pathLine.ShowPath(pawnTransform);
        }
    }
    
    private void HidePath(Guid guid)
    {
        if (pathLineVisuals.TryGetValue(guid, out PathLineVisual pathLine))
        {
            MapCursor.SetActive(true);
            pathLine.HidePath();
        }
    }

    public void FreezePath(Guid guid)
    {
        if (pathLineVisuals.TryGetValue(guid, out PathLineVisual pathLine))
        {
            pathLine.FreezePath();
        }
    }

    public void UnfreezePath(Guid guid)
    {
        if (pathLineVisuals.TryGetValue(guid, out PathLineVisual pathLine))
        {
            pathLine.UnfreezePath();
        }
    }

    public void ShowTacticUI()
    {
        TacticUI.SetActive(true);
    }
    public void HideTacticUI()
    {
        TacticUI.SetActive(false);
    }
    public void ShowShootingUI()
    {
        ShootingUI.SetActive(true);
    }
    public void HideShootingUI()
    {
        ShootingUI.SetActive(false);
    }

    public void EndTurnClear()
    {
        HideTacticUI();
        if (thisUnitData == null) return;

        HidePath(thisUnitData.id);
        thisUnitData.selectedUnitSelectionSign.SetActive(false);
        print(thisUnitData.selectedUnitSelectionSign + " is now " + thisUnitData.selectedUnitSelectionSign.activeSelf);
    }
    public void Exit()
    {
        Application.Quit();
    }
}