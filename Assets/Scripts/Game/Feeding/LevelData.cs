//using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine;

[System.Serializable]
public class SSlotData
{
    public int x;
    public int y;
	public EPipeType pt;  // pipes type
	public int p;   // parameter
	public int c;   // color
	
	public SSlotData() 
	{
		x = 0;
		y = 0;
		pt = EPipeType.None;
		p = -1;
		c = 0;
	}

	public SSlotData(int xx, int yy, EPipeType ppt, int pp, int cc)
    {
        x = xx;
        y = yy;
		pt = ppt;
		p = pp;
		c = cc;
	}
}

[System.Serializable]
public class LevelData
{
    public List<long>      	Resources;
	public List<SSlotData>  Slots;
    public int              ReshufflePowerups;
    public int              BreakePowerups;
    public int              ChainPowerups;
    public int              DestroyColorsPowerups;
    public int              SwapPowerups;
    public bool             AddsViewed; // for getting Chain Powerup
    public List<int>        QueueState;
    // statistic
    public float			timePlayed;

	public LevelData() 
    {
		Slots = new List<SSlotData>();
		Resources = new List<long>();
        QueueState = new List<int>();
        ReshufflePowerups = 0;
        BreakePowerups = 0;
        ChainPowerups = 0;
        DestroyColorsPowerups = 0;
        SwapPowerups = 0;
        AddsViewed = false;
		for (int i = 0; i < Consts.COLORS.Length; ++i)
		{
			Resources.Add(0);
		}
        //for (int i = 0; i <= QueuePanel.SIZE; ++i)
        //{
        //    QueueState.Add(GameManager.Instance.BoardData.GetRandomColor());
        //}
		timePlayed = 0;
	}

}