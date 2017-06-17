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

        [Header("References")]
        public VRTK.VRTK_HeadsetFade headsetFade;

        [Header("Config")]
        public float stateChangeHeadsetFadeTime = .35f;

        [HideInInspector] public bool initialized = false;

        private CBStateMachine stateMachine;
        private CBDefaultState defaultState;
        private CBTacticalState tacticalState;
        private CBPlayState playState;
        private List<GameObject> headquarterObjects;

        private bool stateChangeInProgress = false;

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

                if (!stateChangeInProgress)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        StartCoroutine(SetTacticalThenPlayState(() => StartPlayMode(0)));
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        StartCoroutine(SetTacticalThenPlayState(() => StartPlayMode(1)));
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        StartCoroutine(SetTacticalThenPlayState(() => StartPlayModeAsSpectator()));
                    }

                    if (stateMachine.CurrentState is CBPlayState && Input.GetKeyDown(KeyCode.Escape))
                    {
                        StartTacticalMode();
                    }
                }
            }
        }

        /// <summary>
        /// Will set state to play state ensuring that the game properly sets itself to tactical state first
        /// and handles any setup/state changes
        /// </summary>
        public IEnumerator SetTacticalThenPlayState(System.Action setStateAction)
        {
            if (stateMachine.CurrentState is CBPlayState)
            {
                Coroutine a = StartCoroutine(StartFadingChangeState<CBTacticalState>(true));
                yield return a;
            }

            setStateAction();
        }

        public void StartPlayMode(int playerIndex)
        {
            if (stateMachine.CurrentState is CBTacticalState )
            {
                PlayerController pc = CBPlayerManager.instance.GetActivePlayerAtIndex(playerIndex);
                playState.primaryPlayer = pc;
                StartCoroutine(StartFadingChangeState<CBPlayState>());
            }
        }

        public void StartTacticalMode()
        {
            StartCoroutine(StartFadingChangeState<CBTacticalState>());
        }

        public void StartPlayModeAsSpectator()
        {
            if (stateMachine.CurrentState is CBTacticalState)
            {
                playState.isSpectator = true;
                //playState.spectatorObject = something
                StartCoroutine(StartFadingChangeState<CBPlayState>());
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

            stateMachine.ChangeState<CBDefaultState>();
            stateMachine.ChangeState<CBTacticalState>(); //probably need a refresh state function more than changing twice to call StartState
        }

        /// <summary>
        /// Changes the state with headset fade
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IEnumerator StartFadingChangeState<T>(bool stayFaded = false) where T : CBState
        {
            if (!stateChangeInProgress)
            {
                stateChangeInProgress = true;
                headsetFade.Fade(Color.black, stateChangeHeadsetFadeTime);
                yield return new WaitForSeconds(stateChangeHeadsetFadeTime);
                stateMachine.ChangeState<T>();
            }

            if (!stayFaded)
            {
                stateMachine.ChangeState<T>();
                headsetFade.Unfade(stateChangeHeadsetFadeTime);
                yield return new WaitForSeconds(stateChangeHeadsetFadeTime);
                stateChangeInProgress = false;
            }
        }
    }
}