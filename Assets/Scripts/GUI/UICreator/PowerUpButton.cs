using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
//using System.Reflection;

public class PowerUpButton : MonoBehaviour
{
	public UIButton AUIButton;
	//public CanvasGroup AGroup;
	public Text AText;
	public Image SelectionImage;
	public GameData.PowerUpType PowerupType;
    public GameObject CollectEffect;

	public Color SelectedColor;
	public Color NotSelectedColor;
    public GameObject ZeroNumberBack;
    public GameObject NotZeroNumberBack;

    private ZActionWorker _worker;

    void Awake()
    {
        _worker = new ZActionWorker();
    }
		
	public void ResetPowerup()
	{
		UpdatePowerup();
	}

	public void UpdatePowerup() // and unselect
	{
		SelectionImage.color = NotSelectedColor;
		int current = GameManager.Instance.BoardData.PowerUps[PowerupType];
		AText.text = current.ToString();
		if (current <= 0)
		{
            NotZeroNumberBack.SetActive(false);
            ZeroNumberBack.SetActive(true);
            AUIButton.Disable();
		} else
		{
            NotZeroNumberBack.SetActive(true);
            ZeroNumberBack.SetActive(false);
            AUIButton.Enable();
		}
	}

	public void SelectPowerup()
	{
		if (SelectionImage.color == NotSelectedColor)
		{
			SelectionImage.color = SelectedColor;
		} else
		{
			SelectionImage.color = NotSelectedColor;
		}
	}

	public void PlayAddAnimation(float xx, float yy)
	{
        float delay = 0.1f;
        if (Consts.SHOW_ADD_POINTS_ANIMATION)
        {
            Vector3 startPos = transform.parent.transform.InverseTransformPoint(new Vector3(xx, yy, 0));
            Vector3 endPos = transform.parent.transform.InverseTransformPoint(transform.position); // + new Vector3(-150.0f, RESOURCES_EFFECT_OFFSET - ((colorType - 1) * RESOURCES_EFFECT_OFFSET), 0.0f);
            GameObject effect = GameObject.Instantiate(CollectEffect, Vector3.zero, Quaternion.identity) as GameObject;
            effect.transform.SetParent(transform.parent.transform, false);
            effect.transform.localPosition = startPos;
            List<Vector3> path = GameManager.Instance.GameData.XMLSplineData[String.Format("chip_get_{0}", UnityEngine.Random.Range(1, 4))];

            MoveSplineAction splineMover = new MoveSplineAction(effect, path, startPos, endPos, Consts.ADD_POINTS_EFFECT_TIME);
            _worker.AddParalelAction(splineMover);
            delay = Consts.ADD_POINTS_EFFECT_TIME;
            GameObject.Destroy(effect, Consts.ADD_POINTS_EFFECT_TIME + 0.1f);
        }

        GameObject obj = AText.gameObject;
		LeanTween.cancel(obj);
		LeanTween.scale(obj, new Vector3(1.5f, 1.5f, 1), 0.3f)
        .setDelay(delay)
		.setOnComplete(
				()=>{
					LeanTween.scale(obj, Vector3.one, 0.3f);
				}
			);
	}

    void Update()
    {
        _worker.UpdateActions(Time.deltaTime);
    }
}