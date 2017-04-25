using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerInput_Simulator : SDK_InputSimulator {

    public bool recording = false;

    public void StartRecording(InputVCR vcr)
    {

    }

    public void StopRecording()
    {

    }

    protected override void UpdatePosition()
    {
        Vector3 moveVector = new Vector3(0, 0, 0);
        moveVector.z += Input.GetAxis("Vertical");
        moveVector.x += Input.GetAxis("Horizontal");
        moveVector.Normalize();
        moveVector = transform.TransformDirection(moveVector * playerMoveMultiplier * Time.deltaTime);

        GetComponent<Rigidbody>().MovePosition(transform.position + moveVector);
    }

    public Vector3 GetRightHandPosition()
    {
        return rightHand.transform.position;
    }

    public Vector3 GetLeftHandPosition()
    {
        return leftHand.transform.position;
    }
}
