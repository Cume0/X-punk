using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private int currentTurn = 1;
    private int currentRound = 1;
    private int turnsPerRound;

    Dictionary<Pawn, int> initiativeOrder;
    List<Pawn> pawns;

    void Start()
    {
        
    }
    void EndTurn()
    {
        currentTurn++;
        
        if (currentTurn != turnsPerRound)
        {
            return;
        }

        currentTurn = 1;
        currentRound++;
        //PerRoundCooldown();
    }

    void turnOrderInit()
    {
        
    }
}
