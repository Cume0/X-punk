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


    //Shooting Mode Activation Events
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

    public event Action<GameObject> onUnitSelection;
    public void UnitSelected(GameObject selectedUnit)
    {
        if (onUnitSelection != null)
        {
            onUnitSelection(selectedUnit);
        }
    }

    public event Action<GameObject> onUnitDeselection;
    public void UnitDeselected(GameObject deselectedUnit)
    {
        if (onUnitDeselection != null)
        {
            onUnitDeselection(deselectedUnit);
        }
    }

    //Tactics UI Events
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
    //Shooting UI Events
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

    //Comabat Events
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

    //Turn Events
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

    //Pathfinding Events
    public event Action<Guid, Vector3, List<Vector3>, Transform> onShowPath;
    public void ShowPath(Guid ID, Vector3 FuturePositionCords, List<Vector3> path, Transform pawnTransform)
    {
        if (onShowPath != null)
        {
            onShowPath(ID, FuturePositionCords, path, pawnTransform);
        }
    }

    public event Action<Guid> onFreezePath;
    public void FreezePath(Guid ID)
    {
        if (onFreezePath != null)
        {
            onFreezePath(ID);
        }
    }

    public event Action<Guid> onUnfreezePath;
    public void UnfreezePath(Guid ID)
    {
        if (onUnfreezePath != null)
        {
            onUnfreezePath(ID);
        }
    }

    public event Action<Guid> onHidePath;
    public void HidePath(Guid ID)
    {
        if (onHidePath != null || ID != null)
        {
            onHidePath(ID);
            return;
        }

        print("HidePath event is null or ID is null");
    }
}