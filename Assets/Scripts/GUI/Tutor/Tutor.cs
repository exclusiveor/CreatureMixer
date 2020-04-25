using UnityEngine;

public class Tutor  : MonoBehaviour 
{
	private const float SHOW_HIDE_TIME = 0.2f;
    public GameObject InnerData = null;

	public void ShowData(string id, bool force)
	{
        GameObject newData = CreateInnerData(id);
		if (force)
		{
			DestroyInnerData();
			InnerData = newData;
		} else
		{
			if (InnerData != null)
			{
                // smooth
                HideData(InnerData);
				InnerData = newData;
				ShowData(InnerData);
			} else
			{
				// force
				InnerData = newData;
			}
		}
    }

	private void DestroyInnerData()
	{
		if (InnerData != null)
		{
            InnerData.transform.SetParent(null);
			GameObject.Destroy(InnerData);
			InnerData = null;
		}
	}

	private void ShowData(GameObject obj)
	{
		CanvasGroup group = obj.GetComponent<CanvasGroup>();
		group.alpha = 0;
		LeanTween.value(obj, 0, 1.0f, SHOW_HIDE_TIME)
			//.setEase(UIConsts.SHOW_EASE)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setOnUpdate
				(
					(float val)=>
					{
						obj.GetComponent<CanvasGroup>().alpha = val;
					}
				);
	}

	private void HideData(GameObject obj)
	{
		LeanTween.cancel(obj);
		CanvasGroup group = obj.GetComponent<CanvasGroup>();
		LeanTween.value(obj, group.alpha, 0.0f, SHOW_HIDE_TIME * group.alpha)
			//.setEase(UIConsts.SHOW_EASE)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setOnUpdate
				(
					(float val)=>
					{
						obj.GetComponent<CanvasGroup>().alpha = val;
					}
				)
				.setOnComplete(
					() => {
					GameObject.Destroy(obj, 0.1f);
					}
				);
	}

	GameObject CreateInnerData(string id)
	{
		GameObject res = (GameObject)Instantiate(Resources.Load("Prefabs/UI/tutor/Tutors/TID_" + id));
		res.transform.SetParent(transform);
		res.transform.localScale = new Vector3(1, 1, 1);
		res.transform.localPosition = new Vector3(0, 0, 0);
		//Text text = res.transform.Find("Text").GetComponent<Text>();
		//text.text = Localer.GetText("TID_" + id);
		return res;
	}

	public static ArrowScript CreateArrow(bool isUI, Transform aparent)
	{
		GameObject arrowObj = null;
		if (isUI)
		{
			arrowObj = (GameObject)Instantiate(Resources.Load("Prefabs/UI/tutor/ArrowUI"));
		} else
		{
			arrowObj = (GameObject)Instantiate(Resources.Load("Prefabs/UI/tutor/ArrowScene"));
		}
		arrowObj.transform.SetParent(aparent);
		arrowObj.transform.SetAsLastSibling();
		arrowObj.transform.localScale = new Vector3(1, 1, 1);
		return arrowObj.GetComponent<ArrowScript>();
	}

	//public static ArrowScript CreateArrowTutor37()
	//{
	//	GameObject arrowObj = null;
	//	arrowObj = (GameObject)Instantiate(Resources.Load("Prefabs/UI/tutor/Tutor37Prefab"));
	//	arrowObj.transform.SetParent(GameManager.Instance.GameFlow.GetFormFromID(UIConsts.FORM_ID.MATCH3_MAIN_MENU).transform.parent);
	//	arrowObj.transform.SetAsLastSibling();
	//	arrowObj.transform.localScale = new Vector3(1, 1, 1);
	//	return arrowObj.GetComponent<ArrowScript>();
	//}
}