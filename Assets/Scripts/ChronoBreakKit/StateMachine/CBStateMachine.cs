using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak.StateMachine
{
    public class CBStateMachine
    {

        public CBState CurrentState
        {
            get { return _currentState; }
        }
        public event Action OnStateChange;
        public float TimeInState = 0f;

        private Dictionary<Type, CBState> _states = new Dictionary<Type, CBState>();
        private CBState _currentState;

        public CBStateMachine(CBState initialState)
        {
            AddState(initialState);
            _currentState = initialState;
        }


        public void Update(float deltaTime)
        {
            TimeInState += Time.deltaTime;
            _currentState.Update(deltaTime);
        }

        public void AddState(CBState newState)
        {
            newState.Initialize(this);
            _states[newState.GetType()] = newState;
        }

        public T ChangeState<T>() where T : CBState
        {
            if (_currentState.GetType() == typeof(T))
                return _currentState as T;

            if (_currentState != null)
                _currentState.EndState();

            if (_states.ContainsKey(typeof(T)))
            {
                _currentState = _states[typeof(T)];
                _currentState.StartState();
                TimeInState = 0.0f;

                if (OnStateChange != null)
                    OnStateChange.Invoke();

                return _currentState as T;
            }
            else
            {
                Debug.LogError("State not stored in _states " + typeof(T).ToString());
                return null;
            }
        }
    }
}