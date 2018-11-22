using Klei.AI;
using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LightMonitor : GameStateMachine<LightMonitor, LightMonitor.Instance>
{
	public class UnburntStates : State
	{
		public SafeStates safe;

		public State burning;
	}

	public class SafeStates : State
	{
		public State unlit;

		public State normal_light;

		public State sunlight;
	}

	public new class Instance : GameInstance
	{
		public Effects effects;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			effects = GetComponent<Effects>();
		}
	}

	public const float BURN_RESIST_RECOVERY_FACTOR = 0.25f;

	public FloatParameter lightLevel;

	public FloatParameter burnResistance = new FloatParameter(120f);

	public UnburntStates unburnt;

	public State get_burnt;

	public State burnt;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = unburnt;
		root.EventTransition(GameHashes.DiseaseAdded, burnt, (Instance smi) => smi.gameObject.GetDiseases().Has(Db.Get().Diseases.Sunburn)).Update(CheckLightLevel, UpdateRate.SIM_1000ms, false);
		unburnt.DefaultState(unburnt.safe).ParamTransition(burnResistance, get_burnt, GameStateMachine<LightMonitor, Instance, IStateMachineTarget, object>.IsLTEZero);
		unburnt.safe.DefaultState(unburnt.safe.unlit).Update(delegate(Instance smi, float dt)
		{
			smi.sm.burnResistance.DeltaClamp(dt * 0.25f, 0f, 120f, smi);
		}, UpdateRate.SIM_200ms, false);
		unburnt.safe.unlit.ParamTransition(lightLevel, unburnt.safe.normal_light, GameStateMachine<LightMonitor, Instance, IStateMachineTarget, object>.IsGTZero);
		unburnt.safe.normal_light.ParamTransition(lightLevel, unburnt.safe.unlit, GameStateMachine<LightMonitor, Instance, IStateMachineTarget, object>.IsLTEZero).ParamTransition(lightLevel, unburnt.safe.sunlight, (Instance smi, float p) => p >= 40000f);
		unburnt.safe.sunlight.ParamTransition(lightLevel, unburnt.safe.normal_light, (Instance smi, float p) => p < 40000f).ParamTransition(lightLevel, unburnt.burning, (Instance smi, float p) => p >= 71999f).ToggleEffect("Sunlight_Pleasant");
		unburnt.burning.ParamTransition(lightLevel, unburnt.safe.sunlight, (Instance smi, float p) => p < 71999f).Update(delegate(Instance smi, float dt)
		{
			smi.sm.burnResistance.DeltaClamp(0f - dt, 0f, 120f, smi);
		}, UpdateRate.SIM_200ms, false).ToggleEffect("Sunlight_Burning");
		get_burnt.Enter(delegate(Instance smi)
		{
			smi.gameObject.GetDiseases().Infect(new DiseaseExposureInfo(Db.Get().Diseases.Sunburn.Id, DUPLICANTS.DISEASES.SUNBURN.SUNEXPOSURE));
		}).GoTo(burnt);
		burnt.EventTransition(GameHashes.DiseaseCured, unburnt, (Instance smi) => !smi.gameObject.GetDiseases().Has(Db.Get().Diseases.Sunburn)).Exit(delegate(Instance smi)
		{
			smi.sm.burnResistance.Set(120f, smi);
		});
	}

	private static void CheckLightLevel(Instance smi, float dt)
	{
		KPrefabID component = smi.GetComponent<KPrefabID>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.HasTag(GameTags.Shaded))
		{
			smi.sm.lightLevel.Set(0f, smi);
		}
		else
		{
			int num = Grid.PosToCell(smi.gameObject);
			if (Grid.IsValidCell(num))
			{
				smi.sm.lightLevel.Set((float)Grid.LightIntensity[num], smi);
			}
		}
	}
}
