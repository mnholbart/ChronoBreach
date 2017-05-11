using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The EquippableItem script handles any item that can be found in the world, picked up, and holstered to a player inventory slot
/// </summary>
/// <remarks>
/// Only handles the logic of equipping and unequipping an object, not its use
/// </remarks>
/// <example>
/// See Gun.cs for an implementation as a firearm
/// </example>
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

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        interactableObject.InteractableObjectUngrabbed += OnUngrabbed;
        interactableObject.InteractableObjectGrabbed += OnGrabbed;
        
        colliders = new List<Collider>(gameObject.GetComponentsInChildren<Collider>());

        parent = transform.parent;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected virtual void OnUngrabbed(object sender, VRTK.InteractableObjectEventArgs args)
    {
        if (touchedHolster != null)
        {
            Equip();
        }
        else Unequip();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected virtual void OnGrabbed(object sender, VRTK.InteractableObjectEventArgs args)
    {
        if (equippedHolster != null)
        {
            Unequip();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        InventoryHolster holster = other.GetComponent<InventoryHolster>();
        if (holster != null)
        {
            touchedHolster = holster;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        InventoryHolster holster = other.GetComponent<InventoryHolster>();
        if (holster != null && holster == touchedHolster)
        {
            touchedHolster = null;
        }
    }

    /// <summary>
    /// Unequip the item and set its state back to the default state
    /// </summary>
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

    /// <summary>
    /// Equip an item to a holster inventory slot
    /// </summary>
    /// <param name="forced">If true will equip to a forced slot rather than the currently touched slot</param>
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
    
    /// <summary>
    /// Forces the item to be equipped in a holster
    /// </summary>
    /// <param name="holster"></param>
    public void ForceEquipToSlot(InventoryHolster holster)
    {
        Unequip();
        equippedHolster = holster;
        Equip(true);
    }

    /// <summary>
    /// Reset an objects state to its default
    /// </summary>
    protected override void ResetObject()
    {
        base.ResetObject();

        if (spawnHolster != null)
            ForceEquipToSlot(spawnHolster);
        else 
            Unequip();
    }
}
