using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponInfo", menuName = "Scriptable Objects/WeaponInfo")]
public class WeaponInfo : ScriptableObject
{
    [Header("View")]
    public string weaponName;
    public GameObject weaponPrefab;
    public AudioClip shootSoundl;

    [Header("Stats")]
    public int attackDamage;
    public int shotsPerAction;

    public int maxDispersionAngle;
    public int minDispersionAngle;

    public int attackRange;
    public int armorPenetration;
}
