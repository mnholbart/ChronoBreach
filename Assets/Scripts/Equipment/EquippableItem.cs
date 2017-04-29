using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquippableItem : TrackedObject {

    /// <summary>
    /// What type of equipment slot does this item go into
    /// </summary>
    /// <param name="Unequippable">Doesn't fit into a slot, everything should be equippable as an EquippableItem, but this flag would be used for a live grenade or something</param>
    /// <param name="Large">Takes up a large equipment slot for something like a gun</param>
    /// <param name="Small">Takes up a small equipment slot for something like a flashbang</param>
    public enum EquipSlotType
    {
        Unequippable,
        Large,
        Small
    }

    [Header("References")]
    public VRTK.VRTK_InteractableObject interactableObject;
    public Rigidbody rigidbody;

    private InventoryHolster equippedHolster;
    private InventoryHolster touchedHolster;
    private List<Collider> colliders;
    private Transform parent;
    [SerializeField]
    private InventoryHolster spawnHolster;

    protected override void Awake()
    {
        base.Awake();

        interactableObject.InteractableObjectUngrabbed += OnUngrabbed;
        interactableObject.InteractableObjectGrabbed += OnGrabbed;
        
        colliders = new List<Collider>(gameObject.GetComponentsInChildren<Collider>());

        parent = transform.parent;
    }

    protected virtual void OnUngrabbed(object sender, VRTK.InteractableObjectEventArgs args)
    {
        if (touchedHolster != null)
        {
            Equip();
        }
        else Unequip();
    }

    protected virtual void OnGrabbed(object sender, VRTK.InteractableObjectEventArgs args)
    {
        if (equippedHolster != null)
        {
            Unequip();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InventoryHolster holster = other.GetComponent<InventoryHolster>();
        if (holster != null)
        {
            touchedHolster = holster;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InventoryHolster holster = other.GetComponent<InventoryHolster>();
        if (holster != null && holster == touchedHolster)
        {
            touchedHolster = null;
        }
    }

    private void Unequip()
    {
        equippedHolster = null;
        transform.SetParent(parent, true);
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;

        foreach (Collider c in colliders)
        {
            c.isTrigger = false;
        }
    }

    private void Equip(bool forced = false)
    {
        equippedHolster = forced ? spawnHolster : touchedHolster;
        transform.position = equippedHolster.transform.position;
        transform.SetParent(equippedHolster.transform, true);
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;

        foreach (Collider c in colliders)
        {
            c.isTrigger = true;
        }
    }
    
    public void ForceEquipToSlot(InventoryHolster holster)
    {
        Unequip();
        equippedHolster = holster;
        Equip(true);
    }

    protected override void ResetObject()
    {
        base.ResetObject();

        if (spawnHolster != null)
            ForceEquipToSlot(spawnHolster);
        else 
            Unequip();
    }
}
