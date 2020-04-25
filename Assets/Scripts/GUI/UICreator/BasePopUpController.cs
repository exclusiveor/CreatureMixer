using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BasePopUpController : MonoBehaviour {

	public bool _active = false;
	protected bool _isOver;
	private EventTrigger _eventTrigger;
	
	public float _popUpWidth;
	public float _popUpHeight;
	public float _dWidth = 200;
	public float _dHeight = 100;
    public float _scale;
	public GameObject FonGameObject;
	public bool ClickOutside = false;
	public bool MouseOutside = false;
    private RectTransform _rectTransform;
    private RectTransform myRect {
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
    private Canvas _canva;   

	//private void AddEventTrigger(UnityAction action, EventTriggerType triggerType)
	//{
	//	EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
	//	trigger.AddListener((eventData) => action());

	//	EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

	//	_eventTrigger.triggers.Add(entry);
	//}

    protected virtual void OnPointerDown()
    {
        //if (!EventSystem.current.IsPointerOverGameObject())
        //{
        //    //Hide();
        //}
    }

    //protected virtual void OnPointerEnter()
    //{
    //	_isOver = true;
    //}

    //protected virtual void OnPointerExit()
    //{
    //	//gameObject.GetComponent<RectTransform>().anchoredPosition3D = UIConsts.START_POSITION;
    //	_isOver = false;
    //	if(MouseOutside)
    //	{
    //		Hide();
    //	}
    //}

    protected virtual void UpdateInput()
	{
		//TODO тут проверяем есть ли клик за окном это тест и он работает надо прикрепить к окнам это дело
		//if (ClickOutside && Input.GetMouseButtonDown(0) && !_isOver )
		//{
		//	//Hide();
		//}
	}

	public virtual void Reset()
	{

	}

	public virtual void Show (Vector3 pos)
	{
		Reset();

        myRect.anchoredPosition3D = UIConsts.START_POSITION;
		
		LeanTween.value(gameObject, UIConsts.START_POSITION, UIConsts.STOP_POSITION, UIConsts.SHOW_TWEEN_TIME)
			.setEase(UIConsts.SHOW_EASE)
				.setDelay(UIConsts.SHOW_DELAY_TIME)
				.setOnUpdate(
					(Vector3 val)=>
					{
                    myRect.anchoredPosition3D = val;
				}
				);
	}
	
	public virtual void Hide ()
	{
		GameManager.Instance.EventManager.CallOnHideWindowEvent();

		LeanTween.value(gameObject, UIConsts.STOP_POSITION, UIConsts.START_POSITION, UIConsts.HIDE_TWEEN_TIME)
			.setEase(UIConsts.HIDE_EASE)
				.setDelay(UIConsts.HIDE_DELAY_TIME)
				.setOnUpdate(
					(Vector3 val)=>
					{
                    myRect.anchoredPosition3D = val;
				}
				);
	}

	void OnShowPopUpEvent(EventData ob = null)
	{

	}

	void OnHidePopUpEvent(EventData ob = null)
	{

	}

	void Awake()
	{
		EventManager.OnShowPopUpEvent += OnShowPopUpEvent;
		EventManager.OnHidePopUpEvent += OnHidePopUpEvent;
        _scale = 1.0f;               
    }

	private void OnDestroy()
	{
		EventManager.OnShowPopUpEvent -= OnShowPopUpEvent;
		EventManager.OnHidePopUpEvent -= OnHidePopUpEvent;
	}

	public virtual void ReInit()
    {
        _canva = GetComponentInParent<Canvas>();
    }

	public virtual Vector3 UiCorrect(Vector3 pos)
	{
        myRect.anchoredPosition3D = pos;
        Vector2 canvaHalfSize = _canva.GetComponent<RectTransform>().sizeDelta / 2.0f;      

        float popupMaxPosY = myRect.anchoredPosition3D.y + myRect.sizeDelta.y * myRect.pivot.y;
        float popupMinPosY = myRect.anchoredPosition3D.y - myRect.sizeDelta.y * myRect.pivot.y;
        float popupMaxPosX = myRect.anchoredPosition3D.x + myRect.sizeDelta.x * 1.7f * myRect.pivot.x;
        float popupMinPosX = myRect.anchoredPosition3D.x - myRect.sizeDelta.x * 1.7f * myRect.pivot.x;

        if (popupMaxPosY > canvaHalfSize.y)
        {
            pos.y += canvaHalfSize.y - popupMaxPosY;
        }

        if (popupMinPosY < -canvaHalfSize.y)
        {
            pos.y += Mathf.Abs(popupMinPosY) - canvaHalfSize.y;
        }

        if (popupMaxPosX > canvaHalfSize.x)
        {
            pos.x += canvaHalfSize.x - popupMaxPosX;
        }

        if (popupMinPosX < -canvaHalfSize.x)
        {
            pos.x += Mathf.Abs(popupMinPosX) - canvaHalfSize.x;
        }

        return pos;
	} 
}
