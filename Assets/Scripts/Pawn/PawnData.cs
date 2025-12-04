using UnityEngine;

public class PawnData : MonoBehaviour
{
    //Serialized data
    [SerializeField] private int id;
    [SerializeField] private string pawnName;
    [SerializeField] public int moveSpeed;
    [SerializeField] public int movePoints;
}
