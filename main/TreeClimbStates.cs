using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TreeClimbStates : GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		private Storage storage;

		private static readonly Vector2 VEL_MIN = new Vector2(-1f, 2f);

		private static readonly Vector2 VEL_MAX = new Vector2(1f, 4f);

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToClimbTree);
			storage = GetComponent<Storage>();
		}

		public void Toss(Pickupable pu)
		{
			Pickupable pickupable = pu.Take(Mathf.Min(1f, pu.UnreservedAmount));
			if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
			{
				storage.Store(pickupable.gameObject, true, false, true, false);
				storage.Drop(pickupable.gameObject, true);
				Throw(pickupable.gameObject);
			}
		}

		private void Throw(GameObject ore_go)
		{
			Vector3 position = base.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			int num = Grid.PosToCell(position);
			int num2 = Grid.CellAbove(num);
			Vector2 initial_velocity = default(Vector2);
			if ((Grid.IsValidCell(num) && Grid.Solid[num]) || (Grid.IsValidCell(num2) && Grid.Solid[num2]))
			{
				initial_velocity = Vector2.zero;
			}
			else
			{
				position.y += 0.5f;
				Vector2 vEL_MIN = VEL_MIN;
				float x = vEL_MIN.x;
				Vector2 vEL_MAX = VEL_MAX;
				float x2 = UnityEngine.Random.Range(x, vEL_MAX.x);
				Vector2 vEL_MIN2 = VEL_MIN;
				float y = vEL_MIN2.y;
				Vector2 vEL_MAX2 = VEL_MAX;
				initial_velocity = new Vector2(x2, UnityEngine.Random.Range(y, vEL_MAX2.y));
			}
			ore_go.transform.SetPosition(position);
			if (GameComps.Fallers.Has(ore_go))
			{
				GameComps.Fallers.Remove(ore_go);
			}
			GameComps.Fallers.Add(ore_go, initial_velocity);
		}
	}

	public class ClimbState : State
	{
		public State pre;

		public State loop;

		public State pst;
	}

	public ApproachSubState<Uprootable> moving;

	public ClimbState climbing;

	public State behaviourcomplete;

	public TargetParameter target;

	[CompilerGenerated]
	private static StateMachine<TreeClimbStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static StateMachine<TreeClimbStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Func<Instance, int> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache3;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = moving;
		State state = root.Enter(SetTarget).Enter(delegate(Instance smi)
		{
			if (!ReserveClimbable(smi))
			{
				smi.GoTo(behaviourcomplete);
			}
		}).Exit(UnreserveClimbable);
		string name = CREATURES.STATUSITEMS.RUMMAGINGSEED.NAME;
		string tooltip = CREATURES.STATUSITEMS.RUMMAGINGSEED.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
		moving.MoveTo(GetClimbableCell, climbing, null, false);
		climbing.DefaultState(climbing.pre);
		climbing.pre.PlayAnim("rummage_pre").OnAnimQueueComplete(climbing.loop);
		climbing.loop.QueueAnim("rummage_loop", true, null).ScheduleGoTo(3.5f, climbing.pst).Update(Rummage, UpdateRate.SIM_1000ms, false);
		climbing.pst.QueueAnim("rummage_pst", false, null).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToClimbTree, false);
	}

	private static void SetTarget(Instance smi)
	{
		smi.sm.target.Set(smi.GetSMI<ClimbableTreeMonitor.Instance>().climbTarget, smi);
	}

	private static bool ReserveClimbable(Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && !gameObject.HasTag(GameTags.Creatures.ReservedByCreature))
		{
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
			return true;
		}
		return false;
	}

	private static void UnreserveClimbable(Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void Rummage(Instance smi, float dt)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			BuddingTrunk component = gameObject.GetComponent<BuddingTrunk>();
			if ((bool)component)
			{
				component.ExtractExtraSeed();
			}
			else
			{
				Storage component2 = gameObject.GetComponent<Storage>();
				if ((bool)component2 && component2.items.Count > 0)
				{
					int index = UnityEngine.Random.Range(0, component2.items.Count - 1);
					GameObject gameObject2 = component2.items[index];
					Pickupable pickupable = (!(bool)gameObject2) ? null : gameObject2.GetComponent<Pickupable>();
					if ((bool)pickupable && pickupable.UnreservedAmount > 0.01f)
					{
						smi.Toss(pickupable);
					}
				}
			}
		}
	}

	private static int GetClimbableCell(Instance smi)
	{
		return Grid.PosToCell(smi.sm.target.Get(smi));
	}
}
