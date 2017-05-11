using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak.StateMachine
{
    public class CBPlayState : CBState
    {
        public PlayerController primaryPlayer = null;

        public event Action OnEndPlayState;
        public event Action OnBeginPlayState;

        private List<CBEntity> entitiesTracked;
        private List<PlayerController> playersTracked = new List<PlayerController>();

        public override void EndState()
        {
            if (OnEndPlayState != null)
                OnEndPlayState.Invoke();

            CBPlayerSpawnManager.instance.RespawnPlayers();

            primaryPlayer = null;
        }

        public override void OnInitialize()
        {
            //StartState will not be called on initial state creation on game startup, so first run stuff should go here
        }

        public override void StartState()
        {
            if (OnBeginPlayState != null)
                OnBeginPlayState.Invoke();

            foreach (PlayerController pc in playersTracked)
            {
                if (pc == primaryPlayer)
                    pc.SetToPrimaryPlayer();
                else
                    pc.SetToSecondaryPlayer();
            }
        }

        public override void Update(float deltaTime)
        {

        }

        public CBPlayState(List<CBEntity> sceneObjects)
        {
            entitiesTracked = sceneObjects;

            foreach (CBEntity e in sceneObjects)
            {
                if (e is PlayerController)
                {
                    playersTracked.Add(e as PlayerController);
                }
                OnBeginPlayState += e.StartPlayState;
                OnEndPlayState += e.EndPlayState;
            }
        }
    }

}
