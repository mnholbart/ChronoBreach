using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The MissionObjectives class manages all objectives that exist on the current mission/scene and handles victory status
/// </summary>
/// <remarks>
/// This script exists as a manager that you presently must add all objectives to for configuration
/// </remarks>
/// <example>
/// See TestMission scene for example usage
/// </example>
public class MissionObjectives : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("How many characters will be spawned in for use in this mission")]
    public int MaxCharactersAllowed = 2;

    private List<BaseObjective> objectives;
    private List<BaseObjective> completedObjectives = new List<BaseObjective>();
    private List<BaseObjective> failedObjectives = new List<BaseObjective>();
    private List<BaseObjective> mandatoryObjectivesRemaining = new List<BaseObjective>();

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        objectives = new List<BaseObjective>(GetComponents<BaseObjective>());

        MissionStateManager.instance.OnSetPlayState += MissionStateManager_OnSetPlayState;
        MissionStateManager.instance.OnSetTacticalState += MissionStateManager_OnSetTacticalState;
    }

    /// <summary>
    /// When the mission state is set to tactical, reset the mission objectives and their statuses
    /// </summary>
    private void MissionStateManager_OnSetTacticalState()
    {
        completedObjectives.Clear();
        failedObjectives.Clear();
        mandatoryObjectivesRemaining.Clear();
        
        foreach (BaseObjective bo in objectives)
        {
            bo.Initialize();
            bo.OnObjectiveCompleted -= Objective_OnObjectiveCompleted;
            bo.OnObjectiveFailed -= Objective_OnObjectiveFailed;
        }
    }

    /// <summary>
    /// When the mission state is set to play, enable callbacks and begin tracking objectives
    /// </summary>
    private void MissionStateManager_OnSetPlayState(int index)
    {
        foreach (BaseObjective bo in objectives)
        {
            if (bo.MandatoryObjective)
            {
                mandatoryObjectivesRemaining.Add(bo);
            }
            bo.ResetObjective();
            bo.OnObjectiveCompleted += Objective_OnObjectiveCompleted;
            bo.OnObjectiveFailed += Objective_OnObjectiveFailed;
        }

        if (mandatoryObjectivesRemaining.Count == 0)
        {
            Debug.LogWarning("No mandatory objectives exist on current mission");
        }
    }

    /// <summary>
    /// Track the failure of an objective
    /// </summary>
    /// <param name="b"></param>
    private void Objective_OnObjectiveFailed(BaseObjective b)
    {
        failedObjectives.Add(b);
        Debug.Log("Failed objective: " + b.ToString());
    }

    /// <summary>
    /// Track the success of an objective
    /// </summary>
    /// <param name="b"></param>
    private void Objective_OnObjectiveCompleted(BaseObjective b)
    {
        completedObjectives.Add(b);
        TryCompleteMandatoryObjective(b);
        Debug.Log("Completed objective: " + b.ToString());
    }

    /// <summary>
    /// Attempts to complete a mandatory objective 
    /// </summary>
    /// <param name="b"></param>
    private void TryCompleteMandatoryObjective(BaseObjective b)
    {
        if (!mandatoryObjectivesRemaining.Contains(b))
            return;
        mandatoryObjectivesRemaining.Remove(b);
        if (mandatoryObjectivesRemaining.Count == 0)
        {
            GameManager.instance.TryFinishMission();
        }
    }
}
