using UnityEngine;
using System.Collections;

public class ButtonShaker: MonoBehaviour 
{
	public float        ShakePowerX = 10.0f;
    public float        ShakePowerY = 10.0f;
    public float        ShakeTime = 0.5f;
	public float        ShakeInterval = 3.0f;
    private float       _shakeDx;
    private float       _shakeDy;
    private bool        _isShake;
    private Vector3     _startShakePosition;
    public float        ScaleMin = 0.95f;
	public float        ScaleMax = 1.0f;
	public float        ScaleTime = 0.25f;
    public GameObject   ObjectToScale;
    public GameObject   ObjectToShake;

	void OnEnable()
	{
        LeanTween.cancel(ObjectToShake);
        LeanTween.cancel(ObjectToScale);
        _shakeDx = 0;
        _shakeDy = 0;
        if (ShakePowerX > 0 && ShakePowerY > 0 && ShakeTime > 0 && ShakeInterval > 0 && ObjectToShake != null)
        {
            _isShake = true;
            ShakeDelayed();
        }
        else
        {
            _isShake = false;
        }
        if (ScaleTime > 0 && ScaleMin > 0 && ScaleMax > 0 && ObjectToScale != null)
        {
            ScaleButton();
        }
	}

    void Start()
    {
        if (ObjectToShake)
        {
            _startShakePosition = ObjectToShake.transform.localPosition;
        }
    }

    void OnDesable()
    {
        LeanTween.cancel(ObjectToShake);
        LeanTween.cancel(ObjectToScale);
        if (ObjectToShake)
        {
            ObjectToShake.transform.localPosition = _startShakePosition;
        }
    }

    private void ShakeDelayed()
    {
        // shake it
        float doublePowerX = ShakePowerX * 2;
        float doublePowerY = ShakePowerY * 2;
        LeanTween.value(ObjectToShake, 0.0f, 1.0f, ShakeTime)
            //.setEase(UIConsts.SHOW_EASE)
            .setDelay(ShakeInterval)
            .setOnUpdate
                (
                    (float val) =>
                    {
                        if (ShakePowerX > 0)
                        {
                            _shakeDx = UnityEngine.Random.Range(0, doublePowerX) - ShakePowerX;
                        }
                        if (ShakePowerY > 0)
                        {
                            _shakeDy = UnityEngine.Random.Range(0, doublePowerY) - ShakePowerY;
                        }
                    }
                )
            .setOnComplete
                (() =>
                {
                    _shakeDx = 0;
                    _shakeDy = 0;
                    ObjectToShake.transform.localPosition = _startShakePosition;
                    ShakeDelayed();
                }
            );
    }

    private void ScaleButton()
    {
        transform.localScale = new Vector3(ScaleMin, ScaleMin, 1);
        LeanTween.scale(ObjectToScale, new Vector3(ScaleMax, ScaleMax, 1), ScaleTime)
            .setLoopType(LeanTweenType.pingPong)
            .setLoopCount(-1)
            .setEase(LeanTweenType.easeInOutSine);
    }

    void Update()
    {
        if (_isShake)
        {
            Vector3 realPos = _startShakePosition;
            realPos.x -= _shakeDx;
            realPos.y -= _shakeDy;
            ObjectToShake.transform.localPosition = realPos;
        }
    }
}