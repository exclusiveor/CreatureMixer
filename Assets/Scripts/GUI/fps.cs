using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class fps : MonoBehaviour
{
	float updateInterval = 0.5f;
	
	private float accum = 0.0f; // FPS accumulated over the interval
	private float frames = 0; // Frames drawn over the interval
	private float timeleft;
	private Text txt;

	void Start () { 
		timeleft = updateInterval;
		txt = gameObject.GetComponent<Text>();
	}


	void Update() {
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		++frames;
		if (timeleft <= 0.0f) {
			txt.text = (accum / frames).ToString();
			timeleft = updateInterval;
			accum = 0.0f;
			frames = 0;
		}
	}
}

