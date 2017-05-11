using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SpawnableLocation designates an area as somewhere player objects can be spawned in in the level
/// </summary>
/// <remarks>
/// This script will save which players it is responsible for spawning and will relay spawn commands and locations
/// </remarks>
/// <example>
/// See the SpawnableLocation prefab in /Prefabs/Level/
/// </example>
public class SpawnableLocation : MonoBehaviour {
    
    private List<PlayerSpawnData> playersAttached = new List<PlayerSpawnData>();

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        MissionStateManager.instance.OnSetPlayState += MissionStateManager_OnSetPlayState;
        MissionStateManager.instance.OnSetTacticalState += MissionStateManager_OnSetTacticalState;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    private void MissionStateManager_OnSetPlayState(int index)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    private void MissionStateManager_OnSetTacticalState()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Attach a player object without a location to be assigned to spawn on this spawnable location
    /// </summary>
    /// <remarks>
    /// todo: need to pick better locations for default spawning and check that the locations exist in the spawnable area
    /// </remarks>
    /// <param name="g"></param>
    /// <param name="playerIndex"></param>
    public void AttachPlayerObject(GameObject g, int playerIndex)
    {
        AttachPlayerObject(g, playerIndex, transform.position + new Vector3(0, g.GetComponentInChildren<CharacterController>().height/2, playerIndex));
    }

    /// <summary>
    /// Attach a player object to be assigned to spawn on this spawnable location 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="playerIndex"></param>
    /// <param name="position"></param>
    public void AttachPlayerObject(GameObject g, int playerIndex, Vector3 position)
    {
        position.y -= GetComponent<Collider>().bounds.size.y / 2;

        foreach (PlayerSpawnData t in playersAttached)
        {
            if (t.playerObject == g)
            {
                t.spawnPosition = position;
                t.playerObject.transform.position = t.spawnPosition;
                t.player.Respawn(t.spawnPosition);
                return;
            }
        }

        PlayerSpawnData d = new PlayerSpawnData();
        d.playerIndex = playerIndex;
        d.playerObject = g;
        d.spawnPosition = position;
        d.player = g.GetComponent<Player>();
        d.player.OnDetachFromSpawnArea += OnPlayerReAttach;
        d.player.OnFailDetachFromSpawnArea += OnPlayerFailAttach;
        d.player.Respawn(d.spawnPosition);
        playersAttached.Add(d);
    }

    /// <summary>
    /// Issue a respawn command to all players with their spawn location
    /// </summary>
    public void RespawnPlayers()
    {
        for (int i = 0; i < playersAttached.Count; i++)
        {
            PlayerSpawnData d = playersAttached[i];
            d.playerObject.GetComponent<Player>().Respawn(d.spawnPosition + new Vector3(0, 0, 0));
        }
    }

    /// <summary>
    /// A callback function for if a Player is attached to a new SpawnableLocation to handle deregistration
    /// </summary>
    /// <param name="g"></param>
    private void OnPlayerReAttach(GameObject g)
    {
        PlayerSpawnData d = playersAttached.Find(f => f.playerObject == g);
        d.player.OnDetachFromSpawnArea -= OnPlayerReAttach;   
        d.player.OnFailDetachFromSpawnArea -= OnPlayerFailAttach;
        playersAttached.Remove(d);
    }

    /// <summary>
    /// A callback function for if a Player fails to attach to a new SpawnableLocation 
    /// </summary>
    /// <param name="g"></param>
    private void OnPlayerFailAttach(GameObject g)
    {
        PlayerSpawnData d = playersAttached.Find(f => f.playerObject == g);
        d.player.Respawn(d.spawnPosition);
        //g.transform.position = d.spawnPosition;
    }

    /// <summary>
    /// A data structure to hold information about a player registered to a SpawnableLocation
    /// </summary>
    [System.Serializable]
    public class PlayerSpawnData
    {
        public int playerIndex;
        public GameObject playerObject;
        public Player player;
        public Vector3 spawnPosition;
    }
}
