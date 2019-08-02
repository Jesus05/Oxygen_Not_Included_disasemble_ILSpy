using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class IlluminationVulnerable : StateMachineComponent<IlluminationVulnerable.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, IlluminationVulnerable, object>.GameInstance
	{
		public bool hasMaturity;

		public StatesInstance(IlluminationVulnerable master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, IlluminationVulnerable>
	{
		public BoolParameter illuminated;

		public State comfortable;

		public State too_dark;

		public State too_bright;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = comfortable;
			root.Update("Illumination", delegate(StatesInstance smi, float dt)
			{
				smi.master.GetAmounts().Get(Db.Get().Amounts.Illumination).SetValue((float)Grid.LightCount[Grid.PosToCell(smi.master.gameObject)]);
			}, UpdateRate.SIM_1000ms, false);
			comfortable.Update("Illumination.Comfortable", delegate(StatesInstance smi, float dt)
			{
				int cell3 = Grid.PosToCell(smi.master.gameObject);
				if (!smi.master.IsCellSafe(cell3))
				{
					State state = (!smi.master.prefersDarkness) ? too_dark : too_bright;
					smi.GoTo(state);
				}
			}, UpdateRate.SIM_1000ms, false).Enter(delegate(StatesInstance smi)
			{
				smi.master.Trigger(1113102781, null);
			});
			too_dark.TriggerOnEnter(GameHashes.IlluminationDiscomfort, null).Update("Illumination.too_dark", delegate(StatesInstance smi, float dt)
			{
				int cell2 = Grid.PosToCell(smi.master.gameObject);
				if (smi.master.IsCellSafe(cell2))
				{
					smi.GoTo(comfortable);
				}
			}, UpdateRate.SIM_1000ms, false);
			too_bright.TriggerOnEnter(GameHashes.IlluminationDiscomfort, null).Update("Illumination.too_bright", delegate(StatesInstance smi, float dt)
			{
				int cell = Grid.PosToCell(smi.master.gameObject);
				if (smi.master.IsCellSafe(cell))
				{
					smi.GoTo(comfortable);
				}
			}, UpdateRate.SIM_1000ms, false);
		}
	}

	public float lightIntensityThreshold;

	private OccupyArea _occupyArea;

	private SchedulerHandle handle;

	public bool prefersDarkness;

	WiltCondition.Condition[] IWiltCause.Conditions
	{
		get
		{
			return new WiltCondition.Condition[2]
			{
				WiltCondition.Condition.Darkness,
				WiltCondition.Condition.IlluminationComfort
			};
		}
	}

	private OccupyArea occupyArea
	{
		get
		{
			if ((Object)_occupyArea == (Object)null)
			{
				_occupyArea = GetComponent<OccupyArea>();
			}
			return _occupyArea;
		}
	}

	public string WiltStateString
	{
		get
		{
			if (base.smi.IsInsideState(base.smi.sm.too_bright))
			{
				return Db.Get().CreatureStatusItems.Crop_Too_Bright.resolveStringCallback(CREATURES.STATUSITEMS.CROP_TOO_BRIGHT.NAME, this);
			}
			if (base.smi.IsInsideState(base.smi.sm.too_dark))
			{
				return Db.Get().CreatureStatusItems.Crop_Too_Dark.resolveStringCallback(CREATURES.STATUSITEMS.CROP_TOO_DARK.NAME, this);
			}
			return string.Empty;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.GetAmounts().Add(new AmountInstance(Db.Get().Amounts.Illumination, base.gameObject));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void SetPrefersDarkness(bool prefersDarkness = false)
	{
		this.prefersDarkness = prefersDarkness;
	}

	protected override void OnCleanUp()
	{
		handle.ClearScheduler();
		base.OnCleanUp();
	}

	public bool IsCellSafe(int cell)
	{
		if (prefersDarkness)
		{
			return (float)Grid.LightIntensity[cell] <= lightIntensityThreshold;
		}
		return (float)Grid.LightIntensity[cell] > lightIntensityThreshold;
	}

	public bool IsComfortable()
	{
		return base.smi.IsInsideState(base.smi.sm.comfortable);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list;
		if (prefersDarkness)
		{
			list = new List<Descriptor>();
			list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_DARKNESS, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_DARKNESS, Descriptor.DescriptorType.Requirement, false));
			return list;
		}
		list = new List<Descriptor>();
		list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_LIGHT, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_LIGHT, Descriptor.DescriptorType.Requirement, false));
		return list;
	}
}
