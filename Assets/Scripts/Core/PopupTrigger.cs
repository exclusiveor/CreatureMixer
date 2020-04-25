using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PopupTrigger : MonoBehaviour
{
    public Object PopupPrefab;
    public GameObject Popup = null;
    private AutosizedPopup _popupScript;
    public string TextId = "";
    protected EventTrigger _eventTrigger;
    protected bool _isOver;
    public Transform _parentForPopup;
    public bool _active;
    public int ParentForPopupCascade = 1;

    public string CanvasLayer = "";
    public int CanvasSortingOrder = 0; // if CanvasLayer != ""
    protected bool DontUpdatepos = false;

    //protected bool						_dontHideOnExit = false;

    public int PointerId;

    public bool HideOnClickForStandalone = true;

    private bool _mobile;

    public void SetPopupPrefab(Object obj)
    {
        PopupPrefab = obj;
    }

    public void SetParentForPopup(Transform parentCanvas)
    {
        _parentForPopup = parentCanvas;
    }

    public void SetTextId(string atext)
    {
        TextId = atext;
        CheckPopup();
        _popupScript.SetText(Localer.GetText(TextId));
    }

    public void SetText(string atext)
    {
        TextId = "";
        CheckPopup();
        _popupScript.SetText(atext);
    }

    public AutosizedPopup GetPopupScript()
    {
        CheckPopup();
        return _popupScript;
    }

    private void CheckPopup()
    {
        if (Popup == null)
        {
            // create popup
            //			if (PopupPrefab == null)
            //			{
            //				Debug.Log ("-----------------------------1 " + gameObject.name);
            //				Debug.Log ("-----------------------------2 " + gameObject.transform.parent.gameObject.name);
            //				Debug.Log ("-----------------------------3 " + TextId);
            //				Debug.Log ("-----------------------------4 " + gameObject.transform.parent.parent.gameObject.name);
            //			}
            Popup = (GameObject)Instantiate(PopupPrefab);
            if (_parentForPopup != null)
            {
                Popup.transform.SetParent(_parentForPopup, false);
            }
            else
            {
                Transform trans = transform;
                for (int i = 0; i < ParentForPopupCascade; ++i)
                {
                    trans = trans.parent;
                }
                Popup.transform.SetParent(trans);
            }

            if (CanvasLayer != "")
            {
                Canvas cnvs = Popup.AddComponent<Canvas>();
                cnvs.overrideSorting = true;
                cnvs.sortingLayerName = CanvasLayer;
                cnvs.sortingOrder = CanvasSortingOrder;
            }

            Popup.transform.SetAsLastSibling();
            _popupScript = Popup.GetComponent<AutosizedPopup>();
            _popupScript.HidePopupForce();
        }
    }

    void Awake()
    {
#if UNITY_STANDALONE
        _mobile = false;
#else
		if (Application.isEditor)
		{
			_mobile = false;
		}
#endif
        if (Popup != null)
        {
            _popupScript = Popup.GetComponent<AutosizedPopup>();
        }
    }

    void Start()
    {
        //_dontHideOnExit = false;
        // create triggers
        _active = true;
        InitTriggers();

        if (TextId != "")
        {
            SetTextId(TextId);
        }

        if (Popup != null)
        {
            if (_parentForPopup != null)
            {
                Popup.transform.SetParent(_parentForPopup, false);
            }
            else
            {
                if (ParentForPopupCascade > 0)
                {
                    Transform trans = transform;
                    for (int i = 0; i < ParentForPopupCascade; ++i)
                    {
                        trans = trans.parent;
                    }
                    Popup.transform.SetParent(trans, false);
                }
            }
            Popup.transform.SetAsLastSibling();
            //_popupScript.HidePopupForce();
        }
    }

    public void InitTriggers()
    {
        _eventTrigger = gameObject.AddComponent<EventTrigger>();
        if (!_eventTrigger)
        {
            _eventTrigger = transform.GetComponent<EventTrigger>();
        }
        if (_eventTrigger.triggers == null) { _eventTrigger.triggers = new List<EventTrigger.Entry>(); }
        AddEventTrigger(OnEnterSelector, EventTriggerType.PointerEnter);
        AddEventTrigger(OnExitSelector, EventTriggerType.PointerExit);
        //		AddEventTrigger(OnClickSelector, EventTriggerType.PointerUp);
        //AddEventTrigger(OnClickSelector, EventTriggerType.PointerClick);
        _isOver = false;
    }
    private void AddEventTrigger(UnityAction<BaseEventData> action, EventTriggerType triggerType)
    {
        EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
        trigger.AddListener((eventData) => action(eventData));
        EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
        _eventTrigger.triggers.Add(entry);
    }
    private void OnEnterSelector(BaseEventData beventData)
    {
        PointerEventData eventData = (PointerEventData)beventData;
        if (!enabled || !_active) return;

        if (_mobile)
        {
            if (Input.touches.Length == 0)
            {
                return;
            }
            else
            {
                PointerId = eventData.pointerId;
            }
        }

        CheckPopup();
        ShowPopup();
    }
    private void OnExitSelector(BaseEventData beventData)
    {
        PointerEventData eventData = (PointerEventData)beventData;
        bool needCheck = false;

        if (_mobile)
        {
            if (eventData.pointerId != PointerId)
            {
                return;
            }
            else
            {
                Touch touch = Input.touches[eventData.pointerId];
                if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                {
                    needCheck = true;
                }
            }
        }

        if (!_mobile) // && !Input.GetMouseButton(0))
        {
            needCheck = true;
        }

        if (!enabled || !_active || !needCheck || (_mobile && DontUpdatepos))  // _dontHideOnExit
        {
            //_dontHideOnExit = false;
            return;
        }
        HidePopup();
    }
    //	private void OnClickSelector()
    //	{
    //		if (!enabled || !_active) return;
    //		_dontHideOnExit = true;
    //	}

    protected virtual void ShowPopup()
    {
        _isOver = true;
        DontUpdatepos = false;
        _popupScript.ShowPopup();
    }

    public virtual void HidePopup()
    {
        _isOver = false;
        DontUpdatepos = false;
        _popupScript.HidePopup();
    }

    public void SetActive(bool b)
    {
        _active = b;
    }

    protected virtual void Update()
    {
        if (_popupScript == null) { return; }
        if (_popupScript.IsLocked)
        {
            //Resize();
        }
        else
        if (_isOver)
        {
            if (DontUpdatepos)
            {
                _popupScript.Resize();
#if UNITY_STANDALONE
                if (HideOnClickForStandalone && Input.GetMouseButton(0))
                {
                    HidePopup();
                }
#else
				if (Input.touchCount > 0)
				{
					HidePopup();
				}
#endif
            }
            else
            {
#if UNITY_STANDALONE
                //DontUpdatepos = true;
                //MusicManager.playSound("phoenix_feather_use");                   
                return;
#else
				if (Input.touchCount == 0) // && !Application.isEditor
				{
					DontUpdatepos = true;
					//MusicManager.playSound("phoenix_feather_use");                   
					return;                 
				}
#endif
                _popupScript.Resize();
                _popupScript.Position();
            }
        }
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