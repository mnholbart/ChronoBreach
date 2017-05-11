using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChronoBreak
{
    public class CBSceneManager : MonoBehaviour
    {
        public static CBSceneManager instance;

        public List<CBEntity> sceneCBEntities = new List<CBEntity>();

        [HideInInspector] public bool initialized = false;

        public SceneData currentSceneData { get; private set; }

        public enum SceneType
        {
            Invalid,
            Menu,
            Mission
        }
        public SceneType LoadedSceneType = SceneType.Invalid;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            LoadedSceneType = GetCurrentSceneType();
        }

        private void Start()
        {
            CBGameManager.instance.InitializeScene += GameManager_InitializeScene;

            StartCoroutine(FindAllSceneCBEntities());
        }

        private IEnumerator FindAllSceneCBEntities()
        {
            yield return new WaitForEndOfFrame();

            currentSceneData = GetCurrentSceneData();
            sceneCBEntities = new List<CBEntity>(GameObject.FindObjectsOfType<CBEntity>());

            initialized = true;
        }

        private void Update()
        {

        }

        public SceneType GetCurrentSceneType()
        {
            //Check path of scene, todo: probably have it as a value in the mission data
            Scene curr = SceneManager.GetActiveScene();
            if (curr.path.Contains("Game"))
            {
                return SceneType.Mission;
            }
            else if (curr.path.Contains("Menu"))
            {
                return SceneType.Menu;
            }

            return SceneType.Invalid;
        }

        private void GameManager_InitializeScene()
        {
            
        }

        private SceneData GetCurrentSceneData()
        {
            SceneData d = FindObjectOfType<SceneData>();
            if (d == null)
            {
                Debug.LogError("Could not locate any SceneData in scene");
            }
            return d;
        }
    }
}