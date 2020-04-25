using UnityEngine;
using UnityEngine.UI;

public class InputWindowUIController : BaseUIController {

	private string _atype;
	public GameObject AButton;
	public InputField AInputField;
	public Text Placeholder;
	public GameObject InputButton;

	public override bool OpenForm(EventData e)
	{
		_atype = (string)e.Data["type"];
		gameObject.SetActive(true);
		AInputField.enabled = false;
		InputButton.SetActive(true);
		ReInit();
		return true;
	}

	// Use this for initialization
	void Start ()
	{
		ClickOutside = false;
		InputField.SubmitEvent se = new InputField.SubmitEvent();
		se.AddListener(InputWindowOnEndEdit);
		AInputField.onEndEdit = se;
		InputField.OnChangeEvent se2 = new InputField.OnChangeEvent();
		se2.AddListener(InputWindowOnValueChanged);
		AInputField.onValueChanged = se2;
		//Reset();
		gameObject.SetActive(false);
	}

	private void InputWindowOnEndEdit (string value)
	{
		
	}

	private void InputWindowOnValueChanged(string value)
	{
		if (value == "")
		{
			AButton.GetComponent<UIButton>().Disable();
		} else
		{
			AButton.GetComponent<UIButton>().Enable();
		}
	}

	private void InputWindowButtonOkOnClick ()
	{
		//Debug.Log(HelperFunctions.GetCurrentMethod() + " " + this.name);
		Hide();
		//if (_atype == "horses")
		//{			
		//	GameManager.Instance.Settings.User.HorseName = AInputField.text;
		//	EventData eventData = new EventData("OnHorseAddedEvent");
		//	//eventData.Data["name"] = GameManager.Instance.Settings.User.HorseName;
		//	GameManager.Instance.EventManager.CallOnHorseAddedEvent(eventData);			
		//	Camera.main.transform.GetComponent<MapScript>().DialogsViewer.ShowQuestDialog("ForgedInTheFire");
		//}
		_atype = "";
	}

	public override void ReInit()
	{
		base.ReInit();
		//if (_atype == "horses")
		//{
		//	Transform fon = transform.Find("InputWindowFon");
		//	AInputField = fon.Find("InputField").GetComponent<InputField>();
		//	fon.Find("CaptionText").GetComponent<Text>().text = Localer.GetText("InputHorseName");
		//	_Placeholder.text = Localer.GetText("InputHorsePlaceholder");
		//	AInputField.text = "";
		//	AInputField.characterLimit = MapConsts.HORSE_NAME_MAX_LENGTH;
		//	_inputButton.GetComponent<Button>().Select();
		//	_button.GetComponent<UIButton>().Disable();
		//}
	}

	void Update()
	{
		if (GameManager.Instance.GameFlow.GetCurrentActiveWindowId() != UIConsts.FORM_ID.INPUT_WINDOW) return;

		if (AInputField.text == "")
		{
			Placeholder.enabled = true;
		} else
		{
			Placeholder.enabled = false;
		}

		//UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(AInputField.gameObject, null);
		//AInputField.Select();

		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (AButton.GetComponent<UIButton>().IsActive())
			{
				AButton.SendMessage("OnClick");
			}
		}
	}

	private void InputButtonOnClick ()
	{
		InputButton.SetActive(false);
		AInputField.enabled = true;
		AInputField.Select();
	}
}
