using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonPressed  : MonoBehaviour 
{

	protected EventTrigger				_eventTrigger;
	protected bool 						_down;
	public bool							_active;

	void Start () 
	{
		//_dontHideOnExit = false;
		// create triggers
		_active = true;
		InitTriggers();
		_down = false;
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
		AddEventTrigger(OnUpSelector, EventTriggerType.PointerUp);
		AddEventTrigger(OnExitSelector, EventTriggerType.PointerExit);
	}

	private void AddEventTrigger(UnityAction<BaseEventData> action, EventTriggerType triggerType)
	{
		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener((eventData) => action(eventData));
		EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
		_eventTrigger.triggers.Add(entry);
	}
		
	public void SetActive(bool b)
	{
		_active = b;
	}

	protected virtual void Update()
	{
		if (_down)
		{
			transform.parent.SendMessage(name + "OnPressed", null, SendMessageOptions.RequireReceiver);
		}
	}

	private void OnDownSelector(BaseEventData beventData)
	{
		//PointerEventData eventData = (PointerEventData)beventData;
		if (!enabled || !_active) return;
		_down = true;
	}

	private void OnUpSelector(BaseEventData beventData)
	{
		//PointerEventData eventData = (PointerEventData)beventData;
		_down = false;
	}

	private void OnExitSelector(BaseEventData beventData)
	{
		_down = false;
	}

	void OnEnable() 
	{
		_down = false;
	}

	void OnDisable() 
	{
		_down = false;
	}

//	//StartCoroutine(WaitAndTryHidePopup(2));
//	public IEnumerator WaitAndTryHidePopup(int frameCount)
//	{
//        while (frameCount > 0)
//        {
//            frameCount--;
//            yield return null;
//        }
//		if (DontUpdatepos)
//		{
//			HidePopup();
//		}
//	}

}