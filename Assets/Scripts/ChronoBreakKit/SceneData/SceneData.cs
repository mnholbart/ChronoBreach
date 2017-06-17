using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak
{
    public class SceneData : MonoBehaviour
    {
        [Header("References")]
        public SceneField attachedScene;
        public GameObject mapModel;

        [Header("Config")]
        public int MaxNumberOfCharacters = 2;

        [Header("Debug")]
        public List<GameObjectType> objectTypes;
        public List<GameObject> spawnAreas;
        public List<PlayerSpawnData> playerSpawnData;

        public void AddObjectTypes(Dictionary<GameObject, string> objects)
        {
            objectTypes = new List<GameObjectType>();
            spawnAreas = new List<GameObject>();
            playerSpawnData = new List<PlayerSpawnData>();

            foreach (KeyValuePair<GameObject, string> pair in objects)
            {
                objectTypes.Add(new GameObjectType(pair.Key, pair.Value));

                System.Type t = System.Type.GetType(pair.Value);
                if (t == typeof(SpawnArea))
                {
                    spawnAreas.Add(pair.Key);
                }
            }
        }

        [System.Serializable]
        public class GameObjectType
        {
            public GameObject myObject;
            public string assemblyName;

            public GameObjectType(GameObject g, string s)
            {
                myObject = g;
                assemblyName = s;
            }
        }

        [System.Serializable]
        public class PlayerSpawnData
        {
            public GameObject spawnArea;
            public Vector3 spawnPosition;
        }
    }
}
