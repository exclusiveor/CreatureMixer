using UnityEngine;
using System.Collections.Generic;

public class QueuePanel : MonoBehaviour 
{
	const float                     MOVE_SPEED = 0.15f;
    public const int                SIZE = 3;
    const float                     PIPES_SCALE = 0.7f;
    const int                       PIPES_DX = 140;

    public GameObject               PipePrefab;

    public Transform                Container;

    private List<SequencePipe>      _pipesPool = new List<SequencePipe>();
    private List<SequencePipe>      _sequence = new List<SequencePipe>();
    private List<Vector3>           _slotsPoses = new List<Vector3>();

    private int                     _lastSlot = -1;

    void Awake()
	{
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
        //ResetPanel();
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
        _pipesPool.Add(res);
        return res;
    }

    public int GetNextColor()
    {
        return _sequence[1].AColor;
    }

    public EPipeType GetNextType()
    {
        return _sequence[1].PipeType;
    }

    public int GetNextParam()
    {
        return _sequence[1].Param;
    }

    public void MoveQueue(EPipeType pipeType, int acolor, int param)
	{
        // add new pipe to last slot
        SequencePipe pipe = CreatePipe(pipeType, acolor, param);
        pipe.ATransform.localPosition = _slotsPoses[_lastSlot];
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
        LeanTween.moveLocalX(pipe.AGameObject, _slotsPoses[SIZE].x, MOVE_SPEED);
        //
    }

    void ResetPanel()
    {
        for (int i = 0; i < _sequence.Count; ++i)
        {
            if (_sequence[i] != null)
            {
                _sequence[i].gameObject.SetActive(false);
                _sequence[i] = null;
            }
        }
        
        //create full queue at start
        for (int i = 1; i <= SIZE; ++i)
        {
            SequencePipe pipe = CreatePipe(EPipeType.Colored, GameManager.Instance.BoardData.GetRandomColor(), 0);
            {
                pipe.ATransform.localPosition = _slotsPoses[i];
                _sequence[i] = pipe;
            }
        }
    }

    public void LoadPanel(List<int> state)
    {
        for (int i = 0; i < _sequence.Count; ++i)
        {
            if (_sequence[i] != null)
            {
                _sequence[i].gameObject.SetActive(false);
                _sequence[i] = null;
            }
        }
        if (state.Count == 0)
        {
            for (int i = 0; i <= SIZE; ++i)
            {
                state.Add(GameManager.Instance.BoardData.GetRandomColor());
            }
        }
        //create full queue at start
        for (int i = 1; i <= SIZE; ++i)
        {
            EPipeType ptype = EPipeType.Colored;
            int param = 0;
            int acolor = state[i - 1];
            if (acolor == -1)
            {
                ptype = EPipeType.Blocker;
                acolor = -1;
                param = -1;
            }
            SequencePipe pipe = CreatePipe(ptype, acolor, param);
            pipe.ATransform.localPosition = _slotsPoses[i];
            _sequence[i] = pipe;
        }
    }

    private SequencePipe CreatePipe(EPipeType pipeType, int acolor, int param)
    {
        SequencePipe pipe = GetPipeFromPool();
        pipe.InitPipe(pipeType, acolor, param);
        Transform pipeTransf = pipe.ATransform;
        pipeTransf.localScale = new Vector3(PIPES_SCALE, PIPES_SCALE, 1);
        return pipe;
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

    public List<int> GetStateToSave()
    {
        List<int> res = new List<int>();
        for (int i = 0; i < _sequence.Count; ++i)
        {
            if (_sequence[i])
            {
                if (_sequence[i].PipeType != EPipeType.Colored)
                {
                    res.Add(-1);
                }
                else
                {
                    res.Add(_sequence[i].AColor);
                }
            }
        }
        return res;
    }

}