using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableLocation : MonoBehaviour {

    [SerializeField]
    private List<PlayerSpawnData> playersAttached = new List<PlayerSpawnData>();

    private void Awake()
    {

    }

    private void Start()
    {
        MissionStateManager.instance.OnSetPlayState += MissionStateManager_OnSetPlayState;
        MissionStateManager.instance.OnSetTacticalState += MissionStateManager_OnSetTacticalState;
    }

    private void MissionStateManager_OnSetPlayState(int index)
    {
        gameObject.SetActive(false);
    }

    private void MissionStateManager_OnSetTacticalState()
    {
        gameObject.SetActive(true);
    }

    public void AttachPlayerObject(GameObject g, int playerIndex)
    {
        AttachPlayerObject(g, playerIndex, transform.position + new Vector3(0, g.GetComponentInChildren<Collider>().bounds.size.y/2, 0));
    }

    public void AttachPlayerObject(GameObject g, int playerIndex, Vector3 position)
    {
        foreach (PlayerSpawnData t in playersAttached)
        {
            if (t.playerObject == g)
            {
                t.spawnPosition = position;
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
        d.playerObject.transform.position = d.spawnPosition;
        playersAttached.Add(d);
    }

    public void RespawnPlayers()
    {
        for (int i = 0; i < playersAttached.Count; i++)
        {
            PlayerSpawnData d = playersAttached[i];
            d.playerObject.GetComponent<Player>().Respawn(d.spawnPosition + new Vector3(i * d.playerObject.GetComponentInChildren<Collider>().bounds.size.x, 0, 0));
        }
    }

    private void OnPlayerReAttach(GameObject g)
    {
        PlayerSpawnData d = playersAttached.Find(f => f.playerObject == g);
        d.player.OnDetachFromSpawnArea -= OnPlayerReAttach;   
        d.player.OnFailDetachFromSpawnArea -= OnPlayerFailAttach;
        playersAttached.Remove(d);
    }

    private void OnPlayerFailAttach(GameObject g)
    {
        PlayerSpawnData d = playersAttached.Find(f => f.playerObject == g);
        g.transform.position = d.spawnPosition;
    }

    [System.Serializable]
    public class PlayerSpawnData
    {
        public int playerIndex;
        public GameObject playerObject;
        public Player player;
        public Vector3 spawnPosition;
    }
}
