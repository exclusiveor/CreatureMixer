using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    // guarantee this will be always a singleton only - can't use the constructor!
    protected GameManager() {

    }

    public ZagravaAdsManager Ads;
    public ZIAPManager IAPManager;
    public ZPlayerSettings Settings;
    public AtlasFramesCache AtlasFramesCache;
    public GameData GameData;
    public GameFlow GameFlow;
	public EventManager EventManager;
	public bool allowQuit;
	public UISetType CurrentMenu;
	
	//Feeding
	public GameBoard Game;
	public GameBoardData BoardData;

public UserData Player
    {
        get
        {
            return Settings.User;
        }
    }
    
    public void Initialize()
    {
        Input.multiTouchEnabled = false;
        Localer.Init();
		CreatePauseListener();
        GameData = new GameData();        
        Settings = new ZPlayerSettings();
        AtlasFramesCache = new AtlasFramesCache();
        GameFlow = new GameFlow();
		EventManager = new EventManager();
		CurrentMenu = UISetType.Global;
		// Feeding
		BoardData = new GameBoardData();
        IAPManager = new ZIAPManager();

        ZAnalitycs.Initialize();
    }

    private void CreatePauseListener()
	{

	}

    new public void OnDestroy()
    {        
        base.OnDestroy();
    }

    public void Load()
    {        
        // first load all constant data from XML's
        GameData.Load();
        // next load writable user data from PlayerPrefs
        Settings.Load();
		
#if (PUBLISHING_PLATFORM_AMAZON)
        Settings.User.NoAds = true;
#endif
        Ads = gameObject.AddComponent<ZagravaAdsManager>();
		Ads.InitAds();

#if (PUBLISHING_PLATFORM_GOOGLE_PLAY || PUBLISHING_PLATFORM_APPSTORE)
        gameObject.AddComponent<Purchaser>();
#endif
    }


    public void TryApplicationQuit()
    {
		//if (Dont hide) 
        //{
		//	Application.CancelQuit();
		//	EventData eventData = new EventData("OnOpenFormNeededEvent");
		//	eventData.Data["form"] = UIConsts.FORM_ID.CONFIRM_WINDOW;
		//	eventData.Data["type"] = "confirm_exitreminder";
		//	eventData.Data["parameter"] = "";
		//	GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
		//} else 
        {
			Instance.GameFlow.ExitGame();
		}
	}

	void OnApplicationQuit()
    {
		TryApplicationQuit();
	}

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (GameManager.Instance.BoardData != null &&
                GameManager.Instance.BoardData.AGameBoard != null)
            {
                GameManager.Instance.BoardData.AGameBoard.SaveGame();
            }
            GameManager.Instance.Settings.Save(true);
        }
    }

    public void HideTutorial(string id = "")
	{
		if (GameManager.Instance.GameFlow.IsSomeWindow() && GameFlow.GetCurrentActiveWindowId() == UIConsts.FORM_ID.TUTOR_WINDOW)
		{
			EventData eventData = new EventData("OnOpenFormNeededEvent");
			eventData.Data["form"] = UIConsts.FORM_ID.TUTOR_WINDOW;
			eventData.Data["id"] = id;
			eventData.Data["hide"] = true;
			GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
		}
	}

	public void ShowTutorial(string id, Vector3 pos, bool exclusive = false)
	{
		if (GameFlow.IsSomeWindow() && GameFlow.GetCurrentActiveWindowId() != UIConsts.FORM_ID.TUTOR_WINDOW)
		{
			// cant show tutorial (some window is opened)
			// TODO try open after delay? no!
			return;
		}

		// variant for new Tutors
		EventData eventData = new EventData("OnOpenFormNeededEvent");
		eventData.Data["form"] = UIConsts.FORM_ID.TUTOR_WINDOW;
		eventData.Data["id"] = id;
		eventData.Data["toqueue"] = GameManager.Instance.GameFlow.IsSomeWindow();
		eventData.Data["exclusive"] = exclusive;
		eventData.Data["pos"] = pos;
		GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
	}

	public void ShowTutorialOnTopOfWindow(string id)
	{
		if (GameManager.Instance.GameFlow.GetCurrentActiveWindowId() != UIConsts.FORM_ID.TUTOR_WINDOW)
		{
			GameManager.Instance.GameFlow.RemoveCurrentWindow();
		}
		
		// variant for new Tutors
		EventData eventData = new EventData("OnOpenFormNeededEvent");
		eventData.Data["form"] = UIConsts.FORM_ID.TUTOR_WINDOW;
		eventData.Data["id"] = id;
		eventData.Data["toqueue"] = GameManager.Instance.GameFlow.IsSomeWindow();
		eventData.Data["exclusive"] = true;
		GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
	}

	public void ShowTutorialDelayed(string id, float delay, Vector3 pos, bool exclusive = false)
	{
		LeanTween.delayedCall(gameObject, delay, ()=>{ ShowTutorial(id, pos, exclusive); });
	}

	public void TryShowTutorFromQueue()
	{
		ShowTutorial("", Vector3.zero);
	}

	//void OnApplicationFocus(bool focusStatus) 
	//{
	//	#if UNITY_STANDALONE
	//	if (GameManager.Instance.Player.Fullscreen)
	//	{
	//		AudioListener.pause = !focusStatus;
	//	} else
	//	{
	//		AudioListener.pause = false;
	//	}
	//	#else
		
	//	#endif
		
	//}

//	void OnGUI()
//	{
//		GUI.DrawTexture (new Rect(Event.current.mousePosition.x-64, Event.current.mousePosition.y-64, 128, 128), GameManager.Instance.Settings.AvailableCursors["custom_cursor_0"].Texture);
//	}

}