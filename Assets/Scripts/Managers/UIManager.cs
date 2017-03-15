using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject TacticalMenuObject;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        ResetUI();
    }

    /// <summary>
    /// Reset or delete any existing UI elements
    /// </summary>
    private void ResetUI()
    {
        TacticalMenuObject.SetActive(false);
    }

    public void LoadTacticalMenu()
    {
        ResetUI();

    }

    public void LoadPlayMenu()
    {
        ResetUI();
    }

    public void OnButtonPress_SetCharacter(int i)
    {
        if (MissionStateManager.instance.CanStartPlayMode())
        {
            MissionStateManager.instance.StartPlayMode(i);
        }
    }
}
