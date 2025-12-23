using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<Transform> pawns;

    public void Initialize()
    {
        for (int pawnIndex = 0; pawnIndex < transform.childCount; pawnIndex++)
        {
            pawns.Add(transform.GetChild(pawnIndex));

            pawns[pawnIndex].GetComponent<Pawn>().id = pawnIndex;

            pawns[pawnIndex].GetComponent<Pawn>().Initialize();
        }
    }
}
