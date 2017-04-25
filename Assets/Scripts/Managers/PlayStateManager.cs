﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStateManager : MonoBehaviour {

    public static PlayStateManager instance;

    [HideInInspector]
    public bool initialized = false;

    private Transform vrRig;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        vrRig = GameObject.FindObjectOfType<SteamVR_ControllerManager>().transform;
    }

    private void Start()
    {
        MissionStateManager.instance.OnSetNullState += DisableSelf;
        MissionStateManager.instance.OnSetPlayState += EnableSelf;
        MissionStateManager.instance.OnSetTacticalState += DisableSelf;

        initialized = true;
        DisableSelf();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MissionStateManager.instance.StartTacticalMode();
        }
    }

    private void DisableSelf()
    {
        enabled = false;
    }

    private void EnableSelf(int charIndex)
    {
        enabled = true;

        vrRig.localScale = Vector3.one;
    }
}
