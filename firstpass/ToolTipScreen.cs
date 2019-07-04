using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipScreen : KScreen
{
	public GameObject ToolTipPrefab;

	public RectTransform anchorRoot;

	private GameObject toolTipWidget;

	private ToolTip prevTooltip;

	private ToolTip tooltipSetting;

	public GameObject labelPrefab;

	private GameObject multiTooltipContainer;

	private bool toolTipIsBlank;

	private Vector2 ScreenEdgePadding = new Vector2(8f, 8f);

	private ToolTip dirtyHoverTooltip = null;

	private bool tooltipIncubating = true;

	public static ToolTipScreen Instance
	{
		get;
		private set;
	}

	protected override void OnActivate()
	{
		Instance = this;
		toolTipWidget = Util.KInstantiate(ToolTipPrefab, base.gameObject, null);
		toolTipWidget.transform.SetParent(base.gameObject.transform, false);
		Util.Reset(toolTipWidget.transform);
		toolTipWidget.SetActive(false);
	}

	protected override void OnCleanUp()
	{
		Instance = null;
	}

	public void SetToolTip(ToolTip tool_tip)
	{
		tooltipSetting = tool_tip;
		multiTooltipContainer = toolTipWidget.transform.Find("MultitooltipContainer").gameObject;
		ConfigureTooltip();
	}

	private void ConfigureTooltip()
	{
		if ((Object)tooltipSetting == (Object)null)
		{
			prevTooltip = null;
		}
		if ((Object)tooltipSetting != (Object)null && (Object)dirtyHoverTooltip != (Object)null && (Object)tooltipSetting == (Object)dirtyHoverTooltip)
		{
			ClearToolTip(dirtyHoverTooltip);
		}
		if ((Object)tooltipSetting != (Object)null)
		{
			tooltipSetting.RebuildDynamicTooltip();
			if (tooltipSetting.multiStringCount == 0)
			{
				clearMultiStringTooltip();
			}
			else if ((Object)prevTooltip != (Object)tooltipSetting || !multiTooltipContainer.activeInHierarchy)
			{
				prepareMultiStringTooltip(tooltipSetting);
				prevTooltip = tooltipSetting;
			}
			bool flag = multiTooltipContainer.transform.childCount != 0;
			toolTipWidget.SetActive(flag);
			if (flag)
			{
				RectTransform rectTransform = (!((Object)tooltipSetting.overrideParentObject == (Object)null)) ? tooltipSetting.overrideParentObject : tooltipSetting.GetComponent<RectTransform>();
				RectTransform component = toolTipWidget.GetComponent<RectTransform>();
				component.transform.SetParent(anchorRoot.transform);
				if (!tooltipSetting.worldSpace)
				{
					anchorRoot.anchoredPosition = rectTransform.transform.GetPosition();
				}
				else
				{
					anchorRoot.anchoredPosition = WorldToScreen(rectTransform.transform.GetPosition()) + new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f);
				}
				RectTransform rectTransform2 = anchorRoot;
				Vector2 anchoredPosition = rectTransform2.anchoredPosition;
				Vector2 up = Vector2.up;
				Vector2 pivot = rectTransform.rectTransform().pivot;
				float y = pivot.y;
				Vector2 sizeDelta = rectTransform.rectTransform().sizeDelta;
				rectTransform2.anchoredPosition = anchoredPosition - up * (y * sizeDelta.y);
				RectTransform rectTransform3 = anchorRoot;
				Vector2 anchoredPosition2 = rectTransform3.anchoredPosition;
				Vector2 right = Vector2.right;
				Vector2 pivot2 = rectTransform.rectTransform().pivot;
				float x = pivot2.x;
				Vector2 sizeDelta2 = rectTransform.rectTransform().sizeDelta;
				rectTransform3.anchoredPosition = anchoredPosition2 - right * (x * sizeDelta2.x);
				RectTransform rectTransform4 = anchorRoot;
				Vector2 anchoredPosition3 = rectTransform4.anchoredPosition;
				Vector2 right2 = Vector2.right;
				Vector2 sizeDelta3 = rectTransform.sizeDelta;
				rectTransform4.anchoredPosition = anchoredPosition3 + right2 * (sizeDelta3.x * tooltipSetting.parentPositionAnchor.x);
				RectTransform rectTransform5 = anchorRoot;
				Vector2 anchoredPosition4 = rectTransform5.anchoredPosition;
				Vector2 up2 = Vector2.up;
				Vector2 sizeDelta4 = rectTransform.sizeDelta;
				rectTransform5.anchoredPosition = anchoredPosition4 + up2 * (sizeDelta4.y * tooltipSetting.parentPositionAnchor.y);
				float num = 1f;
				CanvasScaler component2 = base.transform.parent.GetComponent<CanvasScaler>();
				if ((Object)component2 == (Object)null)
				{
					component2 = base.transform.parent.parent.GetComponent<CanvasScaler>();
				}
				if ((Object)component2 != (Object)null)
				{
					num = component2.scaleFactor;
				}
				RectTransform rectTransform6 = anchorRoot;
				Vector2 anchoredPosition5 = anchorRoot.anchoredPosition;
				float x2 = anchoredPosition5.x / num;
				Vector2 anchoredPosition6 = anchorRoot.anchoredPosition;
				rectTransform6.anchoredPosition = new Vector2(x2, anchoredPosition6.y / num);
				component.pivot = tooltipSetting.tooltipPivot;
				Vector2 vector3 = component.anchorMin = (component.anchorMax = new Vector2(0f, 0f));
				component.anchoredPosition = tooltipSetting.tooltipPositionOffset * num;
				if (!tooltipSetting.worldSpace)
				{
					Rect rect = ((RectTransform)base.transform).rect;
					Vector3 position = base.transform.GetPosition();
					float x3 = position.x;
					Vector3 position2 = base.transform.GetPosition();
					Vector2 vector4 = new Vector2(x3, position2.y) + ScreenEdgePadding;
					Vector3 position3 = base.transform.GetPosition();
					float x4 = position3.x;
					Vector3 position4 = base.transform.GetPosition();
					Vector2 vector5 = new Vector2(x4, position4.y) + rect.width * Vector2.right + rect.height * Vector2.up - ScreenEdgePadding * Mathf.Max(1f, num);
					vector5.x *= num;
					vector5.y *= num;
					Vector3 position5 = component.GetPosition();
					float x5 = position5.x;
					vector3 = component.pivot;
					float x6 = vector3.x;
					Vector2 sizeDelta5 = component.sizeDelta;
					Vector2 vector6 = default(Vector2);
					vector6.x = x5 - x6 * (sizeDelta5.x * num);
					Vector3 position6 = component.GetPosition();
					float y2 = position6.y;
					Vector2 pivot3 = component.pivot;
					float y3 = pivot3.y;
					Vector2 sizeDelta6 = component.sizeDelta;
					vector6.y = y2 - y3 * (sizeDelta6.y * num);
					Vector3 position7 = component.GetPosition();
					float x7 = position7.x;
					Vector2 pivot4 = component.pivot;
					float num2 = 1f - pivot4.x;
					Vector2 sizeDelta7 = component.sizeDelta;
					Vector2 vector7 = default(Vector2);
					vector7.x = x7 + num2 * (sizeDelta7.x * num);
					Vector3 position8 = component.GetPosition();
					float y4 = position8.y;
					Vector2 pivot5 = component.pivot;
					float num3 = 1f - pivot5.y;
					Vector2 sizeDelta8 = component.sizeDelta;
					vector7.y = y4 + num3 * (sizeDelta8.y * num);
					Vector2 vector8 = Vector2.zero;
					if (vector6.x < vector4.x)
					{
						vector8.x = vector4.x - vector6.x;
					}
					if (vector7.x > vector5.x)
					{
						vector8.x = vector5.x - vector7.x;
					}
					if (vector6.y < vector4.y)
					{
						vector8.y = vector4.y - vector6.y;
					}
					if (vector7.y > vector5.y)
					{
						vector8.y = vector5.y - vector7.y;
					}
					vector8 /= num;
					component.anchoredPosition += vector8;
				}
			}
		}
		if (((RectTransform)base.transform).GetSiblingIndex() != base.transform.parent.childCount - 1)
		{
			((RectTransform)base.transform).SetAsLastSibling();
		}
	}

	private void prepareMultiStringTooltip(ToolTip setting)
	{
		int multiStringCount = tooltipSetting.multiStringCount;
		clearMultiStringTooltip();
		for (int i = 0; i < multiStringCount; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(labelPrefab, null, true);
			gameObject.transform.SetParent(multiTooltipContainer.transform);
		}
		for (int j = 0; j < tooltipSetting.multiStringCount; j++)
		{
			Transform child = multiTooltipContainer.transform.GetChild(j);
			LayoutElement component = child.GetComponent<LayoutElement>();
			TextMeshProUGUI component2 = child.GetComponent<TextMeshProUGUI>();
			component2.text = tooltipSetting.GetMultiString(j);
			SetTextStyleSetting component3 = child.GetComponent<SetTextStyleSetting>();
			component3.SetStyle((TextStyleSetting)tooltipSetting.GetStyleSetting(j));
			if (setting.SizingSetting == ToolTip.ToolTipSizeSetting.MaxWidthWrapContent)
			{
				float num2 = component.minWidth = (component.preferredWidth = setting.WrapWidth);
				component.rectTransform().sizeDelta = new Vector2(setting.WrapWidth, 1000f);
				num2 = (component.minHeight = (component.preferredHeight = component2.preferredHeight));
				num2 = (component.minHeight = (component.preferredHeight = component2.preferredHeight));
				component.rectTransform().sizeDelta = new Vector2(setting.WrapWidth, component.minHeight);
				GetComponentInChildren<ContentSizeFitter>(true).horizontalFit = ContentSizeFitter.FitMode.MinSize;
				multiTooltipContainer.GetComponent<LayoutElement>().minWidth = setting.WrapWidth;
			}
			else if (setting.SizingSetting == ToolTip.ToolTipSizeSetting.DynamicWidthNoWrap)
			{
				GetComponentInChildren<ContentSizeFitter>(true).horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
				Vector2 preferredValues = component2.GetPreferredValues();
				LayoutElement component4 = multiTooltipContainer.GetComponent<LayoutElement>();
				float num2 = component.preferredWidth = preferredValues.x;
				num2 = (component4.minWidth = (component.minWidth = num2));
				num2 = (component.minHeight = (component.preferredHeight = preferredValues.y));
				GetComponentInChildren<ContentSizeFitter>(true).SetLayoutHorizontal();
				GetComponentInChildren<ContentSizeFitter>(true).SetLayoutVertical();
				multiTooltipContainer.rectTransform().sizeDelta = new Vector2(component.minWidth, component.minHeight);
				multiTooltipContainer.transform.parent.rectTransform().sizeDelta = multiTooltipContainer.rectTransform().sizeDelta;
			}
			component2.ForceMeshUpdate();
		}
		tooltipIncubating = true;
	}

	private void Update()
	{
		if ((Object)tooltipSetting != (Object)null)
		{
			tooltipSetting.UpdateWhileHovered();
		}
		if (!((Object)multiTooltipContainer == (Object)null) && !((Object)anchorRoot == (Object)null))
		{
			if ((Object)dirtyHoverTooltip != (Object)null)
			{
				ToolTip tt = dirtyHoverTooltip;
				MakeDirtyTooltipClean(tt);
				ClearToolTip(tt);
			}
			if (tooltipIncubating)
			{
				tooltipIncubating = false;
				Image componentInChildren = anchorRoot.GetComponentInChildren<Image>();
				if ((Object)componentInChildren != (Object)null)
				{
					anchorRoot.GetComponentInChildren<Image>(true).enabled = false;
				}
				multiTooltipContainer.transform.localScale = Vector3.zero;
				for (int i = 0; i < multiTooltipContainer.transform.childCount; i++)
				{
					if (multiTooltipContainer.transform.GetChild(i).transform.localScale != Vector3.one)
					{
						multiTooltipContainer.transform.GetChild(i).transform.localScale = Vector3.one;
					}
					LayoutElement component = multiTooltipContainer.transform.GetChild(i).GetComponent<LayoutElement>();
					TextMeshProUGUI component2 = component.GetComponent<TextMeshProUGUI>();
					toolTipIsBlank = ((component2.text == "") ? true : false);
					if (component.minHeight != component2.preferredHeight)
					{
						component.minHeight = component2.preferredHeight;
					}
				}
			}
			else if (multiTooltipContainer.transform.localScale != Vector3.one && !toolTipIsBlank)
			{
				Image componentInChildren2 = anchorRoot.GetComponentInChildren<Image>();
				if ((Object)componentInChildren2 != (Object)null)
				{
					anchorRoot.GetComponentInChildren<Image>(true).enabled = true;
				}
				multiTooltipContainer.transform.localScale = Vector3.one;
			}
		}
	}

	public void HotSwapTooltipString(string newString, int lineIndex)
	{
		Transform transform = null;
		if (multiTooltipContainer.transform.childCount > lineIndex)
		{
			transform = multiTooltipContainer.transform.GetChild(lineIndex);
			TextMeshProUGUI component = transform.GetComponent<TextMeshProUGUI>();
			component.text = newString;
		}
	}

	private void clearMultiStringTooltip()
	{
		for (int num = multiTooltipContainer.transform.childCount - 1; num >= 0; num--)
		{
			Object.DestroyImmediate(multiTooltipContainer.transform.GetChild(num).gameObject);
		}
	}

	public void ClearToolTip(ToolTip tt)
	{
		if ((Object)tt == (Object)tooltipSetting)
		{
			tooltipSetting = null;
			if ((Object)toolTipWidget != (Object)null)
			{
				clearMultiStringTooltip();
				toolTipWidget.SetActive(false);
			}
		}
	}

	public void MarkTooltipDirty(ToolTip tt)
	{
		if ((Object)tt == (Object)tooltipSetting)
		{
			dirtyHoverTooltip = tt;
		}
	}

	public void MakeDirtyTooltipClean(ToolTip tt)
	{
		if ((Object)tt == (Object)dirtyHoverTooltip)
		{
			dirtyHoverTooltip = null;
		}
	}
}
