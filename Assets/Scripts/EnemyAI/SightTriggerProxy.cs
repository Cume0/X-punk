using System.Collections.Generic;
using UnityEngine;

public class SightTriggerProxy : MonoBehaviour
{
    public List<GameObject> pawnsInSight;
    public List<Transform> coverPositionsInSight;

    public void Initialize()
    {
        pawnsInSight = new List<GameObject>();
        coverPositionsInSight = new List<Transform>();
    }
//
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerUnits"))
        {
            Debug.Log($"Trigger entered by: {other.gameObject.name}, layer: {LayerMask.LayerToName(other.gameObject.layer)}");
            pawnsInSight.Add(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("PossibleCover"))
        {
            Debug.Log($"Trigger entered by: {other.gameObject.name}, layer: {LayerMask.LayerToName(other.gameObject.layer)}");
            coverPositionsInSight.Add(other.gameObject.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerUnits"))
        {
            pawnsInSight.Remove(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("PossibleCover"))
        {
            coverPositionsInSight.Remove(other.gameObject.transform);
        } 
    }
}
