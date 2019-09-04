using System;
using System.Diagnostics;

[DebuggerDisplay("{name}")]
public class PowerTransformer : Generator
{
	private Battery battery;

	private bool mLoopDetected;

	private static readonly EventSystem.IntraObjectHandler<PowerTransformer> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<PowerTransformer>(delegate(PowerTransformer component, object data)
	{
		component.OnOperationalChanged(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		battery = GetComponent<Battery>();
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	public override void ApplyDeltaJoules(float joules_delta, bool can_over_power = false)
	{
		battery.ConsumeEnergy(0f - joules_delta);
		base.ApplyDeltaJoules(joules_delta, can_over_power);
	}

	public override void ConsumeEnergy(float joules)
	{
		battery.ConsumeEnergy(joules);
		base.ConsumeEnergy(joules);
	}

	private void OnOperationalChanged(object data)
	{
		if (!(bool)data)
		{
			battery.joulesLostPerSecond = 3.33333325f;
			ResetJoules();
		}
		else
		{
			battery.joulesLostPerSecond = 0f;
		}
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		if (operational.IsOperational)
		{
			AssignJoulesAvailable(Math.Min(battery.JoulesAvailable, base.WattageRating * dt));
		}
		ushort circuitID = battery.CircuitID;
		ushort circuitID2 = base.CircuitID;
		bool flag = circuitID == circuitID2 && circuitID != 65535;
		if (mLoopDetected != flag)
		{
			mLoopDetected = flag;
			selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.PowerLoopDetected, mLoopDetected, this);
		}
	}
}
