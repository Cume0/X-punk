using UnityEngine;

[CreateAssetMenu(fileName = "PawnInfo", menuName = "Scriptable Objects/Pawns/PawnInfo")]
public class PawnInfo : ScriptableObject
{
    [Header ("Identification")]
    public string pawnName;    
    public TeamsEnum team;

    [Header ("Movement")]
    public int moveSpeed;
    public int maxMovePoints;

    [Header ("Combat")]
    public int maxActionPoints;
    public int maxHP;
    public int accuracy;  
    public int defence; 
    public int evasion;
}
