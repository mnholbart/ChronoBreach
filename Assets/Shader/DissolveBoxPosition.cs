using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DissolveBoxPosition : MonoBehaviour {
	public Vector4 boxSize;
	public Vector4 boxPosition;
	public bool isVisible = false;

	public GameObject gizmo = null;

	void Update() {
		if (gizmo)
		{
			if (isVisible == true)
				gizmo.SetActive(true);
			else
				gizmo.SetActive(false);

			gizmo.transform.localScale = boxSize;
			gizmo.transform.localPosition = boxPosition;

			Shader.SetGlobalVector("_BoxSize", boxSize);
			Shader.SetGlobalVector("_BoxPos", transform.TransformPoint(boxPosition));
		}
	}
}
