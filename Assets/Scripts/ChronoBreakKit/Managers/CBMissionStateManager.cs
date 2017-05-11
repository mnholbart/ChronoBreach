using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChronoBreak.StateMachine;

namespace ChronoBreak
{
    public sealed class CBMissionStateManager : MonoBehaviour
    {
        public static CBMissionStateManager instance;

        [HideInInspector] public bool initialized = false;

        private CBStateMachine stateMachine;
        private CBTacticalState tacticalState;
        private CBPlayState playState;

        private void Awake()
        {
            
        }

        private void Start()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            CBGameManager.instance.InitializeScene += GameManager_InitializeScene;

            initialized = true;
        }

        private void Update()
        {
            if (stateMachine != null)
            {
                stateMachine.Update(Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartPlayMode(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartPlayMode(1);
            }

            if (stateMachine != null && stateMachine.CurrentState is CBPlayState && Input.GetKeyDown(KeyCode.Escape))
            {
                stateMachine.ChangeState<CBTacticalState>();
            }
        }

        public void StartPlayMode(int playerIndex)
        {
            if (!(stateMachine.CurrentState is CBPlayState))
            {
                PlayerController pc = CBPlayerSpawnManager.instance.GetPlayerAtIndex(playerIndex);
                playState.primaryPlayer = pc;
                stateMachine.ChangeState<CBPlayState>();
            }
        }

        private void GameManager_InitializeScene()
        {
            if (CBSceneManager.instance.LoadedSceneType == CBSceneManager.SceneType.Invalid)
            { 
                stateMachine = null;
            }
            else if (CBSceneManager.instance.LoadedSceneType == CBSceneManager.SceneType.Menu)
            {
                tacticalState = new CBTacticalState(CBSceneManager.instance.sceneCBEntities);
                playState = new CBPlayState(CBSceneManager.instance.sceneCBEntities);

                stateMachine = new CBStateMachine(tacticalState);
                stateMachine.AddState(playState);
            }
            else if (CBSceneManager.instance.LoadedSceneType == CBSceneManager.SceneType.Mission)
            {
                tacticalState = new CBTacticalState(CBSceneManager.instance.sceneCBEntities);
                playState = new CBPlayState(CBSceneManager.instance.sceneCBEntities);

                stateMachine = new CBStateMachine(tacticalState);
                stateMachine.AddState(playState);
            }
        }
    }
}