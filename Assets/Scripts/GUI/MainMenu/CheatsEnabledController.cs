using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CheatsEnabledController : MonoBehaviour {
    public Text Text;

	// Use this for initialization
	void Start () {
        LeanTween.scale(gameObject, Vector3.one * 2.0f, 2.5f);
        Destroy(gameObject, 3.0f);

        if (Cheats.IsCheatEnabled)
        {
            // turn off            
            Text.text = "Cheats enabled!";
        } else
        {
            // turn on
            Text.text = "Cheats disabled!";
        }
    }
}
