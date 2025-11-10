using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UnitController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] public float jumpHight = 1f;
    [SerializeField] InputActionReference selectAction;
    Transform selectedUnit;

    bool unitSelected = false;

    MapManager mapManager;
    Pathfinding pathfinding;
    private List<Node> path;
    void Start()
    {
        mapManager = FindFirstObjectByType<MapManager>();

        pathfinding = GetComponent<Pathfinding>();
    }

    void Update()
    {
        UnitMovementHandler();

        if (path != null && path.Count > 0)
        {
            MoveAlongPath();
        }
    }
    
    void UnitMovementHandler()
    {
        if (selectAction.action.WasPerformedThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            bool hasHit = Physics.Raycast(ray, out hit);

            if (hasHit)
            {
                if (hit.transform.tag == "Tile")
                {
                    if (unitSelected)
                    {
                        Vector3 pos = selectedUnit.position / mapManager.UnityMapSize;
                        Vector3Int startCords = Vector3Int.RoundToInt(pos);
                        Node startNode = mapManager.GetNode(startCords);

                        Vector3Int targetCords = hit.transform.GetComponent<Labeler>().Cords;
                        Node endNode = mapManager.GetNode(targetCords);

                        path = pathfinding.FindPath(startNode, endNode);

                        unitSelected = false;
                    }
                }

                if (hit.transform.tag == "Unit")
                {
                    selectedUnit = hit.transform;
                    unitSelected = true;
                }
            }
        }
    }

    private void MoveAlongPath()
    {
        Vector3 targetPos = new Vector3(
        path[0].Cords.x * mapManager.UnityMapSize,
        path[0].Cords.y * mapManager.UnityMapSize,
        path[0].Cords.z * mapManager.UnityMapSize
        );

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            path.RemoveAt(0);
        }
    }
}
