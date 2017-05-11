using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The MissionStateManager handles changes between Play and Tactical  modes of the mission
/// </summary>
/// <remarks>
/// No logic is handled here, the MissionStateManager delegates events to other managers and scripts to handle what to do
/// </remarks>
public class MissionStateManager : MonoBehaviour
{
    /// <summary>
    /// The current state of the loaded scene
    /// </summary>
    /// <param name="Null">Something broke</param>
    /// <param name="PlayMode">Currently in play mode</param>
    /// <param name="TacticalMode">Currently in tactical mode</param>
    public enum MissionState
    {
        Null,
        Play,
        Tactical
    }

    public static MissionStateManager instance;

    [HideInInspector] public bool initialized;
    [HideInInspector] public event System.Action OnSetNullState;
    [HideInInspector] public event System.Action OnSetTacticalState;
    [HideInInspector] public event System.Action<int> OnSetPlayState;

    private MissionState missionState = MissionState.Null;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        SetNullState();
	}

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        initialized = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetNullState()
    {
        missionState = MissionState.Null;

        if (OnSetNullState != null)
            OnSetNullState.Invoke();
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetTacticalState()
    {
        missionState = MissionState.Tactical;
        UIManager.instance.LoadTacticalMenu();

        if (OnSetTacticalState != null)
            OnSetTacticalState.Invoke();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="charIndex"></param>
    private void SetPlayState(int charIndex)
    {
        missionState = MissionState.Play;
        UIManager.instance.LoadPlayMenu();

        if (OnSetPlayState != null)
            OnSetPlayState.Invoke(charIndex);
    }

    /// <summary>
    /// Callback for mission initialization, default set to tactical state
    /// </summary>
    public void InitializeMission()
    {
        SetTacticalState();
    }

    /// <summary>
    /// Enter play mode with character "index"
    /// </summary>
    /// <param name="index"></param>
    public void StartPlayMode(int index)
    {
        if (CanStartPlayMode())
            SetPlayState(index);
    }

    /// <summary>
    /// Enter tactical mode
    /// </summary>
    public void StartTacticalMode()
    {
        if (CanStartTacticalMode())
            SetTacticalState();
    }

    /// <summary>
    /// Returns true if we are not restricted from entering play mode
    /// </summary>
    /// <returns></returns>
    public bool CanStartPlayMode()
    {
        if (missionState == MissionState.Play)
            return false;

        return true;
    }

    /// <summary>
    /// Returns true if we are not restricted from entering tactical mode
    /// </summary>
    /// <returns></returns>
    public bool CanStartTacticalMode()
    {
        if (missionState == MissionState.Tactical)
            return false;
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static MissionState GetMissionState()
    {
        return instance.missionState;
    }
}
