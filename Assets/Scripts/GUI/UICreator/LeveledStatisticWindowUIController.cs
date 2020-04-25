using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class LeveledStatisticWindowUIController : BaseUIController
{
	public List<GameObject>         Stars;
	public CanvasGroup				ACanvasGroup;
    public UIButton					HomeButton;
    public UIButton					RestartButton;
    public UIButton 				ButtonNext;
	public Text						LevelText;
    public GameObject               StarEffectPrefab;

    public override bool OpenForm(EventData e)
	{
        //TryRescale();
        HideButtons();
        float canvasTime = 0.5f;
		ACanvasGroup.alpha = 0;
		LeanTween.value(ACanvasGroup.gameObject, 0.0f, 1.0f, canvasTime)
			.setOnUpdate((float val)=>{
				ACanvasGroup.alpha = val;
			});

		float atime = 0.3f;
		LevelText.text = Localer.GetText("Level") + " " + (GameManager.Instance.Player.CurrentLevel + 1).ToString();
		int starsAmount = (int)GameManager.Instance.BoardData.StarsGained;
		int moves = GameManager.Instance.BoardData._allTurns;

		for (int i = 0; i < Stars.Count; ++i)
		{
			if (i < starsAmount)
			{
				Stars[i].SetActive(true);
				Image img = Stars[i].GetComponent<Image>();
				img.color = new Color(1, 1, 1, 0);
                int starNumber = i;
                LeanTween.delayedCall(canvasTime + i * atime, ()=> { PlayStarSound(starNumber); });
                LeanTween.value(img.gameObject, img.color, Color.white, atime)
					.setOnUpdate((Color val)=>{
						img.color = val;
					})
					.setDelay(canvasTime + i * atime);
			} else
			{
				Stars[i].SetActive(false);
			}
		}

        LeanTween.delayedCall(canvasTime + starsAmount * atime, ShowButtons);

		//Invoke("ShowButtons", canvasTime + starsAmount * atime);

		int currentLevel = GameManager.Instance.Player.CurrentLevel;
		LevelState levelState = GameManager.Instance.Player.LevelsStates[currentLevel];
		int prevBest = levelState.BestMoves;
		bool firstTimeComplete = prevBest < 0;
		if (firstTimeComplete)
		{
			levelState.BestMoves = moves;
		} else
		{
			levelState.BestMoves = Mathf.Min(moves, levelState.BestMoves);
		}
		levelState.Stars = starsAmount;
		//
		GameManager.Instance.Player.LevelsStates[currentLevel] = levelState;

		if (currentLevel == Consts.LEVELS_COUNT - 1)
		{
			// it was last level
			if (firstTimeComplete)
			{
				//TODO Message that you completed all levels!!!!
			}
		} else
		{
			// unlock next level
			int nextLevel = currentLevel + 1;
			LevelState nextLevelState = GameManager.Instance.Player.LevelsStates[nextLevel];
			nextLevelState.Unlocked = true;
			GameManager.Instance.Player.LevelsStates[nextLevel] = nextLevelState;
            GameManager.Instance.Settings.Save();
		}
        ZAnalitycs.FinishLevelEvent(currentLevel);
        return true;
	}

    public override bool EscapeOnClick()
    {
        if (_isHiding)
        {
            return true;
        }
        Hide();
        GameManager.Instance.Game.GoHome();
        return true;
    }

    private void PlayStarSound(int starNumber)
	{
		MusicManager.playSound("star_completed");
        CreateStarEffect(starNumber);
    }

	private void ShowButtons()
	{
		RestartButton.Enable();
		HomeButton.Enable();
		bool lastLevel = GameManager.Instance.Player.CurrentLevel >= Consts.LEVELS_COUNT - 1;
		if (!lastLevel)
		{
			ButtonNext.Enable();
		}
	}

	private void HideButtons()
	{
        RestartButton.Disable();
        HomeButton.Disable();
        ButtonNext.Disable();
        bool lastLevel = GameManager.Instance.Player.CurrentLevel >= Consts.LEVELS_COUNT - 1;
        ButtonNext.gameObject.SetActive(!lastLevel);
    }

    public void ButtonHomeOnClick()
    {
        if (_isHiding)
        {
            return;
        }
        Hide();
        GameManager.Instance.Game.GoHome();
    }

    public void ButtonRestartOnClick()
    {
        if (_isHiding)
        {
            return;
        }
        GameManager.Instance.Player.RestartCounter++;

        if (GameManager.Instance.Player.RestartCounter == 3)
        {
            GameManager.Instance.Player.RestartCounter = 0;
            EventManager.OnInterstitialClosedEvent += OnRestartAfterInterstitialClosed;
            GameManager.Instance.Ads.ShowInterstitial();
        }
        else
        {
            Hide();
            GameManager.Instance.Game.RestartGame();
            ZAnalitycs.RestartLevelEvent();
        }
    }

    public void OnRestartAfterInterstitialClosed(EventData e)
    {
        EventManager.OnInterstitialClosedEvent -= OnRestartAfterInterstitialClosed;
        Hide();
        GameManager.Instance.Game.RestartGame();
        ZAnalitycs.RestartLevelEvent();
    }

    public void OnNextAfterInterstitialClosed(EventData e)
    {
        EventManager.OnInterstitialClosedEvent -= OnNextAfterInterstitialClosed;
        GameManager.Instance.Game.PlayLeveledGame();
    }

	public void ButtonNextOnClick()
	{
        if (_isHiding)
        {
            return;
        }
        Hide();
		int nextLevel = GameManager.Instance.Player.CurrentLevel + 1;
		if (nextLevel < GameManager.Instance.Player.LevelsStates.Count)
		{
			GameManager.Instance.Player.CurrentLevel = nextLevel;
		}

        if (nextLevel % 2 == 0)
        {
            EventManager.OnInterstitialClosedEvent += OnNextAfterInterstitialClosed;
            GameManager.Instance.Ads.ShowInterstitial();
        }
        else
        {
            GameManager.Instance.Game.PlayLeveledGame();
        }
	}

	protected override void Awake()
	{
		if (CreationMethod != EFormCreationMethod.Dynamic)
		{
			gameObject.SetActive(false);
		}
	}

    void Start()
    {
        ReInit();
        //gameObject.SetActive(false);
    }

	public override void Show ()
	{
		//_isHiding = true;
		//Active = false;
		Reset();
		myRect.anchoredPosition3D = UIConsts.STOP_POSITION;
		_isHiding = false;
		Active = true;
		OnShowed();
	}

    private void CreateStarEffect(int number)
    {
        GameObject effect = GameObject.Instantiate(StarEffectPrefab, Vector3.zero, Quaternion.identity, Stars[number].transform) as GameObject;
        effect.transform.localPosition = Vector3.zero;
        GameObject.Destroy(effect, 5.0f);
    }
}
