using System;
using System.Collections;
using UnityEngine;

public class EggProtector : GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>
{
	public class Def : BaseDef
	{
		public Tag protectorTag;

		public bool shouldProtect;

		public Def(Tag tag, bool shouldProtect)
		{
			protectorTag = tag;
			this.shouldProtect = shouldProtect;
		}
	}

	public new class Instance : GameInstance
	{
		public GameObject eggToGuard;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			EntityThreatMonitor.Instance sMI = base.gameObject.GetSMI<EntityThreatMonitor.Instance>();
			sMI.allyTag = def.protectorTag;
		}

		public void CheckDistanceToEgg()
		{
			Navigator component = base.smi.GetComponent<Navigator>();
			int navigationCost = component.GetNavigationCost(Grid.PosToCell(eggToGuard));
			if (navigationCost > 20)
			{
				base.sm.needsToMoveCloser.Set(true, base.smi);
			}
			else if (navigationCost < 0)
			{
				base.sm.needsToMoveCloser.Set(false, base.smi);
			}
		}

		public void CanProtectEgg()
		{
			bool flag = true;
			if ((UnityEngine.Object)eggToGuard == (UnityEngine.Object)null)
			{
				flag = false;
			}
			Navigator component = base.smi.GetComponent<Navigator>();
			if (flag)
			{
				int num = 150;
				int navigationCost = component.GetNavigationCost(Grid.PosToCell(eggToGuard));
				if (navigationCost == -1 || navigationCost >= num)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				SetEggToGuard(null);
			}
		}

		public void FindEggToGuard()
		{
			if (base.def.shouldProtect)
			{
				GameObject gameObject = null;
				int num = 100;
				Navigator component = base.smi.GetComponent<Navigator>();
				IEnumerator enumerator = Components.Pickupables.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Pickupable pickupable = (Pickupable)enumerator.Current;
						if (pickupable.HasTag("CrabEgg".ToTag()) && !(Vector2.Distance(base.smi.transform.position, pickupable.transform.position) > 25f))
						{
							int navigationCost = component.GetNavigationCost(Grid.PosToCell(pickupable));
							if (navigationCost != -1 && navigationCost < num)
							{
								gameObject = pickupable.gameObject;
								num = navigationCost;
							}
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				SetEggToGuard(gameObject);
			}
		}

		public void SetEggToGuard(GameObject egg)
		{
			eggToGuard = egg;
			EntityThreatMonitor.Instance sMI = base.gameObject.GetSMI<EntityThreatMonitor.Instance>();
			sMI.entityToProtect = egg;
			base.sm.hasEggToGuard.Set((UnityEngine.Object)egg != (UnityEngine.Object)null, base.smi);
		}

		public int GetEggPos()
		{
			return Grid.PosToCell(eggToGuard);
		}
	}

	public class GuardingStates : State
	{
		public State idle;

		public State return_to_egg;
	}

	public BoolParameter needsToMoveCloser;

	public BoolParameter hasEggToGuard;

	public State idle;

	public GuardingStates guarding;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.ParamTransition(hasEggToGuard, guarding.idle, GameStateMachine<EggProtector, Instance, IStateMachineTarget, Def>.IsTrue).EventHandler(GameHashes.LayEgg, delegate(Instance smi)
		{
			smi.FindEggToGuard();
		}).Update(delegate(Instance smi, float dt)
		{
			smi.FindEggToGuard();
		}, UpdateRate.SIM_4000ms, false);
		guarding.Enter(delegate(Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim("pincher_kanim"), null, "_heat", 0);
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Hostile);
		}).Exit(delegate(Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().RemoveBuildOverride(Assets.GetAnim("pincher_kanim").GetData(), 0);
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Pest);
		}).ParamTransition(hasEggToGuard, idle, GameStateMachine<EggProtector, Instance, IStateMachineTarget, Def>.IsFalse)
			.Update(delegate(Instance smi, float dt)
			{
				smi.CanProtectEgg();
			}, UpdateRate.SIM_1000ms, false);
		guarding.idle.ParamTransition(needsToMoveCloser, guarding.return_to_egg, GameStateMachine<EggProtector, Instance, IStateMachineTarget, Def>.IsTrue);
		guarding.return_to_egg.MoveTo((Instance smi) => smi.GetEggPos(), null, null, true).ParamTransition(needsToMoveCloser, guarding.idle, GameStateMachine<EggProtector, Instance, IStateMachineTarget, Def>.IsFalse);
	}
}
