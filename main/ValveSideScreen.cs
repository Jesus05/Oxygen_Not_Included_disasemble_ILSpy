using STRINGS;
using System.Collections;
using UnityEngine;

public class ValveSideScreen : SideScreenContent
{
	private Valve targetValve;

	[Header("Slider")]
	[SerializeField]
	private KSlider flowSlider;

	[SerializeField]
	private LocText minFlowLabel;

	[SerializeField]
	private LocText maxFlowLabel;

	[Header("Input Field")]
	[SerializeField]
	private KNumberInputField numberInput;

	[SerializeField]
	private LocText unitsLabel;

	private bool isEditing;

	private float targetFlow;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		unitsLabel.text = GameUtil.AddTimeSliceText(UI.UNITSUFFIXES.MASS.GRAM, GameUtil.TimeSlice.PerSecond);
		flowSlider.onReleaseHandle += OnReleaseHandle;
		flowSlider.onDrag += delegate
		{
			ReceiveValueFromSlider(flowSlider.value);
		};
		flowSlider.onPointerDown += delegate
		{
			ReceiveValueFromSlider(flowSlider.value);
		};
		flowSlider.onMove += delegate
		{
			ReceiveValueFromSlider(flowSlider.value);
			OnReleaseHandle();
		};
		numberInput.onEndEdit += delegate
		{
			ReceiveValueFromInput(numberInput.currentValue);
		};
		numberInput.decimalPlaces = 1;
	}

	public void OnReleaseHandle()
	{
		targetValve.ChangeFlow(targetFlow);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<Valve>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		targetValve = target.GetComponent<Valve>();
		if ((Object)targetValve == (Object)null)
		{
			Debug.LogError("The target object does not have a Valve component.");
		}
		else
		{
			flowSlider.minValue = 0f;
			flowSlider.maxValue = targetValve.MaxFlow;
			flowSlider.value = targetValve.DesiredFlow;
			minFlowLabel.text = GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, true, "{0:0.#}");
			maxFlowLabel.text = GameUtil.GetFormattedMass(targetValve.MaxFlow, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, true, "{0:0.#}");
			numberInput.minValue = 0f;
			numberInput.maxValue = targetValve.MaxFlow * 1000f;
			numberInput.SetDisplayValue(GameUtil.GetFormattedMass(Mathf.Max(0f, targetValve.DesiredFlow), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, false, "{0:0.#####}"));
			numberInput.Activate();
		}
	}

	private void ReceiveValueFromSlider(float newValue)
	{
		newValue = Mathf.Round(newValue * 1000f) / 1000f;
		UpdateFlowValue(newValue);
	}

	private void ReceiveValueFromInput(float input)
	{
		float newValue = input / 1000f;
		UpdateFlowValue(newValue);
		targetValve.ChangeFlow(targetFlow);
	}

	private void UpdateFlowValue(float newValue)
	{
		targetFlow = newValue;
		flowSlider.value = newValue;
		numberInput.SetDisplayValue(GameUtil.GetFormattedMass(newValue, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, false, "{0:0.#####}"));
	}

	private IEnumerator SettingDelay(float delay)
	{
		float startTime = Time.realtimeSinceStartup;
		float currentTime = startTime;
		if (currentTime < startTime + delay)
		{
			float num = currentTime + Time.unscaledDeltaTime;
			yield return (object)new WaitForEndOfFrame();
			/*Error: Unable to find new state assignment for yield return*/;
		}
		OnReleaseHandle();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		Debug.Log("ValveSideScreen OnKeyDown");
		if (isEditing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}
}
