using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    [SerializeField] InputActionReference shootAction;
    [SerializeField] InputActionReference aimAction;
    
    GameEvents gameEvents;

    GameObject selectedUnit;
    Pawn selectedUnitData;
    List<GameObject> hittedTargets;

    Ray ShootingRay;
    RaycastHit hitInfo;

    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;

        gameEvents.onUnitSelection += UnitSelected;
        gameEvents.onShootingModeActivated += ActivateShootingMode;
        gameEvents.onShootingModeDeactivated += DeactivateShootingMode;

        hittedTargets = new List<GameObject>();

        enabled = false;
    }

    public void Update()
    {
        if (shootAction.action.WasPerformedThisFrame())
        {
            hittedTargets.Clear();

            ShootingRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Debug.DrawRay(ShootingRay.origin, ShootingRay.direction * 100f, Color.red, 10f);

            if (!Physics.Raycast(ShootingRay, out hitInfo))
            {
                return;
            }
            hittedTargets.Add(hitInfo.collider.gameObject);
            
            gameEvents.AttackTrigger(selectedUnitData.id, hittedTargets);
            
        }

        if (aimAction.action.IsPressed())
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 40f, Time.deltaTime * 5f);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 80f, Time.deltaTime * 5f);
        }
    }

    private void UnitSelected(bool isUnitSelected, GameObject selectedUnit)
    {
        if (isUnitSelected)
        {
            this.selectedUnit = selectedUnit;
            selectedUnitData = selectedUnit.GetComponent<Pawn>();
        }
        else
        {
            this.selectedUnit = null;
            selectedUnitData = null;
        }
    }

    private void ActivateShootingMode()
    {
        enabled = true;
    }

    private void DeactivateShootingMode()
    {
        enabled = false;
    }
}