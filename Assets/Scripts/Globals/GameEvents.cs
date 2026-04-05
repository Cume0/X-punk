using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public void Initialize()
    {
        
    }

    public event Action<Guid, Guid> onAttackTrigger;
    public void AttackTrigger(Guid AttackerID, Guid DefenderID)
    {
        if (onAttackTrigger != null)
        {
            onAttackTrigger(AttackerID, DefenderID);
        }
    }

    public event Action<bool, GameObject> onUnitSelection;
    public void UnitSelected(bool isUnitSelected, GameObject selectedUnit)
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

    public event Action<Guid, Guid> onKill;
    public void kill(Guid killerID, Guid victimID)
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

    public event Action<Guid> onPawnsTurn;
    public void PawnsTurn(Guid ID)
    {
        if (onPawnsTurn != null)
        {
            onPawnsTurn(ID);
        }
    }

    public event Action<Guid> onPawnWalkStart;
    public void PawnWalkStart(Guid ID)
    {
        if (onPawnWalkStart != null)
        {
            onPawnWalkStart(ID);
        }
    }
    public event Action<Guid> onPawnWalkFinish;
    public void PawnWalkFinish(Guid ID)
    {
        if (onPawnWalkFinish != null)
        {
            onPawnWalkFinish(ID);
        }
    }
}

