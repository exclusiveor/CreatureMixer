using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TutorialData 
{
	public string								Id;
	public string								Type; // "ui"(in ui coords) , "scene" or "scroll"(position changed with camera move, so popup olways visible, in scene coors)
	public float								X;
	public float								Y;
	public bool									IsArrow; // true or false
	public string								Align; // auto, left, right, top, bottom
}