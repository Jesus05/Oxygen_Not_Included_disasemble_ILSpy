using UnityEngine;
using UnityEngine.UI;

public class ShadowRect : MonoBehaviour
{
	public RectTransform RectMain;

	public RectTransform RectShadow;

	[SerializeField]
	protected Color shadowColor = new Color(0f, 0f, 0f, 0.6f);

	[SerializeField]
	protected Vector2 ShadowOffset = new Vector2(1.5f, -1.5f);

	private LayoutElement shadowLayoutElement;

	private void OnEnable()
	{
		if ((Object)RectShadow != (Object)null)
		{
			RectShadow.name = "Shadow_" + RectMain.name;
			MatchRect();
		}
		else
		{
			Debug.LogWarning("Shadowrect is missing rectshadow: " + base.gameObject.name, null);
		}
	}

	private void Update()
	{
		MatchRect();
	}

	protected virtual void MatchRect()
	{
		if (!((Object)RectShadow == (Object)null) && !((Object)RectMain == (Object)null))
		{
			if ((Object)shadowLayoutElement == (Object)null)
			{
				shadowLayoutElement = RectShadow.GetComponent<LayoutElement>();
			}
			if ((Object)shadowLayoutElement != (Object)null && !shadowLayoutElement.ignoreLayout)
			{
				shadowLayoutElement.ignoreLayout = true;
			}
			if ((Object)RectShadow.transform.parent != (Object)RectMain.transform.parent)
			{
				RectShadow.transform.SetParent(RectMain.transform.parent);
			}
			if (RectShadow.GetSiblingIndex() >= RectMain.GetSiblingIndex())
			{
				RectShadow.SetAsFirstSibling();
			}
			RectShadow.transform.localScale = Vector3.one;
			if (RectShadow.pivot != RectMain.pivot)
			{
				RectShadow.pivot = RectMain.pivot;
			}
			if (RectShadow.anchorMax != RectMain.anchorMax)
			{
				RectShadow.anchorMax = RectMain.anchorMax;
			}
			if (RectShadow.anchorMin != RectMain.anchorMin)
			{
				RectShadow.anchorMin = RectMain.anchorMin;
			}
			if (RectShadow.sizeDelta != RectMain.sizeDelta)
			{
				RectShadow.sizeDelta = RectMain.sizeDelta;
			}
			if (RectShadow.anchoredPosition != RectMain.anchoredPosition + ShadowOffset)
			{
				RectShadow.anchoredPosition = RectMain.anchoredPosition + ShadowOffset;
			}
			if (RectMain.gameObject.activeInHierarchy != RectShadow.gameObject.activeInHierarchy)
			{
				RectShadow.gameObject.SetActive(RectMain.gameObject.activeInHierarchy);
			}
		}
	}
}
