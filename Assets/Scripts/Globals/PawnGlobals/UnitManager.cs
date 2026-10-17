using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    GameEvents gameEvents;
    TurnManager turnManager;
    public Dictionary<Guid, GameObject> pawns;

    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
        turnManager = GameManager.turnManager;

        gameEvents.onDeath += VictimDeletion;

        pawns = new Dictionary<Guid, GameObject>();

        pawns.Clear();
        
        for (int pawnIndex = 0; pawnIndex < transform.childCount; pawnIndex++)
        {
            transform.GetChild(pawnIndex).GetComponent<Pawn>().Initialize();

            pawns.Add(transform.GetChild(pawnIndex).GetComponent<Pawn>().id, transform.GetChild(pawnIndex).gameObject);
        }
    }

    public void VictimDeletion(Guid victimID)
    {  
        int victimIndex = turnManager.initiativeOrder.FindIndex(x => x.id == victimID);

        if (victimIndex < turnManager.currentTurn)
        {
            turnManager.currentTurn--;
        }

        turnManager.initiativeOrder.Remove(turnManager.initiativeOrder.FirstOrDefault(x => x.id == victimID));
        pawns.Remove(victimID);
    }

    public bool getPawnByID(Guid pawnID, out GameObject pawn)
    {
        return pawns.TryGetValue(pawnID, out pawn);
    }
}