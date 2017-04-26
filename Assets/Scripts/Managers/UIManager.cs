using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The UIManager handles all UI elements
/// </summary>
/// <remarks>
///
/// </remarks>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("References")]
    public GameObject tacticalMenuObject;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        ResetUI();
    }

    /// <summary>
    /// Reset or delete any existing UI elements
    /// </summary>
    private void ResetUI()
    {
        tacticalMenuObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void LoadTacticalMenu()
    {
        ResetUI();

    }

    /// <summary>
    /// 
    /// </summary>
    public void LoadPlayMenu()
    {
        ResetUI();
    }
}
