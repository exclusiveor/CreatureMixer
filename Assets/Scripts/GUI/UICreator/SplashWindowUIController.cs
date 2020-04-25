using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;
using System.IO;

public class SplashWindowUIController : BaseUIController {

	public struct SplashData 
	{
		public bool CanSkip;
		public float ShowTime;
		public Sprite ASprite;
		public string Path;
	};

	private const float TRANSITION_TIME = 0.5f;

	private bool _canSkip;
	public int Current { get; set; }
	public List<string> Paths { get; set; }
	public List<SplashData> Splashes { get; set; }
	public int ImageToLoad { get; set; }
    public Image ImageA;
    public Image ImageB;
    public CanvasGroup GroupA;
    public CanvasGroup GroupB;
    public GameObject Dark;

    new void Awake()
	{
		//MusicManager.PlayMainMenuTrack();
	}

	// Use this for initialization
	void Start ()
	{        
        CreateImages();
	}

	IEnumerator LoadNextImage() 
	{
		#if UNITY_STANDALONE
		++ImageToLoad;
		if (Splashes.Count <= ImageToLoad)
		{
			yield return null;
		} else
		{
			string url = "file://" + Application.streamingAssetsPath + "/" + Splashes[ImageToLoad].Path;
			WWW www = new WWW(url);
			yield return www;
			SplashData sData = Splashes[ImageToLoad];
			sData.ASprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(www.texture.width / 2, www.texture.height / 2));
			Splashes[ImageToLoad] = sData;
			if (ImageToLoad == 0)
			{
				ShowFirstImage();
			}
			StartCoroutine("LoadNextImage");
		}
		#else
		++ImageToLoad;
		if (Splashes.Count <= ImageToLoad)
		{
			yield return null;
		} else
		{
		string url = Splashes[ImageToLoad].Path;
		Sprite sprite = Resources.Load<Sprite>("Art/splash/" + url);
		SplashData sData = Splashes[ImageToLoad];
		sData.ASprite = sprite;
		Splashes[ImageToLoad] = sData;
		if (ImageToLoad == 0)
		{
		ShowFirstImage();
		}
		StartCoroutine("LoadNextImage");
		}
		#endif
	}

	private void ShowFirstImage()
	{
		Current = 0;
		GroupA.alpha = 1;
		GroupB.alpha = 0;
		ImageA.sprite = Splashes[Current].ASprite;
		////>ImageA.SetNativeSize();
		int current = Current;
		LeanTween.delayedCall(ImageA.gameObject,  Splashes[Current].ShowTime + TRANSITION_TIME, ()=>{ SkipSplash(current); });
		Helpers.FadeOutCanvasGroup(Dark, Dark.GetComponent<CanvasGroup>(), TRANSITION_TIME);
		Invoke("SetCanSkip", TRANSITION_TIME);
	}

	private void ShowNextImage()
	{
		_canSkip = false;
		LeanTween.cancel(ImageA.gameObject);
		LeanTween.cancel(ImageB.gameObject);
		++Current;
		if (Current >= Splashes.Count)
		{
            // close
            TransitToMainMenu();
			return;
		}
		//
		ImageB.sprite = Splashes[Current - 1].ASprite;
		ImageA.sprite = Splashes[Current].ASprite;
		////>ImageA.SetNativeSize();
		////>ImageB.SetNativeSize();
		GroupA.alpha = 0;
		GroupB.alpha = 1;
		Helpers.FadeInCanvasGroup(ImageA.gameObject, GroupA, TRANSITION_TIME, false);
		Helpers.FadeOutCanvasGroup(ImageB.gameObject, GroupB, TRANSITION_TIME, false);
		Invoke("SetCanSkip", TRANSITION_TIME);
		int current = Current;
		LeanTween.delayedCall(ImageA.gameObject,  Splashes[Current].ShowTime + TRANSITION_TIME, ()=>{ SkipSplash(current); });
	}

	private void SkipSplash(int id)
	{
		if (Current == id && _canSkip)
		{
			ShowNextImage();
		}
	}

    private void TransitToMainMenu()
    {
        Helpers.FadeInCanvasGroup(Dark, Dark.GetComponent<CanvasGroup>(), 0.5f);
        Invoke("Transit", 0.5f);
    }

	//Image img = transform.Find("SplashImage").GetComponent<Image>();

//	IEnumerator SetImage() 
//	{
//		string url = "file://" + Application.streamingAssetsPath + "/" + "SplashScreen.png";
//		WWW www = new WWW(url);
//		yield return www;
//		Image img = transform.Find("SplashImage").GetComponent<Image>();
//		img.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(www.texture.width / 2, www.texture.height / 2));
//	
//		GameObject Dark = transform.Find("SplashImage").Find("Black").gameObject;
//		Helpers.FadeOutCanvasGroup(Dark, Dark.GetComponent<CanvasGroup>(), 0.5f);
//		Invoke("SetCanClose", 0.5f);
//		if (Consts.SPLASH_SCREEN_AUTO_TIME > 0)
//		{
//			Invoke("SplashImageOnClick", Consts.SPLASH_SCREEN_AUTO_TIME);
//		}
//	}

	private void SetCanSkip()
	{
		_canSkip = true;
	}

	private void CreateImages()
	{
		Splashes = new List<SplashData>();
		Current = -1;
		ImageToLoad = -1;
		_canSkip = false;

		#if UNITY_STANDALONE
		string path = Application.streamingAssetsPath + "/splashdata.xml";
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(path);
		XmlNodeList showList = xmlDoc.GetElementsByTagName("SplashData");
		#else
		TextAsset xmlAsset = Resources.Load<TextAsset>("Data/Splash/splashdata");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xmlAsset.text);
		XmlNodeList showList = xmlDoc.GetElementsByTagName("SplashData");
		#endif
		foreach (XmlNode aInfo in showList)
		{
			SplashData sData;
			sData.Path = aInfo.Attributes["path"].Value;
			sData.ShowTime = float.Parse(aInfo.Attributes["showtime"].Value);
			sData.CanSkip = aInfo.Attributes["canskip"].Value == "true";
			sData.ASprite = null;
			Splashes.Add(sData);
		}

        //		TextAsset splashXml = Resources.Load<TextAsset>("Data/Splash/splashdata"); .....
        //		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
        //		xmlDoc.LoadXml(splashXml.text); // load the file.
        //		XmlNodeList imagesList = xmlDoc.GetElementsByTagName("SplashData"); // array of the nodes.
        //		foreach (XmlNode imageInfo in imagesList)
        //		{
        //			SplashData sData;
        //			sData.Path = imageInfo.Attributes["path"].Value;
        //			sData.ShowTime = float.Parse(imageInfo.Attributes["showtime"].Value);
        //			sData.CanSkip = imageInfo.Attributes["canskip"].Value == "true";
        //			sData.ASprite = null;
        //			Splashes.Add(sData);
        //		}

        if (Splashes.Count == 0)
        {
            TransitToMainMenu();
        }
        else
        {
            StartCoroutine("LoadNextImage");
        }
	}

    public void DarkOnClick()
	{
		if (!_canSkip || !Splashes[Current].CanSkip)
		{
			return;
		}
		ShowNextImage();
	}

	private void Transit()
	{
		GameManager.Instance.GameFlow.TransitToScene(UIConsts.SCENE_ID.GAME_SCENE);
	}

	public override void Show ()
	{
		gameObject.GetComponent<RectTransform>().anchoredPosition3D = UIConsts.STOP_POSITION;
	}

}
