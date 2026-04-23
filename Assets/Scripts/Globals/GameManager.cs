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
    public static ShootingController shootingController;
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
        turnManager = FindAnyObjectByType<TurnManager>();

        unitController = FindAnyObjectByType<UnitController>();
        shootingController = FindAnyObjectByType<ShootingController>();

        UIManager = FindAnyObjectByType<UIManager>();

        //Initialize all managers and controllers
        gameEvents.Initialize();

        unitManager.Initialize(); //Initializes all pawns
        combatManager.Initialize();
        turnManager.Initialize();

        unitController.Initialize();
        shootingController.Initialize();
        
        UIManager.Initialize();
    }
}
