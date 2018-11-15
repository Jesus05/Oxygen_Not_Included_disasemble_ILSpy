using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class ConduitThresholdSensor : ConduitSensor
{
	[SerializeField]
	[Serialize]
	protected float threshold;

	[SerializeField]
	[Serialize]
	protected bool activateAboveThreshold = true;

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
