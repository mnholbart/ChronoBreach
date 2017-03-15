using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryMaterialSwitcher : MonoBehaviour {

    public Material PlayMaterial;
    public Material TacticalMaterial;
    
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetPlayMaterial()
    {
        if (PlayMaterial == null)
            return;

        meshRenderer.material = PlayMaterial;
    }

    public void SetTacticalMaterial()
    {
        if (TacticalMaterial == null)
            return;

        meshRenderer.material = TacticalMaterial;
    }

}
