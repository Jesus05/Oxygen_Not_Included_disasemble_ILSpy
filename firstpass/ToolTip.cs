using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTip : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public enum TooltipPosition
	{
		TopLeft,
		TopCenter,
		TopRight,
		BottomLeft,
		BottomCenter,
		BottomRight,
		Custom
	}

	public enum ToolTipSizeSetting
	{
		MaxWidthWrapContent,
		DynamicWidthNoWrap
	}

	public bool UseFixedStringKey;

	public string FixedStringKey = string.Empty;

	private List<string> multiStringToolTips = new List<string>();

	private List<ScriptableObject> styleSettings = new List<ScriptableObject>();

	public bool worldSpace;

	public bool forceRefresh;

	public bool refreshWhileHovering;

	private bool isHovering;

	private float lastUpdateTime;

	public TooltipPosition toolTipPosition = TooltipPosition.BottomCenter;

	public Vector2 tooltipPivot = new Vector2(0f, 1f);

	public Vector2 tooltipPositionOffset = new Vector2(0f, -25f);

	public Vector2 parentPositionAnchor = new Vector2(0.5f, 0.5f);

	public RectTransform overrideParentObject;

	public ToolTipSizeSetting SizingSetting = ToolTipSizeSetting.DynamicWidthNoWrap;

	public float WrapWidth = 256f;

	private Func<string> _OnToolTip;

	private static readonly EventSystem.IntraObjectHandler<ToolTip> OnClickDelegate = new EventSystem.IntraObjectHandler<ToolTip>(delegate(ToolTip component, object data)
	{
		component.OnClick(data);
	});

	public string toolTip
	{
		set
		{
			SetSimpleTooltip(value);
		}
	}

	public int multiStringCount => multiStringToolTips.Count;

	public Func<string> OnToolTip
	{
		get
		{
			return _OnToolTip;
		}
		set
		{
			_OnToolTip = value;
		}
	}

	protected override void OnPrefabInit()
	{
		if (base.gameObject.GetComponents<ToolTip>().Length > 1)
		{
			Debug.LogError("The object " + base.gameObject.name + " has more than one ToolTip, it conflict when displaying this tooltip.");
		}
		Subscribe(2098165161, OnClickDelegate);
		if (UseFixedStringKey)
		{
			string text2 = toolTip = Strings.Get(new StringKey(FixedStringKey));
		}
		switch (toolTipPosition)
		{
		case TooltipPosition.TopLeft:
			tooltipPivot = new Vector2(1f, 0f);
			tooltipPositionOffset = new Vector2(0f, 20f);
			parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		case TooltipPosition.TopCenter:
			tooltipPivot = new Vector2(0.5f, 0f);
			tooltipPositionOffset = new Vector2(0f, 20f);
			parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		case TooltipPosition.TopRight:
			tooltipPivot = new Vector2(0f, 0f);
			tooltipPositionOffset = new Vector2(0f, 20f);
			parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		case TooltipPosition.BottomLeft:
			tooltipPivot = new Vector2(1f, 1f);
			tooltipPositionOffset = new Vector2(0f, -25f);
			parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		case TooltipPosition.BottomCenter:
			tooltipPivot = new Vector2(0.5f, 1f);
			tooltipPositionOffset = new Vector2(0f, -25f);
			parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		case TooltipPosition.BottomRight:
			tooltipPivot = new Vector2(0f, 1f);
			tooltipPositionOffset = new Vector2(0f, -25f);
			parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		}
	}

	protected override void OnSpawn()
	{
		if (!worldSpace)
		{
			Canvas componentInParent = base.gameObject.GetComponentInParent<Canvas>();
			worldSpace = ((UnityEngine.Object)componentInParent != (UnityEngine.Object)null && (UnityEngine.Object)componentInParent.worldCamera != (UnityEngine.Object)null);
		}
	}

	public void SetSimpleTooltip(string message)
	{
		ClearMultiStringTooltip();
		AddMultiStringTooltip(message, PluginAssets.Instance.defaultTextStyleSetting);
	}

	public void AddMultiStringTooltip(string newString, ScriptableObject styleSetting)
	{
		multiStringToolTips.Add(newString);
		styleSettings.Add(styleSetting);
	}

	public void ClearMultiStringTooltip()
	{
		multiStringToolTips.Clear();
		styleSettings.Clear();
	}

	public string GetMultiString(int idx)
	{
		return multiStringToolTips[idx];
	}

	public ScriptableObject GetStyleSetting(int idx)
	{
		return styleSettings[idx];
	}

	public void SetFixedStringKey(string newKey)
	{
		FixedStringKey = newKey;
		string text2 = toolTip = Strings.Get(new StringKey(FixedStringKey));
	}

	public void RebuildDynamicTooltip()
	{
		if (OnToolTip != null)
		{
			ClearMultiStringTooltip();
			string text = OnToolTip();
			if (!string.IsNullOrEmpty(text))
			{
				AddMultiStringTooltip(text, PluginAssets.Instance.defaultTextStyleSetting);
			}
		}
	}

	public void OnPointerEnter(PointerEventData data)
	{
		OnHoverStateChanged(true);
		isHovering = true;
	}

	public void OnPointerExit(PointerEventData data)
	{
		OnHoverStateChanged(false);
		isHovering = false;
	}

	private void OnClick(object data)
	{
		ToolTipScreen.Instance.ClearToolTip(this);
	}

	private void OnDisable()
	{
		if ((bool)ToolTipScreen.Instance)
		{
			ToolTipScreen.Instance.MarkTooltipDirty(this);
		}
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if ((bool)ToolTipScreen.Instance)
		{
			ToolTipScreen.Instance.MarkTooltipDirty(this);
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if ((bool)ToolTipScreen.Instance)
		{
			ToolTipScreen.Instance.MakeDirtyTooltipClean(this);
		}
	}

	private void OnHoverStateChanged(bool is_over)
	{
		if (!((UnityEngine.Object)ToolTipScreen.Instance == (UnityEngine.Object)null))
		{
			if (is_over)
			{
				ToolTipScreen.Instance.SetToolTip(this);
			}
			else
			{
				ToolTipScreen.Instance.ClearToolTip(this);
			}
		}
	}

	protected override void OnCleanUp()
	{
		if ((UnityEngine.Object)ToolTipScreen.Instance != (UnityEngine.Object)null)
		{
			ToolTipScreen.Instance.ClearToolTip(this);
		}
	}

	public void UpdateWhileHovered()
	{
		if ((forceRefresh || refreshWhileHovering) && Time.unscaledTime - lastUpdateTime > 0.2f)
		{
			lastUpdateTime = Time.unscaledTime;
			if (isHovering)
			{
				RebuildDynamicTooltip();
				for (int i = 0; i < multiStringToolTips.Count; i++)
				{
					ToolTipScreen.Instance.HotSwapTooltipString(multiStringToolTips[i], i);
				}
			}
		}
	}
}
