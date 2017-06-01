using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChronoBreak.StateMachine;
using System;

public class SecondaryPlayerState : PlayerState
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
            Recording recording = vcr.GetRecording();
            if (recording != null)
            {
                vcr.finishedPlayback += Vcr_finishedPlayback;
                vcr.Play();
            }
        }
    }

    private void Vcr_finishedPlayback()
    {
        controller.StopPlayback();
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        
    }

    public SecondaryPlayerState(PlayerController controller, InputVCR vcr) : base (controller, vcr)
    {

    }
}
