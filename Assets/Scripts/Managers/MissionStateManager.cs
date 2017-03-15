using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionStateManager : MonoBehaviour {

    public static MissionStateManager instance;

    [HideInInspector]
    public bool initialized;

    public event System.Action OnSetNullState;
    public event System.Action OnSetTacticalState;
    public event System.Action<int> OnSetPlayState;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        SetNullState();
	}

    private void Start()
    {
        initialized = true;
    }

    public enum MissionState
    {
        Null, //Mission not running
        Tactical, 
        Play
    }
    [SerializeField]
    private MissionState missionState = MissionState.Null;

    private void SetNullState()
    {
        missionState = MissionState.Null;

        if (OnSetNullState != null)
            OnSetNullState.Invoke();
    }

    private void SetTacticalState()
    {
        missionState = MissionState.Tactical;
        UIManager.instance.LoadTacticalMenu();

        if (OnSetTacticalState != null)
            OnSetTacticalState.Invoke();
    }

    private void SetPlayState(int charIndex)
    {
        missionState = MissionState.Play;
        UIManager.instance.LoadPlayMenu();

        if (OnSetPlayState != null)
            OnSetPlayState.Invoke(charIndex);
    }

    public void InitializeMission()
    {
        SetTacticalState();
    }

    public void StartPlayMode(int index)
    {
        if (CanStartPlayMode())
            SetPlayState(index);
    }

    public void StartTacticalMode()
    {
        if (CanStartTacticalMode())
            SetTacticalState();
    }

    public bool CanStartPlayMode()
    {
        if (missionState == MissionState.Play)
            return false;

        return true;
    }

    public bool CanStartTacticalMode()
    {
        if (missionState == MissionState.Tactical)
            return false;
        return true;
    }

    public static MissionState GetMissionState()
    {
        return instance.missionState;
    }
}
