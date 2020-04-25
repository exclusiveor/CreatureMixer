 using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum EGameState
{
	Pause = 0,
	Play = 1,
	Loose = 2
}

public class GameBoardData
{
	private List<long>							_resources;
	private long								_pointsForSequences;
	public EGameState 							GameState = EGameState.Pause;
	public float 								TimePlayed;

	public SSlot                  				DragSlot;
	public GameBoard							AGameBoard;
    public Dictionary<GameData.PowerUpType,int> PowerUps;    
	public bool 								AddsViewed;

    private int									_movesToNextPipe;			// for turn base game
	public int									_allTurns;					// for all game types
	private int									_pipesAdded;
	private int									_pipesToNextBlocker;

	// leveled
	public float								StarsGained;
	public int									MovesLeft;
	//

    public GameBoardData()
    {
        _movesToNextPipe = 0;
        if (GameBoard.AddingType != EAddingType.OnNoMatch)
        {
            _movesToNextPipe = Consts.TURNS_TO_NEXT_PIPE;
        }
        _allTurns = 0;
        _pipesAdded = 0;
        _pipesToNextBlocker = Consts.PIPES_TO_NEXT_BLOCKER;
        _pointsForSequences = 0;
        _resources = new List<long>();
        for (int i = 0; i < Consts.CLASSIC_GAME_COLORS; ++i)
        {
            _resources.Add(0);
            //SetResourceForce(0, i);
        }
		AddsViewed = false;
        GameState = EGameState.Pause;
        TimePlayed = 0;
        DragSlot = null;
        PowerUps = new Dictionary<GameData.PowerUpType, int>();
        foreach (GameData.PowerUpType powerUp in Enum.GetValues(typeof(GameData.PowerUpType)))
        {
            PowerUps.Add(powerUp, 0);
        }
    }

	public void Reset()
	{
		_movesToNextPipe = Consts.TURNS_TO_NEXT_PIPE;
		_allTurns = 0; // for leveled too
		_pipesAdded = 0;
		_pipesToNextBlocker = Consts.PIPES_TO_NEXT_BLOCKER;
        _pointsForSequences = 0;
		for (int i = 0; i < _resources.Count; ++i)
		{
			_resources[i] = 0;
		}
		GameState = EGameState.Pause;
		TimePlayed = 0;
		DragSlot = null;
		AddsViewed = false;

//		// leveled
//		StarsGained = 0;
//		MovesLeft = 0;
//		//

//        // powerups
//        EventData eventData = new EventData("OnPowerUpsResetNeededEvent");
//        //eventData.Data["isforce"] = true;
//        GameManager.Instance.EventManager.CallOnPowerUpsResetNeededEvent(eventData);
    }

	public void SetResourceForce(long amount, int color)
	{
		_resources[color] = amount;
		EventData eventData = new EventData("OnResourcesChangedEvent");
		eventData.Data["isforce"] = true;
		GameManager.Instance.EventManager.CallOnResourcesChangedEvent(eventData);
	}
	
	public void SetResource(long amount, int color)
	{
		_resources[color] = amount;
		EventData eventData = new EventData("OnResourcesChangedEvent");
		eventData.Data["isforce"] = false;
		GameManager.Instance.EventManager.CallOnResourcesChangedEvent(eventData);
	}

	//public void AddResource(long amount, int color)
	//{
	//	long sum = _resources[color] + amount;
	//	SetResource(sum, color);
	//}

    public void AddResource(long amount, int color, Vector3 pos)
    {
        long sum = _resources[color] + amount;
        pos = AGameBoard.ConvertPositionFromLocalToScreenSpace(pos);
        EventData e = new EventData("OnShowAddResourceEffect");
        e.Data["x"] = pos.x;
        e.Data["y"] = pos.y;
        e.Data["amount"] = amount;
        e.Data["color"] = color;
        GameManager.Instance.EventManager.CallOnShowAddResourceEffect(e);
        if (Consts.SHOW_ADD_POINTS_ANIMATION)
        {
            LeanTween.delayedCall(Consts.ADD_POINTS_EFFECT_TIME, () => { SetResource(sum, color); });
        }
        else
        {
            SetResource(sum, color);
        }
    }

    public void RemoveResource(long amount, int color)
	{
		long sum = _resources[color] - amount;
		if (sum < 0)
		{
			sum = 0;
		}
		SetResource(sum, color);
	}

	public void AddResourceByLevelOfColoredPipe(int value, int color, int multiplyer, Vector3 pos) // by level of colored pipe
	{
		AddResource(Consts.POINTS[value] * multiplyer, color, pos);
        // TODO call to ResourcePanel to show "+points" animation
	}

	public long GetResourceAmount(int color)
	{
		return _resources[color];
	}

	public long GetTotalPoints()
	{
		long res = 0;
		for (int i = 0; i <  _resources.Count; ++i)
		{
			res += _resources[i];
		}
		res += _pointsForSequences;
		return res;
	}

    public void AddPointsForSequence(long points)
    {
        _pointsForSequences += points;
        EventData eventData = new EventData("OnResourcesChangedEvent");
        eventData.Data["isforce"] = false;
        eventData.Data["x"] = 0;
        eventData.Data["y"] = 0;
        GameManager.Instance.EventManager.CallOnResourcesChangedEvent(eventData);
    }
    
	public void SetGameState(EGameState astate)
	{
		GameState = astate;
	}
		
		
	public bool IsLoose()
	{
		return GameState == EGameState.Loose;
	}

	public bool IsPause()
	{
		return GameState == EGameState.Pause;
	}

	public int GetRandomColor()
	{
        //if (GameBoard.GameType == EGameType.Classic)
        //{
            return UnityEngine.Random.Range(0, Consts.CLASSIC_GAME_COLORS);
        //}
//        else
//        //if (GameBoard.GameType == EGameType.Inverse)
//        {
//            return UnityEngine.Random.Range(0, Consts.INVERSE_GAME_COLORS);
//        }
	}

	public int GetColorsCount()
	{
        //if (GameBoard.GameType == EGameType.Classic)
        //{
            return Consts.CLASSIC_GAME_COLORS;
        //} 
//		else
//        //if (GameBoard.GameType == EGameType.Inverse)
//        {
//            return Consts.INVERSE_GAME_COLORS;
//        }
	}

	public void UpdateTimer(float deltaTime)
	{
		TimePlayed += deltaTime;
	}

	public int GetMaxColoredLevels()
	{
		return Consts.MAX_COLORED_LEVELS;
	}

	public void OnTurnWasMade(bool wasMatch, bool justAddPipe)
	{
		if (GameBoard.GameType == EGameType.Leveled)
		{
			//if (!justAddPipe)
			//{
			++_allTurns;
			--MovesLeft;
			CheckLeveledWinCondition();
			//}
		} else
		{
			if (!justAddPipe && GameBoard.AddingType == EAddingType.EachXMoves)
			{
				--_movesToNextPipe;
				++_allTurns;
			}
			else
			{
				++_allTurns;
			}
			bool pipeneeded = false;
			if (justAddPipe)
			{
				pipeneeded = true;
			} else
			if (GameBoard.AddingType == EAddingType.EachXMoves)
			{
				if (_movesToNextPipe == 0)
				{
					pipeneeded = true;
				}
			} else
			if (GameBoard.AddingType == EAddingType.OnNoMatch)
			{
				if (!wasMatch)
				{
					pipeneeded = true;
				}
			}

			if (pipeneeded)
			{
				if (GameBoard.AddingType == EAddingType.EachXMoves && _movesToNextPipe == 0)
				{
					_movesToNextPipe = Consts.TURNS_TO_NEXT_PIPE;
				}
				bool needBlocker = false;
				if (GameBoard.AddingType != EAddingType.OnNoMatch || Consts.USE_BLOCKERS_ON_NO_MATCH_ADDING)
				{
					needBlocker = _pipesToNextBlocker == 0;
					if (needBlocker)
					{
						_pipesToNextBlocker = Consts.PIPES_TO_NEXT_BLOCKER;
					}
					else
					{
						--_pipesToNextBlocker;
					}
				}
				// add new pipe to queue and create new
				EPipeType pipeType = EPipeType.Colored;
				if (needBlocker)
				{
					pipeType = EPipeType.Blocker;
				}
				if (pipeneeded)
				{
					if (pipeneeded && AGameBoard.AddRandomPipe(pipeType))
					{
						++_pipesAdded;
					}
					else
					{
						if (pipeType == EPipeType.Blocker)
						{
							// add blocker on next turn
							_pipesToNextBlocker = 0;
						}
					}
				}
			}
		}
	    EventData eventData = new EventData("OnTurnWasMadeEvent");
		eventData.Data["tonextpipe"] = _movesToNextPipe;
		eventData.Data["turnsmade"] = _allTurns;
		eventData.Data["pipesadded"] = _pipesAdded;
		GameManager.Instance.EventManager.CallOnTurnWasMadeEvent(eventData);
	}

	public int GetMovesToNextPipe()
	{
		return _movesToNextPipe;
	}

	private void CheckLeveledWinCondition()
	{
		if (StarsGained >= 3 || MovesLeft <= 0)
		{
			AGameBoard.OnLeveledGameCompleted();
		}
	}

}
