using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private int currentTurn = 1;

    void EndTurn()
    {
        currentTurn++;
    }
}
