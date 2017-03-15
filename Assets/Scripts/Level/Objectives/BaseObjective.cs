using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseObjective : MonoBehaviour {

    public bool MandatoryObjective = false;

    public event System.Action<BaseObjective> OnObjectiveCompleted;
    public event System.Action<BaseObjective> OnObjectiveFailed;

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

    protected void ObjectiveCompleted()
    {
        if (completed)
            return;

        OnObjectiveCompleted.Invoke(this);
        completed = true;
    }

    protected void ObjectiveFailed()
    {
        if (completed)
            return;

        OnObjectiveFailed.Invoke(this);
        completed = true;
    }
}
