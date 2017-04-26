using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The ObjectivePoint of an "ObjectiveGetToPoint" mission objective
/// </summary>
/// <remarks>
/// This script tracks an objective points interactions and state for its ObjectiveGetToPoint
/// </remarks>
/// <example>
/// Attach to an object with a trigger collider
/// </example>
public class ObjectivePoint : MonoBehaviour {

    [Header("References")]
    public MeshRenderer meshRenderer;

    [HideInInspector] public event System.Action<Player> OnPlayerEnterPoint;
    [HideInInspector] public event System.Action<Player> OnPlayerExitPoint;

    private ObjectiveEvents events;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        events = GetComponent<ObjectiveEvents>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {         
        Player p = GetValidPlayer(other);
        if (p != null)
        {
            if (OnPlayerEnterPoint != null)
                OnPlayerEnterPoint.Invoke(p);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        Player p = GetValidPlayer(other);
        if (p != null)
        {
            if (OnPlayerExitPoint != null)
                OnPlayerExitPoint.Invoke(p);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visible"></param>
    public void SetVisible(bool visible)
    {
        meshRenderer.enabled = visible;
    }

    /// <summary>
    /// Check if a Collider received contains a valid player type object
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private Player GetValidPlayer(Collider other)
    {
        if (other.GetComponentInChildren<Player>())
            return other.GetComponentInChildren<Player>();

        if (other.GetComponentInParent<SteamVR_ControllerManager>())
            return PlayerSpawnManager.instance.GetActivePlayer();

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void OnObjectiveComplete(BaseObjective b)
    {
        if (events == null)
            return;

        events.InvokeSuccess();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void OnObjectiveFail(BaseObjective b)
    {
        if (events == null)
            return;

        events.InvokeFailure();
    }
}
