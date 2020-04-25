using UnityEngine;
using System.Collections.Generic;

public class LevelSelectionButtons : MonoBehaviour {

    public int BUTTONS_COUNT = 9;

    public List<LevelButton> Buttons;
    private int _currentPage = 0;
	public GameObject ButtonNextPage;
	public GameObject ButtonPreviousPage;

    private int LevelToPage(int alevel)
	{
		return alevel / BUTTONS_COUNT;
	}

    public void ShowCurrentPage()
    {
        ShowPage(LevelToPage(GameManager.Instance.Player.CurrentLevel));
    }

	private void ShowPage(int page)
	{
		_currentPage = page;
		int startLevel = BUTTONS_COUNT * _currentPage;
		for (int i = 0; i < BUTTONS_COUNT; ++i)
		{
			int levelNumber = i + startLevel;
			LevelButton button = Buttons[i];
			if (levelNumber < GameManager.Instance.Player.LevelsStates.Count)
			{
				button.InitLevelButton(levelNumber, GameManager.Instance.Player.LevelsStates[levelNumber]);
			} else
			{
				button.gameObject.SetActive(false);
			}
		}

		UpdateArrowButtons();
	}

	private void UpdateArrowButtons()
	{
		ButtonPreviousPage.SetActive(_currentPage > 0);
		ButtonNextPage.SetActive(_currentPage < Consts.LEVELS_COUNT / BUTTONS_COUNT - 1);
	}

	public void ButtonNextPageOnClick ()
	{
		ShowPage(_currentPage + 1);
	}
		
	public void ButtonPreviousPageOnClick ()
	{
		ShowPage(_currentPage - 1);
	}
		
}
