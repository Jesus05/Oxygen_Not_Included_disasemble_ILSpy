using Klei;
using Klei.AI;
using System;
using TUNING;

public class WaterCoolerChore : Chore<WaterCoolerChore.StatesInstance>, IWorkerPrioritizable
{
	public class States : GameStateMachine<States, StatesInstance, WaterCoolerChore>
	{
		public class DrinkStates : State
		{
			public State drink;

			public State post;
		}

		public TargetParameter drinker;

		public TargetParameter chitchatlocator;

		public ApproachSubState<WaterCooler> drink_move;

		public DrinkStates drink;

		public ApproachSubState<IApproachable> chat_move;

		public State chat;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = drink_move;
			Target(drinker);
			drink_move.InitializeStates(drinker, masterTarget, drink, null, null, null);
			drink.ToggleAnims("anim_interacts_watercooler_kanim", 0f).DefaultState(drink.drink);
			drink.drink.Face(masterTarget, 0.5f).PlayAnim("working_pre").QueueAnim("working_loop", false, null)
				.OnAnimQueueComplete(drink.post);
			drink.post.Enter("Drink", Drink).PlayAnim("working_pst").OnAnimQueueComplete(chat_move);
			chat_move.InitializeStates(drinker, chitchatlocator, chat, null, null, null);
			chat.ToggleWork<SocialGatheringPointWorkable>(chitchatlocator, success, null, null);
			success.ReturnSuccess();
		}

		private void Drink(StatesInstance smi)
		{
			Storage storage = masterTarget.Get<Storage>(smi);
			Worker worker = stateTarget.Get<Worker>(smi);
			storage.ConsumeAndGetDisease(GameTags.Water, 1f, out SimUtil.DiseaseInfo disease_info, out float _);
			worker.GetSMI<ImmuneSystemMonitor.Instance>()?.TryInjectDisease(disease_info.idx, disease_info.count, GameTags.Water, Disease.InfectionVector.Digestion);
			Effects component = worker.GetComponent<Effects>();
			if (!string.IsNullOrEmpty(smi.master.trackingEffect))
			{
				component.Add(smi.master.trackingEffect, true);
			}
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, WaterCoolerChore, object>.GameInstance
	{
		public StatesInstance(WaterCoolerChore master)
			: base(master)
		{
		}
	}

	public int basePriority = RELAXATION.PRIORITY.TIER2;

	public string specificEffect = "Socialized";

	public string trackingEffect = "RecentlySocialized";

	public WaterCoolerChore(IStateMachineTarget master, Workable chat_workable, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null)
		: base(Db.Get().ChoreTypes.Relax, master, master.GetComponent<ChoreProvider>(), true, on_complete, on_begin, on_end, PriorityScreen.PriorityClass.high, 0, false, true, 0, (Tag[])null)
	{
		smi = new StatesInstance(this);
		smi.sm.chitchatlocator.Set(chat_workable, smi);
		AddPrecondition(ChorePreconditions.instance.CanMoveTo, chat_workable);
		AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Recreation);
		AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, this);
	}

	public override void Begin(Precondition.Context context)
	{
		smi.sm.drinker.Set(context.consumerState.gameObject, smi);
		base.Begin(context);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(trackingEffect) && component.HasEffect(trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(specificEffect) && component.HasEffect(specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
