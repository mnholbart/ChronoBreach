using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : BaseControl {

    public enum DoorState
    {
        Locked, //Door is locked and wont open until unlocked
        Unlocked, //Door is unlocked and can be opened 
        UnlockedStuck //Door is unlocked but something is preventing it from opening
    }
    public DoorState doorState = DoorState.Unlocked;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void ResetControl()
    {

    }
}
