using UnityEngine;

public class HelpWindowUIController : BaseUIController {

	private string _caller;			
	private int	_currentPage;
	private Tutor _tutor;
    public Tutor Tutor;
	public GameObject LeftButton;
    public GameObject RightButton;

	public override bool OpenForm(EventData e)
	{
		TryRescale();
		_currentPage = 0;
		_caller = (string)e.Data["caller"];
		_currentPage = (int)e.Data["page"];
		ChangePageForce();
		return true;
	}
	
	// Use this for initialization
	void Start ()
	{
		_currentPage = 0;
		_caller = "";
		if (CreationMethod != EFormCreationMethod.Dynamic)
        {
            gameObject.SetActive(false);
        }
	}

	public void ButtonOkOnClick ()
	{
		//Debug.Log(HelperFunctions.GetCurrentMethod() + " " + this.name);
		Hide ();
		if (_caller == "options")
		{
			// open options again
			EventData eventData = new EventData("OnOpenFormNeededEvent");
			eventData.Data["form"] = UIConsts.FORM_ID.OPTIONS_WINDOW;
			if (GameManager.Instance.GameFlow.GetCurrentScene() == UIConsts.SCENE_ID.MAINMENU)
			{
				eventData.Data["scene"] = "mainmenu";
			} else
			{
				eventData.Data["scene"] = "match3";
			}
			GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
		}
	}

	public void UpdateArrowButtons()
	{
		if (_currentPage == 0)
		{
			LeftButton.SetActive(false);
		} else
		{
			LeftButton.SetActive(true);
			LeftButton.transform.GetComponent<UIButton>().Enable();
		}

		int maxPage = GameManager.Instance.GameData.XMLhelpPagesData.Count - 1;
		#if UNITY_STANDALONE
		--maxPage;
		#else
		#endif
		if (_currentPage >= maxPage)
		{
			RightButton.SetActive(false);
		} else
		{
			RightButton.SetActive(true);
			RightButton.transform.GetComponent<UIButton>().Enable();
		}
	}

	public void ButtonLeftOnClick ()
	{
		if (_currentPage > 0)
		{
			--_currentPage;
			ChangePage();
		}
	}

	public void ButtonRightOnClick ()
	{
		int maxPage = GameManager.Instance.GameData.XMLhelpPagesData.Count - 1;
		#if UNITY_STANDALONE
		--maxPage;
		#else
		#endif
		if (_currentPage < maxPage)
		{
			++_currentPage;
			ChangePage();
		}
	}

	private void ChangePage()
	{
		Tutor.ShowData(GameManager.Instance.GameData.XMLhelpPagesData[_currentPage], false);
		UpdateArrowButtons();
	}

	private void ChangePageForce()
	{
		Tutor.ShowData(GameManager.Instance.GameData.XMLhelpPagesData[_currentPage], true);
		UpdateArrowButtons();
	}

}
