using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour {

    public static LevelManager instance;

    public event System.Action<SceneState> OnMissionLoaded;
    [HideInInspector]
    public bool initialized;

    private List<GeometryMaterialSwitcher> levelGeometry = new List<GeometryMaterialSwitcher>();
    private MissionObjectives missionObjectives;

    public enum SceneState
    {
        Menu,
        PlayMode,
        TacticalMode
    }
    public SceneState CurrentSceneState = SceneState.Menu;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        GameManager.instance.OnMissionLoaded += GameManager_OnMissionLoaded;
        MissionStateManager.instance.OnSetPlayState += MissionStateManager_OnSetPlayState;
        MissionStateManager.instance.OnSetTacticalState += MissionStateManager_OnSetTacticalState;

        initialized = true;
    }

    private void Update()
    {

    }

    private void GameManager_OnMissionLoaded()
    {
        levelGeometry.Clear();
        levelGeometry = GameObject.FindObjectsOfType<GeometryMaterialSwitcher>().ToList();

        CurrentSceneState = GetLoadedSceneState();
        if (OnMissionLoaded != null)
            OnMissionLoaded.Invoke(CurrentSceneState);
    }

    private void MissionStateManager_OnSetPlayState(int index)
    {
        foreach (GeometryMaterialSwitcher g in levelGeometry)
        {
            g.SetPlayMaterial();
        }
    }

    private void MissionStateManager_OnSetTacticalState()
    {
        foreach (GeometryMaterialSwitcher g in levelGeometry)
        {
            g.SetTacticalMaterial();
        }
    }

    public void LoadMission(int buildSceneIndex)
    {
        SceneManager.LoadScene(buildSceneIndex, LoadSceneMode.Single);
    }

    public SceneState GetLoadedSceneState()
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

    public MissionObjectives GetMissionObjectives()
    {
        return missionObjectives;
    }
}
