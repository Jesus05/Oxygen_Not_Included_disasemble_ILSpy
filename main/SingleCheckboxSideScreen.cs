using UnityEngine;

public class SingleCheckboxSideScreen : SideScreenContent
{
	public KToggle toggle;

	public KImage toggleCheckMark;

	public LocText label;

	private ICheckboxControl target;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		toggle.onValueChanged += OnValueChanged;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<ICheckboxControl>() != null || target.GetSMI<ICheckboxControl>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		if ((Object)target == (Object)null)
		{
			Debug.LogError("The target object provided was null", null);
		}
		else
		{
			this.target = target.GetComponent<ICheckboxControl>();
			if (this.target == null)
			{
				this.target = target.GetSMI<ICheckboxControl>();
			}
			if (this.target == null)
			{
				Debug.LogError("The target provided does not have an ICheckboxControl component", null);
			}
			else
			{
				label.text = this.target.CheckboxLabel;
				toggle.transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(this.target.CheckboxTooltip);
				titleKey = this.target.CheckboxTitleKey;
				toggle.isOn = this.target.GetCheckboxValue();
				toggleCheckMark.enabled = toggle.isOn;
			}
		}
	}

	public override void ClearTarget()
	{
		base.ClearTarget();
		target = null;
	}

	private void OnValueChanged(bool value)
	{
		target.SetCheckboxValue(value);
		toggleCheckMark.enabled = value;
	}
}
