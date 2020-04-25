using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ResourcesPanel : MonoBehaviour 
{
	const float CHANGE_SPEED = 0.025f;
	const float MAX_TIME = 3.0f;
    const float RESOURCES_EFFECT_OFFSET = 100.0f;
    public GameObject StarFlyPrefab;
    public GameObject PointsTrailPrefabs;
    public Text AmountText;
	public Text BestText;
	public GameObject AGameObject;
	private long AmountCurrent;
	private long Amount;
    //private ZActionWorker _worker;


    void Awake()
	{
		AmountCurrent = 0;
		Amount = 0;
		AmountText.text = "0";
		BestText.text = Localer.GetText("BestScore") + GameManager.Instance.Player.BestScore.ToString();
		EventManager.OnResourcesChangedEvent += OnResourcesChanged;
        EventManager.OnShowAddResourceEffect += OnShowAddResourceEffect;
        //_worker = new ZActionWorker();
    }
	
	void OnDestroy()
	{
		EventManager.OnResourcesChangedEvent -= OnResourcesChanged;
        EventManager.OnShowAddResourceEffect -= OnShowAddResourceEffect;
    }

	void OnResourcesChanged(EventData e)
	{		
        if (e.Data.ContainsKey("newbest"))
        {
            BestText.text = Localer.GetText("BestScore") + GameManager.Instance.Player.BestScore.ToString();
            return;
        }
		long amount = GameManager.Instance.BoardData.GetTotalPoints();
		bool isforce = (bool)e.Data["isforce"];
		if (isforce)
		{
			SetAmountForce(amount);
		} else
		{
			SetAmount(amount);
		}
		//
		//if (GameManager.Instance.Player.BestScore < amount)
		//{
		//	GameManager.Instance.Player.BestScore = amount;
		//	BestText.text = Localer.GetText("BestScore") + amount.ToString();
		//}

	}
	
	void SetAmountForce(long amount)
	{
		LeanTween.cancel(AGameObject);
		Amount = amount;
		AmountCurrent = amount;
		AmountText.text = amount.ToString();
	}

	void SetAmount(long amount)
	{
		LeanTween.cancel(AGameObject);
		Amount = amount;

		float time = CHANGE_SPEED * Mathf.Abs(Amount - AmountCurrent);
		if (time > MAX_TIME)
		{
			time = MAX_TIME;
		}
		LeanTween.value(AGameObject, (float)AmountCurrent, (float)Amount, time)
			//.setEase(LeanTweenType.easeInOutSine)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setOnUpdate
				(
					(float val)=>
					{
						int ival = (int)val;
						if (ival != AmountCurrent)
						{
							AmountCurrent = ival;
							AmountText.text = ival.ToString();
						}
					}
				);
	}

    void OnShowAddResourceEffect(EventData e)
    {
        if (GameBoard.GameType != EGameType.Classic)
        {
            return;
        }
        //if (Consts.SHOW_ADD_POINTS_ANIMATION)
        //{
        //    Vector3 startPos2 = new Vector3((float)e.Data["x"], (float)e.Data["y"], 0);
        //    Vector3 endPos2 = transform.parent.transform.InverseTransformPoint(transform.position); // + new Vector3(-150.0f, RESOURCES_EFFECT_OFFSET - ((colorType - 1) * RESOURCES_EFFECT_OFFSET), 0.0f);
        //    GameObject effect2 = GameObject.Instantiate(_effectPrefabs[UnityEngine.Random.Range(0, _effectPrefabs.Length)], Vector3.zero, Quaternion.identity) as GameObject;
        //    effect2.transform.SetParent(transform.parent.transform, false);
        //    effect2.transform.localPosition = startPos2;
        //    List<Vector3> path = GameManager.Instance.GameData.XMLSplineData[String.Format("chip_get_{0}", UnityEngine.Random.Range(1, 4))];

        //    MoveSplineAction splineMover = new MoveSplineAction(effect2, path, startPos2, endPos2, Consts.ADD_POINTS_EFFECT_TIME);
        //    _worker.AddParalelAction(splineMover);

        //    GameObject.Destroy(effect2, Consts.ADD_POINTS_EFFECT_TIME + 0.1f);
        //}
        long amount = (long)e.Data["amount"];
        Vector3 startPos = new Vector3((float)e.Data["x"], (float)e.Data["y"], 0);
        //if (Consts.SHOW_ADD_POINTS_ANIMATION)
        //{
        //    Vector3 endPos = AmountText.transform.parent.parent.transform.InverseTransformPoint(AmountText.transform.parent.position);
        //    GameObject trailEffect = GameObject.Instantiate(PointsTrailPrefabs, Vector3.zero, Quaternion.identity) as GameObject;
        //    trailEffect.transform.SetParent(transform.parent.transform, false);
        //    trailEffect.transform.localPosition = startPos;
        //    LeanTween.moveLocal(trailEffect, endPos, Consts.ADD_POINTS_EFFECT_TIME)
        //        .setOnComplete(() =>
        //        {
        //            Destroy(trailEffect, 2.0f);
        //        });
        //}

        // flying star
        Vector3 endPos = AmountText.transform.parent.parent.transform.InverseTransformPoint(AmountText.transform.parent.position);
        StartCoroutine(FlyingStarsCoroutine(amount, startPos, endPos));
        //

        //////GameObject effect = GameObject.Instantiate(StarFlyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        //////Text atext = effect.transform.Find("Text").GetComponent<Text>();
        //////atext.text = amount.ToString();
        //////effect.transform.SetParent(transform.parent.transform, false);
        //////effect.transform.localPosition = startPos;

        //////CanvasGroup group = effect.GetComponent<CanvasGroup>();
        //////group.alpha = 0;
        //////LeanTween.value(effect, 0.0f, 1.0f, 0.2f)
        //////    .setOnUpdate((float val) =>
        //////    {
        //////        group.alpha = val;
        //////    })
        //////    .setOnComplete(() =>
        //////    {
        //////        LeanTween.value(effect, 1.0f, 0.0f, 0.5f)
        //////            .setDelay(Consts.POINTS_RASE_TIME - 0.55f)
        //////            .setOnUpdate((float val) =>
        //////            {
        //////                group.alpha = val;
        //////            }).
        //////            setOnComplete(() =>
        //////            {
        //////                GameObject.Destroy(effect);
        //////            });
        //////    });
        //////LeanTween.moveLocalY(effect, effect.transform.localPosition.y + 130, Consts.POINTS_RASE_TIME);
    }

    protected IEnumerator FlyingStarsCoroutine(long amount, Vector3 startPos, Vector3 endPos)
    {
        //// curved incompleted
        //for (long i = 0; i < amount; ++i)
        //{
        //    GameObject trailEffect = GameObject.Instantiate(StarFlyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        //    trailEffect.transform.SetParent(transform.parent.transform, false);
        //    trailEffect.transform.localPosition = startPos;
        //    List<Vector3> path = GameManager.Instance.GameData.XMLSplineData[System.String.Format("chip_get_{0}", UnityEngine.Random.Range(1, 4))];
        //    MoveSplineAction splineMover = new MoveSplineAction(trailEffect, path, startPos, endPos, Consts.ADD_POINTS_EFFECT_TIME);
        //    _worker.AddParalelAction(splineMover);
        //    GameObject.Destroy(trailEffect, Consts.ADD_POINTS_EFFECT_TIME + 0.1f);
        //    yield return new WaitForSeconds(0.1f);
        //}
        //
        for (long i = 0; i < amount; ++i)
        {
            GameObject trailEffect = GameObject.Instantiate(StarFlyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            trailEffect.transform.SetParent(transform.parent.transform, false);
            trailEffect.transform.localPosition = startPos;
            trailEffect.transform.localScale = new Vector3(0, 0, 1);
            LeanTween.scale(trailEffect, Vector3.one, Consts.ADD_POINTS_EFFECT_TIME);
            Vector3 topPos = startPos;
            topPos.y += 150f;
            LeanTween.moveLocal(trailEffect, topPos, Consts.ADD_POINTS_EFFECT_TIME)
                .setOnComplete(() =>
                {
                    LeanTween.scale(trailEffect, new Vector3(0, 0, 1), Consts.ADD_POINTS_EFFECT_TIME / 3.0f)
                    .setDelay(Consts.ADD_POINTS_EFFECT_TIME / 3.0f * 2.0f);
                    LeanTween.moveLocal(trailEffect, endPos, Consts.ADD_POINTS_EFFECT_TIME)
                    .setOnComplete(() =>
                    {
                        Destroy(trailEffect, Consts.ADD_POINTS_EFFECT_TIME + 0.2f);
                    });
                });
            yield return new WaitForSeconds(0.15f);
        }

    }

    //void Update()
    //{
    //    _worker.UpdateActions(Time.deltaTime);
    //}

    //void OnItemCollectedShowAnimaion(EventData e)
    //{
    //    Vector3 startPos = new Vector3((float)e.Data["x"], (float)e.Data["y"], 0);
    //    bool moveUp = (bool)e.Data["moveup"];
    //    int count = (int)e.Data["count"];
    //    startPos = TransformPositionFromScreenToLocalUI(startPos);
    //    GameData.ItemType itemType = (GameData.ItemType)e.Data["item"];
    //    string iconName = GameData.GetStringFromInventoryItem(itemType);

    //    RectTransform iconRectTransform;
    //    GameObject item = new GameObject();
    //    item.name = "FloatingIcon_" + iconName;
    //    item.transform.SetParent(gameObject.transform.parent, false);
    //    iconRectTransform = item.AddComponent<RectTransform>();
    //    iconRectTransform.pivot = new Vector2(0.5f, 0.5f);
    //    iconRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
    //    iconRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    //    iconRectTransform.sizeDelta = new Vector2(0f, 0f);
    //    iconRectTransform.anchoredPosition = startPos;
    //    iconRectTransform.transform.localScale = Vector3.one;
    //    item.transform.SetAsLastSibling();

    //    CanvasGroup itemGroup = item.AddComponent<CanvasGroup>();
    //    itemGroup.alpha = 0.0f;
    //    itemGroup.blocksRaycasts = false;

    //    GameObject sunburst = GameObject.Instantiate(_sunburstPrefab, Vector3.zero, Quaternion.identity) as GameObject;
    //    sunburst.transform.SetParent(item.transform, false);
    //    sunburst.transform.localScale = Vector3.zero;
    //    LeanTween.scale(sunburst, Vector3.one, 1.0f);
    //    LeanTween.value(sunburst, 1.0f, 0.0f, 0.5f)
    //        .setDelay(1.5f)
    //        .setOnUpdate((float val) =>
    //        {
    //            sunburst.GetComponent<CanvasGroup>().alpha = val;
    //        });
    //    LeanTween.scale(sunburst, Vector3.zero, 0.5f)
    //        .setDelay(1.5f);

    //    GameObject itemName = GameObject.Instantiate(_itemNamePrefab, Vector3.zero, Quaternion.identity) as GameObject;
    //    itemName.transform.SetParent(item.transform, false);
    //    string localizesItemName = Localer.GetText(iconName);
    //    if (count > 1)
    //    {
    //        localizesItemName = Localer.GetText(iconName + "_plural");
    //        localizesItemName = String.Format("{0} {1}", count, localizesItemName);
    //    }
    //    itemName.transform.Find("Text").GetComponent<Text>().text = localizesItemName;
    //    LeanTween.value(itemName, 1.0f, 0.0f, 0.5f)
    //        .setDelay(1.5f)
    //        .setOnUpdate((float val) =>
    //        {
    //            itemName.GetComponent<CanvasGroup>().alpha = val;
    //        });

    //    GameObject itemImage = addTreasureImage(Vector3.zero, iconName);
    //    itemImage.transform.SetParent(item.transform, false);
    //    Vector3 finalPos = startPos + new Vector3(0, MIN_ITEM_POSITION_TO_FLY, 0);
    //    LeanTween.value(item, 0.0f, 1.0f, 0.5f)
    //        .setOnUpdate((float val) =>
    //        {
    //            itemGroup.alpha = val;
    //        });
    //    if (moveUp)
    //    {
    //        LeanTween.moveLocalY(item, finalPos.y, COLLECT_ANIMATION_TIME)
    //            .setDestroyOnComplete(true);
    //    }
    //    else
    //    {
    //        LeanTween.moveLocalY(item, item.transform.localPosition.y, COLLECT_ANIMATION_TIME)
    //            .setDestroyOnComplete(true);
    //    }
    //}

}