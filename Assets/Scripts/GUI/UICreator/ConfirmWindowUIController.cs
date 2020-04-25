using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Advertisements;

public class ConfirmWindowUIController : BaseUIController {

	private string _atype;
	private string _parameter;
	public GameObject ButtonYes;
    public GameObject ButtonNo;
    private Vector3 _buttonYesPos;
    private Vector3 _buttonNoPos;

	public override bool OpenForm(EventData e)
	{
		TryRescale();
		_atype = (string)e.Data["type"];
		_parameter = (string)e.Data["parameter"];
		ReInit();
		return true;
	}

    public override bool EscapeOnClick()
    {
        //Hide();
        return true;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    // Use this for initialization
    void Start ()
	{
		_buttonYesPos = ButtonYes.transform.localPosition;
		_buttonNoPos = ButtonNo.transform.localPosition;
		gameObject.SetActive(false);
	}

//	public override void Reset()
//	{
//		ReInit();
//	}

	public override void ReInit()
	{
		base.ReInit();
   //     if (_atype == "confirm_quit")
   //     {
			//transform.Find("CaptionText").GetComponent<Text>().text = Localer.GetText("ConfirmQuitLevel");
   //         SetMultiButton();
   //     }
   //     else if (_atype == "confirm_restart")
   //     {
			//transform.Find("CaptionText").GetComponent<Text>().text = Localer.GetText("ConfirmRestart");
   //         SetMultiButton();
   //     }
    }

	private void ButtonYesOnClick ()
	{
		Hide();
//		if (_atype == "confirm_quit")
//		{
//			EventData eventData = new EventData("OnNeedSaveLevelEvent");
//			GameManager.Instance.EventManager.CallOnNeedSaveLevelEvent(eventData);
////			if (_parameter == "exit")
////			{
////					Application.Quit();
////			} else
//			{
//				MusicManager.PlayMainMenuTrack();
//				GameManager.Instance.Settings.Save(true);
//				GameManager.Instance.GameFlow.TransitToScene(UIConsts.SCENE_ID.MAINMENU);
//			}
//		} else
//        if (_atype == "confirm_restart")
//        {            
//			GameManager.Instance.BoardData.AGameBoard.RestartGame();
//        }
	}

	private void ButtonNoOnClick ()
	{
		Hide();
		//if (_atype == "confirm_quit")
		//{
  //          //			if (_parameter == "exit")
  //          //			{
  //          //
  //          //			} else
  //          {
  //          	EventData eventData = new EventData("OnOpenFormNeededEvent");
  //          	eventData.Data["form"] = UIConsts.FORM_ID.OPTIONS_WINDOW;
  //          	eventData.Data["scene"] = "match3";
  //          	GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
  //          }
  //      }
    }
	
	private void SetMultiButton()
	{
		ButtonYes.SetActive(true);
		ButtonNo.SetActive(true);
		ButtonYes.transform.localPosition = _buttonYesPos;
		ButtonNo.transform.localPosition = _buttonNoPos;
	}

	private void SetSingleButton()
	{
		ButtonYes.SetActive(true);
		ButtonNo.SetActive(false);
		ButtonYes.transform.localPosition = (_buttonYesPos + _buttonNoPos) / 2;
	}

	void Update()
	{
		if (GameManager.Instance.GameFlow.GetCurrentActiveWindowId() != UIConsts.FORM_ID.CONFIRM_WINDOW) return;
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null, null);
	}

    //public override void Hide()
    //{
    //    _isHiding = true;
    //    EventData eventData = new EventData("OnHideWindowEvent");
    //    GameManager.Instance.EventManager.CallOnHideWindowEvent(eventData);
    //    MusicManager.playSound("popup_show_hide");
    //    LeanTween.value(gameObject, UIConsts.STOP_POSITION, UIConsts.START_POSITION, UIConsts.HIDE_TWEEN_TIME)
    //        .setEase(UIConsts.HIDE_EASE)
    //            .setDelay(UIConsts.HIDE_DELAY_TIME)
    //            .setOnUpdate(
    //                (Vector3 val) =>
    //                {
    //                    myRect.anchoredPosition3D = val;
    //                }
    //            ).setOnComplete
    //            (
    //                () =>
    //                {
    //                    gameObject.SetActive(false);
    //                    Active = false; //?
    //                    _isHiding = false;
    //                    OnHided();
    //                }
    //            );
    //}
}
