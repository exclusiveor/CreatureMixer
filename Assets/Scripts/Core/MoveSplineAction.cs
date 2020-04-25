using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveSplineAction : ZAction
{
    GameObject _obj;
    private Vector2[] _koefs;
    private List<Vector3> _arrPoints;
    private float _tFullTime;
    private float _tNodeTime;
    private int _nCurrentIndex;
    private Vector2 _pResult;
    private float _timer;
    
    Vector3 _normX = new Vector3();
    Vector3 _normY = new Vector3();
    Vector3 _startPos = new Vector3();    
    Vector3 _endPos = new Vector3();

    private void CalcResult(float time)
    {
        _pResult.x = 0;
        _pResult.y = 0;
        float t = 1F;
        for (int i = 3; i >= 0; --i)
        {
            _pResult.x += t * _koefs[i].x;
            _pResult.y += t * _koefs[i].y;
            t *= time;
        }
        _pResult.y = _pResult.y / 2.0f;
        _pResult.x = _pResult.x / 2.0f;
    }

    public void recalcNorms()
    {        
        _normX = _endPos - _startPos;        
        _normY = new Vector3(-_normX.y, _normX.x);
    }

    private void CalcRotate(float t)
    {
        //float x = 3 * t * t * _koefs[0].x + 2 * t * _koefs[1].x + _koefs[2].x;
        //float y = 3 * t * t * _koefs[0].y + 2 * t * _koefs[1].y + _koefs[2].y;       
    }

    private int CalcIndex(float fdTime)
    {
        int nIndex;
        if (fdTime > _tFullTime)
        {
            nIndex = (int)(_tFullTime / _tNodeTime);
        }
        else if (fdTime <= 0)
        {
            nIndex = 0;
        }
        else
        {
            nIndex = (int)(fdTime / _tNodeTime);
        }
        return nIndex;
    }

    private void CalcKoefs()
    {
        int size = (int)_arrPoints.Count;
        int nPred = (_nCurrentIndex == 0) ? 0 : _nCurrentIndex - 1;
        int nNext = (_nCurrentIndex < size - 1) ? _nCurrentIndex + 1 : size - 1;
        int nNextNext = (nNext < size - 1) ? nNext + 1 : size - 1;
        _koefs[0].x = -_arrPoints[nPred].x + 3 * _arrPoints[_nCurrentIndex].x - 3 * _arrPoints[nNext].x + _arrPoints[nNextNext].x;
        _koefs[1].x = 2 * _arrPoints[nPred].x - 5 * _arrPoints[_nCurrentIndex].x + 4 * _arrPoints[nNext].x - _arrPoints[nNextNext].x;
        _koefs[2].x = -_arrPoints[nPred].x + _arrPoints[nNext].x;
        _koefs[3].x = 2 * _arrPoints[_nCurrentIndex].x;
         
        _koefs[0].y = -_arrPoints[nPred].y + 3 * _arrPoints[_nCurrentIndex].y - 3 * _arrPoints[nNext].y + _arrPoints[nNextNext].y;
        _koefs[1].y = 2 * _arrPoints[nPred].y - 5 * _arrPoints[_nCurrentIndex].y + 4 * _arrPoints[nNext].y - _arrPoints[nNextNext].y;
        _koefs[2].y = -_arrPoints[nPred].y + _arrPoints[nNext].y;
        _koefs[3].y = 2 * _arrPoints[_nCurrentIndex].y;
    }

    public MoveSplineAction(GameObject obj, List<Vector3> points, Vector3 fPointStart, Vector3 fPointEnd, float time)
    {
        _koefs = new Vector2[4];
        _timer = 0;
        _obj = obj;
        _arrPoints = new List<Vector3>();
        _tFullTime = 0;
        _nCurrentIndex = -1;
        _tNodeTime = 0;

        _startPos = fPointStart;
        _endPos = fPointEnd;
        _normX = fPointEnd - fPointStart;
        _normY = new Vector3(-_normX.y, _normX.x, 0.0f);
        SetTime(time);
        Init(points);
    }
    
    public void Init(List<Vector3> points) 
    {
        _arrPoints.Clear();
        _arrPoints.InsertRange(0, points);
        
        _tNodeTime = _tFullTime / (_arrPoints.Count - 1);
        _nCurrentIndex = -1;
    }

    public float GetTime()
    {
        return _tFullTime;
    }

    public void SetTime(float time)
    {
        _tFullTime = time;
        if (_arrPoints.Count > 0)
        {
            _tNodeTime = _tFullTime / (int)(_arrPoints.Count - 1);
        }
    }

    public int GetCurrentIndex()
    {
        return _nCurrentIndex;
    }

    public float GetRotate()
    {
        return 0.0f;
    }

    public int GetPointsCount()
    {
        return _arrPoints.Count;
    }

    public Vector2 GetLastResult()
    {
        return Vector2.zero;
    }    

    public void UpdateSpline(ref Vector2 obj, float fdTime)
	{
		if (_arrPoints.Count > 0)
		{
			int nIndex = CalcIndex(fdTime);
			if (nIndex != _nCurrentIndex)
			{
				_nCurrentIndex = nIndex;
				CalcKoefs();
			}			
			float time = (fdTime - (nIndex * _tNodeTime)) / (float)_tNodeTime;
			CalcResult(time);
		}
		obj = _pResult;		
	}    

    public override void Start()
    {
        
    }

    public override void End()
    {
        
    }

    public override void Update(float dt)
    {
        if (_obj == null)
        {
            SetComplete();
            return;
        }

        _timer += dt;
        Vector2 tp = new Vector2();
        UpdateSpline(ref tp, _timer);
        recalcNorms();
        _obj.transform.localPosition = _startPos + _normX * tp.x + _normY * tp.y;        
        if (_timer >= _tFullTime)
        {
            SetComplete();
            _timer = _tFullTime;
        }
    }

    public override void Break()
    {
        SetComplete();
    }
}
