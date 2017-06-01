using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak
{
    public class CBPlayerManager : MonoBehaviour
    {
        public static CBPlayerManager instance;

        [Header("References")]
        public GameObject playerPrefab;

        [HideInInspector] public bool initialized = false;
        
        private List<PlayerController> playerControllers = new List<PlayerController>();
        private List<SpawnArea> spawnAreas = new List<SpawnArea>();
        private Dictionary<PlayerController, SpawnArea> playersSpawnAreas = new Dictionary<PlayerController, SpawnArea>();

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

            initialized = true;
        }

        public void SetActivePlayers(int count)
        {
            foreach (PlayerController pc in playerControllers)
            {
                Destroy(pc);
            }
            playerControllers.Clear();

            InstantiatePlayerObjects(count);
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

        private void SetDefaultPlayerSpawnAreas()
        {
            if (spawnAreas.Count == 0)
            {
                Debug.LogError("No Spawn Areas found in scene");
                return;
            }
            SpawnArea defaultArea = spawnAreas[0];
            foreach (PlayerController pc in playerControllers)
            {
                defaultArea.AttachPlayerToSpawnArea(pc);
                playersSpawnAreas[pc] = defaultArea;
            }
        }

        private void InstantiatePlayerObjects(int numSpawn)
        {
            for (int i = 0; i < numSpawn; i++)
            {
                GameObject po = Instantiate(playerPrefab) as GameObject;
                PlayerController pc = po.GetComponent<PlayerController>();
                playerControllers.Add(pc);
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

        public PlayerController GetPlayerAtIndex(int i)
        {
            if (playerControllers.Count > i)
                return playerControllers[i];
            return null;
        }
    }

}
