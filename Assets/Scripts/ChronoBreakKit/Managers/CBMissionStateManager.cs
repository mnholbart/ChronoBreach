using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ChronoBreak.StateMachine;

namespace ChronoBreak
{
    public sealed class CBMissionStateManager : MonoBehaviour
    {
        public static CBMissionStateManager instance;

        [HideInInspector] public bool initialized = false;

        private CBStateMachine stateMachine;
        private CBDefaultState defaultState;
        private CBTacticalState tacticalState;
        private CBPlayState playState;
        private List<GameObject> headquarterObjects;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        private IEnumerator Start()
        {
            CBGameManager.instance.InitializeScene += CBGameManager_InitializeScene;
            CBSceneManager.instance.OnMissionLoaded += CBSceneManager_OnMissionLoaded;

            yield return null;

            headquarterObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("HeadquarterGeometry"));


            initialized = true;
        }

        private void Update()
        {
            if (stateMachine != null && stateMachine.CurrentState != null)
            {
                stateMachine.Update(Time.deltaTime);

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    StartPlayMode(0);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    StartPlayMode(1);
                }

                if (stateMachine.CurrentState is CBPlayState && Input.GetKeyDown(KeyCode.Escape))
                {
                    stateMachine.ChangeState<CBTacticalState>();
                }
            }
        }

        public void StartPlayMode(int playerIndex)
        {
            if (stateMachine.CurrentState is CBTacticalState )
            {
                PlayerController pc = CBPlayerManager.instance.GetPlayerAtIndex(playerIndex);
                playState.primaryPlayer = pc;
                stateMachine.ChangeState<CBPlayState>();
            }
        }

        private void CBGameManager_InitializeScene()
        { 
            defaultState = new CBDefaultState();
            tacticalState = new CBTacticalState(headquarterObjects);
            playState = new CBPlayState();
            
            stateMachine = new CBStateMachine(defaultState);
            stateMachine.AddState(tacticalState);
            stateMachine.AddState(playState);
        }

        private void CBSceneManager_OnMissionLoaded(SceneData data, List<CBEntity> entities, List<GameObject> geometry)
        {
            List<CBEntity> trackedEntities = new List<CBEntity>(entities);

            tacticalState.NewTrackedEntities(trackedEntities, geometry);
            playState.NewTrackedEntities(trackedEntities);

            stateMachine.ChangeState<CBTacticalState>();
        }
    }
}