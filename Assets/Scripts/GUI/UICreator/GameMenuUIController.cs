using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using TObject.Shared;
using System;
using System.Runtime.InteropServices;

public class GameMenuUIController : BaseUIController
{
    private Dictionary<GameData.PowerUpType, PowerUpButton> Powerups;
    public GameObject BtnResetSettings;
    public GameObject ButtonPlayEndless;
    public GameObject ButtonPlayEndlessBlocked;
    public GameObject ButtonTurnOffAds;

    // Use this for initialization
    void Start()
    {
        ReInit();
        GameManager.Instance.CurrentMenu = UISetType.MainMenu;
        EventData eventData = new EventData("OnUISwitchNeededEvent");
        eventData.Data["setid"] = UISetType.MainMenu;
        eventData.Data["force"] = true;
        GameManager.Instance.EventManager.CallOnUISwitchNeededEvent(eventData);
    }

    // Update is called once per frame
    void Update()
    {
        //GameManager.Instance.Player.UpdateTimePlayed();
        if (GameManager.Instance == null || GameManager.Instance.Settings == null)
        {
            Debug.Log("ERROR_A --------------------------------------------" + (GameManager.Instance == null).ToString() + "   " + (GameManager.Instance.Settings == null).ToString());
            return;
        }
        //
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.GameFlow.EscapeOnClick())
            {

            }
            else
            if (GameManager.Instance.CurrentMenu == UISetType.MainMenu)
            {
                //EventData eventData = new EventData("OnOpenFormNeededEvent");
                //eventData.Data["form"] = UIConsts.FORM_ID.CONFIRM_WINDOW;
                //eventData.Data["type"] = "confirm_quit";
                //eventData.Data["scene"] = UISetType.MainMenu;
                //GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
                GameManager.Instance.GameFlow.ExitGame();
            }
            else
            if (GameManager.Instance.CurrentMenu == UISetType.ClassicGame || GameManager.Instance.CurrentMenu == UISetType.LeveledGame)
            {
                if (!GameManager.Instance.BoardData.IsLoose() && !GameManager.Instance.BoardData.IsPause())
                {
                    GameManager.Instance.Game.GoHome();
                }
            }
        }
    }

    public void ButtonMenuLeveledOnClick()
    {
        //
        //GameManager.Instance.HideTutorial("");
        //GameManager.Instance.CurrentMenu = "game_menu";
        EventData eventData = new EventData("OnOpenFormNeededEvent");
        eventData.Data["form"] = UIConsts.FORM_ID.OPTIONS_WINDOW;
        eventData.Data["scene"] = "game_menu";
        GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
    }

    public void ButtonMenuOnClick()
    {
        //turn off powerups
        if (Powerups.ContainsKey(GameData.PowerUpType.DestroyColor))
        {
            Powerups[GameData.PowerUpType.DestroyColor].UpdatePowerup();
        }
        if (Powerups.ContainsKey(GameData.PowerUpType.Chain))
        {
            Powerups[GameData.PowerUpType.Chain].UpdatePowerup();
        }
        if (Powerups.ContainsKey(GameData.PowerUpType.Breake))
        {
            Powerups[GameData.PowerUpType.Breake].UpdatePowerup();
        }
        GameManager.Instance.BoardData.AGameBoard.BreakePowerup = false;
        GameManager.Instance.BoardData.AGameBoard.ChainPowerup = false;
        GameManager.Instance.BoardData.AGameBoard.DestroyColorPowerup = false;
        //
        //GameManager.Instance.HideTutorial("");
        //GameManager.Instance.CurrentMenu = "game_menu";
        EventData eventData = new EventData("OnOpenFormNeededEvent");
        eventData.Data["form"] = UIConsts.FORM_ID.OPTIONS_WINDOW;
        eventData.Data["scene"] = "game_menu";
        GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
    }

    public override void Show()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = UIConsts.STOP_POSITION;
        UpdatePlayEndlessButton();
        if (GameManager.Instance.Player.NoAds)
        {
            ButtonTurnOffAds.gameObject.SetActive(false);
        }
    }

    public void UpdatePlayEndlessButton()
    {
        //		test
        //		ButtonPlayEndless.SetActive(true);
        //		ButtonPlayEndlessBlocked.SetActive(false);
        bool unlocked = GameManager.Instance.Player.LevelsStates[Consts.UNLOCK_ENDLES_AFTER + 1].Unlocked;
        ButtonPlayEndless.SetActive(unlocked);
        ButtonPlayEndlessBlocked.SetActive(!unlocked);
    }

    public override void Hide()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = UIConsts.START_POSITION;
    }

    private Vector3 TransformPositionFromScreenToLocalUI(Vector3 startPos)
    {
        startPos -= transform.localPosition;
        return startPos;
    }

    public override void ReInit()
    {
        base.ReInit();
#if (PUBLISHING_PLATFORM_AMAZON)
        ButtonTurnOffAds.SetActive(false);
#endif
        ResetPowerupsAmount();
        Powerups = new Dictionary<GameData.PowerUpType, PowerUpButton>();
        for (int i = 0; i < 4; ++i)
        {
            Transform transf = transform.Find("BoosterButton_" + i.ToString());
            if (transf != null)
            {
                PowerUpButton script = transf.GetComponent<PowerUpButton>();
                Powerups.Add(script.PowerupType, script);
                //script.ResetPowerup();
            }
        }
        GameManager.Instance.BoardData.AGameBoard.PlayGame();
    }

    protected override void Awake()
    {
        base.Awake();
        EventManager.OnPowerUpUsedEvent += OnPowerUpUsed;
        EventManager.OnReachMaxPipeLevelEvent += OnReachMaxPipeLevel;
        EventManager.OnPowerUpsResetNeededEvent += OnPowerUpsResetNeeded;
        EventManager.OnPurchaseCompleteEvent += OnPurchaseCompleteEvent;  
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventManager.OnPowerUpUsedEvent -= OnPowerUpUsed;
        EventManager.OnReachMaxPipeLevelEvent -= OnReachMaxPipeLevel;
        EventManager.OnPowerUpsResetNeededEvent -= OnPowerUpsResetNeeded;
        EventManager.OnPurchaseCompleteEvent -= OnPurchaseCompleteEvent;  
    }

    protected override void OnDeactivate()
    {
        //transform.Find("ScoreDock").GetComponent<ScoreDock>().DeactivateTriggers();
    }

    protected override void OnActivate()
    {
        //transform.Find("ScoreDock").GetComponent<ScoreDock>().ActivateTriggers();
    }

    Vector3 localToFixed(Vector3 pos)
    {
        pos.x = ((UIConsts.DESIGN_RESOLUTION.x / 2 - pos.x) * -1);
        pos.y = ((UIConsts.DESIGN_RESOLUTION.y / 2 - pos.y));
        return pos;
    }

    public void DisableMenu()
    {
        BroadcastMessage("Disable");
        OnDeactivate();
    }

    public void EnableMenu()
    {
        BroadcastMessage("Enable");
        OnActivate();
    }


    ////////////////////// Feeding
    public void OnPowerUpUsed(EventData e)
    {
        GameData.PowerUpType pType = (GameData.PowerUpType)e.Data["type"];
        if (Powerups.ContainsKey(pType))
        {
            Powerups[pType].UpdatePowerup();
        }
    }

    public void OnPowerUpsResetNeeded(EventData e)
    {
        if (Powerups == null)
        {
            return;
        }
        if (!e.Data.ContainsKey("isStart"))
        {
            ResetPowerupsAmount();
        }
        foreach (var p in Powerups)
        {
            p.Value.ResetPowerup();
        }
    }

    public void OnReachMaxPipeLevel(EventData e)
    {
        if (Consts.MAX_COLORED_LEVEL_GIVES_POWERUP)
        {
            //add powerup
            GameManager.Instance.BoardData.PowerUps[Consts.MAX_COLORED_LEVEL_GIVES_POWERUP_TYPE] += 1;
            Powerups[Consts.MAX_COLORED_LEVEL_GIVES_POWERUP_TYPE].UpdatePowerup();
            Powerups[Consts.MAX_COLORED_LEVEL_GIVES_POWERUP_TYPE].PlayAddAnimation((float)e.Data["x"], (float)e.Data["y"]);
        }
    }

    public void BoosterButton_ReshuffleOnClick()
    {
        //GameManager.Instance.HideTutorial("");
        Powerups[GameData.PowerUpType.Reshuffle].SelectPowerup();
        GameManager.Instance.BoardData.AGameBoard.OnPowerUpClicked(GameData.PowerUpType.Reshuffle);
        //unselect other
        if (Powerups.ContainsKey(GameData.PowerUpType.Chain))
        {
            Powerups[GameData.PowerUpType.Chain].UpdatePowerup();
        }
        if (Powerups.ContainsKey(GameData.PowerUpType.Breake))
        {
            Powerups[GameData.PowerUpType.Breake].UpdatePowerup();
        }
    }

    public void BoosterButton_BreakeOnClick()
    {
        //GameManager.Instance.HideTutorial("");
        Powerups[GameData.PowerUpType.Breake].SelectPowerup();
        GameManager.Instance.BoardData.AGameBoard.OnPowerUpClicked(GameData.PowerUpType.Breake);
        //unselect other
        if (Powerups.ContainsKey(GameData.PowerUpType.Chain))
        {
            Powerups[GameData.PowerUpType.Chain].UpdatePowerup();
        }
        if (Powerups.ContainsKey(GameData.PowerUpType.DestroyColor))
        {
            Powerups[GameData.PowerUpType.DestroyColor].UpdatePowerup();
        }
        //TUTOR_3
        if (!GameManager.Instance.Player.IsTutorialShowed("3"))
        {
            GameManager.Instance.ShowTutorial("3", new Vector3(0, 0, 0));
        }
    }

    public void BoosterButton_ChainOnClick()
    {
        //GameManager.Instance.HideTutorial("");
        Powerups[GameData.PowerUpType.Chain].SelectPowerup();
        GameManager.Instance.BoardData.AGameBoard.OnPowerUpClicked(GameData.PowerUpType.Chain);
        //unselect other
        if (Powerups.ContainsKey(GameData.PowerUpType.Breake))
        {
            Powerups[GameData.PowerUpType.Breake].UpdatePowerup();
        }
        if (Powerups.ContainsKey(GameData.PowerUpType.DestroyColor))
        {
            Powerups[GameData.PowerUpType.DestroyColor].UpdatePowerup();
        }
    }

    public void BoosterButton_DestroyColorOnClick()
    {
        //GameManager.Instance.HideTutorial("");
        Powerups[GameData.PowerUpType.DestroyColor].SelectPowerup();
        GameManager.Instance.BoardData.AGameBoard.OnPowerUpClicked(GameData.PowerUpType.DestroyColor);
        //unselect other
        if (Powerups.ContainsKey(GameData.PowerUpType.Chain))
        {
            Powerups[GameData.PowerUpType.Chain].UpdatePowerup();
        }
        if (Powerups.ContainsKey(GameData.PowerUpType.Breake))
        {
            Powerups[GameData.PowerUpType.Breake].UpdatePowerup();
        }
        //TUTOR_4
        if (!GameManager.Instance.Player.IsTutorialShowed("4"))
        {
            GameManager.Instance.ShowTutorial("4", new Vector3(0, 0, 0));
        }
    }

    private void ResetPowerupsAmount()
    {
        GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Reshuffle] = Consts.POWERUPS_RESHUFFLE_AT_START;
        GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Breake] = Consts.POWERUPS_BREAKE_AT_START;
        GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Chain] = Consts.POWERUPS_CHAIN_AT_START;
        GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.DestroyColor] = Consts.POWERUPS_DESTROY_COLOR_AT_START;

    }

    /////////////// MAIN_MENU
    //#if UNITY_STANDALONE
    //IEnumerator LoadLogo() 
    //{
    //	string path = Application.streamingAssetsPath + "/logo.png";
    //	string url = "file://" + path;
    //	WWW www = new WWW(url);
    //	yield return www;
    //	Sprite aSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0)); // new Vector2(www.texture.width / 2, www.texture.height / 2));
    //	Image im = transform.Find("MGLogo").GetComponent<Image>();
    //	im.sprite = aSprite;
    //	im.SetNativeSize();
    //}
    //#else
    //void LoadLogo() 
    //{        
    //Sprite aSprite = Resources.Load<Sprite>("art/Splash/logo");
    //Image im = transform.Find("MGLogo").GetComponent<Image>();
    //im.sprite = aSprite;
    ////im.SetNativeSize();
    //}
    //#endif

    public void ButtonPlayEndlessOnClick()
    {
        //MusicManager.PlayGameTracks();

        //GameManager.Instance.BoardData.AGameBoard.RestartGame(); no need for restart course this clears saved game!
        GameManager.Instance.CurrentMenu = UISetType.ClassicGame;
        EventData eventData = new EventData("OnUISwitchNeededEvent");
        eventData.Data["setid"] = UISetType.ClassicGame;
        GameManager.Instance.EventManager.CallOnUISwitchNeededEvent(eventData);

        GameBoard.AddingType = EAddingType.OnNoMatch;
        if (GameBoard.GameType != EGameType.Classic)
        {
            GameManager.Instance.Game.ClearBoardForce();
            GameManager.Instance.Game.PlayGame();
        }
        else
        {
            GameManager.Instance.Settings.User.SavedGame = null;
        }

        ZAnalitycs.StartEndlessLevelEvent();
    }

    public void ButtonPlayEndlessBlockedOnClick()
    {
        //TUTOR_1
        if (GameManager.Instance.Player.IsTutorialShowed("1"))
        {
            GameManager.Instance.Player.TutorialsShowed.Remove("1");
        }
        GameManager.Instance.ShowTutorial("1", new Vector3(0, 0, 0));
    }

    public void ButtonPlayLevelsOnClick()
    {
        EventData eventData = new EventData("OnOpenFormNeededEvent");
        eventData.Data["form"] = UIConsts.FORM_ID.LEVEL_SELECTION_WINDOW;
        eventData.Data["scene"] = UISetType.MainMenu;
        GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
    }

    //	public void ButtonPlayInverseOnClick()
    //	{
    //		MusicManager.PlayGameTracks();
    //		GameBoard.GameType = EGameType.Inverse;
    //		GameManager.Instance.GameFlow.TransitToScene(UIConsts.SCENE_ID.GAME_SCENE);
    //	}

    public void ButtonOptionsOnClick()
    {
        EventData eventData = new EventData("OnOpenFormNeededEvent");
        eventData.Data["form"] = UIConsts.FORM_ID.OPTIONS_WINDOW;
        eventData.Data["scene"] = "main_menu";
        GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
    }

    public void ButtonOptionsWideOnClick()
    {
        EventData eventData = new EventData("OnOpenFormNeededEvent");
        eventData.Data["form"] = UIConsts.FORM_ID.OPTIONS_WINDOW;
        eventData.Data["scene"] = "main_menu";
        GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
    }

    public void ButtonExitOnClick()
    {
        Debug.LogError("TODO");
        //EventData eventData = new EventData("OnOpenFormNeededEvent");
        //eventData.Data["form"] = UIConsts.FORM_ID.EXIT_GAME_WINDOW;
        //eventData.Data["scene"] = "main_menu";
        //GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
    }

    public void ButtonResetSettingsOnClick()
    {
        GameManager.Instance.Settings.ResetSettings();
        UpdatePlayEndlessButton();
    }

    public void ButtonTrophiesOnClick()
    {
        //EventData eventData = new EventData("OnOpenFormNeededEvent");
        //eventData.Data["form"] = UIConsts.FORM_ID.TROPHIES_WINDOW;
        //eventData.Data["scene"] = "main_menu";
        //GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
    }

    public void ButtonFacebookOnClick()
    {
        //TODO
    }

    public void ButtonTurnOffAddsOnClick()
    {
        GameManager.Instance.IAPManager.InitiatePurchase(GameManager.Instance.IAPManager.GetFullProductName("removeads"));
    }

    private void OnPurchaseCompleteEvent(EventData e)
    {
        ButtonTurnOffAds.SetActive(false);
    } 

    public void ButtonRateGameOnClick()
    {
        #if !UNITY_EDITOR && UNITY_IOS
        if (!requestReview())
        {
            Application.OpenURL("itms-apps://itunes.apple.com/app/id1364640811");
        }
#elif !UNITY_EDITOR && UNITY_ANDROID
        #if (PUBLISHING_PLATFORM_AMAZON)
            Application.OpenURL("amzn://apps/android?p=com.brainblendergames.coverthedice");
        #endif

        #if (PUBLISHING_PLATFORM_GOOGLE_PLAY)
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.brainblendergames.coverthedice");
        #endif
#endif
    }   

#if !UNITY_EDITOR && UNITY_IOS
        [DllImport ("__Internal")] private static extern bool requestReview();
#endif
}
