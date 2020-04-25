using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIDebugText : MonoBehaviour
{
    public int MaxLogs = 6;
	public Text DebugText;
    private List<string> _logs = new List<string>();
    private List<string> _showLogs = new List<string>();
    private int _currentId = -1;
    private bool _pinToLast = true;

    public GameObject Button_Next;
    public GameObject Button_Prev;
    public Toggle PinToLast;
    public Toggle Toggle;

    private void Awake()
    {
        UpdateArrows();
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (_logs.Count > 0 && _logs[_logs.Count - 1] == logString)
        {
            _logs.RemoveAt(_logs.Count - 1);
            _showLogs.RemoveAt(_logs.Count - 1);
        }
        _logs.Add(logString);
        string newLog = logString + " ### " + UnityEngine.Random.Range(0, 9999) + " ### " + stackTrace;
        _showLogs.Add(newLog);

        if (_pinToLast)
        {
            _currentId = _logs.Count - 1;
        }
        else
        {
            if (_showLogs.Count == 1)
            {
                _currentId = 0;
            }
        }
        UpdateText();
    }

    private void UpdateText()
    {
        DebugText.text = "";
        if (_showLogs.Count > 0)
        {
            int minI = Mathf.Max(0, _currentId - MaxLogs);
            for (int i = _currentId; i >= minI; --i)
            {
                DebugText.text += (_showLogs[i] + "\n");
            }
        }
        else
        {
            _currentId = -1;
        }
        transform.SetAsLastSibling();
        UpdateArrows();
    }

    public void ButtonClearOnClicked()
    {
        _logs.Clear();
        _showLogs.Clear();
        _currentId = -1;
        UpdateText();
    }

    public void ButtonNextOnClicked()
    {
        if (_currentId < _logs.Count - 1)
        {
            ++_currentId;
            UpdateText();
            PinToLast.isOn = false;
        }
    }

    public void ButtonPrevOnClicked()
    {
        if (_currentId > MaxLogs)
        {
            --_currentId;
            UpdateText();
            PinToLast.isOn = false;
        }
    }

    public void ToggleOnClick(bool value)
    {
        //DebugText.gameObject.SetActive(!value);
        DebugText.gameObject.SetActive(!Toggle.isOn); 
    }

    public void TogglePinToLastOnClick(bool value)
    {
        //_pinToLast = value;
        _pinToLast = PinToLast.isOn;
        if (_pinToLast)
        {
            _currentId = _logs.Count - 1;
        }
    }

    private void UpdateArrows()
    {
        if (_logs.Count < MaxLogs)
        {
            Button_Next.SetActive(false);
            Button_Prev.SetActive(false);
            return;
        }
        Button_Prev.SetActive(_currentId > MaxLogs);
        Button_Next.SetActive(_currentId < _logs.Count - 1);
    }

    public void ButtonCheat0OnClicked()
    {
        //MatchCheats.ExecuteQWAR = true;
    }

    public void ButtonCheat1OnClicked()
    {
        //MatchCheats.ExecuteWIN = true;

        //////GameManager.Instance.BoardData.AGameBoard.SaveGame();
        ////EventData eventData = new EventData("OnNeedSaveLevelEvent");
        ////GameManager.Instance.EventManager.CallOnNeedSaveLevelEvent(eventData);
        ////GameManager.Instance.Settings.Save(true);
    }

    public void ButtonCheat2OnClicked()
    {
        // add gold
        //GameManager.Instance.Player.AddGoldGlobal(500000);
    }

    public void ShowMessage(string message, bool clearAll)
    {
        if (clearAll)
        {
            ButtonClearOnClicked();
        }
        HandleLog(message, "", LogType.Log);
    }

    private void Update()
    {
        transform.SetAsLastSibling();
    }
}

