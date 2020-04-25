using UnityEngine;
using System.Collections;

public class UniqueTrailMaterial : MonoBehaviour {

    public Material SourceMaterial;
    private Material _clonedMaterial;

	// Use this for initialization
	void Start () {
        _clonedMaterial = Instantiate(SourceMaterial);
        GetComponent<TrailRenderer>().material = _clonedMaterial;
	}

    void OnDestroy()
    {
        GameObject.Destroy(_clonedMaterial);
        _clonedMaterial = null;
    }
}
