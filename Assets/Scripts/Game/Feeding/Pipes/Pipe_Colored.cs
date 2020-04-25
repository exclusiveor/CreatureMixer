using UnityEngine;
using System.Collections.Generic;

public class Pipe_Colored : SPipe
{
	private int 					_currentSide = 0;
	//public List<SpriteRenderer> 	SymbolSprites;
    public GameObject[]             ExplodeEffects;
	public Renderer 				ARenderer;

    public override void InitPipe(int parameter, int color, bool onstart = false)
    {
		_destroyed = false;
		AColor = color;
		SetValueForce(parameter);
		_movable = true;
    }

//	public override void Update()
//	{
//		//TODO
//	}

    public GameObject GetExplodeEffectPrefab()
    {
        return ExplodeEffects[AColor];
    }

	public static Color GetColorByNumber(int color)
	{
		return Consts.COLORS[color];
	}

	public override void RaseCombineAnimation(int dirX, int dirY)
	{
        // animation of colored pipe when other pipe slides to it
        // GameManager.Instance.BoardData.AGameBoard.ShakeCamera(Consts.SHAKE_POWER_ON_PIPE_COMBINE, Consts.SHAKE_POWER_ON_PIPE_COMBINE, Consts.SHAKE_TIME_ON_PIPE_COMBINE);
		SetValue(Param + 1, dirX, dirY);
	}

	public override void RemoveCombineAnimation()
	{
		// animation of colored pipe when it combines after slide
		gameObject.SetActive(false);
	}

	public override void RemoveConsumAnimation()
	{
		// animation of colored pipe when colored pipe slides to base or storage
		gameObject.SetActive(false);
	}

	public void SetValue(int param, int dirX, int dirY)
	{
		// reset rotation of pipe and scale of value sprites
		MusicManager.playSound("chip_rotate");
		ResetCubesTransform();
		Param = param;
		_currentSide = 1;
        LeanTweenType rotateEase = LeanTweenType.easeOutSine;
        UpdateSkin();
		// rotate pipe
		if (dirX!= 0)
		{
			// horizontal move
			//Vector3 newScale = SymbolSprites[1].transform.localScale;
			//newScale.x = -newScale.x;
			//SymbolSprites[1].transform.localScale = newScale; // flip
			if (dirX  < 0)
			{
				LeanTween.rotateLocal(RotateObject, new Vector3(0, -180, 0), Consts.MATCH_ROTATE_TIME)
					.setEase(rotateEase);
			} else
			{
				LeanTween.rotateLocal(RotateObject, new Vector3(0, 180, 0), Consts.MATCH_ROTATE_TIME)
					.setEase(rotateEase);
			}
		} else
		{
			// vertical move
			//Vector3 newScale = SymbolSprites[1].transform.localScale;
			//newScale.y = -newScale.y;
			//SymbolSprites[1].transform.localScale = newScale; // flip
			Vector3 newScale = RotateObject.transform.localScale;
			newScale.y = -newScale.y;
			newScale.x = -newScale.x;
			RotateObject.transform.localScale = newScale;
			if (dirY < 0)
			{
				LeanTween.rotateLocal(RotateObject, new Vector3(180, 0, 0), Consts.MATCH_ROTATE_TIME)
					.setEase(rotateEase);
			} else
			{
				LeanTween.rotateLocal(RotateObject, new Vector3(-180, 0, 0), Consts.MATCH_ROTATE_TIME)
					.setEase(rotateEase);
			}
		}
	}

	public void SetValueForce(int param)
	{
        Param = param;
		ResetCubesTransform();
		RotateObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        UpdateSkin();
    }
		
	public override void UpdateSkin()
	{

		ARenderer.material = GameManager.Instance.Game.GetMaterialForColoredPipe(AColor, Param);
	}    

	private void ResetCubesTransform()
	{
		_currentSide = 0;
		Vector3 newScale = RotateObject.transform.localScale;
		newScale.x = Mathf.Abs(newScale.x);
		newScale.y = Mathf.Abs(newScale.y);
		RotateObject.transform.localScale = newScale;
		RotateObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
		//Vector3 newScale = SymbolSprites[1].transform.localScale;
		//newScale.x = Mathf.Abs(newScale.x);
		//newScale.y = Mathf.Abs(newScale.y);
		//SymbolSprites[1].transform.localScale = newScale;
	}

}