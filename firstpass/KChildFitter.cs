using UnityEngine;
using UnityEngine.UI;

public class KChildFitter : MonoBehaviour
{
	public bool fitWidth;

	public bool fitHeight;

	public float HeightPadding = 0f;

	public float WidthPadding = 0f;

	public float WidthScale = 1f;

	public float HeightScale = 1f;

	public LayoutElement overrideLayoutElement;

	private RectTransform rect_transform;

	private VerticalLayoutGroup VLG;

	private HorizontalLayoutGroup HLG;

	private GridLayoutGroup GLG;

	public bool findTotalBounds = true;

	public bool includeLayoutGroupPadding = true;

	private void Awake()
	{
		rect_transform = GetComponent<RectTransform>();
		VLG = GetComponent<VerticalLayoutGroup>();
		HLG = GetComponent<HorizontalLayoutGroup>();
		GLG = GetComponent<GridLayoutGroup>();
		if ((Object)overrideLayoutElement == (Object)null)
		{
			overrideLayoutElement = GetComponent<LayoutElement>();
		}
	}

	private void LateUpdate()
	{
		FitSize();
	}

	public Vector2 GetPositionRelativeToTopLeftPivot(RectTransform element)
	{
		Vector2 zero = Vector2.zero;
		Vector2 anchoredPosition = element.anchoredPosition;
		float x = anchoredPosition.x;
		Vector2 sizeDelta = element.sizeDelta;
		float x2 = sizeDelta.x;
		Vector2 pivot = element.pivot;
		zero.x = x - x2 * pivot.x;
		Vector2 anchoredPosition2 = element.anchoredPosition;
		float y = anchoredPosition2.y;
		Vector2 sizeDelta2 = element.sizeDelta;
		float y2 = sizeDelta2.y;
		Vector2 pivot2 = element.pivot;
		zero.y = y + y2 * (1f - pivot2.y);
		return zero;
	}

	public void FitSize()
	{
		if (fitWidth || fitHeight)
		{
			Vector2 sizeDelta = rect_transform.sizeDelta;
			if (fitWidth)
			{
				sizeDelta.x = 0f;
			}
			if (fitHeight)
			{
				sizeDelta.y = 0f;
			}
			float num = float.NegativeInfinity;
			float num2 = float.PositiveInfinity;
			float num3 = float.PositiveInfinity;
			float num4 = float.NegativeInfinity;
			int childCount = base.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				LayoutElement component = child.gameObject.GetComponent<LayoutElement>();
				if (((Object)component == (Object)null || !component.ignoreLayout) && child.gameObject.activeSelf)
				{
					RectTransform rectTransform = child as RectTransform;
					if (fitWidth)
					{
						if (findTotalBounds)
						{
							Vector2 positionRelativeToTopLeftPivot = GetPositionRelativeToTopLeftPivot(rectTransform);
							float x = positionRelativeToTopLeftPivot.x;
							Vector2 sizeDelta2 = rectTransform.sizeDelta;
							float num5 = x + sizeDelta2.x;
							if (num5 > num4)
							{
								num4 = num5;
							}
							Vector2 positionRelativeToTopLeftPivot2 = GetPositionRelativeToTopLeftPivot(rectTransform);
							float x2 = positionRelativeToTopLeftPivot2.x;
							if (x2 < num3)
							{
								num3 = x2;
							}
							sizeDelta.x = Mathf.Abs(num4 - num3);
							if (includeLayoutGroupPadding)
							{
								sizeDelta.x += (float)(((Object)VLG != (Object)null) ? (VLG.padding.left + VLG.padding.right) : 0);
								sizeDelta.x += (float)(((Object)HLG != (Object)null) ? (HLG.padding.left + HLG.padding.right) : 0);
								sizeDelta.x += (float)(((Object)GLG != (Object)null) ? (GLG.padding.left + GLG.padding.right) : 0);
							}
						}
						else
						{
							float x3 = sizeDelta.x;
							Vector2 sizeDelta3 = rectTransform.sizeDelta;
							sizeDelta.x = x3 + sizeDelta3.x;
							if ((bool)HLG)
							{
								sizeDelta.x += HLG.spacing;
							}
						}
					}
					if (fitHeight)
					{
						if (findTotalBounds)
						{
							Vector2 positionRelativeToTopLeftPivot3 = GetPositionRelativeToTopLeftPivot(rectTransform);
							if (positionRelativeToTopLeftPivot3.y > num)
							{
								Vector2 positionRelativeToTopLeftPivot4 = GetPositionRelativeToTopLeftPivot(rectTransform);
								num = positionRelativeToTopLeftPivot4.y;
							}
							Vector2 positionRelativeToTopLeftPivot5 = GetPositionRelativeToTopLeftPivot(rectTransform);
							float y = positionRelativeToTopLeftPivot5.y;
							Vector2 sizeDelta4 = rectTransform.sizeDelta;
							if (y - sizeDelta4.y < num2)
							{
								Vector2 positionRelativeToTopLeftPivot6 = GetPositionRelativeToTopLeftPivot(rectTransform);
								float y2 = positionRelativeToTopLeftPivot6.y;
								Vector2 sizeDelta5 = rectTransform.sizeDelta;
								num2 = y2 - sizeDelta5.y;
							}
							sizeDelta.y = Mathf.Abs(num - num2);
							if (includeLayoutGroupPadding)
							{
								sizeDelta.y += (float)(((Object)VLG != (Object)null) ? (VLG.padding.bottom + VLG.padding.top) : 0);
								sizeDelta.y += (float)(((Object)HLG != (Object)null) ? (HLG.padding.bottom + HLG.padding.top) : 0);
								sizeDelta.y += (float)(((Object)GLG != (Object)null) ? (GLG.padding.bottom + GLG.padding.top) : 0);
							}
						}
						else
						{
							float y3 = sizeDelta.y;
							Vector2 sizeDelta6 = rectTransform.sizeDelta;
							sizeDelta.y = y3 + sizeDelta6.y;
							if ((bool)VLG)
							{
								sizeDelta.y += VLG.spacing;
							}
						}
					}
				}
			}
			Vector2 vector = new Vector2(WidthPadding, HeightPadding);
			if (!fitWidth)
			{
				WidthPadding = 0f;
			}
			if (!fitHeight)
			{
				HeightPadding = 0f;
			}
			if ((Object)overrideLayoutElement != (Object)null)
			{
				if (fitWidth && overrideLayoutElement.minWidth != (sizeDelta.x + vector.x) * WidthScale)
				{
					overrideLayoutElement.minWidth = (sizeDelta.x + vector.x) * WidthScale;
				}
				if (fitHeight && overrideLayoutElement.minHeight != (sizeDelta.y + vector.y) * HeightScale)
				{
					overrideLayoutElement.minHeight = (sizeDelta.y + vector.y) * HeightScale;
				}
			}
			Vector2 vector2 = new Vector2(WidthScale * (sizeDelta.x + vector.x), HeightScale * (sizeDelta.y + vector.y));
			if (rect_transform.sizeDelta != vector2)
			{
				rect_transform.sizeDelta = vector2;
				if ((Object)base.transform.parent != (Object)null)
				{
					KChildFitter component2 = base.transform.parent.GetComponent<KChildFitter>();
					if ((Object)component2 != (Object)null)
					{
						component2.FitSize();
					}
				}
			}
		}
	}
}
