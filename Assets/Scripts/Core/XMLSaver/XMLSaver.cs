using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

public static class XMLSaver<ObjectType>
{
    public static void Save(object obj, string fileName)
    {
        StreamWriter fs = new StreamWriter(fileName);
		Debug.Log("StartSaving");
        try
        {
            XmlSerializer xsr = new XmlSerializer(typeof(ObjectType));
            xsr.Serialize(fs, obj);
        }
        catch (SerializationException e)
        {
            Debug.Log("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }

    public static ObjectType Load(string fileName)
    {
        if (!File.Exists(fileName))
            return default(ObjectType);

        StreamReader fs = new StreamReader(fileName);
        ObjectType obj;
        try
        {
            XmlSerializer xsr = new XmlSerializer(typeof(ObjectType));
            obj = (ObjectType)xsr.Deserialize(fs);
        }
        catch (SerializationException e)
        {
            Debug.Log("Failed to deserialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
        return obj;
    }
}

//[System.Serializable]
//public class TwoMatchSlotData
//{
//    // slot
//    public int x;
//    public int y;
//    public bool dirt;
//    // pipe
//    public int pipe;
	
//    public TwoMatchSlotData() {
//        x = -1;
//        y = -1;
//        pipe = -1;
//        dirt = true;
//    }
	
//    public TwoMatchSlotData(int ax, int ay, int apipe, bool adirt) {
//        x = ax;
//        y = ay;
//        pipe = apipe;
//        dirt = adirt;
//    }
//}

//[System.Serializable]
//public class TwoMatchLevelData
//{
//    public int width;
//    public int height;
//    public List<TwoMatchSlotData> Slots;
//    public long points;
//    public int multiplyer;
	
//    public TwoMatchLevelData() 
//    {
//        Slots = new List<TwoMatchSlotData>();
//        points = 0;
//        multiplyer = 1;
//    }
	
//    public TwoMatchLevelData(int w, int h) {
//        Slots = new List<TwoMatchSlotData>();
//        width = w;
//        height = h;
//        points = 0;
//        multiplyer = 1;
//        for (int j = 0; j < height; ++j) 
//        {
//            for (int i = 0; i < width; ++i) 
//            {
//                Slots.Add(new TwoMatchSlotData(i, j, Random.Range(0, 3), true));
//                //Slots.Add(new TwoMatchSlotData(i, j, 1, true)); // for testing
//            }
//        } 
//    }
//}