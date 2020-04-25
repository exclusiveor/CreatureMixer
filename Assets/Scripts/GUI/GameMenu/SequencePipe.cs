using UnityEngine;
using UnityEngine.UI;

public class SequencePipe : MonoBehaviour
{
	public int				Param			{ get; set; }
	public int				AColor			{ get; set; }
    public EPipeType        PipeType        { get; set; }
    public Image            ValueImage;
    public Image            IconImage;
    public Transform        ATransform;
    public GameObject       AGameObject;

    public GameObject       SequencePipeExplodeEffectPrefab;

    public virtual void InitPipe(EPipeType pipeType, int acolor, int param)
    {
        PipeType = pipeType;
        Param = param;
        AColor = acolor;
        UpdateSkin();
    }

    public void OnSequenceAnimation(int order)
    {
        LeanTween.cancel(AGameObject);
        ATransform.SetParent(ATransform.parent.parent, false);
        float shakeDx = 0;
        float shakeDy = 0;
        float doublePower = Consts.PIPES_ON_SEQUENCE_POWER * 2;
        Vector3 startPos = ATransform.localPosition;
        LeanTween.value(AGameObject, 0, 1, Consts.PIPES_ON_SEQUENCE_ANIMATION_TIME)
            //.setEase(UIConsts.SHOW_EASE)
            //	.setDelay(UIConsts.SHOW_DELAY_TIME)
            .setOnUpdate
                (
                    (float val) =>
                    {
                        shakeDx = UnityEngine.Random.Range(0, doublePower) - Consts.PIPES_ON_SEQUENCE_POWER;
                        shakeDy = UnityEngine.Random.Range(0, doublePower) - Consts.PIPES_ON_SEQUENCE_POWER;
                        Vector3 newPos = startPos;
                        newPos.x += shakeDx;
                        newPos.y += shakeDy;
                        ATransform.localPosition = newPos;
                    }
                ).setOnComplete
                (
                () =>
                {
                    //ATransform.localPosition = startPos;
                    // hide pipe
                    Invoke("DisablePipe", 0.1f);
                    // create particle
                    MusicManager.playSound("chip_destroy");
                    GameObject obj = (GameObject)GameObject.Instantiate(SequencePipeExplodeEffectPrefab, Vector3.zero, Quaternion.identity);
                    obj.transform.SetParent(ATransform.parent, false);
                    obj.transform.localPosition = ATransform.localPosition;
                    GameObject.Destroy(obj, 5.0f);
                }
            );


        //LeanTween.moveLocal(AGameObject, new Vector3(xpos, 0, 0), Consts.PIPES_ON_SEQUENCE_ANIMATION_TIME)
        //    .setEase(LeanTweenType.easeInOutSine)
        //    .setOnComplete
        //    (
        //        ()=>
        //        {
        //            // hide pipe
        //            Invoke("Disable", 0.2f);
        //            // create particle
        //            GameObject obj = (GameObject)GameObject.Instantiate(SequencePipeExplodeEffectPrefab, Vector3.zero, Quaternion.identity);
        //            obj.transform.SetParent(ATransform.parent, false);
        //            obj.transform.position = ATransform.localPosition;
        //            GameObject.Destroy(obj, 7.0f);
        //        }
        //    );
    }

    private void DisablePipe()
    {
        AGameObject.SetActive(false);
    }

    public void UpdateSkin()
    {
		SkinData sd = GameManager.Instance.Player.CurrentSkin;
		if (PipeType == EPipeType.Blocker) 
		{
			IconImage.sprite = GameManager.Instance.BoardData.AGameBoard.GetSprite("blocker");
			ValueImage.gameObject.SetActive (false);
		} else
		{
			//if (sd.PipeStructureType == EPipeStructureType.BackFront) 
			//{
				ValueImage.gameObject.SetActive (true);
				IconImage.sprite = GameManager.Instance.BoardData.AGameBoard.GetSprite(sd.BackPrefix + AColor.ToString());
				IconImage.SetNativeSize();
				ValueImage.sprite = GameManager.Instance.BoardData.AGameBoard.GetSprite(sd.FrontPrefix + Param.ToString());
				ValueImage.SetNativeSize();
			//} 
			//else
			//if (sd.PipeStructureType == EPipeStructureType.Solid) 
			//{
			//	ValueImage.gameObject.SetActive (false);
			//	IconImage.sprite = GameManager.Instance.BoardData.AGameBoard.GetSprite(sd.BackPrefix + AColor.ToString() + "_" +  Param.ToString());
			//	IconImage.SetNativeSize();
			//}
		}
    }
}