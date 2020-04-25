using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tutorials: MonoBehaviour 
{
	public GameObject				TutorialPrefab;
	public GameObject				CanvasObject { get; set; }
	public bool						Finished { get; set; }			// when we can't show tutorials (for example when game finished)

	public Dictionary<string, TutorialTip> CurrentTutorials { get; set; }

	void Awake()
	{
		EventManager.OnTutorialNeededEvent += OnTutorialNeeded;
		EventManager.OnTutorialCloseNeededEvent += OnTutorialCloseNeeded;
	}
	
	void OnDestroy()
	{
		EventManager.OnTutorialNeededEvent -= OnTutorialNeeded;
		EventManager.OnTutorialCloseNeededEvent -= OnTutorialCloseNeeded;
	}

	protected void OnTutorialNeeded(EventData e)
	{
		string id = (string)e.Data["id"];
		float x = -6666;
		float y = -6666;
		if (e.Data.ContainsKey("x"))
		{
			x = (float)e.Data["x"];
		}
		if (e.Data.ContainsKey("y"))
		{
			y = (float)e.Data["y"];
		}
		TryShowTutorial(id, x, y);
	}

	protected void OnTutorialCloseNeeded(EventData e)
	{
		string id = (string)e.Data["id"];
		if (id == "")
		{
			// all tutorials
			CloseAllTutorials();
		} else
		{
			// specific tutorial
			TryCloseTutorial(id);
		}
	}

	private bool TryShowTutorial(string id, float x, float y)
	{
		if (GameManager.Instance.Settings.User.IsTutorialShowed(id) || Finished) { return false; }

		if (!GameManager.Instance.Settings.User.Tutorials)
		{
			GameManager.Instance.Settings.User.SetTutorialShowed(id);
			return false;
		}

		TutorialData data = GameManager.Instance.GameData.XMLtutorialsData[id];
		if (x != -6666) { data.X = x; }
		if (y != -6666) { data.Y = y; }
		if (CurrentTutorials.ContainsKey(id))
		{
			return false;
		} else
		{
			ShowTutorial(data);
			return true;
		}
	}

	private void ShowTutorial(TutorialData data)
	{
		GameManager.Instance.Settings.User.SetTutorialShowed(data.Id);
		//
		if (CanvasObject == null)
		{
			CanvasObject = GameObject.Find("UICanvas"); 
		}
		// create tip
		GameObject tipObj =  GameObject.Instantiate (TutorialPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		tipObj.transform.SetParent(CanvasObject.transform, false);
		tipObj.transform.SetAsLastSibling();
		TutorialTip tip = tipObj.GetComponent<TutorialTip>();
		tip.InitTip(data);
		tip.ShowTip();

		CurrentTutorials.Add(data.Id, tip);


	}

	private void CloseAllTutorials()
	{
		// close all scene tutorials
		List<string> ids = new List<string>();
		foreach(KeyValuePair<string, TutorialTip> entry in CurrentTutorials)
		{
			ids.Add(entry.Key);
		}
		for (int i = 0; i < ids.Count; ++i)
		{
			CloseTutorial(ids[i]);
		}
		ids.Clear();
	}

	private void TryCloseTutorial(string id)
	{
		if (!CurrentTutorials.ContainsKey(id))
		{
			return;
		}
		CloseTutorial(id);
	}

	private void CloseTutorial(string id)
	{
		TutorialTip tip = CurrentTutorials[id];
		CurrentTutorials.Remove(id);
		tip.CloseTip();
	}

	public bool IsShowing(string id)
	{
		return CurrentTutorials.ContainsKey(id);
	}

	void Start () 
	{
		Finished = false;
		CanvasObject = null;
		CurrentTutorials = new Dictionary<string, TutorialTip>();
		//string sceneName = GameManager.Instance.GameFlow.GetCurrentScene().ToString();
		//TODO show some tutorials at start
	}
}