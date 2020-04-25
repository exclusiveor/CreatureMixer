using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PulsatingText : MonoBehaviour
{
    private const float PULSATE_SCALE_MAX = 1.0f;
	// Use this for initialization
	void Start ()
    {
        PulsateUp();
	}

    private void PulsateUp()
    {

        LeanTween.scale(gameObject, new Vector3(PULSATE_SCALE_MAX, PULSATE_SCALE_MAX, 1.0f), 1.0f)
            .setEase(LeanTweenType.easeInSine)
            .setOnComplete(() =>
            {
                PulsateDown();
            });
    }

    private void PulsateDown()
    {
        LeanTween.scale(gameObject, new Vector3(0.9f, 0.9f, 0.9f), 1.0f)
            .setEase(LeanTweenType.easeOutSine)
            .setOnComplete(() =>
            {
                PulsateUp();
            });
    }

    void OnDestroy()
    {
        LeanTween.cancel(gameObject);
    }
}
