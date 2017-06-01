using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChronoBreak;
using ChronoBreak.StateMachine;
using System;

public class PlayerController : CBEntity
{
    [Header("References")]
    public InputVCR inputVCR;
    public VRTK.VRTK_InteractableObject interactableObject;

    private CBStateMachine stateMachine;

    public void OnEnable()
    {
    }

    protected override void Awake()
    {
        base.Awake();

        SetupStateMachine();

        SetToIdlePlayer();
    }

    protected void Start()
    {

    }

    protected void Update()
    {
        if (stateMachine != null)
            stateMachine.Update(Time.deltaTime);
    }

    protected override void ResetObjectToDefault()
    {

    }

    protected override void SaveDefaultProperties()
    {
        
    }

    public override void StartTacticalState()
    {
        base.StartTacticalState();

        interactableObject.enabled = true;
    }

    public override void EndTacticalState()
    {
        base.EndTacticalState();

        interactableObject.enabled = false;
    }

    public void SetToPrimaryPlayer()
    {
        stateMachine.ChangeState<PrimaryPlayerState>();
    }

    public void SetToSecondaryPlayer()
    {
        stateMachine.ChangeState<SecondaryPlayerState>();
    }

    public void SetToIdlePlayer()
    {
        stateMachine.ChangeState<IdlePlayerState>();
    }

    private void SetupStateMachine()
    {
        IdlePlayerState idleState = new IdlePlayerState(this, inputVCR);
        PrimaryPlayerState primaryState = new PrimaryPlayerState(this, inputVCR);
        SecondaryPlayerState secondaryState = new SecondaryPlayerState(this, inputVCR);

        stateMachine = new CBStateMachine(idleState);
        stateMachine.AddState(primaryState);
        stateMachine.AddState(secondaryState);
    }

    public void Respawn(Vector3 pos)
    {
        SetToIdlePlayer();
        transform.position = pos + new Vector3(0, transform.localScale.y, 0);
        transform.rotation = Quaternion.identity;
    }

    public void StopPlayback()
    {
        stateMachine.ChangeState<IdlePlayerState>();
    }
}
