using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A type of BaseObjective that requires player(s) get to an area
/// </summary>
/// <remarks>
/// This allows an easily configurable trigger area volume that a player must reach to complete an objective or trigger an event
/// </remarks>
/// <example>
/// Attach to the mission objectives and configure the objective point object
/// </example>
public class ObjectiveGetToPoint : BaseObjective
{
    [Header("Config")]
    [Tooltip("The number of players that must be present in the trigger area to complete objective")]
    public float playersRequiredOnPoint = 1;
    [Tooltip("How long the required number of players must stay present on this point")]
    public float timeRequiredInPoint = 0;
    [Tooltip("The ObjectivePoint that must be occupied to complete this objective")]
    public ObjectivePoint objectivePoint;

    private List<Player> playersInPoint = new List<Player>();
    private Coroutine pointTimer;

    /// <summary>
    /// Initialize the objective for tactical mode
    /// </summary>
    public override void Initialize()
    {
        objectivePoint.SetVisible(true);
        if (initialized)
            return;
        base.Initialize();
        VerifyPoint();
    }

    /// <summary>
    /// Reset the objective for play mode
    /// </summary>
    public override void ResetObjective()
    {
        base.ResetObjective();
        playersInPoint.Clear();
        if (pointTimer != null)
        {
            StopCoroutine(pointTimer);
            pointTimer = null;
        }
        objectivePoint.SetVisible(false);
    }

    /// <summary>
    /// Check that our ObjectivePoint has been created correctly
    /// </summary>
    private void VerifyPoint()
    {
        Collider c = objectivePoint.transform.GetComponent<Collider>();
        if (c == null)
        {
            Debug.LogWarning("(ObjectiveGetToPoint) - " + objectivePoint.name + " does not have a collider");
            return;
        }
        if (!c.isTrigger)
        {
            Debug.LogWarning("(ObjectiveGetToPoint) - " + objectivePoint.name + " is not set as a trigger");
            return;
        }
        objectivePoint.OnPlayerEnterPoint += OnPlayerEnterPoint;
        objectivePoint.OnPlayerExitPoint += OnPlayerExitPoint;
        OnObjectiveCompleted += objectivePoint.OnObjectiveComplete;
        OnObjectiveFailed += objectivePoint.OnObjectiveFail;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    private void OnPlayerEnterPoint(Player p)
    {
        playersInPoint.Add(p);
        if (playersInPoint.Count >= playersRequiredOnPoint)
        {
            if (pointTimer == null)
            {
                pointTimer = StartCoroutine(PointTimer());
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    private void OnPlayerExitPoint(Player p)
    {
        playersInPoint.Remove(p);
        if (playersInPoint.Count < playersRequiredOnPoint)
        {
            if (pointTimer != null)
            {
                StopCoroutine(pointTimer);
                pointTimer = null;
            }
        }
    }

    /// <summary>
    /// Track the timer of how long players have occupied the point
    /// </summary>
    /// <returns></returns>
    private IEnumerator PointTimer()
    {
        float t = 0;
        while (t < timeRequiredInPoint)
        {
            yield return null;
            t += Time.deltaTime;
        }
        ObjectiveCompleted();
    }

    public override string ToString()
    {
        return "ObjectiveGetToPoint: Point(" + objectivePoint.name + ")";
    }
}
