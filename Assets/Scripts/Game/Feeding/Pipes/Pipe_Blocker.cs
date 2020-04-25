using UnityEngine;

public class Pipe_Blocker : SPipe
{
    public override void InitPipe(int parameter, int color,  bool onstart = false)
    {
		_destroyed = false;
		AColor = color;
        Param = parameter;
        _movable = true;
    }

	public override void PlayAddAnimation()
	{
		// animation when pipe added to board
		//TODO
		//LeanTween.cancel(obj);
		transform.localScale = new Vector3(0, 0, 1);
		LeanTween.scale(gameObject, new Vector3(1.0f, 1.0f, 1), 0.25f);
	}

	public override void RemoveConsumAnimation()
	{
		// animation of blocker destroyed by booster
		gameObject.SetActive(false);
	}
}