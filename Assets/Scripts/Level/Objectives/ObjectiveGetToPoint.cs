using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveGetToPoint : BaseObjective {

    public float playersRequiredOnPoint = 1;
    public float timeRequiredInPoint = 0;

    [SerializeField]
    private ObjectivePoint objectivePoint;

    private List<Player> playersInPoint = new List<Player>();
    private Coroutine pointTimer;

    public override void Initialize()
    {
        objectivePoint.SetVisible(true);
        if (initialized)
            return;
        base.Initialize();
        VerifyPoint();
    }

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
    }

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
