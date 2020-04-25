using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnOverLeaveTrigger  : MonoBehaviour 
{
	public string						EnterFunction = "";
	public string						LeaveFunction = "";
	private EventTrigger				_eventTrigger;
	public bool							_active;
	public int							ParentCascade = 1;
	

	void Start () 
	{
		// create triggers
		_active = true;
		InitTriggers();
	}

	public void InitTriggers()
	{
		_eventTrigger = gameObject.AddComponent<EventTrigger>();
		if (!_eventTrigger)
		{
			_eventTrigger = transform.GetComponent<EventTrigger>();
		}
		if(_eventTrigger.triggers == null)	{ _eventTrigger.triggers = new List<EventTrigger.Entry>();	}
		AddEventTrigger(OnEnterSelector, EventTriggerType.PointerEnter);
		AddEventTrigger(OnExitSelector, EventTriggerType.PointerExit);
		//AddEventTrigger(OnClickSelector, EventTriggerType.PointerClick);
	}
	private void AddEventTrigger(UnityAction action, EventTriggerType triggerType)
	{
		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener((eventData) => action());
		EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
		_eventTrigger.triggers.Add(entry);
	}
	private void OnEnterSelector()
	{
		if (!enabled || !_active) return;

		Transform trans = transform;
		for (int i = 0; i < ParentCascade; ++i)
		{
			trans = trans.parent;
		}
		trans.SendMessage(EnterFunction);
	}
	private void OnExitSelector()
	{
		if (!enabled || !_active) return;

		Transform trans = transform;
		for (int i = 0; i < ParentCascade; ++i)
		{
			trans = trans.parent;
		}
		trans.SendMessage(LeaveFunction);
	}

	public void SetActive(bool b)
	{
		_active = b;
	}
}