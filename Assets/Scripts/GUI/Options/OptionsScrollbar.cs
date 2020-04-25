using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class OptionsScrollbar : MonoBehaviour {

	private const int STEP = 1;

	private string _currentScene;

	public Scrollbar AScrollbar;
	public Image Filler;

	// Use this for initialization
	void Awake ()
	{
		AScrollbar.onValueChanged.AddListener(OnValueChanged);
	}
		
	public void ButtonLessOnClick ()
	{
		float value = AScrollbar.value;
		int ivalue = (int)(value * 10 + 0.5f);
		ivalue -= STEP;
		if (ivalue < 0)
		{
			ivalue = 0;
		}
		value = ivalue / 10.0f;
		AScrollbar.value = value;
	}

	public void ButtonMoreOnClick ()
	{
		float value = AScrollbar.value;
		int ivalue = (int)(value * 10 + 0.5f);
		ivalue += STEP;
		if (ivalue > 10)
		{
			ivalue = 10;
		}
		value = ivalue / 10.0f;
		AScrollbar.value = value;
	}
		
	void OnValueChanged(float value)
	{
		//GameManager.Instance.Settings.SetMusicVolume(value);
		Filler.fillAmount = value;
	}
		
}
