using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public delegate void ClickAction(GameObject obj);

public class SSlot : MonoBehaviour {

	//public BoxCollider2D	Collider;

	public int 				X = 0; //			{ get; set; }
	public int 				Y = 0; //			{ get; set; }
	public SPipe     		Pipe = null; // 	{ get; set; }

    public Sprite           SpriteNormal;
    public Sprite           SpriteDouble;
    public SpriteRenderer   Image;

    public int      		Id 			{ get; set; }
	public bool				WaitForPipe	{ get; set; }

	private Vector2			MouseDownPos;
    public bool             IsDoubleSlot { get; set; }

	#if UNITY_ANDROID || UNITY_IOS
		private int				TouchId;
	#endif

    public void InitSlot(int x, int y)
    {
        X = x;
        Y = y;
        Id = X * 100 + Y;
		name = "Slot_" + X + "_" + Y;
		WaitForPipe = false;

        IsDoubleSlot = false;
        Image.sprite = SpriteNormal;
    }

    public void AddSlotDouble()
    {
        IsDoubleSlot = true;
        Image.sprite = SpriteDouble;
    }

    public void InitSavedSlot(SSlotData sData)
	{
		// for saved slots with pipe
		X = sData.x;
		Y = sData.y;
		Id = X * 100 + Y;
		name = "Slot_" + X + "_" + Y;
		WaitForPipe = false;
	}
		
	public bool IsEmpty() 
	{
		return (!WaitForPipe && Pipe == null);
	}

	public void SetPipe(SPipe apipe, bool setPositionToo = true) 
	{
		Pipe = apipe;
        Pipe.SetXY(X, Y, setPositionToo);
	}

//    public void ApplyPipe(SPipe apipe)
//    {
//        Pipe = apipe;
//        Pipe.SetXY(X, Y, false);
//    }
	
	public SPipe TakePipe() 
	{
		SPipe res = Pipe;
		Pipe = null;
		return res;
	}

	public bool IsMovable()
	{
		return (!WaitForPipe);
	}

	public void SetAsHole()
	{
		//gameObject.SetActive(false);
		//Collider.enabled = false;
	}

	public void SetAsNotHole()
	{
		//gameObject.SetActive(true);
		//Collider.enabled = true;
	}


//	public void OnMouseDown()
//	{
//		if (GameManager.Instance.CurrentMenu != UISetType.ClassicGame && GameManager.Instance.CurrentMenu != UISetType.LeveledGame)
//		{
//			return;
//		}
//		if (Pipe == null || GameManager.Instance.BoardData.GameState != EGameState.Play || GameManager.Instance.BoardData.DragSlot != null)
//		{
//			// can't drag
//			return;
//		}
//
//		if (GameManager.Instance.GameFlow.IsSomeWindow())
//		{
//			//			if (GameManager.Instance.GameFlow.GetCurrentActiveWindowId() == UIConsts.FORM_ID.TUTOR_WINDOW)
//			//			{
//			//				GameManager.Instance.HideTutorial("");
//			//			} else
//			{
//				return;
//			}
//		}
//
//		GameBoard board = GameManager.Instance.BoardData.AGameBoard;
//		if (board.BreakePowerup)
//		{
//			board.OnBreakePowerupUsed(this);
//			return;
//		} else
//		if (board.ChainPowerup)
//		{
//			board.OnChainPowerupUsed(this);
//			return;
//		} else
//		if (board.DestroyColorPowerup)
//		{
//			board.OnDestroyColorPowerupUsed(this);
//			return;
//		}
//
//		if (!IsMovable() || !Pipe.IsMovable())
//		{
//			// can't drag - pipe or slot is immovable
//			return;
//		} else
//		{
//			// mark as slot that we drag now
//			GameManager.Instance.BoardData.DragSlot = this;
//			if (Application.isEditor)
//			{
//				MouseDownPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//			}
//			else
//			{
//				#if UNITY_ANDROID || UNITY_IOS
//				if (Input.touchCount > 0)
//				{
//					Touch touch = Input.GetTouch(Input.touchCount - 1);
//					MouseDownPos = Camera.main.ScreenToWorldPoint(touch.position);
//					TouchId = touch.fingerId;
//				}
//				#else
//				MouseDownPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//				#endif
//			}
//			GameManager.Instance.BoardData.AGameBoard.ShowSelection(transform.position);
//		}
//	}
//
//	public void OnMouseUp()
//	{
//		if (GameManager.Instance.CurrentMenu != UISetType.ClassicGame && GameManager.Instance.CurrentMenu != UISetType.LeveledGame)
//		{
//			return;
//		}
//		if (GameManager.Instance.BoardData.DragSlot == this)
//		{
//			GameManager.Instance.BoardData.AGameBoard.HideSelection();
//			GameManager.Instance.BoardData.DragSlot = null;
//
//			Vector2 endPos = Vector2.zero;
//			if (Application.isEditor)
//			{
//				endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//			}
//			else
//			{
//				#if UNITY_ANDROID || UNITY_IOS
//				foreach (Touch touch in Input.touches)
//				{
//					if (touch.fingerId == TouchId)
//					{
//						endPos = Camera.main.ScreenToWorldPoint(touch.position);
//						break;
//					}
//				}
//				#else
//				endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//				#endif
//			}
//			// check if enough distance to slide and find direction
//			float dx = endPos.x - MouseDownPos.x;
//			float dy = endPos.y - MouseDownPos.y;
//			float absDx = Mathf.Abs(dx);
//			float absDy = Mathf.Abs(dy);
//			if (Mathf.Max(absDx, absDy) < Consts.IMPULSE_DISTANCE)
//			{
//				// impulse too short (cancelled impulse)
//
//			} else
//			{
//				if (absDx > absDy)
//				{
//					// try horizontal impulse
//					if (dx > 0)
//					{
//						// impulse to right
//						GameManager.Instance.BoardData.AGameBoard.SlidePipe(this, 1, 0);
//					} else
//					{
//						// impulse to left
//						GameManager.Instance.BoardData.AGameBoard.SlidePipe(this, -1, 0);
//					}
//				} else
//				{
//					// try vertical impulse
//					if (dy > 0)
//					{
//						// impulse to top
//						GameManager.Instance.BoardData.AGameBoard.SlidePipe(this, 0, 1);
//					} else
//					{
//						// impulse to bottom
//						GameManager.Instance.BoardData.AGameBoard.SlidePipe(this, 0, -1);
//					}
//				}
//			}
//		}
//	}

	// INPUT USING POSITIONS

	public void OnMouseDownByPosition(Vector2 pos)
	{
		GameBoard board = GameManager.Instance.BoardData.AGameBoard;
		if (board.BreakePowerup)
		{
			board.OnBreakePowerupUsed(this);
			return;
		}
		else
		if (board.ChainPowerup)
		{
			board.OnChainPowerupUsed(this);
			return;
		}
		else
		if (board.DestroyColorPowerup)
		{
			board.OnDestroyColorPowerupUsed(this);
			return;
		}

		if (!IsMovable() || !Pipe.IsMovable())
		{
			// can't drag - pipe or slot is immovable
			return;
		}
		else
		{
			// mark as slot that we drag now
			GameManager.Instance.BoardData.DragSlot = this;
			MouseDownPos = pos;
			GameManager.Instance.BoardData.AGameBoard.ShowSelection(transform.position);
		}
	}

	public void OnMouseUpByPosition(Vector2 endPos)
	{
		GameManager.Instance.BoardData.AGameBoard.HideSelection();
		GameManager.Instance.BoardData.DragSlot = null;
		// check if enough distance to slide and find direction
		float dx = endPos.x - MouseDownPos.x;
		float dy = endPos.y - MouseDownPos.y;
		float absDx = Mathf.Abs(dx);
		float absDy = Mathf.Abs(dy);
		if (Mathf.Max(absDx, absDy) < Consts.IMPULSE_DISTANCE)
		{
			// impulse too short (cancelled impulse)
			return;
		}
        Slide(dx, dy, absDx, absDy);
	}

    private void Slide(float dx, float dy, float absDx, float absDy)
    {
        if (absDx > absDy)
        {
            // try horizontal impulse
            if (dx > 0)
            {
                // impulse to right
                GameManager.Instance.BoardData.AGameBoard.SlidePipe(this, 1, 0);
            }
            else
            {
                // impulse to left
                GameManager.Instance.BoardData.AGameBoard.SlidePipe(this, -1, 0);
            }
        }
        else
        {
            // try vertical impulse
            if (dy > 0)
            {
                // impulse to top
                GameManager.Instance.BoardData.AGameBoard.SlidePipe(this, 0, 1);
            }
            else
            {
                // impulse to bottom
                GameManager.Instance.BoardData.AGameBoard.SlidePipe(this, 0, -1);
            }
        }
    }

    public void UpdateSlot(Vector2 endPos)
    {
        // check if enough distance to slide and find direction
        float dx = endPos.x - MouseDownPos.x;
        float dy = endPos.y - MouseDownPos.y;
        float absDx = Mathf.Abs(dx);
        float absDy = Mathf.Abs(dy);
        if (Mathf.Max(absDx, absDy) < Consts.IMPULSE_DISTANCE)
        {
            // impulse too short (cancelled impulse)
            return;
        }
        GameManager.Instance.BoardData.AGameBoard.HideSelection();
        GameManager.Instance.BoardData.DragSlot = null;
        Slide(dx, dy, absDx, absDy);
    }
}