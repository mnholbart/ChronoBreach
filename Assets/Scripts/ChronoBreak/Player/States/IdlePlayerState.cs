using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChronoBreak.StateMachine;
using System;

public class IdlePlayerState : PlayerState
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
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }

    public IdlePlayerState(PlayerController controller, InputVCR vcr) : base(controller, vcr)
    {
        
    }
}
