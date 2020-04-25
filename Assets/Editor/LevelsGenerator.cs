using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelsGenerator : EditorWindow
{
	private GUIStyle guiStyle = new GUIStyle();

	private string 					_level_string = "0-9";
	private int                     _level_First = 0;
	private int                     _level_Last = 98;
	private float					_levelsCount = 1;

	private string 					_blockersCount_string = "0-0";
	private int                     _blockersCount_First = 0;
	private int                     _blockersCount_Last = 0;
	private float					_blockersCount_increment = 0;

	private string 					_holesCount_string = "0-0";
	private int                     _holesCount_First = 0;
	private int                     _holesCount_Last = 0;
	private float					_holesCount_increment = 0;

	private string 					_maxMovesCount_string = "10-30";
	private int                     _maxMovesCount_First = 10;
	private int                     _maxMovesCount_Last = 30;
	private float					_maxMovesCount_increment = 0;

	private string 					_dividesPercentage_string = "70-50";
	private int                     _dividesPercentage_First = 50;
	private int                     _dividesPercentage_Last = 50;
	private float					_dividesPercentage_increment = 0;

	[MenuItem ("Window/LEVELS GENERATOR")]
	public static void  ShowWindow ()
	{
		EditorWindow.GetWindow(typeof(LevelsGenerator));
	}

	void OnGUI ()
	{
		// GUI style
		guiStyle.fontSize = 20;
		guiStyle.fontStyle = FontStyle.Bold;
		guiStyle.alignment = TextAnchor.MiddleCenter;
		//
		GUILayout.Label ("LEVELS", EditorStyles.boldLabel);
		_level_string = GUILayout.TextField(_level_string,7,guiStyle);
		GUILayout.Label ("BLOCKERS", EditorStyles.boldLabel);
		_blockersCount_string = GUILayout.TextField(_blockersCount_string,5,guiStyle);
		GUILayout.Label ("HOLES", EditorStyles.boldLabel);
		_holesCount_string = GUILayout.TextField(_holesCount_string,5,guiStyle);
		GUILayout.Label ("MOVES", EditorStyles.boldLabel);
		_maxMovesCount_string = GUILayout.TextField(_maxMovesCount_string,5,guiStyle);
		GUILayout.Label ("DIVIDES %", EditorStyles.boldLabel);
		_dividesPercentage_string = GUILayout.TextField(_dividesPercentage_string,5,guiStyle);
		//
		GUILayout.Label ("-----------------------------", EditorStyles.boldLabel);
		if (GUILayout.Button("GENERATE"))
		{
			if (TakeParamsFromString())
			{
				Generate();
			}
		}	
	}

	private bool TakeParamsFromString()
	{
		string[] levels = _level_string.Split('-');
		if (levels.Length != 2)
		{
			Debug.LogError("Wrong Levels Count");
			return false;
		}
		if (!int.TryParse(levels[0], out _level_First) ||
			!int.TryParse(levels[1], out _level_Last))
			{
				Debug.LogError("Wrong Levels");
				return false;
			}
		_levelsCount = _level_Last - _level_First;
		if (_levelsCount <= 0)
		{
			Debug.LogError("WrongLevels - 0");
			return false;
		}

		string[] blockers = _blockersCount_string.Split('-');
		if (blockers.Length != 2)
		{
			Debug.LogError("Wrong Blockers Count");
			return false;
		}
		if (!int.TryParse(blockers[0], out _blockersCount_First) ||
			!int.TryParse(blockers[1], out _blockersCount_Last))
		{
			Debug.LogError("Wrong Blockers");
			return false;
		}
		_blockersCount_increment = (_blockersCount_Last - _blockersCount_First) / _levelsCount;

		string[] holes = _holesCount_string.Split('-');
		if (holes.Length != 2)
		{
			Debug.LogError("Wrong Holes Count");
			return false;
		}
		if (!int.TryParse(holes[0], out _holesCount_First) ||
			!int.TryParse(holes[1], out _holesCount_Last))
		{
			Debug.LogError("Wrong Holes");
			return false;
		}
		_holesCount_increment = (_holesCount_Last - _holesCount_First) / _levelsCount;

		string[] moves = _maxMovesCount_string.Split('-');
		if (moves.Length != 2)
		{
			Debug.LogError("Wrong Moves Count");
			return false;
		}
		if (!int.TryParse(moves[0], out _maxMovesCount_First) ||
			!int.TryParse(moves[1], out _maxMovesCount_Last))
		{
			Debug.LogError("Wrong Moves");
			return false;
		}
		_maxMovesCount_increment = (_maxMovesCount_Last - _maxMovesCount_First) / _levelsCount;

		string[] divides = _dividesPercentage_string.Split('-');
		if (divides.Length != 2)
		{
			Debug.LogError("Wrong Divides Count");
			return false;
		}
		if (!int.TryParse(divides[0], out _dividesPercentage_First) ||
			!int.TryParse(divides[1], out _dividesPercentage_Last))
		{
			Debug.LogError("Wrong Divides");
			return false;
		}
		_dividesPercentage_increment = (_dividesPercentage_Last - _dividesPercentage_First) / _levelsCount;
		return true;
	}

	private void Generate()
	{
		int n = 0;
		string path = "Assets/Resources/Levels/"; //string path = Application.dataPath + "/Resources/Levels/";
		Debug.Log(path);
		for (int i = _level_First; i <= _level_Last; ++i)
		{
			ScriptableLevelData asset = ScriptableObject.CreateInstance<ScriptableLevelData> ();
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "Level_" + i.ToString() + ".asset");
			asset.Id = i;
			asset.HolesCount = (int)(_holesCount_First + n * _holesCount_increment);
			asset.BlockersCount = (int)(_blockersCount_First + n * _blockersCount_increment);
			asset.MaxMovesCount = (int)(_maxMovesCount_First + n * _maxMovesCount_increment);
			asset.DividesPercentage = (int)(_dividesPercentage_First + n * _dividesPercentage_increment);
			asset.MinMovesCount = FeedingLevelEditor.FLAG_TO_GENERATE_LEVEL; // means we need to generate level
			//Debug.Log(assetPathAndName);
			AssetDatabase.CreateAsset (asset, assetPathAndName);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
			++n;
		}
		//AssetDatabase.Refresh();
	}

//	private void LoadResourses()
//	{
//		string fileName = EditorUtility.OpenFilePanel("open level", "\\..\\", "xml");
//		if (fileName.ToString() == "") { return; }
//
//		List<string> pashSplited = new List<string>(fileName.Split(new char[] { '/' }));
//		//string formName = pashSplited[pashSplited.Count - 1].Split(new char[] { '.' })[0];
//		pashSplited.RemoveAt(pashSplited.Count - 1);
//		string path = String.Join("/", pashSplited.ToArray()) + "/RestoreWindow/";
//
//		Debug.Log(fileName);
//		//Debug.Log(formName);
//		Debug.Log(path);
//
//		FileUtil.DeleteFileOrDirectory("Assets/Resources/art/RestoreWindow/"); // + formName);
//		FileUtil.ReplaceDirectory(path, "Assets/Resources/art/RestoreWindow/");   //FileUtil.ReplaceDirectory(path + "/" + formName , "Assets/Difference/Resources/difference_levels/" + formName);
//
//		AssetDatabase.Refresh();
//	}

}
