using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GasBreatherFromWorldProvider : OxygenBreather.IGasProvider
{
	private SuffocationMonitor.Instance suffocationMonitor;

	private SafeCellMonitor.Instance safeCellMonitor;

	private OxygenBreather oxygenBreather;

	[CompilerGenerated]
	private static Action<Sim.MassConsumedCallback, object> _003C_003Ef__mg_0024cache0;

	public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
	{
		suffocationMonitor = new SuffocationMonitor.Instance(oxygen_breather);
		suffocationMonitor.StartSM();
		safeCellMonitor = new SafeCellMonitor.Instance(oxygen_breather);
		safeCellMonitor.StartSM();
		oxygenBreather = oxygen_breather;
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
		suffocationMonitor.StopSM("Removed gas provider");
		safeCellMonitor.StopSM("Removed gas provider");
	}

	public bool ShouldEmitCO2()
	{
		return true;
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float gas_consumed)
	{
		SimHashes getBreathableElement = oxygen_breather.GetBreathableElement;
		if (getBreathableElement != SimHashes.Vacuum)
		{
			HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(OnSimConsumeCallback, this, "GasBreatherFromWorldProvider");
			SimMessages.ConsumeMass(oxygen_breather.mouthCell, getBreathableElement, gas_consumed, 3, handle.index);
			return true;
		}
		return false;
	}

	private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data)
	{
		((GasBreatherFromWorldProvider)data).OnSimConsume(mass_cb_info);
	}

	private void OnSimConsume(Sim.MassConsumedCallback mass_cb_info)
	{
		if (!((UnityEngine.Object)oxygenBreather == (UnityEngine.Object)null) && !oxygenBreather.GetComponent<KPrefabID>().HasTag(GameTags.Dead))
		{
			Game.Instance.accumulators.Accumulate(oxygenBreather.O2Accumulator, mass_cb_info.mass);
			float value = 0f - mass_cb_info.mass;
			ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, value, oxygenBreather.GetProperName(), null);
			oxygenBreather.Consume(mass_cb_info);
		}
	}
}
