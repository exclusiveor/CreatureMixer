using UnityEngine;
using UnityEngine.UI;

public class OptionsWindowUIController : BaseUIController
{
    private string _currentScene;

    public GameObject ButtonRestart;
    public GameObject ButtonMainMenu;
    public GameObject ButtonContinue;
    public GameObject ButtonFacebook;
    public GameObject ButtonRestore;
    public Toggle CheckboxSound;
    public Text VersionInfoText;

	public CanvasGroup CreditsGroup;
	public GameObject CreditsObject;
	private const int CLICKS_TO_SHOW_CREDITS = 13;
	private int clicksToShowCredits = 0;

	protected override void Awake()
    {
        ClickOutside = false;
        ReInit();
        if (CreationMethod != EFormCreationMethod.Dynamic)
        {
            gameObject.SetActive(false);
        }
    }

    public override bool OpenForm(EventData e)
    {
        _currentScene = (string)e.Data["scene"];
        TryRescale();
        Refill();
		// reset credits
		clicksToShowCredits = CLICKS_TO_SHOW_CREDITS;
		CreditsGroup.alpha = 0;
		//
        return true;
    }

    private void Refill()
    {
        bool ismainmenu = _currentScene == "main_menu";
        ButtonRestart.SetActive(!ismainmenu);
        ButtonMainMenu.SetActive(!ismainmenu);
        if (ButtonFacebook != null)
        {
            ButtonFacebook.SetActive(ismainmenu);
        }
        if (ButtonRestore != null)
        {
            ButtonRestore.SetActive(ismainmenu);
        }
#if (PUBLISHING_PLATFORM_AMAZON)
        ButtonRestore.SetActive(false);
#endif
        CheckboxSound.isOn = GameManager.Instance.Settings.User.SoundVolume > 0;
    }

    void Update()
    {
        UpdateInput();
    }

    public override bool EscapeOnClick()
    {
        if (_isHiding)
        {
            return true;
        }
        Hide();
        return true;
    }

    public void ButtonContinueOnClick()
    {
        Hide();
    }

    public void ButtonRestartOnClick()
    {
        Hide();
        GameManager.Instance.BoardData.AGameBoard.RestartGame();
        //EventData eventData = new EventData("OnOpenFormNeededEvent");
        //eventData.Data["form"] = UIConsts.FORM_ID.CONFIRM_WINDOW;
        //eventData.Data["type"] = "confirm_restart";
        //eventData.Data["parameter"] = "";
        //GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
    }

    public void ButtonMainMenuOnClick()
    {
        Hide();
        GameManager.Instance.BoardData.AGameBoard.GoHome();
    }

    public void ButtonHelpOnClick()
    {
        Hide();
        EventData eventData = new EventData("OnOpenFormNeededEvent");
        eventData.Data["form"] = UIConsts.FORM_ID.HELP_WINDOW;
        eventData.Data["caller"] = "options";
        eventData.Data["page"] = 0;
        GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
    }

    public void ButtonTimeScaleOnClick()
    {
        if (Time.timeScale == 0.1f)
        {
            Time.timeScale = 1.0f;
        }
        else
        {
            Time.timeScale = 0.1f;
        }
    }

    public void OnSoundValueChanged()
    {
        float soundvolume = 0;
        float musicvolume = 0;
        if (CheckboxSound.isOn)
        {
            soundvolume = Consts.SOUND_VOLUME_MAX;
            musicvolume = Consts.MUSIC_VOLUME_MAX;
        }
        GameManager.Instance.Settings.SetSoundVolume(soundvolume);
        GameManager.Instance.Settings.SetMusicVolume(musicvolume);
    }

    public void ButtonRestoreOnClick()
    {
#if (UNITY_IOS || UNITY_ANDROID)
        Purchaser purchaser = GameManager.Instance.GetComponent<Purchaser>();
        if (purchaser)
        {
            purchaser.RestorePurchases();
        }
#endif
    }

    public void ButtonFacebookOnClick()
    {
        Application.OpenURL("https://www.facebook.com/brainblendergames");
    }

    //public void ButtonSkin_3OnClick()
    //{
    //    SkinData sd;
    //    sd.Id = 3;
    //    sd.Name = "starter";
    //    sd.BackPrefix = "starter_";
    //    sd.FrontPrefix = "vi_";
    //    sd.PipeStructureType = EPipeStructureType.BackFront;
    //    UpdateSkin(sd);
    //}

    //private void UpdateSkin(SkinData val)
    //{
    //    GameManager.Instance.Player.CurrentSkin = val;
    //    UpdateSkinButtons();
    //    //if (_currentScene == "game_menu")
    //    {
    //        GameManager.Instance.BoardData.AGameBoard.UpdateSkin();
    //    }
    //}

	public void CreditsButtonOnClick()
	{
		
		if (clicksToShowCredits <= 0)
		{
			clicksToShowCredits = CLICKS_TO_SHOW_CREDITS;
			LeanTween.cancel(CreditsObject);
			LeanTween.value(CreditsObject, CreditsGroup.alpha, 0.0f, 0.25f)
				.setOnUpdate((float val)=>
					{
						CreditsGroup.alpha = val;
					});
		} else
		if (clicksToShowCredits > 0)
		{
			--clicksToShowCredits;
			if (clicksToShowCredits <= 0)
			{
				LeanTween.cancel(CreditsObject);
				LeanTween.value(CreditsObject, CreditsGroup.alpha, 1.0f, 0.25f)
					.setOnUpdate((float val)=>
						{
							CreditsGroup.alpha = val;
						});
			}
		}
	}
}