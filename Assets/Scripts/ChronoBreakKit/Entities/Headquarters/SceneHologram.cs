using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak
{
    public abstract class SceneHologram : MonoBehaviour
    {
        public System.Action OnLevelTryStart;
        public System.Action OnLevelTryChangeDecreaseIndex;
        public System.Action OnLevelTryChangeIncreaseIndex;

        [Header("References")]
        public GameObject HologramBase;

        [Header("Config")]
        public float ScrollSpeed = 2f;
        public Material GameObjectMaterial;
        public Material SpawnAreaMaterial;

        [Header("Debug")]
        public GameObject CurrentlyRenderedScene;
        public Vector3 sceneModelOffset = new Vector3(0, 0, 0);

        private SceneData currentData;

        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (OnLevelTryStart != null)
                {
                    OnLevelTryStart.Invoke();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (OnLevelTryChangeDecreaseIndex != null)
                    OnLevelTryChangeDecreaseIndex.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (OnLevelTryChangeIncreaseIndex != null)
                    OnLevelTryChangeIncreaseIndex.Invoke();
            }

            Vector3 movement = new Vector3();
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement.x -= 1;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                movement.z += 1;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                movement.x += 1;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                movement.z -= 1;
            }
            movement.Normalize();

            if (CurrentlyRenderedScene != null)
            {
                currentData.mapModel.transform.position += movement * Time.deltaTime * ScrollSpeed;
                sceneModelOffset = currentData.mapModel.transform.localPosition;
            }
        }

        public void SetActivelyRenderedScene(GameObject sceneDataObject, bool forceLoadNow = false)
        {
            if (CurrentlyRenderedScene != null)
            {
                StopDisplaying();
            }

            CurrentlyRenderedScene = Instantiate(sceneDataObject);
            currentData = CurrentlyRenderedScene.GetComponent<SceneData>();

            CurrentlyRenderedScene.transform.SetParent(HologramBase.transform);
            CurrentlyRenderedScene.transform.localPosition = new Vector3(0, 1.1f, 0);

            UpdateObjectMaterialsForType();

            if (forceLoadNow)
            {
                OnLevelTryStart.Invoke();
            }
        }

        private void StopDisplaying()
        {
            Destroy(CurrentlyRenderedScene);
        }

        private void UpdateObjectMaterialsForType()
        {
            SceneData d = currentData;
            
            foreach (SceneData.GameObjectType o in d.objectTypes)
            {
                GameObject g = o.myObject;
                System.Type t = System.Type.GetType(o.assemblyName);

                Renderer r = g.GetComponent<MeshRenderer>();
                Material m = r.material;

                if (t == typeof(GameObject))
                {
                    m = GameObjectMaterial;
                }
                else if (t == typeof(SpawnArea))
                {
                    m = SpawnAreaMaterial;
                    o.myObject.AddComponent<HologramSpawnArea>();
                }

                if (m != null)
                    r.material = m;
            }
        }
    }
}

