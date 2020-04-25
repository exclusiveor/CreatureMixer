using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCameraScaler : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        Camera camera = GetComponent<Camera>();
        float curSize = camera.orthographicSize;
        // 0.58 = 600x1024 ()
        // 0.75 = 768x1024 (3:4)       7.68
        // 0.5625 = 1080x1920 (16:9)   9.6

        float curAspectRatio = (float)Screen.width / (float)Screen.height;
        float iPhoneX = (1125.0f / 2436.0f);
        //float scale = curAspectRatio / fullHDAsperctRatio;
        if (curAspectRatio == iPhoneX)
        {
            camera.orthographicSize = 10.7f;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
