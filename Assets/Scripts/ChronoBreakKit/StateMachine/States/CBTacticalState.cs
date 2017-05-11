using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak.StateMachine
{
    public class CBTacticalState : CBState
    {
        public event Action OnEndTacticalState;
        public event Action OnBeginTacticalState;

        private List<CBEntity> entitiesTracked;

        public override void EndState()
        {
            if (OnEndTacticalState != null)
                OnEndTacticalState.Invoke();
        }

        public override void OnInitialize()
        {
            //StartState will not be called on initial state creation on game startup, so first run stuff should go here
            foreach (CBEntity e in entitiesTracked)
            {
                OnBeginTacticalState += e.StartTacticalState;
                OnEndTacticalState += e.EndTacticalState;
            }

            OnBeginTacticalState += CBPlayerSpawnManager.instance.RespawnPlayers;

            if (OnBeginTacticalState != null)
                OnBeginTacticalState.Invoke();
        }

        public override void StartState()
        {
            if (OnBeginTacticalState != null)
                OnBeginTacticalState.Invoke();
        }

        public override void Update(float deltaTime)
        {

        }

        public CBTacticalState(List<CBEntity> sceneObjects)
        {
            entitiesTracked = sceneObjects;
        }
    }

}
