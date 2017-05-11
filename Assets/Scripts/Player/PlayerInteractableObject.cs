using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChronoBreak;

public class PlayerInteractableObject : VRTK.VRTK_InteractableObject
{
    [Header("References")]
    public PlayerController controller;
    public PlayerSpawnIndicator indicator;

    [Header("Config")]
    [Tooltip("LayerMask for identifying what objects to raycast against for finding a spawnable location")]
    public LayerMask spawnZoneLayerMask;

    private bool tacticalMode = true;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        controller.OnStartTacticalState += Controller_OnStartTacticalState;
        controller.OnEndTacticalState += Controller_OnEndTacticalState;
    }

    private void Controller_OnEndTacticalState()
    {
        StopGrabbingInteractions();
        StopTouchingInteractions();
        tacticalMode = false;
    }

    private void Controller_OnStartTacticalState()
    {
        tacticalMode = true;
    }

    public override void Grabbed(GameObject currentGrabbingObject)
    {
        base.Grabbed(currentGrabbingObject);

        indicator.gameObject.SetActive(true);
    }

    public override void Ungrabbed(GameObject previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);

        indicator.gameObject.SetActive(false);

        Vector3 hitPoint;
        SpawnArea spawnArea;
        if (IsValidDropPoint(out hitPoint, out spawnArea))
        {
            CBPlayerSpawnManager.instance.UpdatePlayersSpawnPoint(controller, hitPoint, spawnArea);
        }
    }

    public override void StartTouching(GameObject currentTouchingObject)
    {
        if (!tacticalMode)
            return;

        base.StartTouching(currentTouchingObject);
    }

    public bool IsValidDropPoint()
    {
        Vector3 v;
        SpawnArea area;
        return IsValidDropPoint(out v, out area);
    }

    public bool IsValidDropPoint(out Vector3 hitPoint, out SpawnArea spawnArea)
    {
        Ray r = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 5, spawnZoneLayerMask) && hit.collider.GetComponent<SpawnArea>())
        {
            hitPoint = hit.point;
            spawnArea = hit.collider.GetComponent<SpawnArea>();
            return true;
        }
        else
        {
            hitPoint = Vector3.zero;
            spawnArea = null;
            return false;
        }
    }
}
