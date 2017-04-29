using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : EquippableItem {

    [Header("Config")]
    public int maxAmmo;

    private int ammoRemaining;

    protected override void Awake()
    {
        base.Awake();

        interactableObject.InteractableObjectUsed += OnUsed;
        RefillAmmo();
    }

    private void OnUsed(object sender, VRTK.InteractableObjectEventArgs args)
    {
        if (CanShoot())
        {
            Shoot();
        }
    }

    public bool CanShoot()
    {
        if (ammoRemaining <= 0)
            return false;

        return true;
    }

    private void Shoot()
    {

    }

    private void RefillAmmo()
    {
        ammoRemaining = maxAmmo;
    }

    protected override void ResetObject()
    {
        base.ResetObject();

        RefillAmmo();
    }
}
