using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public void skipTurn()
    {
        GameManager.gameEvents.EndTurn();

        print("Turn skipped");
    }
}
