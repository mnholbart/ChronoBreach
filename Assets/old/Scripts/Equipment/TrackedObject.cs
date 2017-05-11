using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The TrackedObject script will be tracked by the LevelManager and reset on mission state changes
/// </summary>
/// <remarks>
/// Only handles the most basic of resetting functionality for any entity in the scene, extend and add further reset behavior for more advanced objects
/// </remarks>
/// <example>
/// See EquippableItem.cs
/// </example>
public abstract class TrackedObject : MonoBehaviour {
    
    private Vector3 startScale;
    private Vector3 startPosition;
    private Vector3 startRotation;

    /// <summary>
    /// 
    /// </summary>
    protected virtual void Awake()
    {
        startScale = transform.localScale;
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }

    /// <summary>
    /// Reset the object to its initial state, virtual to reset any behavior of inherited scripts
    /// </summary>
    protected virtual void ResetObject()
    {
        transform.localScale = startScale;
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(startRotation);
    }

    /// <summary>
    /// Reset the object to its initial state
    /// </summary>
    public void Reset()
    {
        ResetObject();
    }

}
