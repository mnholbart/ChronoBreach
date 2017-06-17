using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace ChronoBreak
{
    public class CBSceneManager : MonoBehaviour
    {
        public static CBSceneManager instance;

        public System.Action<SceneData, List<CBEntity>, List<GameObject>> OnMissionLoaded;
        public System.Action OnMissionUnloaded;

        [HideInInspector] public bool initialized = false;

        private SceneHologram hologram;
        private List<GameObject> sceneDataObjects = new List<GameObject>();
        private Dictionary<GameObject, SceneData> sceneData = new Dictionary<GameObject, SceneData>();
        private const string sceneDataPath = "SceneData";
        private int currentHologramSceneIndex;
        private Scene loadedGameScene;
        private Scene menuScene;
        private SceneData selectedSceneData
        {
            get
            {
                return sceneData[sceneDataObjects[currentHologramSceneIndex]];
            }
        }

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

            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;
        }

        private void Start()
        {
            CBGameManager.instance.InitializeScene += GameManager_InitializeScene;

            LoadedSceneType = GetCurrentSceneType();
            if (LoadedSceneType == SceneType.Menu)
            {
                menuScene = SceneManager.GetActiveScene();
                hologram = LoadHQHologram();
                sceneDataObjects = LoadSceneData();

                if (hologram == null || sceneDataObjects == null || sceneDataObjects.Count == 0)
                {
                    Debug.LogError("Failed to initialize CBSceneManager");
                }
                else
                {
                    hologram.OnLevelTryStart += TryLoadSelectedMission;
                    hologram.OnLevelTryChangeIncreaseIndex += IncreaseSceneIndex;
                    hologram.OnLevelTryChangeDecreaseIndex += DecreaseSceneIndex;
                    CacheSceneData();
                    initialized = true;
                }
            }
            else
            {
                Debug.LogError("Trying to initialize/load game into non menu scene -not supported");
            }
        }

        private void TryLoadSelectedMission()
        {
            if (loadedGameScene.name == selectedSceneData.attachedScene.SceneName)
                return;

            StartCoroutine(LoadCurrentHologramSceneIndexMission());
        }

        private IEnumerator LoadCurrentHologramSceneIndexMission()
        {
            Coroutine UnloadCoroutine = StartCoroutine(UnloadCurrentGameScene());
            yield return UnloadCoroutine;

            SceneData data = selectedSceneData;

            CBPlayerManager.instance.SetActivePlayers(data.MaxNumberOfCharacters);
            SceneManager.LoadScene(data.attachedScene, LoadSceneMode.Additive);
        }

        private IEnumerator UnloadCurrentGameScene()
        {
            Scene s = SceneManager.GetActiveScene();

            if (s == menuScene) //no game scene to unload
                yield break;

            SceneManager.SetActiveScene(menuScene);
            yield return SceneManager.UnloadSceneAsync(loadedGameScene);
        }

        private void Update()
        {

        }

        private void IncreaseSceneIndex()
        {
            //If we go out of upper bound, wrap back to 0
            if (currentHologramSceneIndex + 1 >= sceneDataObjects.Count)
            {
                currentHologramSceneIndex = 0;
            }
            else
            {
                currentHologramSceneIndex++;
            }
            hologram.SetActivelyRenderedScene(sceneDataObjects[currentHologramSceneIndex]);
        }

        private void DecreaseSceneIndex()
        {
            //If 0, wrap to the upper bound
            if (currentHologramSceneIndex == 0)
            {
                currentHologramSceneIndex = sceneDataObjects.Count - 1;
            }
            else
            {
                currentHologramSceneIndex--;
            }
            hologram.SetActivelyRenderedScene(sceneDataObjects[currentHologramSceneIndex]);
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

        private void SceneUnloaded(Scene s)
        {
            if (OnMissionUnloaded != null)
                OnMissionUnloaded.Invoke();
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.path.Contains("Menu")) //todo probably not a good way to check if menu
                return;
            
            SceneData data = sceneData[sceneDataObjects[currentHologramSceneIndex]];
            if (data.attachedScene.SceneName == scene.name)
            {
                loadedGameScene = scene;
                SceneManager.SetActiveScene(loadedGameScene);

                CBEntity[] e = GameObject.FindObjectsOfType<CBEntity>();
                List<CBEntity> entities = new List<CBEntity>(e.Where(a => a.GetComponent<PlayerController>() == null));
                List<GameObject> geometry = new List<GameObject>(GameObject.FindGameObjectsWithTag("MissionGeometry"));
                
                if (OnMissionLoaded != null) 
                    OnMissionLoaded.Invoke(data, entities, geometry);
            }
        }

        private void GameManager_InitializeScene()
        {
            hologram.SetActivelyRenderedScene(sceneDataObjects[0], true);
            currentHologramSceneIndex = 0;
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

        private SceneHologram LoadHQHologram()
        {
            SceneHologram sh = GameObject.FindObjectOfType<SceneHologram>();
            if (sh != null)
            {
                return sh;
            }
            else
            {
                Debug.LogError("Could not find a SceneHologram");
                return null;
            }
        }

        private List<GameObject> LoadSceneData()
        {
            List<GameObject> objs = new List<GameObject>(Resources.LoadAll<GameObject>(sceneDataPath) as GameObject[]);
            List<GameObject> invalid = new List<GameObject>();
            foreach (GameObject o in objs)
            {
                SceneData data = o.GetComponent<SceneData>();
                if (data == null)
                {
                    invalid.Add(o);
                }
            }

            foreach (GameObject o in invalid)
            {
                objs.Remove(o);
            }

            if (objs == null || objs.Count == 0)
            {
                Debug.LogError("Failed to load any valid SceneData objects");
            }

            return objs;
        }

        private void CacheSceneData()
        {
            foreach (GameObject g in sceneDataObjects)
            {
                SceneData d = g.GetComponent<SceneData>();
                if (d == null)
                {
                    Debug.LogWarning("null scenedata found on scene gameobject " + g.name);
                    continue;
                }
                sceneData.Add(g, d);
            }
        }
    }
}