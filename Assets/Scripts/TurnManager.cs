using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public int CurrentRound = 1;
    public int CurrentTurn = 1;
    public int TurnsPerRound;
    
    List<Pawn> PlayerPawns;
    List<Pawn> NonPlayerPawns;

    public void Start()
    {
        
    }
    public void EndTurn()
    {
        CurrentTurn++;
        TurnsPerRound = PlayerPawns.Count + NonPlayerPawns.Count;

        if (CurrentTurn != TurnsPerRound) { return; }


    }
}
