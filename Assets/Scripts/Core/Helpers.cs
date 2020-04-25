using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

using System.Runtime.CompilerServices;
using System.Diagnostics;

public class Helpers : ScriptableObject
{

    // SETTERS
    public static void SetAlpha(GameObject obj, float a)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr)
        {
            Color acolor = sr.color;
            acolor.a = a;
            sr.color = acolor;
        }
    }

    public static void SetAlpha(SpriteRenderer sr, float a)
    {
        if (sr)
        {
            Color acolor = sr.color;
            acolor.a = a;
            sr.color = acolor;
        }
    }

    public static void SetRotationRad(GameObject obj, float angle)
    {
        obj.transform.rotation = Quaternion.Euler(0, 0, angle * 180 / Mathf.PI);
    }

    public static void SetRotationDgr(GameObject obj, float angle)
    {
        obj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public static void SetColor(GameObject obj, Color color)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr)
        {
            sr.color = color;
        }
    }

    public static void SetXY(Transform trans, float x, float y)
    {
        Vector3 pos = trans.position;
        pos.x = x;
        pos.y = y;
        trans.position = pos;
    }

    public static void SetXY(GameObject obj, float x, float y)
    {
        SetXY(obj.transform, x, y);
    }

    public static void SetXY(GameObject obj, Vector2 pos)
    {
        SetXY(obj.transform, pos.x, pos.y);
    }

    public static void SetXYLocal(Transform trans, float x, float y)
    {
        Vector3 pos = trans.localPosition;
        pos.x = x;
        pos.y = y;
        trans.localPosition = pos;
    }

    public static void SetXYLocal(GameObject obj, float x, float y)
    {
        SetXYLocal(obj.transform, x, y);
    }

    public static void SetXYLocal(GameObject obj, Vector2 pos)
    {
        SetXYLocal(obj.transform, pos.x, pos.y);
    }

    public static void SetZ(Transform trans, float z)
    {
        Vector3 pos = trans.position;
        pos.z = z;
        trans.position = pos;
    }

    public static void SetZ(GameObject obj, float z)
    {
        SetZ(obj.transform, z);
    }

    public static void SetZLocal(Transform trans, float z)
    {
        Vector3 pos = trans.localPosition;
        pos.z = z;
        trans.localPosition = pos;
    }

    public static void SetZLocal(GameObject obj, float z)
    {
        SetZLocal(obj.transform, z);
    }

    public static void SetX(Transform trans, float x)
    {
        Vector3 pos = trans.position;
        pos.x = x;
        trans.position = pos;
    }

    public static void SetX(GameObject obj, float x)
    {
        SetX(obj.transform, x);
    }

    public static void SetXLocal(Transform trans, float x)
    {
        Vector3 pos = trans.localPosition;
        pos.x = x;
        trans.localPosition = pos;
    }

    public static void SetXLocal(GameObject obj, float x)
    {
        SetXLocal(obj.transform, x);
    }

    public static void SetY(Transform trans, float y)
    {
        Vector3 pos = trans.position;
        pos.y = y;
        trans.position = pos;
    }

    public static void SetY(GameObject obj, float y)
    {
        SetY(obj.transform, y);
    }

    public static void SetYLocal(Transform trans, float y)
    {
        Vector3 pos = trans.localPosition;
        pos.y = y;
        trans.localPosition = pos;
    }

    public static void SetYLocal(GameObject obj, float y)
    {
        SetYLocal(obj.transform, y);
    }

    public static void SetScaleXY(GameObject obj, float x, float y)
    {
        SetScaleXY(obj.transform, x, y);
    }

    public static void SetScaleXY(Transform trans, float x, float y)
    {
        Vector3 scale = trans.localScale;
        scale.x = x;
        scale.y = y;
        trans.localScale = scale;
    }

    public static void SetScaleXY(GameObject obj, Vector2 xy)
    {
        SetScaleXY(obj.transform, xy.x, xy.y);
    }

    public static void SetScaleXY(Transform trans, Vector2 xy)
    {
        SetScaleXY(trans, xy.x, xy.y);
    }



    //

    public static float GetAngleBetweenPointsRad(Vector2 a, Vector2 b)
    {
        return Mathf.Atan2(b.y - a.y, b.x - a.x);
    }

    public static float GetAngleBetweenPointsDgs(Vector2 a, Vector2 b)
    {
        float angle = Mathf.Atan2(b.y - a.y, b.x - a.x);
        return (angle * 180 / (float)Mathf.PI);
    }

    public static Vector2 GetPositionOfPoint(Vector3 A, Vector3 B, float distance)
    {
        // NOT TESTED !!!!!
        // позиція точки , що лежить на прямій яка з"єднує точки А і В при відомій дальності 
        float angle = Mathf.Atan2(B.y - A.y, B.x - A.x);
        angle = angle * 180 / (float)Mathf.PI;
        float resХ = A.x + distance * Mathf.Cos(angle);
        float resY = A.y + distance * Mathf.Sin(angle);
        return new Vector2(resХ, resY);
    }

    // 
    public static LTDescr FadeInCanvasGroup(GameObject obj, CanvasGroup cg, float atime, bool autoTime = true) // good variant
	{
		float time = atime;
		if (autoTime)
		{
			time = atime * (1 - cg.alpha);
		}
			
		return LeanTween.value(obj, cg.alpha, 1.0f, time)
			//.setEase(UIConsts.SHOW_EASE)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setOnUpdate
				(
					(float val)=>
					{
					cg.alpha = val;
				}
				)
				.setOnComplete(() => {cg.alpha = 1;});
	}

	public static LTDescr FadeOutCanvasGroup(GameObject obj, CanvasGroup cg, float atime, bool autoTime = false) // good variant
	{
		float time = atime;
		if (autoTime)
		{
			time = atime * cg.alpha;
		}
		
		return LeanTween.value(obj, cg.alpha, 0.0f, time)
			//.setEase(UIConsts.SHOW_EASE)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setOnUpdate
				(
					(float val)=>
					{
					cg.alpha = val;
				}
				)
				.setOnComplete(() => {cg.alpha = 0;});
	}

    public static IEnumerator FadeOutCanvasGroupCoroutine(CanvasGroup cg, float atime)
    {
        float time = atime;
        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime / time;
            yield return null;
        }
        cg.alpha = 0;
        yield return null;
    }

    public static IEnumerator FadeInCanvasGroupCoroutine(CanvasGroup cg, float atime)
    {
        float time = atime;
        while (cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime / time;
            yield return null;
        }
        cg.alpha = 1;
        yield return null;
    }

    public static string ObjectToStr<T>(T _saveMe)
    {
        BinaryFormatter _bin = new BinaryFormatter();
        MemoryStream _mem = new MemoryStream();
        _bin.Serialize(_mem, _saveMe);

        return Convert.ToBase64String(
            _mem.GetBuffer()
            );
    }

    public static T StrToObject<T>(string _data) where T : class
    {
        if (!String.IsNullOrEmpty(_data))
        {
            BinaryFormatter _bin = new BinaryFormatter();
            try
            {
                MemoryStream _mem = new MemoryStream(Convert.FromBase64String(_data));

                T _obj = _bin.Deserialize(_mem) as T;

                return _obj;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        else
        {
            throw new Exception("_data is null or empty");
        }
    }

    public static List<E> ShuffleList<E>(List<E> inputList)
    {
        List<E> randomList = new List<E>();

        System.Random r = new System.Random();
        int randomIndex = 0;
        while (inputList.Count > 0)
        {
            randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
            randomList.Add(inputList[randomIndex]); //add it to the new, random list
            inputList.RemoveAt(randomIndex); //remove to avoid duplicates
        }
        return randomList; //return the new random list
    }

    // INSTANTIATE
    public static new GameObject Instantiate(UnityEngine.Object obj)
    {
        return GameObject.Instantiate(obj, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
    }

    public static GameObject Instantiate(string path)
    {
        return (GameObject)Instantiate(Resources.Load(path)); // "Prefabs/UI/match_3_main_menu/ObstacleUnlockingItemsContainer"
    }

    // CANVAS GROUP VIA LeanTween
    //public static void FadeOutCanvasGroup(GameObject gameObject, CanvasGroup group, float speed)
    //{
    //    // hide popup
    //    LeanTween.cancel(gameObject);
    //    LeanTween.value(gameObject, group.alpha, 0.0f, speed * group.alpha)
    //        //.setEase(UIConsts.SHOW_EASE)
    //        //	.setDelay(UIConsts.SHOW_DELAY_TIME)
    //        .setOnUpdate
    //            (
    //                (float val) =>
    //                {
    //                    group.alpha = val;
    //                }
    //            );
    //}

    //public static void FadeInCanvasGroup(GameObject gameObject, CanvasGroup group, float speed)
    //{
    //    // hide popup
    //    LeanTween.cancel(gameObject);
    //    LeanTween.value(gameObject, group.alpha, 1.0f, speed * (1 - group.alpha))
    //        //.setEase(UIConsts.SHOW_EASE)
    //        //	.setDelay(UIConsts.SHOW_DELAY_TIME)
    //        .setOnUpdate
    //            (
    //                (float val) =>
    //                {
    //                    group.alpha = val;
    //                }
    //            );
    //}

    // LINE SEGMENTS INTERSECTION
    public static bool IsLineSegmentsIntersect(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
    {
        Vector2 dir1 = end1 - start1;
        Vector2 dir2 = end2 - start2;

        //считаем уравнения прямых проходящих через отрезки
        float a1 = -dir1.y;
        float b1 = +dir1.x;
        float d1 = -(a1 * start1.x + b1 * start1.y);

        float a2 = -dir2.y;
        float b2 = +dir2.x;
        float d2 = -(a2 * start2.x + b2 * start2.y);

        //подставляем концы отрезков, для выяснения в каких полуплоскотях они
        float seg1_line2_start = a2 * start1.x + b2 * start1.y + d2;
        float seg1_line2_end = a2 * end1.x + b2 * end1.y + d2;

        float seg2_line1_start = a1 * start2.x + b1 * start2.y + d1;
        float seg2_line1_end = a1 * end2.x + b1 * end2.y + d1;

        //если концы одного отрезка имеют один знак, значит он в одной полуплоскости и пересечения нет.
        if (seg1_line2_start * seg1_line2_end >= 0 || seg2_line1_start * seg2_line1_end >= 0)
            return false;
        //float u = seg1_line2_start / (seg1_line2_start - seg1_line2_end);
        //*out_intersection =  start1 + u*dir1; // точка перетину
        return true;
    }

    //	bool intersection(Point2f start1, Point2f end1, Point2f start2, Point2f end2, Point2f *out_intersection)
    //	{
    //		Point2f dir1 = end1 - start1;
    //		Point2f dir2 = end2 - start2;
    //		
    //		//считаем уравнения прямых проходящих через отрезки
    //		float a1 = -dir1.y;
    //		float b1 = +dir1.x;
    //		float d1 = -(a1*start1.x + b1*start1.y);
    //		
    //		float a2 = -dir2.y;
    //		float b2 = +dir2.x;
    //		float d2 = -(a2*start2.x + b2*start2.y);
    //		
    //		//подставляем концы отрезков, для выяснения в каких полуплоскотях они
    //		float seg1_line2_start = a2*start1.x + b2*start1.y + d2;
    //		float seg1_line2_end = a2*end1.x + b2*end1.y + d2;
    //		
    //		float seg2_line1_start = a1*start2.x + b1*start2.y + d1;
    //		float seg2_line1_end = a1*end2.x + b1*end2.y + d1;
    //		
    //		//если концы одного отрезка имеют один знак, значит он в одной полуплоскости и пересечения нет.
    //		if (seg1_line2_start * seg1_line2_end >= 0 || seg2_line1_start * seg2_line1_end >= 0) 
    //			return false;
    //		
    //		float u = seg1_line2_start / (seg1_line2_start - seg1_line2_end);
    //		*out_intersection =  start1 + u*dir1;
    //		
    //		return true;
    //	}

    //

    public static GameObject GetChildGameObject(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetCurrentMethod()
    {
        StackTrace st = new StackTrace();
        StackFrame sf = st.GetFrame(1);
        return sf.GetMethod().Name;
    }


    //------------------------->   LEAN_TWEEN DELAYED CALL
    //private static void PlayNextMatch3Track()
    //{
    //	LeanTween.cancel(gameObject);
    //	float adelay = 10.0f
    //		LeanTween.delayedCall(gameObject, adelay, PlayNextMatch3Track);
    //}
    //<------------------------


    //------------------------->   FRAME CASH
    //item.sprite = GameManager.Instance.AtlasFramesCache.CreateSpriteFromAtlas("art/items/items_atlas",_data.RequiredItems[i].ToString() + ".png");


    //------------------------->   NAME OF LAYER TO LAYER
    //ganeObject.layer = LayerMask.NameToLayer("UI")


    //-------------------------> ADD EVENT TRIGGERS VIA SCRIPT
    //public void InitTriggers(int number)
    //{
    //	//_eventTrigger = gameObject.AddComponent<EventTrigger>();
    //	_eventTrigger = transform.GetComponent<EventTrigger>();
    //	if(_eventTrigger.delegates == null)	{ _eventTrigger.delegates = new List<EventTrigger.Entry>();	}
    //	AddEventTrigger(OnEnterSelector, EventTriggerType.PointerEnter);
    //	AddEventTrigger(OnExitSelector, EventTriggerType.PointerExit);
    //	AddEventTrigger(OnClickSelector, EventTriggerType.PointerClick);
    //	_isOver = false;
    //}
    //private void AddEventTrigger(UnityAction action, EventTriggerType triggerType)
    //{
    //	EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
    //	trigger.AddListener((eventData) => action());
    //	EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
    //	_eventTrigger.delegates.Add(entry);
    //}
    //private void OnEnterSelector()
    //{
    //	//...
    //}
    //private void OnExitSelector()
    //{
    //	//...
    //}
    //private void OnClickSelector()
    //{
    //	//...
    //}
    //<--------------------------


    //------------------------->  ADD LISTENER TO BUTTON
    //_btn.onClick.AddListener(() => OnClick());
    //void OnClick()
    //{
    //	if(_active)
    //	{
    //		_active = false;
    //		if(UIConsts.ENABLED_INTERACTABLE){ _btn.interactable = _active; }
    //		Invoke("antiMuliClick", UIConsts.ANTI_MULTI_CKLICK_TIMEOUT);
    //		GameObject buttonParent = transform.parent.gameObject;
    //		buttonParent.SendMessage(name + HelperFunctions.GetCurrentMethod(), null, SendMessageOptions.RequireReceiver);
    //	}
    //}
    //<------------------------


    //------------------------->   ADD LISTENER TO SCROLLBAR
    //ScrollbarMusic = HelperFunctions.GetChildGameObject(gameObject, "ScrollBar_Music").GetComponent<Scrollbar>();
    //ScrollbarMusic.onValueChanged.AddListener(OnMusicValueChanged);
    //void OnMusicValueChanged(float value)
    //{
    //	Debug.Log ("------- MUSIC ------- " + value.ToString());
    //}
    //<------------------------


    //-------------------------> ADD LISTENERS TO INPUT FIELD
    //void Start ()
    //{
    //	_button = transform.Find("InputWindowButtonOk").gameObject;
    //	InputField input = transform.Find("InputField").GetComponent<InputField>();
    //	InputField.SubmitEvent se = new InputField.SubmitEvent();
    //	se.AddListener(InputWindowOnEndEdit);
    //	input.onEndEdit = se;
    //	InputField.OnChangeEvent se2 = new InputField.OnChangeEvent();
    //	se2.AddListener(InputWindowOnValueChanged);
    //	input.onValueChange = se2;
    //}
    //private void InputWindowOnEndEdit (string value)
    //{
    //	//Debug.Log(HelperFunctions.GetCurrentMethod() + " " + this.name);
    //	InputWindowButtonOkOnClick ();
    //}
    //private void InputWindowOnValueChanged(string value)
    //{
    //	if (value == "")
    //	{
    //		_button.SetActive(false);
    //	} else
    //	{
    //		_button.SetActive(true);
    //	}
    //}
    //<------------------------


    //-------------------------> LOAD TEXTURES
    //curs1.Texture = (Texture2D)Resources.LoadAssetAtPath("Assets/Resources/art/ui/common/custom_cursor_1.png", typeof(Texture2D));
    //curs0.Texture = Resources.Load("art/ui/common/custom_cursor_0", typeof(Texture2D)) as Texture2D;
    //<------------------------

}





//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//using System.Runtime.Serialization.Formatters.Binary;
//using System.IO;
//using System;
//using UnityEngine.UI;

//public class Helpers : ScriptableObject
//{

//    public static void SetAlpha(GameObject obj, float a)
//    {
//        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
//        if (sr)
//        {
//            Color acolor = sr.color;
//            acolor.a = a;
//            sr.color = acolor;
//        }
//    }

//    public static void SetImageAlpha(Image im, float a)
//    {
//        if (im)
//        {
//            Color acolor = im.color;
//            acolor.a = a;
//            im.color = acolor;
//        }
//    }

//    public static IEnumerator FadeOutCanvasGroup(CanvasGroup cg, float atime) // bad variant, can be troubles
//    {
//        float time = atime;
//        while (cg.alpha > 0)
//        {
//            cg.alpha -= Time.deltaTime / time;
//            yield return null;
//        }
//        cg.alpha = 0;
//        yield return null;
//    }

//    public static LTDescr FadeInCanvasGroup(GameObject obj, CanvasGroup cg, float atime, bool autoTime = true) // good variant
//    {
//        float time = atime;
//        if (autoTime)
//        {
//            time = atime * (1 - cg.alpha);
//        }

//        return LeanTween.value(obj, cg.alpha, 1.0f, time)
//            //.setEase(UIConsts.SHOW_EASE)
//            //	.setDelay(UIConsts.SHOW_DELAY_TIME)
//            .setOnUpdate
//                (
//                    (float val) =>
//                    {
//                        cg.alpha = val;
//                    }
//                )
//                .setOnComplete(() => { cg.alpha = 1; });
//    }

//    public static LTDescr FadeOutCanvasGroup(GameObject obj, CanvasGroup cg, float atime, bool autoTime = false) // good variant
//    {
//        float time = atime;
//        if (autoTime)
//        {
//            time = atime * cg.alpha;
//        }

//        return LeanTween.value(obj, cg.alpha, 0.0f, time)
//            //.setEase(UIConsts.SHOW_EASE)
//            //	.setDelay(UIConsts.SHOW_DELAY_TIME)
//            .setOnUpdate
//                (
//                    (float val) =>
//                    {
//                        cg.alpha = val;
//                    }
//                )
//                .setOnComplete(() => { cg.alpha = 0; });
//    }

//    public static LTDescr FadeInImage(GameObject obj, Image im, float atime, bool autoTime = true)
//    {
//        float time = atime;
//        if (autoTime)
//        {
//            time = atime * (1 - im.color.a);
//        }

//        return LeanTween.value(obj, im.color.a, 1.0f, time)
//            //.setEase(UIConsts.SHOW_EASE)
//            //	.setDelay(UIConsts.SHOW_DELAY_TIME)
//            .setOnUpdate
//                (
//                    (float val) =>
//                    {
//                        Color color = im.color;
//                        color.a = val;
//                        im.color = color;
//                    }
//                )
//                .setOnComplete(() => {
//                    Color color = im.color;
//                    color.a = 1;
//                    im.color = color;
//                });
//    }

//    public static LTDescr FadeOutImage(GameObject obj, Image im, float atime, bool autoTime = false) // good variant
//    {
//        float time = atime;
//        if (autoTime)
//        {
//            time = atime * im.color.a;
//        }

//        return LeanTween.value(obj, im.color.a, 0.0f, time)
//            //.setEase(UIConsts.SHOW_EASE)
//            //	.setDelay(UIConsts.SHOW_DELAY_TIME)
//            .setOnUpdate
//                (
//                    (float val) =>
//                    {
//                        Color color = im.color;
//                        color.a = val;
//                        im.color = color;
//                    }
//                )
//                .setOnComplete(() => {
//                    Color color = im.color;
//                    color.a = 0;
//                    im.color = color;
//                });
//    }

//    public static IEnumerator FadeInCanvasGroup(CanvasGroup cg, float atime) // bad variant, can be troubles
//    {
//        float time = atime;
//        while (cg.alpha < 1)
//        {
//            cg.alpha += Time.deltaTime / time;
//            yield return null;
//        }
//        cg.alpha = 1;
//        yield return null;
//    }

//    public static string ObjectToStr<T>(T _saveMe)
//    {
//        BinaryFormatter _bin = new BinaryFormatter();
//        MemoryStream _mem = new MemoryStream();
//        _bin.Serialize(_mem, _saveMe);

//        return Convert.ToBase64String(
//            _mem.GetBuffer()
//            );
//    }

//    public static T StrToObject<T>(string _data) where T : class
//    {
//        if (!String.IsNullOrEmpty(_data))
//        {
//            BinaryFormatter _bin = new BinaryFormatter();
//            try
//            {
//                MemoryStream _mem = new MemoryStream(Convert.FromBase64String(_data));

//                T _obj = _bin.Deserialize(_mem) as T;

//                return _obj;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }

//        }
//        else
//        {
//            throw new Exception("_data is null or empty");
//        }
//    }

//    public static List<E> ShuffleList<E>(List<E> inputList)
//    {
//        List<E> randomList = new List<E>();

//        System.Random r = new System.Random();
//        int randomIndex = 0;
//        while (inputList.Count > 0)
//        {
//            randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
//            randomList.Add(inputList[randomIndex]); //add it to the new, random list
//            inputList.RemoveAt(randomIndex); //remove to avoid duplicates
//        }
//        return randomList; //return the new random list
//    }



//    public static void SetXY(Transform trans, float x, float y)
//    {
//        Vector3 pos = trans.position;
//        pos.x = x;
//        pos.y = y;
//        trans.position = pos;
//    }

//    public static void SetXY(GameObject obj, float x, float y)
//    {
//        SetXY(obj.transform, x, y);
//    }

//    public static void SetXY(GameObject obj, Vector2 pos)
//    {
//        SetXY(obj.transform, pos.x, pos.y);
//    }

//    public static void SetXYLocal(Transform trans, float x, float y)
//    {
//        Vector3 pos = trans.localPosition;
//        pos.x = x;
//        pos.y = y;
//        trans.localPosition = pos;
//    }

//    public static void SetXYLocal(GameObject obj, float x, float y)
//    {
//        SetXYLocal(obj.transform, x, y);
//    }

//    public static void SetXYLocal(GameObject obj, Vector2 pos)
//    {
//        SetXYLocal(obj.transform, pos.x, pos.y);
//    }

//    public static void SetZ(Transform trans, float z)
//    {
//        Vector3 pos = trans.position;
//        pos.z = z;
//        trans.position = pos;
//    }

//    public static void SetZ(GameObject obj, float z)
//    {
//        SetZ(obj.transform, z);
//    }

//    public static void SetZLocal(Transform trans, float z)
//    {
//        Vector3 pos = trans.localPosition;
//        pos.z = z;
//        trans.localPosition = pos;
//    }

//    public static void SetZLocal(GameObject obj, float z)
//    {
//        SetZLocal(obj.transform, z);
//    }

//    public static void SetX(Transform trans, float x)
//    {
//        Vector3 pos = trans.position;
//        pos.x = x;
//        trans.position = pos;
//    }

//    public static void SetX(GameObject obj, float x)
//    {
//        SetX(obj.transform, x);
//    }

//    public static void SetXLocal(Transform trans, float x)
//    {
//        Vector3 pos = trans.localPosition;
//        pos.x = x;
//        trans.localPosition = pos;
//    }

//    public static void SetXLocal(GameObject obj, float x)
//    {
//        SetXLocal(obj.transform, x);
//    }

//    public static void SetY(Transform trans, float y)
//    {
//        Vector3 pos = trans.position;
//        pos.y = y;
//        trans.position = pos;
//    }

//    public static void SetY(GameObject obj, float y)
//    {
//        SetY(obj.transform, y);
//    }

//    public static void SetYLocal(Transform trans, float y)
//    {
//        Vector3 pos = trans.localPosition;
//        pos.y = y;
//        trans.localPosition = pos;
//    }

//    public static void SetYLocal(GameObject obj, float y)
//    {
//        SetYLocal(obj.transform, y);
//    }

//    public static void SetScaleXY(GameObject obj, float x, float y)
//    {
//        SetScaleXY(obj.transform, x, y);
//    }

//    public static void SetScaleXY(Transform trans, float x, float y)
//    {
//        Vector3 scale = trans.localScale;
//        scale.x = x;
//        scale.y = y;
//        trans.localScale = scale;
//    }

//    public static void SetScaleXY(GameObject obj, Vector2 xy)
//    {
//        SetScaleXY(obj.transform, xy.x, xy.y);
//    }

//    public static void SetScaleXY(Transform trans, Vector2 xy)
//    {
//        SetScaleXY(trans, xy.x, xy.y);
//    }


//    public static float GetAngleBetweenPointsRad(Vector2 a, Vector2 b)
//    {
//        return Mathf.Atan2(b.y - a.y, b.x - a.x);
//    }

//    public static float GetAngleBetweenPointsDgs(Vector2 a, Vector2 b)
//    {
//        float angle = Mathf.Atan2(b.y - a.y, b.x - a.x);
//        return (angle * 180 / (float)Mathf.PI);
//    }

//    public static Vector2 GetPositionOfPoint(Vector3 A, Vector3 B, float distance)
//    {
//        // NOT TESTED !!!!!
//        // позиція точки , що лежить на прямій яка з"єднує точки А і В при відомій дальності 
//        float angle = Mathf.Atan2(B.y - A.y, B.x - A.x);
//        angle = angle * 180 / (float)Mathf.PI;
//        float resХ = A.x + distance * Mathf.Cos(angle);
//        float resY = A.y + distance * Mathf.Sin(angle);
//        return new Vector2(resХ, resY);
//    }
//}
