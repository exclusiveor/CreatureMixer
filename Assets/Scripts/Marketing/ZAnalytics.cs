using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Analytics;
using GameAnalyticsSDK;

public class ZAnalitycs {

    public static void Initialize()
    {
        GameAnalytics.Initialize();
    }

    public static void StartLevelEvent(int level)
    {
        //Dictionary<string, object> data = new Dictionary<string, object>();
        //data["level_number"] = GameManager.Instance.Player.CurrentLevel;
        //Analytics.CustomEvent("level_started", data);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level_" + level.ToString("D3"));
    }

    public static void FinishLevelEvent(int level)
    {
        //Dictionary<string, object> data = new Dictionary<string, object>();
        //int currentLevel = GameManager.Instance.Player.CurrentLevel;
        //if (currentLevel < GameManager.Instance.Player.LevelsStates.Count)
        //{
        //    LevelState levelState = GameManager.Instance.Player.LevelsStates[currentLevel];
        //    //    data["level_stars"] = levelState.Stars;
        //    //    data["level_bestmoves"] = levelState.BestMoves;
        //    //}

        //    //data["level_number"] = GameManager.Instance.Player.CurrentLevel;
        //    //Analytics.CustomEvent("level_finished", data);
        //    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", levelState.Stars);
        //}
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level_" + level.ToString("D3"));
    }

    public static void RestartLevelEvent()
    {
        //Dictionary<string, object> data = new Dictionary<string, object>();
        //data["level_number"] = GameManager.Instance.Player.CurrentLevel;
        //Analytics.CustomEvent("level_restarted", data);
    }

    public static void StartEndlessLevelEvent()
    {
        //Dictionary<string, object> data = new Dictionary<string, object>();        
        //Analytics.CustomEvent("endless_started", data);
    }
}
