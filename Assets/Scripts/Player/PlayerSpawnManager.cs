using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour {

    public static PlayerSpawnManager instance;

    public Transform headsetCameraRig;
    [HideInInspector]
    public bool initialized;

    [SerializeField]
    private GameObject playerPrefab;
    MissionObjectives missionObjectives;
    //private MissionData data;
    //private MissionData.MissionInfo currentMissionInfo;
    [SerializeField]
    private List<GameObject> playerObjects = new List<GameObject>();
    [SerializeField]
    private List<SpawnableLocation> spawnPoints;
    private Transform playerParentObject;
    private GameObject primaryPlayerInUse;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        LevelManager.instance.OnMissionLoaded += LevelManager_OnMissionLoaded;

        MissionStateManager.instance.OnSetPlayState += MissionStateManager_OnSetPlayState;
        MissionStateManager.instance.OnSetTacticalState += MissionStateManager_OnSetTacticalState;
        MissionStateManager.instance.OnSetNullState += MissionStateManager_OnSetNullState;
        
        CreateChildParentObject();

        initialized = true;
    }

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
    private void MissionStateManager_OnSetPlayState(int charIndex)
    {
        for (int i = 0; i < playerObjects.Count; i++) 
        {
            GameObject g = playerObjects[i];
            Player p = g.GetComponent<Player>();

            p.SetInteractableEnabled(false);

            if (i == charIndex)
            {
                if (headsetCameraRig != null)
                {
                    p.SetToPrimaryPlayer(headsetCameraRig.gameObject);
                    primaryPlayerInUse = g;
                }
            } else
            {
                p.SetToSecondaryPlayer(playerParentObject);
            }
        }
    }

    private void MissionStateManager_OnSetTacticalState()
    {
        if (primaryPlayerInUse != null)
        {
            primaryPlayerInUse.transform.SetParent(playerParentObject, true);
            primaryPlayerInUse = null;
        }

        foreach (SpawnableLocation l in spawnPoints)
        {
            l.RespawnPlayers();
        }

        foreach (GameObject g in playerObjects)
        {
            Player p = g.GetComponent<Player>();
            p.SetInteractableEnabled(true);
        }
    }

    private void MissionStateManager_OnSetNullState()
    {

    }

    /// <summary>
    /// Called after OnMissionLoaded
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

    private void SpawnPlayerCharacter(int i)
    {
        GameObject t = Instantiate(playerPrefab) as GameObject;
        Player p = t.GetComponent<Player>();
        p.myPlayerIndex = i;
        t.transform.SetParent(playerParentObject, true);
        playerObjects.Add(t);
        spawnPoints[0].AttachPlayerObject(t, i);
    }
    
    private void CreateChildParentObject()
    {
        GameObject g = new GameObject("Players");
        playerParentObject = g.transform;
        playerParentObject.transform.position = Vector3.zero;
        playerParentObject.transform.rotation = Quaternion.identity;
    }

    public Player GetActivePlayer()
    {
        return primaryPlayerInUse.GetComponent<Player>();
    }
}
