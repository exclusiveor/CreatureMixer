//=====================================================================================
//
// Game Flow
// Description: Class uses to transit between scenes, create and manage UI elements
//
// Example: GameManager.Instance.GameFlow.TransitToScene("MainMenu");
// 
// Company: ZagravaGames
// 2015/03
//=====================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TObject.Shared;
using System;
using Prime31.TransitionKit;
using UnityEngine.SceneManagement;

public class FormData
{
    public UIConsts.FORM_ID Id;
    public int Type;
	public EFormCreationMethod CreationMethod;
    public string Path;
	public int FaderType; 

	public FormData(int type, UIConsts.FORM_ID formType, string path, int faderType, EFormCreationMethod creationMethod = EFormCreationMethod.Static)
    {        
        Type = type;
        Id = formType;
        CreationMethod = creationMethod;
        Path = path;
		FaderType = faderType;
    }
}

public class SceneData
{
    public string Name;
    public List<FormData> Forms;
    public UIConsts.FORM_ID Menu;
    public UIConsts.SCENE_ID PreloadSceneName;
    public int DesignResolutionWidth;
    public int DesignResolutionHeight;
    public float DesignMatchWidthOrHeight;

    public SceneData(string name)
    {
        Menu = UIConsts.FORM_ID.NONE;
        Name = name;
        Forms = new List<FormData>();
        PreloadSceneName = UIConsts.SCENE_ID.NONE;
        DesignResolutionWidth = 0;
        DesignResolutionHeight = 0;
        DesignMatchWidthOrHeight = 1; // by default match height
    }

    public UIConsts.FORM_ID[] GetFormsArray()
    {
        List<UIConsts.FORM_ID> ids = new List<UIConsts.FORM_ID>();
        foreach(var form in Forms)
        {
            ids.Add(form.Id);
        }

        return ids.ToArray();
    }
}

public class GameFlow
{
    private UIConsts.SCENE_ID _currentScene;
	private UIConsts.FORM_ID _currentBackgroundMenu;
    private Dictionary<string, SceneData> _scenesData;
    private UICreator _uiCreator;
    private Dictionary<UIConsts.SCENE_ID, AsyncOperation> _scenePreloadersOperations;

    private List<BaseUIController> WindowsQueue;

	//
	public static GameObject CurrentActivePopUp;
	public static GameObject BaseWindow;
	public static string SceneToTransit;
	//

	public GameFlow() 
	{
        WindowsQueue = new List<BaseUIController>();
        EventManager.OnHideWindowEvent += OnHideWindowEvent;
		EventManager.OnOpenFormNeededEvent += OnOpenFormNeeded;
        _scenePreloadersOperations = new Dictionary<UIConsts.SCENE_ID, AsyncOperation>();
        _scenesData = new Dictionary<string, SceneData>();
        parseXmlData();
	}      

	private void OnOpenFormNeeded(EventData e)
	{
		UIConsts.FORM_ID formId = (UIConsts.FORM_ID)e.Data["form"];
		ShowFormFromID(formId, e);
	}

	public void InitInNewScene(UICreator uiCreator)
	{
        WindowsQueue.Clear();
        _uiCreator = uiCreator;
        CreateUiForms();        
	}

    public void OnDestroyWindow(UIConsts.FORM_ID formID) {
        _uiCreator.RemoveWindowsFromList(formID);
    }

	private void CreateUiForms()
	{
        SceneData sceneData = _scenesData[UIConsts.SCENE_NAMES[(int)_currentScene]];
        if (sceneData.Menu != UIConsts.FORM_ID.NONE )
        {
            _currentBackgroundMenu = sceneData.Menu;
        }
		_uiCreator.CreateUIs(sceneData);      
		ShowBackgroundMenu();
        if (sceneData.PreloadSceneName != UIConsts.SCENE_ID.NONE &&
            !_scenePreloadersOperations.ContainsKey(sceneData.PreloadSceneName))
        {
            GameManager.Instance.StartCoroutine(PreloadSceneAsync(sceneData.PreloadSceneName));
            //Debug.Log("Preloading " + UIConsts.SCENE_NAMES[(int)sceneData.PreloadSceneName] + " scene in background.");
        }
	}

    public UIConsts.SCENE_ID GetCurrentScene()
    {
        return _currentScene;
    }

	private void ShowBackgroundMenu()
	{
        // запуск фонового меню
		GameObject menuObj = _uiCreator.GetFormFromID(_currentBackgroundMenu);
        menuObj.SendMessage("Show");
		BaseWindow = menuObj; // menuId
    }

    public bool IsOpened(UIConsts.FORM_ID id)
    {
        // find if double
        bool isDouble = false;
        for (int i = 0; i < WindowsQueue.Count; ++i)
        {
            if (WindowsQueue[i].FormID == id)
            {
                isDouble = true;
                break;
            }
        }
        if (isDouble)
        {
            return true;
        }
        return false;
    }

	//public void ShowFormFromID(UIConsts.FORM_ID id)
	//{
	//	if (IsOpened(id))
	//	{
	//		return;
	//	}
 //       // turn on clicks catcher
 //       _uiCreator.ClicksCatcher.SetActive(true);
 //       _uiCreator.ClicksCatcher.transform.SetAsLastSibling();
 //       //
	//	GameObject windowObj = _uiCreator.GetFormFromID(id);
 //       windowObj.SetActive(true);
 //       windowObj.SendMessage("Show");
 //       windowObj.transform.SetAsLastSibling();
 //       BaseUIController window = windowObj.GetComponent<BaseUIController>();
 //       WindowsQueue.Add(window);
	//	GameManager.Instance.EventManager.CallOnShowWindowEvent();
	//}

	public void ShowFormFromID(UIConsts.FORM_ID id, EventData e)
	{
        if (id != UIConsts.FORM_ID.TUTOR_WINDOW && IsOpened(id))
        {
            // only tutorial can be opened many times
            return;
        }
        if (!_uiCreator.WindowExists(id))
        {
            _uiCreator.CreateWindow(GetFormDataByID(id));
        }

        GameObject windowObj = _uiCreator.GetFormFromID(id);
        if (windowObj == null) { return; }

        BaseUIController window = windowObj.GetComponent<BaseUIController>();
        if (window.OpenForm(e) == true)
		{
            // turn on clicks catcher
			_uiCreator.ShowClicksCatcher(GetFormDataByID(id).FaderType);
            //
            windowObj.SetActive(true);
            windowObj.SendMessage("Show");
            windowObj.transform.SetAsLastSibling();
            WindowsQueue.Add(window);
            GameManager.Instance.EventManager.CallOnShowWindowEvent();
        }
	}

    public void TransitToScene(UIConsts.SCENE_ID nextScene)
    {
		if (nextScene == UIConsts.SCENE_ID.MAINMENU || nextScene == UIConsts.SCENE_ID.SPLASH_SCENE)
		{
			FadeTransition.ExtraDelay = 0.7f;
		} else
		{
			FadeTransition.ExtraDelay = 0.1f;
		}
        if (_currentScene != UIConsts.SCENE_ID.STARTSCENE)
        {
            var fader = new FadeTransition()
            {                
                fadeToColor = Color.black
            };
            TransitionKit.instance.transitionWithDelegate(fader);
        }

        LeanTween.delayedCall(0.5f, () => TransitToSceneDelayed(nextScene));
    }

    private void TransitToSceneDelayed(UIConsts.SCENE_ID nextScene)
    {
        _uiCreator = null;
        _currentScene = nextScene;

        AsyncOperation preloadAsyncOperation;
        if (_scenePreloadersOperations.TryGetValue(nextScene, out preloadAsyncOperation))
        {
            preloadAsyncOperation.allowSceneActivation = true;
            _scenePreloadersOperations.Remove(nextScene);
        }
        else
        {
            _scenePreloadersOperations.Clear();
            //Debug.Log("GameFlow: transiting to scene <" + nextScene + ">");
            SceneManager.LoadScene(UIConsts.SCENE_NAMES[(int)nextScene]);
        }    
    }

    private IEnumerator PreloadSceneAsync(UIConsts.SCENE_ID sceneToPreload)
    {
        //AsyncOperation asyncOperation = Application.LoadLevelAsync(UIConsts.SCENE_NAMES[(int)sceneToPreload]);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(UIConsts.SCENE_NAMES[(int)sceneToPreload]);
        asyncOperation.allowSceneActivation = false;
        _scenePreloadersOperations.Add(sceneToPreload, asyncOperation);

        yield return asyncOperation;
        //Debug.Log("Preload done ____________________________");
    }   

	private void parseXmlData()
	{
		TextAsset _xmlString = Resources.Load<TextAsset>("Data/GameFlow");
				
		NanoXMLDocument document = new NanoXMLDocument(_xmlString.text);
		NanoXMLNode RotNode = document.RootNode;
		
		foreach (NanoXMLNode node in RotNode.SubNodes)
		{
			if (node.Name.Equals("screens"))
			{
				foreach (NanoXMLNode nodeScreens in node.SubNodes)
				{   
                    string sceneName = nodeScreens.GetAttribute("name").Value;
                    SceneData sceneData = new SceneData(sceneName);
                    string preloadNextScene = nodeScreens.GetAttribute("preloadNextScene").Value;
                    if (!String.IsNullOrEmpty(preloadNextScene))
                    {
                        sceneData.PreloadSceneName = convertSceneNameToID(preloadNextScene);
                    }


                    string designResolutionWidth = nodeScreens.GetAttribute("DesignResolutionWidth").Value;
                    if (!String.IsNullOrEmpty(designResolutionWidth))
                    {
                        sceneData.DesignResolutionWidth = Int32.Parse(designResolutionWidth);
                    }

                    string designResolutionHeight = nodeScreens.GetAttribute("DesignResolutionHeight").Value;
                    if (!String.IsNullOrEmpty(designResolutionHeight))
                    {
                        sceneData.DesignResolutionHeight = Int32.Parse(designResolutionHeight);
                    }

                    string designMatchWidthOrHeight = nodeScreens.GetAttribute("DesignMatchWidthOrHeight").Value;
                    if (!String.IsNullOrEmpty(designMatchWidthOrHeight))
                    {
                        sceneData.DesignMatchWidthOrHeight = float.Parse(designMatchWidthOrHeight);
                    }

                    if (nodeScreens.Name.Equals("screen"))
					{
						foreach (NanoXMLNode nodeScreensScreen in nodeScreens.SubNodes)
						{
                            string formName = nodeScreensScreen.GetAttribute("name").Value.ToString();
                            UIConsts.FORM_ID formID = (UIConsts.FORM_ID)System.Enum.Parse(typeof(UIConsts.FORM_ID), formName);
                            int formType = int.Parse(nodeScreensScreen.GetAttribute("type").Value);
							EFormCreationMethod creationMethod = EFormCreationMethod.Static;
                            string path = UIConsts.DEFAULT_UI_PATH + formName;
                            NanoXMLAttribute attrPath = nodeScreensScreen.GetAttribute("path");
                            if (attrPath != null)
                            {
                                path = attrPath.Value;
                            }
							int faderType = 1; //show transparrent
							NanoXMLAttribute attrFader = nodeScreensScreen.GetAttribute("fader");
							if (attrFader != null)
							{
								faderType = int.Parse(attrFader.Value);
							}
                            NanoXMLAttribute attr = nodeScreensScreen.GetAttribute("creation");
                            if (attr != null)
                            {
                                if (!string.IsNullOrEmpty(attr.Value))
                                {
									if (attr.Value.ToLower() == "static")
									{
										creationMethod = EFormCreationMethod.Static;
									} else
									if (attr.Value.ToLower() == "dynamic")
									{
										creationMethod = EFormCreationMethod.Dynamic;
									}
                                }
                            }

							FormData formData = new FormData(formType, formID, path, faderType, creationMethod);
                            sceneData.Forms.Add(formData);
                            if (formType == 1)
                            {
                                sceneData.Menu = formID;
                            }
						}
					}
                    _scenesData.Add(sceneName, sceneData);
                }
			}
		}
	}

    private UIConsts.SCENE_ID convertSceneNameToID(string sceneName)
    {
        for (int i = 0; i <= UIConsts.SCENE_NAMES.Length; i++)
        {
            if (UIConsts.SCENE_NAMES[i] == sceneName)
            {
                return (UIConsts.SCENE_ID)i;
            }
        }
        return UIConsts.SCENE_ID.NONE;
    }

	public void ExitGame()
	{
		MusicManager.StopAll();
		GameManager.Instance.Settings.Save(true);
		Application.Quit();
	}

	private void OnHideWindowEvent(EventData ob = null)
	{
        RemoveCurrentWindow();
        if (WindowsQueue.Count == 0)
        {
            // turn off clicks catcher
            _uiCreator.HideClicksCatcher();
            //
            GameManager.Instance.Invoke("TryShowTutorFromQueue", 0.1f);
        }
        else
        {
            //// turn off clicks catcher already shown
            //_uiCreator.ClicksCatcher.transform.SetAsLastSibling();
            //_uiCreator.ClicksCatcher.SetActive(false);
            ////
            Transform windowTransform = WindowsQueue[WindowsQueue.Count - 1].transform;
            windowTransform.SetAsLastSibling();
        }
        
	}

	public bool IsSomeWindow()
	{
        return WindowsQueue.Count > 0;
	}

    public bool IsUICreatorReady()
    {
        return _uiCreator != null;
    }

	public GameObject GetFormFromID(UIConsts.FORM_ID id)
	{
        //Debug.Log ("++++++++++++++++++++++++ " + id.ToString());
        if (_uiCreator != null)
        {
            return _uiCreator.GetFormFromID(id);
        } else
        {
            return null;
        }
	}
    public FormData GetFormDataByID(UIConsts.FORM_ID id) {
        foreach (FormData formData in _scenesData[UIConsts.SCENE_NAMES[(int)_currentScene]].Forms) {
            if (formData.Id == id) {
                return formData;
            }
        }
        return null;
    }

	public Camera GetUICamera()
	{
		if (!_uiCreator) return null;
		return _uiCreator.UiCamera;
	}

	public void SetUICamera(Camera cam)
	{
		_uiCreator.UiCamera = cam;
	}

    public UIConsts.FORM_ID GetCurrentActiveWindowId()
    {
        if (WindowsQueue.Count == 0)
        {
            return UIConsts.FORM_ID.NONE;
        }
        return WindowsQueue[WindowsQueue.Count - 1].FormID;
    }

    public void RemoveCurrentWindow()
    {
        if (WindowsQueue.Count > 0)
        {
            WindowsQueue.RemoveAt(WindowsQueue.Count - 1);
        }
    }

    public BaseUIController GetCurrentActiveWindow()
    {
        if (WindowsQueue.Count > 0)
        {
            return WindowsQueue[WindowsQueue.Count - 1];
        }
        return null;
    }

    public bool EscapeOnClick()
    {
        if (IsSomeWindow())
        {
            return WindowsQueue[0].EscapeOnClick();
        }
        return false;
    }
}
