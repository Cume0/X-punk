using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponInfo", menuName = "Scriptable Objects/WeaponInfo")]
public class WeaponInfo : ScriptableObject
{
    [Header("Identification")]
    public string weaponName;
    public Guid assignedPawnID;

    [Header("Combat")]
    public int attackDamage;
    public int accuracy;
}
