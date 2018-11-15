using STRINGS;
using UnityEngine;

public class ActiveRangeSideScreen : SideScreenContent
{
	private IActivationRangeTarget target;

	[SerializeField]
	private KSlider activateValueSlider;

	[SerializeField]
	private KSlider deactivateValueSlider;

	[SerializeField]
	private LocText activateLabel;

	[SerializeField]
	private LocText deactivateLabel;

	[SerializeField]
	private LocText activateValueLabel;

	[SerializeField]
	private LocText deactivateValueLabel;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		activateValueLabel.text = "100";
		deactivateValueLabel.text = "100";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		activateValueSlider.onValueChanged.AddListener(OnActivateValueChanged);
		deactivateValueSlider.onValueChanged.AddListener(OnDeactivateValueChanged);
	}

	private void OnActivateValueChanged(float new_value)
	{
		activateValueLabel.text = new_value.ToString();
		target.ActivateValue = new_value;
		if (target.ActivateValue < target.DeactivateValue)
		{
			activateValueSlider.value = deactivateValueSlider.value;
		}
		RefreshTooltips();
	}

	private void OnDeactivateValueChanged(float new_value)
	{
		deactivateValueLabel.text = new_value.ToString();
		target.DeactivateValue = new_value;
		if (target.DeactivateValue > target.ActivateValue)
		{
			deactivateValueSlider.value = activateValueSlider.value;
		}
		RefreshTooltips();
	}

	private void RefreshTooltips()
	{
		activateValueSlider.GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(target.ActivateTooltip, activateValueSlider.value));
		deactivateValueSlider.GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(target.DeactivateTooltip, deactivateValueSlider.value));
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IActivationRangeTarget>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if ((Object)new_target == (Object)null)
		{
			Debug.LogError("Invalid gameObject received", null);
		}
		else
		{
			target = new_target.GetComponent<IActivationRangeTarget>();
			if (target == null)
			{
				Debug.LogError("The gameObject received does not contain a IActivationRangeTarget component", null);
			}
			else
			{
				activateLabel.text = target.ActivateSliderLabelText;
				deactivateLabel.text = target.DeactivateSliderLabelText;
				activateValueSlider.onValueChanged.RemoveListener(OnActivateValueChanged);
				activateValueSlider.minValue = target.MinValue;
				activateValueSlider.maxValue = target.MaxValue;
				activateValueSlider.value = target.ActivateValue;
				activateValueSlider.wholeNumbers = target.UseWholeNumbers;
				activateValueLabel.text = target.ActivateValue.ToString();
				activateValueSlider.onValueChanged.AddListener(OnActivateValueChanged);
				deactivateValueSlider.onValueChanged.RemoveListener(OnDeactivateValueChanged);
				deactivateValueSlider.minValue = target.MinValue;
				deactivateValueSlider.maxValue = target.MaxValue;
				deactivateValueSlider.value = target.DeactivateValue;
				deactivateValueSlider.wholeNumbers = target.UseWholeNumbers;
				deactivateValueLabel.text = target.DeactivateValue.ToString();
				deactivateValueSlider.onValueChanged.AddListener(OnDeactivateValueChanged);
				RefreshTooltips();
			}
		}
	}

	public override string GetTitle()
	{
		if (target != null)
		{
			return target.ActivationRangeTitleText;
		}
		return UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.NAME;
	}
}
