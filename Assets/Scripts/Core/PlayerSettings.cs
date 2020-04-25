
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public struct LevelState
{
	public int 				BestMoves;
	public bool 			Unlocked;
	public int              Stars;
	//public LevelData        SavedGame;
}

[System.Serializable]
public struct SkinData
{
	public int								Id;
	public string                           Name;
	public string                           BackPrefix;
	public string                           FrontPrefix;
	public EPipeStructureType               PipeStructureType;
}

public enum ResourceType {
    Gold = 0
}

[System.Serializable]
public class UserData
{
	[System.Serializable]
	public class DictPowerUpsStates: SerializableDictionary<GameData.PowerUpType, PowerUpState> {}
	[System.Serializable]
	public class DictTrophiesItems: SerializableDictionary<ETrophyType, TrophyCompleteData> {}

    //[NonSerialized()]
	public string							Name;
	public DictPowerUpsStates               PowerUpsState;
	public DictTrophiesItems			    TrophiesItems;
    //
    public string                           LastGameTrack;

	// time played
	public float 							TimePlayed;

    public bool                             NoAds;
    public bool                             Bump;
    public bool                             Shake;
    public int                              Gold;
    public bool                             Tutorials;
    public List<string>                     TutorialsShowed;
    public float                            MusicVolume;
    public float                            SoundVolume;
    public LevelData                        SavedGame;
    public List<Vector2>                    SlotsDoubles;

	public SkinData							CurrentSkin;

	public long								BestScore;

	// levels
	public List<LevelState>					LevelsStates;
	public int								CurrentLevel;
    public int                              RestartCounter;
	//

    public UserData()
    {

    } 

	public void SetDefaults()
	{
        PowerUpsState = new DictPowerUpsStates();

        RestartCounter = 0;
        LastGameTrack = "";

		// trophies
		TrophiesItems = new DictTrophiesItems();
//		foreach (var td in GameManager.Instance.GameData.XMLtrophiesData)
//		{
//			TrophyCompleteData trophy = new TrophyCompleteData();
//			trophy.Param = 0;
//			trophy.Completed = false;
//			TrophiesItems.Add(td.Key, trophy);
//		}

        foreach (GameData.PowerUpType powerUp in Enum.GetValues(typeof(GameData.PowerUpType)))
        {
            //TODO real data
            PowerUpState pw = new PowerUpState();
            pw.AmountPerLevel = 10;
            pw.Level = 1;
            PowerUpsState.Add(powerUp, pw);
        }
        // options
        TutorialsShowed = new List<string>();
		// time played
		TimePlayed = 0;
        NoAds = false;
        Bump = true;
        Shake = true;
        Gold = Consts.START_GOLD_COUNT;
        Tutorials = true;
        MusicVolume = Consts.MUSIC_VOLUME_MAX;
		SoundVolume = Consts.SOUND_VOLUME_MAX;
        TutorialsShowed = new List<string>();   
		SavedGame = null;
        SlotsDoubles = new List<Vector2>();
        //SlotsDoubles.Add(new Vector2(2, 2)); // center
        // skins
//		CurrentSkin.Id = 0;
//		CurrentSkin.Name = "default";
//		CurrentSkin.BackPrefix = "default_";
//		CurrentSkin.FrontPrefix = "";
//		CurrentSkin.PipeStructureType = EPipeStructureType.Solid;
		CurrentSkin.Id = 0;
		CurrentSkin.Name = "default";
		CurrentSkin.BackPrefix = "Color_";
		CurrentSkin.FrontPrefix = "Symbol_";
		CurrentSkin.PipeStructureType = EPipeStructureType.BackFront;
		//
		BestScore = 0;
		// levels states
		CurrentLevel = 0;
		LevelsStates = new List<LevelState>();
		LevelState firstLevel;
		firstLevel.BestMoves = -1;
		firstLevel.Unlocked = true;
		//firstLevel.SavedGame = null;
		firstLevel.Stars = 0;
		LevelsStates.Add(firstLevel);
		for (int i = 0; i < Consts.LEVELS_COUNT; ++i)
		{
			LevelState levelState;
			levelState.BestMoves = -1;
			levelState.Unlocked = false;
			//levelState.SavedGame = null;
			levelState.Stars = 0;
			LevelsStates.Add(levelState);
		}
		//
    }

	public bool IsTutorialShowed(string id)
	{
		return TutorialsShowed.Contains(id);
	}

	public void SetTutorialShowed(string id)
	{
		if (!IsTutorialShowed(id))
		{
			TutorialsShowed.Add(id);
		}
	}

    /// <summary>
    /// On win level
    /// </summary>
    //public void OnWinLevel(string id, long points)
    //{
    //       //TROPHY CompleteXLevels2, CompleteXLevels1, CompleteXLevels0
    //       if (new buv zaversheniy ranishe)
    //       {
    //           //TROPHY CompleteXLevels2
    //           if (!GameManager.Instance.Player.TrophiesItems[ETrophyType.CompleteXLevels2].Completed)
    //           {
    //               GameManager.Instance.Player.OnTrophyEvent(ETrophyType.CompleteXLevels2, 1, true);
    //               //TROPHY CompleteXLevels1
    //               if (!GameManager.Instance.Player.TrophiesItems[ETrophyType.CompleteXLevels1].Completed)
    //               {
    //                   GameManager.Instance.Player.OnTrophyEvent(ETrophyType.CompleteXLevels1, 1, true);
    //                   //TROPHY CompleteXLevels0
    //                   if (!GameManager.Instance.Player.TrophiesItems[ETrophyType.CompleteXLevels0].Completed)
    //                   {
    //                       GameManager.Instance.Player.OnTrophyEvent(ETrophyType.CompleteXLevels0, 1, true);
    //                   }
    //               }
    //           }
    //       }
    //       //
    //       ClearSavedGame();	
    //}	

    public int GetGoldCount()
	{
		return Gold;
	}

    //public void AddGold(int amount)
    //{
    //    Gold += amount;
    //    EventData eventData = new EventData("OnGoldCountChangedEvent");
    //    GameManager.Instance.EventManager.CallOnGoldCountChangedEvent(eventData);
    //    //TROPHY EarnXGoldCumulatively
    //    GameManager.Instance.Player.OnTrophyEvent(ETrophyType.EarnXGoldCumulatively, amount);
    //    //TROPHY HaveXGoldInTheBank
    //    int val = GameManager.Instance.Player.TrophiesItems[ETrophyType.HaveXGoldInTheBank].Param;
    //    GameManager.Instance.Player.OnTrophyEvent(ETrophyType.HaveXGoldInTheBank, val, false);
    //    //
    //}

	//public void RemoveGold(int amount)
	//{
	//	Gold -= amount;
	//	EventData eventData = new EventData("OnGoldCountChangedEvent");			
	//	GameManager.Instance.EventManager.CallOnGoldCountChangedEvent(eventData);
	//}

	////Trophies
	public void OnTrophyEvent(ETrophyType tType, int param, bool commulative = true)
	{
        if (TrophiesItems[tType].Completed)
		{
			return;
		}
		int newParam = 0;
		if (commulative)
		{
			newParam = TrophiesItems[tType].Param + param;
		} else
		{
			newParam = param;
		}

		TrophyData tData = GameManager.Instance.GameData.XMLtrophiesData[tType];
		if (newParam > tData.Param)
		{
			newParam = tData.Param;
		}
		TrophiesItems[tType].Param = newParam;
		if (newParam == tData.Param)
		{
			// completed
			Debug.Log("Trophy " + tType.ToString() + " completed!");
			TrophiesItems[tType].Completed = true;
			EventData eventData = new EventData("OnTrophyCompletedEvent");
			eventData.Data["type"] = tType;
			GameManager.Instance.EventManager.CallOnTrophyCompletedEvent(eventData);

			//TROPHY GetAllOtherTrophies    -    check if final trophy completed
			int completedAmount = 0;
			foreach (var trophy in TrophiesItems)
			{
				if (trophy.Value.Completed)
				{
					++completedAmount;
				}
			}
			if (completedAmount == TrophiesItems.Count - 1)
			{
				// completed all trophies!
				Debug.Log("All trophies " + tType.ToString() + " completed!");
				TrophiesItems[ETrophyType.GetAllOtherTrophies].Param = 1;
				TrophiesItems[ETrophyType.GetAllOtherTrophies].Completed = true;
				EventData eventData2 = new EventData("OnTrophyCompletedEvent");
				eventData2.Data["type"] = ETrophyType.GetAllOtherTrophies;
				GameManager.Instance.EventManager.CallOnTrophyCompletedEvent(eventData2);
			}
		}
	}
	//

	//public void UpdateTimePlayed()
	//{
	//	TimePlayed += Time.deltaTime;
	//	//TROPHY PlayMoreThan5hoursCumulative
	//	if (!GameManager.Instance.Player.TrophiesItems[ETrophyType.PlayMoreThan5hoursCumulative].Completed)
	//	{
	//		int seconds = (int)(TimePlayed);
	//		GameManager.Instance.Player.OnTrophyEvent(ETrophyType.PlayMoreThan5hoursCumulative, seconds, false);
	//	}
	//	//
	//}
	
	
	
	
	
	
	
	
	
	
	
	
	
	public void ClearSavedGame()
	{
		SavedGame = null;
	}

	public void SaveLastGame(LevelData data)
	{
		SavedGame = data;
        GameManager.Instance.Settings.Save();
	}
	
	public void AddGold(int gold)
	{
		Gold += gold;
		EventData eventData = new EventData("OnCoinsCountChangedEvent");
		//eventData.Data["lives"] = Lives;
		GameManager.Instance.EventManager.CallOnGoldCountChangedEvent(eventData);
	}

    public void AddGoldWithoutMessage(int amount)
    {
        Gold += amount;
    }

    public void RemoveGold(int gold)
	{
		Gold -= gold;
		if (Gold < 0)
		{
			Gold = 0;
			Debug.LogError("GoldCoins < 0 !!!");
		}
		EventData eventData = new EventData("OnCoinsCountChangedEvent");
		//eventData.Data["lives"] = Lives;
		GameManager.Instance.EventManager.CallOnGoldCountChangedEvent(eventData);
	}
	
}

/// <summary>
/// Class used to save/load player data into Unity's PlayerPrefs
/// </summary>
/// 
/*[System.Serializable]*/
public sealed class ZPlayerSettings
{
    public static int SettingsVersion = 0;
	public UserData User;		

    public ZPlayerSettings()
    {
        User = null;
    }

    /// <summary>
    /// Loads user data from Unity's PlayerPrefs
    /// </summary>
    public void Load()
    {
        //FINAL Debug.Log("SETTINGS: Start Loading");
        int savedVersion = PlayerPrefs.GetInt("SettingsVersion", -1);

        if (PlayerPrefs.HasKey(Consts.PROJECT_NAME) && savedVersion == Consts.APP_VERSION)
        {
            //FINAL Debug.Log("\tSETTINGS: Loading from local device...");
            string rawStringBinaryData = PlayerPrefs.GetString(Consts.PROJECT_NAME);
            StandaloneSavedData standaloneSavedData = null;
            try
            {
                string decompressedJSON = GZipHelper.Decompress(rawStringBinaryData);
                standaloneSavedData = JsonUtility.FromJson<StandaloneSavedData>(decompressedJSON);
            }
            catch (Exception ex)
            {
                //FINAL Debug.Log("SETTING: Error parsing local saved settings object.\n" + ex.Message);
            }
            LoadSettingsFromSavedData(standaloneSavedData);
        }
        else
        {
            //FINAL Debug.Log("SETTINGS: No saved settings found. Creating new settings.");
            ResetSettings();            
            Save();
            Load();            
        }        
    }

    public void LoadSettingsFromSavedData(StandaloneSavedData sData)
    {
        if (sData == null)
        {
            // new game data
            ResetSettings();
            //FINAL Debug.Log("SETTINGS: Error loading StandaloneSavedData, creating new settings.");
        }
        else
        {
            if (sData.SettingsVersion != Consts.APP_VERSION)
            {
                ResetSettings();
            }
            else
            {
                if (sData.SettingsVersion > Consts.APP_VERSION)
                {
                    Debug.LogError("New version avialable. Please update the application.");
                    //TODO: Application.Quit();
                }
                while (sData.SettingsVersion < Consts.APP_VERSION) // update to actual version
                {
                    UpgradeSavedData(ref sData);
                }
                User = sData.User;
                ApplyUser();
                //FINAL Debug.Log("SETTINGS: Loaded successfully.");
            }
        }       
    }

    void UpgradeSavedData(ref StandaloneSavedData data) {
        //TODO: Зробити апгрейд данних, якщо треба
        //if (data.SettingsVersion < 82)
        //{
        //    Debug.Log("XX->82");
        //    data.SettingsVersion = 82;
        //}
    }

    /// <summary>
    /// Saves user data to Unity's PlayerPrefs
    /// </summary>
	public void Save(bool isExitSave = false)
    {   	
		StandaloneSavedData sData = new StandaloneSavedData();
		sData.User = User;
        sData.SettingsVersion = Consts.APP_VERSION;
        string json = GZipHelper.Compress(JsonUtility.ToJson (sData));
		PlayerPrefs.SetString(Consts.PROJECT_NAME, json);
        //FINAL Debug.Log("SETTINGS: Saving to local device.");       
        PlayerPrefs.SetInt("SettingsVersion", Consts.APP_VERSION); //SettingsVersion); 
		PlayerPrefs.Save();
    }

    /// <summary>
    /// Clear all the settings and set them to default values
    /// </summary>
    public void ResetSettings()
    {     
        PlayerPrefs.DeleteAll();
        User = null;
		AddUser("default");
		ApplyUser();
    }    
    
	public void AddUser(string name)
	{
		User = new UserData();
        User.Name = name;
        User.SetDefaults();
	}

	public void RemoveUser(string name)
	{
        User = null;
	}

	public void ApplyUser()
	{
		SetMusicVolume(User.MusicVolume);
		SetSoundVolume(User.SoundVolume);
    }

	public void SetMusicVolume(float fVolume)
	{
		User.MusicVolume = fVolume;
		MusicManager.setMusicVolume(fVolume);
	}
	
	public void SetSoundVolume(float fVolume)
	{
		User.SoundVolume = fVolume;
		MusicManager.setSoundVolume(fVolume);
	}

	public void SetTutorials(bool isTutorials)
	{
		User.Tutorials = isTutorials;
	}

}