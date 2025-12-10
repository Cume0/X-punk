using UnityEngine;

public class Pawn : MonoBehaviour
{
    //Serialized data
    [SerializeField] private int id;
    [SerializeField] private TeamsEnum team;
    [SerializeField] private string pawnName;
    [SerializeField] public int moveSpeed;
    [SerializeField] public int movePoints;
}
