using System;
using System.Collections.Generic;
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
        turnManager.initiativeOrder.Remove(pawns[victimID]);
        pawns.Remove(victimID);
    }
}


