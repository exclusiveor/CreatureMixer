using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class UICreator : MonoBehaviour
{
	private Dictionary<UIConsts.FORM_ID, GameObject> _uiList;
	private GameObject _newCanvas;
    private GameObject _exitCanvas;
    public GameObject ClicksCatcher;
    public Camera UiCamera { get; set; }
    public List<GameObject> WindowsAvailableAtStart;

	void Start ()
	{
        //--------------------------------------------
		_uiList = new Dictionary<UIConsts.FORM_ID, GameObject>();
		CreateCanvas();
		GameManager.Instance.GameFlow.InitInNewScene(this);
	}

	void CreateCanvas()
	{
		GameObject uiCameraObj = GameObject.Find("UICamera");
		Camera uiCamera = null;
		if (uiCameraObj != null)
		{
			uiCamera = uiCameraObj.GetComponent<Camera>();
            uiCamera.enabled = false;
            uiCamera.enabled = true;
            UiCamera = uiCamera;
		} else
		{
			UiCamera = Camera.main;
		}

		_newCanvas = GameObject.FindGameObjectWithTag("UICanvas");
		if (_newCanvas == null)
		{
			_newCanvas = new GameObject();
			_newCanvas.name = "UICanvas";
			_newCanvas.layer = 5;
			Canvas _newCanvasCanvas = _newCanvas.AddComponent<Canvas>();
			CanvasScaler _newCanvasCanvasScaler = _newCanvas.AddComponent<CanvasScaler>();
			//GraphicRaycaster _newCanvasGraphicRaycaster =_newCanvas.AddComponent<GraphicRaycaster>();
			_newCanvas.AddComponent<GraphicRaycaster>();

			//_newCanvasCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			_newCanvasCanvas.renderMode = RenderMode.ScreenSpaceCamera;

			if (uiCamera != null)
			{
				_newCanvasCanvas.worldCamera = uiCamera;
			} else
			{
				_newCanvasCanvas.worldCamera = Camera.main;
			}

			_newCanvasCanvas.sortingLayerName = Consts.SORTING_LAYER_UI;
			_newCanvasCanvas.pixelPerfect = false; //true
			_newCanvasCanvas.planeDistance = 1;

			_newCanvasCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			_newCanvasCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			_newCanvasCanvasScaler.matchWidthOrHeight = 0.0f;
			_newCanvasCanvasScaler.referenceResolution = UIConsts.DESIGN_RESOLUTION;
		} else
		{
			Canvas _newCanvasCanvas = _newCanvas.GetComponent<Canvas>();
			if (_newCanvasCanvas.worldCamera == null)
			{
				if (uiCamera != null)
				{
					_newCanvasCanvas.worldCamera = uiCamera;
				} else
				{
					_newCanvasCanvas.worldCamera = Camera.main;
				}
			}
		}

        // canvas for exit window --------------------------
		_exitCanvas = GameObject.FindGameObjectWithTag("ExitCanvas");
		if (_exitCanvas == null)
		{
			_exitCanvas = new GameObject();
			_exitCanvas.name = "ExitCanvas";
			_exitCanvas.layer = 5;
			Canvas exitCanvasCanvas = _exitCanvas.AddComponent<Canvas>();
			CanvasScaler exitCanvasCanvasScaler = _exitCanvas.AddComponent<CanvasScaler>();
			//GraphicRaycaster _newCanvasGraphicRaycaster =_newCanvas.AddComponent<GraphicRaycaster>();
			_exitCanvas.AddComponent<GraphicRaycaster>();

			//_newCanvasCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			exitCanvasCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

			if (uiCamera != null)
			{
				exitCanvasCanvas.worldCamera = uiCamera;
			} else
			{
				exitCanvasCanvas.worldCamera = Camera.main;
			}

			exitCanvasCanvas.sortingLayerName = Consts.SORTING_LAYER_UI;
			exitCanvasCanvas.pixelPerfect = false; //true
			exitCanvasCanvas.planeDistance = 1;
			exitCanvasCanvas.sortingOrder = 1;

			exitCanvasCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			exitCanvasCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			exitCanvasCanvasScaler.referenceResolution = UIConsts.DESIGN_RESOLUTION;
		} else
		{
			Canvas exitCanvasCanvas = _exitCanvas.GetComponent<Canvas>();
			if (exitCanvasCanvas.worldCamera == null)
			{
				if (uiCamera != null)
				{
					exitCanvasCanvas.worldCamera = uiCamera;
				} else
				{
					exitCanvasCanvas.worldCamera = Camera.main;
				}
			}
		}
        //
		Instantiate(Resources.Load("Prefabs/UI/UIEventSystem"));
        // create clicks catcher
        ClicksCatcher = new GameObject();
        ClicksCatcher.name = "ClicksCatcher";
        RectTransform rt = ClicksCatcher.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(2400, 2000);
        ClicksCatcher.transform.SetParent(_newCanvas.transform, false);
        Image image = ClicksCatcher.AddComponent<Image>();
        Color acolor = UIConsts.CLICKS_CATCHER_COLOR;
        acolor.a = 0;
        image.color = acolor;
        ClicksCatcher.SetActive(false);
    }    
	
	public void CreateUIs(SceneData sceneData)
	{
        CanvasScaler canvasScaler =  _newCanvas.GetComponent<CanvasScaler>();
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasScaler.referenceResolution = new Vector2(sceneData.DesignResolutionWidth, sceneData.DesignResolutionHeight);

        foreach (FormData form in sceneData.Forms)
		{
			if (form.CreationMethod == EFormCreationMethod.Static)
			{
				FindWindowOnSceneOrCreate(form);
			}// else
			//if (form.CreationMethod == EFormCreationMethod.Dynamic)
			//{
			//
			//}
		}
        WindowsAvailableAtStart.Clear();
    }

    public void CreateWindow(FormData formData)
    {
        UIConsts.FORM_ID formID = formData.Id;

        // проверка на наличие дублей форм
        if (_uiList.ContainsKey(formID))
        {
            Debug.LogError("FORM TYPE '" + formID.ToString() + "' ALREADY EXISTS");
            return;
        }

        Object loadetPrefab = Resources.Load(formData.Path);
        if (loadetPrefab == null)
        {
            Debug.LogError("PREFAB '" + formData.Path + "' NOT EXIST!");
            return;
        }
        GameObject obj = (GameObject)Instantiate(loadetPrefab);
        obj.name = formID.ToString();
        obj.transform.SetParent(_newCanvas.transform, false);
        obj.GetComponent<BaseUIController>().CreationMethod = formData.CreationMethod;
        _uiList.Add(formID, obj);
        RectTransform rectTransf = obj.GetComponent<RectTransform>();
        // start position
        rectTransf.pivot = new Vector2(0.5f, 0.5f);
        rectTransf.anchorMin = new Vector2(0f, 0f);
        rectTransf.anchorMax = new Vector2(1f, 1f);
        rectTransf.sizeDelta = new Vector2(0f, 0f);
        rectTransf.anchoredPosition3D = UIConsts.START_POSITION;
        // scale
        //obj.transform.localScale = new Vector3(formData.scale, formData.scale, 1);

        //BoardUIResponder resp = obj.GetComponent<BoardUIResponder>();
        //if (resp != null)
        //{
        //    resp.AfterUIInitialized();
        //}
    }

    public bool WindowExists(UIConsts.FORM_ID formID) {
        return _uiList.ContainsKey(formID);
    }

    public void RemoveWindowsFromList(UIConsts.FORM_ID formID) {
        if (WindowExists(formID)) {
            _uiList.Remove(formID);
        }
    }
		
	public void FindWindowOnSceneOrCreate(FormData formData)
	{
		UIConsts.FORM_ID formID = formData.Id;
        GameObject newForm = null;
        string formName = formID.ToString();
        foreach (var w in WindowsAvailableAtStart)
        {
            if (w.name == formName)
            {
                newForm = w;
                break;
            }
        }
        if (newForm != null)
        {
            newForm.GetComponent<BaseUIController>().CreationMethod = formData.CreationMethod;
            _uiList.Add(formID, newForm);
        }
        else
        {
            CreateWindow(formData);
        }
	}

	// Возвращает ссылку на экземпляр формы типа GameObject
	public GameObject GetFormFromID(UIConsts.FORM_ID form)
	{
		if(_uiList.ContainsKey(form))
		{
			GameObject f = _uiList[form];
			return f;
		}
		else
		{
			Debug.LogError("FORM NAME '" + form.ToString() + "' NOT EXIST!");
			return null;
		}
	}

	public void ShowClicksCatcher(int faderType)
    {
		if (faderType == 0)
		{
			return;
		}
        ClicksCatcher.SetActive(true);
        ClicksCatcher.transform.SetAsLastSibling();
        Image fader = ClicksCatcher.GetComponent<Image>();
        LeanTween.cancel(ClicksCatcher);
		Color newColor = UIConsts.CLICKS_CATCHER_COLOR;
		if (faderType == 1)
		{
			newColor.a = 0;
		}
		LeanTween.value(ClicksCatcher, fader.color, newColor, UIConsts.CLICKS_CATCHER_TIME)
            .setOnUpdate((Color val) =>
            {
                fader.color = val;
            });
    }

    public void HideClicksCatcher()
    {
		LeanTween.cancel(ClicksCatcher);
		Image fader = ClicksCatcher.GetComponent<Image>();
		if (!ClicksCatcher.activeSelf)
		{
			Color ncolor = UIConsts.CLICKS_CATCHER_COLOR;
			ncolor.a = 0;
			fader.color = ncolor;
			return;
		}      
        
		if (fader.color.a == 0)
		{
			ClicksCatcher.SetActive(false);
			return;
		}

        Color acolor = UIConsts.CLICKS_CATCHER_COLOR;
        acolor.a = 0;
        LeanTween.value(ClicksCatcher, fader.color, acolor, UIConsts.CLICKS_CATCHER_TIME)
            .setOnUpdate((Color val) =>
            {
                fader.color = val;
            })
            .setOnComplete(() =>
            {
                ClicksCatcher.SetActive(false);
            });
    }

}
