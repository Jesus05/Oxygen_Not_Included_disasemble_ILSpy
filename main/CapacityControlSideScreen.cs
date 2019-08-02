using UnityEngine;

public class CapacityControlSideScreen : SideScreenContent
{
	private IUserControlledCapacity target;

	[Header("Slider")]
	[SerializeField]
	private KSlider slider;

	[Header("Number Input")]
	[SerializeField]
	private KNumberInputField numberInput;

	[SerializeField]
	private LocText unitsLabel;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		unitsLabel.text = target.CapacityUnits;
		slider.onDrag += delegate
		{
			ReceiveValueFromSlider(slider.value);
		};
		slider.onPointerDown += delegate
		{
			ReceiveValueFromSlider(slider.value);
		};
		slider.onMove += delegate
		{
			ReceiveValueFromSlider(slider.value);
		};
		numberInput.onEndEdit += delegate
		{
			ReceiveValueFromInput(numberInput.currentValue);
		};
		numberInput.decimalPlaces = 1;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IUserControlledCapacity>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if ((Object)new_target == (Object)null)
		{
			Debug.LogError("Invalid gameObject received");
		}
		else
		{
			target = new_target.GetComponent<IUserControlledCapacity>();
			if (target == null)
			{
				Debug.LogError("The gameObject received does not contain a IThresholdSwitch component");
			}
			else
			{
				slider.minValue = target.MinCapacity;
				slider.maxValue = target.MaxCapacity;
				slider.value = target.UserMaxCapacity;
				slider.GetComponentInChildren<ToolTip>();
				unitsLabel.text = target.CapacityUnits;
				numberInput.minValue = target.MinCapacity;
				numberInput.maxValue = target.MaxCapacity;
				numberInput.currentValue = Mathf.Max(target.MinCapacity, Mathf.Min(target.MaxCapacity, target.UserMaxCapacity));
				numberInput.Activate();
				UpdateMaxCapacityLabel();
			}
		}
	}

	private void ReceiveValueFromSlider(float newValue)
	{
		UpdateMaxCapacity(newValue);
	}

	private void ReceiveValueFromInput(float newValue)
	{
		UpdateMaxCapacity(newValue);
	}

	private void UpdateMaxCapacity(float newValue)
	{
		target.UserMaxCapacity = newValue;
		slider.value = newValue;
		UpdateMaxCapacityLabel();
	}

	private void UpdateMaxCapacityLabel()
	{
		numberInput.SetDisplayValue(target.UserMaxCapacity.ToString());
	}
}
