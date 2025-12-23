using UnityEngine;

[ExecuteAlways]
public class Tile : MonoBehaviour 
{
    [SerializeField] public bool blockNode = false;
    [SerializeField] public int costToMove = 10;

    //Pathfinding data
    public Vector3Int Cords;
    public int G; //Distance to startPoint
    public int H; //Distance to endPoint

    public int F { get { return G + H; } }

    public Tile previous;
    public TileOverlay tileOverlay;

    public bool Blocked;

    public void Initialize(Vector3Int cords, bool blocked)
    {
        Cords = cords;
        Blocked = blocked;

        GetComponent<Labeler>().Initialize();

        tileOverlay = GetComponentInChildren<TileOverlay>();

        tileOverlay.Initialize();
    }
}