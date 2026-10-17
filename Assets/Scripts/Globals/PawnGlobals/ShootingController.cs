using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    [SerializeField] InputActionReference shootAction;
    [SerializeField] InputActionReference aimAction;
    [SerializeField] InputActionReference cancelAction;
    GameEvents gameEvents;
    Camera mainCamera;

    GameObject selectedUnit;
    Pawn selectedUnitData;
    List<GameObject> hittedTargets;
    List<Ray> attackRays;
    Ray ShootingRay;
    RaycastHit hitInfo;

    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
        mainCamera = Camera.main;
        
        gameEvents.onUnitSelection += UnitSelected;
        gameEvents.onUnitDeselection += UnitDeselected;

        gameEvents.onShootingModeActivated += ActivateShootingMode;
        gameEvents.onShootingModeDeactivated += DeactivateShootingMode;

        hittedTargets = new List<GameObject>();
        attackRays = new List<Ray>();

        enabled = false;
    }

    private void UnitSelected(GameObject selectedUnit)
    {
        if (selectedUnit == null)
        {
            print("Selected unit is null");
            return;
        }

        this.selectedUnit = selectedUnit;
        selectedUnitData = selectedUnit.GetComponent<Pawn>();
    }

    private void UnitDeselected(GameObject deselectedUnit)
    {
        if (deselectedUnit == null || deselectedUnit != selectedUnit)
        {
            print("Deselected unit is null or not the currently selected unit");
            return;
        }

        selectedUnit = null;
        selectedUnitData = null;
    }

    public void Update()
    {
        if (selectedUnitData == null) return;

        if (shootAction.action.WasPerformedThisFrame())
        {
            if (selectedUnitData.actionPoints <= 0)
            {
                print("No action points left");
                return;
            }

            hittedTargets.Clear();
            
            ShootingRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Debug.DrawRay(ShootingRay.origin, ShootingRay.direction * 100f, Color.red, 10f);

            int maskExceptMarker = LayerMask.GetMask("EnemyUnits", "PlayerUnits", "Objects", "Terrain");

            if (!Physics.Raycast(ShootingRay, out hitInfo, selectedUnitData.weaponInfo.attackRange, maskExceptMarker))
            {
                return;
            }
            hittedTargets.Add(hitInfo.collider.gameObject);
            
            gameEvents.AttackTrigger(selectedUnitData.id, hittedTargets);

            selectedUnitData.actionPoints--;
        }

        if (aimAction.action.IsPressed())
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 40f, Time.deltaTime * 5f);
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 80f, Time.deltaTime * 5f);
        }
        
        if (cancelAction.action.WasPerformedThisFrame())
        {
            GameManager.UIManager.ExitShootingUI();
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