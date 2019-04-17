using STRINGS;
using UnityEngine;

public class AutomatableSideScreen : SideScreenContent
{
	public KToggle allowManualToggle;

	public KImage allowManualToggleCheckMark;

	public GameObject content;

	private GameObject target;

	public LocText DescriptionText;

	private Automatable targetAutomatable;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		allowManualToggle.transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.AUTOMATABLE_SIDE_SCREEN.ALLOWMANUALBUTTONTOOLTIP);
		allowManualToggle.onValueChanged += OnAllowManualChanged;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<Automatable>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		if ((Object)target == (Object)null)
		{
			Debug.LogError("The target object provided was null");
		}
		else
		{
			targetAutomatable = target.GetComponent<Automatable>();
			if ((Object)targetAutomatable == (Object)null)
			{
				Debug.LogError("The target provided does not have an Automatable component");
			}
			else
			{
				allowManualToggle.isOn = !targetAutomatable.GetAutomationOnly();
				allowManualToggleCheckMark.enabled = allowManualToggle.isOn;
			}
		}
	}

	private void OnAllowManualChanged(bool value)
	{
		targetAutomatable.SetAutomationOnly(!value);
		allowManualToggleCheckMark.enabled = value;
	}
}
