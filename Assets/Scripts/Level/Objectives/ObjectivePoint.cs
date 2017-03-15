using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePoint : MonoBehaviour {

    [SerializeField]
    private MeshRenderer meshRenderer;

    public event System.Action<Player> OnPlayerEnterPoint;
    public event System.Action<Player> OnPlayerExitPoint;

    private void OnTriggerEnter(Collider other)
    {
        Player p = other.GetComponentInChildren<Player>();
        if (p != null)
        {
            if (OnPlayerEnterPoint != null)
                OnPlayerEnterPoint.Invoke(p);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player p = other.GetComponentInChildren<Player>();
        if (p != null)
        {
            if (OnPlayerExitPoint != null)
                OnPlayerExitPoint.Invoke(p);
        }
    }

    public void SetVisible(bool visible)
    {
        meshRenderer.enabled = visible;
    }
}
