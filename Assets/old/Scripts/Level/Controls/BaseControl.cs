using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseControl : MonoBehaviour {


    protected virtual void Start()
    {
        MissionStateManager.instance.OnSetTacticalState += ResetControl;
    }

    protected virtual void Update()
    {

    }

    protected abstract void ResetControl();
}
