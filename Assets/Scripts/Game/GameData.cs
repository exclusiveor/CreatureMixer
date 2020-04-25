//=====================================================================================
// Zanuda Games - Kozhemyakin Vitaliy, Mykhailo Zhovnach, Taras Lishchuk
//=====================================================================================

using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class GameData
{
    public enum PowerUpType
    {
        Reshuffle = 0,
        Breake = 1,
        Chain = 2,
        DestroyColor = 3
    }
    
	public Dictionary<ETrophyType, TrophyData> XMLtrophiesData;
    public Dictionary<string, List<Vector3>> XMLSplineData;
	public Dictionary<string, TutorialData> XMLtutorialsData;
	public List<string> XMLhelpPagesData; // what tutors show in HelpWindow

    public LevelData StartLevelData;

    public GameData()
    {

    }

    //loads all data from various XML sources
    public void Load()
    {
		LoadTutorialsData();
        LoadSplineData();
		LoadHelpPagesData();
		//LoadTrophiesData();
        LoadStartLevelData();
    }

    private void LoadSplineData()
    {
        XMLSplineData = new Dictionary<string, List<Vector3>>();
        TextAsset splinesXml = Resources.Load<TextAsset>("Data/Splines");
        XmlReader reader = XmlReader.Create(new StringReader(splinesXml.text));

        while (reader.Read())
        {
            if (reader.IsStartElement("ASpline"))
            {
                reader.ReadStartElement("ASpline");
                while (reader.ReadToNextSibling("Spline"))
                {
                    List<Vector3> points = new List<Vector3>();
                    reader.ReadToFollowing("Name");
                    string splineName = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("APoint");
                    reader.ReadStartElement("APoint");
                    while (reader.ReadToNextSibling("PointF"))                    
                    {
                        reader.ReadToFollowing("X");
                        float x = reader.ReadElementContentAsFloat();
                        reader.ReadToFollowing("Y");
                        float y = reader.ReadElementContentAsFloat();
                        points.Add(new Vector3(x, y, 0));
                        reader.Read();
                    };
                    XMLSplineData[splineName] = points;
                    reader.ReadToFollowing("Category");
                    reader.ReadElementContentAsString();
                    reader.Read();
                }
            }
        }
    }

	private void LoadTutorialsData()
	{
		XMLtutorialsData = new Dictionary<string, TutorialData>();
		
		TextAsset tutorXml = Resources.Load<TextAsset>("Data/Tutorials/tutorialsdata");
		
		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.LoadXml(tutorXml.text); // load the file.
		XmlNodeList itemsList = xmlDoc.GetElementsByTagName("item"); // array of the tutorials nodes.
		
		foreach (XmlNode itemInfo in itemsList)
		{
			TutorialData tData = new TutorialData();
			tData.Id = itemInfo.Attributes["id"].Value;
			tData.Type = itemInfo.Attributes["type"].Value;
			tData.X = float.Parse(itemInfo.Attributes["x"].Value);
			tData.Y = float.Parse(itemInfo.Attributes["y"].Value);
			tData.IsArrow = itemInfo.Attributes["isarrow"].Value == "true";
			tData.Align = itemInfo.Attributes["align"].Value;
			XMLtutorialsData[tData.Id] = tData;
		}
	}

//	private void LoadTrophiesData()
//	{
//		XMLtrophiesData = new Dictionary<ETrophyType, TrophyData>();
//
//		TextAsset aXml = Resources.Load<TextAsset>("Data/TrophiesData");
//		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
//		xmlDoc.LoadXml(aXml.text); // load the file.
//		XmlNodeList itemsList = xmlDoc.GetElementsByTagName("trophy");
//
//		foreach (XmlNode itemInfo in itemsList)
//		{
//			string sid = itemInfo.Attributes["id"].Value;
//			ETrophyType id = (ETrophyType)System.Enum.Parse( typeof( ETrophyType ), sid );
//			TrophyData tData = new TrophyData();
//			tData.Reward = int.Parse(itemInfo.Attributes["reward"].Value);
//			tData.Param = int.Parse(itemInfo.Attributes["param"].Value);
//			tData.IsSingle = bool.Parse(itemInfo.Attributes["issingle"].Value);
//			XMLtrophiesData.Add(id, tData);
//		}
//	}

	private void LoadHelpPagesData()
	{
		XMLhelpPagesData = new List<string>();
		XMLhelpPagesData.Add("1");
		XMLhelpPagesData.Add("2");
		XMLhelpPagesData.Add("3");
		XMLhelpPagesData.Add("4");
		XMLhelpPagesData.Add("5");
		XMLhelpPagesData.Add("6");
		XMLhelpPagesData.Add("7");
		XMLhelpPagesData.Add("8");
		XMLhelpPagesData.Add("9");
		XMLhelpPagesData.Add("10");
		XMLhelpPagesData.Add("11");
		XMLhelpPagesData.Add("12");
		XMLhelpPagesData.Add("13");
		XMLhelpPagesData.Add("14");
		XMLhelpPagesData.Add("15");
		XMLhelpPagesData.Add("16");
		XMLhelpPagesData.Add("17");
		XMLhelpPagesData.Add("18");
		XMLhelpPagesData.Add("19");
		XMLhelpPagesData.Add("20");
		XMLhelpPagesData.Add("21");
		XMLhelpPagesData.Add("22");
		XMLhelpPagesData.Add("23");
		XMLhelpPagesData.Add("24");
		XMLhelpPagesData.Add("25");
		XMLhelpPagesData.Add("26");
		XMLhelpPagesData.Add("27");
		XMLhelpPagesData.Add("28");
		XMLhelpPagesData.Add("29");
		XMLhelpPagesData.Add("30");
		XMLhelpPagesData.Add("31");
		XMLhelpPagesData.Add("32");
		XMLhelpPagesData.Add("33");
	}

    private void LoadStartLevelData()
    {
        StartLevelData = new LevelData();
        TextAsset levelXml = Resources.Load<TextAsset>("Data/startleveldata");

        XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
        xmlDoc.LoadXml(levelXml.text); // load the file.
        XmlNodeList slotsList = xmlDoc.GetElementsByTagName("slot"); // array of the slots nodes.

        foreach (XmlNode slotInfo in slotsList)
        {
            SSlotData sData = new SSlotData();
            sData.x = int.Parse(slotInfo.Attributes["x"].Value);
            sData.y = int.Parse(slotInfo.Attributes["y"].Value);
            sData.pt = (EPipeType)System.Enum.Parse(typeof(EPipeType), slotInfo.Attributes["pt"].Value);
            sData.p = int.Parse(slotInfo.Attributes["p"].Value);
            sData.c = int.Parse(slotInfo.Attributes["c"].Value);
            StartLevelData.Slots.Add(sData);
        }

        StartLevelData.ReshufflePowerups = Consts.POWERUPS_RESHUFFLE_AT_START;
        StartLevelData.BreakePowerups = Consts.POWERUPS_BREAKE_AT_START;
        StartLevelData.ChainPowerups = Consts.POWERUPS_CHAIN_AT_START;
        StartLevelData.DestroyColorsPowerups = Consts.POWERUPS_DESTROY_COLOR_AT_START;
        StartLevelData.AddsViewed = false; // set true for paid version
    }

}