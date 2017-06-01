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

        public GameObject CurrentlyRenderedScene;

        public void SetActivelyRenderedScene(GameObject sceneDataObject, bool forceLoadNow = false)
        {
            if (CurrentlyRenderedScene == null)
            {
                StopDisplaying();
            }

            CurrentlyRenderedScene = Instantiate(sceneDataObject);

            CurrentlyRenderedScene.transform.SetParent(transform);
            CurrentlyRenderedScene.transform.localPosition = Vector3.zero;

            if (forceLoadNow)
            {
                OnLevelTryStart.Invoke();
            }
        }

        private void StopDisplaying()
        {
            Destroy(CurrentlyRenderedScene);
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
        }
    }
}

