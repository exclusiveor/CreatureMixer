using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using TObject.Shared;

public class Localer : MonoBehaviour
{
	/*
	http://docs.unity3d.com/ScriptReference/SystemLanguage.html
	http://docs.unity3d.com/Manual/StyledText.html
	*/

	private static Dictionary<string,string> _textBase;
	private static string _defaultLocale = "English";

	public static void Init()
	{
		_textBase = new Dictionary<string,string>();
		Reload(GetSistemLanguage());
	}

	public static void Reload(string locale = "English")
	{
		if (_textBase == null)
		{
			_textBase = new Dictionary<string,string>();
		}
		
		#if UNITY_STANDALONE
		string path = Application.streamingAssetsPath + "/Locales/text/text.xml"; // string path = Application.streamingAssetsPath + "/Locales/" + locale + "/text/text.xml";

		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(path);

		XmlNodeList textsList = xmlDoc.GetElementsByTagName("String");
		foreach (XmlNode textInfo in textsList)
		{
			_textBase.Add(textInfo.Attributes["id"].Value, NormalizeDataString(textInfo.InnerText));
		}
		#else
        TextAsset _localeString = Resources.Load<TextAsset>("Data/Locales/" + locale + "/text/text");

        if (_localeString == null)
        {
            Debug.LogWarning("CAN'T FIND LOCALE '" + locale + "'. LOADING DEFAULT LOCALE '" + _defaultLocale + "'.");
            _localeString = Resources.Load<TextAsset>("Data/Locales/" + _defaultLocale + "/text/text");
        }

        NanoXMLDocument document = new NanoXMLDocument(_localeString.text);
        NanoXMLNode RotNode = document.RootNode;

        foreach (NanoXMLNode node in RotNode.SubNodes)
        {
            if (node.Name.Equals("String"))
            {
                _textBase.Add(node.GetAttribute("id").Value, NormalizeDataString(node.Value));
            }
        }
		#endif
    }

	public static string GetText(string id)
	{
		if ( _textBase != null && _textBase.ContainsKey(id) )
		{
			return _textBase[id];
		}
		else
		{
			return "#"+id+"#";
		}
	}
	
	public static string GetSistemLanguage()
	{
		return Application.systemLanguage.ToString();
	}

	public static string GetDefaultLocale()
	{
		return _defaultLocale;
	}

	private static string NormalizeDataString(string ampersandTaggetString)
	{
		ampersandTaggetString = ampersandTaggetString.Replace("&lt;", "<");
		ampersandTaggetString = ampersandTaggetString.Replace("&gt;", ">");
		ampersandTaggetString = ampersandTaggetString.Replace("&#13;", "\n");
		ampersandTaggetString = ampersandTaggetString.Replace("\r", "\n");
		return ampersandTaggetString;
	}
	
}
