using UnityEngine;
using System.Collections.Generic;

public enum EHideDirection
{
	Horizontal,
	Vertical,
	Alpha
}

public enum UISetType
{
	Global = 0,
	MainMenu = 1,
	ClassicGame = 2,
	LeveledGame = 3
}

public class UISetsChanger : MonoBehaviour
{
	private const float SHOW_TIME = 0.5f;
	private const float HIDE_TIME = 0.25f;

    private RectTransform _rectTransform;
    private Vector3 _visiblePosition;
    private Vector3 _notVisiblePosition;
	public EHideDirection HideDirection; 
	public float NotVisibleOffset = 200;
	public List<UISetType> SetIds;

	private CanvasGroup _canvasGroup;
	private bool _inited = false;
	private bool _startShowHide = true; // show at start

    void Awake()
    {   
		EventManager.OnUISwitchNeededEvent += OnUISwitchNeeded;        
        _rectTransform = GetComponent<RectTransform>();



//		if (HideDirection == EHideDirection.Horizontal)
//		{
//			_notVisiblePosition.x = -1.0f * (Mathf.Abs(_visiblePosition.x) + NotVisibleOffset);
//			if (_rectTransform.pivot.x == 1.0f)
//			{
//				_notVisiblePosition.x *= -1.0f;
//			}
//		}
//		else if (HideDirection == EHideDirection.Vertical)
//		{
//			_notVisiblePosition.y = -1.0f * (Mathf.Abs(_visiblePosition.y) + NotVisibleOffset);
//			if (_rectTransform.pivot.y == 1.0f)
//			{
//				_notVisiblePosition.y *= -1.0f;
//			}
//		}
    }

	void Start()
	{
		_visiblePosition = _rectTransform.anchoredPosition3D;        
		_notVisiblePosition = _visiblePosition;
		//HideDirection = EHideDirection.Alpha;
		if (HideDirection == EHideDirection.Alpha)
		{
			_canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
		} else
		if (HideDirection == EHideDirection.Horizontal)
		{
			if (_rectTransform.anchorMin.x == 0)
			{
				_notVisiblePosition.x = -1.0f * (Mathf.Abs(_visiblePosition.x) + NotVisibleOffset);
			} else
			if (_rectTransform.anchorMin.x == 1)
			{
				_notVisiblePosition.x = 1.0f * (Mathf.Abs(_visiblePosition.x) + NotVisibleOffset);
			} else
			//if (_rectTransform.anchorMin.x == 0.5f)
			{
				HideDirection = EHideDirection.Alpha;
				_canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
		} else
		if (HideDirection == EHideDirection.Vertical)
		{
			if (_rectTransform.anchorMin.y == 0)
			{
				_notVisiblePosition.y = -1.0f * (Mathf.Abs(_visiblePosition.y) + NotVisibleOffset);
			} else
			if (_rectTransform.anchorMin.y == 1)
			{
				_notVisiblePosition.y = 1.0f * (Mathf.Abs(_visiblePosition.y) + NotVisibleOffset);
			} else
			//if (_rectTransform.anchorMin.y == 0.5f)
			{
				HideDirection = EHideDirection.Alpha;
				_canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
		}
		_inited = true;
		if (_startShowHide)
		{
			ShowElement(false);
		} else
		{
			HideElement(false);
		}
	}

    void OnDestroy()
    {        
		EventManager.OnUISwitchNeededEvent -= OnUISwitchNeeded;        
    }

    // Use this for initialization
	void OnUISwitchNeeded(EventData e)
    {
		bool force = e.Data.ContainsKey("force") && (bool)e.Data["force"] == true;
		UISetType setid = (UISetType)e.Data["setid"];

		bool needShow = false;
		for (int i = 0; i < SetIds.Count; ++i)
		{
			if (SetIds[i] == setid)
			{
				needShow = true;
				break;
			}
		}

		if (needShow)
		{
			if (_inited)
			{
				ShowElement(!force);
			} else
			{
				_startShowHide = true;
			}
		} else
		{
			if (_inited)
			{
				HideElement(!force);
			} else
			{
				_startShowHide = false;
			}
		}
    }

    private void HideElement(bool animated)
    {
        if (animated)
        {
			if (HideDirection == EHideDirection.Alpha)
			{
				LeanTween.value(gameObject, _canvasGroup.alpha, 0, HIDE_TIME)
					.setOnUpdate((float alpha) =>
						{
							_canvasGroup.alpha = alpha;
						})
					.setOnComplete(() =>
						{
							gameObject.SetActive(false);
						});
			} else
			{
				LeanTween.value(gameObject, _rectTransform.anchoredPosition3D, _notVisiblePosition, HIDE_TIME)
					.setOnUpdate((Vector3 pos) =>
						{
							_rectTransform.anchoredPosition3D = pos;
						})
					.setOnComplete(() =>
						{
							gameObject.SetActive(false);
						});
			}
        } else
        {
			if (HideDirection == EHideDirection.Alpha)
			{
				_canvasGroup.alpha = 0;
				gameObject.SetActive(false);
			} else
			{
				_rectTransform.anchoredPosition3D = _notVisiblePosition;
				gameObject.SetActive(false);
			}
        }
    }

    private void ShowElement(bool animated)
    {
        gameObject.SetActive(true);
        if (animated)
        {            
			if (HideDirection == EHideDirection.Alpha)
			{
				LeanTween.value(gameObject, _canvasGroup.alpha, 1, SHOW_TIME)
					.setOnUpdate((float alpha) =>
						{
							_canvasGroup.alpha = alpha;
						});
			} else
			{
				LeanTween.value(gameObject, _rectTransform.anchoredPosition3D, _visiblePosition, SHOW_TIME)
					.setOnUpdate((Vector3 pos) =>
						{
							_rectTransform.anchoredPosition3D = pos;
						});
			}
        } else
        {
			if (HideDirection == EHideDirection.Alpha)
			{
				_canvasGroup.alpha = 1;
			} else
			{
				_rectTransform.anchoredPosition3D = _visiblePosition;
			}
        }
    }   
}
