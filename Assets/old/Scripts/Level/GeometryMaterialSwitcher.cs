using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The GeometryMaterialSwitcher switches material type based on game play or tactical state
/// </summary>
/// <remarks>
/// This script should probably be extended into a play and tactical type script for different pieces of geometry to handle things like layer or tag changes
/// </remarks>
/// <example>
/// Attach to any object and assign a material for its play and tactical states
/// </example>
public class GeometryMaterialSwitcher : MonoBehaviour {

    [Header("Config")]
    public Material playMaterial;
    public Material tacticalMaterial;
    
    private MeshRenderer meshRenderer; //todo maybe this should be set as a reference depending on performance hit, probably better to do it but make an automated editor script so every piece of geometry made doesnt have to be assigned

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Updates material to our playMaterial
    /// </summary>
    public void SetPlayMaterial()
    {
        if (playMaterial == null)
            return;

        meshRenderer.material = playMaterial;
    }

    /// <summary>
    /// Updates material to our tacticalMaterial
    /// </summary>
    public void SetTacticalMaterial()
    {
        if (tacticalMaterial == null)
            return;

        meshRenderer.material = tacticalMaterial;
    }

}
