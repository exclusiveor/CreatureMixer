using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.Threading;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour {

	public Slider Progress;
	public Text ProgressText;	
	private AsyncOperation async;

	void Start () 
	{
		StartCoroutine(LoadAsynk());
	}

	public IEnumerator LoadAsynk()
	{		
		string level = GameFlow.SceneToTransit;
		Progress.value = 0;
		ProgressText.text = "0%";
		//async = Application.LoadLevelAsync (level);
        async = SceneManager.LoadSceneAsync(level);
		while(!async.isDone) 
		{
			int loadProgress = (int)(async.progress * 100);
			ProgressText.text = loadProgress.ToString() + "%";
			Progress.value = async.progress;
			yield return null;
		}
	}

	private void TransitEnable()
	{
		async.allowSceneActivation = true;
	}
}