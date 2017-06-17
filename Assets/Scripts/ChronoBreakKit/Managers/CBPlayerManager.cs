using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak
{
    public class CBPlayerManager : MonoBehaviour
    {
        public static CBPlayerManager instance;

        [Header("References")]
        public GameObject PlayerPrefab;
        public Transform PlayerSpawnLocation;

        [HideInInspector] public bool initialized = false;

        private List<PlayerController> allPlayerControllers = new List<PlayerController>();
        private List<PlayerController> activePlayerControllers = new List<PlayerController>();
        private List<SpawnArea> spawnAreas = new List<SpawnArea>();
        private Dictionary<PlayerController, SpawnArea> playersSpawnAreas = new Dictionary<PlayerController, SpawnArea>();

        private const int maxPlayerObjects = 4;

        public void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        public void Start()
        {
            CBSceneManager.instance.OnMissionLoaded += CBSceneManager_OnMissionLoaded;
            CBSceneManager.instance.OnMissionUnloaded += CBSceneManager_OnMissionUnloaded;

            initialized = true;
        }

        public void SetActivePlayers(int count)
        {
            if (activePlayerControllers == null || activePlayerControllers.Count == 0)
                InstantiatePlayerObjects(maxPlayerObjects);

            activePlayerControllers.Clear();
            for (int i = 0; i < maxPlayerObjects; i++)
            {
                PlayerController pc = allPlayerControllers[i];

                if (i < count)
                {
                    pc.gameObject.SetActive(true);
                    activePlayerControllers.Add(pc);
                }
                else
                {
                    pc.gameObject.SetActive(false);
                }
            }
        }

        private void CBSceneManager_OnMissionLoaded(SceneData data, List<CBEntity> entities, List<GameObject> geometry)
        {
            foreach (CBEntity e in entities)
            {
                if (e is SpawnArea)
                {
                    spawnAreas.Add(e as SpawnArea);
                }
            }

            SetDefaultPlayerSpawnAreas();
        }

        private void CBSceneManager_OnMissionUnloaded()
        {
            playersSpawnAreas.Clear();
            spawnAreas.Clear();
        }

        private void SetDefaultPlayerSpawnAreas()
        {
            playersSpawnAreas.Clear();

            if (spawnAreas.Count == 0)
            {
                Debug.LogError("No Spawn Areas found in scene");
                return;
            }
            SpawnArea defaultArea = spawnAreas[0];
            foreach (PlayerController pc in activePlayerControllers)
            {
                defaultArea.AttachPlayerToSpawnArea(pc);
                playersSpawnAreas[pc] = defaultArea;
            }
        }

        private void InstantiatePlayerObjects(int numSpawn)
        {
            for (int i = 0; i < numSpawn; i++)
            {
                GameObject po = Instantiate(PlayerPrefab) as GameObject;
                PlayerController pc = po.GetComponent<PlayerController>();
                allPlayerControllers.Add(pc);
            }
        }

        public void RespawnPlayers()
        {
            foreach (PlayerController pc in playersSpawnAreas.Keys)
            {
                SpawnArea area = playersSpawnAreas[pc];
                area.RespawnPlayer(pc);
            }            
        }

        public void UpdatePlayersSpawnPoint(PlayerController controller, Vector3 hitPoint, SpawnArea newSpawnArea)
        {
            SpawnArea oldSpawnArea = playersSpawnAreas[controller];
            if (oldSpawnArea == newSpawnArea)
            {
                oldSpawnArea.UpdateSpawnLocation(controller, hitPoint);
            }
            else
            {
                oldSpawnArea.RemovePlayer(controller);
                playersSpawnAreas[controller] = newSpawnArea;
                newSpawnArea.AttachPlayerToSpawnArea(controller, hitPoint);
            }
        }

        public PlayerController GetActivePlayerAtIndex(int i)
        {
            if (activePlayerControllers.Count > i)
                return activePlayerControllers[i];
            return null;
        }

        public PlayerController GetAllPlayerAtIndex(int i)
        {
            if (allPlayerControllers.Count > i)
                return allPlayerControllers[i];
            return null;
        }

        public void MovePlayersToHeadquarterPositions()
        {
            Debug.Log("ASD");
            foreach (PlayerController pc in allPlayerControllers)
            {
            }
        }
    }

}
