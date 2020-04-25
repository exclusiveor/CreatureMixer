using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour 
{
	public Text AText;

	public void SetText()
	{
		AText.text = Localer.GetText("Level") + " " + (GameManager.Instance.Player.CurrentLevel + 1).ToString();
	}
}