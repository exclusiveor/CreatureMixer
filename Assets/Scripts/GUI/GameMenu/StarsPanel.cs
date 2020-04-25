using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class StarsPanel : MonoBehaviour 
{
    //public Image Filler;
    public RectTransform FillerTransform;
    public float FillerWidth = 912;

    private List<bool> StarsObtained;
    public List<Transform> Stars;

    const float CHANGE_SPEED = 0.5f;
	const float MAX_TIME = 1.0f;

	public GameObject AGameObject;
	private float AmountCurrent;
	private float Amount;
	private GameObject[] _effectPrefabs;
	private ZActionWorker _worker;
    public GameObject StarEffectPrefab;

    void Awake()
	{
		AmountCurrent = 0;
		Amount = 0;

        EventManager.OnReachMaxPipeLevelEvent += OnReachMaxPipeLevel;

		_effectPrefabs = new GameObject[3];
		_effectPrefabs[0] = Resources.Load("Prefabs/Effects/ResourcesEffect1", typeof(GameObject)) as GameObject;
		_effectPrefabs[1] = Resources.Load("Prefabs/Effects/ResourcesEffect2", typeof(GameObject)) as GameObject;
		_effectPrefabs[2] = Resources.Load("Prefabs/Effects/ResourcesEffect3", typeof(GameObject)) as GameObject;
		_worker = new ZActionWorker();
    }

	void OnDestroy()
	{
		EventManager.OnReachMaxPipeLevelEvent -= OnReachMaxPipeLevel;
	}

	void OnReachMaxPipeLevel(EventData e)
	{
        if (GameBoard.GameType != EGameType.Leveled)
        {
            return;
        }
		float amount = GameManager.Instance.BoardData.StarsGained;
        bool isforce = false;
        if (e.Data.ContainsKey("isforce"))
        {
            isforce = (bool)e.Data["isforce"];
        }
        if (isforce)
        {
            LeanTween.cancel(AGameObject);
            SetAmountForce(amount);
        }
        else
        {
            SetAmount(amount);
        }
        if (Consts.SHOW_ADD_POINTS_ANIMATION)
        {
            Vector3 startPos = new Vector3((float)e.Data["x"], (float)e.Data["y"], 0);
            startPos = transform.parent.transform.InverseTransformPoint(startPos);
            Vector3 endPos = transform.parent.transform.InverseTransformPoint(transform.position); // + new Vector3(-150.0f, RESOURCES_EFFECT_OFFSET - ((colorType - 1) * RESOURCES_EFFECT_OFFSET), 0.0f);
            GameObject effect = GameObject.Instantiate(_effectPrefabs[UnityEngine.Random.Range(0, _effectPrefabs.Length)], Vector3.zero, Quaternion.identity) as GameObject;
            effect.transform.SetParent(transform.parent.transform, false);
            effect.transform.localPosition = startPos;
            List<Vector3> path = GameManager.Instance.GameData.XMLSplineData[String.Format("chip_get_{0}", UnityEngine.Random.Range(1, 4))];

            MoveSplineAction splineMover = new MoveSplineAction(effect, path, startPos, endPos, Consts.ADD_POINTS_EFFECT_TIME);
            _worker.AddParalelAction(splineMover);

            GameObject.Destroy(effect, Consts.ADD_POINTS_EFFECT_TIME + 0.1f);

            LeanTween.delayedCall(AGameObject, Consts.ADD_POINTS_EFFECT_TIME, () =>
            {
                SetAmountForce(amount);
            });
        }
	}

    public void ResetScores()
    {
        if (StarsObtained == null)
        {
            StarsObtained = new List<bool>();
        }
        StarsObtained.Clear();
        for (int i = 0; i < 3; ++i)
        {
            StarsObtained.Add(false);
        }
        //for (int i = 0; i < 3; ++i)
        //{
        //    Glows[i].SetActive(false);
        //}
        Amount = 0;
        AmountCurrent = 0;
        SetAmountForce(Amount);
    }

    void UpdateStar(int number, float points)
    {
        if (StarsObtained[number])
        {
            return;
        }
        float pointsForLocalStar = 1;
        if (number > 0)
        {
            points -= number;
        }
        if (points < 0)
        {
            points = 0;
        }
        float norm = (float)points / pointsForLocalStar;
        if (norm > 1) { norm = 1; }
        if (norm == 1)
        {
            //Just obtain star, play sound, particles, ...
            MusicManager.playSound("star_completed");
            StarsObtained[number] = true;
            // glow
            //Glows[number].SetActive(true);
            // particles
            CreateStarEffect(number);
        }
    }

    private void CreateStarEffect(int number)
    {
        GameObject effect = GameObject.Instantiate(StarEffectPrefab, Vector3.zero, Quaternion.identity, Stars[number]) as GameObject;
        effect.transform.localPosition = Vector3.zero;
        GameObject.Destroy(effect, 5.0f);
    }

    public void SetAmountForce(float amount)
	{
        Amount = amount;
        AmountCurrent = amount;
        float norm = amount / 3.0f;
        //Filler.fillAmount = norm
        FillerTransform.sizeDelta = new Vector2(FillerWidth * norm, FillerTransform.sizeDelta.y);
        for (int i = 0; i < 3; ++i)
        {
            UpdateStar(i, AmountCurrent);
        }
        //EventData ePoints = new EventData("OnScoreTextUpdatedEvent");
        //ePoints.Data["points"] = points;
        //GameManager.Instance.EventManager.CallOnScoreTextUpdatedEvent(ePoints);
    }

    void SetAmount(float amount)
    {
        LeanTween.cancel(AGameObject);
        Amount = amount;
        //UpdateStarsReal(count);
        float atime = CHANGE_SPEED * Mathf.Abs(Amount - AmountCurrent);
        if (atime > MAX_TIME)
        {
            atime = MAX_TIME;
        }
        LeanTween.value(AGameObject, (float)AmountCurrent, (float)Amount, atime)
            //.setEase(LeanTweenType.easeInOutSine)
            //	.setDelay(UIConsts.SHOW_DELAY_TIME)
            .setOnUpdate
                (
                    (float val) =>
                    {
                        SetAmountForce(val);
                    }
                );
    }

    void Update()
	{
		_worker.UpdateActions(Time.deltaTime);
    }

}