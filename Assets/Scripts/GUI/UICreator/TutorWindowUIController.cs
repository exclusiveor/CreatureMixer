using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorWindowUIController : BaseUIController {

	private Dictionary<string, Vector3> Positions;
	private List<string> Queue;
	private string _id;
    public Tutor ATutor;
    //private GameObject _greenButton;

    protected override void Awake()
	{
		base.Awake();
		//_greenButton = transform.Find ("ButtonOk").gameObject;
		Queue = new List<string>();
		gameObject.SetActive(false);
		Positions = new Dictionary<string, Vector3>();

        if (CreationMethod != EFormCreationMethod.Dynamic)
        {
            gameObject.SetActive(false);
        }
    }

	public override bool OpenForm(EventData e)
	{
		//TryRescale();
		string id = (string)e.Data["id"];

		if (e.Data.ContainsKey("hide"))
		{
			if (id == "" || id == _id)
			{
				ButtonOkOnClick();
			}
			return false;
		}

		bool exclusive = (bool)e.Data["exclusive"];
		if (exclusive && (Queue.Count > 0 || gameObject.activeSelf))
		{
			return false; // can't show this tutorial (tutors in Queue or window now show another tutor)
		}
		bool toQueue = (bool)e.Data["toqueue"];
		Vector3 pos = Vector3.zero;
		if (e.Data.ContainsKey("pos"))
		{
			pos = (Vector3)e.Data["pos"];
		}
		return TryShowTutorial(id, toQueue, true, pos);
	}

    public override bool EscapeOnClick()
    {
        if (_isHiding)
        {
            return true;
        }
        ButtonOkOnClick();
        return true;
    }

    private bool TryShowTutorial(string id, bool toQueue, bool forceChange, Vector3 pos)
	{
		if (id == "")
		{
			// try show tutorial from queue
			//toQueue = false;
			if (Queue.Count == 0)
			{
				return false;
			} else
			{
				id = Queue[0];
				Queue.RemoveAt(0);
			}
		} else
		{
			if (!Positions.ContainsKey(id))
			{
				Positions.Add(id, pos);
			}
		}

		if (GameManager.Instance.Settings.User.IsTutorialShowed(id)) { return false; }
		
		if (!GameManager.Instance.Settings.User.Tutorials)
		{
			GameManager.Instance.Settings.User.SetTutorialShowed(id);
			return false;
		}

		if (toQueue)
		{
			if (!Queue.Contains(id))
			{
				Queue.Add(id);
			}
			return false;
		} else
		{
			//TutorialData data = GameManager.Instance.GameData.XMLtutorialsData[id];
			ShowTutorial(id, forceChange);
			return true;
		}
	}

	private void ShowTutorial(string id, bool force)
	{
		_id = id;
		GameManager.Instance.Settings.User.SetTutorialShowed(id);
        ATutor.ShowData(id, force);
	}

	// Use this for initialization
	void Start ()
	{
		ReInit();
	}

	public void ButtonOkOnClick ()
	{
		//Debug.Log(HelperFunctions.GetCurrentMethod() + " " + this.name);
		if (!TryShowTutorial("", false, false, Vector3.zero))
		{
			Hide ();
		}
	}

	public override void Show ()
	{
		myRect.anchoredPosition3D = Positions[_id];
		gameObject.SetActive(true);
	}

	public override void Hide ()
	{
        gameObject.SetActive(false);
        GameManager.Instance.EventManager.CallOnHideWindowEvent();
	}

	public override void ReInit()
	{
		base.ReInit();
	}
}
