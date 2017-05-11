using System;
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
public class Player : MonoBehaviour, IReceivesProjectileHits
{
    [Header("References")]
    public CharacterController collider;
    public GameObject playerMesh;
    public GameObject playerBodyMesh;
    public PlayerInput input;
    public VRTK.VRTK_InteractableObject myInteractableObject;
    public PlayerSpawnIndicator spawnIndicator;

    [Header("Config")]
    [Tooltip("LayerMask for identifying what objects to raycast against for finding a spawnable location")]
    public LayerMask spawnZoneLayerMask;
    [Tooltip("Player index assigned to me")]
    public int myPlayerIndex;
    [Tooltip("Maximum health the player can reset or heal to")]
    public int maxHealth = 100;

    [HideInInspector] public event Action<GameObject> OnDetachFromSpawnArea;
    [HideInInspector] public event Action<GameObject> OnFailDetachFromSpawnArea;
    [HideInInspector] public event Action<Player> OnPlayerDeath;

    private GameObject VRObject;
    private GameObject interactingObject;
    private Color touchColor;
    private bool isDead = true;
    private int currentHealth = 0;
    private float baseHeight;
    private Vector3 baseScale;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        spawnIndicator.gameObject.SetActive(false);
        myInteractableObject.InteractableObjectUngrabbed += OnUngrabbed;
        myInteractableObject.InteractableObjectGrabbed += OnGrabbed;
        touchColor = myInteractableObject.touchHighlightColor;

        baseHeight = collider.height;
        baseScale = playerBodyMesh.transform.localScale;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Die();
        }
    }

    /// <summary>
    /// Sets whether the players interactbleobject is enabled or not for grabbing in tactical mode
    /// </summary>
    /// <param name="b"></param>
    public void SetInteractableEnabled(bool b)
    {
        myInteractableObject.isGrabbable = b;
        myInteractableObject.enabled = b;
        if (b)
        {
            myInteractableObject.touchHighlightColor = touchColor;
        } else
        {
            myInteractableObject.touchHighlightColor = Color.clear;
        }
    }

    /// <summary>
    /// Initialize the player to tactical state
    /// </summary>
    public void OnSetTacticalState()
    {
        ResetPlayer();
        SetInteractableEnabled(true);
    }

    /// <summary>
    /// Initialize the player to play state, and set primary if told to
    /// </summary>
    /// <param name="primary"></param>
    /// <param name="vrRig"></param>
    public void OnSetPlayState(Transform parentTransform, bool primary = false)
    {
        //collider.isTrigger = false;
        playerMesh.SetActive(!primary);

        if (primary)
        {
            SetToPrimaryPlayer(parentTransform);
        }
        else
        {
            SetToSecondaryPlayer(parentTransform);
        }
        SetInteractableEnabled(false);
    }

    /// <summary>
    /// Assign this player as the primary player and initialize
    /// </summary>
    /// <param name="vrRig"></param>
    public void SetToPrimaryPlayer(Transform vrRig)
    {
        VRObject = vrRig.gameObject;
        VRObject.transform.position = transform.position - new Vector3(0, collider.height/2, 0);
        VRObject.transform.rotation = transform.rotation;

        input.SetRecordMode(VRObject, GameManager.instance.GetVRMode());
    }

    /// <summary>
    /// Assign this player as a secondary player and initialize
    /// </summary>
    /// <param name="parent"></param>
    public void SetToSecondaryPlayer(Transform parent)
    {
        VRObject = null;

        input.SetPlaybackMode();
    }

    /// <summary>
    /// Issue a respawn command to respawn the player and reset its state
    /// </summary>
    /// <param name="pos"></param>
    public void Respawn(Vector3 pos)
    {
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        playerBodyMesh.transform.localScale = baseScale;
        collider.height = baseHeight;
    }

    /// <summary>
    /// Reset the players state and behavior
    /// </summary>
    private void ResetPlayer()
    {
        //Reset state stuff
        input.SetIdleMode();
        playerMesh.SetActive(true);
        //collider.isTrigger = true;
        VRObject = null;

        //Reset properties
        isDead = false;
        currentHealth = maxHealth;
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
        if (Physics.Raycast(r, out hit, 5, spawnZoneLayerMask) && hit.collider.GetComponent<SpawnableLocation>())
        {
            OnDetachFromSpawnArea.Invoke(gameObject);
            Vector3 spawnPoint = new Vector3(hit.point.x, hit.collider.transform.position.y + GetComponent<CharacterController>().height/2, hit.point.z);
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
        if (Physics.Raycast(r, out hit, 5, spawnZoneLayerMask) && hit.collider.GetComponent<SpawnableLocation>())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ReceiveHit(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        
        if (OnPlayerDeath != null)
            OnPlayerDeath.Invoke(this);
    }
}
