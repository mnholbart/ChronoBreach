using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Gun script acts as an abstract class for any fierarm to handle its state, ammunition, and input functionality
/// </summary>
/// <remarks>
/// Abstract class, extend into specific gun functionality
/// </remarks>
/// <example>
/// See PistolType1.cs
/// </example>
public abstract class Gun : EquippableItem {

    [Header("Config")]
    public int maxAmmo;

    protected int ammoRemaining;

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        interactableObject.InteractableObjectUsed += OnUsed;
        RefillAmmo();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnUsed(object sender, VRTK.InteractableObjectEventArgs args)
    {
        if (CanShoot())
        {
            Shoot();
        }
    }

    /// <summary>
    /// Check if the gun has any restrictions on shooting
    /// </summary>
    /// <returns>If gun can shoot</returns>
    public bool CanShoot()
    {
        if (ammoRemaining <= 0)
            return false;

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// todo: might just need to be abstract 
    /// </remarks>
    protected virtual void Shoot()
    {

    }

    /// <summary>
    /// Refill the guns ammunition and handle any ammo UI or hud resets
    /// </summary>
    private void RefillAmmo()
    {
        ammoRemaining = maxAmmo;
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void ResetObject()
    {
        base.ResetObject();

        RefillAmmo();
    }

}
