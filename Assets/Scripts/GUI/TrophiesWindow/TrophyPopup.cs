using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TrophyPopup : MonoBehaviour
{
	private const float SHOW_HIDE_TIME = 0.5f;
	private const float DISOLAYIN_TIME = 7.0f;

	public GameObject LeanObj;
	public Text DescriptionText;
	public Text RewardAmountText;
	private ETrophyType _trophyType; 
	public CanvasGroup Group;

	private EventTrigger	_eventTrigger;

	public void InitPopup(ETrophyType trophyType)
	{
		_trophyType = trophyType;
		RewardAmountText.text = "$" + GameManager.Instance.GameData.XMLtrophiesData[trophyType].Reward.ToString();
		DescriptionText.text = Localer.GetText(_trophyType.ToString());
		ShowPopup();
	}

	void Start()
	{
		InitTriggers();
	}

	private void AddEventTrigger(UnityAction<BaseEventData> action, EventTriggerType triggerType)
	{
		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener((eventData) => action(eventData));
		EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
		_eventTrigger.triggers.Add(entry);
	}

	public void InitTriggers()
	{
		_eventTrigger = gameObject.AddComponent<EventTrigger>();
		if (!_eventTrigger)
		{
			_eventTrigger = transform.GetComponent<EventTrigger>();
		}
		if(_eventTrigger.triggers == null)	{ _eventTrigger.triggers = new List<EventTrigger.Entry>();	}
		AddEventTrigger(OnDownSelector, EventTriggerType.PointerDown);
		//		AddEventTrigger(OnClickSelector, EventTriggerType.PointerUp);
		//AddEventTrigger(OnClickSelector, EventTriggerType.PointerClick);
	}

	private void ShowPopup()
	{
		gameObject.SetActive(true);
		LeanTween.cancel(LeanObj);
		float startAlpha = 0;
		Group.alpha = 0;
		Group.blocksRaycasts = true; //false
		LeanTween.value(LeanObj, startAlpha, 1,  SHOW_HIDE_TIME * (1 - startAlpha))
			.setOnUpdate
			(
				(float val)=>
				{
					Group.alpha = val;
				}
			)
			.setOnComplete
			(
				()=>
				{
					//Group.blocksRaycasts = true;
					Invoke("HidePopup", DISOLAYIN_TIME);
				}
			);
	}

	private void HidePopup()
	{
		CancelInvoke();
		LeanTween.cancel(LeanObj);
		Group.blocksRaycasts = false;
		LeanTween.value(LeanObj, Group.alpha, 0,  SHOW_HIDE_TIME * Group.alpha)
			.setOnUpdate
			(
				(float val)=>
				{
					Group.alpha = val;
				}
			)
			.setOnComplete
			(
				()=>
				{
					gameObject.SetActive(false);
					transform.parent.SendMessage("OnPopupHided", this);
				}
			);
	}

	private void OnDownSelector(BaseEventData beventData)
	{
		//PointerEventData eventData = (PointerEventData)beventData;
		HidePopup();
	}

		
}
