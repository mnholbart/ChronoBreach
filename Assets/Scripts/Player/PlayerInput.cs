using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    public float MovementSpeed = 2.0f;
    public float mouseRotationModifier = 2.0f;

    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private InputVCR VCR;
    [SerializeField]
    private Player player;

    private Vector3 lastMouseVector = new Vector3(0, 0, 0);
    private VRTK.SDK_InputSimulator inputSimulator;

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

    }

    void Update ()
    {
        if (mode == Mode.Idle)
            return;

        Vector3 moveVector = new Vector3(0, 0, 0);
        moveVector.z += VCR.GetAxis("Vertical") * Time.deltaTime * MovementSpeed;
        moveVector.x += VCR.GetAxis("Horizontal") * Time.deltaTime * MovementSpeed;

        Vector3 mouseVector = new Vector3(0, 0, 0);
        mouseVector.x += VCR.GetAxis("Mouse X");
        mouseVector.y += VCR.GetAxis("Mouse Y");
        Vector3 mouseDelta = mouseVector - lastMouseVector;
        
        if (inputSimulator != null)
        {
            inputSimulator.UpdatePosition(moveVector);
            inputSimulator.UpdateRotation(mouseDelta, mouseRotationModifier);
        }
        else //Default replay input
        {
            //rotation
            Vector3 rot = transform.rotation.eulerAngles;
            rot.y += (mouseDelta * mouseRotationModifier).x;
            transform.localRotation = Quaternion.Euler(rot);

            //position
            //transform.Translate(moveVector, Space.Self);
            moveVector = transform.TransformDirection(moveVector);
            controller.Move(moveVector);
        }
    }

    public void SetRecordMode(GameObject vrRig, GameManager.VRTKMode vrMode)
    {
        if (vrMode == GameManager.VRTKMode.Simulator)
            inputSimulator = vrRig.GetComponent<VRTK.SDK_InputSimulator>();

        controller.enabled = false;

        mode = Mode.Record;
        VCR.NewRecording();
    }

    public void SetPlaybackMode()
    {
        if (VCR.GetRecording() == null)
        {
            SetIdleMode();
            return;
        }

        controller.enabled = true;

        mode = Mode.Playback;
        VCR.Play();
        StartCoroutine(Playback());
    }

    private IEnumerator Playback()
    {
        ResetInputs();

        while (VCR.mode != InputVCRMode.Passthru)
            yield return null;

        SetIdleMode();
    }

    public void SetIdleMode()
    {
        mode = Mode.Idle;
        VCR.Stop();
    }

    private void ResetInputs()
    {
        inputSimulator = null;
    }
}
