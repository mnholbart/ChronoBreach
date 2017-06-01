using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak
{
    public abstract class CBEntity : MonoBehaviour
    {
        [Header("Config")]
        [Tooltip("Will the entities transform be saved for reset on tactical mode")]
        public bool SaveTransform = true; 

        public event Action OnStartPlayState;
        public event Action OnEndPlayState;
        public event Action OnStartTacticalState;
        public event Action OnEndTacticalState;

        private Vector3 startPosition;
        private Vector3 startRotation;
        private Vector3 startScale;
        private Transform startParent;

        protected virtual void Awake()
        {
            SaveProperties();
        }

        /// <summary>
        /// Called whenever TacticalState is enabled, reset any properties such as health, states, or positions here
        /// </summary>
        protected abstract void ResetObjectToDefault();

        public virtual void StartTacticalState()
        {
            if (OnStartTacticalState != null)
                OnStartTacticalState.Invoke();

            ResetObject();
            ResetObjectToDefault();

            gameObject.SetActive(false);
        }

        public virtual void EndTacticalState()
        {
            if (OnEndTacticalState != null)
                OnEndTacticalState.Invoke();
        }

        public virtual void StartPlayState()
        {
            if (OnStartPlayState != null)
                OnStartPlayState.Invoke();
        }

        public virtual void EndPlayState()
        {
            if (OnEndPlayState != null)
                OnEndPlayState.Invoke();
        }

        private void ResetObject()
        {
            if (SaveTransform)
            {
                transform.position = startPosition;
                transform.rotation = Quaternion.Euler(startRotation);
                transform.localScale = startScale;
                transform.SetParent(startParent, true);
            }
        }

        /// <summary>
        /// Save properties on startup that act as a default, properties are reset with ResetObjectToDefault()
        /// </summary>
        protected abstract void SaveDefaultProperties();

        private void SaveProperties()
        {
            startPosition = transform.position;
            startRotation = transform.eulerAngles;
            startScale = transform.localScale;
            startParent = transform.parent;

            SaveDefaultProperties();
        }
    }
}
