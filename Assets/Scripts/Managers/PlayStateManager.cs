using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// The PlayStateManager acts as a less authoritative GameManager during the Play state of a mission
/// </summary>
/// <remarks>
/// todo: This should be refactored with TacticalStateManager into 1 class that is extended for each state
/// </remarks>
public class PlayStateManager : MonoBehaviour {

    public static PlayStateManager instance;
    
    [Header("Config")]
    [Tooltip("Layer mask for the VRTK bodyPhysics, play mode mask should ignore geometry of the tactical overworld")]
    public LayerMask bodyPhysicsLayerMaskPlay;

    [HideInInspector] public bool initialized = false;

    private VRTK_SlideObjectControlAction[] sliders;
    private VRTK_CustomRaycast bodyPhysRaycast;
    private Transform vrRig;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        vrRig = GameObject.FindObjectOfType<SteamVR_ControllerManager>().transform;
        sliders = vrRig.GetComponentsInChildren<VRTK_SlideObjectControlAction>();
        bodyPhysRaycast = GameObject.FindObjectOfType<VRTK_CustomRaycast>();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        MissionStateManager.instance.OnSetNullState += DisableSelf;
        MissionStateManager.instance.OnSetPlayState += EnableSelf;
        MissionStateManager.instance.OnSetTacticalState += DisableSelf;

        initialized = true;
        DisableSelf();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MissionStateManager.instance.StartTacticalMode();
        }
    }

    /// <summary>
    /// Disable the PlayStateManager 
    /// </summary>
    private void DisableSelf()
    {
        enabled = false;
    }

    /// <summary>
    /// Enable the PlayStateManager
    /// </summary>
    /// <param name="charIndex"></param>
    private void EnableSelf(int charIndex)
    {
        enabled = true;

        vrRig.localScale = Vector3.one;

        UpdateControllerSliderSpeed();
        SetLayerOnAll(vrRig.gameObject, LayerMask.NameToLayer("VRRig"));
        bodyPhysRaycast.layersToIgnore = bodyPhysicsLayerMaskPlay;
    }

    /// <summary>
    /// Set the layer of all children of an object to a layer
    /// </summary>
    /// <remarks>
    /// todo: Should be a static helper function somewhere
    /// </remarks>
    /// <param name="obj"></param>
    /// <param name="layer"></param>
    private void SetLayerOnAll(GameObject obj, int layer)
    {
        foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layer;
        }
    }

    /// <summary>
    /// Set the movement speed for controller movement
    /// </summary>
    private void UpdateControllerSliderSpeed()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            //todo use player move speed
            sliders[i].maximumSpeed = 3;
        }
    }
}
