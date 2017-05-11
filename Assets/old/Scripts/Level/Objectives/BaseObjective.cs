using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The BaseObjective class can be extended to create any objective to be managed by the MissionObjectives manager
/// </summary>
/// <remarks>
/// This script exists to provide a simple interface for mission objectives resulting in a mission complete or mission failure
/// </remarks>
/// <example>
/// See ObjectiveGetToPoint.cs to see an implementation example
/// </example>
public abstract class BaseObjective : MonoBehaviour {

    [Header("Config")]
    [Tooltip("A mandatory objective indicates to the mission that this is required to have a mission success")]
    public bool MandatoryObjective = false;

    [HideInInspector] public event System.Action<BaseObjective> OnObjectiveCompleted;
    [HideInInspector] public event System.Action<BaseObjective> OnObjectiveFailed;

    protected bool initialized = false;
    protected bool completed = false;

    /// <summary>
    /// Reset the objective for play mode
    /// </summary>
    public virtual void ResetObjective()
    {
        completed = false;
    }

    /// <summary>
    /// Initialize the objective for tactical mode
    /// </summary>
    public virtual void Initialize()
    {
        if (initialized)
            return;

        initialized = true;
    }

    /// <summary>
    /// 
    /// </summary>
    protected void ObjectiveCompleted()
    {
        if (completed)
            return;

        OnObjectiveCompleted.Invoke(this);
        completed = true;
    }

    /// <summary>
    /// 
    /// </summary>
    protected void ObjectiveFailed()
    {
        if (completed)
            return;

        OnObjectiveFailed.Invoke(this);
        completed = true;
    }
}
