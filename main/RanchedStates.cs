using STRINGS;
using System;
using System.Runtime.CompilerServices;

internal class RanchedStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public float originalSpeed;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			originalSpeed = GetComponent<Navigator>().defaultSpeed;
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToGetRanched);
		}

		public RanchStation.Instance GetRanchStation()
		{
			return this.GetSMI<RanchableMonitor.Instance>().targetRanchStation;
		}

		public void AbandonedRanchStation()
		{
			if (GetRanchStation() != null)
			{
				GetRanchStation().Trigger(-364750427, null);
			}
		}
	}

	public class RanchStates : State
	{
		public class CheerStates : State
		{
			public State pre;

			public State cheer;

			public State pst;
		}

		public class MoveStates : State
		{
			public State movetoranch;

			public State getontable;

			public State waitforranchertobeready;
		}

		public CheerStates cheer;

		public MoveStates move;

		public State ranching;
	}

	private RanchStates ranch;

	private State wavegoodbye;

	private State runaway;

	private State behaviourcomplete;

	[CompilerGenerated]
	private static StateMachine<RanchedStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<Instance, int> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static StateMachine<RanchedStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static StateMachine<RanchedStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static StateMachine<RanchedStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Func<Instance, int> _003C_003Ef__mg_0024cache5;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = ranch;
		root.Exit("AbandonedRanchStation", delegate(Instance smi)
		{
			smi.AbandonedRanchStation();
		});
		ranch.EventTransition(GameHashes.RanchStationNoLongerAvailable, (State)null, (Transition.ConditionCallback)null).DefaultState(ranch.cheer).Exit(ClearLayerOverride);
		State state = ranch.cheer.DefaultState(ranch.cheer.pre);
		string name = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME;
		string tooltip = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
		ranch.cheer.pre.ScheduleGoTo(0.9f, ranch.cheer.cheer);
		ranch.cheer.cheer.Enter("FaceRancher", delegate(Instance smi)
		{
			smi.GetComponent<Facing>().Face(smi.GetRanchStation().transform.GetPosition());
		}).PlayAnim("excited_loop").OnAnimQueueComplete(ranch.cheer.pst);
		ranch.cheer.pst.ScheduleGoTo(0.2f, ranch.move);
		State state2 = ranch.move.DefaultState(ranch.move.movetoranch);
		tooltip = CREATURES.STATUSITEMS.GETTING_RANCHED.NAME;
		name = CREATURES.STATUSITEMS.GETTING_RANCHED.TOOLTIP;
		main = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(tooltip, name, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
		ranch.move.movetoranch.Enter("Speedup", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed * 1.25f;
		}).MoveTo(GetTargetRanchCell, ranch.move.getontable, null, false).Exit("RestoreSpeed", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed;
		});
		ranch.move.getontable.Enter(GetOnTable).OnAnimQueueComplete(ranch.move.waitforranchertobeready);
		ranch.move.waitforranchertobeready.Enter("SetCreatureAtRanchingStation", delegate(Instance smi)
		{
			smi.GetRanchStation().Trigger(-1357116271, null);
		}).EventTransition(GameHashes.RancherReadyAtRanchStation, ranch.ranching, null);
		State state3 = ranch.ranching.Enter(PlayGroomingLoopAnim).EventTransition(GameHashes.RanchingComplete, wavegoodbye, null);
		name = CREATURES.STATUSITEMS.GETTING_RANCHED.NAME;
		tooltip = CREATURES.STATUSITEMS.GETTING_RANCHED.TOOLTIP;
		main = Db.Get().StatusItemCategories.Main;
		state3.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
		State state4 = wavegoodbye.Enter(PlayGroomingPstAnim).OnAnimQueueComplete(runaway);
		tooltip = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME;
		name = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP;
		main = Db.Get().StatusItemCategories.Main;
		state4.ToggleStatusItem(tooltip, name, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
		State state5 = runaway.MoveTo(GetRunawayCell, behaviourcomplete, behaviourcomplete, false);
		name = CREATURES.STATUSITEMS.IDLE.NAME;
		tooltip = CREATURES.STATUSITEMS.IDLE.TOOLTIP;
		main = Db.Get().StatusItemCategories.Main;
		state5.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToGetRanched, false);
	}

	private static RanchStation.Instance GetRanchStation(Instance smi)
	{
		return smi.GetSMI<RanchableMonitor.Instance>().targetRanchStation;
	}

	private static void ClearLayerOverride(Instance smi)
	{
		smi.Get<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
	}

	private static void GetOnTable(Instance smi)
	{
		Navigator navigator = smi.Get<Navigator>();
		if (navigator.IsValidNavType(NavType.Floor))
		{
			navigator.SetCurrentNavType(NavType.Floor);
		}
		smi.Get<Facing>().SetFacing(false);
		smi.Get<KBatchedAnimController>().Queue(GetRanchStation(smi).def.ranchedPreAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	private static void PlayGroomingLoopAnim(Instance smi)
	{
		smi.Get<KBatchedAnimController>().Queue(GetRanchStation(smi).def.ranchedLoopAnim, KAnim.PlayMode.Loop, 1f, 0f);
	}

	private static void PlayGroomingPstAnim(Instance smi)
	{
		smi.Get<KBatchedAnimController>().Queue(GetRanchStation(smi).def.ranchedPstAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	private static int GetTargetRanchCell(Instance smi)
	{
		RanchStation.Instance ranchStation = GetRanchStation(smi);
		return ranchStation.GetTargetRanchCell();
	}

	private static int GetRunawayCell(Instance smi)
	{
		int cell = Grid.PosToCell(smi.transform.GetPosition());
		int num = Grid.OffsetCell(cell, 2, 0);
		if (Grid.Solid[num])
		{
			num = Grid.OffsetCell(cell, -2, 0);
		}
		return num;
	}
}
