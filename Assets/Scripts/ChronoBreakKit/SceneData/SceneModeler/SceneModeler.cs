using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ChronoBreak
{
    public class SceneModeler : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Assign a SceneData prefab in the Resources/SceneData/ directory")]
        public GameObject ThisResourcePrefab;

        [Header("Config")]
        public float ModelScale = 1;


        // Use this for initialization
        void Start()
        {
            Destroy(gameObject);
        }

        [ContextMenu("Create Scene Model")]
        public void CreateSceneModel()
        {
            //GameObject basePrefab = Instantiate(ThisResourcePrefab);
            GameObject basePrefab = (GameObject)PrefabUtility.InstantiatePrefab(ThisResourcePrefab);
            SceneData data = basePrefab.GetComponent<SceneData>();

            GameObject mapModel = null;
            foreach (Transform t in basePrefab.transform)
            {
                if (t.name == "Map Mode")
                {
                    mapModel = t.gameObject;
                }
                else
                {
                    DestroyImmediate(t.gameObject);
                }
            }

            if (mapModel == null)
            {
                mapModel = new GameObject("Map Model");
                mapModel.tag = "Hologram";
                mapModel.transform.SetParent(basePrefab.transform);
                mapModel.transform.localPosition = Vector3.zero;

                data.mapModel = mapModel;
            }

            List<GameObject> allEntities = new List<GameObject>(GameObject.FindObjectsOfType<GameObject>());
            Dictionary<GameObject, string> objectTypes = new Dictionary<GameObject, string>();

            foreach (GameObject g in allEntities)
            {
                if (g.GetComponent<Light>() || g.GetComponent<SceneModeler>() || g.tag == "Hologram") //Any type of object to be ignored add here
                {
                    continue;
                }

                GameObject newGO = Instantiate(g);
                newGO.transform.SetParent(mapModel.transform);
                newGO.tag = "Hologram";

                if (newGO.GetComponent<CBEntity>())
                {
                    objectTypes.Add(newGO, newGO.GetComponent<CBEntity>().GetType().AssemblyQualifiedName);
                }
                else
                {
                    objectTypes.Add(newGO, typeof(GameObject).AssemblyQualifiedName);
                }

                StripObjectOfScripts(newGO);
            }

            data.AddObjectTypes(objectTypes);

            mapModel.transform.localScale = new Vector3(ModelScale, ModelScale, ModelScale);

            foreach (Transform t in mapModel.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = LayerMask.NameToLayer("Hologram");
            }

            PrefabUtility.ReplacePrefab(basePrefab, ThisResourcePrefab);
            DestroyImmediate(basePrefab);
        }

        private void StripObjectOfScripts(GameObject g)
        {
            CBEntity e = g.GetComponent<CBEntity>();

            DestroyImmediate(e);
        }
    }
}
