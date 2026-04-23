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

    bool AttackButtonPressed = false;
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

        if (isUnitSelected)
        {
            ShowTacticUI();
        }
        else
        {
            HideTacticUI();
        }
    }

    public void ShowTacticUI()
    {
        TacticUI.SetActive(true);
    }

    public void HideTacticUI()
    {
        HideUnitsInSight();
        AttackButtonPressed = false;
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

        isometricCameraController.enabled = true;

        firstPersonCameraController.playerBody = null;
        firstPersonCameraController.enabled = false;

        MapCursor.SetActive(true);
    }

    public void toogleShowUnitsInRange()
    {
        AttackButtonPressed = !AttackButtonPressed;

        if (AttackButtonPressed)
        {
            ShowUnitsInSight();
        }
        else
        {
            HideUnitsInSight();
        }
    }

    public void ShowUnitsInSight()
    {
        HideUnitsInSight();

        int thisUnitSightRange = thisUnitData.SightDistance;

        foreach (GameObject unit in GameManager.unitManager.pawns.Values)
        {
            if ( Vector3.Distance(thisUnit.transform.position, unit.transform.position) < thisUnitSightRange && 
                !unit.Equals(thisUnit) && 
                unit.GetComponent<Pawn>().team == TeamsEnum.Enemy )
            {
                thisUnitData.pawnsInSight.Add(unit);

                var enemyOnButton = Instantiate(enemyButton, enemyBar, false);
                enemyOnButton.Initialize(thisUnitData.id, unit.GetComponent<Pawn>().id);
            }
        }
    }

    public void HideUnitsInSight()
    {
        if (thisUnitData.pawnsInSight.Count != 0)
        {
            thisUnitData.pawnsInSight.Clear();
        }

        if (enemyBar.childCount == 0)
        {
            return;
        }

        for (int childIndex = 0; childIndex < enemyBar.childCount; childIndex++)
        {
            Destroy(enemyBar.transform.GetChild(childIndex).gameObject);
        }
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

    public void EndTurnClear()
    {
        HideTacticUI();
        HidePath(thisUnitData.id);
        thisUnitData.path.Clear();
    }
    public void Exit()
    {
        Application.Quit();
    }
}