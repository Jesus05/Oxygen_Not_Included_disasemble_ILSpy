using System;
using UnityEngine;

public class RanchStation : GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>
{
	public class Def : BaseDef
	{
		public Func<GameObject, Instance, bool> isCreatureEligibleToBeRanchedCb;

		public Action<GameObject> onRanchCompleteCb;

		public HashedString ranchedPreAnim = "idle_loop";

		public HashedString ranchedLoopAnim = "idle_loop";

		public HashedString ranchedPstAnim = "idle_loop";

		public HashedString rancherInteractAnim = "anim_interacts_rancherstation_kanim";

		public int interactLoopCount = 1;

		public bool synchronizeBuilding;

		public Func<Instance, int> getTargetRanchCell = (Instance smi) => Grid.PosToCell(smi);
	}

	public class OperationalState : State
	{
	}

	public new class Instance : GameInstance
	{
		public RanchableMonitor.Instance targetRanchable
		{
			get;
			private set;
		}

		public bool shouldCreatureGoGetRanched
		{
			get;
			private set;
		}

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public Chore CreateChore()
		{
			return new RancherChore(GetComponent<KPrefabID>());
		}

		public int GetTargetRanchCell()
		{
			return base.def.getTargetRanchCell(this);
		}

		public bool IsCreatureAvailableForRanching()
		{
			if (targetRanchable == null)
			{
				return false;
			}
			int targetRanchCell = GetTargetRanchCell();
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(targetRanchCell);
			return CanRanchableBeRanchedAtRanchStation(targetRanchable, this, cavityForCell, targetRanchCell);
		}

		public void SetRancherIsAvailableForRanching()
		{
			shouldCreatureGoGetRanched = true;
		}

		public void ClearRancherIsAvailableForRanching()
		{
			shouldCreatureGoGetRanched = false;
		}

		private static bool CanRanchableBeRanchedAtRanchStation(RanchableMonitor.Instance ranchable, Instance ranch_station, CavityInfo ranch_cavity_info, int ranch_cell)
		{
			if (ranchable.IsRunning())
			{
				if (ranchable.targetRanchStation != ranch_station && ranchable.targetRanchStation != null && ranchable.targetRanchStation.IsRunning())
				{
					return false;
				}
				if (ranch_station.def.isCreatureEligibleToBeRanchedCb(ranchable.gameObject, ranch_station))
				{
					if (ranchable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<RanchedStates>())
					{
						int cell = Grid.PosToCell(ranchable.transform.GetPosition());
						CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
						if (cavityForCell != null && cavityForCell == ranch_cavity_info)
						{
							int navigationCost = ranchable.GetComponent<Navigator>().GetNavigationCost(ranch_cell);
							if (navigationCost != -1)
							{
								return true;
							}
							return false;
						}
						return false;
					}
					return false;
				}
				return false;
			}
			return false;
		}

		public void FindRanchable()
		{
			int targetRanchCell = GetTargetRanchCell();
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(targetRanchCell);
			if (cavityForCell == null)
			{
				TriggerRanchStationNoLongerAvailable();
			}
			else
			{
				if (this.targetRanchable != null && !CanRanchableBeRanchedAtRanchStation(this.targetRanchable, this, cavityForCell, targetRanchCell))
				{
					TriggerRanchStationNoLongerAvailable();
				}
				if (this.targetRanchable.IsNullOrStopped())
				{
					CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(targetRanchCell);
					RanchableMonitor.Instance targetRanchable = null;
					if (cavityForCell2 != null && cavityForCell2.creatures != null)
					{
						foreach (KPrefabID creature in cavityForCell2.creatures)
						{
							if (!((UnityEngine.Object)creature == (UnityEngine.Object)null))
							{
								RanchableMonitor.Instance sMI = creature.GetSMI<RanchableMonitor.Instance>();
								if (!sMI.IsNullOrStopped() && CanRanchableBeRanchedAtRanchStation(sMI, this, cavityForCell2, targetRanchCell))
								{
									targetRanchable = sMI;
									break;
								}
							}
						}
					}
					this.targetRanchable = targetRanchable;
					if (!this.targetRanchable.IsNullOrStopped())
					{
						this.targetRanchable.targetRanchStation = this;
					}
				}
			}
		}

		public void TriggerRanchStationNoLongerAvailable()
		{
			if (!targetRanchable.IsNullOrStopped())
			{
				targetRanchable.targetRanchStation = null;
				targetRanchable.Trigger(1689625967, null);
				targetRanchable = null;
			}
		}

		public void RanchCreature()
		{
			if (!targetRanchable.IsNullOrStopped())
			{
				base.def.onRanchCompleteCb(targetRanchable.gameObject);
				targetRanchable.Trigger(1827504087, null);
			}
		}
	}

	public State unoperational;

	public OperationalState operational;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = operational;
		unoperational.TagTransition(GameTags.Operational, operational, false);
		operational.TagTransition(GameTags.Operational, unoperational, true).ToggleChore((Instance smi) => smi.CreateChore(), unoperational, unoperational).Update("FindRanachable", delegate(Instance smi, float dt)
		{
			smi.FindRanchable();
		}, UpdateRate.SIM_1000ms, false);
	}
}
