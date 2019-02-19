using System;
using UnityEngine;

public class ReactEmoteChore : Chore<ReactEmoteChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, ReactEmoteChore, object>.GameInstance
	{
		public HashedString[] emoteAnims;

		public HashedString emoteKAnim;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;

		public StatesInstance(ReactEmoteChore master, GameObject emoter, EmoteReactable reactable, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode mode)
			: base(master)
		{
			emoteKAnim = emote_kanim;
			emoteAnims = emote_anims;
			this.mode = mode;
			base.sm.reactable.Set(reactable, base.smi);
			base.sm.emoter.Set(emoter, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, ReactEmoteChore>
	{
		public TargetParameter emoter;

		public ObjectParameter<EmoteReactable> reactable;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			Target(emoter);
			root.ToggleThought((StatesInstance smi) => reactable.Get(smi).thought).ToggleExpression((StatesInstance smi) => reactable.Get(smi).expression).ToggleAnims((StatesInstance smi) => smi.emoteKAnim)
				.ToggleThought(Db.Get().Thoughts.Unhappy, null)
				.PlayAnims((StatesInstance smi) => smi.emoteAnims, (StatesInstance smi) => smi.mode)
				.OnAnimQueueComplete(null)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.GetComponent<Facing>().Face(Grid.CellToPos(reactable.Get(smi).sourceCell));
				});
		}
	}

	private Func<StatusItem> getStatusItem;

	public ReactEmoteChore(IStateMachineTarget target, ChoreType chore_type, EmoteReactable reactable, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode play_mode, Func<StatusItem> get_status_item)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, (Tag[])null, false, ReportManager.ReportType.WorkTime)
	{
		AddPrecondition(ChorePreconditions.instance.IsMoving, null);
		AddPrecondition(ChorePreconditions.instance.IsOffLadder, null);
		AddPrecondition(ChorePreconditions.instance.NotInTube, null);
		AddPrecondition(ChorePreconditions.instance.IsAwake, null);
		getStatusItem = get_status_item;
		base.smi = new StatesInstance(this, target.gameObject, reactable, emote_kanim, emote_anims, play_mode);
	}

	protected override StatusItem GetStatusItem()
	{
		return (getStatusItem == null) ? base.GetStatusItem() : getStatusItem();
	}

	public override string ToString()
	{
		if (base.smi.emoteKAnim.IsValid)
		{
			return "ReactEmoteChore<" + base.smi.emoteKAnim + ">";
		}
		return "ReactEmoteChore<" + base.smi.emoteAnims[0] + ">";
	}
}
