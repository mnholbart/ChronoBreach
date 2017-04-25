using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    [Tooltip("Adjust hand movement speed.")]
    public float handMoveMultiplier = 0.2f;
    [Tooltip("Adjust player movement speed")]
    public float MovementSpeed = 2.0f;
    [Tooltip("Adjust mouse sensitivity")]
    public float mouseRotationModifier = 2.0f;

    [SerializeField] private CapsuleCollider controller;
    [SerializeField] private InputVCR VCR;
    [SerializeField] private Player player;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Rigidbody rigidbody;
    private Vector3 rightHandStartPosition;
    private Vector3 leftHandStartPosition;
    private Vector3 oldPos = Vector3.zero;    

    //VRRig Simulator stuff
    private PlayerInput_Simulator inputSimulator;
    private SteamVR_ControllerManager steamVRRig;
    private bool handMode = false;
    private bool rightHandActive = true;
    private Transform currentHand = null;

    private const string RIGHTHAND_POS_PROPERTY = "righthandpos";
    private const string LEFTHAND_POS_PROPERTY = "lefthandpos";

    public enum Mode
    {
        Idle,
        Record,
        Playback
    }
    private Mode _mode = Mode.Idle;
    public Mode mode
    {
        get { return _mode; }
        private set { _mode = value; }
    }

    private void Awake()
    {
        leftHandStartPosition = leftHand.transform.localPosition;
        rightHandStartPosition = rightHand.transform.localPosition;
    }

    private void Update ()
    {
        if (mode == Mode.Record)
        {
            if (inputSimulator != null)
            {
                rightHand.transform.position = inputSimulator.GetRightHandPosition();
                leftHand.transform.position = inputSimulator.GetLeftHandPosition();
            } else if (steamVRRig != null)
            {
                //todo come up with a way to calculate height from floor and adjust based on headset height
                transform.position = VRTK.VRTK_SDKManager.instance.actualHeadset.transform.position - new Vector3(0,.2f,0);
                rightHand.transform.position = steamVRRig.right.transform.position;
                leftHand.transform.position = steamVRRig.left.transform.position;
            }
            VCR.SyncProperty(RIGHTHAND_POS_PROPERTY, InputVCR.Vector3ToString(rightHand.transform.localPosition));
            VCR.SyncProperty(LEFTHAND_POS_PROPERTY, InputVCR.Vector3ToString(leftHand.transform.localPosition));

            VCR.SyncPosition();
        }

        if (mode == Mode.Playback)
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
            //UpdatePlaybackInput();
        }
    }

    /// <summary>
    /// Update all player input
    /// </summary>
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
    private void UpdateMovementInput()
    {
        //Position
        Vector3 moveVector = new Vector3(0, 0, 0);
        moveVector.z += VCR.GetAxis("Vertical");
        moveVector.x += VCR.GetAxis("Horizontal");
        moveVector.Normalize();
        moveVector = transform.TransformDirection(moveVector * MovementSpeed * Time.deltaTime);
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
    [System.Obsolete("Not used anymore or up to date")]
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
            inputSimulator.playerMoveMultiplier = MovementSpeed;
            inputSimulator.StartRecording(VCR);
        } else if (vrMode == GameManager.VRTKMode.SteamVR)
        {
            steamVRRig = vrRig.GetComponent<SteamVR_ControllerManager>();
        }

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
}
