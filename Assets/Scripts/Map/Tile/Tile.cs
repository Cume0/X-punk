using UnityEngine;

[ExecuteAlways]
public class Tile : MonoBehaviour //class to change tile properties in inspector
{
    [SerializeField] public bool blockNode = false;
    [SerializeField] public int costToMove = 10;
}