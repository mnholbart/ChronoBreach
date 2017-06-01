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
        private List<GameObject> geometryTracked;
        private List<GameObject> headquarterObjects;

        public override void EndState()
        {
            if (OnEndTacticalState != null)
                OnEndTacticalState.Invoke();

            foreach (GameObject g in geometryTracked)
            {
                g.SetActive(true);
            }

            foreach (GameObject g in headquarterObjects)
            {
                g.SetActive(false);
            }
        }

        public override void OnInitialize()
        {
            //StartState will not be called on initial state creation on game startup, so first run stuff should go here
            if (entitiesTracked == null)
                return;

            foreach (CBEntity e in entitiesTracked)
            {
                OnBeginTacticalState += e.StartTacticalState;
                OnEndTacticalState += e.EndTacticalState;
            }

            OnBeginTacticalState += CBPlayerManager.instance.RespawnPlayers;
        }

        public override void StartState()
        {
            if (OnBeginTacticalState != null)
                OnBeginTacticalState.Invoke();

            foreach (GameObject g in geometryTracked)
            {
                g.SetActive(false);
            }

            foreach (GameObject g in headquarterObjects)
            {
                g.SetActive(true);
            }
        }

        public override void Update(float deltaTime)
        {

        }

        public CBTacticalState(List<GameObject> hqObjects)
        {
            headquarterObjects = hqObjects;
        }

        public void NewTrackedEntities(List<CBEntity> newSceneObjects, List<GameObject> geometry)
        {
            if (entitiesTracked != null)
            {
                foreach (CBEntity e in entitiesTracked)
                {
                    OnBeginTacticalState -= e.StartTacticalState;
                    OnEndTacticalState -= e.EndTacticalState;
                }
                entitiesTracked.Clear();
            }

            if (geometryTracked != null)
            {
                geometryTracked.Clear();
            }

            entitiesTracked = newSceneObjects;
            geometryTracked = geometry;
            OnInitialize();
        }
    }

}
