using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;
    public void Initialize()
    {
        current = this;
    }

    public event Action onShootingModeActivated;
    public void ShootingModeActivated()
    {
        if (onShootingModeActivated != null)
        {
            onShootingModeActivated();
        }
    }

    public event Action onShootingModeDeactivated;
    public void ShootingModeDeactivated()
    {
        if (onShootingModeDeactivated != null)
        {
            onShootingModeDeactivated();
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

    public event Action onShowTacticUI;
    public void ShowTacticUI()
    {
        if (onShowTacticUI != null)
        {
            onShowTacticUI();
        }
    }

    public event Action onHideTacticUI;
    public void HideTacticUI()
    {
        if (onHideTacticUI != null)
        {
            onHideTacticUI();
        }
    }

    public event Action onShowShootingUI;
    public void ShowShootingUI()
    {
        if (onShowShootingUI != null)
        {
            onShowShootingUI();
        }
    }

    public event Action onHideShootingUI;
    public void HideShootingUI()
    {
        if (onHideShootingUI != null)
        {
            onHideShootingUI();
        }
    }

    public event Action<Guid, List<GameObject>> onAttackTrigger;
    public void AttackTrigger(Guid AttackerID, List<GameObject> targets)
    {
        if (onAttackTrigger != null)
        {
            onAttackTrigger(AttackerID, targets);
        }
    }


    public event Action<Guid> onDeath;
    public void Die(Guid victimID)
    {
        if (onDeath != null)
        {
            onDeath(victimID);
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