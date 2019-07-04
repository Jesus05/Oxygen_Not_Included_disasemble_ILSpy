using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EggProtectionMonitor : GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>
{
	public class Def : BaseDef
	{
		public Tag[] allyTags;
	}

	public class GuardEggStates : State
	{
		public State safe;

		public State threatened;
	}

	public new class Instance : GameInstance
	{
		public GameObject eggToProtect;

		public FactionAlignment alignment;

		private Navigator navigator;

		private GameObject mainThreat;

		private List<FactionAlignment> threats = new List<FactionAlignment>();

		private int maxThreatDistance = 12;

		private Action<object> refreshThreatDelegate;

		public GameObject MainThreat => mainThreat;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			alignment = master.GetComponent<FactionAlignment>();
			navigator = master.GetComponent<Navigator>();
			refreshThreatDelegate = RefreshThreat;
		}

		public void CanProtectEgg()
		{
			bool flag = true;
			if ((UnityEngine.Object)eggToProtect == (UnityEngine.Object)null)
			{
				flag = false;
			}
			if (flag)
			{
				int num = 150;
				int navigationCost = navigator.GetNavigationCost(Grid.PosToCell(eggToProtect));
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
			GameObject eggToGuard = null;
			int num = 100;
			IEnumerator enumerator = Components.Pickupables.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Pickupable pickupable = (Pickupable)enumerator.Current;
					if (pickupable.HasTag("CrabEgg".ToTag()) && !(Vector2.Distance(base.smi.transform.position, pickupable.transform.position) > 25f))
					{
						int navigationCost = navigator.GetNavigationCost(Grid.PosToCell(pickupable));
						if (navigationCost != -1 && navigationCost < num)
						{
							eggToGuard = pickupable.gameObject;
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
			SetEggToGuard(eggToGuard);
		}

		public void SetEggToGuard(GameObject egg)
		{
			eggToProtect = egg;
			base.sm.hasEggToGuard.Set((UnityEngine.Object)egg != (UnityEngine.Object)null, base.smi);
		}

		public void SetMainThreat(GameObject threat)
		{
			if (!((UnityEngine.Object)threat == (UnityEngine.Object)mainThreat))
			{
				if ((UnityEngine.Object)mainThreat != (UnityEngine.Object)null)
				{
					mainThreat.Unsubscribe(1623392196, refreshThreatDelegate);
					mainThreat.Unsubscribe(1969584890, refreshThreatDelegate);
					if ((UnityEngine.Object)threat == (UnityEngine.Object)null)
					{
						Trigger(2144432245, null);
					}
				}
				if ((UnityEngine.Object)mainThreat != (UnityEngine.Object)null)
				{
					mainThreat.Unsubscribe(1623392196, refreshThreatDelegate);
					mainThreat.Unsubscribe(1969584890, refreshThreatDelegate);
				}
				mainThreat = threat;
				if ((UnityEngine.Object)mainThreat != (UnityEngine.Object)null)
				{
					mainThreat.Subscribe(1623392196, refreshThreatDelegate);
					mainThreat.Subscribe(1969584890, refreshThreatDelegate);
				}
			}
		}

		public void Cleanup(object data)
		{
			if ((bool)mainThreat)
			{
				mainThreat.Unsubscribe(1623392196, refreshThreatDelegate);
				mainThreat.Unsubscribe(1969584890, refreshThreatDelegate);
			}
		}

		public void GoToThreatened()
		{
			base.smi.GoTo(base.sm.guard.threatened);
		}

		public void RefreshThreat(object data)
		{
			if (IsRunning() && !((UnityEngine.Object)eggToProtect == (UnityEngine.Object)null))
			{
				if (base.smi.CheckForThreats())
				{
					GoToThreatened();
				}
				else if (base.smi.GetCurrentState() != base.sm.guard.safe)
				{
					Trigger(-21431934, null);
					base.smi.GoTo(base.sm.guard.safe);
				}
			}
		}

		public bool CheckForThreats()
		{
			if (!((UnityEngine.Object)eggToProtect == (UnityEngine.Object)null))
			{
				GameObject x = FindThreat();
				SetMainThreat(x);
				return (UnityEngine.Object)x != (UnityEngine.Object)null;
			}
			return false;
		}

		public GameObject FindThreat()
		{
			threats.Clear();
			ListPool<ScenePartitionerEntry, ThreatMonitor>.PooledList pooledList = ListPool<ScenePartitionerEntry, ThreatMonitor>.Allocate();
			Extents extents = new Extents(Grid.PosToCell(eggToProtect), maxThreatDistance);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.attackableEntitiesLayer, pooledList);
			for (int i = 0; i < pooledList.Count; i++)
			{
				ScenePartitionerEntry scenePartitionerEntry = pooledList[i];
				FactionAlignment factionAlignment = scenePartitionerEntry.obj as FactionAlignment;
				if (!((UnityEngine.Object)factionAlignment.transform == (UnityEngine.Object)null) && !((UnityEngine.Object)factionAlignment == (UnityEngine.Object)alignment) && factionAlignment.IsAlignmentActive() && navigator.CanReach(factionAlignment.attackable))
				{
					bool flag = false;
					Tag[] allyTags = base.def.allyTags;
					foreach (Tag tag in allyTags)
					{
						if (factionAlignment.HasTag(tag))
						{
							flag = true;
						}
					}
					if (!flag)
					{
						threats.Add(factionAlignment);
					}
				}
			}
			pooledList.Recycle();
			return PickBestTarget(threats);
		}

		public GameObject PickBestTarget(List<FactionAlignment> threats)
		{
			float num = 1f;
			Vector2 a = base.gameObject.transform.GetPosition();
			GameObject result = null;
			float num2 = float.PositiveInfinity;
			for (int num3 = threats.Count - 1; num3 >= 0; num3--)
			{
				FactionAlignment factionAlignment = threats[num3];
				float num4 = Vector2.Distance(a, factionAlignment.transform.GetPosition()) / num;
				if (num4 < num2)
				{
					num2 = num4;
					result = factionAlignment.gameObject;
				}
			}
			return result;
		}
	}

	public BoolParameter hasEggToGuard;

	public State find_egg;

	public GuardEggStates guard;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = find_egg;
		root.EventHandler(GameHashes.ObjectDestroyed, delegate(Instance smi, object d)
		{
			smi.Cleanup(d);
		});
		find_egg.Enter(delegate(Instance smi)
		{
			smi.FindEggToGuard();
		}).Update("find_egg", delegate(Instance smi, float dt)
		{
			smi.FindEggToGuard();
		}, UpdateRate.SIM_1000ms, true).ParamTransition(hasEggToGuard, guard.safe, GameStateMachine<EggProtectionMonitor, Instance, IStateMachineTarget, Def>.IsTrue);
		guard.Enter(delegate(Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim("pincher_kanim"), null, "_heat", 0);
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Hostile);
		}).Exit(delegate(Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().RemoveBuildOverride(Assets.GetAnim("pincher_kanim").GetData(), 0);
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Pest);
		}).Update("evaulate_egg", delegate(Instance smi, float dt)
		{
			smi.CanProtectEgg();
		}, UpdateRate.SIM_1000ms, true)
			.ParamTransition(hasEggToGuard, find_egg, GameStateMachine<EggProtectionMonitor, Instance, IStateMachineTarget, Def>.IsFalse);
		guard.safe.Enter(delegate(Instance smi)
		{
			smi.RefreshThreat(null);
		}).Update("safe", delegate(Instance smi, float dt)
		{
			smi.RefreshThreat(null);
		}, UpdateRate.SIM_200ms, true);
		guard.threatened.ToggleBehaviour(GameTags.Creatures.Defend, (Instance smi) => (UnityEngine.Object)smi.MainThreat != (UnityEngine.Object)null, delegate(Instance smi)
		{
			smi.GoTo(guard.safe);
		}).Update("Threatened", CritterUpdateThreats, UpdateRate.SIM_200ms, false);
	}

	private static void CritterUpdateThreats(Instance smi, float dt)
	{
		if (!smi.isMasterNull && !smi.CheckForThreats())
		{
			smi.GoTo(smi.sm.guard.safe);
		}
	}
}
