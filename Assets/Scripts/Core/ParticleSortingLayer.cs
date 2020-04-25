using UnityEngine;
using System.Collections;

public class ParticleSortingLayer : MonoBehaviour
{
    public string SortingLayerName = "";
    public int SortingOrder = 0;
    Renderer _renderer;

    void Awake() {
        _renderer = GetComponent<ParticleSystem>().GetComponent<Renderer>();
    }

    void Start()
    {
        // Set the sorting layer of the particle system.
        if (_renderer != null)
        {
            _renderer.sortingLayerName = SortingLayerName;
            _renderer.sortingOrder = SortingOrder;
        } else {
            Debug.Log("Renderer=null");
        }
    }
}