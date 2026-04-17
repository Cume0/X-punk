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

    GameEvents gameEvents;

    GameObject battleUI;
    GameObject thisUnit;
    Pawn thisUnitData;

    bool AttackButtonPressed = false;
    bool isThisUnitSelected = false;
    
    Dictionary<Guid, PathLineVisual> pathLineVisuals = new Dictionary<Guid, PathLineVisual>();
    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;

        gameEvents.onUnitSelection += UnitSelected;
        gameEvents.onShowBattleUI += ShowBattleUI;
        gameEvents.onHideBattleUI += HideBattleUI;
        gameEvents.onKill += AfterKillUIRefresh;
        gameEvents.onShowPath += ShowPath;
        gameEvents.onHidePath += HidePath;

        gameEvents.onEndTurnButtonPressed += EndTurnClear;

        GameObject.Find("EndTurnButton").GetComponent<EndTurnButton>().Initialize();

        foreach (GameObject pawn in GameManager.unitManager.pawns.Values)
        {
            var pathLine = Instantiate(pathLineVisual, transform.Find("WorldSpaceUI").Find("PathsLineVisuals"));
            Pawn pawnData = pawn.GetComponent<Pawn>();
            pathLine.Initialize(pawnData.id);
            pathLineVisuals.Add(pawnData.id, pathLine);
        }

        battleUI = GameObject.Find("BattleUI");
        battleUI.SetActive(false);
    }

    public void UnitSelected(bool isUnitSelected, GameObject selectedUnit)
    {
        thisUnit = selectedUnit;
        thisUnitData = selectedUnit.GetComponent<Pawn>();
        isThisUnitSelected = isUnitSelected;

        if (isUnitSelected)
        {
            ShowBattleUI();
        }
        else
        {
            HideBattleUI();
        }
    }

    public void ShowBattleUI()
    {
        battleUI.SetActive(true);
    }

    public void HideBattleUI()
    {
        HideUnitsInSight();
        AttackButtonPressed = false;
        battleUI.SetActive(false);
    }

    private void AfterKillUIRefresh(Guid killerID, Guid victimID)
    {
        HideBattleUI();
        ShowBattleUI();
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
        HideBattleUI();
        HidePath(thisUnitData.id);
        thisUnitData.path.Clear();
    }
    public void Exit()
    {
        Application.Quit();
    }
}