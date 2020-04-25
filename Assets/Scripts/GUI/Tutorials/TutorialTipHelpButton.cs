using UnityEngine;
using UnityEngine.UI;
//using System.Collections;
//using System.Collections.Generic;

public class TutorialTipHelpButton : MonoBehaviour 
{
	private const float					SHOW_HIDE_SPEED = 0.2f;

	CanvasGroup							ButtonGroup;
	//CanvasGroup							OverGroup;
	
	protected bool						_isOver;

	public bool IsOver()
	{
		return _isOver;
	}

	public void InitAll()
	{
		ButtonGroup = transform.Find("Button").GetComponent<CanvasGroup>();
		//OverGroup = transform.Find("OverPoint").GetComponent<CanvasGroup>();
		HideButtonForce();
		_isOver = false;
		ReInit();
	}

	public void OnEnterOverPoint() 
	{
		if (_isOver)
		{
			return;
		}
		_isOver = true;

		// show button
		ButtonGroup.blocksRaycasts = true;
		GameObject buttonObj = ButtonGroup.gameObject;
		LeanTween.cancel(buttonObj);
		float time = SHOW_HIDE_SPEED * (1.0f - ButtonGroup.alpha);
		LeanTween.value(buttonObj, ButtonGroup.alpha, 1.0f, time)
			//.setEase(UIConsts.SHOW_EASE)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
				.setOnUpdate
				(
					(float val)=>
					{
						ButtonGroup.alpha = val;
					}
				);
	}
	
	void OnExitButton() 
	{
		if (!_isOver)
		{
			return;
		}
		_isOver = false;
		// hide button
		ButtonGroup.blocksRaycasts = false;
		GameObject buttonObj = ButtonGroup.gameObject;
		LeanTween.cancel(buttonObj);
		float time = SHOW_HIDE_SPEED * ButtonGroup.alpha;
		LeanTween.value(buttonObj, ButtonGroup.alpha, 0.0f, time)
			//.setEase(UIConsts.SHOW_EASE)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setOnUpdate
				(
					(float val)=>
					{
					ButtonGroup.alpha = val;
					}
				);
	}

	void HideButtonForce()
	{
		GameObject buttonObj = ButtonGroup.gameObject;
		ButtonGroup.blocksRaycasts = false;
		LeanTween.cancel(buttonObj);
		ButtonGroup.alpha = 0;
		_isOver = false;
	}

	public void ButtonOnClick ()
	{
		//Debug.Log(HelperFunctions.GetCurrentMethod() + " " + this.name);
		EventData eventData = new EventData("OnOpenFormNeededEvent");
		eventData.Data["form"] = UIConsts.FORM_ID.HELP_WINDOW;
		eventData.Data["caller"] = "tip";
		eventData.Data["page"] = 0; // TODO get real page
		GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
	}

	void Start () 
	{
		InitAll();
	}

	void ReInit()
	{		
		ButtonGroup.transform.Find("Text").GetComponent<Text>().text = Localer.GetText("MoreInfo");
	}
}