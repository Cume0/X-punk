using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] EnemyButton enemyButton;
    [SerializeField] Transform enemyBar;
    [SerializeField] Transform menu;
    [SerializeField] Transform volume;
    GameObject battleUI;

    GameEvents gameEvents;
    Pathfinding pathfinding;
    MapManager mapManager;

    Transform thisUnit;
    Pawn thisUnitData;

    List<Tile> highlightedTiles = new List<Tile>();
    bool AttackButtonPressed = false;
    bool isThisUnitSelected = false;
    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
        pathfinding = GameManager.pathfinding;
        mapManager = GameManager.mapManager;

        gameEvents.onUnitSelection += UnitSelected;
        gameEvents.onShowBattleUI += ShowBattleUI;
        gameEvents.onHideBattleUI += HideBattleUI;
        gameEvents.onKill += AfterKillUIRefresh;

        gameEvents.onPawnWalkFinish += PawnWalkFinished;
        gameEvents.onPawnWalkStart += PawnWalkStart;

        GameObject.Find("EndTurnButton").GetComponent<EndTurnButton>().Initialize();

        battleUI = GameObject.Find("BattleUI");
        battleUI.SetActive(false);
    }

    //

    public void UnitSelected(bool isUnitSelected, Transform selectedUnit)
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
        ShowMovementArea();
        battleUI.SetActive(true);
    }

    public void HideBattleUI()
    {
        HideMovementArea();
        HideUnitsInSight();
        AttackButtonPressed = false;
        battleUI.SetActive(false);
    }

    private void AfterKillUIRefresh(int killerID, int victimID)
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
    
    public void PawnWalkStart(int ID)
    {
        if (ID != thisUnitData.id)
        {
            return;
        }

        HideBattleUI();
    }
    public void PawnWalkFinished(int ID)
    {
        if (ID != thisUnitData.id)
        {
            return;
        }

        ShowBattleUI();
    }

    public void Exit()
    {
        Application.Quit();
    }

    //

    public void ShowMovementArea()
    {
        HideMovementArea();

        Vector3Int startCords = Vector3Int.RoundToInt(thisUnit.position / mapManager.UnityMapSize);

        Tile startTile = mapManager.GetTile(startCords);

        var tiles = pathfinding.GetReachableTiles(
            startTile,
            thisUnitData.movePoints
        );

        foreach (var Tile in tiles)
        {
            if (Tile.tileOverlay != null)
            {
                Tile.tileOverlay.Show();
                highlightedTiles.Add(Tile);
            }
        }
    }

    void HideMovementArea()
    {
        foreach (var Tile in highlightedTiles)
            Tile.tileOverlay?.Hide();

        highlightedTiles.Clear();
    }

    public void ShowUnitsInSight()
    {
        HideUnitsInSight();

        int thisUnitSightRange = thisUnitData.SightDistance;

        foreach (Transform unit in GameManager.unitManager.pawns.Values)
        {
            if ( Vector3.Distance(thisUnit.position, unit.position) < thisUnitSightRange && 
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
        thisUnitData.pawnsInSight.Clear();

        if (enemyBar.childCount == 0)
        {
            return;
        }

        for (int childIndex = 0; childIndex < enemyBar.childCount; childIndex++)
        {
            Destroy(enemyBar.transform.GetChild(childIndex).gameObject);
        }
    }
}
