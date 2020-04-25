using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHintScript : MonoBehaviour
{
    public GameObject AGameObject;
    public SpriteRenderer[] ArrowSprites;
    public Color[] Colors;
    public float Speed;
    //private float _timer;
    //private int _index;
    private Vector3 _startPos;
    private Vector3 _endPos;
    private bool _hiding = false;

    public void ShowHint(GameBoard.MatchHintData mhData, GameBoard gBoard)
    {
        _hiding = false;
        //if (mhData.XA == mhData.XB && mhData.YA == mhData.YB)
        //{
        //    int r = 0;
        //}
        //Debug.Log(mhData.XA + "/" + mhData.YA + " --- " + mhData.XB + "/" + mhData.YB);
        // rotating
        if (mhData.XA != mhData.XB)
        {
            // horizontal slide
            if (mhData.XA < mhData.XB)
            {
                // slide right
                //Helpers.SetRotationDgr(AGameObject, 0);
            }
            else
            {
                // slide left
                Helpers.SetRotationDgr(AGameObject, 180);
            }
        }
        else
        {
            // vertical slide
            if (mhData.YA < mhData.YB)
            {
                // slide up
                Helpers.SetRotationDgr(AGameObject, 90);
            }
            else
            {
                // slide down
                Helpers.SetRotationDgr(AGameObject, -90);
            }
        }

        // positioning
        SSlot slot = gBoard.GetSlot(mhData.XA, mhData.YA);
        if (slot != null)
        {
            AGameObject.transform.SetParent(slot.transform.parent, false);
            Vector3 pos = slot.transform.position;
            pos.z = -5;
            AGameObject.transform.position = pos;
        }

        //SetColorsWithIndex(0);
        //_timer = 0.0f;
        //_index = 0;
        _startPos = ArrowSprites[0].transform.localPosition;
        _endPos = ArrowSprites[ArrowSprites.Length - 1].transform.localPosition;
        _endPos = _endPos + new Vector3(ArrowSprites[ArrowSprites.Length - 1].sprite.rect.width / 100.0f, 0.0f, 0.0f);
    }

    public void HideHint()
    {
        _hiding = true;
        GameObject.Destroy(AGameObject, 0.5f);
    }

    public void HideHintForce()
    {
        GameObject.Destroy(AGameObject);
    }

    // Update is called once per frame
    void Update ()
    {
        //_timer += Time.deltaTime;
        //if (_timer >= Speed)
        //{            
        //    SetColorsWithIndex(_index);
        //    ++_index;
        //    if (_index == ArrowSprites.Length)
        //    {
        //        _index = 0;
        //    }
        //    _timer = 0.0f;
        //}
        if (!_hiding)
        {
            foreach (var arrow in ArrowSprites)
            {
                arrow.transform.localPosition = Vector3.MoveTowards(arrow.transform.localPosition, _endPos, Speed * Time.deltaTime);
                Color color = arrow.color;
                color.a = Vector3.Distance(arrow.transform.localPosition, _endPos) / 4.0f * Vector3.Distance(arrow.transform.localPosition, _startPos);
                arrow.color = color;

                if (arrow.transform.localPosition == _endPos)
                {
                    arrow.transform.localPosition = _startPos;
                }
            }
        }
        else
        {
            foreach (var arrow in ArrowSprites)
            {
                arrow.transform.localPosition = Vector3.MoveTowards(arrow.transform.localPosition, _endPos, Speed * Time.deltaTime);
                Color color = arrow.color;
                if (color.a > 0)
                {
                    color.a = color.a -= Time.deltaTime * 2.0f;
                    if (color.a < 0)
                    {
                        color.a = 0;
                    }
                    arrow.color = color;

                    if (arrow.transform.localPosition == _endPos)
                    {
                        arrow.transform.localPosition = _startPos;
                    }
                }
            }
        }
        
	}

    private void SetColorsWithIndex(int index)
    {
        int curColorIndex = index;
        foreach(var arrow in ArrowSprites)
        {
            arrow.color = Colors[curColorIndex];
            ++curColorIndex;
            if (curColorIndex == Colors.Length)
            {
                curColorIndex = 0;
            }
        }
    }
}
