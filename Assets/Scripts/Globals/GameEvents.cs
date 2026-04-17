using System;
using System.Collections.Generic;
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

    public event Action onEndTurnButtonPressed;
    public void EndTurn()
    {
        if (onEndTurnButtonPressed != null)
        {
            onEndTurnButtonPressed();
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

    public event Action<Guid, Vector3, List<Vector3>> onShowPath;
    public void ShowPath(Guid ID, Vector3 FuturePositionCords, List<Vector3> path)
    {
        if (onShowPath != null)
        {
            onShowPath(ID, FuturePositionCords, path);
        }
    }

    public event Action<Guid> onHidePath;
    public void HidePath(Guid ID)
    {
        if (onHidePath != null)
        {
            onHidePath(ID);
        }
    }
}