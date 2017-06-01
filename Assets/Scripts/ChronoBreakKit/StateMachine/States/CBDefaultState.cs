using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ChronoBreak.StateMachine
{
    public class CBDefaultState : CBState
    {
        public event Action OnEndDefaultState;
        public event Action OnBeginDefaultState;

        public override void EndState()
        {
            if (OnEndDefaultState != null)
                OnEndDefaultState.Invoke();
        }

        public override void OnInitialize()
        {
            //StartState will not be called on initial state creation on game startup, so first run stuff should go here
            if (OnBeginDefaultState != null)
                OnBeginDefaultState.Invoke();
        }

        public override void StartState()
        {
            if (OnBeginDefaultState != null)
                OnBeginDefaultState.Invoke();
        }

        public override void Update(float deltaTime)
        {

        }
    }
}
