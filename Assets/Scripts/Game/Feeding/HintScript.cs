using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HintScript : MonoBehaviour {

	private const float HIDING_SPEED = 0.3f;
	private const float SHOWING_SPEED = 0.3f;
	public List<SpriteRenderer> ARenderers;
	public List<GameObject> AObjects;
	public GameObject AGameObject;

	public void ShowHint(GameBoard.MatchHintData mhData, GameBoard gBoard)
    {
		// rotating
		if (mhData.XA != mhData.XB)
		{
			// horizontal slide
			if (mhData.XA < mhData.XB)
			{
				// slide right
				//Helpers.SetRotationDgr(AGameObject, 0);
			} else
			{
				// slide left
				Helpers.SetRotationDgr(AGameObject, 180);
			}
		} else
		{
			// vertical slide
			if (mhData.YA < mhData.YB)
			{
				// slide up
				Helpers.SetRotationDgr(AGameObject, 90);
			} else
			{
				// slide down
				Helpers.SetRotationDgr(AGameObject, -90);
			}
		}

		// positioning
		SSlot slot = gBoard.GetSlot(mhData.XA, mhData.YA);
		if (slot != null)
		{
			AGameObject.transform.SetParent(slot.transform.parent, false);
			Vector3 pos = slot.transform.position;
			pos.z = -5;
			AGameObject.transform.position = pos;
		}
		// fade in
		Color startColor = new Color(1, 1, 1, 0);
		Color finishColor = new Color(1, 1, 1, 1);
		for (int i = 0; i < AObjects.Count; ++i)
		{
			GameObject obj = AObjects[i];
			SpriteRenderer rend = ARenderers[i];
			LeanTween.cancel(obj);
			rend.color = startColor;
			LeanTween.value(obj, startColor, finishColor, HIDING_SPEED)
				.setOnUpdate(
					(Color val)=>{
						rend.color = val;
					}
				)
				.setOnComplete(
					()=>{
						AnimateHint();
					}
				);

		}

		//
		float startX = 0.0f;
		float finishX = 2.0f;
		Helpers.SetXLocal(AObjects[0], startX);
		LeanTween.moveLocalX(AObjects[0], finishX, 0.2f).setLoopPingPong(-1); //.setEase(LeanTweenType.easeInOutElastic);
    }

    public void HideHint()
    {
		Color finishColor = new Color(1, 1, 1, 0);
		for (int i = 0; i < AObjects.Count; ++i)
		{
			GameObject obj = AObjects[i];
			SpriteRenderer rend = ARenderers[i];
			LeanTween.cancel(obj);
			LeanTween.value(obj, rend.color, finishColor, HIDING_SPEED)
				.setOnUpdate(
					(Color val)=>
						{
							rend.color = val;
						}
			);
		}
		GameObject.Destroy(AGameObject, HIDING_SPEED + 0.1f);
    }

	public void HideHintForce()
	{
		for (int i = 0; i < AObjects.Count; ++i)
		{
			LeanTween.cancel(AObjects[i]);
		}
		GameObject.Destroy(AGameObject);
	}

	private void AnimateHint()
	{

	}

}