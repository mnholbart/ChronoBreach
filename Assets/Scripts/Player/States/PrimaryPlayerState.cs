using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChronoBreak.StateMachine;
using System;

public class PrimaryPlayerState : PlayerState
{
    public override void EndState()
    {
        base.EndState();
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
    }

    public override void StartState()
    {
        base.StartState();

        if (vcr != null)
        {
            vcr.NewRecording();
        }
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }

    public PrimaryPlayerState(PlayerController controller, InputVCR vcr) : base(controller, vcr)
    {

    }
}
