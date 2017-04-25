using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public LayerMask SpawnZoneLayerMask;
    public int myPlayerIndex;

    public event System.Action<GameObject> OnDetachFromSpawnArea;
    public event System.Action<GameObject> OnFailDetachFromSpawnArea;
    [HideInInspector]
    public GameObject VRObject;

    [SerializeField]
    private CapsuleCollider collider;
    [SerializeField]
    private GameObject playerMesh;
    [SerializeField]
    private PlayerInput input;

    [SerializeField]
    private VRTK.VRTK_InteractableObject myInteractableObject;

    private void Awake()
    {
        myInteractableObject.InteractableObjectUngrabbed += OnUngrabbed;
    }

    private void Update()
    {

    }

    public void SetInteractableEnabled(bool b)
    {
        myInteractableObject.enabled = b;
    }

    public void SetToPrimaryPlayer(GameObject vrRig)
    {
        collider.isTrigger = false;
        playerMesh.SetActive(false);
        VRObject = vrRig;
        VRObject.transform.position = transform.position;
        VRObject.transform.rotation = transform.rotation;
        //transform.SetParent(vrRig.transform, true);
        //transform.localPosition += new Vector3(0, 1, 0);
        //VRObject.transform.position -= new Vector3(0, 1, 0);
        input.SetRecordMode(VRObject, GameManager.instance.GetVRMode());
    }

    public void SetToSecondaryPlayer(Transform parent)
    {
        collider.isTrigger = false;
        playerMesh.SetActive(true);
        VRObject = null;
        //transform.SetParent(parent, true);
        input.SetPlaybackMode();
    }

    public void Respawn(Vector3 pos)
    {
        ResetPlayer();
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        collider.isTrigger = true;
    }

    private void ResetPlayer()
    {
        VRObject = null;
        //inputMode = GameManager.VRTKMode.Null;
        //inputSimulator = null;
        playerMesh.SetActive(true);
        input.SetIdleMode();
    }

    private void OnUngrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        Debug.Log("onungrabbed");
        Ray r = new Ray(e.interactingObject.transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 5, SpawnZoneLayerMask))
        {
            OnDetachFromSpawnArea.Invoke(gameObject);
            Vector3 spawnPoint = hit.point + new Vector3(0, GetComponent<Collider>().bounds.size.y / 2, 0);
            hit.transform.GetComponent<SpawnableLocation>().AttachPlayerObject(gameObject, myPlayerIndex, spawnPoint);
        } else
        {
            OnFailDetachFromSpawnArea.Invoke(gameObject);
        }
    }


}
