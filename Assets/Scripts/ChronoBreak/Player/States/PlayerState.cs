using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChronoBreak.StateMachine;
using System;

public abstract class PlayerState : CBState
{
    protected InputVCR vcr;
    protected PlayerController controller;

    public override void EndState()
    {
        if (vcr != null)
        {
            vcr.Stop();
        }
    }

    public override void OnInitialize()
    {

    }

    public override void StartState()
    {

    }

    public override void Update(float deltaTime)
    {

    }    

    public PlayerState(PlayerController player, InputVCR newVCR)
    {
        vcr = newVCR;
        controller = player;
    }
}
