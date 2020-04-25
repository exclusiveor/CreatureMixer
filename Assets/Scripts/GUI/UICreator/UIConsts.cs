using UnityEngine;

public class UIConsts
{
	public static Vector3 DESIGN_RESOLUTION = new Vector2(1080, 1920);

	public static Vector3 START_POSITION = new Vector3(0f,1920f,0f);
	public static Vector3 STOP_POSITION = new Vector3(0f,0f,0f);

	public static LeanTweenType SHOW_EASE = LeanTweenType.easeOutBack;
	public static LeanTweenType HIDE_EASE = LeanTweenType.easeInBack;

	public static float SHOW_DELAY_TIME = 0.25f;
	public static float HIDE_DELAY_TIME = 0.25f;

	public static float SHOW_TWEEN_TIME = 0.35f;
	public static float HIDE_TWEEN_TIME = 0.35f;

    //public static float SCROLL_TIME = 0.2f;
    //public static LeanTweenType SCROLL_EASE = LeanTweenType.easeOutCubic;

    public static float ANTI_MULTI_CKLICK_TIMEOUT = 0.4f; // currently dissabled
	public static bool ENABLED_INTERACTABLE = true;

    public static string DEFAULT_UI_PATH = "Prefabs/UI/";

    public static Color CLICKS_CATCHER_COLOR = new Color(0, 0, 0, 0.63f); // fader
    public static float CLICKS_CATCHER_TIME = 0.25f;

    // UI WINDOWS
    public enum FORM_ID
	{
		NONE = -1,
		MAIN_MENU = 0,
		OPTIONS_WINDOW = 1,
		STATISTIC_WINDOW = 2,
		INPUT_WINDOW = 3,
		CONFIRM_WINDOW = 4,
		HELP_WINDOW = 5,
		TUTOR_WINDOW = 6,
        SPLASH_WINDOW = 7,
		TROPHIES_WINDOW = 8,
        GAME_MAIN_MENU = 9,
		LEVEL_SELECTION_WINDOW = 10,
		LEVELED_STATISTIC_WINDOW = 11
    }

    // SCENES IDs
    public enum SCENE_ID
    {
        NONE = -1,
        STARTSCENE = 0,
        MAINMENU = 1,   
		SPLASH_SCENE = 2,
        GAME_SCENE = 3,
        GAME_SCENE_WIDE = 4
    }

    public static string[] SCENE_NAMES = {
                                           "StartGameScene",
                                           "MainMenu",
										   "SplashScene",
                                           "GameScene",
                                           "GameSceneWide"};
}
