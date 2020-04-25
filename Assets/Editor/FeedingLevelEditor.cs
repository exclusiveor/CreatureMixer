using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

//=====================================================================================
// Zanuda Games - Kozhemyakin Vitaliy, Mykhailo Zhovnach, Taras Lishchuk
//=====================================================================================

[CustomEditor(typeof(ScriptableLevelData))]
public class FeedingLevelEditor : Editor
{
	public const int 				FLAG_TO_GENERATE_LEVEL = -7;
	private const int               PIPE_TYPE_HOLE_ID = 27; //32 for LAST_COLOR_PARAM == 5
	private const int               PIPE_TYPE_NONE_ID = 26; //31 for LAST_COLOR_PARAM == 5
	private const int               PIPE_TYPE_BLOCKER_ID = 25; // 30 for LAST_COLOR_PARAM == 5
	private const int               LAST_COLOR_PARAM = 4; // = Consts.MAX_COLORS_LEVEL - 1

	private const int               STARS_ON_LEVEL = 5; // how many groups we should destroy
    private const int               MAX_BLOCKERS_ON_LEVEL = 5;
    private const int               MAX_HOLES_ON_LEVEL = 5;
    private const int               MAX_MOVES_ON_LEVEL = 60;

	private const float 			CHANCE_TO_MOVE_BLOCKER = -1;
	private const float				CHANCE_TO_MOVE_RANDOMLY = -1;

    private int 					SLOT_SIZE = 60;
	private int 					_w = 5;
	private int 					_h = 5;
	private List<List<SSlotData>>	_startStates = new List<List<SSlotData>>();
	private List<List<SSlotData>>	_neededStates = new List<List<SSlotData>>();
	public List<MoveInfo>			_correctMoves = new List<MoveInfo>();
	private int						_minMovesCount;

    // current brush
    private EPipeType               _carrentBrushType = EPipeType.None;
    private int                     _carrentBrushColor = -1;
    private int                     _carrentBrushParam = -1;
    private int                     _intBrush = PIPE_TYPE_NONE_ID;
    private int                     _indexStart = -1;

	private EMoveType				_drawState = EMoveType.None;
    // 

    private GUIStyle 				guiStyle = new GUIStyle();

	private Texture[] 				_statesTexs;
	private Texture[] 				_textures;
	private Texture[] 				_texturesStart;

    ScriptableLevelData 			_myLevel = null;

    //autogeneration
    private int                     _blockersCount = 0;
    private int                     _holesCount = 0;
    private int                     _maxMovesCount = 0;
	private int                     _dividesPercentage = 0;

	List<BoardPos> 					_freeCells = new List<BoardPos>();
	List<BoardPos> 					_maxColoredCells = new List<BoardPos>();
	BoardPos						_lastWorkedCell;
	BoardPos 						_forbiddenDiraction; // cant move pipe in opposite directions turn by turn


	private Dictionary<int, int>	_colorsWorked = new Dictionary<int, int>();


    void OnEnable()
	{
        Texture text_0_0 = EditorGUIUtility.Load("buttons/editor_0_0.png") as Texture;
        Texture text_0_1 = EditorGUIUtility.Load("buttons/editor_0_1.png") as Texture;
        Texture text_0_2 = EditorGUIUtility.Load("buttons/editor_0_2.png") as Texture;
        Texture text_0_3 = EditorGUIUtility.Load("buttons/editor_0_3.png") as Texture;
        //Texture text_0_4 = EditorGUIUtility.Load("buttons/editor_0_4.png") as Texture;
        Texture text_0_max = EditorGUIUtility.Load("buttons/editor_max_0.png") as Texture;

        Texture text_1_0 = EditorGUIUtility.Load("buttons/editor_1_0.png") as Texture;
        Texture text_1_1 = EditorGUIUtility.Load("buttons/editor_1_1.png") as Texture;
        Texture text_1_2 = EditorGUIUtility.Load("buttons/editor_1_2.png") as Texture;
        Texture text_1_3 = EditorGUIUtility.Load("buttons/editor_1_3.png") as Texture;
        //Texture text_1_4 = EditorGUIUtility.Load("buttons/editor_1_4.png") as Texture;
        Texture text_1_max = EditorGUIUtility.Load("buttons/editor_max_1.png") as Texture;

        Texture text_2_0 = EditorGUIUtility.Load("buttons/editor_2_0.png") as Texture;
        Texture text_2_1 = EditorGUIUtility.Load("buttons/editor_2_1.png") as Texture;
        Texture text_2_2 = EditorGUIUtility.Load("buttons/editor_2_2.png") as Texture;
        Texture text_2_3 = EditorGUIUtility.Load("buttons/editor_2_3.png") as Texture;
        //Texture text_2_4 = EditorGUIUtility.Load("buttons/editor_2_4.png") as Texture;
        Texture text_2_max = EditorGUIUtility.Load("buttons/editor_max_2.png") as Texture;

        Texture text_3_0 = EditorGUIUtility.Load("buttons/editor_3_0.png") as Texture;
        Texture text_3_1 = EditorGUIUtility.Load("buttons/editor_3_1.png") as Texture;
        Texture text_3_2 = EditorGUIUtility.Load("buttons/editor_3_2.png") as Texture;
        Texture text_3_3 = EditorGUIUtility.Load("buttons/editor_3_3.png") as Texture;
        //Texture text_3_4 = EditorGUIUtility.Load("buttons/editor_3_4.png") as Texture;
        Texture text_3_max = EditorGUIUtility.Load("buttons/editor_max_3.png") as Texture;

        Texture text_4_0 = EditorGUIUtility.Load("buttons/editor_4_0.png") as Texture;
        Texture text_4_1 = EditorGUIUtility.Load("buttons/editor_4_1.png") as Texture;
        Texture text_4_2 = EditorGUIUtility.Load("buttons/editor_4_2.png") as Texture;
        Texture text_4_3 = EditorGUIUtility.Load("buttons/editor_4_3.png") as Texture;
        //Texture text_4_4 = EditorGUIUtility.Load("buttons/editor_4_4.png") as Texture;
        Texture text_4_max = EditorGUIUtility.Load("buttons/editor_max_4.png") as Texture;

        Texture text_blocker = EditorGUIUtility.Load("buttons/editor_pBlocker.png") as Texture;
        Texture text_none = EditorGUIUtility.Load("buttons/editor_none.png") as Texture;
		Texture text_hole = EditorGUIUtility.Load("buttons/editor_hole.png") as Texture;

		_lastWorkedCell.x =-1;
		_lastWorkedCell.y = -1;
		_forbiddenDiraction.x = 0;
		_forbiddenDiraction.y = 0;

        _statesTexs = new Texture[]
		{
            text_0_0, text_0_1, text_0_2, text_0_3, //text_0_4, 
			text_0_max,
            text_1_0, text_1_1, text_1_2, text_1_3, //text_1_4, 
			text_1_max,
            text_2_0, text_2_1, text_2_2, text_2_3, //text_2_4, 
			text_2_max,
            text_3_0, text_3_1, text_3_2, text_3_3, //text_3_4, 
			text_3_max,
            text_4_0, text_4_1, text_4_2, text_4_3, //text_4_4, 
			text_4_max,
			text_blocker, text_none, text_hole
        };
		_myLevel = (ScriptableLevelData)target;

		ReloadLevel();
		if (_myLevel.MinMovesCount == -7)
		{
			_myLevel.MinMovesCount = 0;
			Autogenerate(true);
		}
	}

    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();
        if (!Application.isPlaying)
        {
            guiStyle.fontSize = 20;
            guiStyle.fontStyle = FontStyle.Bold;
            guiStyle.alignment = TextAnchor.MiddleCenter;

			GUILayout.BeginHorizontal();
			GUILayout.Label(_w.ToString(), guiStyle);
			if (GUILayout.Button("-- Width"))
			{
				if (_w > 1)
				{
					--_w;
					RecreateLevel();
				}
			}
			if (GUILayout.Button("++ Width"))
			{
				++_w;
				RecreateLevel();
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label(_h.ToString(), guiStyle);
			if (GUILayout.Button("-- Height"))
			{
				if (_h > 1)
				{
					--_h;
					RecreateLevel();
				}
			}
			if (GUILayout.Button("++ Height"))
			{
				++_h;
				RecreateLevel();
			}
			GUILayout.EndHorizontal();

			if (GUILayout.Button("RECREATE LEVEL"))
			{
				RecreateLevel();
			}

			GUILayout.Label("temp min moves = " + _minMovesCount.ToString(), guiStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("PALETTE", guiStyle);
            GUILayout.Label("NEEDED", guiStyle);
            GUILayout.Label("START", guiStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            // palette of type
			_intBrush = GUILayout.SelectionGrid(_intBrush, _statesTexs, LAST_COLOR_PARAM + 1, EditorStyles.miniButton, GUILayout.Width(SLOT_SIZE * 6), GUILayout.Height(SLOT_SIZE * 6));
            EditorsIntToPipeParams(_intBrush);
			GUILayout.Space (20);
            // needed
			int WIDTH = SLOT_SIZE * _w;
			int HEIGHT = SLOT_SIZE * _h;
			// receive clicks on needed buttons, draw buttons
			int indexNeeded = GUILayout.SelectionGrid(-1, _textures, _w, EditorStyles.miniButton, GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT));
			if (indexNeeded >= 0)
			{
				int yy1 = GetSlotPosY(indexNeeded);
				int xx1 = GetSlotPosX(indexNeeded);
				if (indexNeeded >= 0 && IsOnBoard(xx1, yy1))
				{
                    _neededStates[xx1][yy1].pt = _carrentBrushType;
                    _neededStates[xx1][yy1].c = _carrentBrushColor;
                    _neededStates[xx1][yy1].p = _carrentBrushParam;
                    _textures[indexNeeded] = _statesTexs[_intBrush];
                    ResetStartStates();
                }
            }
			GUILayout.Space (20);
			// receive clicks on start buttons, draw buttons
			int prevSelected = _indexStart;
			_indexStart = GUILayout.SelectionGrid(_indexStart, _texturesStart, _w, EditorStyles.miniButton, GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT));
            
			if (prevSelected != _indexStart)
			{
				if (_drawState != EMoveType.None)
                {
                    int fromX = GetSlotPosX(prevSelected);
                    int fromY = GetSlotPosY(prevSelected);
                    int toX = GetSlotPosX(_indexStart);
                    int toY = GetSlotPosY(_indexStart);
					if (CanMoveOrDivide(fromX, fromY, toX, toY, _drawState))
                    {
						if (_drawState == EMoveType.Move)
                        {
                            MoveTo(prevSelected, _indexStart, fromX, fromY, toX, toY);
                        } else
						if (_drawState == EMoveType.Divide)
                        {
                            DivideTo(prevSelected, _indexStart, fromX, fromY, toX, toY);
                        }
                    }
                    else
                    {
                        // select previous
                        _indexStart = prevSelected;
                    }
                }
            }

			GUILayout.EndHorizontal();
            //
            if (GUILayout.Button("Reset Start States"))
			{
				ResetStartStates();
			}
			if (GUILayout.Button("Check"))
			{
				CheckCurrentLevel();
			}
            if (GUILayout.Button("SAVE"))
            {
				if (CheckCurrentLevel())
				{
					SaveLevel();
				}
            }
            //
            if (_indexStart > -1)
            {
				if (_drawState == EMoveType.Move)
				{
                	GUILayout.Label("------------- MOVE TO... -------------", guiStyle);
				} else
				if (_drawState == EMoveType.Divide)
				{
					GUILayout.Label("------------- DIVIDE TO... -------------", guiStyle);
				} else
				if (_drawState == EMoveType.Divide)
				{
					GUILayout.Label("----------------------------------------", guiStyle);
				}
                GUILayout.BeginHorizontal();

				if (IsCanClickMove())
				{
					if (GUILayout.Button("MOVE TO..."))
					{
						_drawState = EMoveType.Move;
					}
				}

				if (IsCanClickDivide())
				{
					if (GUILayout.Button("DIVIDE TO..."))
					{
						_drawState = EMoveType.Divide;
					}
				}

				if (IsCanClickCancel())
				{
					if (GUILayout.Button("CANCEL"))
					{
						_drawState = EMoveType.None;
					}
				}
                GUILayout.EndHorizontal();
            }
            // 
            GUILayout.Label("AUTOGENERATION", guiStyle);
            if (GUILayout.Button("GENERATE"))
            {
				Autogenerate();
            }
        }
    }

	private bool CanMoveOrDivide(int fromX, int fromY, int toX, int toY, EMoveType moveType)
    {
        if (_startStates[toX][toY].pt != EPipeType.None)
        {
            return false;
        }

        bool sameX = fromX == toX;
        bool sameY = fromY == toY;
        if ((sameX && sameY) || (!sameX && !sameY))
        {
            return false;
        }

        if (sameY)
        {
            int yy = toY;
            if (fromX < toX)
            {
				// can't move from empty cell or same pipe!
				if (moveType == EMoveType.Move)
				{
					if (fromX > 0)
					{
						EPipeType pTypeBack = _startStates[fromX - 1][fromY].pt;
						if (pTypeBack == EPipeType.None) { return false; }
						if (pTypeBack == EPipeType.Colored)
						{
							int colorStart = _startStates[fromX][fromY].c;
							int colorBack = _startStates[fromX - 1][fromY].c;
							if (colorBack == colorStart)
							{
								int paramStart = _startStates[fromX][fromY].p;
								int paramBack = _startStates[fromX - 1][fromY].p;
								if (paramStart == paramBack)
								{
									return false;
								}
							}
						}
					}
				}
				//
                for (int i = fromX + 1; i < toX; ++i)
                {
                    EPipeType pType = _startStates[i][yy].pt;
                    if (pType == EPipeType.None)
                    {

                    } else
                    if (pType == EPipeType.Colored)
                    {
                        int param = _startStates[i][yy].p;
                        if (param != LAST_COLOR_PARAM)
                        {
                            return false;
                        }
                    } else
                    {
                        return false;
                    }
                }
            } else
            //if (fromX > toX)
            {
				// can't move from empty cell or same pipe!
				if (moveType == EMoveType.Move)
				{
					if (fromX < _w - 1)
					{
						EPipeType pTypeBack = _startStates[fromX + 1][fromY].pt;
						if (pTypeBack == EPipeType.None) { return false; }
						if (pTypeBack == EPipeType.Colored)
						{
							int colorStart = _startStates[fromX][fromY].c;
							int colorBack = _startStates[fromX + 1][fromY].c;
							if (colorBack == colorStart)
							{
								int paramStart = _startStates[fromX][fromY].p;
								int paramBack = _startStates[fromX + 1][fromY].p;
								if (paramStart == paramBack)
								{
									return false;
								}
							}
						}
					}
				}
				//
                for (int i = fromX - 1; i > toX; --i)
                {
                    EPipeType pType = _startStates[i][yy].pt;
                    if (pType == EPipeType.None)
                    {

                    }
                    else
                    if (pType == EPipeType.Colored)
                    {
                        int param = _startStates[i][yy].p;
                        if (param != LAST_COLOR_PARAM)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        } else
        //if (sameX)
        {
            int xx = toX;
            if (fromY < toY)
            {
				// can't move from empty cell or same pipe!
				if (moveType == EMoveType.Move)
				{
					if (fromY > 0)
					{
						EPipeType pTypeBack = _startStates[fromX][fromY - 1].pt;
						if (pTypeBack == EPipeType.None) { return false; }
						if (pTypeBack == EPipeType.Colored)
						{
							int colorStart = _startStates[fromX][fromY].c;
							int colorBack = _startStates[fromX][fromY - 1].c;
							if (colorBack == colorStart)
							{
								int paramStart = _startStates[fromX][fromY].p;
								int paramBack = _startStates[fromX][fromY - 1].p;
								if (paramStart == paramBack)
								{
									return false;
								}
							}
						}
					}
				}
				//
                for (int j = fromY + 1; j < toY; ++j)
                {
                    EPipeType pType = _startStates[xx][j].pt;
                    if (pType == EPipeType.None)
                    {

                    }
                    else
                    if (pType == EPipeType.Colored)
                    {
                        int param = _startStates[xx][j].p;
                        if (param != LAST_COLOR_PARAM)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            //if (fromY > toY)
            {
				// can't move from empty cell or same pipe!
				if (moveType == EMoveType.Move)
				{
					if (fromY < _h - 1)
					{
						EPipeType pTypeBack = _startStates[fromX][fromY + 1].pt;
						if (pTypeBack == EPipeType.None) { return false; }
						if (pTypeBack == EPipeType.Colored)
						{
							int colorStart = _startStates[fromX][fromY].c;
							int colorBack = _startStates[fromX][fromY + 1].c;
							if (colorBack == colorStart)
							{
								int paramStart = _startStates[fromX][fromY].p;
								int paramBack = _startStates[fromX][fromY + 1].p;
								if (paramStart == paramBack)
								{
									return false;
								}
							}
						}
					}
				}
				//
                for (int j = fromY - 1; j > toY; --j)
                {
                    EPipeType pType = _startStates[xx][j].pt;
                    if (pType == EPipeType.None)
                    {

                    }
                    else
                    if (pType == EPipeType.Colored)
                    {
                        int param = _startStates[xx][j].p;
                        if (param != LAST_COLOR_PARAM)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private void MoveTo(int indexFrom, int indexTo, int fromX, int fromY, int toX, int toY)
    {
        // copy from old position to new
        _texturesStart[indexTo] = _texturesStart[indexFrom];
        _startStates[toX][toY].pt = _startStates[fromX][fromY].pt;
        _startStates[toX][toY].c = _startStates[fromX][fromY].c;
        _startStates[toX][toY].p = _startStates[fromX][fromY].p;
        // reset on old position
        _texturesStart[indexFrom] = _statesTexs[PIPE_TYPE_NONE_ID];
        _startStates[fromX][fromY].pt = EPipeType.None;
        _startStates[fromX][fromY].c = -1;
        _startStates[fromX][fromY].p = -1;
        //
		_drawState = EMoveType.None;
        _minMovesCount += 1;
        //
        MoveInfo moveInfo;
        moveInfo.MoveType = EMoveType.Move;
        moveInfo.FromX = toX;
        moveInfo.FromY = toY;
        moveInfo.ToX = fromX;
        moveInfo.ToY = fromY;
        _correctMoves.Add(moveInfo);
        //
    }

    private void DivideTo(int indexFrom, int indexTo, int fromX, int fromY, int toX, int toY)
    {
        // decrease param on old position
        _startStates[fromX][fromY].p -= 1;
        // copy from old position to new
        _startStates[toX][toY].pt = _startStates[fromX][fromY].pt;
        _startStates[toX][toY].c = _startStates[fromX][fromY].c;
        _startStates[toX][toY].p = _startStates[fromX][fromY].p;
        //
        int newIndex = PipeParamsToEditorsInt(_startStates[fromX][fromY].pt, _startStates[fromX][fromY].c, _startStates[fromX][fromY].p);
        _texturesStart[indexFrom] = _statesTexs[newIndex];
        _texturesStart[indexTo] = _statesTexs[newIndex];
        //
		_drawState = EMoveType.None;
        _minMovesCount += 1;
        //
        MoveInfo moveInfo;
        moveInfo.MoveType = EMoveType.Divide;
        moveInfo.FromX = toX;
        moveInfo.FromY = toY;
        moveInfo.ToX = fromX;
        moveInfo.ToY = fromY;
        _correctMoves.Add(moveInfo);
        //
    }

    private bool IsCanClickMove()
	{
		int yy = GetSlotPosY(_indexStart);
		int xx = GetSlotPosX(_indexStart);
		if (_indexStart < 0 || !IsOnBoard(xx, yy))
		{
			Debug.LogError("SOMETHING WRONG");
			return false;
		}
		//
		if (_startStates[xx][yy].pt != EPipeType.Colored && _startStates[xx][yy].pt != EPipeType.Blocker)
		{
			return false;
		}
		if (_startStates[xx][yy].pt == EPipeType.Colored && _startStates[xx][yy].p == LAST_COLOR_PARAM)
		{
			return false;
		}
		return true;
	}

	private bool IsCanClickDivide()
	{
		int yy = GetSlotPosY(_indexStart);
		int xx = GetSlotPosX(_indexStart);
		if (_indexStart < 0 || !IsOnBoard(xx, yy))
		{
			Debug.LogError("SOMETHING WRONG");
			return false;
		}
		//
		if (_startStates[xx][yy].pt != EPipeType.Colored)
		{
			return false;
		}
		if (_startStates[xx][yy].p <= 0)
		{
			return false;
		}
		return true;
	}

	private bool IsCanClickCancel()
	{
		return _drawState == EMoveType.Move || _drawState == EMoveType.Divide;
	}

    private int PipeParamsToEditorsInt(EPipeType brushType, int brushColor, int brushParam)
    {
        if (brushType == EPipeType.Blocker)
        {
            return PIPE_TYPE_BLOCKER_ID;
        } else
        if (brushType == EPipeType.None)
        {
            return PIPE_TYPE_NONE_ID;
        } else
		if (brushType == EPipeType.Hole)
		{
			return PIPE_TYPE_HOLE_ID;
		} else
        {
			return brushColor * (LAST_COLOR_PARAM + 1) + brushParam;
        }
    }
    
    private void EditorsIntToPipeParams(int editorsInt)
    {
        if (editorsInt == PIPE_TYPE_BLOCKER_ID)
        {
            _carrentBrushType = EPipeType.Blocker;
            _carrentBrushColor = -1;
            _carrentBrushParam = -1;
        } else
        if (editorsInt == PIPE_TYPE_NONE_ID)
        {
            _carrentBrushType = EPipeType.None;
            _carrentBrushColor = -1;
            _carrentBrushParam = -1;
        } else
		if (editorsInt == PIPE_TYPE_HOLE_ID)
		{
			_carrentBrushType = EPipeType.Hole;
			_carrentBrushColor = -1;
			_carrentBrushParam = -1;
		} else
        {
			_carrentBrushType = EPipeType.Colored;
			_carrentBrushColor = editorsInt / (LAST_COLOR_PARAM + 1);
			_carrentBrushParam = editorsInt % (LAST_COLOR_PARAM + 1);
        }
    }

    private bool IsOnBoard(int i, int j)
	{
		return (i >= 0 && j >= 0 && i < _w && j < _h);
	}

	private int GetSlotPosX(int index)
	{
		int i = index % _w;
		return i;
	}

	private int GetSlotPosY(int index)
	{
		int j = _h - 1 - index / _w ; //<-fix for vertical flip  //int j = index / _w ;
		return j;
	}

	private int GetSlotIndex(int i, int j)
	{
		return (_h - j - 1) * _w + i; //<-fix for vertical flip  // return j * _w + i;
	}

	private void RecreateLevel()
	{
        _carrentBrushType = EPipeType.None;
        _carrentBrushColor = -1;
        _carrentBrushParam = -1;
        _intBrush = PIPE_TYPE_NONE_ID;
        _indexStart = -1;
		_drawState = EMoveType.None;

        _textures = new Texture[_w * _h];
        _texturesStart = new Texture[_w * _h];
        _startStates.Clear();
        _neededStates.Clear();
        for (int i = 0; i < _w; ++i)
        {
            List<SSlotData> rowS = new List<SSlotData>();
            List<SSlotData> rowN = new List<SSlotData>();
            for (int j = 0; j < _h; ++j)
            {
                int id = GetSlotIndex(i, j);
                _textures[id] = _statesTexs[PIPE_TYPE_NONE_ID];
                _texturesStart[id] = _statesTexs[PIPE_TYPE_NONE_ID];

                SSlotData neededSlot = new SSlotData();
                neededSlot.x = i;
                neededSlot.y = j;
                neededSlot.c = -1;
                neededSlot.p = -1;
                neededSlot.pt = EPipeType.None;
                rowN.Add(neededSlot);

                SSlotData startSlot = new SSlotData();
                startSlot.x = i;
                startSlot.y = j;
                startSlot.c = -1;
                startSlot.p = -1;
                startSlot.pt = EPipeType.None;
                rowS.Add(startSlot);
            }
            _startStates.Add(rowS);
            _neededStates.Add(rowN);
        }
        _minMovesCount = 0;
        _correctMoves.Clear();

		_freeCells.Clear();
		_maxColoredCells.Clear();
		_lastWorkedCell.x =-1;
		_lastWorkedCell.y = -1;
    }

    private void ResetStartStates()
	{
        _indexStart = -1;
		_drawState = EMoveType.None;
        for (int i = 0; i < _w; ++i)
        {
            for (int j = 0; j < _h; ++j)
            {
                _startStates[i][j].pt = _neededStates[i][j].pt;
                _startStates[i][j].c = _neededStates[i][j].c;
                _startStates[i][j].p = _neededStates[i][j].p;
                //int id = GetSlotIndex(i, j);
                //_texturesStart[id] = _textures[id];
            }
        }
        for (int i = 0; i < _texturesStart.Length; ++i)
        {
            _texturesStart[i] = _textures[i];
        }
        _minMovesCount = 0;
        _correctMoves.Clear();
    }

	private void ReloadLevel()
	{
		if (_myLevel.MinMovesCount == FLAG_TO_GENERATE_LEVEL || _myLevel.NeededStates == null || _myLevel.StartStates == null)
        {
            RecreateLevel();
            return;
        }
        _w = _myLevel.W;
        _h = _myLevel.H;
		_minMovesCount = _myLevel.MinMovesCount;
        _correctMoves.Clear();
        for (int i = 0; i < _myLevel.CorrectMoves.Count; ++i)
        {
            _correctMoves.Add(_myLevel.CorrectMoves[i]);
        }

        _textures = new Texture[_w * _h];
        _texturesStart = new Texture[_w * _h];

		_neededStates = new List<List<SSlotData>>();
		_startStates = new List<List<SSlotData>>();

        for (int i = 0; i < _w; ++i)
        {
			List<SSlotData> rowS = new List<SSlotData>();
			List<SSlotData> rowN = new List<SSlotData>();
            for (int j = 0; j < _h; ++j)
            {
                int id = GetSlotIndex(i, j);

				SSlotData savedNeededSlot = _myLevel.NeededStates[id];
				SSlotData neededSlot = new SSlotData();
				neededSlot.x = i;
				neededSlot.y = j;
				neededSlot.c = savedNeededSlot.c;
				neededSlot.p = savedNeededSlot.p;
				neededSlot.pt = savedNeededSlot.pt;
				rowN.Add(neededSlot);
				int intNeededIndex = PipeParamsToEditorsInt(neededSlot.pt, neededSlot.c, neededSlot.p);
				_textures[id] = _statesTexs[intNeededIndex];

				SSlotData savedStartSlot = _myLevel.StartStates[id];
				SSlotData startSlot = new SSlotData();
				startSlot.x = i;
				startSlot.y = j;
				startSlot.c = savedStartSlot.c;
				startSlot.p = savedStartSlot.p;
				startSlot.pt = savedStartSlot.pt;
				rowS.Add(startSlot);
                int intStartIndex = PipeParamsToEditorsInt(startSlot.pt, startSlot.c, startSlot.p);
                _texturesStart[id] = _statesTexs[intStartIndex];
            }
			_startStates.Add(rowS);
			_neededStates.Add(rowN);
        }
    }

	private void SaveLevel()
	{
        if (_myLevel.NeededStates == null)
        {
            _myLevel.NeededStates = new List<SSlotData>();
            _myLevel.StartStates = new List<SSlotData>();
            _myLevel.CorrectMoves = new List<MoveInfo>();
        }

        _myLevel.MinMovesCount = _minMovesCount;
        _myLevel.CorrectMoves.Clear();
        for (int i = 0; i < _correctMoves.Count; ++i)
        {
            _myLevel.CorrectMoves.Add(_correctMoves[i]);
        }
        _myLevel.W = _w;
		_myLevel.H = _h;
		_myLevel.NeededStates = new List<SSlotData>();
		_myLevel.StartStates = new List<SSlotData>();

		//for (int j = 0; j < _h; ++j)
		//{
			//for (int i = 0; i < _w; ++i)
			//{
		for (int j = _h - 1; j >= 0; --j)
		{
			for (int i = 0; i < _w; ++i)
			{
				SSlotData neededData = _neededStates[i][j];
				SSlotData neededSlot = new SSlotData();
				neededSlot.x = i;
				neededSlot.y = j;
				neededSlot.c = neededData.c;
				neededSlot.p = neededData.p;
				neededSlot.pt = neededData.pt;
				_myLevel.NeededStates.Add(neededSlot);

				SSlotData startData = _startStates[i][j];
				SSlotData startSlot = new SSlotData();
				startSlot.x = i;
				startSlot.y = j;
				startSlot.c = startData.c;
				startSlot.p = startData.p;
				startSlot.pt = startData.pt;
				_myLevel.StartStates.Add(startSlot);
			}
		}
		EditorUtility.SetDirty(_myLevel);
	}

	private void Autogenerate(bool save = false)
	{
		_blockersCount = _myLevel.BlockersCount;
		_holesCount = _myLevel.HolesCount;
		_maxMovesCount = _myLevel.MaxMovesCount;
		_dividesPercentage = _myLevel.DividesPercentage;
		_minMovesCount = 0;
		if (_maxMovesCount <= 5 || _dividesPercentage <= 0)
		{
			Debug.LogError("Wrong Autogenerating Params");
			return;
		}

		RecreateLevel();
		for (int j = 0; j < _h; ++j)
		{
			for (int i = 0; i < _w; ++i)
			{
				BoardPos bPos;
				bPos.x = i;
				bPos.y = j;
				_freeCells.Add(bPos);
			}
		}
		_freeCells = Helpers.ShuffleList(_freeCells);
		if (Autogenerate_AddHoles() && Autogenerate_AddBlockers() && Autogenerate_AddColored() && Autogenerate_GenerateMoves())
		{
			Debug.Log("LEVEL " + _myLevel.Id + " WAS GENERATED");
		} else
		{
			Debug.LogError("FAIL TO GENERATE LEVEL " + _myLevel.Id);
			Autogenerate();
			return;
		}
		if (CheckCurrentLevel())
		{
			if (save)
			{
				SaveLevel();
			}
		} else
		{
			Autogenerate();
		}
	}

	private bool Autogenerate_AddHoles()
	{
		int holesNeeded = _holesCount;
		for (int j = _freeCells.Count - 1; j >= 0; --j)
		{
			if (holesNeeded == 0)
			{
				return true;
			}
				
			BoardPos bPos = _freeCells[j];

			_neededStates[bPos.x][bPos.y].pt = EPipeType.Hole;

			if (CheckHoles())
			{
				_startStates[bPos.x][bPos.y].pt = EPipeType.Hole;
				int id = GetSlotIndex(bPos.x, bPos.y);
				_textures[id] = _statesTexs[PIPE_TYPE_HOLE_ID];
				_texturesStart[id] = _textures[id];
				_freeCells.RemoveAt(j);
				--holesNeeded;
			} else
			{
				_neededStates[bPos.x][bPos.y].pt = EPipeType.None;
			}
		}
			
		if (holesNeeded > 0)
		{
			Debug.LogError("NOT ALL HOLES ADDED");
			return false;
		} else
		{
			return true;
		}
	}

	private bool CheckHoles()
	{
		// get first free cell
		BoardPos startPos;
		if (_freeCells.Count > 1) // last is temporary hole
		{
			startPos = _freeCells[0];
		} else
		{
			return false; // all field occupied
		}
		// flood method
		Dictionary<int, bool> notUpdatedCells = new Dictionary<int, bool>();
		for (int j = 0; j < _h; ++j)
		{
			for (int i = 0; i < _w; ++i)
			{
				if (_neededStates[i][j].pt != EPipeType.Hole)
				{
					notUpdatedCells.Add(j * 1000 + i, false);
				}
			}
		}
		Flood(notUpdatedCells, startPos);
		return notUpdatedCells.Count == 0;
	}

	private void Flood(Dictionary<int, bool> notUpdatedCells, BoardPos pos)
	{
		if (pos.x < 0 || pos.y < 0 || pos.x >= _w || pos.y >= _h)
		{
			return;
		}
		int id = pos.y * 1000 + pos.x;
		if (!notUpdatedCells.ContainsKey(id))
		{
			return;
		}
		notUpdatedCells.Remove(id);

		BoardPos posLeft;
		posLeft.x = pos.x - 1;
		posLeft.y = pos.y;
		Flood(notUpdatedCells, posLeft);

		BoardPos posRight;
		posRight.x = pos.x + 1;
		posRight.y = pos.y;
		Flood(notUpdatedCells, posRight);

		BoardPos posDown;
		posDown.x = pos.x;
		posDown.y = pos.y - 1;
		Flood(notUpdatedCells, posDown);

		BoardPos posUp;
		posUp.x = pos.x;
		posUp.y = pos.y + 1;
		Flood(notUpdatedCells, posUp);
	}

	private bool Autogenerate_AddBlockers()
	{
		int blockersNeeded = _blockersCount;
		for (int j = _freeCells.Count - 1; j >= 0; --j)
		{
			if (blockersNeeded == 0)
			{
				return true;
			}

			BoardPos bPos = _freeCells[j];

			_neededStates[bPos.x][bPos.y].pt = EPipeType.Blocker;
			_startStates[bPos.x][bPos.y].pt = EPipeType.Blocker;
			int id = GetSlotIndex(bPos.x, bPos.y);
			_textures[id] = _statesTexs[PIPE_TYPE_BLOCKER_ID];
			_texturesStart[id] = _textures[id];
			_freeCells.RemoveAt(j);
			--blockersNeeded;
		}

		if (blockersNeeded > 0)
		{
			Debug.LogError("NOT ALL BLOCKERS ADDED");
			return false;
		} else
		{
			return true;
		}
	}

	private bool Autogenerate_AddColored()
	{
		int coloredNeeded = STARS_ON_LEVEL;
		int colorParam = 0;
		for (int j = _freeCells.Count - 1; j >= 0; --j)
		{
			if (coloredNeeded == 0)
			{
				return true;
			}

			BoardPos bPos = _freeCells[j];

			_neededStates[bPos.x][bPos.y].pt = EPipeType.Colored;
			_startStates[bPos.x][bPos.y].pt = EPipeType.Colored;
			_neededStates[bPos.x][bPos.y].c = colorParam;
			_startStates[bPos.x][bPos.y].c = colorParam;
			_neededStates[bPos.x][bPos.y].p = LAST_COLOR_PARAM;
			_startStates[bPos.x][bPos.y].p = LAST_COLOR_PARAM;
			int id = GetSlotIndex(bPos.x, bPos.y);
			int intNeededIndex = PipeParamsToEditorsInt(EPipeType.Colored, colorParam, LAST_COLOR_PARAM);
			_textures[id] = _statesTexs[intNeededIndex];
			_texturesStart[id] = _textures[id];
			_freeCells.RemoveAt(j);
			--coloredNeeded;
			++colorParam;
			_maxColoredCells.Add(bPos);
		}

		if (coloredNeeded > 0)
		{
			Debug.LogError("NOT ALL COLORED ADDED");
			return false;
		} else
		{
			return true;
		}
	}

	private bool Autogenerate_GenerateMoves()
	{
		_colorsWorked.Clear();
		for (int i = -1; i < Consts.CLASSIC_GAME_COLORS; ++i)
		{
			_colorsWorked.Add(i, 0); // 0 moves made for i color
		}
		do
		{
			if (_minMovesCount >= _maxMovesCount)
			{
				break;
			}
			if (TryDivideColoredMax())
			{
				continue;
			}
			if (!GenerateRandomMove())
			{
				// no more moves
				break;
			}
		} while (true);
		if (_maxColoredCells.Count > 0)
		{
			Debug.LogError("NOT ALL COLORED WHERE DIVIDED");
			return false;
		}
		return true;
	}

	private bool TryDivideColoredMax()
	{
		if (_maxColoredCells.Count > 0)
		{
			for (int i = _maxColoredCells.Count - 1; i >= 0; --i)
			{
				if (GenerateDivideMoveFromCell(_maxColoredCells[i]))
				{
					_maxColoredCells.RemoveAt(i);
					_colorsWorked[i]++;
					return true;
				}
			}
		} 
		return false;
	}

	private bool GenerateRandomMove()
	{
		// creating lists with potencial moves
		List<BoardPos> cellsForMove = new List<BoardPos>();
		List<BoardPos> cellsForDivide = new List<BoardPos>();
		for (int j = 0; j < _h; ++j)
		{
			for (int i = 0; i < _w; ++i)
			{
				if (_startStates[i][j].pt == EPipeType.Colored)
				{
					if (_startStates[i][j].p < LAST_COLOR_PARAM)
					{
						BoardPos bp;
						bp.x = i;
						bp.y = j;
						cellsForMove.Add(bp);

						if (_startStates[i][j].p > 0)
						{
							cellsForDivide.Add(bp);
						}
					}
				}
				if (_startStates[i][j].pt == EPipeType.Blocker)
				{
					BoardPos bp;
					bp.x = i;
					bp.y = j;
					cellsForMove.Add(bp);
				}
			}
		}
		//
		if (cellsForMove.Count == 0 && cellsForDivide.Count == 0)
		{
			// no more moves
			return false;
		} else
		if (cellsForMove.Count == 0)
		{
			return (TryGenerateDivideMove(cellsForDivide));
		} else
		if (cellsForDivide.Count == 0)
		{
			return (TryGenerateMoveMove(cellsForMove));
		} else
		{
			bool divideFirst = UnityEngine.Random.Range(1, 101) <= _dividesPercentage;
			if (divideFirst)
			{
				// at first try to divide
				if (TryGenerateDivideMove(cellsForDivide))
				{
					return true;
				} else
				{
					return (TryGenerateMoveMove(cellsForMove));
				}
			} else
			{
				// at first try to move
				if (TryGenerateMoveMove(cellsForMove))
				{
					return true;
				} else
				{
					return (TryGenerateDivideMove(cellsForDivide));
				}
			}
		}
	}

	private List<BoardPos> SortCellsByColorsAppearance(List<BoardPos> acells)
	{
		Dictionary<BoardPos, int> pipes = new Dictionary<BoardPos, int>();
		for (int i = 0; i < acells.Count; ++i)
		{
			//if (isOnBoard(cells[i]))
			{
				pipes.Add(acells[i], _startStates[acells[i].x][acells[i].y].c);
			}
		}
		acells.Clear();

		List<BoardPos> res = new List<BoardPos>();
		float rand = UnityEngine.Random.Range(0.0f, 100.0f);
		if (rand <= CHANCE_TO_MOVE_BLOCKER)
		{
			foreach (var p in pipes)
			{
				if (p.Value == -1)
				{
					res.Add(p.Key);
				}
			}
			for (int i = 0; i < res.Count; ++i)
			{
				pipes.Remove(res[i]);
			}
		}
		rand = UnityEngine.Random.Range(0.0f, 100.0f);
		if (rand <= CHANCE_TO_MOVE_RANDOMLY)
		{
			foreach (var p in pipes)
			{
				res.Add(p.Key);
			}
		} else
		{
			List<BoardPos>colorsWork = new List<BoardPos>();
			for (int i = 0; i < Consts.CLASSIC_GAME_COLORS; ++i)
			{
				BoardPos p;
				p.x = i;
				p.y = 0;
				colorsWork.Add(p);
			}
			foreach(var p in pipes)
			{
				if (p.Value >= 0)
				{
					int ac = p.Value;
					BoardPos bp = colorsWork[ac];
					bp.y++;
					colorsWork[ac] = bp;
				}
			}
			//sort
			for (int i = 0; i < Consts.CLASSIC_GAME_COLORS - 1; ++i)
			{
				for (int j = i + 1; j < Consts.CLASSIC_GAME_COLORS; ++j)
				{
					if (colorsWork[i].y > colorsWork[j].y)
					{
						BoardPos temp = colorsWork[i];
						colorsWork[i] = colorsWork[j];
						colorsWork[j] = temp;
					}
				}
			}
			//
			//Debug.Log("==================== " + colorsWork[0].x);
			List<BoardPos> listToDelete = new List<BoardPos>();
			for (int j = 0; j < colorsWork.Count; ++j)
			{
				int acolor = colorsWork[j].x;
				foreach (var k in pipes)
				{
					if (k.Value == acolor)
					{
						res.Add(k.Key);
						listToDelete.Add(k.Key);
					}
				}
				for (int i = 0 ; i < listToDelete.Count; ++i)
				{
					pipes.Remove(listToDelete[i]);
				}
				listToDelete.Clear();
			}
			//add last
			foreach (var k in pipes)
			{
				res.Add(k.Key);
			}
			pipes.Clear();
		}
			
		return res;
	}

	private bool TryGenerateDivideMove(List<BoardPos> cells)
	{
		List<BoardPos> sortedCells = SortCellsByColorsAppearance(cells);
		for (int i = 0; i < sortedCells.Count; ++i)
		{
			int acolor = _startStates[sortedCells[i].x][sortedCells[i].y].c;
			if (GenerateDivideMoveFromCell(sortedCells[i]))
			{
				_colorsWorked[acolor]++;
				return true;
			}
		}
		return false;
	}

	private bool TryGenerateMoveMove(List<BoardPos> cells)
	{
		List<BoardPos> sortedCells = SortCellsByColorsAppearance(cells);
		for (int i = 0; i < sortedCells.Count; ++i)
		{
			int acolor = _startStates[sortedCells[i].x][sortedCells[i].y].c;
			if (GenerateMoveMoveFromCell(sortedCells[i]))
			{
				_colorsWorked[acolor]++;
				return true;
			}
		}
		return false;
	}

	private bool GenerateMoveMoveFromCell(BoardPos pos)
	{
		BoardPos toLeft;
		toLeft.x = -1; toLeft.y = -1;
		BoardPos toRight;
		toRight.x = -1; toRight.y = -1;
		BoardPos toDown;
		toDown.x = -1; toDown.y = -1;
		BoardPos toUp;
		toUp.x = -1; toUp.y = -1;

		if (_lastWorkedCell.x == pos.x && _lastWorkedCell.y == pos.y)
		{
			if (_forbiddenDiraction.x != -1) { toLeft = FindPlaceForMoveOrDivide(pos, -1, 0, true); };
			if (_forbiddenDiraction.x != 1) { toRight = FindPlaceForMoveOrDivide(pos, 1, 0, true); };
			if (_forbiddenDiraction.y != -1) { toDown = FindPlaceForMoveOrDivide(pos, 0, -1, true); };
			if (_forbiddenDiraction.y != 1) { toUp = FindPlaceForMoveOrDivide(pos, 0, 1, true); };
		} else
		{
			toLeft = FindPlaceForMoveOrDivide(pos, -1, 0, true);
			toRight = FindPlaceForMoveOrDivide(pos, 1, 0, true);
			toDown = FindPlaceForMoveOrDivide(pos, 0, -1, true);
			toUp = FindPlaceForMoveOrDivide(pos, 0, 1, true);
		}

		List<BoardPos> possibleMoves = new List<BoardPos>();
		if (toLeft.x >= 0)
		{
			possibleMoves.Add(toLeft);
		}
		if (toRight.x >= 0)
		{
			possibleMoves.Add(toRight);
		}
		if (toDown.x >= 0)
		{
			possibleMoves.Add(toDown);
		}
		if (toUp.x >= 0)
		{
			possibleMoves.Add(toUp);
		}
		if (possibleMoves.Count == 0)
		{
			return false;
		}
		BoardPos res = possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Count)];
		int prevSelected = GetSlotIndex(pos.x, pos.y);
		int indexStart = GetSlotIndex(res.x, res.y);
		MoveTo(prevSelected, indexStart, pos.x, pos.y, res.x, res.y);
		//
		_lastWorkedCell = res;
		_forbiddenDiraction.x = 0;
		_forbiddenDiraction.y = 0;
		if (pos.x < res.x)
		{
			_forbiddenDiraction.x = -1;
		} else
		if (pos.x > res.x)
		{
			_forbiddenDiraction.x = 1;
		} else
		if (pos.y < res.y)
		{
			_forbiddenDiraction.y = -1;
		} else
		if (pos.y > res.y)
		{
			_forbiddenDiraction.y = 1;
		}
		//
		return true;
	}

	private bool GenerateDivideMoveFromCell(BoardPos pos)
	{
		BoardPos toLeft = FindPlaceForMoveOrDivide(pos, -1, 0);
		BoardPos toRight = FindPlaceForMoveOrDivide(pos, 1, 0);
		BoardPos toDown = FindPlaceForMoveOrDivide(pos, 0, -1);
		BoardPos toUp = FindPlaceForMoveOrDivide(pos, 0, 1);
		List<BoardPos> possibleDivides = new List<BoardPos>();

		if (toLeft.x >= 0)
		{
			possibleDivides.Add(toLeft);
		}
		if (toRight.x >= 0)
		{
			possibleDivides.Add(toRight);
		}
		if (toDown.x >= 0)
		{
			possibleDivides.Add(toDown);
		}
		if (toUp.x >= 0)
		{
			possibleDivides.Add(toUp);
		}
		if (possibleDivides.Count == 0)
		{
			return false;
		}
		BoardPos res = possibleDivides[UnityEngine.Random.Range(0, possibleDivides.Count)];
		int prevSelected = GetSlotIndex(pos.x, pos.y);
		int indexStart = GetSlotIndex(res.x, res.y);
		DivideTo(prevSelected, indexStart, pos.x, pos.y, res.x, res.y);
		//
		_lastWorkedCell.x = -1;
		_lastWorkedCell.y = -1;
		return true;
	}

	private BoardPos FindPlaceForMoveOrDivide(BoardPos from, int dirX, int dirY, bool isMoveMove = false)
	{
        BoardPos prev = from;
        EPipeType prevType = EPipeType.Colored;

        BoardPos resFalse;
        resFalse.x = -1;
        resFalse.y = -1;

		if (isMoveMove)
		{
			// check if the same pipe behind or max pipe behind or empty cell behind
			BoardPos behindCell = from;
			behindCell.x -= dirX;
			behindCell.y -= dirY;
			if (IsOnBoard(behindCell.x, behindCell.y))
			{
				EPipeType behindType = _startStates[behindCell.x][behindCell.y].pt;
				int behindParam = _startStates[behindCell.x][behindCell.y].p;
				int behindColor = _startStates[behindCell.x][behindCell.y].c;
				if (behindType == EPipeType.None)
				{
					return resFalse;
				} else
				if (behindType != EPipeType.Colored)
				{
					// its ok, can move
				} else
				{
					// color pipe
					if (behindParam >= LAST_COLOR_PARAM)
					{
						return resFalse;
					} else
					{
						int fromColor = _startStates[from.x][from.y].c;
						if (behindColor == fromColor)
						{
							int fromParam =  _startStates[from.x][from.y].p;
							if (behindParam == fromParam)
							{
								return resFalse;
							}
						}
					}
				}
			}
		}

        bool isOnBoard = true;

        do
        {
            BoardPos nextPos = prev;
            nextPos.x += dirX;
            nextPos.y += dirY;

            isOnBoard = IsOnBoard(nextPos.x, nextPos.y);
            if (!isOnBoard)
            {
                if (prevType == EPipeType.None)
                {
                    return prev;
                }
                else
                {
                    return resFalse;
                }
            }
            else
            {
                EPipeType pType = _startStates[nextPos.x][nextPos.y].pt;
                int param = _startStates[nextPos.x][nextPos.y].p;
				if (pType == EPipeType.None) //??? - wrong || (pType == EPipeType.Colored && param == LAST_COLOR_PARAM))
                {
                    prev = nextPos;
                    prevType = pType;
                }
                else
                {
                    if (prevType == EPipeType.None)
                    {
                        return prev;
                    }
                    else
                    {
                        return resFalse;
                    }
                }
            }
        } while (isOnBoard);

        return resFalse;
    }

	private bool CheckCurrentLevel()
	{
        // copy start states
        List<List<SSlotData>> states = new List<List<SSlotData>>();
        for (int i = 0; i < _startStates.Count; ++i)
        {
            List<SSlotData> line = new List<SSlotData>();
            for (int j = 0; j < _startStates[i].Count; ++j)
            {
                SSlotData data = new SSlotData();
                data.x = _startStates[i][j].x;
                data.y = _startStates[i][j].y;
                data.pt = _startStates[i][j].pt;
                data.p = _startStates[i][j].p;
                data.c = _startStates[i][j].c;
                line.Add(data);
            }
            states.Add(line);
        }
        //		// check if copy correct
        //		for (int i = 0; i < _startStates.Count; ++i)
        //		{
        //			for (int j = 0; j < _startStates[i].Count; ++j)
        //			{
        //				if (_startStates[i][j].x != states[i][j].x || 
        //					_startStates[i][j].y != states[i][j].y ||
        //					_startStates[i][j].p != states[i][j].p ||
        //					_startStates[i][j].pt != states[i][j].pt ||
        //					_startStates[i][j].c != states[i][j].c)
        //				{
        //					Debug.LogError("!!! " + i + "    " + j);
        //				}
        //			}
        //		}
        // do moves from the end
        for (int k = _correctMoves.Count - 1; k >= 0; --k) //for (int k = 0; k < _correctMoves.Count; ++k)
        {
            MoveInfo moveInfo = _correctMoves[k];
            SSlotData slotFrom = states[moveInfo.FromX][moveInfo.FromY];
            EPipeType pTypeFrom = slotFrom.pt;
            if (pTypeFrom != EPipeType.Blocker && pTypeFrom != EPipeType.Colored)
            {
                Debug.LogError("Step " + (_correctMoves.Count - 1 - k) + " is wrong - pipe is not movable or dont exists");
                return false;
            }
            // find direction
            int dirX = moveInfo.ToX - moveInfo.FromX;
            int dirY = moveInfo.ToY - moveInfo.FromY;
            if (dirX != 0)
            {
                dirX /= Mathf.Abs(dirX);
            }
            if (dirY != 0)
            {
                dirY /= Mathf.Abs(dirY);
            }
            // find if something on the way of moving
            if (dirX != 0)
            {
                for (int i = moveInfo.FromX + dirX; i != moveInfo.ToX; i += dirX)
                {
                    if (states[i][moveInfo.ToY].pt != EPipeType.None)
                    {
                        Debug.LogError("Step " + (_correctMoves.Count - 1 - k) + " is wrong - somesing on the path of pipe horizontally");
                        return false;
                    }
                }
            }
            else
            //if (dirY != 0)
            {
                for (int j = moveInfo.FromY + dirY; j != moveInfo.ToY; j += dirY)
                {
                    if (states[moveInfo.ToX][j].pt != EPipeType.None)
                    {
                        Debug.LogError("Step " + (_correctMoves.Count - 1 - k) + " is wrong - somesing on the path of pipe vertically");
                        return false;
                    }
                }
            }
            //
            if (moveInfo.MoveType == EMoveType.Move)
            {
                SSlotData slotTo = states[moveInfo.ToX][moveInfo.ToY];
                if (slotTo.pt != EPipeType.None)
                {
                    Debug.LogError("Step " + (_correctMoves.Count - 1 - k) + " is wrong - cell to move is occupied");
                    return false;
                }
                if (pTypeFrom == EPipeType.Colored)
                {
                    // check if something behind
                    int behindX = moveInfo.ToX + dirX;
                    int behindY = moveInfo.ToY + dirY;
                    if (IsOnBoard(behindX, behindY))
                    {
                        SSlotData behindSlot = states[behindX][behindY];
                        if (behindSlot.pt == EPipeType.None)
                        {
                            Debug.LogError("Step " + (_correctMoves.Count - 1 - k) + " is wrong - cell behind destination is free");
                            return false;
                        }
                        else
                        if (behindSlot.pt == EPipeType.Colored && behindSlot.c == slotFrom.c && behindSlot.p == slotFrom.p)
                        {
                            Debug.LogError("Step " + (_correctMoves.Count - 1 - k) + " is wrong - cell behind destination is the same - combine insted move");
                            return false;
                        }
                    }
                }
                // do move 

                slotTo.pt = slotFrom.pt;
                slotTo.p = slotFrom.p;
                slotTo.c = slotFrom.c;
                slotFrom.c = -1;
                slotFrom.p = -1;
                slotFrom.pt = EPipeType.None;
            }
            else
            //if (moveInfo.MoveType == EMoveType.Divide)
            {
                // try to combine
                SSlotData slotTo = states[moveInfo.ToX][moveInfo.ToY];
                if (slotFrom.pt != EPipeType.Colored)
                {
                    Debug.LogError("Step " + (_correctMoves.Count - 1 - k) + " is wrong - can't combine not colored pipe");
                    return false;
                }
                if (slotTo.pt != EPipeType.Colored)
                {
                    Debug.LogError("Step " + (_correctMoves.Count - 1 - k) + " is wrong - cell to combine is not colored");
                    return false;
                }
                if (slotTo.c != slotFrom.c)
                {
                    Debug.LogError("Step " + (_correctMoves.Count - 1 - k) + " is wrong - can't combine different colors");
                    return false;
                }
                if (slotTo.p != slotFrom.p)
                {
                    Debug.LogError("Step " + (_correctMoves.Count - 1 - k) + " is wrong - can't combine different params");
                    return false;
                }
                // combine
                slotFrom.pt = EPipeType.None;
                slotFrom.p = -1;
                slotFrom.c = -1;
                slotTo.p += 1;
                if (slotTo.p >= Consts.MAX_COLORED_LEVELS)
                {
                    // destroy max
                    slotTo.pt = EPipeType.None;
                    slotTo.p = -1;
                    slotTo.c = -1;
                }
                //
            }
        }
        for (int i = 0; i < states.Count; ++i)
        {
            for (int j = 0; j < states[i].Count; ++j)
            {
                if (states[i][j].pt != _neededStates[i][j].pt ||
                    states[i][j].c != _neededStates[i][j].c ||
                    states[i][j].p != _neededStates[i][j].p)
                {
                    Debug.LogError("Needed cell is not matches!: " + i + "/" + j);
                    return false;
                }
            }
        }
        Debug.Log("Level is correct");
        return true;
	}
}

//_maxMovesCount, _dividesPercentage