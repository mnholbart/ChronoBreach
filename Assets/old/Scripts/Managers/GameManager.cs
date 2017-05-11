using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// The GameManager is the highest level of authority in managing game state and handles game initialization as well as singleton references
/// to all other managers
/// </summary>
/// <remarks>
/// This script holds information pertinent to game state such as VR modes and scene load states
/// </remarks>
public class GameManager : MonoBehaviour {

    /// <summary>
    /// What mode of VR is in use for game control
    /// </summary>
    /// <param name="Null">No VR mode has been set... things aren't going to work</param>
    /// <param name="Simulator">NYI - Simulator allows mouse and keyboard controls</param>
    /// <param name="SteamVR">SteamVR is loaded and the SteamVR camera rig is going to be used</param>
    public enum VRTKMode
    {
        Null,
        Simulator,
        SteamVR
    }

    public static GameManager instance;

    [Header("References")]
    public VRTK.VRTK_ControllerEvents LeftController;
    public VRTK.VRTK_ControllerEvents RightController;

    private LevelManager levelManager;
    private MissionStateManager missionStateManager;
    private PlayerSpawnManager playerSpawnManager;
    private TacticalStateManager tacticalStateManager;
    private PlayStateManager playStateManager;
    private VRTKMode mode = VRTKMode.Null;

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator Start()
    {
        while (!levelManager.initialized || !missionStateManager.initialized || !playerSpawnManager.initialized 
            || !tacticalStateManager.initialized || !playStateManager.initialized)
            yield return null;
        yield return new WaitForEndOfFrame();

        InitGame();
    }

    /// <summary>
    /// Initialize the game will be called after every other manager is initialized and ready to setup
    /// </summary>
    /// <remarks>
    /// This function currently handles loading into a scene or the main menu to allow quicker testing
    /// </remarks>
    private void InitGame()
    {
        InitializeLoadedScene();
        InitializeMission();
    }

    /// <summary>
    /// 
    /// </summary>
    private void InitializeLoadedScene()
    {
        levelManager.InitializeLoadedScene();
    }

    /// <summary>
    /// Attempt to reach a state of mission completion and return to menu/tactical mode
    /// </summary>
    /// <remarks>
    /// todo: NYI
    /// </remarks>
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

    /// <summary>
    /// Load a mission by index including initialization
    /// </summary>
    /// <param name="ind">mission build index</param>
    private IEnumerator LoadingMission(int ind)
    {
        LevelManager.instance.LoadMission(ind);

        yield return null;

        InitializeLoadedScene();
        InitializeMission();
    }

    /// <summary>
    /// Initializes the current mission and scene
    /// </summary>
    private void InitializeMission()
    {
        missionStateManager.InitializeMission();
    }

    /// <summary>
    /// Set VR mode initialized
    /// </summary>
    private void SetVRMode()
    {
        if (VRTK.VRTK_SDKManager.instance == null)
        {
            mode = VRTKMode.Null;
            return;
        }

        VRTK.SDK_BaseSystem system = VRTK.VRTK_SDKManager.instance.GetSystemSDK();
        if (system is VRTK.SDK_SimSystem)
        {
            mode = VRTKMode.Simulator;
        } else if (system is VRTK.SDK_SteamVRSystem)
        {
            mode = VRTKMode.SteamVR;
        }
    }

    /// <summary>
    /// Get current VR mode
    /// </summary>
    /// <returns></returns>
    public VRTKMode GetVRMode()
    {
        return mode;
    }
}
