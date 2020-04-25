using UnityEngine;
using System.Collections.Generic;

public enum EMoveType
{
	None = 0,
	Move = 1,
	Divide = 2 // or Combine
}

[System.Serializable]
public struct MoveInfo
{
	public int FromX;
	public int FromY;
	public int ToX;
	public int ToY;
	public EMoveType MoveType;
}

[CreateAssetMenu(fileName = "level_", menuName = "Create Level", order = 1)]
public class ScriptableLevelData : ScriptableObject 
{
	public int                   		Id;
	public int                   		MinMovesCount;
	[HideInInspector]
	public int							W = 5;
	[HideInInspector]
	public int							H = 5;
	[HideInInspector]
	public List<SSlotData>				StartStates;
	[HideInInspector]
	public List<SSlotData>				NeededStates;
	public List<MoveInfo>				CorrectMoves;
	//
	public int          	           BlockersCount = 0;
	public int                  	   HolesCount = 0;
	public int                  	   MaxMovesCount = 0;  // for autogenerator only
	public int                    	   DividesPercentage = 0;
}