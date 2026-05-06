using System;
using System.Collections.Generic;
using System.Linq;
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

    bool isThisUnitSelected = false;

    Dictionary<Guid, PathLineVisual> pathLineVisuals = new Dictionary<Guid, PathLineVisual>();
    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;

        gameEvents.onUnitSelection += UnitSelected;  
        gameEvents.onEndTurnButtonPressed += EndTurnClear;          

        gameEvents.onShowTacticUI += ShowTacticUI;
        gameEvents.onHideTacticUI += HideTacticUI;

        gameEvents.onShowPath += ShowPath;
        gameEvents.onHidePath += HidePath;

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

    public void UnitSelected(bool isUnitSelected, GameObject selectedUnit)
    {
        thisUnit = selectedUnit;
        thisUnitData = selectedUnit.GetComponent<Pawn>();
        isThisUnitSelected = isUnitSelected;
        thisUnitData.selectedUnitSelectionSign.SetActive(isUnitSelected);

        if (isUnitSelected)
        {
            ShowTacticUI();
        }
        else
        {
            HideTacticUI();
        }
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

    private void ShowPath(Guid guid, Vector3 FuturePositionCords, List<Vector3> path)
    {
        if (pathLineVisuals.TryGetValue(guid, out PathLineVisual pathLine))
        {
            pathLine.ShowPath(FuturePositionCords, path);
        }
    }
    
    private void HidePath(Guid guid)
    {
        if (pathLineVisuals.TryGetValue(guid, out PathLineVisual pathLine))
        {
            pathLine.HidePath();
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
        if (thisUnitData != null)
        {
            HidePath(thisUnitData.id);
            thisUnitData.selectedUnitSelectionSign.SetActive(false);
        }
    }
    public void Exit()
    {
        Application.Quit();
    }
}