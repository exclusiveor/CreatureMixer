using UnityEngine;

//=====================================================================================
// Zanuda Games - Kozhemyakin Vitaliy, Mykhailo Zhovnach, Taras Lishchuk
//=====================================================================================

public class Consts
{
    public static string APPSTORE_URL_RATEME = "";
    public static string GOOGLEPLAY_URL_RATEME = "";
    public static string AMAZON_URL_RATEME = "";

    public static string PROJECT_NAME = "Matchventures";
    public static int APP_VERSION = 10;
    public static string FB_APP_ID = "1675739949333389";
    public static string FB_APP_NAMESPACE = "silvertale-test";

    public static string SORTING_LAYER_TOPEFFECTS = "TopEffects";
    public static string SORTING_LAYER_UNDERUIEFFECTS = "UnderUIEffects";
    public static string SORTING_LAYER_UI = "UI";    
    public static string SORTING_LAYER_UIEFFECTS = "UIEffects";

    public static int USER_NAME_MAX_LENGTH = 12;
    public static int START_GOLD_COUNT = 0;
	public static int UNLOCK_TOKENS_COUNT = 1;
	public static int SEQUENCE_REWARD_MIN = 15;
	public static int SEQUENCE_REWARD_MAX = 25;

    //public static string ASCII_CHARACHTERS = ":;<=>()/*!\"#$%&'?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
    //public static string ASCII_NUMBERS = "+-,.0123456789";
    //public static int[] CHAR_SIZES = { 16,18,19,20,22,24,25,26,27,28,29,30,31,32,33,34,36,37,38,40,41,42,45,50,52,53,55,60,63,76,79,101,114};

    /// <summary>
    /// FEEDING
    /// </summary>




    public static Color[] COLORS =
    {
        new Color(255/255.0f, 50/255.0f, 0/255.0f, 1),
        new Color(255/255.0f, 255/255.0f, 0/255.0f, 1),
        new Color(50/255.0f, 255/255.0f, 0/255.0f, 1),
        new Color(50/255.0f, 0/255.0f, 255/255.0f, 1),
		new Color(150/255.0f, 0/255.0f, 255/255.0f, 1),
		//new Color(150/255.0f, 150/255.0f, 150/255.0f, 1),
	};

    public static long[] POINTS =		// points for each level of colored pipe
	{
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9
	};

    public static int MAX_COLORED_LEVELS = 5; // on 5 level pipe will be removed
    public static bool BAD_PIXEL_MACHANIC_IN_CLASSIC_GAME = false;
    public static bool MAX_COLORED_LEVEL_REMOVES = true;
    public static bool MAX_COLORED_LEVEL_GIVES_POWERUP = false;
    public static GameData.PowerUpType MAX_COLORED_LEVEL_GIVES_POWERUP_TYPE = GameData.PowerUpType.Reshuffle; // when pipe reached MAX_COLORED_LEVELS it blast off and gives powerup
    public static float DARK_SCREEN_SHOW_HIDE_TIME = 0.5f;
    public static int CLASSIC_GAME_COLORS = 5;
    public static float IMPULSE_DISTANCE = 0.3f;
    public static float IMPULSE_SPEED = 0.05f;              // speed of moving pipe when it slide after impulse (for each slot)
	public static float MATCH_ROTATE_TIME = 0.4f;
	public static float EXTRA_DX_DY_WHEN_MATCHING 	= 0.3f;
	public static float DX_DY_OF_PIPE_WHEN_PIPE_BUMPS_INTO_IT = 0.25f;
	public static bool USE_BLOCKERS_ON_NO_MATCH_ADDING = false;

    // shakes and bumps
    //public static float SHAKE_POWER_ON_PIPE_DESTROYED = 0.2f;
    //public static float SHAKE_TIME_ON_PIPE_DESTROYED = 0.15f;
    //public static float SHAKE_POWER_ON_PIPE_COMBINE = 0.2f;
    //public static float SHAKE_TIME_ON_PIPE_COMBINE = 0.1f;
    public static float BUMP_PER_SLOT = 0.035f;
    public static float BUMP_EXTRA = 0.01f;
    public static float BUMP_TIME = 0.1f;
    public static bool BUMP_ON_MATCH = false;

    public static int TURNS_TO_NEXT_PIPE = 3;
    public static int PIPES_TO_NEXT_BLOCKER = 30;

    public static int PU__POWER_PER_LEVEL_RESHUFFLE = 5;
    public static float PU__RESHUFFLE_TIME_PER_SLOT = 0.1f;
    public static float PU__CHAIN_TIME_PER_ITERATION = 0.1f;
    public static float PU__DESTROY_COLOR_TIME_PER_ITERATION = 0.1f;
    public static float PU__SWAP_TIME = 1.0f;

    public static float SHAKE_POWER_ON_SEQUENCE = 0.25f;
    public static float SHAKE_TIME_ON_SEQUENCE = 0.2f;
    public static float PIPES_ON_SEQUENCE_ANIMATION_TIME = 0.5f;
    public static float PIPES_ON_SEQUENCE_POWER = 10.0f;

	//Powerups
	public static int POWERUPS_RESHUFFLE_AT_START = 3;
	public static int POWERUPS_BREAKE_AT_START = 3;
	public static int POWERUPS_CHAIN_AT_START = 0;
    public static int POWERUPS_DESTROY_COLOR_AT_START = 1;
    //

    public static float HINT_DELAY = 15;
    public static bool SHOW_ADD_POINTS_ANIMATION = false;

    public static float ADD_POINTS_EFFECT_TIME = 0.25f;
    public static float POINTS_RASE_TIME = 0.5f;

    public static int LEVELS_COUNT = 99;

    public static float PIPE_SIZE = 180;

	public static float STAR_PROGRESS = 0.6f;

	public static int UNLOCK_ENDLES_AFTER = 1;

    public static float TUTOR_2_DELAY = 13.0f;


    public static float MUSIC_VOLUME_MAX = 0.15f;
    public static float SOUND_VOLUME_MAX = 0.6f;

	public static bool IS_TOUCH_DEVICE = false;

    public static bool SLIDE_WITHOUT_MOUSE_UP = true;
#if UNITY_ANDROID || UNITY_IOS
    public static bool START_SLIDE_ON_NO_MOUSE_DOWN = true;
#else
    public static bool START_SLIDE_ON_NO_MOUSE_DOWN = false;
#endif

    public static int MAX_FPS = 60;
}