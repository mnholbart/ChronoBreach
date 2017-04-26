using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Player script manages the player object both during replay and recording, it relies on helper
/// classes such as PlayerInput to function
/// </summary>
/// <remarks>
/// 
/// </remarks>
public class Player : MonoBehaviour
{
    [Header("References")]
    public CapsuleCollider collider;
    public GameObject playerMesh;
    public PlayerInput input;
    public VRTK.VRTK_InteractableObject myInteractableObject;
    public PlayerSpawnIndicator spawnIndicator;

    [Header("Config")]
    [Tooltip("LayerMask for identifying what is a SpawnableLocation")]
    public LayerMask spawnZoneLayerMask;
    [Tooltip("Player index assigned to me")]
    public int myPlayerIndex;

    [HideInInspector] public event System.Action<GameObject> OnDetachFromSpawnArea;
    [HideInInspector] public event System.Action<GameObject> OnFailDetachFromSpawnArea;

    private GameObject VRObject;
    private GameObject interactingObject;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        spawnIndicator.gameObject.SetActive(false);
        myInteractableObject.InteractableObjectUngrabbed += OnUngrabbed;
        myInteractableObject.InteractableObjectGrabbed += OnGrabbed;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// Sets whether the players interactbleobject is enabled or not for grabbing in tactical mode
    /// </summary>
    /// <param name="b"></param>
    public void SetInteractableEnabled(bool b)
    {
        myInteractableObject.isGrabbable = b;
        myInteractableObject.enabled = b;
    }

    /// <summary>
    /// Assign this player as the primary player and initialize
    /// </summary>
    /// <param name="vrRig"></param>
    public void SetToPrimaryPlayer(GameObject vrRig)
    {
        collider.isTrigger = false;
        playerMesh.SetActive(false);
        VRObject = vrRig;
        VRObject.transform.position = transform.position;
        VRObject.transform.rotation = transform.rotation;
        //transform.SetParent(vrRig.transform, true);
        //transform.localPosition += new Vector3(0, 1, 0);
        //VRObject.transform.position -= new Vector3(0, 1, 0);
        input.SetRecordMode(VRObject, GameManager.instance.GetVRMode());
    }

    /// <summary>
    /// Assign this player as a secondary player and initialize
    /// </summary>
    /// <param name="parent"></param>
    public void SetToSecondaryPlayer(Transform parent)
    {
        collider.isTrigger = false;
        playerMesh.SetActive(true);
        VRObject = null;
        //transform.SetParent(parent, true);
        input.SetPlaybackMode();
    }

    /// <summary>
    /// Issue a respawn command to respawn the player and reset its state
    /// </summary>
    /// <param name="pos"></param>
    public void Respawn(Vector3 pos)
    {
        ResetPlayer();
        transform.position = pos;
        transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Reset the players state and behavior
    /// </summary>
    private void ResetPlayer()
    {
        VRObject = null;
        collider.isTrigger = true;
        //inputMode = GameManager.VRTKMode.Null;
        //inputSimulator = null;
        playerMesh.SetActive(true);
        input.SetIdleMode();
    }

    /// <summary>
    /// Callback for being ungrabbed in tactical state to check for attachment to a new spawnable location
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnUngrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        spawnIndicator.gameObject.SetActive(false);
        Ray r = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 5, spawnZoneLayerMask))
        {
            OnDetachFromSpawnArea.Invoke(gameObject);
            Vector3 spawnPoint = hit.point + new Vector3(0, GetComponent<Collider>().bounds.size.y / 2, 0);
            hit.transform.GetComponent<SpawnableLocation>().AttachPlayerObject(gameObject, myPlayerIndex, spawnPoint);
        }
        else
        {
            OnFailDetachFromSpawnArea.Invoke(gameObject);
        }
    }

    /// <summary>
    /// When grabbed enable the spawnIndicator for visual feedback on valid spawn locations
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        spawnIndicator.gameObject.SetActive(true);
    }

    /// <summary>
    /// Check if our current drop position is a valid spawn point
    /// </summary>
    /// <returns></returns>
    public bool IsValidDropPoint()
    {
        Ray r = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 5, spawnZoneLayerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
