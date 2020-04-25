using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;

public class UIButton : MonoBehaviour
{

	private bool _active = true;
	private Button _btn;
	private Text _txt;
	public bool ClickSound = true;
	public int CascadeLevel = 1;

	public string Label
	{
		get
		{
			if(_txt)
			{
				return _txt.text;
			}
			else
			{
				return "";
			}
		}

		set
		{
			if(_txt)
			{
				_txt.text = value;
			}
		}
	}

	void Awake ()
	{
		_btn = GetComponent<Button>();
        _btn.onClick.AddListener(() => OnClick());
		// navigation
		Navigation navi = _btn.navigation;
		navi.mode = Navigation.Mode.None;
		_btn.navigation = navi;
		//
		GameObject _go = HelperFunctions.GetChildGameObject(_btn.gameObject, "Label");
		if(_go)
		{
			_txt = _go.GetComponent<Text>();
		}
		else
		{
			_txt = null;
		}
	}

	void OnClick()
	{
		if(_active)
		{
			if (ClickSound)
			{
				MusicManager.playSound("button_click");
			}
			_active = false;
			if(UIConsts.ENABLED_INTERACTABLE){ _btn.interactable = _active; }
            //Invoke("antiMuliClick", UIConsts.ANTI_MULTI_CKLICK_TIMEOUT);
            antiMuliClick();



            Transform buttonParent = transform.parent;
			if (CascadeLevel > 1)
			{
				for (int i = 1; i < CascadeLevel; ++i)
				{
					buttonParent = buttonParent.parent;
				}
			}

			buttonParent.SendMessage(name + "OnClick", null, SendMessageOptions.RequireReceiver);
		}
	}

	void antiMuliClick()
	{
		_active = true;
		if(UIConsts.ENABLED_INTERACTABLE){ _btn.interactable = _active; }
	}


	public void Disable()
	{
        if (_btn == null)
        {
            _btn = GetComponent<Button>();
        }
        _btn.interactable = false;
	}

	
	public void Enable()
	{
        if (_btn == null)
        {
            _btn = GetComponent<Button>();
        }
        if (_active)
        {
            _btn.interactable = true;
        }
	}

	public bool IsActive()
	{
		return _active && _btn.interactable;
	}
}
