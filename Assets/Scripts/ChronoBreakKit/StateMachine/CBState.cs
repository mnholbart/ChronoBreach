using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak.StateMachine
{
    public abstract class CBState
    {
        private CBStateMachine _machine;


        public abstract void Update(float deltaTime);


        public abstract void StartState();


        public abstract void EndState();


        public virtual void Initialize(CBStateMachine machine)
        {
            _machine = machine;
            OnInitialize();
        }

        public abstract void OnInitialize();
    }
}