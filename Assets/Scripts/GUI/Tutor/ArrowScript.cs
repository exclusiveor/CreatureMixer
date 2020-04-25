using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//using System.Collections.Generic;

public class ArrowScript: MonoBehaviour 
{
	private const float BOUNCE_SPEED = 0.25f;
	private const float SHOW_HIDE_ARROW_SPEED = 0.25f;
	public GameObject ArrowObj;
	public float BounceDistance = 20.0f;
	public bool IsUI = true;
	public bool HideAtStart = true;

	public CanvasGroup Group;

	public void HideArrow()
	{
		LeanTween.cancel(gameObject);
		if (IsUI)
		{
			float time = SHOW_HIDE_ARROW_SPEED * Group.alpha;
			LeanTween.value(gameObject, Group.alpha, 0.0f, time)
				//.setEase(UIConsts.SHOW_EASE)
				//	.setDelay(UIConsts.SHOW_DELAY_TIME)
				.setOnUpdate
					(
						(float val)=>
						{
							Group.alpha = val;
						}
					);
		} else
		{
			float startAlpha = ArrowObj.GetComponent<SpriteRenderer>().color.a;
			float time = SHOW_HIDE_ARROW_SPEED * startAlpha;
			LeanTween.value(gameObject, startAlpha, 0.0f, time)
				//.setEase(UIConsts.SHOW_EASE)
				//	.setDelay(UIConsts.SHOW_DELAY_TIME)
				.setOnUpdate
					(
						(float val)=>
						{
							Helpers.SetAlpha(ArrowObj, val);
						}
					);
		}
	}

	public void HideArrowForce()
	{
		LeanTween.cancel(gameObject);
		if (IsUI)
		{
			Group.alpha = 0;
		} else
		{
			Helpers.SetAlpha(ArrowObj, 0);
		}
	}

	public void ShowArrow()
	{
		LeanTween.cancel(gameObject);
		if (IsUI)
		{
			float time = SHOW_HIDE_ARROW_SPEED * (1 - Group.alpha);
			LeanTween.value(gameObject, Group.alpha, 1.0f, time)
				//.setEase(UIConsts.SHOW_EASE)
				//	.setDelay(UIConsts.SHOW_DELAY_TIME)
				.setOnUpdate
					(
						(float val)=>
						{
							Group.alpha = val;
						}
					);
		} else
		{
			float startAlpha = ArrowObj.GetComponent<SpriteRenderer>().color.a;
			float time = SHOW_HIDE_ARROW_SPEED * (1 - startAlpha);
			LeanTween.value(gameObject, startAlpha, 1.0f, time)
				//.setEase(UIConsts.SHOW_EASE)
				//	.setDelay(UIConsts.SHOW_DELAY_TIME)
				.setOnUpdate
					(
						(float val)=>
						{
							Helpers.SetAlpha(ArrowObj, val);
						}
					);
		}
	}

	public void ShowArrowForce()
	{
		LeanTween.cancel(gameObject);
		if (IsUI)
		{
			Group.alpha = 1;
		} else
		{
			Helpers.SetAlpha(ArrowObj, 1);
		}
	}

	public void SetFromTopToBottom()
	{
		transform.localRotation = Quaternion.Euler(0, 0, 0);
	}

	public void SetFromBottomToTop()
	{
		transform.localRotation = Quaternion.Euler(0, 0, 180);
	}

	public void SetFromLeftToRight()
	{
		transform.localRotation = Quaternion.Euler(0, 0, 90);
	}

	public void SetFromRightToLeft()
	{
		transform.localRotation = Quaternion.Euler(0, 0, -90);
	}

	void Awake()
	{
		if (HideAtStart)
		{
			HideArrowForce();
		}
	}

	void Start () 
	{
		LeanTween.moveLocalY(ArrowObj, BounceDistance, BOUNCE_SPEED).setLoopPingPong().setLoopCount(-1).setEase(LeanTweenType.easeInOutSine);
	}
}