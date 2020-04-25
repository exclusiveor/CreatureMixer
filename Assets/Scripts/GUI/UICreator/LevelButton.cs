using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelButton : MonoBehaviour {
	public Button 		    AButton;
	public Text 		    AText;
	public int 			    CascadeLevel = 1;
	public CanvasGroup 	    AGroup;
	public int 			    Level;
    public List<GameObject> Stars;
    public GameObject       StarsBack;
	public GameObject		Lock;
//    public Text             BestMovesText;

    // Use this for initialization
	void Awake ()
	{
		AButton.onClick.AddListener(() => OnClick());
		// navigation
		Navigation navi = AButton.navigation;
		navi.mode = Navigation.Mode.None;
		AButton.navigation = navi;
		//
	}

	void OnClick()
	{
		MusicManager.playSound("button_click");
		Transform buttonParent = transform.parent;
		if (CascadeLevel > 1)
		{
			for (int i = 1; i < CascadeLevel; ++i)
			{
				buttonParent = buttonParent.parent;
			}
		}
		buttonParent.SendMessage("LevelButtonOnClick", this, SendMessageOptions.RequireReceiver);
	}

	public void InitLevelButton(int number, LevelState levelState)
	{
        for (int i = 0; i < Stars.Count; ++i)
        {
            Stars[i].SetActive(false);
        }

        Level = number;
		AText.text = (Level + 1).ToString();

		if (levelState.Unlocked)
		{
            AText.gameObject.SetActive(true);
            Lock.SetActive(false);
			AGroup.interactable = true;
            //AGroup.alpha = 1;
            StarsBack.SetActive(true);
            if (levelState.BestMoves > 0)
			{
			    //BestMovesText.text = levelState.BestMoves.ToString();
			    //BestMovesText.gameObject.SetActive(true);
				int starsCount = levelState.Stars;
				for (int i = 0; i < starsCount; ++i)
				{
					Stars[i].SetActive(true);
				}
			}
			else
			{
			    //BestMovesText.gameObject.SetActive(false);
            }
        } else
		{
            
            for (int i = 0; i < Stars.Count; ++i)
            {
                Stars[i].SetActive(false);
            }
			Lock.SetActive(true);
            AText.gameObject.SetActive(false);
            AGroup.interactable = false;
			StarsBack.SetActive(false);
			//AGroup.alpha = 0.5f;
		}
	}
}