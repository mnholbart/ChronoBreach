using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObjectives : MonoBehaviour {

    public int MaxCharactersAllowed = 2;

    private List<BaseObjective> objectives;
    private List<BaseObjective> completedObjectives = new List<BaseObjective>();
    private List<BaseObjective> failedObjectives = new List<BaseObjective>();
    private List<BaseObjective> mandatoryObjectivesRemaining = new List<BaseObjective>();

    private void Start()
    {
        objectives = new List<BaseObjective>(GetComponents<BaseObjective>());

        MissionStateManager.instance.OnSetPlayState += MissionStateManager_OnSetPlayState;
        MissionStateManager.instance.OnSetTacticalState += MissionStateManager_OnSetTacticalState;
    }

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

    private void Objective_OnObjectiveFailed(BaseObjective b)
    {
        failedObjectives.Add(b);
        Debug.Log("Failed objective: " + b.ToString());
    }


    private void Objective_OnObjectiveCompleted(BaseObjective b)
    {
        completedObjectives.Add(b);
        TryCompleteMandatoryObjective(b);
        Debug.Log("Completed objective: " + b.ToString());
    }

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
