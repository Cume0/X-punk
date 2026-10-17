using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct InitiativeData
{
    public Guid id;
    public int initiative;
    public GameObject unit;

    public InitiativeData(Guid id, int initiative, GameObject unit)
    {
        this.id = id;
        this.initiative = initiative;
        this.unit = unit;
    }
}
//TO DO: Add check for simutaneous initiative values, and add a tie-breaker system (maybe based on unit speed or something)
public class TurnManager : MonoBehaviour
{
    public int currentTurn = 0;
    public int currentRound = 0;
    Guid currentActivePawnId;
    public List<InitiativeData> initiativeOrder;

    GameEvents gameEvents;
    UnitManager unitManager;
    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
        unitManager = GameManager.unitManager;

        gameEvents.onEndTurn += turnShift;

        initiativeOrder = new List<InitiativeData>();
        foreach (GameObject unit in unitManager.pawns.Values)
        {
            initiativeOrder.Add(new InitiativeData(unit.GetComponent<Pawn>().id, unit.GetComponent<Pawn>().getInitiative(), unit));
            print(unit.GetComponent<Pawn>().name + " " + initiativeOrder[initiativeOrder.Count - 1].initiative);
        }

        currentActivePawnId = initiativeOrder.OrderByDescending(n => n.initiative).Select(n => n.id).First();
        unitManager.pawns[currentActivePawnId].GetComponent<Pawn>().state = StatesEnum.Active;
        gameEvents.PawnsTurn(currentActivePawnId);
    }

    public void turnShift()
    {
        StartCoroutine(turnShiftRoutine());
    }

    private IEnumerator turnShiftRoutine()
    {
        currentTurn++;

        if (currentTurn >= initiativeOrder.Count)
        {
            currentRound++;
            currentTurn = 0;
        }

        if (unitManager.pawns.ContainsKey(currentActivePawnId))
        {
            yield return StartCoroutine(unitManager.pawns[currentActivePawnId].GetComponent<Pawn>().EndTurn());
        }

        currentActivePawnId = initiativeOrder.ElementAt(currentTurn).id;
        gameEvents.PawnsTurn(currentActivePawnId);
    }
}
