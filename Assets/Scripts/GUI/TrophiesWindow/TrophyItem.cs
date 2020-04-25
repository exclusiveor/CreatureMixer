using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrophyItem : MonoBehaviour
{
	private const float SHOW_HIDE_TIME = 0.15f;

	private Vector3 _slotsPos;
	private float 	_slotSize;

	public GameObject Obj;
	public GameObject CompleteObj;
	public Image Filler;
	public Text MoneyText;
	public Text ProgressText;
	public Text DescriptionText;
	public CanvasGroup Group;
	public ETrophyType Id = ETrophyType.None;

	public void UpdateTrophy(ETrophyType id)
	{
		Id = id;
		Group.alpha = 1;
		TrophyData trophyData = GameManager.Instance.GameData.XMLtrophiesData[id];
		TrophyCompleteData completeData = GameManager.Instance.Player.TrophiesItems[id];
		// progress
		if (trophyData.Param <= 1 || trophyData.IsSingle)
		{
			ProgressText.gameObject.SetActive(false);
		} else
		{
			ProgressText.gameObject.SetActive(true);
			//if (Id == ETrophyType.PlayMoreThan5hoursCumulative)
			//{
			//	int hours = completeData.Param / (60 * 60);
			//	ProgressText.text = hours.ToString() + "/5";
			//} else
			{
			ProgressText.text = completeData.Param.ToString() + "/" + trophyData.Param.ToString();
			}
		}
		// money
		MoneyText.text = trophyData.Reward.ToString();
		// description text
		DescriptionText.text = Localer.GetText(id.ToString());
		// trophy icon
		if (completeData.Completed)
		{
			CompleteObj.SetActive(true);
			Filler.gameObject.SetActive(false);
		} else
		{
			CompleteObj.SetActive(false);
			if (completeData.Param <= 0)
			{
				Filler.gameObject.SetActive(false);
			} else
			{
				Filler.gameObject.SetActive(true);
				Filler.fillAmount = (float)completeData.Param / (float)trophyData.Param;
			}
		}
	}

	public void HideTrophy()
	{
		LeanTween.cancel(Obj);
		float startAlpha = Group.alpha;
		LeanTween.value(Obj, startAlpha, 0, SHOW_HIDE_TIME * startAlpha)
			.setOnUpdate(
				(float val)=>
				{
					Group.alpha = val;
				}
			)
			.setOnComplete(
				()=>
				{
					Obj.SetActive(false);
				}
			);
	}

	public void ShowTrophy()
	{
		Obj.SetActive(true);
		LeanTween.cancel(Obj);
		float startAlpha = 0;
		Group.alpha = 0;
		LeanTween.value(Obj, startAlpha, 1,  SHOW_HIDE_TIME * (1 - startAlpha))
		.setOnUpdate(
				(float val)=>
				{
					Group.alpha = val;
				}
			);
	}

	public void HideTrophyForce()
	{
		LeanTween.cancel(Obj);
		Obj.SetActive(false);
	}

	public void ShowTrophyForce()
	{
		LeanTween.cancel(Obj);
		Obj.SetActive(true);
	}

}
