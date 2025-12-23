using UnityEngine;

public class CombatManager : MonoBehaviour
{
    GameManager gameManager;
    Transform selectedUnit;

    public void Initialize()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
}
