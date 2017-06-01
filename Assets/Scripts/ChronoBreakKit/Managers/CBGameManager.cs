using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak
{
    public sealed class CBGameManager : MonoBehaviour
    {

        public static CBGameManager instance;

        public event System.Action InitializeScene; //load scene data and type on startup or scene load
        

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        private IEnumerator Start()
        {
            while (!CBMissionStateManager.instance.initialized || !CBSceneManager.instance.initialized || !CBPlayerManager.instance.initialized)
                yield return null;

            if (InitializeScene != null)
                InitializeScene.Invoke();
        }
    }
}