using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : Door
{
    [Header("Config")]
    [Tooltip("Direction the door will open in")]
    public Vector3 DoorOpenDirection = Vector3.left; //todo directions that arent left right forward?
    [Tooltip("How far the door will open")]
    public float DoorOpenDistance = 1; //todo have an option to just open the size of the door instead of manual
    [Tooltip("How fast the door will open and close")]
    public float DoorOpenSpeed = 1; //todo different open and close speeds?

    private Vector3 startPosition;
    private Vector3 finalPosition;
    private float openProgress = 0;
    private Coroutine toggleDelay;

    public enum SlidingDoorState
    {
        Closed,
        Open,
        Closing,
        Opening
    }
    public SlidingDoorState slidingDoorState = SlidingDoorState.Closed;

    private enum SlidingDoorType
    {
        Mechanized, //Opens mechanically by using buttons/pads/controls/etc
        Physics //Can be slid open, grabbed and pulled open and closed
    }
    private SlidingDoorType slidingDoorType = SlidingDoorType.Mechanized;

    protected override void Start()
    {
        base.Start();

        startPosition = transform.position;
        finalPosition = transform.position + (DoorOpenDirection * DoorOpenDistance);
    }

    protected override void Update()
    {
        base.Update();

        if (slidingDoorState == SlidingDoorState.Closed || slidingDoorState == SlidingDoorState.Open)
            return;

        if (slidingDoorState == SlidingDoorState.Opening)
        {
            openProgress += DoorOpenSpeed * Time.deltaTime;
        }
        else if (slidingDoorState == SlidingDoorState.Closing)
        {
            openProgress -= DoorOpenSpeed * Time.deltaTime;
        }
        openProgress = Mathf.Clamp(openProgress, 0.0f, 1.0f);
        Vector3 newPosition = Vector3.Lerp(startPosition, finalPosition, openProgress);
        transform.position = newPosition;

        if (openProgress <= 0)
        {
            slidingDoorState = SlidingDoorState.Closed;
        }
        else if (openProgress >= 1)
        {
            slidingDoorState = SlidingDoorState.Open;
        }
    }

    /// <summary>
    /// Tries to open the door and respects locking mechanisms
    /// </summary>
    public void TryOpen()
    {
        if (slidingDoorState == SlidingDoorState.Open || slidingDoorState == SlidingDoorState.Opening)
            return;
        
        if (doorState == DoorState.Locked || doorState == DoorState.UnlockedStuck)
            return;

        slidingDoorState = SlidingDoorState.Opening;
    }

    /// <summary>
    /// Tries to close the door and respects locking mechanisms
    /// </summary>
    public void TryClose()
    {
        if (slidingDoorState == SlidingDoorState.Closed || slidingDoorState == SlidingDoorState.Closing)
            return;

        if (doorState == DoorState.Locked || doorState == DoorState.UnlockedStuck)
            return;

        slidingDoorState = SlidingDoorState.Closing;
    }

    /// <summary>
    /// Opens the door without checking any restrictions
    /// </summary>
    public void ForceOpen()
    {
        slidingDoorState = SlidingDoorState.Opening;
    }

    /// <summary>
    /// Closes the door without checking any restrictions
    /// </summary>
    public void ForceClose()
    {
        slidingDoorState = SlidingDoorState.Closing;
    }

    /// <summary>
    /// Will attempt to trigger the doors opening or closing respective of its current state
    /// </summary>
    public void ToggleDoorOpening()
    {
        if (toggleDelay != null)
            return;

        toggleDelay = StartCoroutine(ToggleDelay());
        if (slidingDoorState == SlidingDoorState.Closed || slidingDoorState == SlidingDoorState.Closing)
            TryOpen();
        else if (slidingDoorState == SlidingDoorState.Open || slidingDoorState == SlidingDoorState.Opening)
            TryClose();
    }

    /// <summary>
    /// A delay on toggling the door open and close to prevent pressing multiple times on accident
    /// </summary>
    /// <returns></returns>
    private IEnumerator ToggleDelay()
    {
        float t = .1f;
        while (t > 0)
        {
            yield return null;
            t -= Time.deltaTime;
        }
        toggleDelay = null;
    }

    /// <summary>
    /// Resets the door to its initial state
    /// </summary>
    protected override void ResetControl()
    {
        base.ResetControl();

        slidingDoorState = SlidingDoorState.Closed;
        transform.position = startPosition;
        openProgress = 0;
        if (toggleDelay != null)
        {
            StopCoroutine(ToggleDelay());
            toggleDelay = null;
        }
    }
}
