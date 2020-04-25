using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameNotification : MonoBehaviour
{
    public enum NotifyType
    {
        None = 0,
        UsePowerup = 1,
        GameComplete = 2
    }

    public GameObject LevelTileIcon;
    public Text NotifyText;

    public List<NotifyType> Notifications;

    public float MoveOffset = 500.0f;
    public float ShowHideTime = 0.5f;
    public float ShowingTime = 2.0f;
    private Vector3 _startPosition;
    private Vector3 _endPosition;    
    private NotifyType _currentNotifyType;
    private NotifyType _nextNotifyType;
    private RectTransform _rectTransform;

	// Use this for initialization
	void Start () {
        EventManager.OnShowNotificationEvent += OnShowNotification;
        EventManager.OnHideNotificationEvent += OnHideNotification;

        _rectTransform = gameObject.GetComponent<RectTransform>();
        _startPosition = _rectTransform.anchoredPosition3D;
        _endPosition = _startPosition + new Vector3(0, MoveOffset, 0);
        _rectTransform.anchoredPosition3D = _endPosition;
        _currentNotifyType = NotifyType.None;
	}

    void OnDestroy()
    {
        EventManager.OnShowNotificationEvent -= OnShowNotification;
        EventManager.OnHideNotificationEvent -= OnHideNotification;
    }

    void OnShowNotification(EventData e)
    {
        NotifyType nType = (NotifyType)e.Data["type"];
        if (!IsWorkWithNotification(nType))
        {
            return;
        }
        _nextNotifyType = nType;
        StartCoroutine(ShowSequence(false));        
    }    

    void OnHideNotification(EventData e)
    {
        NotifyType nType = (NotifyType)e.Data["type"];
        if (!IsWorkWithNotification(nType) || nType != _currentNotifyType)
        {
            return;
        }
        StopCoroutine("ShowSequence");
        HidePopup();
    }

    protected virtual void HidePopup()
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, _rectTransform.anchoredPosition3D, _endPosition, ShowHideTime)
            .setOnUpdate((Vector3 val) =>
            {
                _rectTransform.anchoredPosition3D = val;
            })
            .setEase(LeanTweenType.easeInBack);
    }

    protected virtual IEnumerator ShowSequence(bool stayOnScreen)
    {
        LeanTween.cancel(gameObject);
        if (_currentNotifyType != _nextNotifyType)
        {
            HidePopup();
            yield return new WaitForSeconds(ShowHideTime);
            _currentNotifyType = _nextNotifyType;
        }
        LeanTween.value(gameObject, _rectTransform.anchoredPosition3D, _startPosition, ShowHideTime)
            .setOnUpdate( (Vector3 val) =>
            {
                _rectTransform.anchoredPosition3D = val;
            })
            .setEase(LeanTweenType.easeOutBack);

        if (!stayOnScreen)
        {
            yield return new WaitForSeconds(ShowingTime);
            LeanTween.value(gameObject, _rectTransform.anchoredPosition3D, _endPosition, ShowHideTime)
                .setOnUpdate((Vector3 val) =>
                {
                    _rectTransform.anchoredPosition3D = val;
                })
                .setEase(LeanTweenType.easeInBack);
        }
        yield return null;
    }	

    private bool IsWorkWithNotification(NotifyType nType)
    {
        for (int i = 0; i < Notifications.Count; ++i)
        {
            if (Notifications[i] == nType)
            {
                return true;
            }
        }
        return false;
    }
}
