using UnityEngine;

public class GameManager : MonoBehaviour
{
    //"Glue"
    public static GameManager current;
    public static GameEvents gameEvents;

    //Map
    public static MapManager mapManager;

    //Units
    public static CombatManager combatManager;
    public static UnitController unitController;
    public static UnitManager unitManager;
    public static TurnManager turnManager;

    //UI
    public static UIManager UIManager;

    
    void Awake()
    {
        current = this;
        gameEvents = FindAnyObjectByType<GameEvents>();

        mapManager = FindAnyObjectByType<MapManager>();

        unitManager = FindAnyObjectByType<UnitManager>();
        combatManager = FindAnyObjectByType<CombatManager>();
        unitController = FindAnyObjectByType<UnitController>();
        turnManager = FindAnyObjectByType<TurnManager>();

        UIManager = FindAnyObjectByType<UIManager>();
    }

    void Start()
    {
        gameEvents.Initialize();

        unitManager.Initialize(); //Initializes all pawns
        combatManager.Initialize();
        unitController.Initialize();
        turnManager.Initialize();
        
        UIManager.Initialize();
    }
}
