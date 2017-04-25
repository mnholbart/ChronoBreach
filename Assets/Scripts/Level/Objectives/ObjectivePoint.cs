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
        Player p = GetValidPlayer(other);
        if (p != null)
        {
            if (OnPlayerEnterPoint != null)
                OnPlayerEnterPoint.Invoke(p);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player p = GetValidPlayer(other);
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

    private Player GetValidPlayer(Collider other)
    {
        if (other.GetComponentInChildren<Player>())
            return other.GetComponentInChildren<Player>();

        if (other.GetComponentInParent<SteamVR_ControllerManager>())
            return PlayerSpawnManager.instance.GetActivePlayer();

        return null;
    }
}
