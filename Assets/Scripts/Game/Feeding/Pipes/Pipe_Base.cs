using UnityEngine;
using System.Collections.Generic;

public class Pipe_Base : SPipe
{
    public override void InitPipe(int parameter, int color, bool onstart = false)
    {
		_destroyed = false;
		Param = parameter;
		AColor = color;
		_movable = false;
    }

//	protected override void OnDisable()
//	{
////		if (GameManager.Instance != null && GameManager.Instance.BoardData != null)
////		{
////			
////		}
//	}

//	public override void Update()
//	{
//		//TODO add resources, or something
//	}

	public override void BaseConsumAnimation(int value, int color)
	{
		// animation of base or storage when colored pipe slides to it
		//TODO
		GameManager.Instance.BoardData.AddResourceByLevelOfColoredPipe(value, color, 1, transform.position);
	}

}