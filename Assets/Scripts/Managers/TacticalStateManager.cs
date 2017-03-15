using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalStateManager : MonoBehaviour {

    public static TacticalStateManager instance;

    [HideInInspector]
    public bool initialized = false;
    [SerializeField]
    private VRTK.VRTK_DestinationMarker LeftControllerPointer;
    [SerializeField]
    private VRTK.VRTK_DestinationMarker RightControllerPointer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    private void Start()
    {
        MissionStateManager.instance.OnSetNullState += DisableSelf;
        MissionStateManager.instance.OnSetPlayState += DisableSelf;
        MissionStateManager.instance.OnSetTacticalState += EnableSelf;

        initialized = true;
        DisableSelf();
    }

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

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void DisableSelf()
    {
        DisableSelf(-1);
    }

    private void DisableSelf(int charIndex)
    {
        enabled = false;
    }

    private void EnableSelf()
    {
        enabled = true;
    }
}
