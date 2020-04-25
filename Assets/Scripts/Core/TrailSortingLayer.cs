using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrailSortingLayer : MonoBehaviour
{
    public TrailRenderer Trail;
    public string SortingLayerName = "";
    public int SortingOrder = 0;

    // Use this for initialization
    void Start()
    {
        Trail.sortingLayerName = SortingLayerName;
        Trail.sortingOrder = SortingOrder;
    }
}