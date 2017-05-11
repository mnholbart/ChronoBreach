using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using VRTK;

/// <summary>
/// The LevelManager handles changing scenes as well as handling scene geometry
/// </summary>
/// <remarks>
/// 
/// </remarks>
public class LevelManager : MonoBehaviour {

    /// <summary>
    /// The current state of the loaded scene
    /// </summary>
    /// <param name="Menu">We are in a menu scene</param>
    /// <param name="PlayMode">Currently in a game scene in play mode</param>
    /// <param name="TacticalMode">Currently in a game scene in tactical mode</param>
    public enum SceneState
    {
        Menu,
        PlayMode,
        TacticalMode
    }

    public static LevelManager instance;

    [Header("References")]
    public GameObject VRTKRightController;
    public GameObject VRTKLeftController;

    [HideInInspector] public SceneState CurrentSceneState = SceneState.Menu;
    [HideInInspector] public event System.Action<SceneState> OnMissionLoaded;
    [HideInInspector] public bool initialized;

    private List<GeometryMaterialSwitcher> levelGeometry = new List<GeometryMaterialSwitcher>();
    private List<TrackedObject> levelObjects = new List<TrackedObject>();
    private MissionObjectives missionObjectives;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        MissionStateManager.instance.OnSetPlayState += MissionStateManager_OnSetPlayState;
        MissionStateManager.instance.OnSetTacticalState += MissionStateManager_OnSetTacticalState;

        initialized = true;
    }

    /// <summary>
    /// Callback for a mission being initially loaded
    /// </summary>
    public void InitializeLoadedScene()
    {
        levelGeometry.Clear();
        levelObjects.Clear();
        levelGeometry = GameObject.FindObjectsOfType<GeometryMaterialSwitcher>().ToList();
        levelObjects = GameObject.FindObjectsOfType<TrackedObject>().ToList();

        CurrentSceneState = GetLoadedSceneType();
        if (OnMissionLoaded != null)
            OnMissionLoaded.Invoke(CurrentSceneState);
    }

    /// <summary>
    /// Callback for state change to play mode
    /// </summary>
    /// <param name="index"></param>
    private void MissionStateManager_OnSetPlayState(int index)
    {
        VRTKLeftController.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
        VRTKLeftController.GetComponent<VRTK_InteractGrab>().ForceRelease();
        VRTKRightController.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
        VRTKRightController.GetComponent<VRTK_InteractGrab>().ForceRelease();

        foreach (GeometryMaterialSwitcher g in levelGeometry)
        {
            g.SetTacticalMaterial();
        }
        foreach (TrackedObject o in levelObjects)
        {
            o.Reset();
        }
    }

    /// <summary>
    /// Callback for state change to tactical mode
    /// </summary>
    private void MissionStateManager_OnSetTacticalState()
    {
        VRTKLeftController.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
        VRTKLeftController.GetComponent<VRTK_InteractGrab>().ForceRelease();
        VRTKRightController.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
        VRTKRightController.GetComponent<VRTK_InteractGrab>().ForceRelease();

        foreach (GeometryMaterialSwitcher g in levelGeometry)
        {
            g.SetTacticalMaterial();
        }
        foreach (TrackedObject o in levelObjects)
        {
            o.Reset();
        }
    }

    /// <summary>
    /// Load a mission by index
    /// </summary>
    /// <param name="buildSceneIndex"></param>
    public void LoadMission(int buildSceneIndex)
    {
        SceneManager.LoadScene(buildSceneIndex, LoadSceneMode.Single);
    }

    /// <summary>
    /// Get the scene state of a currently loaded scene
    /// </summary>
    /// <returns></returns>
    public SceneState GetLoadedSceneType()
    {
        Scene s = SceneManager.GetActiveScene();
        string n = SceneUtility.GetScenePathByBuildIndex(s.buildIndex);
        if (n.Contains("Game/"))
        {
            missionObjectives = GameObject.FindObjectOfType<MissionObjectives>();
            if (missionObjectives == null)
                Debug.LogError("Failed to find MissionObjectives object in scene");
            
            return SceneState.TacticalMode;
        }
        return SceneState.Menu;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Mission objectives in the loaded mission</returns>
    public MissionObjectives GetMissionObjectives()
    {
        return missionObjectives;
    }
}
