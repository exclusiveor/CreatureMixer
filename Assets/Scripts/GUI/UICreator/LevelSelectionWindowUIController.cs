using UnityEngine;

public class LevelSelectionWindowUIController : BaseUIController {

    public LevelSelectionButtons Buttons;
	private CanvasGroup _canvasGroup = null;

	public override bool OpenForm(EventData e)
	{
		TryRescale();
        Buttons.ShowCurrentPage();
        return true;
	}

    public override bool EscapeOnClick()
    {
        if (_isHiding)
        {
            return true;
        }
        ButtonHomeOnClick();
        return true;
    }

    protected override void Awake ()
	{
		if (CreationMethod != EFormCreationMethod.Dynamic)
		{
			gameObject.SetActive(false);
		}
		_canvasGroup = gameObject.GetComponent<CanvasGroup>();
		_canvasGroup.alpha = 0;
	}

//	public override void ReInit()
//	{
//		base.ReInit();
//    }

	private void ButtonHomeOnClick ()
	{
		Hide();
		GameManager.Instance.CurrentMenu = UISetType.MainMenu;
		EventData eventData = new EventData("OnUISwitchNeededEvent");
		eventData.Data["setid"] = UISetType.MainMenu;
		GameManager.Instance.EventManager.CallOnUISwitchNeededEvent(eventData);
	}

	public void LevelButtonOnClick(UnityEngine.Object buttonObject)
	{
		Hide();

		//MusicManager.PlayGameTracks();

		LevelButton lbutton = (LevelButton)buttonObject;
		int level = lbutton.Level;
		GameManager.Instance.Player.CurrentLevel = level;

		GameManager.Instance.CurrentMenu = UISetType.LeveledGame;
		EventData eventData = new EventData("OnUISwitchNeededEvent");
		eventData.Data["setid"] = UISetType.LeveledGame;
		GameManager.Instance.EventManager.CallOnUISwitchNeededEvent(eventData);

        //TODO switch UI
        GameManager.Instance.Game.ClearBoardForce();
        GameManager.Instance.Game.PlayLeveledGame();
	}

	public override void Show ()
	{
		myRect.anchoredPosition3D = UIConsts.STOP_POSITION;
		_canvasGroup.interactable = false;
		_isHiding = true;
		Active = false;
		//float delayBeforeShow = _delayBeforeShow;
		//if (delayBeforeShow < 0)
		//{
		//	delayBeforeShow = UIConsts.SHOW_DELAY_TIME;
		//}
		//LeanTween.delayedCall(delayBeforeShow, () => {MusicManager.playSound("popup_show_hide");});
		LeanTween.value(gameObject, _canvasGroup.alpha, 1.0f, UIConsts.SHOW_TWEEN_TIME)
			//.setDelay(delayBeforeShow)
			.setOnUpdate(
				(float val)=>
				{
					_canvasGroup.alpha = val;
				}
			).setOnComplete
			(
				()=>
				{
					_isHiding = false;
					Active = true;
					_canvasGroup.interactable = true;
					OnShowed();
				}
			);
	}


	public override void Hide ()
	{
		_isHiding = true;
		_canvasGroup.interactable = false;
		//MusicManager.playSound("popup_show_hide");
		LeanTween.value(gameObject, _canvasGroup.alpha, 0.0f, UIConsts.HIDE_TWEEN_TIME)
			//.setDelay(UIConsts.HIDE_DELAY_TIME)
			.setOnUpdate(
				(float val)=>
				{
					_canvasGroup.alpha = val;
				}
			).setOnComplete
			(
				()=>
				{
					GameManager.Instance.EventManager.CallOnHideWindowEvent();
					gameObject.SetActive(false);
					Active = false;
					_isHiding = false;
					OnHided();
				}
			);
	}

}
