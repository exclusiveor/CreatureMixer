using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public enum EFormCreationMethod
{
	Static = 0, // take from scene or create if needed, dont destroy on hide
	Dynamic = 1 // creates whenn needed and destroy on hide
}

public class BaseUIController : MonoBehaviour {

	public bool Active = false;
	public bool RootUIController = true;
	protected bool _isOver;
	private EventTrigger _eventTrigger;
	public bool ClickOutside = false;
    public bool _isHiding = false;
	public float BaseScale = 1;
	public bool NeedRescale = true;	// need rescale window according to height of screen
    public UIConsts.FORM_ID FormID;
	public EFormCreationMethod CreationMethod = EFormCreationMethod.Static;

	protected float	_delayBeforeShow = -1;

    private RectTransform _rectTransform;
    public RectTransform myRect {
        get {
            if (_rectTransform == null) {
                _rectTransform = gameObject.GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
        set {
            _rectTransform = value;
        }
    }

	public virtual bool OpenForm(EventData e)
	{
		// example
		//_atype = (string)e.Data["type"];
		//ReInit();
		TryRescale();
		return true;
	}

	protected void TryRescale()
	{
		if (NeedRescale)
		{
			float windowScale = BaseScale;
			//Debug.Log("------------> " + transform.parent.GetComponent<RectTransform>().sizeDelta.y);
			windowScale *= transform.parent.GetComponent<RectTransform>().sizeDelta.y / UIConsts.DESIGN_RESOLUTION.y;
			transform.localScale = new Vector3(windowScale, windowScale,1);
		}
	}

	private void AddEventTrigger(UnityAction action, EventTriggerType triggerType)
	{
		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener((eventData) => action());
		
		EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
		
		_eventTrigger.triggers.Add(entry);
	}
	
	private void OnPointerEnter()
	{
		_isOver = true;
	}
	
	private void OnPointerExit()
	{
		//gameObject.GetComponent<RectTransform>().anchoredPosition3D = UIConsts.START_POSITION;
		_isOver = false;
	}

	protected virtual void UpdateInput()
	{
		//TODO тут проверяем есть ли клик за окном это тест и он работает надо прикрепить к окнам это дело
		if (ClickOutside && Active && Input.GetMouseButtonDown(0) && !_isOver && !_isHiding)
		{
			Hide();
		}
	}

	public virtual void Reset()
	{

	}

	public virtual void Show ()
	{
		//TODO Организовать инициализацию окна по умолчанию
//		GetComponent<CanvasGroup>().blocksRaycasts = false;
		_isHiding = true;
        Active = false;
		Reset();
        myRect.anchoredPosition3D = UIConsts.START_POSITION;

		float delayBeforeShow = _delayBeforeShow;
		if (delayBeforeShow < 0)
		{
			delayBeforeShow = UIConsts.SHOW_DELAY_TIME;
		}

		LeanTween.delayedCall(delayBeforeShow, () => {MusicManager.playSound("popup_show_hide");});

		LeanTween.value(gameObject, UIConsts.START_POSITION, UIConsts.STOP_POSITION, UIConsts.SHOW_TWEEN_TIME)
			.setEase(UIConsts.SHOW_EASE)
			.setDelay(delayBeforeShow)
				.setOnUpdate(
					(Vector3 val)=>
					{
					myRect.anchoredPosition3D = val;
				}
			).setOnComplete
			(
				()=>
				{
//					//Invoke("EnableBlockRexcast", 3.25f);
//					EnableBlockRexcast();
					_isHiding = false;
                    Active = true;
					OnShowed();
				}
			);
	}

	protected virtual void OnShowed()
	{

	}

	protected virtual void OnHided()
	{
		if (CreationMethod == EFormCreationMethod.Dynamic)
        {
            Destroy(gameObject);
            GameManager.Instance.GameFlow.OnDestroyWindow(FormID);
        }
	}

	protected void EnableBlockRexcast()
	{
		GetComponent<CanvasGroup>().blocksRaycasts = true;
	}
	
	public virtual void Hide ()
	{
        _isHiding = true;
        //GetComponent<CanvasGroup>().blocksRaycasts = false;
        //MusicManager.playSound("popup_show_hide");

        LeanTween.delayedCall(UIConsts.HIDE_DELAY_TIME, () => { MusicManager.playSound("popup_show_hide"); });

        LeanTween.value(gameObject, UIConsts.STOP_POSITION, UIConsts.START_POSITION, UIConsts.HIDE_TWEEN_TIME)
			.setEase(UIConsts.HIDE_EASE)
				.setDelay(UIConsts.HIDE_DELAY_TIME)
				.setOnUpdate(
					(Vector3 val)=>
					{
                    myRect.anchoredPosition3D = val;
				    }
				).setOnComplete
				(
					()=>
					{
                        GameManager.Instance.EventManager.CallOnHideWindowEvent();
                        gameObject.SetActive(false);
					    Active = false;
                        _isHiding = false;
						OnHided();
					}
				);
	}

//    void OnEventShowWindow(EventData ob = null)
//    {
//        if (RootUIController)
//        {
//            ActivateDeActivate();
//        }
//    }
//
//    void OnEventHideWindow(EventData ob = null)
//    {
//        if (RootUIController)
//        {
//            ActivateDeActivate();
//        }
//    }

//    void ActivateDeActivate()
//    {
//        if (!gameObject.activeSelf) return;
//        if (GameFlow.BaseWindow != gameObject) return;
//        //if (GameManager.Instance.GameFlow.GetCurrentActiveWindowId() != FormID)
//        if (GameManager.Instance.GameFlow.IsSomeWindow() && Active)
//        {
//            Active = false;
//            ////BroadcastMessage("Disable");
//            OnDeactivate();
//        }
//        else
//        {
//            Active = true;
//            ////BroadcastMessage("Enable");
//            OnActivate();
//        }
//    }

    protected virtual void OnDeactivate()
	{

	}

	protected virtual void OnActivate()
	{
		
	}

	protected virtual void Awake()
	{
//		EventManager.OnShowWindowEvent += OnEventShowWindow;
//		EventManager.OnHideWindowEvent += OnEventHideWindow;
	}

    protected virtual void OnDestroy()
	{
//		EventManager.OnShowWindowEvent -= OnEventShowWindow;
//		EventManager.OnHideWindowEvent -= OnEventHideWindow;
	}

	public virtual void ReInit()
	{
		//Debug.Log ("BaseUIController.ReInit");

		_eventTrigger = gameObject.AddComponent<EventTrigger>();
		if(_eventTrigger.triggers == null)	{ _eventTrigger.triggers = new List<EventTrigger.Entry>();	}
		
		AddEventTrigger(OnPointerEnter, EventTriggerType.PointerEnter);
		AddEventTrigger(OnPointerExit, EventTriggerType.PointerExit);
	}

    public virtual bool EscapeOnClick()
    {
        if (_isHiding)
        {
            return true;
        }
        return false; // for modal windows = true!
    }
}
