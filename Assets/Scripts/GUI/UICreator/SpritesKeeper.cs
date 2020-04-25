using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpritesKeeper : MonoBehaviour
{
	public List<Sprite> Sprites;
	public List<string> Names;
	private Dictionary<string, Sprite> Dict;

	void Awake()
	{
		Dict = new Dictionary<string, Sprite>();
		ReformKeeper();
	}

	public void ReformKeeper()
	{
		Dict.Clear();
		int min = Mathf.Min(Sprites.Count, Names.Count);
		if (min == 0)
		{
			// no sprites or names
		} else
		{
			for (int i = 0; i < min; ++i)
			{
				Dict.Add(Names[i], Sprites[i]);
			}
		}
	}

	public Sprite GetSprite(string name)
	{
		Sprite res = null;
		Dict.TryGetValue(name, out res);
		return res;
	}
}
