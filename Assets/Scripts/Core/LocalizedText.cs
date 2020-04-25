using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string TextID = "";

	// Use this for initialization
    void Start()
    {
        Text text = GetComponent<Text>();
        if (text != null)
        {
            text.text = Localer.GetText(TextID);
        }
        else
        {
            Debug.LogError("Can't set localized text: Text component not found in " + gameObject.name);
        }
	}
}
