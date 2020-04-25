using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// for buttons with text color changed on enter
public class UserNameButtonColorizer : MonoBehaviour
{
	private Text _txt;
	private EventTrigger _eventTrigger;

	public Color EnterColor;
	public Color ExitColor;

	void Awake()
	{
		GameObject _go = HelperFunctions.GetChildGameObject(transform.gameObject, "Text");
		if(_go)
		{
			_txt = _go.GetComponent<Text>();
			InitTriggers();
		}
		else
		{
			_txt = null;
		}
	}

	public void InitTriggers()
	{
		_eventTrigger = gameObject.AddComponent<EventTrigger>();
		//_eventTrigger = transform.GetComponent<EventTrigger>();
		if(_eventTrigger.triggers == null)	{ _eventTrigger.triggers = new List<EventTrigger.Entry>();	}
		AddEventTrigger(OnEnterSelector, EventTriggerType.PointerEnter);
		AddEventTrigger(OnExitSelector, EventTriggerType.PointerExit);
		//AddEventTrigger(OnClickSelector, EventTriggerType.PointerClick);

		OnExitSelector();
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
		//if (_txt != null)
			_txt.color = EnterColor;
	}
	private void OnExitSelector()
	{
		//if (_txt != null)
			_txt.color = ExitColor;
	}

	public void Activate()
	{
		_eventTrigger.enabled = true;
	}

	public void Deactivate()
	{
		OnExitSelector();
		_eventTrigger.enabled = false;
	}


//	private void OnClickSelector()
//	{
//		//...
//	}

}
