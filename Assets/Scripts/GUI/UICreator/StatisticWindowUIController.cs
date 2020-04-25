using UnityEngine;
using UnityEngine.UI;

public class StatisticWindowUIController : BaseUIController
{
    public Text         PointsText;
    //public GameObject   NewBest;
    public Camera       ShareCamera;

    protected override void Awake()
    {
        base.Awake();
        ShareCamera.enabled = false;
        Camera.onPostRender += MyPostRender;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Camera.onPostRender -= MyPostRender;
    }

    public override bool OpenForm(EventData e)
	{
        //TryRescale();
        long points = GameManager.Instance.BoardData.GetTotalPoints();
        PointsText.text = points.ToString();
        if (GameManager.Instance.Player.BestScore < points)
        {
            GameManager.Instance.Player.BestScore = points;
            EventData eventData = new EventData("OnResourcesChangedEvent");
            eventData.Data["newbest"] = true;
            GameManager.Instance.EventManager.CallOnResourcesChangedEvent(eventData);
        }
        ShareCamera.enabled = true;
        return true;
	}

    public override bool EscapeOnClick()
    {
        if (_isHiding)
        {
            return true;
        }
        Hide();
        GameManager.Instance.Game.RestartGame();
        GameManager.Instance.Game.GoHome();
        return true;
    }

    public void StatisticWindowButtonHomeOnClick()
    {
        if (_isHiding)
        {
            return;
        }
        Hide();
        GameManager.Instance.Game.RestartGame();
        GameManager.Instance.Game.GoHome();
    }

    public void StatisticWindowButtonRestartOnClick()
    {
        if (_isHiding)
        {
            return;
        }
        Hide();
        GameManager.Instance.BoardData.AGameBoard.RestartGame();
    }

    public void StatisticWindowButtonShareOnClick()
    {
        //TODO
    }

    void Start()
    {
        ReInit();
        gameObject.SetActive(false);
    }

    public void MyPostRender(Camera cam)
    {
        if (cam == ShareCamera)
        {
            TurnOffCamera();
        }
        //Debug.Log("PostRender " + gameObject.name + " from camera " + cam.gameObject.name);
    }

    public void TurnOffCamera()
    {
        ShareCamera.enabled = false;
    }
}
