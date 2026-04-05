using System;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] EnemyButton enemyButton;
    [SerializeField] Transform enemyBar;

    UnitManager unitManager;
    GameEvents gameEvents;
    MapManager mapManager;
    TurnManager turnManager;

    Pawn thisUnitData;
    bool isThisUnitSelected;

    public void Initialize()
    {
        unitManager = GameManager.unitManager;
        gameEvents = GameManager.gameEvents;
        mapManager = GameManager.mapManager;
        turnManager = GameManager.turnManager;

        gameEvents.onUnitSelection += UnitSelection;
        gameEvents.onAttackTrigger += UnitAttaking;
    }
    
    public void UnitSelection(bool isUnitSelected, GameObject selectedUnit)
    {
        thisUnitData = selectedUnit.GetComponent<Pawn>();

        isThisUnitSelected = isUnitSelected;
    }

    public void UnitAttaking(Guid givenAttackerID, Guid givenDefenderID)
    {
        if (thisUnitData == null || unitManager.pawns[givenDefenderID] == null) return;
        
        if (givenAttackerID == thisUnitData.id && isThisUnitSelected)
        {
            Pawn attackerData = unitManager.pawns[givenAttackerID].GetComponent<Pawn>();
            Pawn defenderData = unitManager.pawns[givenDefenderID].GetComponent<Pawn>();

            if (attackerData.actionPoints < 0)
            {
                return;
            }

            unitManager.pawns[givenAttackerID].GetComponent<Pawn>().LookAtTarget(unitManager.pawns[givenDefenderID].GetComponent<Pawn>().cords);

            if (!calculateAttack(attackerData.accuracy, defenderData.defence))
            {
                return;
            }

            defenderData.HP--;
            attackerData.actionPoints--;

            print("HP of " + givenDefenderID + " is " + defenderData.HP);

            if (defenderData.HP <= 0)
            {
                Destroy(unitManager.pawns[givenDefenderID].gameObject);

                turnManager.initiativeOrder.Remove(unitManager.pawns[givenDefenderID]);
                
                gameEvents.kill(givenAttackerID, givenDefenderID);
            }
        }
    }

    public bool calculateAttack(int Accuracy, int Defence)
    {
        if (UnityEngine.Random.Range(1, 101) + Accuracy < Defence)
        {
            print("Miss");
            return false;
        }
        
        return true;
    }
}
