using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] EnemyButton enemyButton;
    [SerializeField] Transform enemyBar;

    UnitManager unitManager;
    GameEvents gameEvents;
    TurnManager turnManager;
    Pawn thisUnitData;

    public void Initialize()
    {
        unitManager = GameManager.unitManager;
        gameEvents = GameManager.gameEvents;
        turnManager = GameManager.turnManager;

        gameEvents.onUnitSelection += UnitSelection;
        gameEvents.onAttackTrigger += AttackCalculation;
    }
    
    public void UnitSelection(bool isUnitSelected, GameObject selectedUnit)
    {
        thisUnitData = selectedUnit.GetComponent<Pawn>();
    }

    public void AttackCalculation(Guid givenAttackerID, List<GameObject> hittedTargets)
    {
        thisUnitData = unitManager.pawns[givenAttackerID].GetComponent<Pawn>();

        foreach (GameObject target in hittedTargets)
        {
            if (target.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(thisUnitData.attackDamage);
            }
        }
    }
}
