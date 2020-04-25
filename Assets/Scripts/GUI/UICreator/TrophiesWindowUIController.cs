using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TrophiesWindowUIController : BaseUIController {

	//private string _currentScene;
	private const int HEIGHT = 4;
	private const float SCROLL_SPEED = 0.2f;
	private const float DY = 70.0f;
	private const float START_Y = 150;

    public GameObject ButtonUp;
    public GameObject ButtonDown;

    public Text CaptionText;
	private List<TrophyItem> Items;
	private List<ETrophyType> Ids;
	private int _current;
	private bool _canScroll;

	new void Awake ()
	{
		ClickOutside = false;
        CreateItems();
		Refill();
		_canScroll = true;
	}
	
    void OnEnable() {
        //EventManager.OnFacebookLoginEvent += RefreshButtonFacebook;
    }

    void OnDisable() {
        //EventManager.OnFacebookLoginEvent -= RefreshButtonFacebook;
    }

	// Update is called once per frame
	void Update ()
	{
		UpdateInput ();
	}

	public override bool OpenForm(EventData e)
	{
		//_currentScene = (string)e.Data["scene"];
		TryRescale();
		_canScroll = true;
		// update visible trophies
		for (int i = 1; i <= HEIGHT; ++i)
		{
			Items[i].UpdateTrophy(Items[i].Id);
		}
		// update caption
		int completed = 0;
		foreach(var td in GameManager.Instance.Player.TrophiesItems)
		{
			if (td.Value.Completed)
			{
				++completed;
			}
		}
		CaptionText.text = Localer.GetText("Trophies") + " " + completed.ToString() + "/" + GameManager.Instance.Player.TrophiesItems.Count.ToString();
		//
		return true;
	}

	public void ButtonCloseOnClick ()
	{
		Hide ();
	}

	public void TrophiesButtonUpOnClick ()
	{
//		if (_canScroll)
//		{
//			ScrollUp();
//		}
	}

	public void TrophiesButtonDownOnClick ()
	{
//		if (_canScroll)
//		{
//			ScrollDown();
//		}
	}

	public void TrophiesButtonUpOnPressed ()
	{
		if (_canScroll)
		{
			ScrollUp();
		}
	}

	public void TrophiesButtonDownOnPressed ()
	{
		if (_canScroll)
		{
			ScrollDown();
		}
	}


	public void ScrollUp()
	{
		_canScroll = false;
		--_current;
		// hide bottom item
		Items[4].HideTrophy();
		// show new
		Items[0].transform.localPosition = new Vector3(0, GetItemPos(0), 0);
		Items[0].UpdateTrophy(Ids[_current]);
		Items[0].ShowTrophy();
		// move items down
		for (int i = 0; i < Items.Count - 1; ++i)
		{
			GameObject obj = Items[i].gameObject;
			LeanTween.moveLocalY(obj, GetItemPos(i + 1), SCROLL_SPEED);
		}
		Invoke("SetCanScroll", SCROLL_SPEED + 0.01f);
		// reform list
		TrophyItem temp = Items[5];
		for (int i = 4; i >= 0; --i)
		{
			Items[i + 1] = Items[i];
		}
		Items[0] = temp;
	}

	public void ScrollDown()
	{
		_canScroll = false;
		++_current;
		// hide top item
		Items[1].HideTrophy();
		// show new
		Items[5].transform.localPosition = new Vector3(0, GetItemPos(5), 0);
		Items[5].UpdateTrophy(Ids[_current + 3]);
		Items[5].ShowTrophy();
		// move items down
		for (int i = 0; i < Items.Count; ++i)
		{
			GameObject obj = Items[i].gameObject;
			LeanTween.moveLocalY(obj, GetItemPos(i - 1), SCROLL_SPEED);
		}
		Invoke("SetCanScroll", SCROLL_SPEED + 0.01f);
		// reform list
		TrophyItem temp = Items[0];
		for (int i = 1; i < Items.Count; ++i)
		{
			Items[i - 1] = Items[i];
		}
		Items[5] = temp;
	}

	void SetCanScroll()
	{
		_canScroll = true;
		UpdateButtons();
	}

	float GetItemPos(int i)
	{
		return START_Y - DY * i;
	}

	void CreateItems()
	{
		Ids = new List<ETrophyType>();
		foreach(var tt in GameManager.Instance.GameData.XMLtrophiesData)
		{
			Ids.Add(tt.Key);
		}

		Items = new List<TrophyItem>();
		GameObject prefab = Resources.Load ("Prefabs/UI/trophies_window/trophyItem") as GameObject;
		for (int i = 0; i < 6; ++i)
		{
			GameObject obj = Instantiate (prefab);
			TrophyItem item = obj.GetComponent<TrophyItem>();
			Items.Add(item);
			obj.transform.SetParent(transform);
			obj.transform.localScale = new Vector3(1,1,1);
		}
	}

	private void Refill()
	{
		_current = 0;
		for (int i = 1; i <= HEIGHT; ++i)
		{
			if (i <= Ids.Count)
			{
				Items[i].UpdateTrophy(Ids[_current + i - 1]);
				Items[i].transform.localPosition = new Vector3(0, GetItemPos(i), 0);
			} else
			{
				Items[i].HideTrophyForce();
			}
		}
		Items[0].HideTrophyForce();
		Items[5].HideTrophyForce();
		UpdateButtons();
	}

	void UpdateButtons()
	{
		ButtonUp.SetActive(_current != 0);
		ButtonDown.SetActive(_current < Ids.Count - HEIGHT);
	}

}
