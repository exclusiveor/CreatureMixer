using UnityEngine;
using System;

[ExecuteInEditMode]
public class SetBundleVersion : MonoBehaviour
{
    
    // Date of build
    public int ayear;
    public int amonth;
    public int aday;
    
    // Write build date
    #if UNITY_EDITOR
    public void Awake() {
        if (Application.isEditor && !Application.isPlaying) {
            DateTime nowDate = System.DateTime.Now;
            ayear = nowDate.Year;
            amonth = nowDate.Month;
            aday = nowDate.Day;
			UnityEditor.PlayerSettings.bundleVersion = string.Format("{0:0000}{1:00}{2:00}", ayear, amonth, aday);
        }
    }
    #endif
}