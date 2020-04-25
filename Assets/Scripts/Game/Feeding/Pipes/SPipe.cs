using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

public enum EPipeType
{
	None			= 	0,
	Colored			=	1,	// кольорові фішки-ресурси
	Base			=	2,	// база				- збирає фішки як завод + головна будівля (гейм овер, якщо втратим)
	Blocker			=	3,
	Hole			=	4,	// дірка в полі, в подальшому можливо ембелішменти
	Last			=	5
}
[System.Serializable]
public enum EPipeStructureType
{
    BackFront = 0, // uses 2 pictures - for back and value
    Solid = 1,     // uses only one sprite
    //Prefab = 2
}

public class SPipe : MonoBehaviour
{
	public EPipeType		PipeType;
	public int				Param;
	public int				AColor;
	public int				X = 0; 
	public int            	Y = 0; 
	public GameObject		RotateObject;
	public GameObject		CubeObject;

	protected bool			_movable;
	protected bool			_destroyed = false;

    public virtual void InitPipe(int parameter, int color, bool onstart = false)
    {
		Param = parameter;
		AColor = color;
		_movable = true;
		_destroyed = false;
    }

	protected virtual void OnDisable()
	{
		
	}

	public bool IsMovable()
	{
		return _movable && !_destroyed; 
	}

	public void SetXY(int x, int y, bool setPositionToo = false)
	{
		X = x;
		Y = y;
		if (setPositionToo)
		{
			gameObject.transform.position = GameBoard.PipePos(X, Y);
		}
	}

	public bool IsColored()
	{
		return PipeType == EPipeType.Colored;
	}

	public virtual void RaseCombineAnimation(int dirX, int dirY)
	{
		// animation of colored pipe when other pipe slides to it
	}

	public virtual void RemoveCombineAnimation()
	{
		// animation of colored pipe when it combines after slide
	}

	public virtual void RemoveConsumAnimation()
	{
		// animation of colored pipe when colored pipe slides to base or storage
	}

	public virtual void BaseConsumAnimation(int value, int color)
	{
		// animation of base or storage when colored pipe slides to it
	}

	public virtual void Update()
	{

	}

	public virtual void PlayAddAnimation()
	{
        // animation when pipe added to board
        //LeanTween.cancel(gameObject);
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        LeanTween.scale(gameObject, new Vector3(1.0f, 1.0f, 1), 0.25f)
            .setEase(LeanTweenType.easeOutBack);
    }

    public virtual void PlayHideAnimation()
    {
        // animation when pipe added to board
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, new Vector3(0.5f, 0.5f, 1), 0.25f)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(()=> { gameObject.SetActive(false); });
    }

    public bool IsCanConsumeColoredPipes()
	{
		return (PipeType == EPipeType.Base);
	}

    public virtual void UpdateSkin()
    {

    }

}