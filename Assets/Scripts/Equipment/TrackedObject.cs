using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackedObject : MonoBehaviour {
    
    private Vector3 startScale;
    private Vector3 startPosition;
    private Vector3 startRotation;

    protected virtual void Awake()
    {
        startScale = transform.localScale;
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }

    protected virtual void ResetObject()
    {
        transform.localScale = startScale;
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(startRotation);
    }

    public void Reset()
    {
        ResetObject();
    }

}
