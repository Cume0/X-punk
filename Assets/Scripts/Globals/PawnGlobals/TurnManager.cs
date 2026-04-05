using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public int currentTurn = 0;
    public int currentRound = 0;
    GameObject currentActivePawn;
    public Dictionary<GameObject, int> initiativeOrder;

    GameEvents gameEvents;
    UnitManager unitManager;
    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
        unitManager = GameManager.unitManager;

        gameEvents.onEndTurn += EndTurn;

        initiativeOrder = new Dictionary<GameObject, int>();
        foreach (GameObject unit in unitManager.pawns.Values)
        {
            initiativeOrder.Add(unit, unit.GetComponent<Pawn>().getInitiative());
            print(unit.GetComponent<Pawn>().name + " " + initiativeOrder[unit]);
        }

        currentActivePawn = initiativeOrder.OrderBy(n => n.Value).First().Key;
        currentActivePawn.GetComponent<Pawn>().state = StatesEnum.Active;
        gameEvents.PawnsTurn(currentActivePawn.GetComponent<Pawn>().id);
    }

    public void EndTurn()
    {
        gameEvents.HideBattleUI();

        currentTurn++;

        if (currentTurn >= initiativeOrder.Count)
        {
            currentRound++;
            currentTurn = 0;
            print("Round " + currentRound);
        }
        print("turn " + currentTurn);

        currentActivePawn.GetComponent<Pawn>().state = StatesEnum.InActive;
        currentActivePawn = initiativeOrder.OrderBy(n => n.Value).Select(n => n.Key).ElementAt(currentTurn);;

        gameEvents.PawnsTurn(currentActivePawn.GetComponent<Pawn>().id);
    }
}
