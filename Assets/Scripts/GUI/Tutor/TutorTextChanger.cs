using UnityEngine;
using UnityEngine.UI;

public class TutorTextChanger : MonoBehaviour 
{
	public string TextId;
	void Start () 
	{
		string atext = Localer.GetText(TextId);
		#if UNITY_STANDALONE
		atext = atext.Replace("{ct}", Localer.GetText("click"));
		#else
		atext = atext.Replace("{ct}", Localer.GetText("tap"));
		#endif
		transform.GetComponent<Text>().text = atext;
	}
}