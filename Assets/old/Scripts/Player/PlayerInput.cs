using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The PlayerInput script manages how the player will proccess its movement
/// </summary>
/// <remarks>
/// todo: currently only implements SteamVR and still has a few hiccups with choppy movement and untested movement of the rigidbody.moveposition and changes in continuity
/// </remarks>
public class PlayerInput : MonoBehaviour {

    /// <summary>
    /// What mode of input the player is going to function on
    /// </summary>
    /// <param name="Idle">No input will be proccessed</param>
    /// <param name="Record">Input will be proccessed by recording and following the movement of the VR Rig</param>
    /// <param name="Playback">Input will be processed as a playback of a recording</param>
    public enum Mode
    {
        Idle,
        Record,
        Playback
    }

    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform mesh;
    [SerializeField] private InputVCR VCR;
    [SerializeField] private Player player;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Rigidbody rb;

    [Header("Config")]
    [Tooltip("Adjust hand movement speed.")]
    public float handMoveMultiplier = 0.2f;
    [Tooltip("Adjust player movement speed")]
    public float movementSpeed = 2.0f;
    [Tooltip("Adjust mouse sensitivity")]
    public float mouseRotationModifier = 2.0f;

    [HideInInspector]
    public Mode mode
    {
        get { return _mode; }
        private set { _mode = value; }
    }

    private Vector3 rightHandStartPosition;
    private Vector3 leftHandStartPosition;
    private Vector3 oldPos = Vector3.zero;    

    //VRRig Simulator stuff
    private PlayerInput_Simulator inputSimulator;
    private SteamVR_ControllerManager steamVRRig;
    private bool handMode = false;
    private bool rightHandActive = true;
    private Transform currentHand = null;
    private Mode _mode = Mode.Idle;
    private bool isDead;

    private const string RIGHTHAND_POS_PROPERTY = "righthandpos";
    private const string LEFTHAND_POS_PROPERTY = "lefthandpos";
    private const string POSITION_PROPERTY = "position";
    private const string ROTATION_PROPERTY = "rotation";
    private const string SCALE_PROPERTY = "scale";

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        leftHandStartPosition = leftHand.transform.localPosition;
        rightHandStartPosition = rightHand.transform.localPosition;

        player.OnPlayerDeath += OnPlayerDeath;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update ()
    {
        if (isDead)
            return;

        if (mode == Mode.Record)
        {
            RecordUpdate();
        }
        if (mode == Mode.Playback)
        {
            PlaybackUpdate();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void RecordUpdate()
    {
        UpdatePlayerPositions();
        SyncPlayerPositions();
    }

    /// <summary>
    /// Update player positions to stay with the VR rig
    /// </summary>
    private void UpdatePlayerPositions()
    {
        if (inputSimulator != null)
        {
            rightHand.transform.position = inputSimulator.GetRightHandPosition();
            leftHand.transform.position = inputSimulator.GetLeftHandPosition();
        }
        else if (steamVRRig != null)
        {
            //todo probably needs a minimum height and definitely not negative 
            Transform headset = VRTK.VRTK_SDKManager.instance.actualHeadset.transform;
            controller.height = headset.localPosition.y;
            Vector3 newScale = mesh.localScale;
            newScale.y = controller.height / 2;
            mesh.localScale = newScale;
            transform.position = headset.position - new Vector3(0, mesh.localScale.y, 0);
            rightHand.transform.position = steamVRRig.right.transform.position;
            leftHand.transform.position = steamVRRig.left.transform.position;
        }
    }

    /// <summary>
    /// Sync player positions with the recording
    /// </summary>
    private void SyncPlayerPositions()
    {
        VCR.SyncProperty(POSITION_PROPERTY, InputVCR.Vector3ToString(transform.position));
        VCR.SyncProperty(RIGHTHAND_POS_PROPERTY, InputVCR.Vector3ToString(rightHand.transform.localPosition));
        VCR.SyncProperty(LEFTHAND_POS_PROPERTY, InputVCR.Vector3ToString(leftHand.transform.localPosition));
        VCR.SyncProperty(ROTATION_PROPERTY, InputVCR.Vector3ToString(transform.eulerAngles));
        VCR.SyncProperty(SCALE_PROPERTY, InputVCR.Vector3ToString(mesh.localScale));
    }

    /// <summary>
    /// 
    /// </summary>
    private void PlaybackUpdate()
    {
        PlaybackHandPositions();
        PlaybackPlayerPosition();
    }

    /// <summary>
    /// Sync hand positions with the recording
    /// </summary>
    private void PlaybackHandPositions()
    {
        if (!string.IsNullOrEmpty(VCR.GetProperty(RIGHTHAND_POS_PROPERTY)))
        {
            Vector3 rightPos = transform.TransformPoint(InputVCR.ParseVector3(VCR.GetProperty(RIGHTHAND_POS_PROPERTY)));
            rightHand.transform.position = rightPos;
        }
        if (!string.IsNullOrEmpty(VCR.GetProperty(LEFTHAND_POS_PROPERTY)))
        {
            Vector3 leftPos = transform.TransformPoint(InputVCR.ParseVector3(VCR.GetProperty(LEFTHAND_POS_PROPERTY)));
            leftHand.transform.position = leftPos;
        }
    }

    /// <summary>
    /// Sync player position with the recording
    /// </summary>
    private void PlaybackPlayerPosition()
    {
        if (!string.IsNullOrEmpty(VCR.GetProperty(POSITION_PROPERTY)))
        {
            Vector3 pos = InputVCR.ParseVector3(VCR.GetProperty(POSITION_PROPERTY));
            //rb.MovePosition(pos);
            controller.Move(pos - transform.position);
        }

        if (!string.IsNullOrEmpty(VCR.GetProperty(ROTATION_PROPERTY)))
        {
            Vector3 rot = InputVCR.ParseVector3(VCR.GetProperty(ROTATION_PROPERTY));
            //rb.MoveRotation(Quaternion.Euler(rot));
            transform.rotation = Quaternion.Euler(rot);
        }

        if (!string.IsNullOrEmpty(VCR.GetProperty(SCALE_PROPERTY)))
        {
            Vector3 scale = InputVCR.ParseVector3(VCR.GetProperty(SCALE_PROPERTY));
            mesh.localScale = scale;
            controller.height = 2 * scale.y;
        }
    }

    /// <summary>
    /// Turns on record mode to record input for this character
    /// </summary>
    public void SetRecordMode(GameObject vrRig, GameManager.VRTKMode vrMode)
    {
        inputSimulator = null;
        steamVRRig = null;
        if (vrMode == GameManager.VRTKMode.Simulator)
        {
            inputSimulator = vrRig.GetComponent<PlayerInput_Simulator>();
            inputSimulator.handMoveMultiplier = handMoveMultiplier;
            inputSimulator.playerRotationMultiplier = mouseRotationModifier;
            inputSimulator.playerMoveMultiplier = movementSpeed;
            inputSimulator.StartRecording(VCR);
        } else if (vrMode == GameManager.VRTKMode.SteamVR)
        {
            steamVRRig = vrRig.GetComponent<SteamVR_ControllerManager>();
        }

        isDead = false;
        controller.enabled = false;
        leftHand.gameObject.SetActive(false);
        rightHand.gameObject.SetActive(false);

        mode = Mode.Record;
        VCR.NewRecording();
    }

    /// <summary>
    /// Set to playback mode to replay a recording if it exists
    /// </summary>
    public void SetPlaybackMode()
    {
        if (VCR.GetRecording() == null)
        {
            SetIdleMode();
            return;
        }
        isDead = false;
        controller.enabled = true;
        leftHand.gameObject.SetActive(true);
        rightHand.gameObject.SetActive(true);

        mode = Mode.Playback;
        VCR.finishedPlayback += SetIdleMode;
        VCR.Play();
        ResetInputs();
    }

    /// <summary>
    /// Disable recording or playback
    /// </summary>
    public void SetIdleMode()
    {
        VCR.finishedPlayback -= SetIdleMode;
        mode = Mode.Idle;
        VCR.Stop();
        isDead = false;
        controller.enabled = true;
        leftHand.gameObject.SetActive(true);
        rightHand.gameObject.SetActive(true);
    }

    /// <summary>
    /// Reset anything input related for respawn
    /// </summary>
    private void ResetInputs()
    {
        rightHand.transform.localPosition = rightHandStartPosition;
        leftHand.transform.localPosition = leftHandStartPosition;
        oldPos = VCR.mousePosition;

        //Simulator
        handMode = false;
        rightHandActive = true;
        currentHand = rightHand;
        if (inputSimulator != null)
        {
            inputSimulator.StopRecording();
            inputSimulator = null;
        }

        //steamvr
        if (steamVRRig != null)
        {
            steamVRRig = null;
        }
    }
    
    /// <summary>
    /// Get mouse delta from last frame for mouse movement related input
    /// </summary>
    private Vector3 GetMouseDelta()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            return new Vector3(VCR.GetAxis("Mouse X"), VCR.GetAxis("Mouse Y"));
        }
        else
        {
            Vector3 mouseDiff = VCR.mousePosition - oldPos;
            oldPos = VCR.mousePosition;

            return mouseDiff;
        }
    }

    /// <summary>
    /// Update all player input
    /// </summary>
    [System.Obsolete]
    private void UpdatePlaybackInput()
    {
        if (VCR.GetButtonDown("HandMode"))
            handMode = !handMode;

        if (handMode && VCR.GetKeyDown("tab"))
        {
            rightHandActive = !rightHandActive;
            if (rightHandActive)
                currentHand = rightHand;
            else currentHand = leftHand;
        }

        if (!handMode)
            UpdateMovementInput();
    }

    /// <summary>
    /// Update character primary mesh input
    /// </summary>
    [System.Obsolete]
    private void UpdateMovementInput()
    {
        //Position
        Vector3 moveVector = new Vector3(0, 0, 0);
        moveVector.z += VCR.GetAxis("Vertical");
        moveVector.x += VCR.GetAxis("Horizontal");
        moveVector.Normalize();
        moveVector = transform.TransformDirection(moveVector * movementSpeed * Time.deltaTime);
        //rigidbody.MovePosition(moveVector);
        transform.Translate(moveVector);

        //Rotation
        Vector3 mouseDiff = GetMouseDelta();
        Vector3 rot = transform.localRotation.eulerAngles;
        rot.y += (mouseDiff * mouseRotationModifier).x;
        transform.localRotation = Quaternion.Euler(rot);
    }

    /// <summary>
    /// Update character hand input
    /// </summary>
    [System.Obsolete]
    private void UpdateHandInput()
    {
        Vector3 mouseDiff = GetMouseDelta();

        if (VCR.GetKey("left ctrl")) //flip the axis 
        {
            Vector3 pos = Vector3.zero;
            pos += mouseDiff * handMoveMultiplier;
            pos = currentHand.TransformDirection(pos);
            currentHand.GetComponentInChildren<Rigidbody>().MovePosition(currentHand.position + (pos * Time.deltaTime));
        }
        else
        {
            Vector3 pos = Vector3.zero;
            pos.x += (mouseDiff * handMoveMultiplier).x;
            pos.z += (mouseDiff * handMoveMultiplier).y;
            pos = currentHand.TransformDirection(pos);
            //Debug.Log(currentHand.position + pos * Time.deltaTime);
            //currentHand.GetComponentInChildren<Rigidbody>().MovePosition(currentHand.position + (pos * Time.deltaTime));
            currentHand.transform.Translate(pos * Time.deltaTime);
            //Debug.Log(currentHand.position);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    private void OnPlayerDeath(Player p)
    {
        isDead = true;
    }
}
