using UnityEngine;
using System.Collections;

public class StartGamePoint : MonoBehaviour
{    
    public UIConsts.SCENE_ID StartingScene;

	// Use this for initialization
	void Awake () {
        //if (PlayerPrefs.GetInt("TimeBomb") != 1)
        {
            //Application.targetFrameRate = 300;
            GameManager.Instance.Initialize();
            MusicManager.InitMusicManager();
            GameManager.Instance.Load();
            LeanTween.init(800);
#if UNITY_EDITOR
            Cheats.IsCheatEnabled = true;
#else
            Cheats.IsCheatEnabled = false;
#endif

#if UNITY_STANDALONE_WIN
            string[] args = System.Environment.GetCommandLineArgs();
            foreach(var arg in args)
            {
                if (arg == "/killian")
                {
                    Cheats.IsCheatEnabled = true;
                }                
            }
#endif
        }
    }

    void Start()
    {
        MusicManager.PlayMainMenuTrack();
        //if (PlayerPrefs.GetInt("TimeBomb") == 1)
        //{
        //    Application.Quit();
        //}
        //else
        {
            GameManager.Instance.GameFlow.TransitToScene(StartingScene);
        }
    }
}

