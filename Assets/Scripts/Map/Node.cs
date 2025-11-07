using UnityEngine;

public class Node
{
    public Vector3Int Cords;
    public int G; //Distance to startPoint
    public int H; //Distance to endPoint

    public int F { get { return G + H; } }

    public Node previous;

    public bool Blocked;

    public Node (Vector3Int cords, bool blocked)
    {
        this.Cords = cords;
        this.Blocked = blocked;
    }
}
