using System;
using System.Diagnostics;

[DebuggerDisplay("{name}")]
public class PowerTransformer : Generator
{
	private Battery battery;

	private static readonly EventSystem.IntraObjectHandler<PowerTransformer> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<PowerTransformer>(delegate(PowerTransformer component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public override float JoulesAvailable => Math.Min(battery.JoulesAvailable, base.WattageRating * 0.2f);

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

	private void OnOperationalChanged(object data)
	{
		if (!(bool)data)
		{
			battery.ConsumeEnergy(3.40282347E+38f);
			ResetJoules();
		}
	}
}
