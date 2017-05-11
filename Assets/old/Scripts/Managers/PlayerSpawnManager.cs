using System;
using System.Collections;
using System.Collections.Generic;
using ChronoBreak;
using UnityEngine;

/// <summary>
/// The PlayerSpawnManager manages when to spawn and respawn players and tracks the primary player
/// </summary>
/// <remarks>
/// 
/// </remarks>
public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager instance;

    [Header("References")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform headsetCameraRig;

    [HideInInspector] public bool initialized;

    private MissionObjectives missionObjectives;
    private List<GameObject> playerObjects = new List<GameObject>();
    private List<SpawnableLocation> spawnPoints;
    private Transform playerParentObject;
    private GameObject primaryPlayerInUse;

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
        LevelManager.instance.OnMissionLoaded += LevelManager_OnMissionLoaded;

        MissionStateManager.instance.OnSetPlayState += MissionStateManager_OnSetPlayState;
        MissionStateManager.instance.OnSetTacticalState += MissionStateManager_OnSetTacticalState;
        MissionStateManager.instance.OnSetNullState += MissionStateManager_OnSetNullState;
        
        CreateChildParentObject();

        initialized = true;
    }

    /// <summary>
    /// Callback for mission loaded, handles menu or mission loading, initializes player instantiation and setup
    /// </summary>
    /// <param name="state"></param>
    private void LevelManager_OnMissionLoaded(LevelManager.SceneState state)
    {
        if (state == LevelManager.SceneState.Menu)
        {
            playerObjects.Clear();
            spawnPoints.Clear();
            primaryPlayerInUse = null;
            playerParentObject = null;
            enabled = false;
        }
        else if (state == LevelManager.SceneState.TacticalMode)
        {
            enabled = true;
            if (headsetCameraRig == null && GameManager.instance.GetVRMode() == GameManager.VRTKMode.Simulator)
                headsetCameraRig = GameObject.FindObjectOfType<VRTK.SDK_InputSimulator>().transform;
            if (headsetCameraRig == null && GameManager.instance.GetVRMode() == GameManager.VRTKMode.SteamVR)
            {
                headsetCameraRig = GameObject.FindObjectOfType<SteamVR_ControllerManager>().transform;
                //Debug.Log(headsetCameraRig);
            }
            missionObjectives = LevelManager.instance.GetMissionObjectives();
            if (missionObjectives == null)
                Debug.LogError("Failed to get valid MissionObjectives");
            InitializeMission();
        }
    }

    /// <summary>
    /// Called after OnMissionLoaded to spawn players
    /// </summary>
    public void InitializeMission()
    {
        SpawnableLocation[] locs = GameObject.FindObjectsOfType<SpawnableLocation>();
        spawnPoints = new List<SpawnableLocation>(locs);

        for (int i = 0; i < missionObjectives.MaxCharactersAllowed; i++)
        {
            SpawnPlayerCharacter(i);
        }
    }

    /// <summary>
    /// Callback for change of state to Play mode, initializes player objects to primary or secondary player objects
    /// </summary>
    /// <param name="charIndex"></param>
    private void MissionStateManager_OnSetPlayState(int charIndex)
    {
        for (int i = 0; i < playerObjects.Count; i++) 
        {
            GameObject g = playerObjects[i];
            Player p = g.GetComponent<Player>();

            if (i == charIndex)
            {
                if (headsetCameraRig != null)
                {
                    p.OnSetPlayState(headsetCameraRig, true);
                    primaryPlayerInUse = g;
                    primaryPlayerInUse.GetComponent<Player>().OnPlayerDeath += OnPrimaryPlayerDeath;
                }
            }
            else
            {
                p.OnSetPlayState(playerParentObject);
            }
        }
    }

    /// <summary>
    /// On set to tactical state, remove primary player and reinitialize players for tactical mode
    /// </summary>
    private void MissionStateManager_OnSetTacticalState()
    {
        if (primaryPlayerInUse != null)
        {
            primaryPlayerInUse.GetComponent<Player>().OnPlayerDeath -= OnPrimaryPlayerDeath;
            primaryPlayerInUse.transform.SetParent(playerParentObject, true);
            primaryPlayerInUse = null;
        }

        foreach (GameObject g in playerObjects)
        {
            Player p = g.GetComponent<Player>();
            p.OnSetTacticalState();
        }

        foreach (SpawnableLocation l in spawnPoints)
        {
            l.RespawnPlayers();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void MissionStateManager_OnSetNullState()
    {

    }

    /// <summary>
    /// Callback for when the primary controlled player dies
    /// </summary>
    /// <param name="p"></param>
    private void OnPrimaryPlayerDeath(Player p)
    {
        MissionStateManager.instance.StartTacticalMode();
    }

    /// <summary>
    /// Creates a player and attaches it to a spawnable location 
    /// </summary>
    /// <remarks>
    /// Spawns the player at the first spawnable location it finds
    /// 
    /// todo: designate a default spawn location for each scene
    /// </remarks>
    /// <param name="i"></param>
    private void SpawnPlayerCharacter(int i)
    {
        GameObject t = Instantiate(playerPrefab) as GameObject;
        Player p = t.GetComponent<Player>();
        p.myPlayerIndex = i;
        t.transform.SetParent(playerParentObject, true);
        playerObjects.Add(t);
        spawnPoints[0].AttachPlayerObject(t, i);
    }
    
    /// <summary>
    /// Create a parent object to organize player objects
    /// </summary>
    private void CreateChildParentObject()
    {
        GameObject g = new GameObject("Players");
        playerParentObject = g.transform;
        playerParentObject.transform.position = Vector3.zero;
        playerParentObject.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Get the current primary player
    /// </summary>
    /// <returns></returns>
    public Player GetActivePlayer()
    {
        return primaryPlayerInUse.GetComponent<Player>();
    }
}
