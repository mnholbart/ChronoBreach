using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The PlayerSpawnIndicator, when enabled, gives a visual indicator of valid drop locations for the player object
/// </summary>
/// <remarks>
/// 
/// </remarks>
public class PlayerSpawnIndicator : MonoBehaviour
{
    [Header("References")]
    public PlayerInteractableObject interactableObject;
    public LineRenderer lineRenderer;

    [Header("Config")]
    [Tooltip("LayerMask for valid floors to raycast against")]
    public LayerMask FloorRaycastLayerMask;
    [Tooltip("Color to display when a spawn is valid")]
    public Color validColor;
    [Tooltip("Color to display when a spawn is NOT valid")]
    public Color invalidColor;

    /// <summary>
    /// 
    /// </summary>
	void Update ()
    {
		if (interactableObject.IsValidDropPoint())
        {
            SetColor(validColor);
        } else
        {
            SetColor(invalidColor);
        }
        float h = GetDistanceFromFloor();
        lineRenderer.SetPosition(1, transform.InverseTransformDirection(Vector3.down) * h);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        Update();
        lineRenderer.enabled = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        lineRenderer.enabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    private void SetColor(Color c)
    {
        lineRenderer.startColor = c;
        lineRenderer.endColor = c;
    }

    /// <summary>
    /// Get the distance from any allowable floor under the FloorRaycastLayerMask
    /// </summary>
    /// <returns></returns>
    private float GetDistanceFromFloor()
    {
        Ray r = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 5, FloorRaycastLayerMask))
        {
            return hit.distance;
        }
        return 0; 
    }
}
