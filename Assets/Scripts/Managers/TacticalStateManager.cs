using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// The TacticalStateManager acts as a less authoritative GameManager during the Tactical state of a mission
/// </summary>
/// <remarks>
/// todo: This should be refactored with PlayStateManager into 1 class that is extended for each state
/// </remarks>
public class TacticalStateManager : MonoBehaviour {

    public static TacticalStateManager instance;

    [Header("Config")]
    [Tooltip("Layer mask for the VRTK bodyPhysics, tactical mode mask should ignore geometry of the play world to prevent interaction like being able to open doors")]
    public LayerMask bodyPhysicsLayerMaskTactical;
    [Tooltip("Scalar for size of the VR rig in tactical mode")]
    public float tacticalRigScalar = 10;
    [Tooltip("Movement speed during tactical mode of the VR rig")]
    public float tacticalMoveSpeed = 6;

    [HideInInspector] public bool initialized = false;

    private VRTK_SlideObjectControlAction[] sliders;
    private VRTK_CustomRaycast bodyPhysRaycast;
    private Vector3 tacticalSpawnPosition;
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
        tacticalSpawnPosition = vrRig.position;
        bodyPhysRaycast = GameObject.FindObjectOfType<VRTK_CustomRaycast>();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        MissionStateManager.instance.OnSetNullState += DisableSelf;
        MissionStateManager.instance.OnSetPlayState += DisableSelf;
        MissionStateManager.instance.OnSetTacticalState += EnableSelf;

        initialized = true;
        DisableSelf();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// todo: change from input driven to an interaction in the tactical world, or both with these only in use during editor/debug mode
    /// </remarks>
    private void Update()
    {
        //todo: not input driven
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MissionStateManager.instance.StartPlayMode(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MissionStateManager.instance.StartPlayMode(1);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void DisableSelf()
    {
        DisableSelf(-1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="charIndex"></param>
    private void DisableSelf(int charIndex)
    {
        enabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void EnableSelf()
    {
        enabled = true;

        vrRig.localScale = Vector3.one * tacticalRigScalar;
        vrRig.position = tacticalSpawnPosition;

        UpdateControllerSliderSpeed();
        SetLayerOnAll(vrRig.gameObject, "VRRigTactical");
        bodyPhysRaycast.layersToIgnore = bodyPhysicsLayerMaskTactical;
    }

    /// <summary>
    /// Set the layer of all children of an object to a layer
    /// </summary>
    /// <remarks>
    /// todo: Should be a static helper function somewhere
    /// </remarks>
    /// <param name="obj"></param>
    /// <param name="layer"></param>
    private void SetLayerOnAll(GameObject obj, string layer)
    {
        foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
        {
            if (LayerMask.LayerToName(trans.gameObject.layer).Contains("IgnoreRaycast"))
                trans.gameObject.layer = LayerMask.NameToLayer(layer + "IgnoreRaycast");
            else trans.gameObject.layer = LayerMask.NameToLayer(layer);
        }
    }

    /// <summary>
    /// Set the movement speed for controller movement
    /// </summary>
    private void UpdateControllerSliderSpeed()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].maximumSpeed = tacticalMoveSpeed;
        }
    }
}
