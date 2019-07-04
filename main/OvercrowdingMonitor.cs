using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class OvercrowdingMonitor : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>
{
	public class Def : BaseDef
	{
		public int spaceRequiredPerCreature;
	}

	public new class Instance : GameInstance
	{
		public CavityInfo cavity;

		public bool isBaby;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			BabyMonitor.Def def2 = master.gameObject.GetDef<BabyMonitor.Def>();
			isBaby = (def2 != null);
		}

		protected override void OnCleanUp()
		{
			KPrefabID component = base.master.GetComponent<KPrefabID>();
			if (cavity != null)
			{
				GetCreatureCollection(this, cavity).Remove(component);
			}
		}
	}

	public const float OVERCROWDED_FERTILITY_DEBUFF = -1f;

	public static Effect futureOvercrowdedEffect;

	public static Effect overcrowdedEffect;

	public static Effect stuckEffect;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.Update(UpdateState, UpdateRate.SIM_1000ms, true);
		futureOvercrowdedEffect = new Effect("FutureOvercrowded", CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, CREATURES.MODIFIERS.FUTURE_OVERCROWDED.TOOLTIP, 0f, true, false, true, null, 0f, null);
		futureOvercrowdedEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, true, false, true));
		overcrowdedEffect = new Effect("Overcrowded", CREATURES.MODIFIERS.OVERCROWDED.NAME, CREATURES.MODIFIERS.OVERCROWDED.TOOLTIP, 0f, true, false, true, null, 0f, null);
		overcrowdedEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -5f, CREATURES.MODIFIERS.OVERCROWDED.NAME, false, false, true));
		stuckEffect = new Effect("Confined", CREATURES.MODIFIERS.CONFINED.NAME, CREATURES.MODIFIERS.CONFINED.TOOLTIP, 0f, true, false, true, null, 0f, null);
		stuckEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, CREATURES.MODIFIERS.CONFINED.NAME, false, false, true));
	}

	private static bool IsConfined(Instance smi)
	{
		if (!smi.HasTag(GameTags.Creatures.Burrowed))
		{
			if (!smi.HasTag(GameTags.Creatures.Digger))
			{
				if (smi.cavity != null)
				{
					if (smi.cavity.numCells >= smi.def.spaceRequiredPerCreature)
					{
						return false;
					}
					return true;
				}
				return true;
			}
			return false;
		}
		return false;
	}

	private static bool IsFutureOvercrowded(Instance smi)
	{
		if (smi.def.spaceRequiredPerCreature != 0)
		{
			if (smi.cavity == null)
			{
				return false;
			}
			int num = smi.cavity.creatures.Count + smi.cavity.eggs.Count;
			if (num != 0 && smi.cavity.eggs.Count != 0)
			{
				int num2 = smi.cavity.numCells / num;
				return num2 < smi.def.spaceRequiredPerCreature;
			}
			return false;
		}
		return false;
	}

	private static bool IsOvercrowded(Instance smi)
	{
		if (smi.def.spaceRequiredPerCreature != 0)
		{
			FishOvercrowdingMonitor.Instance sMI = smi.GetSMI<FishOvercrowdingMonitor.Instance>();
			if (sMI == null)
			{
				if (smi.cavity != null && smi.cavity.creatures.Count > 1)
				{
					int num = smi.cavity.numCells / smi.cavity.creatures.Count;
					return num < smi.def.spaceRequiredPerCreature;
				}
				return false;
			}
			int fishCount = sMI.fishCount;
			if (fishCount <= 0)
			{
				return false;
			}
			int cellCount = sMI.cellCount;
			int num2 = cellCount / fishCount;
			return num2 < smi.def.spaceRequiredPerCreature;
		}
		return false;
	}

	private static void UpdateState(Instance smi, float dt)
	{
		UpdateCavity(smi, dt);
		bool flag = IsConfined(smi);
		bool flag2 = IsOvercrowded(smi);
		bool flag3 = !smi.isBaby && IsFutureOvercrowded(smi);
		KPrefabID component = smi.gameObject.GetComponent<KPrefabID>();
		component.SetTag(GameTags.Creatures.Confined, flag);
		component.SetTag(GameTags.Creatures.Overcrowded, flag2);
		component.SetTag(GameTags.Creatures.Expecting, flag3);
		SetEffect(smi, stuckEffect, flag);
		SetEffect(smi, overcrowdedEffect, !flag && flag2);
		SetEffect(smi, futureOvercrowdedEffect, !flag && flag3);
	}

	private static void SetEffect(Instance smi, Effect effect, bool set)
	{
		Effects component = smi.GetComponent<Effects>();
		if (set)
		{
			component.Add(effect, false);
		}
		else
		{
			component.Remove(effect);
		}
	}

	private static List<KPrefabID> GetCreatureCollection(Instance smi, CavityInfo cavity_info)
	{
		if (!smi.HasTag(GameTags.Egg))
		{
			return cavity_info.creatures;
		}
		return cavity_info.eggs;
	}

	private static void UpdateCavity(Instance smi, float dt)
	{
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi));
		if (cavityForCell != smi.cavity)
		{
			KPrefabID component = smi.GetComponent<KPrefabID>();
			if (smi.cavity != null)
			{
				GetCreatureCollection(smi, smi.cavity).Remove(component);
			}
			smi.cavity = cavityForCell;
			if (smi.cavity != null)
			{
				GetCreatureCollection(smi, smi.cavity).Add(component);
			}
		}
	}
}
