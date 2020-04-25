using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum EGameType
{
    Classic = 0,
	Leveled = 1
}
public enum EAddingType
{
    EachXMoves = 0,
    OnNoMatch = 1
}

public enum ESlideType
{
	None = 0,
	Move = 1,
	Consum = 2,
	Match = 3
}

public struct SlideData
{
	public BoardPos 		PosSlideFrom;
	public BoardPos 		PosAforMatchSlide;
	public BoardPos 		FinalPosForSlide;
	public SSlot 			Slot;
	public SSlot 			Slot2;				// slot in which we arrive
	public SPipe 			Pipe;
	public SPipe 			Pipe2;				// pipe at slot Slot2
	public int 				DirX;
	public int 				DirY;
	public int 				DistX;
	public int 				DistY;
	public ESlideType 		SlideType;
}

public class GameBoard : MonoBehaviour
{
	public struct MatchHintData
	{
		public int XA;
		public int YA;
		public int XB;
		public int YB;
		public bool IsMatch;
	}

    private struct ChainInfo
    {
        public int X;
        public int Y;
        public int Color;
        public int Param;
        public int Id;
        public EPipeType PipeType;
    }

    public const int 							WIDTH 					= 5;
	public const int 							HEIGHT 					= 5;
    public static Vector2                       DXDY = Vector2.zero; // зміщення боарда, щоб потрапив в поле зору камери
    public static float							SlotZ                 	= 0.3f;
    public static float             			PipeZ                	= 0.26f;
	public static float             			PipeZForMatch			= -2.0f;
    public static float             			PipeDragZ             	= -0.3f;
    public static float             			SlotSize              	= 1.86f;
	public static float             			ImpulseSpeed       	  	= 30.0f;                            		// speed of moving pipe when it slide after impulse
    public static EGameType                     GameType                = EGameType.Classic;
    public static EAddingType                   AddingType              = EAddingType.EachXMoves;

    public float             					ImpulseDistance	  		= 0.5f;//TODO в опшнси винести!				// how far need slide to pull pipe
	public SSlot[,] 			    			Slots { get; set; }
	public int                   				MaxX { get; set; }
	public int                   				MinX { get; set; }
	public int                   				MaxY { get; set; }
	public int                   				MinY { get; set; }

	public GameObject							BumpShakeObject;
	private float								_shakeDx = 0;
	private float								_shakeDy = 0;

	private Vector3								_cameraPos;

	public QueuePanel                           AQueuePanel;
	public SequencePanel                        ASequencePanel;
	public MovesPanel							AMovesPanel;
	public StarsPanel							AStarsPanel;
	public LevelPanel							ALevelPanel;

    // sprites
    private Dictionary<string, Sprite>          _sprites = new Dictionary<string, Sprite>();
    public List<Sprite>                         Sprites;
    


    public List<GameObject>               		PipesPrefabs;														// prefabs for pipes
	public List<GameObject>						ColoredPipesPrefabs;
	// pool
    public GameObject               			SSlotPrefab;
	public Dictionary<string, List<GameObject>> Pool { get; set; }
	//
    public GameObject               			Selection;                                                			// selection for pipe that we move

	public Transform							SlotsContainer;
    
    //private Vector2               				_dragDxDy             			= new Vector2();
	protected Vector2               			_startPos               		= new Vector2();                    // position when we click on pipe

	//
//    private int                   				_prevXin;
//    private int                   				_prevYin;
//    private float                 				_prevXpos;
//    private float                 				_prevYpos;

	public GameObject                           BreakeEffectPrefab;
    public GameObject                           ChainEffectPrefab;
	public GameObject							MatchEffectPrefab;

    public bool                                 BreakePowerup { get; set; }
    public bool                                 ChainPowerup { get; set; }
    public bool                                 DestroyColorPowerup { get; set; }

    private Dictionary<int, ChainInfo>          _chainInfos = new Dictionary<int, ChainInfo>();
    private Dictionary<int, ChainInfo>          _checkedChainInfos = new Dictionary<int, ChainInfo>();
	private int 								_maxColoredLevels = 9;

	private float 								_hintTimer = 0;
    private float                               _tutor2Timer = 0;
    private NewHintScript				    	_hint;
	public GameObject							HintPrefab;
    private int                                 _startSequenceState = 0;
    private MatchHintData                       _startTutorHintData;

    public GameMenuUIController                 GameMenuUIController;

    private Camera _camera;
    private Canvas _canvas;
    public Canvas ACanvas
    {
        set
        {
            _canvas = value;
        }

        get
        {
            if (_canvas == null)
            {
                _canvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
            }
            return _canvas;
        }
    }

	private int                               _currentTouchId = -1; // for EInputType.UsingPositions
	public Material[] ColoredMaterials;

    void Awake()
    {
        // limiting FPS
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Consts.MAX_FPS;
        //
        _camera = Camera.main;
        GameManager.Instance.BoardData = new GameBoardData();
		GameManager.Instance.BoardData.AGameBoard = this;
		GameManager.Instance.Game = this;
        //EventManager.OnTransitToMenu += OnTransitToMenu;
        //EventManager.OnStartPlayPressedEvent += CallOnStartPlayPressed;

        DXDY = new Vector2(-WIDTH * SlotSize / 2.0f + SlotSize / 2.0f, -HEIGHT * SlotSize / 2.0f + SlotSize / 2.0f);
        DXDY.y += 0.54f;
        DXDY.x += transform.localPosition.x;
        DXDY.y += transform.localPosition.y;

        // Pools
        Pool = new Dictionary<string, List<GameObject>>();
		Pool.Add("SSlots", new List<GameObject>());
		for (int i = 0; i < Consts.CLASSIC_GAME_COLORS; ++i)
		{
			Pool.Add("Colored_" + i.ToString(), new List<GameObject>());
		}
		for (EPipeType i = EPipeType.Base; i < EPipeType.Last; ++i)
		{
			Pool.Add("Pipe_" + ((int)i).ToString(), new List<GameObject>());
		}
        //
        BreakePowerup = false;
        ChainPowerup = false;
        DestroyColorPowerup = false;
        //
        SetGameState(EGameState.Pause);
		CreateSlots();
        // 
        for (int i = 0; i < Sprites.Count; ++i)
        {
            _sprites.Add(Sprites[i].name, Sprites[i]);
        }
    }

    void OnDestroy()
    {
        //EventManager.OnTransitToMenu -= OnTransitToMenu;
		//EventManager.OnStartPlayPressedEvent -= CallOnStartPlayPressed;
    }
    // Use this for initialization
    void Start () 
	{
		_camera.transform.position = new Vector3(0, 0.2f, _camera.transform.position.z);
        //Invoke("PlayGame", 0.15f);
		//PlayGame();
    }

    //void CallOnStartPlayPressed(EventData e)
    //{
    //	PlayGame();
    //}

    // Update is called once per frame
    void Update () 
	{
        // limiting FPS
        if (Application.targetFrameRate != Consts.MAX_FPS)
        {
            Application.targetFrameRate = Consts.MAX_FPS;
            //Debug.LogError("!!!!!");
        }
        //
        if (GameManager.Instance.CurrentMenu != UISetType.ClassicGame && GameManager.Instance.CurrentMenu != UISetType.LeveledGame)
		{
			return;
		}
        Cheats.CheckMatchCheats(this);
		// update drag of slot
		if (GameManager.Instance.BoardData.IsPause() || GameManager.Instance.BoardData.IsLoose())
		{
			return;
		}
        TryShowHint();
        TryShowTutor2();
        //camera shakes	
        Vector3 realPos = _cameraPos;
        realPos.x -= _shakeDx;
        realPos.y -= _shakeDy;
        _camera.transform.position = realPos;

		UpdateInput();
    }

  //  void OnTransitToMenu(EventData e)
  //  {
		//CancelInvoke();
  //  }

	///////////
	public static Vector3 SlotPos(int x, int y)
	{
		return new Vector3(DXDY.x + x * SlotSize, DXDY.y + y * SlotSize, SlotZ);
	}

	public static Vector2 SlotPos(Vector2 posin)
	{
		return SlotPos((int)posin.x, (int)posin.y);
	}

	public static Vector3 PipePos(int x, int y)
	{
		Vector3 res = SlotPos(x, y);
		res.z = PipeZ;
		return res;
	}

	public static Vector2 PipePos(Vector2 posin)
	{
		Vector3 res = SlotPos((int)posin.x, (int)posin.y);
		res.z = PipeZ;
		return res;
	}

	public static BoardPos SlotPosIn(float x, float y)
	{
		BoardPos res;
		float resx = (x - DXDY.x) / SlotSize - 0.5f;
		float resy = (y - DXDY.y) / SlotSize - 0.5f;
		res.x = Mathf.CeilToInt(resx);
		res.y = Mathf.CeilToInt(resy);
		return res;
	}

	public static BoardPos SlotPosIn(Vector2 pos)
	{
		return SlotPosIn(pos.x, pos.y);
	}

	public bool IsSlotInBoard(int i, int j)
	{
		if (i >= WIDTH || i < 0 || j >= HEIGHT || j < 0) 
		{
			return false;
		}
		return true;
	}

	public bool IsSlotInBoard(BoardPos posInd)
	{
		return IsSlotInBoard(posInd.x, posInd.y);
	}

	public SSlot GetSlot (int i, int j) 
	{
		return Slots[i,j];
	}

	public SSlot GetSlot (BoardPos posInd) 
	{
		return GetSlot(posInd.x, posInd.y);
	}

	public void SetSlot (int i, int j, SSlot slot) 
	{
		slot.X = i;
		slot.Y = j;
		Slots[i,j] = slot;
		slot.transform.position = SlotPos(i, j);
	}
	
	////////////
	protected void CreateSlots()
	{
		// create empty invissible slots
		Slots = new SSlot[WIDTH, HEIGHT];
		for (int i = 0; i < WIDTH; ++i)
		{
			for (int j = 0; j < HEIGHT; ++j)
			{
				GameObject slotObj = GetSSlotFromPoolWithCreation();
				slotObj.transform.SetParent(SlotsContainer, false);
				SSlot slot = slotObj.GetComponent<SSlot>();
				slotObj.transform.position = SlotPos(i, j);
				slot.InitSlot(i, j);
				Slots[i, j] = slot;
			}
		}
        //
        AddSlotDoubles();
	}

    private void AddSlotDoubles()
    {
        // adding CellDoubles
        for (int i = 0; i < GameManager.Instance.Player.SlotsDoubles.Count; ++i)
        {
            Vector2 pos = GameManager.Instance.Player.SlotsDoubles[i];
            int x = (int)pos.x;
            int y = (int)pos.y;
            Slots[x, y].AddSlotDouble();
        }
    }

	protected SPipe CreatePipe(EPipeType pType, int parameter, int color)
	{
		SPipe res = null;
		GameObject pipeObj = null;
		if (pType == EPipeType.Colored && color < 0)
		{
			color = GameManager.Instance.BoardData.GetRandomColor();
		}
		pipeObj = GetPipeFromPool(pType, color);

		pipeObj.transform.SetParent(SlotsContainer, false);
		SPipe pipe = pipeObj.GetComponent<SPipe>();
		pipe.InitPipe(parameter, color);
		//pipe.transform.parent = transform;
		res = pipe;
		res.transform.localScale = new Vector3(1, 1, 1);
		LeanTween.cancel(pipeObj);
		return res;
	}

    public void ClearBoardForce()
    {
        if (Slots == null) return;
        for (int i = 0; i < WIDTH; ++i)
        {
            for (int j = 0; j < HEIGHT; ++j)
            {
                SPipe pipe = Slots[i, j].TakePipe();
                if (pipe) pipe.gameObject.SetActive(false);
            }
        }
    }

    protected IEnumerator ClearBoard()
    {
        if (Slots == null) yield return null;

        for (int i = 0; i < WIDTH; ++i)
        {
            for (int j = 0; j < HEIGHT; ++j)
            {
                SPipe pipe = Slots[i, j].TakePipe();
                if (pipe)
                {
                    pipe.PlayHideAnimation();
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
    }

    protected IEnumerator CreateLevel(LevelData levelData) 
	{
        _currentTouchId = -1;
        GameManager.Instance.BoardData.DragSlot = null;
        GameManager.Instance.BoardData.AGameBoard.HideSelection();
        //
        AQueuePanel.LoadPanel(levelData.QueueState);
        for (int i = 0; i < WIDTH; ++i)
		{
			for (int j = 0; j < HEIGHT; ++j)
			{
				Slots[i, j].SetAsNotHole();
			}
		}

		GameManager.Instance.BoardData.TimePlayed = levelData.timePlayed;
		for (int i = 0; i < levelData.Resources.Count; ++i)
		{
			GameManager.Instance.BoardData.SetResourceForce(levelData.Resources[i], i);
		}

		GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Reshuffle] = levelData.ReshufflePowerups;
        GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Breake] = levelData.BreakePowerups;
        GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Chain] = levelData.ChainPowerups;
        GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.DestroyColor] = levelData.DestroyColorsPowerups;
        GameManager.Instance.BoardData.AddsViewed = levelData.AddsViewed;

		// powerups
		EventData eventData = new EventData("OnPowerUpsResetNeededEvent");
		eventData.Data["isStart"] = true;
		GameManager.Instance.EventManager.CallOnPowerUpsResetNeededEvent(eventData);
        BreakePowerup = false;
        ChainPowerup = false;
        DestroyColorPowerup = false;

        yield return new WaitForSeconds(Consts.DARK_SCREEN_SHOW_HIDE_TIME);
        yield return StartCoroutine(ClearBoard());
        // create pipes force
        for (int i = 0; i < levelData.Slots.Count; ++i)
        {
            int x = levelData.Slots[i].x;
            int y = levelData.Slots[i].y;
            Slots[x, y].InitSavedSlot(levelData.Slots[i]);
            // pipe
            EPipeType pType = (EPipeType)levelData.Slots[i].pt;
            if (pType != EPipeType.None)
            {
                // create pipe
                SPipe pipe = CreatePipe(pType, levelData.Slots[i].p, levelData.Slots[i].c);
                Slots[x, y].SetPipe(pipe);
                pipe.PlayAddAnimation();
                yield return new WaitForSeconds(0.05f);
            }
        }
        yield return new WaitForSeconds(0.25f);
        UnsetPause();
    }

	protected IEnumerator CreateLeveledLevel(ScriptableLevelData levelData) 
	{
        _currentTouchId = -1;
        GameManager.Instance.BoardData.DragSlot = null;
        GameManager.Instance.BoardData.AGameBoard.HideSelection();
        //
        GameManager.Instance.BoardData.TimePlayed = 0;
		GameManager.Instance.BoardData.MovesLeft = levelData.MinMovesCount;
		GameManager.Instance.BoardData.StarsGained = 0;
		AMovesPanel.SetAmountForce(GameManager.Instance.BoardData.MovesLeft);
        AStarsPanel.ResetScores();
        //AStarsPanel.SetAmountForce(GameManager.Instance.BoardData.StarsGained);
		ALevelPanel.SetText();
		
		GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Reshuffle] = 0;
		GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Breake] = 0;
		GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Chain] = 0;
		GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.DestroyColor] = 0;
		GameManager.Instance.BoardData.AddsViewed = true;

        // powerups
        EventData eventData = new EventData("OnPowerUpsResetNeededEvent");
        eventData.Data["isStart"] = true;
        GameManager.Instance.EventManager.CallOnPowerUpsResetNeededEvent(eventData);
        BreakePowerup = false;
        ChainPowerup = false;
        DestroyColorPowerup = false;

        yield return new WaitForSeconds(Consts.DARK_SCREEN_SHOW_HIDE_TIME);
        yield return StartCoroutine(ClearBoard());
        // create pipes force
        for (int i = 0; i < levelData.StartStates.Count; ++i)
        {
            int x = levelData.StartStates[i].x;
            int y = levelData.StartStates[i].y;
            Slots[x, y].InitSavedSlot(levelData.StartStates[i]);
            // pipe
            EPipeType pType = (EPipeType)levelData.StartStates[i].pt;
            if (pType != EPipeType.None)
            {
                // create pipe
                SPipe pipe = CreatePipe(pType, levelData.StartStates[i].p, levelData.StartStates[i].c);
                Slots[x, y].SetPipe(pipe);
                pipe.PlayAddAnimation();
                if (pType == EPipeType.Hole)
                {
                    Slots[x, y].SetAsHole();
                }
                yield return new WaitForSeconds(0.05f);
            }
        }
        yield return new WaitForSeconds(0.25f);
        UnsetPause();
    }
		
	public void PlayGame()
    {
		GameType = EGameType.Classic;
		_maxColoredLevels = GameManager.Instance.BoardData.GetMaxColoredLevels();
		ResetHint();

        GameManager.Instance.BoardData.Reset();
        _cameraPos = _camera.transform.position;
		_cameraPos.x = 0;
		_cameraPos.y = 0.2f;
		_camera.transform.position = _cameraPos;
		ShowDarkScreenForce();
		Selection.SetActive(false);
		SetGameState(EGameState.Pause);

        LevelData levelData = GameManager.Instance.Settings.User.SavedGame;
		if (levelData == null || levelData.Slots.Count == 0)
		{
			levelData = GameManager.Instance.GameData.StartLevelData;
		} else
		{
			GameManager.Instance.Settings.User.SavedGame = null;
			GameManager.Instance.Settings.Save();
		}
        StartCoroutine(CreateLevel(levelData));
    }

	public void PlayLeveledGame()
	{
        HideHint();
		GameType = EGameType.Leveled;
		_maxColoredLevels = GameManager.Instance.BoardData.GetMaxColoredLevels();
        ResetTutor2Timer();
        TryStartStartTutorSequence();

        GameManager.Instance.BoardData.Reset();
		_cameraPos = _camera.transform.position;
		_cameraPos.x = 0;
		_cameraPos.y = 0.2f;
		_camera.transform.position = _cameraPos;
		ShowDarkScreenForce();
		Selection.SetActive(false);
		SetGameState(EGameState.Pause);

		string path = "Levels/level_" + GameManager.Instance.Player.CurrentLevel.ToString();
		ScriptableLevelData leveledlevelData = (ScriptableLevelData)Resources.Load<ScriptableLevelData>(path);
		StartCoroutine(CreateLeveledLevel(leveledlevelData));
        ZAnalitycs.StartLevelEvent(GameManager.Instance.Player.CurrentLevel);
	}

	public void UnsetPause()
	{
		SetGameState(EGameState.Play);
	}

	public void SetGameState(EGameState state)
	{
		GameManager.Instance.BoardData.SetGameState(state);
	}
	
	protected LevelData GetLevelToSave()
    {
        LevelData res = new LevelData();
		res.timePlayed = GameManager.Instance.BoardData.TimePlayed;
		for (int i = 0; i < Consts.COLORS.Length; ++i)
		{
			res.Resources[i] = GameManager.Instance.BoardData.GetResourceAmount(i);
		}
        // pipes
        for (int i = 0; i < WIDTH; ++i)
        {
            for (int j = 0; j < HEIGHT; ++j)
            {
                SSlot slotScript = Slots[i, j];
				SPipe pipeScript = slotScript.Pipe;
				if (pipeScript != null)
				{
					SSlotData slotData = new SSlotData();
					slotData.x = slotScript.X;
					slotData.y = slotScript.Y;
					slotData.pt = pipeScript.PipeType;
					slotData.c = pipeScript.AColor;
					slotData.p = pipeScript.Param;
					res.Slots.Add(slotData);
				}
            }
        }
        // queue state
        res.QueueState = AQueuePanel.GetStateToSave();
        //
        res.ReshufflePowerups = GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Reshuffle];
        res.BreakePowerups = GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Breake];
        res.ChainPowerups = GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Chain];
        res.DestroyColorsPowerups = GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.DestroyColor];
        res.AddsViewed = GameManager.Instance.BoardData.AddsViewed;
        return res;
    }

	protected void ShowDarkScreenForce()
    {
//		EventData eventData = new EventData("OnWindowNeededEvent");
//		eventData.Data["name"] = "DarkScreen";
//		eventData.Data["isforce"] = true;
//		GameManager.Instance.EventManager.CallOnWindowNeededEvent(eventData);
	}
	
    protected void ShowDarkScreen()
    {
//		EventData eventData = new EventData("OnWindowNeededEvent");
//		eventData.Data["name"] = "DarkScreen";
//		eventData.Data["isforce"] = false;
//		GameManager.Instance.EventManager.CallOnWindowNeededEvent(eventData);
	}
	
	// ------------> POOLS
	public GameObject GetSSlotFromPoolWithCreation()
	{
		// TODO slots olways on game screen? no instantiation?
		List<GameObject> objects = Pool["SSlots"];
		GameObject sslot = (GameObject)GameObject.Instantiate(SSlotPrefab, Vector3.zero, Quaternion.identity);
		objects.Add(sslot);
		return sslot;
	}

	public GameObject GetSSlotFromPool()
	{
		// TODO slots olways on game screen? no instantiation?
		List<GameObject> objects = Pool["SSlots"];
		for (int i = 0; i < objects.Count; ++i)
		{
			if (!objects[i].activeSelf)
			{
				objects[i].SetActive(true);
				return objects[i];
			}
		}
		GameObject sslot = (GameObject)GameObject.Instantiate(SSlotPrefab, Vector3.zero, Quaternion.identity);
		objects.Add(sslot);
		return sslot;
	}
		
	public GameObject GetPipeFromPool(EPipeType pType, int color = -1)
	{
		int pid = (int)pType;
		string sid = pid.ToString();
		List<GameObject> objects = null;
		if (pType == EPipeType.Colored)
		{
			objects = Pool["Colored_" + color];
		} else
		{
			objects = Pool["Pipe_" + sid];
		}
		for (int i = 0; i < objects.Count; ++i)
		{
			if (!objects[i].activeSelf)
			{
				objects[i].SetActive(true);
				return objects[i];
			}
		}

		GameObject pipe = null;
		if (pType == EPipeType.Colored)
		{
			pipe = (GameObject)GameObject.Instantiate(ColoredPipesPrefabs[color], Vector3.zero, Quaternion.identity);
		} else
		{
			pipe = (GameObject)GameObject.Instantiate(PipesPrefabs[pid], Vector3.zero, Quaternion.identity);
		}

		pipe.transform.SetParent(SlotsContainer, false);
		objects.Add(pipe);
		return pipe;
	}

	// <-----------

	private void FindMinXToSlide(ref SlideData slideData)
	{
		for (int i = slideData.PosSlideFrom.x - 1; i >= 0; --i)
	    {
			SSlot slot2 = Slots[i, slideData.PosSlideFrom.y];
			SPipe pipe2 = slot2.Pipe;
			if (slot2.IsEmpty())
			{
	
			} else
			{
				slideData.Slot2 = slot2;
				slideData.Pipe2 = pipe2;
				slideData.SlideType = CheckPipesForCooperation(slideData.Pipe, slideData.Pipe2, slideData.Slot2);
				if (slideData.SlideType == ESlideType.Match)
				{
					slideData.FinalPosForSlide.x = i;
					slideData.PosAforMatchSlide.x = slideData.FinalPosForSlide.x + 1;
				} else
				{
					slideData.FinalPosForSlide.x = i + 1;
					slideData.Slot2 = GetSlot(slideData.FinalPosForSlide);
					slideData.Pipe2 = slideData.Slot2.Pipe;
				}
				return;
			}
	    }
		slideData.SlideType = ESlideType.Move;
		slideData.FinalPosForSlide.x = 0;
		slideData.PosAforMatchSlide.x = slideData.FinalPosForSlide.x;
		slideData.Slot2 = GetSlot(slideData.FinalPosForSlide);
		slideData.Pipe2 = slideData.Slot2.Pipe;
	}

	private void FindMaxXToSlide(ref SlideData slideData)
	{
		for (int i = slideData.PosSlideFrom.x + 1; i < WIDTH; ++i)
		{
			SSlot slot2 = Slots[i, slideData.PosSlideFrom.y];
			SPipe pipe2 = slot2.Pipe;
			if (slot2.IsEmpty())
			{

			} else
			{
				slideData.Slot2 = slot2;
				slideData.Pipe2 = pipe2;
				slideData.SlideType = CheckPipesForCooperation(slideData.Pipe, slideData.Pipe2, slideData.Slot2);
				if (slideData.SlideType == ESlideType.Match)
				{
					slideData.FinalPosForSlide.x = i;
					slideData.PosAforMatchSlide.x = slideData.FinalPosForSlide.x - 1;
				} else
				{
					slideData.FinalPosForSlide.x = i - 1;
					slideData.Slot2 = GetSlot(slideData.FinalPosForSlide);
					slideData.Pipe2 = slideData.Slot2.Pipe;
				}
				return;
			}
		}
		slideData.SlideType = ESlideType.Move;
		slideData.FinalPosForSlide.x = WIDTH - 1;
		slideData.PosAforMatchSlide.x = slideData.FinalPosForSlide.x;
		slideData.Slot2 = GetSlot(slideData.FinalPosForSlide);
		slideData.Pipe2 = slideData.Slot2.Pipe;
	}
		
	private void FindMinYToSlide(ref SlideData slideData)
	{
		for (int i = slideData.PosSlideFrom.y - 1; i >= 0; --i)
		{
			SSlot slot2 = Slots[slideData.PosSlideFrom.x, i];
			SPipe pipe2 = slot2.Pipe;
			if (slot2.IsEmpty())
			{

			} else
			{
				slideData.Slot2 = slot2;
				slideData.Pipe2 = pipe2;
				slideData.SlideType = CheckPipesForCooperation(slideData.Pipe, slideData.Pipe2, slideData.Slot2);
				if (slideData.SlideType == ESlideType.Match)
				{
					slideData.FinalPosForSlide.y = i;
					slideData.PosAforMatchSlide.y = slideData.FinalPosForSlide.y + 1;
				} else
				{
					slideData.FinalPosForSlide.y = i + 1;
					slideData.Slot2 = GetSlot(slideData.FinalPosForSlide);
					slideData.Pipe2 = slideData.Slot2.Pipe;
				}
				return;
			}
		}
		slideData.SlideType = ESlideType.Move;
		slideData.FinalPosForSlide.y = 0;
		slideData.PosAforMatchSlide.y = slideData.FinalPosForSlide.y;
		slideData.Slot2 = GetSlot(slideData.FinalPosForSlide);
		slideData.Pipe2 = slideData.Slot2.Pipe;
	}

	private void FindMaxYToSlide(ref SlideData slideData)
	{
		for (int i = slideData.PosSlideFrom.y + 1; i < HEIGHT; ++i)
		{
			SSlot slot2 = Slots[slideData.PosSlideFrom.x, i];
			SPipe pipe2 = slot2.Pipe;
			if (slot2.IsEmpty())
			{

			} else
			{
				slideData.Slot2 = slot2;
				slideData.Pipe2 = pipe2;
				slideData.SlideType = CheckPipesForCooperation(slideData.Pipe, slideData.Pipe2, slideData.Slot2);
				if (slideData.SlideType == ESlideType.Match)
				{
					slideData.FinalPosForSlide.y = i;
					slideData.PosAforMatchSlide.y = slideData.FinalPosForSlide.y - 1;
				} else
				{
					slideData.FinalPosForSlide.y = i - 1;
					slideData.Slot2 = GetSlot(slideData.FinalPosForSlide);
					slideData.Pipe2 = slideData.Slot2.Pipe;
				}
				return;
			}
		}
		slideData.SlideType = ESlideType.Move;
		slideData.FinalPosForSlide.y = HEIGHT - 1;
		slideData.PosAforMatchSlide.y = slideData.FinalPosForSlide.y;
		slideData.Slot2 = GetSlot(slideData.FinalPosForSlide);
		slideData.Pipe2 = slideData.Slot2.Pipe;
	}

	private ESlideType CheckPipesForCooperation(SPipe pipe, SPipe pipe2, SSlot slot2)
	{
		if (!slot2.IsMovable() || pipe.PipeType != EPipeType.Colored)
		{
			// slot not movable or slide not colored pipe (blocker)
			return ESlideType.Move;
		}
		if (pipe.PipeType == EPipeType.Colored)
		{
			// slide color pipe, check type of pipe2
			EPipeType pType2 = pipe2.PipeType;
			if (pipe2.IsCanConsumeColoredPipes())
			{
				// base consum color pipes
				return ESlideType.Consum;
			} else
			{
				if (pType2 == EPipeType.Colored && pipe.AColor == pipe2.AColor && _maxColoredLevels > pipe.Param && pipe.Param == pipe2.Param)
				{
					// can match them
					return ESlideType.Match;
				} else
				{
					return ESlideType.Move;
				}
			}
		}
		return ESlideType.None;
	}

	private SlideData CreateSlideData(SSlot slot, int xDir, int yDir)
	{
		SlideData res;
		res.Slot2 = null;
		res.Pipe2 = null;
		res.Slot = slot;
		res.Pipe = slot.Pipe;
		res.DirX = xDir;
		res.DirY = yDir;
		res.SlideType = ESlideType.None;
		res.PosSlideFrom.x = res.Slot.X;
		res.PosSlideFrom.y = res.Slot.Y;
		res.FinalPosForSlide.x = res.PosSlideFrom.x;
		res.FinalPosForSlide.y = res.PosSlideFrom.y;
		res.PosAforMatchSlide.x = res.FinalPosForSlide.x;
		res.PosAforMatchSlide.y = res.FinalPosForSlide.y;
		res.DistX = 0;
		res.DistY = 0;


		if (res.DirX != 0)
		{
			// try horizontal impulse
			if (res.DirX > 0)
			{
				// impulse to right
				FindMaxXToSlide(ref res);
			} else
			{
				// impulse to left
				FindMinXToSlide(ref res);
			}
		} else
		{
			// try vertical impulse
			if (res.DirY > 0)
			{
				// impulse to top
				FindMaxYToSlide(ref res);
			} else
			{
				// impulse to bottom
				FindMinYToSlide(ref res);
			}
		}
		// find distances in slots
		res.DistX = Mathf.Abs(res.PosSlideFrom.x - res.FinalPosForSlide.x);
		res.DistY = Mathf.Abs(res.PosSlideFrom.y - res.FinalPosForSlide.y);
		//
		return res;
	}

	public void SlidePipe(SSlot slot, int xDir, int yDir)
    {
        if (slot.Pipe == null)
		{
			// nothing to slide
			return;
		}

        SlideData slideData = CreateSlideData(slot, xDir, yDir);
		if ((slideData.DistX == 0 && slideData.DistY == 0) || slideData.SlideType == ESlideType.None)
		{
			// cant slide
			return;
		}
        // start tutor
        if (_startSequenceState > 0 && ( _startTutorHintData.XA != slot.X || _startTutorHintData.YA != slot.Y || _startTutorHintData.XB != slideData.Slot2.X || _startTutorHintData.YB != slideData.Slot2.Y))
        {
            return;
        }
        else
        {
            if (_startSequenceState > 0)
            {
                HideHint();
                LeanTween.delayedCall(0.5f, () => { ForceToSwipe(); });
            }
        }
        // slide according to type
        if (slideData.SlideType == ESlideType.Match)
		{
			// match
			SlidePipeWithMatch(slideData);
		} else
		{
			// no match
			SlidePipeWithoutMatch(slideData);
		}
        ResetHint();
        ResetTutor2Timer();
	}

	private void SlidePipeWithMatch(SlideData slideData)
	{
		slideData.Slot2.WaitForPipe = true;
		SPipe pipe = slideData.Slot.TakePipe();
		Helpers.SetZ(pipe.transform, PipeZForMatch);
		// match
		if (slideData.DirX != 0)
		{
			float xPos = GetSlot(slideData.PosAforMatchSlide).transform.position.x;
			xPos += slideData.DirX * Consts.EXTRA_DX_DY_WHEN_MATCHING;
			float atime2 = (slideData.DistX - 1) * Consts.IMPULSE_SPEED;
            if (atime2 == 0.0f)
            {
                atime2 = 0.05f;
            }
            LeanTween.moveX(pipe.gameObject, xPos, atime2)
				.setOnComplete(() =>
					{
						OnPipeArrivedToSlotWithMatch(slideData);
					});
		} else
		//if (slideData.DirY != 0)
		{
			float yPos = GetSlot(slideData.PosAforMatchSlide).transform.position.y;
			yPos += slideData.DirY * Consts.EXTRA_DX_DY_WHEN_MATCHING;
			float atime2 = (slideData.DistY - 1) * Consts.IMPULSE_SPEED;
            if (atime2 == 0.0f)
            {
                atime2 = 0.05f;
            }
			LeanTween.moveY(pipe.gameObject, yPos, atime2)
				.setOnComplete(() =>
					{
						OnPipeArrivedToSlotWithMatch(slideData);
					});
		}
	}

	private void SlidePipeWithoutMatch(SlideData slideData)
	{
		slideData.Slot2.WaitForPipe = true;
		SPipe pipe = slideData.Slot.TakePipe();
		float atime = Consts.IMPULSE_SPEED * slideData.DistX + Consts.IMPULSE_SPEED * slideData.DistY;
		pipe.SetXY(slideData.FinalPosForSlide.x, slideData.FinalPosForSlide.y);
		if (slideData.DirX != 0)
		{
			// horizontal slide
			float xPos = slideData.Slot2.transform.position.x;
			LeanTween.moveX(pipe.gameObject, xPos, atime)
				.setOnComplete(() =>
					{
						OnPipeArrivedToSlotWithoutMatch(slideData);
					});
		} else
		if (slideData.DirY != 0)
		{
			// vertical slide
			float yPos = slideData.Slot2.transform.position.y;
			LeanTween.moveY(pipe.gameObject, yPos, atime)
				.setOnComplete(() =>
					{
						OnPipeArrivedToSlotWithoutMatch(slideData);
					});
		}
	}

	private void OnPipeArrivedToSlotWithMatch(SlideData slideData)
	{
		SSlot slot2 = slideData.Slot2;
		SPipe pipe2 = slideData.Pipe2;
		SPipe pipe = slideData.Pipe;
//		// rotate pipe
//		if (slideData.DirX != 0)
//		{
//			// horizontal move
//			if (slideData.DirX  < 0)
//			{
//				LeanTween.rotateLocal(pipe.CubeObject, new Vector3(pipe.CubeObject.transform.localEulerAngles.x, pipe.CubeObject.transform.localEulerAngles.y + 180, 0), rotateTime)
//					.setEase(LeanTweenType.easeOutSine);
//			} else
//			{
//				LeanTween.rotateLocal(pipe.CubeObject, new Vector3(pipe.CubeObject.transform.localEulerAngles.x, pipe.CubeObject.transform.localEulerAngles.y - 180, 0), rotateTime)
//					.setEase(LeanTweenType.easeOutSine);
//			}
//		} else
//		{
//			// vertical move
//			if (slideData.DirY < 0)
//			{
//				LeanTween.rotateLocal(pipe.CubeObject, new Vector3(pipe.CubeObject.transform.localEulerAngles.x + 180, pipe.CubeObject.transform.localEulerAngles.y, 0), rotateTime)
//					.setEase(LeanTweenType.easeOutSine);
//			} else
//			{
//				//LeanTween.rotateX(pipe.CubeObject, 180.0f, rotateTime)
//					//.setEase(LeanTweenType.easeOutSine);
//				LeanTween.rotateLocal(pipe.CubeObject, new Vector3(pipe.CubeObject.transform.localEulerAngles.x - 180, pipe.CubeObject.transform.localEulerAngles.y, 0), rotateTime)
//					.setEase(LeanTweenType.easeOutSine);
//			}
//		}

		// rase value and rotate animation
		slideData.Pipe.RaseCombineAnimation(slideData.DirX, slideData.DirY);
        // points
        int multiplyer = 1;
        if (GameManager.Instance.Player.SlotsDoubles.Contains(new Vector2(slot2.X, slot2.Y)))
        {
            multiplyer = 2;
        }
        //LeanTween.delayedCall(Consts.MATCH_ROTATE_TIME, () =>
        //{
            GameManager.Instance.BoardData.AddResourceByLevelOfColoredPipe(pipe.Param, pipe.AColor, multiplyer, slot2.transform.position);
        //});
        // move new pipe to center of new slot
        Vector2 newPos = slot2.transform.position;
		LeanTween.move(pipe.gameObject, newPos, Consts.MATCH_ROTATE_TIME)
            .setEase(LeanTweenType.easeOutSine)
			.setOnComplete(()=>{
				Vector3 pos = slideData.Slot2.transform.position;
				pos.z = PipeZ;
				slideData.Pipe.transform.position = newPos;
				OnPipeLandAfterMatch(slideData);
			});
        //// move prev pipe slightly
        ////Vector3 prevPos = pipe2.transform.position;
        ////prevPos.z += 0.05f;
        ////LeanTween.move(pipe2.gameObject, new Vector3(prevPos.x + slideData.DirX * Consts.DX_DY_OF_PIPE_WHEN_PIPE_BUMPS_INTO_IT, prevPos.y + slideData.DirY * Consts.DX_DY_OF_PIPE_WHEN_PIPE_BUMPS_INTO_IT, prevPos.z), Consts.MATCH_ROTATE_TIME / 4.0f)
        ////	.setEase(LeanTweenType.easeOutSine)
        ////	.setOnComplete(() =>
        ////		{
        ////			LeanTween.move(pipe2.gameObject, prevPos, Consts.MATCH_ROTATE_TIME / 4.0f);
        ////		});
        //// bump

//        GameObject coloredObj = pipe2.RotateObject.transform.Find("Color_0").gameObject;
//        Pipe_Colored coloredPipe2 = (Pipe_Colored)pipe2;
//        Renderer pipe2Rend = coloredObj.GetComponent<Renderer>();
//        pipe2Rend.material = new Material(pipe2Rend.material);
//        Color prevColor = pipe2Rend.material.color;
//        LeanTween.value(pipe2.gameObject, pipe2Rend.material.color, new Color(0.4f, 0.4f, 0.4f), Consts.MATCH_ROTATE_TIME)
//                 .setDelay(0.05f)
//                 .setEase(LeanTweenType.easeOutSine)
//                 .setOnUpdate((Color col) =>
//                {
//                    pipe2Rend.material.color = col;
//                })
//                 .setOnComplete(() =>
//               {
//                   pipe2Rend.material.color = prevColor;
//               });
//        LeanTween.value(pipe2.gameObject, Color.white, Color.black, Consts.MATCH_ROTATE_TIME)
//                 .setDelay(0.05f)
//                 .setEase(LeanTweenType.easeOutSine)
//                 .setOnUpdate((Color col) =>
//                {
//                    coloredPipe2.SymbolSprites[0].color = col;
//                    coloredPipe2.SymbolSprites[1].color = col;
//                })
//                 .setOnComplete(() =>
//               {
//                    coloredPipe2.SymbolSprites[0].color = Color.white;
//                    coloredPipe2.SymbolSprites[1].color = Color.white;
//               });
        

		if (Consts.BUMP_ON_MATCH)
		{
			Bump(slideData);
		}
    }

	private void OnPipeLandAfterMatch(SlideData slideData)
	{
		MusicManager.playSound("chip_hit");
		slideData.Pipe2.RemoveCombineAnimation();
		EventData eventData = new EventData("OnCombineWasMadeEvent");
		eventData.Data["acolor"] = slideData.Pipe.AColor;
		eventData.Data["double"] = slideData.Slot2.IsDoubleSlot;
		eventData.Data["param"] = slideData.Pipe.Param;
		slideData.Slot2.SetPipe(slideData.Pipe);
		if (slideData.Pipe.Param == _maxColoredLevels - 1 && Consts.MAX_COLORED_LEVEL_REMOVES)
		{
            // reached max pipe             
            BreakePipeInSlot(slideData.Slot2, (slideData.Pipe as Pipe_Colored).GetExplodeEffectPrefab()); //BreakeEffectPrefab);
			if (GameType == EGameType.Leveled)
			{
				GameManager.Instance.BoardData.StarsGained += Consts.STAR_PROGRESS;
			}
            else
            //if (GameType == EGameType.Classic)
            {
                if (Consts.BAD_PIXEL_MACHANIC_IN_CLASSIC_GAME)
                {
                    SPipe bPipe = GetPipeFromPool(EPipeType.Hole).GetComponent<SPipe>();
                    bPipe.InitPipe(0, -1, false);
                    slideData.Slot2.SetPipe(bPipe);
                    bPipe.PlayAddAnimation();
                }
            }
            EventData eventData2 = new EventData("OnReachMaxPipeLevelEvent");
            eventData2.Data["x"] = slideData.Pipe.transform.position.x;
            eventData2.Data["y"] = slideData.Pipe.transform.position.y;
            GameManager.Instance.EventManager.CallOnReachMaxPipeLevelEvent(eventData2);



            //			Vector3 pos = ConvertPositionFromLocalToScreenSpace(slideData.Pipe.transform.position);
            //			EventData e = new EventData("OnShowAddResourceEffect");
            //			e.Data["x"] = pos.x;
            //			e.Data["y"] = pos.y;
            //			e.Data["screenpos"] = pos;
            //
        } else
		{
			//GameObject effect = (GameObject)GameObject.Instantiate(MatchEffectPrefab, Vector3.zero, Quaternion.identity);
			//effect.transform.SetParent(SlotsContainer, false);
			//Vector3 pos = slideData.Slot2.transform.position;
			//pos.z = PipeZ + 0.05f;
			//effect.transform.position = pos;
			//GameObject.Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
		}
		GameManager.Instance.EventManager.CallOnCombineWasMadeEvent(eventData);
		//
		slideData.Slot2.WaitForPipe = false;
		GameManager.Instance.BoardData.OnTurnWasMade(true, false);
	}

	private void OnPipeArrivedToSlotWithoutMatch(SlideData slideData)
	{
		// see how react with new slot
		if (slideData.SlideType == ESlideType.Move)
		{
			slideData.Slot2.SetPipe(slideData.Pipe);
			// bump
			Bump(slideData);
		} else
		//if (slideData.SlideType == ESlideType.Consum)
		{
			//if (slideData.Pipe2.IsCanConsumeColoredPipes())
			//{
				// play consum animation
				slideData.Pipe.BaseConsumAnimation(slideData.Pipe.Param, slideData.Pipe.AColor);
				slideData.Pipe.RemoveConsumAnimation();
			//}
		}
		slideData.Slot2.WaitForPipe = false;
		GameManager.Instance.BoardData.OnTurnWasMade(false, false);
	}

	private void Bump(SlideData slideData)
	{
		if (slideData.DistX != 0)
		{
			// horizontal bump
			float horizontalBump = slideData.DirX * Consts.BUMP_PER_SLOT * slideData.DistX + Consts.BUMP_EXTRA * slideData.DirX;
			BumpCameraHorizontal(horizontalBump, Consts.BUMP_TIME);
		} else
		{
			// vertical bump
			float verticalBump = slideData.DirY * Consts.BUMP_PER_SLOT * slideData.DistY + Consts.BUMP_EXTRA * slideData.DirY;
			BumpCameraVertical(verticalBump, Consts.BUMP_TIME);
		}
	}
	
//	protected void DragedPipeToFinger()
//    {
//        Vector3 pos = m_drag.transform.position;
//        pos.x = m_dragDxDy.x + m_downGamePos.x;
//        pos.y = m_dragDxDy.y + m_downGamePos.y;
//        m_drag.transform.position = pos;
//    }
	
	public void ShowSelection(Vector2 pos)
    {
        Selection.SetActive(true);
		Helpers.SetXY(Selection, pos);
    }

    public void HideSelection()
    {
		Selection.SetActive(false);
    }

	private SSlot TryGetSlot(int x, int y)
	{
		if (x < 0 || y < 0 || x >= WIDTH || y >= HEIGHT)
		{
			return null;
		} else
		{
			return Slots[x, y];
		}
	}

	private List<SSlot> GetEmptySSlotsNearPos(int x, int y)
	{
		List<SSlot> res = new List<SSlot>();
		// from left
		SSlot slot0 = TryGetSlot(x - 1, y);
		if (slot0.IsEmpty()) { res.Add(slot0); }
		// from right
		SSlot slot1 = TryGetSlot(x + 1, y);
		if (slot1.IsEmpty()) { res.Add(slot1); }
		// from bottom
		SSlot slot2 = TryGetSlot(x, y - 1);
		if (slot2.IsEmpty()) { res.Add(slot2); }
		// from top
		SSlot slot3 = TryGetSlot(x, y + 1);
		if (slot3.IsEmpty()) { res.Add(slot3); }
		return res;
	}

	public int GeneratePipesNearSlot(int x, int y)
	{
		List<SSlot> slots = GetEmptySSlotsNearPos(x, y);
		for (int i = 0; i < slots.Count; ++i)
		{
			// add colored pipe to slot
			int color = GameManager.Instance.BoardData.GetRandomColor();
			SPipe cPipe = GetPipeFromPool(EPipeType.Colored, color).GetComponent<SPipe>();
			cPipe.InitPipe(0, color, false);
			slots[i].SetPipe(cPipe);
			cPipe.PlayAddAnimation();
		}
		return slots.Count;
	}

    //without queue
    //public void AddRandomPipe(bool needBlocker)
    //{
    //	List<SSlot> slots = GetEmptySSlots();
    //	if (slots.Count == 0)
    //	{
    //           Debug.LogError("NO FREE SLOT!");
    //       } else
    //	{
    //		SSlot slot = slots[UnityEngine.Random.Range(0, slots.Count)];
    //           if (needBlocker)
    //           {
    //               // add blocker
    //               SPipe bPipe = GetPipeFromPool(EPipeType.Blocker).GetComponent<SPipe>();
    //               bPipe.InitPipe(0, -1, false);
    //               slot.SetPipe(bPipe);
    //               bPipe.PlayAddAnimation();
    //           }
    //           else
    //           {
    //			// add colored pipe to slot
    //			SPipe cPipe = GetPipeFromPool(EPipeType.Colored).GetComponent<SPipe>();
    //			cPipe.InitPipe(0, GameManager.Instance.BoardData.GetRandomColor(), false);
    //			slot.SetPipe(cPipe);
    //			cPipe.PlayAddAnimation();
    //		}
    //	}
    //       if (slots.Count <= 1)
    //       {
    //           bool movesExists = CheckIfCanMatchSomethingInTheEnd();
    //           if (!movesExists)
    //           {
    //               //TODO we loose, send signal
    //               Debug.LogError("We Loose! Send Signal!");
    //               SetGameState(EGameState.Loose);
    //               MusicManager.playSound("Fart_1");
    //               Invoke("ToMainMenu", 4.0f);
    //           }
    //       }
    //   }

    public bool AddRandomPipe(EPipeType newPipeType)
    {
        List<SSlot> slots = GetEmptySSlots();
        if (slots.Count == 0)
        {
            Debug.LogError("NO FREE SLOT!");
            return false;
        }
        else
        {
            // get info about first pipe from queue
            EPipeType pipeType = AQueuePanel.GetNextType();
            int acolor = AQueuePanel.GetNextColor();
            // add new pipe to queue
            if (newPipeType == EPipeType.Blocker)
            {
                AQueuePanel.MoveQueue(newPipeType, -1, 0);
            } else
            //if (newPipeType == EPipeType.Colored)
            {
                AQueuePanel.MoveQueue(newPipeType, GameManager.Instance.BoardData.GetRandomColor(), 0);
            }
            //
            SSlot slot = slots[UnityEngine.Random.Range(0, slots.Count)];
            if (pipeType == EPipeType.Blocker)
            {
                // add blocker
                SPipe bPipe = GetPipeFromPool(EPipeType.Blocker).GetComponent<SPipe>();
                bPipe.InitPipe(0, -1, false);
                slot.SetPipe(bPipe);
                bPipe.PlayAddAnimation();
            } else
            //if (pipeType == EPipeType.Colored)
            {
                // add colored pipe to slot
				SPipe cPipe = GetPipeFromPool(EPipeType.Colored, acolor).GetComponent<SPipe>();
                cPipe.InitPipe(0, acolor, false);
                slot.SetPipe(cPipe);
                cPipe.PlayAddAnimation();
            }
        }
        CheckIfLoose();
        return true;
    }

    public void ToMainMenu()
    {
        GameManager.Instance.GameFlow.TransitToScene(UIConsts.SCENE_ID.MAINMENU);
    }

    private bool CheckIfCanMatchSomethingInTheEnd()
    {
        // check if can match something when all board filled with pipes
        for (int i = 0; i < WIDTH; ++i)
        {
            for (int j = 0; j < HEIGHT; ++j)
            {
                SPipe pipe = Slots[i, j].Pipe;
                if (pipe.PipeType == EPipeType.Colored)
                {
                    // check right
                    int ii = i + 1;
                    if (ii < WIDTH)
                    {
                        SPipe pipe2 = Slots[ii, j].Pipe;
                        if (pipe2.PipeType == EPipeType.Colored)
                        {
                            if (pipe.AColor == pipe2.AColor && pipe.Param == pipe2.Param && pipe.Param < _maxColoredLevels)
                            {
                                Debug.Log("You can match ---> " + pipe2.X + " / " + pipe2.Y + "  ...  " + +pipe.X + " / " + pipe.Y);
                                return true;
                            }
                        }
                    }
                    // check top
                    int jj = j + 1;
                    if (jj < HEIGHT)
                    {
                        SPipe pipe2 = Slots[i, jj].Pipe;
                        if (pipe2.PipeType == EPipeType.Colored)
                        {
                            if (pipe.AColor == pipe2.AColor && pipe.Param == pipe2.Param && pipe.Param < _maxColoredLevels)
                            {
                                Debug.Log("You can match ---> " + pipe2.X + " / " + pipe2.Y + "  ...  " + +pipe.X + " / " + pipe.Y);
                                return true;
                            }
                        }
                    }
                }

            }
        }
        // check powerups
        if (GameBoard.GameType == EGameType.Classic)
        {
            for (var powerup = GameData.PowerUpType.Reshuffle; powerup <= GameData.PowerUpType.DestroyColor; ++powerup)
            {
                if (GameManager.Instance.BoardData.PowerUps[powerup] > 0) // && GameManager.Instance.Player.PowerUpsState.ContainsKey(powerup) && GameManager.Instance.Player.PowerUpsState[powerup].Level > 0)
                {
                    // show notification
                    EventData eventData = new EventData("OnShowNotificationEvent");
                    eventData.Data["type"] = GameNotification.NotifyType.UsePowerup;
                    GameManager.Instance.EventManager.CallOnShowNotificationEvent(eventData);
                    return true;
                }
            }
        }
        return false;
    }

    public List<SSlot> GetEmptySSlots()
    {
        List<SSlot> slots = new List<SSlot>();
        for (int i = 0; i < WIDTH; ++i)
        {
            for (int j = 0; j < HEIGHT; ++j)
            {
                if (Slots[i, j].IsEmpty())
                {
                    slots.Add(Slots[i, j]);
                }
            }
        }
        return slots;
    }

    public void BumpCameraHorizontal(float xpower, float time)
	{
        if (!GameManager.Instance.Player.Bump)
        {
            return;
        }
        // shake it
        MusicManager.playSound("chip_hit");
        LeanTween.cancel(BumpShakeObject);
		LeanTween.value(BumpShakeObject, 0.0f, 1.0f, time)
			.setLoopPingPong()
			.setLoopCount(2)
		//.setEase(UIConsts.SHOW_EASE)
		//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setOnUpdate
			(
				(float val)=>
				{
					_shakeDx = val * xpower;
				}
			).setOnComplete
			(
				() =>
				{
					_shakeDx = 0;
					_shakeDy = 0;
				}
			);
	}

	public void BumpCameraVertical(float ypower, float time)
	{
        if (!GameManager.Instance.Player.Bump)
        {
            return;
        }
        // shake it
        MusicManager.playSound("chip_hit");
        LeanTween.cancel(BumpShakeObject);
		LeanTween.value(BumpShakeObject, 0.0f, 1.0f, time)
			.setLoopPingPong()
			.setLoopCount(2)
			//.setEase(UIConsts.SHOW_EASE)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setOnUpdate
			(
				(float val)=>
				{
					_shakeDy = val * ypower;
				}
			).setOnComplete
			(
				() =>
				{
					_shakeDx = 0;
					_shakeDy = 0;
				}
			);
	}

	public void ShakeCamera(float xpower, float ypower, float time)
	{
        if (!GameManager.Instance.Player.Shake)
        {
            return;
        }
        // shake it
        float doublePowerX = xpower * 2;
		float doublePowerY = ypower * 2;
		LeanTween.cancel(BumpShakeObject);
		LeanTween.value(BumpShakeObject, 0, 1, time)
			//.setEase(UIConsts.SHOW_EASE)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setOnUpdate
				(
					(float val)=>
					{
						if (xpower > 0)
						{
							_shakeDx = UnityEngine.Random.Range(0, doublePowerX) - xpower;
						}
						if (ypower > 0)
						{
							_shakeDy = UnityEngine.Random.Range(0, doublePowerY) - ypower;
						}
					}
				).setOnComplete
				(
				() =>
				{
					_shakeDx = 0;
					_shakeDy = 0;
				}
			);
	}
		
	public void OnPowerUpClicked(GameData.PowerUpType type)
	{
        if (!IsPlay())
        {
            return;
        }
		ResetHint();
		if (type == GameData.PowerUpType.Reshuffle)
        {
            // reshuffle
            PowerUp_Reshuffle();
            return;
        } else
		if (type == GameData.PowerUpType.Breake)
        {
            // break
            PowerUp_Breake();
            return;
        } else
		if (type == GameData.PowerUpType.Chain)
        {
            // chain booster
            PowerUp_Chain();
        } else
        if (type == GameData.PowerUpType.DestroyColor)
        {
            // chain booster
            PowerUp_DestroyColor();
        }



        //List<SSlot> slots = new List<SSlot>();
        //for (int i = 0; i < WIDTH; ++i)
        //{
        //    for (int j = 0; j < HEIGHT; ++j)
        //    {
        //        SSlot slot = Slots[i, j];
        //        SPipe pipe = slot.Pipe;
        //        if (pipe != null) // && pipe.PipeType == EPipeType.Blocker)
        //        {
        //            slots.Add(slot);
        //        }
        //    }
        //}
        //if (slots.Count > 1) // > 0 if for blockers only!!!
        //{
        //    PowerUpsButtons[button].Disable();
        //    //Vector3 startEffectPos = new Vector3(7, 7, -6); //BoosterButtons[button].transform.position;
        //                                                    //startEffectPos.z = -6;
        //    int busterPower = Mathf.Min(GameManager.Instance.Player.BoosterLevel * Consts.PU__POWER_PER_LEVEL_RESHUFFLE, slots.Count);
        //    busterPower = Mathf.Min(slots.Count - 1, busterPower); //this for destroing not blockers version!!!!
        //    slots = Helpers.ShuffleList(slots);
        //    for (int i = 0; i < busterPower; ++i)
        //    {
        //        BreakePipeInSlot(slots[i]); //, startEffectPos);
        //    }
        //}
    }

	private void BreakePipeInSlot(SSlot slot, GameObject prefab) //, Vector3 startEffectPos)
	{
        MusicManager.playSound("chip_destroy");
        SPipe pipe = slot.TakePipe();
        // explosion effect
        GameObject effect = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, prefab.transform.rotation);
        effect.transform.SetParent(SlotsContainer, false);
        effect.transform.position = slot.transform.position;
        effect.SetActive(true);
        GameObject.Destroy(effect, 3.0f);
        //
        pipe.RemoveConsumAnimation();
	}

    private void PowerUp_Reshuffle()
    {
        SetGameState(EGameState.Pause);
        List<SSlot> slots = new List<SSlot>();
        for (int i = 0; i < WIDTH; ++i)
        {
            for (int j = 0; j < HEIGHT; ++j)
            {
                SSlot slot = Slots[i, j];
                SPipe pipe = slot.Pipe;
                if (pipe != null)
                {
                    slots.Add(slot);
                }
            }
        }
        if (slots.Count > 0)
        {
            //int boosterPower = GameManager.Instance.Player.BoosterLevel * Consts.PU__POWER_PER_LEVEL_RESHUFFLE;
            //boosterPower = Mathf.Min(slots.Count, boosterPower);
            int boosterPower = slots.Count;
            // take pipes from slots
            slots = Helpers.ShuffleList(slots);
            List<SPipe> pipes = new List<SPipe>();
            for (int i = 0; i < boosterPower; ++i)
            {
                pipes.Add(slots[i].TakePipe());
            }
            // find free slots
            List<SSlot> freeSlots = GetEmptySSlots();
            // randomly move pipes to slots
            float maxTime = 0;
            for (int i = 0; i < boosterPower; ++i)
            {
                // add to new slot
                SPipe pipe = pipes[i];
                int randI = UnityEngine.Random.Range(0, freeSlots.Count);
                SSlot slot = freeSlots[randI];

                if (slot.X == pipe.X && slot.Y == pipe.Y && freeSlots.Count > 1)
                {
                    // we must change slot
                    --i;
                    continue;
                }

                freeSlots.RemoveAt(randI);
                // find distance
                float dx = pipe.X - slot.X;
                float dy = pipe.Y - slot.Y;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                // find move time, check max time
                float moveTime = distance * Consts.PU__RESHUFFLE_TIME_PER_SLOT;
                if (moveTime > maxTime)
                {
                    maxTime = moveTime;
                }
                //
                slot.SetPipe(pipe, false);
                // move upper
                Vector3 oldPos = pipe.transform.position;
                oldPos.z = PipeZ - 1.0f;
                pipe.transform.position = oldPos;
                // fly to new slot
                GameObject pipeObj = pipe.gameObject;
                LeanTween.cancel(pipeObj);
                Vector3 newPos = slot.transform.position;
                newPos.z = PipeZ;
                LeanTween.move(pipeObj, newPos, moveTime)
                    //.setDelay(i * 0.01f)
                    .setEase(LeanTweenType.easeInOutSine);
            }
            MusicManager.playSound("reshuffle");
            pipes.Clear();
            freeSlots.Clear();
			int current = GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Reshuffle];
			--current;
			GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Reshuffle] = current;

			//
			EventData eventData = new EventData("OnPowerUpUsedEvent");
			eventData.Data["type"] = GameData.PowerUpType.Reshuffle;
			GameManager.Instance.EventManager.CallOnPowerUpUsedEvent(eventData);
			//
			ChainPowerup = false;
			BreakePowerup = false;
            DestroyColorPowerup = false;
            //
            Invoke("CheckIfLoose", maxTime);
        }
        else
        {
            //TODO wrong sound, nothing happens
        }
    }

    private void PowerUp_Breake()
    {
        if (BreakePowerup)
        {
            BreakePowerup = false;
        }
        else
        {
            BreakePowerup = true;
        }
		ChainPowerup = false;
        DestroyColorPowerup = false;
    }

    public void OnBreakePowerupUsed(SSlot slot)
    {
		BreakePowerup = false;
        BreakePipeInSlot(slot, (slot.Pipe as Pipe_Colored).GetExplodeEffectPrefab()); //BreakePipeInSlot(slot, BreakeEffectPrefab);
        int current = GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Breake];
        --current;
        GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Breake] = current;
		//
		EventData eventData = new EventData("OnPowerUpUsedEvent");
		eventData.Data["type"] = GameData.PowerUpType.Breake;
		GameManager.Instance.EventManager.CallOnPowerUpUsedEvent(eventData);
		//
        
        // if no pipes left - add new pipe on board without move counting
        if (GetMovablePipesCount() == 0)
        {
            GameManager.Instance.BoardData.OnTurnWasMade(false, true);
        }
        //
    }

    private void PowerUp_Chain()
    {
        if (ChainPowerup)
        {
            ChainPowerup = false;
        }
        else
        {
            ChainPowerup = true;
        }
		BreakePowerup = false;
        DestroyColorPowerup = false;
    }

    public void OnChainPowerupUsed(SSlot slot)
    {
		ChainPowerup = false;
        SetGameState(EGameState.Pause);
        int current = GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Chain];
        --current;
        GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Chain] = current;

		//
		EventData eventData = new EventData("OnPowerUpUsedEvent");
		eventData.Data["type"] = GameData.PowerUpType.Chain;
		GameManager.Instance.EventManager.CallOnPowerUpUsedEvent(eventData);
		//

        _chainInfos.Clear();
        _checkedChainInfos.Clear();
        ChainInfo info = new ChainInfo();
        info.X = slot.X;
        info.Y = slot.Y;
        info.Color = slot.Pipe.AColor;
        info.Param = slot.Pipe.Param;
        info.PipeType = slot.Pipe.PipeType;
        info.Id = info.X * 1000 + info.Y;
        _chainInfos.Add(info.Id, info);
        RemoveChainIteration();
    }

    private void ChainDestroyPipeAtInfo(ChainInfo info)
    {
        SSlot slot = Slots[info.X, info.Y];
        BreakePipeInSlot(slot, ChainEffectPrefab);
    }

    private void RemoveChainIteration()
    {
        List<int> dxs = new List<int>() { -1, 0, 1, 0 };
        List<int> dys = new List<int>() { 0, 1, 0, -1 };

        List<ChainInfo> tempList = new List<ChainInfo>(); 
        foreach (var chInfo in _chainInfos)
        {
            ChainInfo info = chInfo.Value;
            // breake it
            ChainDestroyPipeAtInfo(info);
            tempList.Add(info);
            _checkedChainInfos.Add(info.Id, info);
        }
        // try add next wave (neighbours)
        _chainInfos.Clear();
        foreach (var info in tempList)
        {
            for (int i = 0; i < 4; ++i)
            {
                int ax = info.X + dxs[i];
                int ay = info.Y + dys[i];
                int akey = ax * 1000 + ay;
                if (!_checkedChainInfos.ContainsKey(akey) && !_chainInfos.ContainsKey(akey))
                {
                    if (IsSlotInBoard(ax, ay))
                    {
                        SPipe pipe = Slots[ax, ay].Pipe;
                        if ((pipe != null) && (info.PipeType == pipe.PipeType) && (info.PipeType == EPipeType.Blocker || info.Color == pipe.AColor || (info.Param == pipe.Param)))
                        {
                            ChainInfo newInfo = new ChainInfo();
                            newInfo.X = ax;
                            newInfo.Y = ay;
                            newInfo.Color = pipe.AColor;
                            newInfo.Param = pipe.Param;
                            newInfo.PipeType = pipe.PipeType;
                            newInfo.Id = akey;
                            _chainInfos.Add(newInfo.Id, newInfo);
                        }
                    }
                }   
            }
        }
        tempList.Clear();

        if (_chainInfos.Count == 0)
        {
            _checkedChainInfos.Clear();
            // if no pipes left - add new pipe on board without move counting
            if (GetMovablePipesCount() == 0)
            {
                GameManager.Instance.BoardData.OnTurnWasMade(false, true);
            }
			UnsetPause();
        } else
        {
            Invoke("RemoveChainIteration", Consts.PU__CHAIN_TIME_PER_ITERATION);
        }
    }

    private void PowerUp_DestroyColor()
    {
        if (DestroyColorPowerup)
        {
            DestroyColorPowerup = false;
        }
        else
        {
            DestroyColorPowerup = true;
        }
        BreakePowerup = false;
        ChainPowerup = false;
    }

    public void OnDestroyColorPowerupUsed(SSlot slot)
    {
        DestroyColorPowerup = false;
        SetGameState(EGameState.Pause);
        // find all pipes with this color
        int colorToDestroy = slot.Pipe.AColor;
        List<SSlot> slots = new List<SSlot>();
        slots.Add(slot);
        for (int i = 0; i < WIDTH; ++i)
        {
            for (int j = 0; j < HEIGHT; ++j)
            {
                SSlot aslot = Slots[i, j];
                SPipe apipe = aslot.Pipe;
                if (apipe != null && apipe.AColor == colorToDestroy && aslot != slot)
                {
                    slots.Add(aslot);
                }
            }
        }
        // destroy this pipes
        for (int i = 0; i < slots.Count; ++i)
        {
            SSlot dSlot = slots[i];
            LeanTween.delayedCall(dSlot.gameObject, i * Consts.PU__DESTROY_COLOR_TIME_PER_ITERATION, () => { BreakePipeInSlot(dSlot, (dSlot.Pipe as Pipe_Colored).GetExplodeEffectPrefab());  }); //BreakePipeInSlot(dSlot, BreakeEffectPrefab);
        }
        LeanTween.delayedCall(gameObject, slots.Count * Consts.PU__DESTROY_COLOR_TIME_PER_ITERATION, () => 
            {
                // if no pipes left - add new pipe on board without move counting
                if (GetMovablePipesCount() == 0)
                {
                    GameManager.Instance.BoardData.OnTurnWasMade(false, true);
                }
                //
                UnsetPause();
            });
        slots.Clear();
        //
        int current = GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.DestroyColor];
        --current;
        GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.DestroyColor] = current;
        //
        EventData eventData = new EventData("OnPowerUpUsedEvent");
        eventData.Data["type"] = GameData.PowerUpType.DestroyColor;
        GameManager.Instance.EventManager.CallOnPowerUpUsedEvent(eventData);
    }

    public bool IsPlay()
    {
        return GameManager.Instance.BoardData.GameState == EGameState.Play;
    }

    public int GetPipesCount()
    {
        int res = 0;
        for (int i = 0; i < WIDTH; ++i)
        {
            for (int j = 0; j < HEIGHT; ++j)
            {
                SSlot slot = Slots[i, j];
                SPipe pipe = slot.Pipe;
                if (pipe != null)
                {
                    ++res;
                }
            }
        }
        return res;
    }

    public int GetMovablePipesCount()
    {
        int res = 0;
        for (int i = 0; i < WIDTH; ++i)
        {
            for (int j = 0; j < HEIGHT; ++j)
            {
                SSlot slot = Slots[i, j];
                SPipe pipe = slot.Pipe;
                if (pipe != null && slot.IsMovable() && pipe.IsMovable())
                {
                    ++res;
                }
            }
        }
        return res;
    }

    public bool IsMoreThenOnePipeLeft()
    {
        int count = 0;
        for (int i = 0; i < WIDTH; ++i)
        {
            for (int j = 0; j < HEIGHT; ++j)
            {
                SSlot slot = Slots[i, j];
                SPipe pipe = slot.Pipe;
                if (pipe != null)
                {
                    ++count;
                    if (count == 2)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void UpdateSkin()
    {
        for (int i = 0; i < WIDTH; ++i)
        {
            for (int j = 0; j < HEIGHT; ++j)
            {
                SPipe pipe = Slots[i, j].Pipe;
                if (pipe != null)
                {
					if (pipe.PipeType == EPipeType.Hole)
					{
						Slots[i, j].SetAsHole();
					} else
					{
                    	pipe.UpdateSkin();
					}
                }
            }
        }

        if (ASequencePanel != null)
        {
            ASequencePanel.UpdateSkins();
        }
        AQueuePanel.UpdateSkins();
    }
		

	public void RestartGame()
	{
		if (GameType == EGameType.Leveled)
		{
			PlayLeveledGame();
		} else
		{
			PlayGame();
		}
    }

	public void GoHome()
	{
        SaveGame();
		//MusicManager.PlayMainMenuTrack();
		GameManager.Instance.CurrentMenu = UISetType.MainMenu;
		EventData eventData = new EventData("OnUISwitchNeededEvent");
		eventData.Data["setid"] = UISetType.MainMenu;
		GameManager.Instance.EventManager.CallOnUISwitchNeededEvent(eventData);
		GameMenuUIController.UpdatePlayEndlessButton();
	}

    public void SaveGame()
    {
		if (GameType == EGameType.Classic)
		{
			if (GameManager.Instance.BoardData.GameState != EGameState.Loose)
		  	{
		  	    GameManager.Instance.Player.SaveLastGame(GetLevelToSave());
		  	}
		  	else
		  	{
		  	    GameManager.Instance.Player.SavedGame = null;
		  	}
		}
    }

    private void CheckIfLoose()
    {
        List<SSlot> slots = GetEmptySSlots();
        if (slots.Count == 0)
        {
            bool movesExists = CheckIfCanMatchSomethingInTheEnd();
            if (!movesExists)
            {
                OnLoose();
                return;
            }
        }
        UnsetPause();
    }

    public void OnLoose()
    {
        Debug.Log("You Loose!");
        MusicManager.playSound("level_lost");
        SetGameState(EGameState.Loose);
        GameManager.Instance.Player.SavedGame = null;
        LeanTween.delayedCall(1.0f, () => {
            EventData eventData = new EventData("OnOpenFormNeededEvent");
            eventData.Data["form"] = UIConsts.FORM_ID.STATISTIC_WINDOW;
            GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
        });
    }

	private List<MatchHintData> FindPossibleHints(bool withoutMatchToo)
	{
		List<MatchHintData> possibleHints = new List<MatchHintData>();
		for (int i = 0; i < WIDTH; ++i)
		{
			for (int j = 0; j < HEIGHT; ++j)
			{
				int xB = -1;
				int yB = -1;
				SPipe pipe = Slots[i, j].Pipe;
				if (pipe != null && pipe.IsMovable())
				{
					// try slide left
					xB = i;
					for (int xx = i - 1; xx >= 0; --xx)
					{
						SPipe pipeB = Slots[xx, j].Pipe;
						if (pipeB != null)
						{
							if (!CheckPipesforHint(pipe, pipeB, i, j, xx, j, ref possibleHints))
							{
								xB = i;
							}
							break;
						} else
						{
							xB = xx;
						}
					}
					if (withoutMatchToo && xB != i)
					{
						AddPossibleHintWithoutMatch(i, j, xB, j, ref possibleHints);
					}
					// try slide right
					xB = i;
					for (int xx = i + 1; xx < WIDTH; ++xx)
					{
						SPipe pipeB = Slots[xx, j].Pipe;
						if (pipeB != null)
						{
							if (!CheckPipesforHint(pipe, pipeB, i, j, xx, j, ref possibleHints))
							{
								xB = i;
							}
							break;
						} else
						{
							xB = xx;
						}
					}
					if (withoutMatchToo && xB != i)
					{
						AddPossibleHintWithoutMatch(i, j, xB, j, ref possibleHints);
					}
					// try slide up
					yB = j;
					for (int yy = j + 1; yy < HEIGHT; ++yy)
					{
						SPipe pipeB = Slots[i, yy].Pipe;
						if (pipeB != null)
						{
							if (!CheckPipesforHint(pipe, pipeB, i, j, i, yy, ref possibleHints))
							{
								yB = j;
							}
							break;
						} else
						{
							yB = yy;
						}
					}
					if (withoutMatchToo && yB != j)
					{
						AddPossibleHintWithoutMatch(i, j, i, yB, ref possibleHints);
					}
					//try slide down
					yB = j;
					for (int yy = j - 1; yy >= 0; --yy)
					{
						SPipe pipeB = Slots[i, yy].Pipe;
						if (pipeB != null)
						{
							if (!CheckPipesforHint(pipe, pipeB, i, j, i, yy, ref possibleHints))
							{
								yB = j;
							}
							break;
						} else
						{
							yB = yy;
						}
					}
					if (withoutMatchToo && yB != j)
					{
						AddPossibleHintWithoutMatch(i, j, i, yB, ref possibleHints);
					}
				}
			}
		}
		return possibleHints;
	}

	private bool CheckPipesforHint(SPipe pipeA, SPipe pipeB, int xA, int yA, int xB, int yB, ref List<MatchHintData> possibleHints)
	{
		if (pipeA.AColor == pipeB.AColor && pipeA.Param == pipeB.Param && pipeA.Param < _maxColoredLevels)
		{
			MatchHintData mhData;
			mhData.XA = xA;
			mhData.YA = yA;
			mhData.XB = xB;
			mhData.YB = yB;
            mhData.IsMatch = true;
			possibleHints.Add(mhData);
			return true;
		}
		return false;
	}

	private void AddPossibleHintWithoutMatch(int xA, int yA, int xB, int yB, ref List<MatchHintData> possibleHints)
	{
		MatchHintData mhData;
		mhData.XA = xA;
		mhData.YA = yA;
		mhData.XB = xB;
		mhData.YB = yB;
		mhData.IsMatch = false;
		possibleHints.Add(mhData);
	}

	private void TryShowHint()
	{
        if (GameType != EGameType.Classic || GameManager.Instance.GameFlow.IsSomeWindow())
        {
            return;
        }
        if (_hintTimer <= 0)
        {
            return;
        }
		_hintTimer -= Time.deltaTime;
		if (_hintTimer <= 0)
		{
			//ResetHint(true); // to show random hints
			List<MatchHintData> mhData = FindPossibleHints(false);
			if (mhData.Count > 0)
			{
				//show random hint
				MatchHintData data = mhData[UnityEngine.Random.Range(0, mhData.Count)];
				GameObject obj = (GameObject)GameObject.Instantiate(HintPrefab, Vector3.zero, Quaternion.identity);
				_hint = obj.GetComponent<NewHintScript>();
				_hint.ShowHint(data, this);
			}
		}
	}


    //TUTOR_2
    private void TryShowTutor2()
	{
        if (_tutor2Timer <= 0 || GameManager.Instance.GameFlow.IsSomeWindow())
        {
            return;
        }
        _tutor2Timer -= Time.deltaTime;
        if (_tutor2Timer <= 0)
        {
            if (GameBoard.GameType == EGameType.Classic && !GameManager.Instance.Player.IsTutorialShowed("2"))
            {
                for (var powerup = GameData.PowerUpType.Reshuffle; powerup <= GameData.PowerUpType.DestroyColor; ++powerup)
                {
                    if (GameManager.Instance.BoardData.PowerUps[powerup] > 0) // && GameManager.Instance.Player.PowerUpsState.ContainsKey(powerup) && GameManager.Instance.Player.PowerUpsState[powerup].Level > 0)
                    {
                        GameManager.Instance.ShowTutorial("2", new Vector3(0, 0, 0));
                        return;
                    }
                }
            }
        }
	}

	private void ResetHint(bool resetByTime = false)
	{
        if (GameType != EGameType.Classic)
        {
            return;
        }
		if (_hint != null)
		{
            HideHint();
            if (resetByTime)
			{
				_hintTimer = Consts.HINT_DELAY / 3.0f;
			} else
			{
				_hintTimer = Consts.HINT_DELAY;
			}
		} else
		{
			_hintTimer = Consts.HINT_DELAY;
		}
	}

    //TUTOR_2
    private void ResetTutor2Timer()
    {
        if (GameType != EGameType.Leveled)
        {
            return;
        }
        if (!GameManager.Instance.Player.IsTutorialShowed("2"))
        {
            _tutor2Timer = Consts.TUTOR_2_DELAY;
        }
    }

    public Vector3 ConvertPositionFromLocalToScreenSpace(Vector3 localPos)
    {
        Vector3 screenPos = _camera.WorldToViewportPoint(localPos);
        RectTransform canvasRect = ACanvas.GetComponent<RectTransform>();
        screenPos.x *= canvasRect.rect.width;
        screenPos.y *= canvasRect.rect.height;
        screenPos.x -= canvasRect.rect.width * canvasRect.pivot.x;
        screenPos.y -= canvasRect.rect.height * canvasRect.pivot.y;
        return screenPos;
    }

	public Sprite GetSprite(string id)
	{
		Sprite res = null;
		if (_sprites.TryGetValue(id, out res))
		{
			return res;
		}
        string path = "art\\Pipes\\" + id;
        res = Resources.Load<Sprite>(path);
		_sprites.Add(id, res);
		return res;
	}

	public void OnLeveledGameCompleted()
	{
		SetGameState(EGameState.Loose);
        LeanTween.delayedCall(1.0f, () =>
        {
            EventData eventData = new EventData("OnOpenFormNeededEvent");
            eventData.Data["form"] = UIConsts.FORM_ID.LEVELED_STATISTIC_WINDOW;
            GameManager.Instance.EventManager.CallOnOpenFormNeededEvent(eventData);
        });
	}

    private void TryStartStartTutorSequence()
    {
        _startSequenceState = 0;
        _startTutorHintData.XA = 0;
        _startTutorHintData.YA = 0;
        _startTutorHintData.XB = 0;
        _startTutorHintData.YB = 0;
        if (!GameManager.Instance.Player.IsTutorialShowed("start"))
        {
            GameManager.Instance.Player.SetTutorialShowed("start");
            ForceToSwipe();
        }
    }

    private void ForceToSwipe()
    {
        ++_startSequenceState;
        //TUTOR_5
        if (_startSequenceState >= 6)
        {
            _startSequenceState = 0;
            LeanTween.delayedCall(1.0f, ()=> { GameManager.Instance.ShowTutorial("5", new Vector3(0, 0, 0)); });
            return;
        }
        if (_startSequenceState == 1)
        {
            _startTutorHintData.XA = 4;
            _startTutorHintData.YA = 2;
            _startTutorHintData.XB = 1;
            _startTutorHintData.YB = 2;
        } else
        if (_startSequenceState == 2)
        {
            _startTutorHintData.XA = 1;
            _startTutorHintData.YA = 2;
            _startTutorHintData.XB = 1;
            _startTutorHintData.YB = 4;
        } else
        if (_startSequenceState == 3)
        {
            _startTutorHintData.XA = 1;
            _startTutorHintData.YA = 4;
            _startTutorHintData.XB = 4;
            _startTutorHintData.YB = 4;
        } else
        if (_startSequenceState == 4)
        {
            _startTutorHintData.XA = 4;
            _startTutorHintData.YA = 4;
            _startTutorHintData.XB = 4;
            _startTutorHintData.YB = 2;
        } else
        if (_startSequenceState == 5)
        {
            _startTutorHintData.XA = 4;
            _startTutorHintData.YA = 2;
            _startTutorHintData.XB = 0;
            _startTutorHintData.YB = 2;
        }
        // show hint
        ShowHint(_startTutorHintData);
    }

    private void ShowHint(MatchHintData mhData)
    {
        HideHint();
        GameObject obj = (GameObject)GameObject.Instantiate(HintPrefab, Vector3.zero, Quaternion.identity);
        _hint = obj.GetComponent<NewHintScript>();
        _hint.ShowHint(mhData, this);
    }

    private void HideHint()
    {
        if (_hint != null)
        {
            _hint.HideHint();
            _hint = null;
        }
    }

	protected void UpdateInput()
	{
		if (!Consts.IS_TOUCH_DEVICE && Input.touchCount > 0)
		{
			Consts.IS_TOUCH_DEVICE = true;
		}

		if (Consts.IS_TOUCH_DEVICE)
		{
            //use touches
            if (Input.touchCount == 0 && _currentTouchId != -1)
            {
                _currentTouchId = -1;
                GameManager.Instance.BoardData.DragSlot = null;
                GameManager.Instance.BoardData.AGameBoard.HideSelection();
            } else
            // only 1 touch - first touch (B real) for clicks on buttons and popups
            if (Input.touchCount > 0)
            {
                //GameObject uiObj = GameManager.Instance.GameFlow.GetFormFromID(UIConsts.FORM_ID.MAP_MAIN_MENU);
                //if (uiObj != null)
                //{
                //    BaseUIController ui = uiObj.GetComponent<BaseUIController>();
                //    if (ui != null)
                //    {
                //        if (ui.IsOver())
                //        {
                //            canClick = false;
                //        }
                //    }
                //}
                if (_currentTouchId == -1)
                {
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                        Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
                        // left down
                        //Debug.Log("left down");
                        _currentTouchId = Input.touches[0].fingerId;
                        LeftMouseDownByPosition(touchPosWorld2D);
                    }
                    else
                    if (Consts.START_SLIDE_ON_NO_MOUSE_DOWN)
                    {
                        Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                        Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
                        // left down
                        //Debug.Log("left over-down");
                        _currentTouchId = Input.touches[0].fingerId;
                        LeftMouseDownByPosition(touchPosWorld2D);
                    }
                }
                else
                {
                    foreach (var touch in Input.touches)
                    {
                        if (touch.fingerId == _currentTouchId)
                        {
                            Vector2 downGamePosNew2 = Camera.main.ScreenToWorldPoint(touch.position);
                            //if (touch.phase == TouchPhase.Moved)
                            //{
                            //    OnMouseMove(downGamePosNew2);
                            //}
                            //else
                            if (touch.phase == TouchPhase.Ended)
                            {
                                //Debug.Log("left up");
                                _currentTouchId = -1;
                                LeftMouseUpByPosition(downGamePosNew2);
                            }
                            else
                            if (GameManager.Instance.BoardData.DragSlot != null && Consts.SLIDE_WITHOUT_MOUSE_UP)
                            {
                                GameManager.Instance.BoardData.DragSlot.UpdateSlot(downGamePosNew2);
                            }
                            break;
                        }
                    }
                }
            }
			//MusicManager.playSound("horse");
			//MusicManager.playSound("unlock_upgrade");
		}
		else
		{
			// use mouse
			Vector2 downScreenPos = Input.mousePosition;
			Vector2 downGamePosNew = Camera.main.ScreenToWorldPoint(downScreenPos);
			if (Input.GetMouseButtonDown(0))
            {
                // left down
                //Debug.Log("over-left down");
                LeftMouseDownByPosition(downGamePosNew);
            } else
            if (Consts.START_SLIDE_ON_NO_MOUSE_DOWN && Input.GetMouseButton(0) && GameManager.Instance.BoardData.DragSlot == null)
            {
                // left down
                //Debug.Log("left down");
                LeftMouseDownByPosition(downGamePosNew);
            }
            else
            if (Input.GetMouseButtonUp(0))
            {
                // left up
                //Debug.Log("left up");
                LeftMouseUpByPosition(downGamePosNew);
            }
            else
            if (GameManager.Instance.BoardData.DragSlot != null && Consts.SLIDE_WITHOUT_MOUSE_UP)
            {
                GameManager.Instance.BoardData.DragSlot.UpdateSlot(downGamePosNew);
            }
		}
	}

	protected virtual void LeftMouseDownByPosition(Vector2 downGamePos)
	{
		if (GameManager.Instance.CurrentMenu != UISetType.ClassicGame && GameManager.Instance.CurrentMenu != UISetType.LeveledGame)
		{
			return;
		}
		if (GameManager.Instance.BoardData.GameState != EGameState.Play || GameManager.Instance.BoardData.DragSlot != null)
		{
			// can't drag
			return;
		}
		if (GameManager.Instance.GameFlow.IsSomeWindow())
		{
			return;
		}

		//if (Consts.IS_TOUCH_DEVICE)
		//{
		////We now raycast with this information. If we have hit something we can process it.
		//RaycastHit2D hitInformation = Physics2D.Raycast(downGamePos, Camera.main.transform.forward);
		//if (hitInformation.collider != null)
		//{
		//    SSlot touchObject = hitInformation.transform.GetComponent<SSlot>();
		//    if (touchObject != null)
		//    {
		//        //MusicManager.playSound("horse");
		//        touchObject.LeftMouseDownByPosition(downGamePos);
		//    }
		//}
		//}

		BoardPos slotPosIn = GameBoard.SlotPosIn(downGamePos);
		if (IsSlotInBoard(slotPosIn))
		{
			SSlot slot = GetSlot(slotPosIn);
			SPipe pipe = slot.Pipe;
			if (pipe)
			{
				slot.OnMouseDownByPosition(downGamePos);
			}
		}
        if (Consts.START_SLIDE_ON_NO_MOUSE_DOWN && GameManager.Instance.BoardData.DragSlot == null)
        {
            // we clicked on empty cell
            _currentTouchId = -1;
        }
	}

	protected virtual void LeftMouseUpByPosition(Vector2 downGamePos)
	{
		if (GameManager.Instance.CurrentMenu != UISetType.ClassicGame && GameManager.Instance.CurrentMenu != UISetType.LeveledGame)
		{
			return;
		}
		if (GameManager.Instance.BoardData.DragSlot != null)
		{
			GameManager.Instance.BoardData.DragSlot.OnMouseUpByPosition(downGamePos);
		}
	}

	public Material GetMaterialForColoredPipe(int acolor, int param)
	{
		return ColoredMaterials[acolor * Consts.CLASSIC_GAME_COLORS + param];
	}

}