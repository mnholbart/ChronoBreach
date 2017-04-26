using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ObjectiveEvents can be added as a component onto an objective object to allow and receive callbacks on objective events
/// </summary>
/// <remarks>
/// This script allows a modular approach to hooking up functionality to an objective instead of requiring a control
/// </remarks>
/// <example>
/// Attach to an objective and hook  up to a doors toggleopening function to allow an objective completion to open a door
/// </example>
[RequireComponent(typeof(BaseObjective))]
public class ObjectiveEvents : MonoBehaviour {

    [Header("Events")]
    public UnityEvent OnObjectiveSuccess;
    public UnityEvent OnObjectiveFail;

    public void InvokeSuccess()
    {
        if (OnObjectiveSuccess != null)
            OnObjectiveSuccess.Invoke();
    }

    public void InvokeFailure()
    {
        if (OnObjectiveFail != null)
            OnObjectiveFail.Invoke();
    }

}
