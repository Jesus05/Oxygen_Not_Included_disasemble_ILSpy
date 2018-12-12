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

	[Header("Number Input")]
	[SerializeField]
	private KNumberInputField activateValueLabel;

	[SerializeField]
	private KNumberInputField deactivateValueLabel;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		activateValueLabel.maxValue = target.MaxValue;
		activateValueLabel.minValue = target.MinValue;
		deactivateValueLabel.maxValue = target.MaxValue;
		deactivateValueLabel.minValue = target.MinValue;
		activateValueSlider.onValueChanged.AddListener(OnActivateValueChanged);
		deactivateValueSlider.onValueChanged.AddListener(OnDeactivateValueChanged);
	}

	private void OnActivateValueChanged(float new_value)
	{
		target.ActivateValue = new_value;
		if (target.ActivateValue < target.DeactivateValue)
		{
			target.ActivateValue = target.DeactivateValue;
			activateValueSlider.value = target.ActivateValue;
		}
		activateValueLabel.SetDisplayValue(target.ActivateValue.ToString());
		RefreshTooltips();
	}

	private void OnDeactivateValueChanged(float new_value)
	{
		target.DeactivateValue = new_value;
		if (target.DeactivateValue > target.ActivateValue)
		{
			target.DeactivateValue = activateValueSlider.value;
			deactivateValueSlider.value = target.DeactivateValue;
		}
		deactivateValueLabel.SetDisplayValue(target.DeactivateValue.ToString());
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
				activateValueLabel.Activate();
				deactivateValueLabel.Activate();
				activateValueSlider.onValueChanged.RemoveListener(OnActivateValueChanged);
				activateValueSlider.minValue = target.MinValue;
				activateValueSlider.maxValue = target.MaxValue;
				activateValueSlider.value = target.ActivateValue;
				activateValueSlider.wholeNumbers = target.UseWholeNumbers;
				activateValueSlider.onValueChanged.AddListener(OnActivateValueChanged);
				activateValueLabel.SetDisplayValue(target.ActivateValue.ToString());
				activateValueLabel.onEndEdit += delegate
				{
					float result2 = target.ActivateValue;
					float.TryParse(activateValueLabel.field.text, out result2);
					OnActivateValueChanged(result2);
					activateValueSlider.value = result2;
				};
				deactivateValueSlider.onValueChanged.RemoveListener(OnDeactivateValueChanged);
				deactivateValueSlider.minValue = target.MinValue;
				deactivateValueSlider.maxValue = target.MaxValue;
				deactivateValueSlider.value = target.DeactivateValue;
				deactivateValueSlider.wholeNumbers = target.UseWholeNumbers;
				deactivateValueSlider.onValueChanged.AddListener(OnDeactivateValueChanged);
				deactivateValueLabel.SetDisplayValue(target.DeactivateValue.ToString());
				deactivateValueLabel.onEndEdit += delegate
				{
					float result = target.DeactivateValue;
					float.TryParse(deactivateValueLabel.field.text, out result);
					OnDeactivateValueChanged(result);
					deactivateValueSlider.value = result;
				};
				RefreshTooltips();
			}
		}
	}

	public override string GetTitle()
	{
		if (target == null)
		{
			return UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.NAME;
		}
		return target.ActivationRangeTitleText;
	}
}
