using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

public class MainMenuUIController : BaseUIController {

    private GameObject _btnResetSettings;
    private int _prevTouchCount;

    protected override void Awake()
	{
		base.Awake();
        Input.multiTouchEnabled = true;
    }
	
	protected override void OnDestroy()
	{
		base.OnDestroy();
        Input.multiTouchEnabled = false;
    }

	// Use this for initialization
	void Start ()
	{
		ReInit();		
		////TROPHY ViewEntireCredits
		//if (!GameManager.Instance.Player.TrophiesItems[ETrophyType.ViewEntireCredits].Completed && GameManager.Instance.Player.TrophiesItems[ETrophyType.ViewEntireCredits].Param == 1)
		//{
		//	GameManager.Instance.Player.OnTrophyEvent(ETrophyType.ViewEntireCredits, 0);
		//}
		////
	}
		
	#if UNITY_STANDALONE
	IEnumerator LoadLogo() 
	{
		string path = Application.streamingAssetsPath + "/logo.png";
		string url = "file://" + path;
		WWW www = new WWW(url);
		yield return www;
		Sprite aSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0)); // new Vector2(www.texture.width / 2, www.texture.height / 2));
		Image im = transform.Find("MGLogo").GetComponent<Image>();
		im.sprite = aSprite;
		im.SetNativeSize();
	}
	#else
	void LoadLogo() 
	{        
	Sprite aSprite = Resources.Load<Sprite>("art/Splash/logo");
	Image im = transform.Find("MGLogo").GetComponent<Image>();
	im.sprite = aSprite;
	//im.SetNativeSize();
	}
	#endif

    // Update is called once per frame
    void Update ()
	{
		if (GameManager.Instance == null || GameManager.Instance.Settings == null)
		{
			Debug.Log ("ERROR_A --------------------------------------------" + (GameManager.Instance == null).ToString()  + "   " + (GameManager.Instance.Settings == null).ToString());
			return;
		}
		//GameManager.Instance.Player.UpdateTimePlayed();

        //if ((Input.touchCount == 3 && _prevTouchCount != Input.touchCount) || Input.GetMouseButtonDown(1))
        //{
        //    GameObject showPrefab = Resources.Load<GameObject>("Prefabs/UI/main_menu/CheatEnabledPrefab");
        //    GameObject showEffect = Instantiate(showPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        //    showEffect.transform.SetParent(transform, false);
        //    Cheats.IsCheatEnabled = !Cheats.IsCheatEnabled;
        //    _btnResetSettings.SetActive(Cheats.IsCheatEnabled);
        //}
        _prevTouchCount = Input.touchCount;
    }

    public void ButtonPlayOnClick ()
	{
        //MusicManager.PlayGameTracks();
        GameBoard.GameType = EGameType.Classic;
        GameBoard.AddingType = EAddingType.OnNoMatch;
        GameManager.Instance.GameFlow.TransitToScene(UIConsts.SCENE_ID.GAME_SCENE);        
	}

    public void ButtonOptionsOnClick ()
	{	
		EventData eventData = new EventData("OnOpenFormNeededEvent");
		eventData.Data["form"] = UIConsts.FORM_ID.OPTIONS_WINDOW;
		eventData.Data["scene"] = "main_menu";
		GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
	}
	
	public void ButtonExitOnClick ()
	{
        Debug.LogError("TODO");
		//EventData eventData = new EventData("OnOpenFormNeededEvent");
		//eventData.Data["form"] = UIConsts.FORM_ID.EXIT_GAME_WINDOW;
		////eventData.Data["scene"] = "mainmenu";
		//GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
	}

	public void ButtonResetSettingsOnClick ()
	{
        GameManager.Instance.Settings.ResetSettings();
	}

	public void ButtonTrophiesOnClick ()
	{
		EventData eventData = new EventData("OnOpenFormNeededEvent");
		eventData.Data["form"] = UIConsts.FORM_ID.TROPHIES_WINDOW;
		//eventData.Data["scene"] = "mainmenu";
		GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
	}

	public override void Show ()
	{
		gameObject.GetComponent<RectTransform>().anchoredPosition3D = UIConsts.STOP_POSITION;
	}
	
	public override void Hide ()
	{
		gameObject.GetComponent<RectTransform>().anchoredPosition3D = UIConsts.START_POSITION;
	}
	
	public override void ReInit()
	{
		base.ReInit();
        _btnResetSettings = HelperFunctions.GetChildGameObject(gameObject, "ButtonResetSettings");

#if (!UNITY_EDITOR)		
		_btnResetSettings.SetActive(false);      
#endif
    }

}
