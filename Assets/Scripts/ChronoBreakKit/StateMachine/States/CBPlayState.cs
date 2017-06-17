using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak.StateMachine
{
    public class CBPlayState : CBState
    {
        public PlayerController primaryPlayer = null;
        public bool isSpectator = false;

        public event Action OnEndPlayState;
        public event Action OnBeginPlayState;

        private List<CBEntity> entitiesTracked;
        private List<PlayerController> playersTracked = new List<PlayerController>();

        public override void EndState()
        {
            if (OnEndPlayState != null)
                OnEndPlayState.Invoke();

            CBPlayerManager.instance.RespawnPlayers();

            primaryPlayer = null;
            isSpectator = false;
        }

        public override void OnInitialize()
        {
            if (entitiesTracked == null)
                return;

            //StartState will not be called on initial state creation on game startup, so first run stuff should go here
            foreach (CBEntity e in entitiesTracked)
            {
                if (e is PlayerController)
                {
                    playersTracked.Add(e as PlayerController);
                }
                OnBeginPlayState += e.StartPlayState;
                OnEndPlayState += e.EndPlayState;
            }
        }

        public override void StartState()
        {
            if (OnBeginPlayState != null)
                OnBeginPlayState.Invoke();

            foreach (PlayerController pc in playersTracked)
            {
                if (!isSpectator && pc == primaryPlayer)
                    pc.SetToPrimaryPlayer();
                else
                    pc.SetToSecondaryPlayer();
            }
        }

        public override void Update(float deltaTime)
        {

        }

        public void NewTrackedEntities(List<CBEntity> newSceneObjects)
        {
            if (entitiesTracked != null)
                entitiesTracked.Clear();

            OnBeginPlayState = null;
            OnEndPlayState = null;

            entitiesTracked = newSceneObjects;
            OnInitialize();
        }
    }

}
