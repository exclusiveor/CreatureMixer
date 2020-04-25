using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TrophiesUI : MonoBehaviour
{
	private const float MOVE_SPEED = 720.0f;
	public Vector3 StartPos = new Vector3(-200, -100, 0);
	public float DY = 170;
	public GameObject TrophyPopupPrefab;
	public Transform CameraTransform;
	public GameObject CameraObject;

	public Canvas TrophiesCanvas;

	private List<TrophyPopup> _pool = new List<TrophyPopup>();
	private List<TrophyPopup> _popups  = new List<TrophyPopup>();

	void Awake()
	{
		DontDestroyOnLoad(TrophiesCanvas.transform.gameObject);
		CameraTransform.SetParent(transform.parent.parent, false);
		CameraTransform.localScale = Vector3.one;
		CameraTransform.position = new Vector3(0, 0, -10);
		DontDestroyOnLoad(CameraObject);
		EventManager.OnTrophyCompletedEvent += OnTrophyCompleted;
	}

	void OnDestroy()
	{
		EventManager.OnTrophyCompletedEvent -= OnTrophyCompleted;
	}

	TrophyPopup GetFreePopup()
	{
		for (int i = 0; i < _pool.Count; ++i)
		{
			if (!_pool[i].gameObject.activeSelf)
			{
				LeanTween.cancel(_pool[i].gameObject);
				return _pool[i];
			}
		}
		GameObject obj = Instantiate(TrophyPopupPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.SetParent(transform, false);                    
		//obj.name = "slot";
		TrophyPopup tPopup = obj.GetComponent<TrophyPopup>();
		_pool.Add(tPopup);
		return tPopup;
	}

	private Vector3 GetPositionByNumber(int i)
	{
		Vector3 res = StartPos;
		res.y -= i * DY;
		return res;
	}

	private void OnTrophyCompleted(EventData e)
	{
		ETrophyType tType = (ETrophyType)e.Data["type"];
		TrophyPopup tPopup = GetFreePopup();
		int i = _popups.Count;
		_popups.Add(tPopup);
		tPopup.transform.localPosition = GetPositionByNumber(i);
		tPopup.InitPopup(tType);
	}

	private void OnPopupHided(Object popup)
	{
		TrophyPopup tPopup = (TrophyPopup)popup;
		_popups.Remove(tPopup);

		for (int i = 0; i < _popups.Count; ++i)
		{
			GameObject obj = _popups[i].gameObject;
			float currentY = obj.transform.localPosition.y;
			Vector3 neededPos = GetPositionByNumber(i);
			if (currentY != neededPos.y)
			{
				LeanTween.cancel(obj);
				float atime = Mathf.Abs(currentY - neededPos.y) / MOVE_SPEED;
				LeanTween.moveLocalY(obj, neededPos.y, atime);
			}
		}
	}

	void Update()
	{
		CameraObject.SetActive(false);
		CameraObject.SetActive(true);

//		if (Input.GetMouseButtonDown(1))		//for test only
//		{
//			EventData eventData = new EventData("OnTrophyCompletedEvent");
//			eventData.Data["type"] = ETrophyType.FinishXPuzzle0;
//			GameManager.Instance.EventManager.CallOnTrophyCompletedEvent(eventData);
//		}
	}
		
}
