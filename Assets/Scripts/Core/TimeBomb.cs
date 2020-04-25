using UnityEngine;
using System;

[ExecuteInEditMode]
public class TimeBomb : MonoBehaviour
{    
    // Date of build
    public int ayear = 2015;
    public int amonth = 7;
    public int aday = 3;
	public int timebombLength;

    public bool BombActivated = false;
    
    // Write build date
#if UNITY_EDITOR
    public void Awake() {
        if (Application.isEditor && !Application.isPlaying) {
            DateTime nowDate = System.DateTime.Now;
            ayear = nowDate.Year;
            amonth = nowDate.Month;
            aday = nowDate.Day;
        }
    }
#else
    // Use this for initialization
    void Awake ()
    {
        DateTime deathDate = new DateTime(ayear, amonth, aday);
        DateTime nowDate = System.DateTime.Now;
        
        TimeSpan elapsed = nowDate.Subtract(deathDate);
        
        if (elapsed.TotalDays > timebombLength && BombActivated)
        {            
            PlayerPrefs.SetInt("TimeBomb", 1);
        }
        else
        {
            PlayerPrefs.SetInt("TimeBomb", 0);
        }
        
    }
#endif
}