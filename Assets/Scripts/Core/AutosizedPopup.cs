using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AutosizedPopup  : MonoBehaviour 
{
	// TODO parameter Anchor!!!!! Щоб попап показувати відносно кута

	private const float					SHOW_HIDE_SPEED = 0.2f;

	public Text							Text;
	public CanvasGroup					Group;
	public float						AdditionalHeight = 0;
	public float						AdditionalWidth = 0;
	public Transform					ParentToAdd = null;
	public bool							ScreenCoords = false;

	public float						ExtraDY = 0.1f;
	public float						ExtraDX = 0.0f;

	public int 							DirectionX = 0; // 1 : move right, -1 : move left, 0 : center
	public int 							DirectionY = 1; // 1 : move up, -1 : move down, 0 : center

	public bool							NeedResizing = true;
    private bool                        _resized = true;

	public bool							IsLocked = false;

	public void SetText(string atext)
	{
		Text.text = atext;
		if (NeedResizing)
		{
			_resized = false;			
		} else
		{
			_resized = true;
		}
	}

	public void Resize()
	{
		if (!NeedResizing || _resized) return;
		_resized = true;
		if (!NeedResizing) return;
		if (gameObject.activeSelf)
		{
			StartCoroutine("InvokeResizeNextFrame");
		}
	}

	private IEnumerator InvokeResizeNextFrame()
	{
		yield return null;
		float newWidth = Text.transform.GetComponent<RectTransform>().sizeDelta.x * Text.transform.localScale.x + AdditionalWidth;
		float newHeight = Text.transform.GetComponent<RectTransform>().sizeDelta.y * Text.transform.localScale.y + AdditionalHeight;
		transform.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, newHeight);
	}

	public float GetDy()
	{
		if (ScreenCoords)
		{
			float h = DirectionY * (Text.transform.GetComponent<RectTransform>().sizeDelta.y / 2.0f) + ExtraDY; //// /2 / 100
			return h;
		} else
		{
			float h = DirectionY * (Text.transform.GetComponent<RectTransform>().sizeDelta.y / 200.0f) + ExtraDY; //// /2 / 100
			return h;
		}
	}
	public float GetDx()
	{
		if (ScreenCoords)
		{
			float w = DirectionX * (Text.transform.GetComponent<RectTransform>().sizeDelta.x / 2.0f) + ExtraDX; //// /2 / 100
			return w;
		} else
		{
			float w = DirectionX * (Text.transform.GetComponent<RectTransform>().sizeDelta.x / 200.0f) + ExtraDX; //// /2 / 100
			return w;
		}
	}

	public void ShowPopupForce()
	{		
		gameObject.SetActive(true); //<---
		if (NeedResizing)
		{
			_resized = false;			
		} else
		{
			_resized = true;
		}
		LeanTween.cancel(gameObject);
		Group.alpha = 1;
	}

	public void ShowPopup()
	{		
		if (NeedResizing)
		{
			_resized = false;			
		} else
		{
			_resized = true;
		}
		gameObject.SetActive(true); //<---
		LeanTween.cancel(gameObject);
		transform.SetAsLastSibling();
		LeanTween.value(gameObject, Group.alpha, 1.0f, SHOW_HIDE_SPEED * (1.0f - Group.alpha))			
			.setOnUpdate
				(
					(float val)=>
					{
					Group.alpha = val;
					}
				);
	}

	public void HidePopupForce()
	{		
		LeanTween.cancel(gameObject);
		Group.alpha = 0;
		gameObject.SetActive(false); //<---
	}

	public void HidePopup()
	{	
		LeanTween.cancel(gameObject);
		LeanTween.value(gameObject, Group.alpha, 0.0f, SHOW_HIDE_SPEED * Group.alpha)			
			.setOnUpdate
				(
					(float val)=>
					{
						Group.alpha = val;
					}
				)
			.setOnComplete(() =>
				{
					gameObject.SetActive(false); //<---
				});
	}

	void Awake()
	{
		gameObject.SetActive(false);
		Group.alpha = 0;
	}

	void Start () 
	{
		transform.localScale = new Vector3(1,1,1);
		//gameObject.SetActive(false);
		//Group.alpha = 0;
	}

	void Update()
	{
		if (IsLocked)
		{
			Resize();
		} else {
			Resize ();
			Position ();
		}
	}

	public void Position()
	{
		if (ScreenCoords)
		{
			Vector3 newPos = Input.mousePosition;
			newPos.z = 0;
			newPos.x += GetDx();
			newPos.y += GetDy();
			transform.position = newPos;
		} else
		{
			Camera uiCamera = GameManager.Instance.GameFlow.GetUICamera();
			if (uiCamera)
			{
				Vector3 newPos = uiCamera.ScreenToWorldPoint(Input.mousePosition); // Camera.main.ScreenToWorldPoint(Input.mousePosition);
				newPos.z = 0;
				newPos.x += GetDx() * uiCamera.orthographicSize / 6.190476f; //Camera.main.orthographicSize / 6.190476f;   // 6.190476f - camera size of map
				newPos.y += GetDy() * uiCamera.orthographicSize / 6.190476f; //Camera.main.orthographicSize / 6.190476f;
				transform.position = newPos;
			}
		}
	}
		
}