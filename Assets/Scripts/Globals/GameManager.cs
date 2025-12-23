using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CombatManager combatManager;
    public MapManager mapManager;
    public Pathfinding pathfinding;
    public UnitController unitController;
    public UnitManager unitManager;
    void Awake()
    {
        combatManager = FindAnyObjectByType<CombatManager>();
        mapManager = FindAnyObjectByType<MapManager>();
        pathfinding = FindAnyObjectByType<Pathfinding>();
        unitController = FindAnyObjectByType<UnitController>();
        unitManager = FindAnyObjectByType<UnitManager>();
    }

    void Start()
    {
        combatManager.Initialize();
        mapManager.Initialize(); //Initializes all tiles
        pathfinding.Initialize();
        unitController.Initialize();
        unitManager.Initialize(); //Initializes all pawns
    }
}
