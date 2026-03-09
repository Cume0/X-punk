using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    GameEvents gameEvents;
    public Dictionary<int, Transform> pawns;

    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
        gameEvents.onKill += VictimDeletion;

        pawns = new Dictionary<int, Transform>();

        pawns.Clear();
        
        for (int pawnIndex = 0; pawnIndex < transform.childCount; pawnIndex++)
        {
            pawns.Add(pawnIndex, transform.GetChild(pawnIndex));

            pawns[pawnIndex].GetComponent<Pawn>().id = pawnIndex;

            pawns[pawnIndex].GetComponent<Pawn>().Initialize();
        }
    }

    public void VictimDeletion(int killerID, int victimID)
    {
        pawns.Remove(victimID);
        GameManager.turnManager.initiativeOrder.Remove(pawns[victimID]); //
    }
}



