using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipRotationAnimator : MonoBehaviour
{
    public GameObject OtherChip;
    public ParticleSystem DropEffect;
    private Vector3 _startPos;

	// Use this for initialization
	void Start () {
        _startPos = transform.position;
        //Time.timeScale = 0.1f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Animate());
        }
	}

    private IEnumerator Animate()
    {
        LeanTween.cancelAll(false);
        transform.position = _startPos;
        transform.rotation = Quaternion.identity;
        LeanTween.moveX(gameObject, 1.0f, 0.5f)
                 .setOnComplete(() =>
                {
                    LeanTween.moveX(OtherChip, -0.05f, 0.1f)
                                .setEase(LeanTweenType.easeOutSine)
                                .setOnComplete(() =>
                                 {
                                     LeanTween.moveX(OtherChip, 0.0f, 0.1f);
                                 });
                    LeanTween.moveX(gameObject, 0.0f, 0.4f)
                             .setEase(LeanTweenType.easeOutSine);
                    LeanTween.rotateY(gameObject, -180.0f, 0.4f)
                             .setEase(LeanTweenType.easeOutSine);
                    LeanTween.delayedCall(0.30f, () =>
                                    {
                                        DropEffect.Play();
                                    });
                });
        yield return new WaitForSeconds(1.0f);
        //transform.position = _startPos;
    }
}
