using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ESequenceType
{
    None = 0,
    SameColor = 100,
    SameParam = 200,
    Straight = 400,
    SuperStraight = 600,
    Identical = 1000
}

public class SequencePanel : MonoBehaviour 
{
	const float                     MOVE_SPEED = 0.15f;
    public const int                SIZE = 4;
    const float                     PIPES_SCALE = 0.9f;
    const int                       PIPES_DX = 220;

    public GameObject               PipePrefab;
    public GameObject               SequenceCompletePopupPrefab;

    public Transform                Container;

    private List<SequencePipe>      _pipesPool = new List<SequencePipe>();
    private List<SequencePipe>      _sequence = new List<SequencePipe>();
    private List<Vector3>           _slotsPoses = new List<Vector3>();

    private int                     _lastSlot = -1;

    void Awake()
	{
		EventManager.OnCombineWasMadeEvent += OnCombineWasMade;
        EventManager.OnStartPlayPressedEvent += CallOnStartPlayPressed;
        _lastSlot = SIZE + 1;
        float allWidth = (_lastSlot + 1) * PIPES_DX;
        float firstPos = -allWidth / 2.0f + PIPES_DX / 2.0f;
        for (int i = 0; i <= _lastSlot; ++i)
        {
            Vector3 pos = new Vector3(firstPos + i * PIPES_DX, 0, 0);
            _slotsPoses.Add(pos);
            _sequence.Add(null);
        }
    }
	
    void Start()
    {
        CallOnStartPlayPressed(null);
    }

	void OnDestroy()
	{
		EventManager.OnCombineWasMadeEvent -= OnCombineWasMade;
        EventManager.OnStartPlayPressedEvent -= CallOnStartPlayPressed;
    }

    private SequencePipe GetPipeFromPool()
    {
        SequencePipe res = null;
        // try take from pool
        for (int i = 0; i < _pipesPool.Count; ++i)
        {
            GameObject obj = _pipesPool[i].AGameObject;
            if (!obj.activeSelf)
            {
                res = _pipesPool[i];
                LeanTween.cancel(obj);
                res.ATransform.SetParent(Container, false);
                obj.SetActive(true);
                return res;
            }
        }
        // create new
        GameObject pipeObj = (GameObject)GameObject.Instantiate(PipePrefab, Vector3.zero, Quaternion.identity);
        res = pipeObj.GetComponent<SequencePipe>();
        res.ATransform.SetParent(Container, false);
        return res;
    }

    void OnCombineWasMade(EventData e)
	{
        // add new pipe to last slot
        int acolor = (int)e.Data["acolor"];
        int param = (int)e.Data["param"];
        //bool isdouble = (bool)e.Data["double"];
        SequencePipe pipe = GetPipeFromPool();
        pipe.InitPipe(EPipeType.Colored, acolor, param);
        Transform pipeTransf = pipe.ATransform;
        pipeTransf.localScale = new Vector3(PIPES_SCALE, PIPES_SCALE, 1);
        pipeTransf.localPosition = _slotsPoses[_lastSlot];
        // move prev pipes to the left
        for (int i = 1; i <= SIZE; ++i)
        {
            if (_sequence[i] == null) { continue; }
            int newi = i - 1;
            SequencePipe p = _sequence[i];
            GameObject pobj = p.AGameObject;
            LeanTween.cancel(p.gameObject);
            if (newi == 0)
            {
                // move to the left and remove
                LeanTween.moveLocalX(pobj, _slotsPoses[newi].x, MOVE_SPEED)
                    .setOnComplete
                    (
                        ()=>
                        {
                            pobj.SetActive(false);
                        }
                    );
            }
            else
            {
                // just move to the left
                _sequence[newi] = p;
                LeanTween.moveLocalX(pobj, _slotsPoses[newi].x, MOVE_SPEED);
            }
        }
        // move new pipe
        _sequence[SIZE] = pipe;
        LeanTween.moveLocalX(pipe.AGameObject, _slotsPoses[SIZE].x, MOVE_SPEED)
            .setOnComplete
            (
                ()=>
                {
                    CheckIfSomeSequenceCompleted();
                }
            );
        //
    }

    private void CheckIfSomeSequenceCompleted()
    {
        // check if all SIZE pipes
        for (int i = 1; i <= SIZE; ++i)
        {
            if (_sequence[i] == null)
            {
                return;
            }
        }
        //
        List<SequencePipe> orderedPipes = new List<SequencePipe>();
        for (int i = 1; i <= SIZE; ++i)
        {
            orderedPipes.Add(_sequence[i]);
        }
        for (int i = 0; i < orderedPipes.Count - 1; ++i)
        {
            for (int j = i + 1; j < orderedPipes.Count; ++j)
            {
                if (orderedPipes[j].Param < orderedPipes[j - 1].Param)
                {
                    SequencePipe temp = orderedPipes[j];
                    orderedPipes[j] = orderedPipes[j - 1];
                    orderedPipes[j - 1] = temp;
                }
            }
        }

        //
        bool isSameColor = true;
        for (int i = 1; i < orderedPipes.Count; ++i)
        {
            if (orderedPipes[i].AColor != orderedPipes[0].AColor)
            {
                isSameColor = false;
                break;
            }
        }
        //
        bool isSameParam = true;
        for (int i = 1; i < orderedPipes.Count; ++i)
        {
            if (orderedPipes[i].Param != orderedPipes[0].Param)
            {
                isSameParam = false;
                break;
            }
        }
        //
        if (isSameColor)
        {
            if (isSameParam)
            {
                StartCoroutine(OnSequenceCompleted(orderedPipes, ESequenceType.Identical));
                return;
            }
        } else
        if (isSameParam)
        {
            StartCoroutine(OnSequenceCompleted(orderedPipes, ESequenceType.SameParam));
            return;
        }
        //
        bool isStraight = true;
        for (int i = 1; i < orderedPipes.Count; ++i)
        {
            if (orderedPipes[i].Param != (orderedPipes[i - 1].Param + 1))
            {
                isStraight = false;
                break;
            }
        }
        // 
        if (isStraight)
        {
            if (isSameColor)
            {
                StartCoroutine(OnSequenceCompleted(orderedPipes, ESequenceType.SuperStraight));
                return;
            } else
            {
                StartCoroutine(OnSequenceCompleted(orderedPipes, ESequenceType.Straight));
                return;
            }
        } else
        if (isSameColor)
        {
            StartCoroutine(OnSequenceCompleted(orderedPipes, ESequenceType.SameColor));
            return;
        }
    }

    private IEnumerator OnSequenceCompleted(List<SequencePipe> orderedPipes, ESequenceType sResult)
    {
        // count points, remove pipes from sequence
        long points = 0;
        int multiplyer = (int)sResult;
        for (int i = 1; i <= SIZE; ++i)
        {
            points += _sequence[i].Param * multiplyer;
            _sequence[i] = null;
        }
        // fly pipes to center and explode
        for (int i = 0; i < orderedPipes.Count; ++i)
        {
            orderedPipes[i].OnSequenceAnimation(SIZE - 1 - i);
        }
        yield return new WaitForSeconds(Consts.PIPES_ON_SEQUENCE_ANIMATION_TIME);
        // shake screen
        GameManager.Instance.BoardData.AGameBoard.ShakeCamera(Consts.SHAKE_POWER_ON_SEQUENCE, Consts.SHAKE_POWER_ON_SEQUENCE, Consts.SHAKE_TIME_ON_SEQUENCE);
        // show popup with points
        GameObject popupObj = (GameObject)GameObject.Instantiate(SequenceCompletePopupPrefab, Vector3.zero, Quaternion.identity);
        popupObj.transform.SetParent(transform, false);
        popupObj.transform.localPosition = new Vector3(0, 52, 0);
        popupObj.GetComponent<Canvas>().overrideSorting = true;
        Transform textsContainer = popupObj.transform.Find("Container");
        Text typeText = textsContainer.Find("Text0").GetComponent<Text>();
        typeText.text = Localer.GetText(sResult.ToString());
        Text pointsText = textsContainer.Find("Text1").GetComponent<Text>();
        pointsText.text = Localer.GetText(points.ToString());
        GameObject.Destroy(popupObj, 5.0f);
        // add points
        GameManager.Instance.BoardData.AddPointsForSequence(points);
        //
        yield return null;
    }

    void CallOnStartPlayPressed(EventData e)
    {
        for (int i = 0; i < _sequence.Count; ++i)
        {
            if (_sequence[i] != null)
            {
                _sequence[i].gameObject.SetActive(false);
                _sequence[i] = null;
            }
        }
    }

    public void UpdateSkins()
    {
        for (int i = 0; i < _sequence.Count; ++i)
        {
            if (_sequence[i] != null)
            {
                _sequence[i].UpdateSkin();
            }
        }
    }

}