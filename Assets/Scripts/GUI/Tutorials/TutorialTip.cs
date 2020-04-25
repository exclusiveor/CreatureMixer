using UnityEngine;
using UnityEngine.UI;
//using System.Collections;
//using System.Collections.Generic;

public class TutorialTip  : MonoBehaviour 
{
	private const float					SHOW_HIDE_SPEED = 0.2f;
	private const float					TIP_WIDTH = 282.0f;
	private const float					TIP_HEIGHT = 158.0f;
	private const float					ARROW_WIDTH = 61.0f;
	private const float					ARROW_HEIGHT = 67.0f;
	private const float					CURSOR_DX_DY = -20.0f;

	public Text							Text { get; set; }
	public GameObject					Cross { get; set; }
	public GameObject					Arrow { get; set; }
	public GameObject					TipBody { get; set; }
	protected bool						_isOver;
	protected TutorialData 				_data;

	public void InitTip(TutorialData data)
	{
		_data = data;
		transform.localScale = new Vector3(1,1,1);
		TipBody = transform.Find("TipBody").gameObject;
		Cross = TipBody.transform.Find("Cross").gameObject;
		Arrow = transform.Find("Arrow").gameObject;
		Text = TipBody.transform.Find("Text").GetComponent<Text>();
		Text.text = Localer.GetText("Tutorial_") + data.Id;
		if (!data.IsArrow)
		{
			Arrow.SetActive(false);
		}
		if (data.Type == "ui")
		{
			SetUITutorialPos(new Vector3(data.X, data.Y, 0));
		} else
		{
			//SetTutorialPos(SceneToUiPosition(new Vector3(data.X, data.Y, 0)));
			UpdateScrollPosition();
		}
		//transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
		//transform.GetComponent<CanvasGroup>().interactable = false;
		HideTipForce();
	}

	public void ShowTip()
	{
		LeanTween.cancel(gameObject);
		gameObject.SetActive(true);
		transform.GetComponent<UnityEngine.EventSystems.EventTrigger>().enabled = true;
		if (_data.IsArrow)
		{
			// blink arrow
			//LeanTween.alpha(Arrow, 0.0f, 0.2f); //.setLoopPingPong();
			CanvasGroup group = Arrow.GetComponent<CanvasGroup>();
			LeanTween.value(Arrow, 0.60f, 1.0f, 0.5f)
				//.setEase(UIConsts.SHOW_EASE)
				//	.setDelay(UIConsts.SHOW_DELAY_TIME)
				.setLoopPingPong()
				.setOnUpdate
					(
						(float val)=>
						{
							group.alpha = val;
						}
					);
		}
		//
		transform.SetAsLastSibling();
		LeanTween.value(gameObject, gameObject.GetComponent<CanvasGroup>().alpha, 1.0f, SHOW_HIDE_SPEED * (1.0f - gameObject.GetComponent<CanvasGroup>().alpha))
			//.setEase(UIConsts.SHOW_EASE)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setOnUpdate
				(
					(float val)=>
					{
						gameObject.GetComponent<CanvasGroup>().alpha = val;
					}
				);
		//gameObject.SetActive(true);
	}

	public void HideTipForce()
	{
		LeanTween.cancel(gameObject);
		transform.GetComponent<UnityEngine.EventSystems.EventTrigger>().enabled = false;
		gameObject.GetComponent<CanvasGroup>().alpha = 0;
		//gameObject.SetActive(false);
		_isOver = false;
		Cross.SetActive(false);
	}

	public void HideTip()
	{
		LeanTween.cancel(gameObject);
		transform.GetComponent<UnityEngine.EventSystems.EventTrigger>().enabled = false;
		//gameObject.SetActive(false);
		_isOver = false;
		Cross.SetActive(false);
		//
		LeanTween.value(gameObject, gameObject.GetComponent<CanvasGroup>().alpha, 0.0f, SHOW_HIDE_SPEED * gameObject.GetComponent<CanvasGroup>().alpha)
			//.setEase(UIConsts.SHOW_EASE)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setOnUpdate
				(
					(float val)=>
					{
						gameObject.GetComponent<CanvasGroup>().alpha = val;
					}
				);
		//gameObject.SetActive(false);
	}

	public void CloseTip()
	{
		HideTip();
		GameObject.Destroy(gameObject, SHOW_HIDE_SPEED);
	}

	void OnEnterTip() 
	{
		_isOver = true;
		Cross.SetActive(true);
	}
	
	void OnExitTip() 
	{
		_isOver = false;
		Cross.SetActive(false);
	}

	Vector3 SceneToUiPosition(Vector3 pos)
	{
		return Camera.main.WorldToScreenPoint(pos); // Camera.main.WorldToViewportPoint(pos);
	}

	void SetUITutorialPos(Vector3 apos)
	{
		transform.localPosition = apos;
		Arrow.transform.localPosition = new Vector3(0, 0, 0);
		TipBody.transform.localPosition = new Vector3(0, 0, 0);
		Vector3 tempPos = transform.localPosition;

		float ScreenW_2 = UIConsts.DESIGN_RESOLUTION.x / 2;
		float ScreenH_2 = UIConsts.DESIGN_RESOLUTION.y / 2;

		float sWidth = Screen.width;
		float sHeight = Screen.height;

		int iRotateQuater = -1;
		
		if (_data.Align == "left")
		{
			iRotateQuater = 2;
		} else
		if (_data.Align == "right")
		{
			iRotateQuater = 0;
		} else
		if (_data.Align == "top")
		{
			iRotateQuater = 3;
		} else
		if (_data.Align == "bottom")
		{
			iRotateQuater = 1;
		} else
		if (_data.Align == "leftorright")
		{
			if (apos.x < 0) iRotateQuater = 0; else
				iRotateQuater = 2;
		} else
		if (_data.Align == "toporbottom")
		{
			if (apos.y < 0) iRotateQuater = 1; else
				iRotateQuater = 3;
		} else
		{
			// auto
			Vector2 direction = new Vector2();
			direction.x = tempPos.x > 0?-1.0f:1.0f;
			direction.y = tempPos.y > 0?-1.0f:1.0f;

			if (tempPos.y > ScreenH_2 - TIP_HEIGHT) 
			{
				iRotateQuater = 3; 
			} else
				if (tempPos.y < -ScreenH_2 + TIP_HEIGHT) 
			{
				iRotateQuater = 1; 
			} else
				if (tempPos.x > ScreenW_2 - TIP_WIDTH) 
			{
				iRotateQuater = 2; 
			} else
				if (tempPos.x < -ScreenW_2 + TIP_WIDTH) 
			{
				iRotateQuater = 0;
			} 
			if (iRotateQuater == -1)
			{
				if (direction.x == 1)
				{
					iRotateQuater = (direction.y==1)?0:3;
				}
				else
				{
					iRotateQuater = (direction.y==1)?1:2;
				}
			}
		}

		if (_data.IsArrow)
		{
			Arrow.transform.localPosition = new Vector3(0, 0, 0);
			Arrow.SetActive(true);
			Arrow.transform.localRotation = Quaternion.Euler(0, 0, (iRotateQuater + 1) * 90.0f - 180); // m_arrowNode->set_rotation((iRotateQuater) * 90);	
			float posx = 0;
			float posy = 0;
			Vector3 newPos = new Vector3(0, 0, 0);
			if (iRotateQuater == 0)
			{
				posx = newPos.x + ARROW_WIDTH / 2 + TIP_WIDTH / 2 - CURSOR_DX_DY;
				posy = newPos.y;
				float perc = (tempPos.y + ScreenH_2) / sHeight;
				if (perc > 0.8f) perc = 0.8f;
				if (perc < 0.2f) perc = 0.2f; 
				float dy = TIP_HEIGHT * perc;
				posy = posy - dy + TIP_HEIGHT / 2;
			} else
				if (iRotateQuater == 1)
			{
				posy = newPos.y + ARROW_HEIGHT / 2 + TIP_HEIGHT / 2 - CURSOR_DX_DY;
				posx = newPos.x;
				float perc = (tempPos.x + ScreenW_2) / sWidth;
				if (perc > 0.8f) perc = 0.8f;
				if (perc < 0.2f) perc = 0.2f; 
				float dx = TIP_WIDTH * perc;
				posx = posx - dx + TIP_WIDTH / 2;
			} else
				if (iRotateQuater == 2)
			{
				posx = newPos.x - ARROW_WIDTH / 2 - TIP_WIDTH / 2 + CURSOR_DX_DY;
				posy = newPos.y;
				float perc = (tempPos.y + ScreenH_2) / sHeight;
				if (perc > 0.8f) perc = 0.8f;
				if (perc < 0.2f) perc = 0.2f; 
				float dy = TIP_HEIGHT * perc;
				posy = posy - dy + TIP_HEIGHT / 2;
			} else
				if (iRotateQuater == 3)
			{
				posy = newPos.y - ARROW_HEIGHT / 2 - TIP_HEIGHT / 2 + CURSOR_DX_DY;
				posx = newPos.x;
				float perc = (tempPos.x + ScreenW_2) / sWidth;
				if (perc > 0.8f) perc = 0.8f;
				if (perc < 0.2f) perc = 0.2f; 
				float dx = TIP_WIDTH * perc;
				posx = posx - dx + TIP_WIDTH / 2;
			}
			TipBody.transform.localPosition = new Vector3(posx, posy, 0);
		} else
		{
			TipBody.transform.localPosition = new Vector3(0, 0, 0);
		}
	}

	public void UpdateScrollPosition()
	{
		float sWidth = Screen.width;
		float sHeight = Screen.height;

		float limit = 40.0f;
		Vector3 newPos = SceneToUiPosition(new Vector3(_data.X, _data.Y, 0));
		if (newPos.x < limit)
		{
			newPos.x = limit;
		} else
		if (newPos.x > sWidth - limit)
		{
			newPos.x = sWidth - limit;
		}
				
		if (newPos.y < limit)
		{
			newPos.y = limit;
		} else
		if (newPos.y > sHeight - limit)
		{
			newPos.y = sHeight - limit;
		}
			
		newPos = Camera.main.ScreenToWorldPoint(newPos); 
		transform.position = newPos;
		Vector3 tempPos = transform.localPosition;

		float ScreenW_2 = UIConsts.DESIGN_RESOLUTION.x / 2;
		float ScreenH_2 = UIConsts.DESIGN_RESOLUTION.y / 2;
		Vector2 direction = new Vector2();
		direction.x = tempPos.x > 0?-1.0f:1.0f;
		direction.y = tempPos.y > 0?-1.0f:1.0f;
		int iRotateQuater = -1;
		// auto
		if (tempPos.y > ScreenH_2 - TIP_HEIGHT) 
		{
			iRotateQuater = 3; 
		} else
		if (tempPos.y < -ScreenH_2 + TIP_HEIGHT) 
		{
			iRotateQuater = 1; 
		} else
		if (tempPos.x > ScreenW_2 - TIP_WIDTH) 
		{
			iRotateQuater = 2; 
		} else
		if (tempPos.x < -ScreenW_2 + TIP_WIDTH) 
		{
			iRotateQuater = 0;
		} 
		if (iRotateQuater == -1)
		{
			if (direction.x == 1)
			{
				iRotateQuater = (direction.y==1)?0:3;
			}
			else
			{
				iRotateQuater = (direction.y==1)?1:2;
			}
		}

		if (_data.IsArrow)
		{
			Arrow.transform.localPosition = new Vector3(0, 0, 0);
			Arrow.SetActive(true);
			Arrow.transform.localRotation = Quaternion.Euler(0, 0, (iRotateQuater + 1) * 90.0f - 180); // m_arrowNode->set_rotation((iRotateQuater) * 90);	
			float posx = 0;
			float posy = 0;
			newPos = new Vector3(0, 0, 0);
			if (iRotateQuater == 0)
			{
				posx = newPos.x + ARROW_WIDTH / 2 + TIP_WIDTH / 2 - CURSOR_DX_DY;
				posy = newPos.y;
				float perc = (tempPos.y + ScreenH_2) / sHeight;
				if (perc > 0.8f) perc = 0.8f;
				if (perc < 0.2f) perc = 0.2f; 
				float dy = TIP_HEIGHT * perc;
				posy = posy - dy + TIP_HEIGHT / 2;
			} else
			if (iRotateQuater == 1)
			{
				posy = newPos.y + ARROW_HEIGHT / 2 + TIP_HEIGHT / 2 - CURSOR_DX_DY;
				posx = newPos.x;
				float perc = (tempPos.x + ScreenW_2) / sWidth;
				if (perc > 0.8f) perc = 0.8f;
				if (perc < 0.2f) perc = 0.2f; 
				float dx = TIP_WIDTH * perc;
				posx = posx - dx + TIP_WIDTH / 2;
			} else
			if (iRotateQuater == 2)
			{
				posx = newPos.x - ARROW_WIDTH / 2 - TIP_WIDTH / 2 + CURSOR_DX_DY;
				posy = newPos.y;
				float perc = (tempPos.y + ScreenH_2) / sHeight;
				if (perc > 0.8f) perc = 0.8f;
				if (perc < 0.2f) perc = 0.2f; 
				float dy = TIP_HEIGHT * perc;
				posy = posy - dy + TIP_HEIGHT / 2;
			} else
			if (iRotateQuater == 3)
			{
				posy = newPos.y - ARROW_HEIGHT / 2 - TIP_HEIGHT / 2 + CURSOR_DX_DY;
				posx = newPos.x;
				float perc = (tempPos.x + ScreenW_2) / sWidth;
				if (perc > 0.8f) perc = 0.8f;
				if (perc < 0.2f) perc = 0.2f; 
				float dx = TIP_WIDTH * perc;
				posx = posx - dx + TIP_WIDTH / 2;
			}
			TipBody.transform.localPosition = new Vector3(posx, posy, 0);
		} else
		{
			TipBody.transform.localPosition = new Vector3(0, 0, 0);
		}
	}

	void Update () 
	{
		if (_data.Type == "scroll")
		{
			UpdateScrollPosition();
		}
		if (_isOver)
		{
			if (Input.GetMouseButtonDown(0) && !IsOverButton())
			{ 
				// click on tutorial
				SendCloseSignal();
			} //else
//			if (Input.GetMouseButtonUp(0))
//			{ 
//				// click on tutorial
//				SendCloseSignal();
//			}
		}
	}

	private bool IsOverButton()
	{
		return transform.Find ("TipBody").Find("TutorialTipHelpButton").GetComponent<TutorialTipHelpButton>().IsOver();
	}

	private void SendCloseSignal()
	{
		EventData eventData = new EventData("OnTutorialCloseNeededEvent");
		eventData.Data["id"] = _data.Id;
		GameManager.Instance.EventManager.CallOnTutorialCloseNeededEvent(eventData);
	}
}