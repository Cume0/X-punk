using UnityEngine;

[CreateAssetMenu(fileName = "PawnInfo", menuName = "Scriptable Objects/Pawns/PawnInfo")]
public class PawnInfo : ScriptableObject
{
    [Header ("Identification")]
    public string basePawnName;    
    public TeamsEnum team;

    [Header ("Movement")]
    public int baseMoveSpeed;
    public int baseMovePoints;

    [Header ("Combat")]
    public int baseActionPoints;
    public int baseMaxHealth;
    public int baseCurrentHealth;
    public int baseAccuracy;  
    public int baseDefence; 
    public int baseEvasion;
}
