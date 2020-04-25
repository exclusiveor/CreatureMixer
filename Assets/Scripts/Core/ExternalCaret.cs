using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ExternalCaret : MonoBehaviour
{
    public Transform Caret;
	public Image CaretImage;
	public Text AText;
	public float BlinkTime = 0.1f;
	public InputField AInputField;
//	protected UIVertex[] m_CursorVerts = null;
	public Color CaretColor;
//	public float AHeight = 40;
//	public float AWidth = 5;
	private CanvasRenderer m_CachedInputRenderer;
	private RectTransform caretRectTrans;
	public bool HideOriginalCursor = true;


	void Start()
	{
		LeanTween.value(AText.gameObject, 1.0f, 0.0f, BlinkTime)
			//.setEase(UIConsts.SHOW_EASE)
			//	.setDelay(UIConsts.SHOW_DELAY_TIME)
			.setLoopPingPong()
			.setLoopCount(-1)
			.setOnUpdate
				(
					(float val)=>
					{
						Color c = CaretColor;
						c.a = val;
						CaretImage.color = c;
					}
				);

//		m_CursorVerts = new UIVertex[4];
//		for (int i = 0; i < m_CursorVerts.Length; i++)
//		{
//			m_CursorVerts[i] = UIVertex.simpleVert;
//			m_CursorVerts[i].color = Color.black;
//			m_CursorVerts[i].uv0 = Vector2.zero;
//		}
		//
//		GameObject go = new GameObject(transform.name + " Adj Caret");
//		go.hideFlags = HideFlags.DontSave;
//		go.transform.SetParent(transform.parent);
//		go.transform.SetAsFirstSibling();
//		go.layer = gameObject.layer;
//		
//		caretRectTrans = go.AddComponent<RectTransform>();
//		m_CachedInputRenderer = go.AddComponent<CanvasRenderer>();
//		m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, null);
//		
//		// Needed as if any layout is present we want the caret to always be the same as the text area.
//		go.AddComponent<LayoutElement>().ignoreLayout = true;
//		AssignPositioningIfNeeded();

		if (HideOriginalCursor)
		{
			Invoke("RemoveOriginalCaret", 0.1f);
		}
	}

	void RemoveOriginalCaret()
	{
		Transform tr = AInputField.transform.Find("InputField Input Caret");
		if (tr != null)
		{
			tr.gameObject.AddComponent<CanvasGroup>().alpha = 0;
		} else
		{
			Invoke("RemoveOriginalCaret", 0.1f);
		}
	}

    void Update()
    {
		if (!AInputField.isFocused)
		{
//			m_CachedInputRenderer.SetVertices(null, 0);
			Caret.gameObject.SetActive(false);
			return;
		} else
		{
			Caret.gameObject.SetActive(true);
		}
		Vector3 tr = Caret.transform.localPosition;

//		Rect inputRect = AText.rectTransform.rect;
//		Vector2 extents = inputRect.size;
//		
//		// get the text alignment anchor point for the text in local space
//		Vector2 textAnchorPivot = Text.GetTextAnchorPivot(AText.alignment);
//		Vector2 refPoint = Vector2.zero;
//		refPoint.x = Mathf.Lerp(inputRect.xMin, inputRect.xMax, textAnchorPivot.x);
//		refPoint.y = Mathf.Lerp(inputRect.yMin, inputRect.yMax, textAnchorPivot.y);
//		
//		// Ajust the anchor point in screen space
//		Vector2 roundedRefPoint = AText.PixelAdjustPoint(refPoint);
//		
//		// Determine fraction of pixel to offset text mesh.
//		// This is the rounding in screen space, plus the fraction of a pixel the text anchor pivot is from the corner of the text mesh.
//		Vector2 roundingOffset = roundedRefPoint - refPoint + Vector2.Scale(extents, textAnchorPivot);
//		roundingOffset.x = roundingOffset.x - Mathf.Floor(0.5f + roundingOffset.x);
//		roundingOffset.y = roundingOffset.y - Mathf.Floor(0.5f + roundingOffset.y);


		TextGenerator gen = AText.cachedTextGenerator;
		float x = AText.rectTransform.rect.xMin;
		int count = Mathf.Min(AInputField.caretPosition, gen.characters.Count);
		//Debug.Log ("AInputField.caretPosition " + count);
		if (count > 0)
		{
			UICharInfo cursorChar = gen.characters[count-1];
			x = (cursorChar.cursorPos.x + cursorChar.charWidth) / AText.pixelsPerUnit;
			//Caret.localPosition = new Vector3(x, tr.y, tr.z);
			//Debug.Log ("============ " + (x * XCorrection + Dx));
		} else
		{
			//Caret.localPosition = new Vector3(x, tr.y, tr.z);
		}
		//x /= AText.pixelsPerUnit;
//		x += roundingOffset.x;
		Caret.localPosition = new Vector3(x, tr.y, tr.z);

//		m_CursorVerts[0].position = new Vector3(x, -AHeight, 0.0f);
//		m_CursorVerts[1].position = new Vector3(x + AWidth, -AHeight, 0.0f);
//		m_CursorVerts[2].position = new Vector3(x + AWidth, AHeight, 0.0f);
//		m_CursorVerts[3].position = new Vector3(x, AHeight, 0.0f);
//		List<UIVertex> vbo = new List<UIVertex>();
//		if (roundingOffset != Vector2.zero)
//		{
//			for (int i = 0; i < m_CursorVerts.Length; i++)
//			{
//				UIVertex uiv = m_CursorVerts[i];
//				uiv.position.x += roundingOffset.x;
//				uiv.position.y += roundingOffset.y;
//				vbo.Add(uiv);
//			}
//		}
//		m_CachedInputRenderer.SetVertices(vbo.ToArray(), vbo.Count);

	}

//	private void AssignPositioningIfNeeded()
//	{
//		if (AText != null && caretRectTrans != null &&
//		    (caretRectTrans.localPosition != AText.rectTransform.localPosition ||
//		 caretRectTrans.localRotation != AText.rectTransform.localRotation ||
//		 caretRectTrans.localScale != AText.rectTransform.localScale ||
//		 caretRectTrans.anchorMin != AText.rectTransform.anchorMin ||
//		 caretRectTrans.anchorMax != AText.rectTransform.anchorMax ||
//		 caretRectTrans.anchoredPosition != AText.rectTransform.anchoredPosition ||
//		 caretRectTrans.sizeDelta != AText.rectTransform.sizeDelta ||
//		 caretRectTrans.pivot != AText.rectTransform.pivot))
//		{
//			caretRectTrans.localPosition = AText.rectTransform.localPosition;
//			caretRectTrans.localRotation = AText.rectTransform.localRotation;
//			caretRectTrans.localScale = AText.rectTransform.localScale;
//			caretRectTrans.anchorMin = AText.rectTransform.anchorMin;
//			caretRectTrans.anchorMax = AText.rectTransform.anchorMax;
//			caretRectTrans.anchoredPosition = AText.rectTransform.anchoredPosition;
//			caretRectTrans.sizeDelta = AText.rectTransform.sizeDelta;
//			caretRectTrans.pivot = AText.rectTransform.pivot;
//		}
//	}
}
