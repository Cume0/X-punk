using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public void Initialize()
    {
        
    }

    public event Action<int, int> onAttackTrigger;
    public void AttackTrigger(int AttackerID, int DefenderID)
    {
        if (onAttackTrigger != null)
        {
            onAttackTrigger(AttackerID, DefenderID);
        }
    }

    public event Action<bool, Transform> onUnitSelection;
    public void UnitSelected(bool isUnitSelected, Transform selectedUnit)
    {
        if (onUnitSelection != null)
        {
            onUnitSelection(isUnitSelected, selectedUnit);
        }
    }

    public event Action onShowBattleUI;
    public void ShowBattleUI()
    {
        if (onShowBattleUI != null)
        {
            onShowBattleUI();
        }
    }

    public event Action onHideBattleUI;
    public void HideBattleUI()
    {
        if (onHideBattleUI != null)
        {
            onHideBattleUI();
        }
    }

    public event Action<int, int> onKill;
    public void kill(int killerID, int victimID)
    {
        if (onKill != null)
        {
            onKill(killerID, victimID);
        }
    }

    public event Action onEndTurn;
    public void EndTurn()
    {
        if (onEndTurn != null)
        {
            onEndTurn();
        }
    }

    public event Action<int> onPawnsTurn;
    public void PawnsTurn(int ID)
    {
        if (onPawnsTurn != null)
        {
            onPawnsTurn(ID);
        }
    }

    public event Action<int> onPawnWalkStart;
    public void PawnWalkStart(int ID)
    {
        if (onPawnWalkStart != null)
        {
            onPawnWalkStart(ID);
        }
    }
    public event Action<int> onPawnWalkFinish;
    public void PawnWalkFinish(int ID)
    {
        if (onPawnWalkFinish != null)
        {
            onPawnWalkFinish(ID);
        }
    }
}

