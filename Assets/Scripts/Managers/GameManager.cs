using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public VRTK.VRTK_ControllerEvents LeftController;
    public VRTK.VRTK_ControllerEvents RightController;

    public event Action OnMissionInitialized;
    public event Action OnMissionLoaded;
    public bool DebugMode = false;

    private LevelManager levelManager;
    private MissionStateManager missionStateManager;
    private PlayerSpawnManager playerSpawnManager;
    private TacticalStateManager tacticalStateManager;
    private PlayStateManager playStateManager;

    public enum VRTKMode
    {
        Null,
        Simulator,
        SteamVR
    }
    private VRTKMode mode = VRTKMode.Null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        levelManager = GetComponent<LevelManager>();
        missionStateManager = GetComponent<MissionStateManager>();
        playerSpawnManager = GetComponent<PlayerSpawnManager>();
        tacticalStateManager = GetComponent<TacticalStateManager>();
        playStateManager = GetComponent<PlayStateManager>();
        SetVRMode();
    }


    private IEnumerator Start()
    {
        while (!levelManager.initialized || !missionStateManager.initialized || !playerSpawnManager.initialized 
            || !tacticalStateManager.initialized || !playStateManager.initialized)
            yield return null;
        InitGame();
    }

    private void InitGame()
    {
        LevelManager.SceneState initializedState = levelManager.GetLoadedSceneState();
        if (initializedState == LevelManager.SceneState.TacticalMode) //Started the game already in TacticalMode skipping mission selection
        {
            if (OnMissionLoaded != null)
                OnMissionLoaded.Invoke();
            InitializeMission();
            if (OnMissionInitialized != null)
                OnMissionInitialized.Invoke();
        }
        else if (initializedState == LevelManager.SceneState.Menu)
        {
            if (OnMissionLoaded != null)
                OnMissionLoaded.Invoke();
            InitializeMission();
            if (OnMissionInitialized != null)
                OnMissionInitialized.Invoke();
        }
    }

    public void TryFinishMission()
    {
        //Called when objectives are completed
        //Not sure what should be done here yet, cant just end it if the player wants to try to do a better job
        //todo placeholder for mission complete
    }

    /// <summary>
    /// Load mission with scene index
    /// </summary>
    /// <param name="missionIndex"></param>
    public void LoadMission(int missionIndex)
    {
        StartCoroutine(LoadingMission(missionIndex));

    }

    private IEnumerator LoadingMission(int ind)
    {
        LevelManager.instance.LoadMission(ind);

        yield return null;

        if (OnMissionLoaded != null)
            OnMissionLoaded.Invoke();
        InitializeMission();
        if (OnMissionInitialized != null)
            OnMissionInitialized.Invoke();
    }

    /// <summary>
    /// Initializes the current mission and scene
    /// </summary>
    private void InitializeMission()
    {
        missionStateManager.InitializeMission();
    }

    private void SetVRMode()
    {
        VRTK.SDK_BaseSystem system = VRTK.VRTK_SDKManager.instance.GetSystemSDK();
        if (system is VRTK.SDK_SimSystem)
        {
            mode = VRTKMode.Simulator;
        }
    }

    public VRTKMode GetVRMode()
    {
        return mode;
    }
}
