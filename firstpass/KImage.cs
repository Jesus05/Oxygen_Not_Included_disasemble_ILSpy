using UnityEngine;
using UnityEngine.UI;

public class KImage : Image
{
	public enum ColorSelector
	{
		Active,
		Inactive,
		Disabled,
		Hover
	}

	public ColorSelector defaultState = ColorSelector.Inactive;

	private ColorSelector colorSelector = ColorSelector.Inactive;

	public ColorStyleSetting colorStyleSetting;

	public bool clearMaskOnDisable = true;

	public ColorSelector ColorState
	{
		set
		{
			colorSelector = value;
			ApplyColorStyleSetting();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		ColorState = defaultState;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[ContextMenu("Apply Color Style Settings")]
	public void ApplyColorStyleSetting()
	{
		if ((Object)colorStyleSetting != (Object)null)
		{
			switch (colorSelector)
			{
			case ColorSelector.Active:
				color = colorStyleSetting.activeColor;
				break;
			case ColorSelector.Inactive:
				color = colorStyleSetting.inactiveColor;
				break;
			case ColorSelector.Disabled:
				color = colorStyleSetting.disabledColor;
				break;
			case ColorSelector.Hover:
				color = colorStyleSetting.hoverColor;
				break;
			}
		}
	}
}
