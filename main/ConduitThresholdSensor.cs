using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class ConduitThresholdSensor : ConduitSensor
{
	[SerializeField]
	[Serialize]
	protected float threshold = 0f;

	[SerializeField]
	[Serialize]
	protected bool activateAboveThreshold = true;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<ConduitThresholdSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ConduitThresholdSensor>(delegate(ConduitThresholdSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public abstract float CurrentValue
	{
		get;
	}

	public float Threshold
	{
		get
		{
			return threshold;
		}
		set
		{
			threshold = value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return activateAboveThreshold;
		}
		set
		{
			activateAboveThreshold = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		ConduitThresholdSensor component = gameObject.GetComponent<ConduitThresholdSensor>();
		if ((Object)component != (Object)null)
		{
			Threshold = component.Threshold;
			ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
	}

	protected override void ConduitUpdate(float dt)
	{
		float containedMass = GetContainedMass();
		if (!(containedMass <= 0f))
		{
			float currentValue = CurrentValue;
			if (activateAboveThreshold)
			{
				if ((currentValue > threshold && !base.IsSwitchedOn) || (currentValue <= threshold && base.IsSwitchedOn))
				{
					Toggle();
				}
			}
			else if ((currentValue > threshold && base.IsSwitchedOn) || (currentValue <= threshold && !base.IsSwitchedOn))
			{
				Toggle();
			}
		}
	}

	private float GetContainedMass()
	{
		int cell = Grid.PosToCell(this);
		ConduitFlow flowManager = Conduit.GetFlowManager(conduitType);
		ConduitFlow.ConduitContents contents = flowManager.GetContents(cell);
		return contents.mass;
	}
}
