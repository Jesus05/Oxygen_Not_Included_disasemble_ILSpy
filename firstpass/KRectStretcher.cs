using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class KRectStretcher : KMonoBehaviour
{
	public enum ParentSizeReferenceValue
	{
		SizeDelta,
		RectDimensions
	}

	public enum aspectFitOption
	{
		WidthDictatesHeight,
		HeightDictatesWidth,
		EnvelopeParent
	}

	private RectTransform rect;

	private DrivenRectTransformTracker rectTracker;

	public bool StretchX;

	public bool StretchY;

	public float XStretchFactor = 1f;

	public float YStretchFactor = 1f;

	public ParentSizeReferenceValue SizeReferenceMethod;

	public Vector2 Padding;

	public bool lerpToSize;

	public float lerpTime = 1f;

	public LayoutElement OverrideLayoutElement;

	public bool PreserveAspectRatio;

	public float aspectRatioToPreserve = 1f;

	public aspectFitOption AspectFitOption;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		rectTracker = default(DrivenRectTransformTracker);
		UpdateStretching();
	}

	private void Update()
	{
		if (base.transform.parent.hasChanged || ((Object)OverrideLayoutElement != (Object)null && OverrideLayoutElement.transform.hasChanged))
		{
			UpdateStretching();
		}
	}

	public void UpdateStretching()
	{
		if ((Object)rect == (Object)null)
		{
			rect = GetComponent<RectTransform>();
		}
		if (!((Object)rect == (Object)null) && (!((Object)base.transform.parent == (Object)null) || !((Object)OverrideLayoutElement == (Object)null)))
		{
			RectTransform rectTransform = base.transform.parent.rectTransform();
			Vector3 vector = Vector3.zero;
			if (SizeReferenceMethod == ParentSizeReferenceValue.SizeDelta)
			{
				vector = rectTransform.sizeDelta;
			}
			else if (SizeReferenceMethod == ParentSizeReferenceValue.RectDimensions)
			{
				vector = rectTransform.rect.size;
			}
			Vector2 vector2 = Vector2.zero;
			if (PreserveAspectRatio)
			{
				switch (AspectFitOption)
				{
				case aspectFitOption.WidthDictatesHeight:
				{
					float x5;
					if (StretchX)
					{
						x5 = vector.x;
					}
					else
					{
						Vector2 sizeDelta9 = rect.sizeDelta;
						x5 = sizeDelta9.x;
					}
					float y4;
					if (StretchY)
					{
						y4 = vector.x / aspectRatioToPreserve;
					}
					else
					{
						Vector2 sizeDelta10 = rect.sizeDelta;
						y4 = sizeDelta10.y;
					}
					vector2 = new Vector2(x5, y4);
					break;
				}
				case aspectFitOption.HeightDictatesWidth:
				{
					float x4;
					if (StretchX)
					{
						x4 = vector.y * aspectRatioToPreserve;
					}
					else
					{
						Vector2 sizeDelta7 = rect.sizeDelta;
						x4 = sizeDelta7.x;
					}
					float y3;
					if (StretchY)
					{
						y3 = vector.y;
					}
					else
					{
						Vector2 sizeDelta8 = rect.sizeDelta;
						y3 = sizeDelta8.y;
					}
					vector2 = new Vector2(x4, y3);
					break;
				}
				case aspectFitOption.EnvelopeParent:
				{
					Vector2 sizeDelta = rectTransform.sizeDelta;
					float x = sizeDelta.x;
					Vector2 sizeDelta2 = rectTransform.sizeDelta;
					if (x / sizeDelta2.y > aspectRatioToPreserve)
					{
						float x2;
						if (StretchX)
						{
							x2 = vector.x;
						}
						else
						{
							Vector2 sizeDelta3 = rect.sizeDelta;
							x2 = sizeDelta3.x;
						}
						float y;
						if (StretchY)
						{
							y = vector.x / aspectRatioToPreserve;
						}
						else
						{
							Vector2 sizeDelta4 = rect.sizeDelta;
							y = sizeDelta4.y;
						}
						vector2 = new Vector2(x2, y);
					}
					else
					{
						float x3;
						if (StretchX)
						{
							x3 = vector.y * aspectRatioToPreserve;
						}
						else
						{
							Vector2 sizeDelta5 = rect.sizeDelta;
							x3 = sizeDelta5.x;
						}
						float y2;
						if (StretchY)
						{
							y2 = vector.y;
						}
						else
						{
							Vector2 sizeDelta6 = rect.sizeDelta;
							y2 = sizeDelta6.y;
						}
						vector2 = new Vector2(x3, y2);
					}
					break;
				}
				}
			}
			else
			{
				float x6;
				if (StretchX)
				{
					x6 = vector.x;
				}
				else
				{
					Vector2 sizeDelta11 = rect.sizeDelta;
					x6 = sizeDelta11.x;
				}
				float y5;
				if (StretchY)
				{
					y5 = vector.y;
				}
				else
				{
					Vector2 sizeDelta12 = rect.sizeDelta;
					y5 = sizeDelta12.y;
				}
				vector2 = new Vector2(x6, y5);
			}
			if (StretchX)
			{
				vector2.x *= XStretchFactor;
			}
			if (StretchY)
			{
				vector2.y *= YStretchFactor;
			}
			if (StretchX)
			{
				vector2.x += Padding.x;
			}
			if (StretchY)
			{
				vector2.y += Padding.y;
			}
			if (rect.sizeDelta != vector2)
			{
				if (lerpToSize)
				{
					if ((Object)OverrideLayoutElement != (Object)null)
					{
						if (StretchX)
						{
							OverrideLayoutElement.minWidth = Mathf.Lerp(OverrideLayoutElement.minWidth, vector2.x, Time.unscaledDeltaTime * lerpTime);
						}
						if (StretchY)
						{
							OverrideLayoutElement.minHeight = Mathf.Lerp(OverrideLayoutElement.minHeight, vector2.y, Time.unscaledDeltaTime * lerpTime);
						}
					}
					else
					{
						rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, vector2, lerpTime * Time.unscaledDeltaTime);
					}
				}
				else
				{
					if ((Object)OverrideLayoutElement != (Object)null)
					{
						if (StretchX)
						{
							OverrideLayoutElement.minWidth = vector2.x;
						}
						if (StretchY)
						{
							OverrideLayoutElement.minHeight = vector2.y;
						}
					}
					rect.sizeDelta = vector2;
				}
			}
			for (int i = 0; i < base.transform.childCount; i++)
			{
				KRectStretcher component = base.transform.GetChild(i).GetComponent<KRectStretcher>();
				if ((bool)component)
				{
					component.UpdateStretching();
				}
			}
			rectTracker.Clear();
			if (StretchX)
			{
				rectTracker.Add(this, rect, DrivenTransformProperties.SizeDeltaX);
			}
			if (StretchY)
			{
				rectTracker.Add(this, rect, DrivenTransformProperties.SizeDeltaY);
			}
		}
	}
}
