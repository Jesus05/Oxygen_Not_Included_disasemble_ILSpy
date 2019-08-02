using System;
using UnityEngine;

public class StressEmoteChore : Chore<StressEmoteChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, StressEmoteChore, object>.GameInstance
	{
		public HashedString[] emoteAnims;

		public HashedString emoteKAnim;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;

		public StatesInstance(StressEmoteChore master, GameObject emoter, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode mode)
			: base(master)
		{
			emoteKAnim = emote_kanim;
			emoteAnims = emote_anims;
			this.mode = mode;
			base.sm.emoter.Set(emoter, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, StressEmoteChore>
	{
		public TargetParameter emoter;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			Target(emoter);
			root.ToggleAnims((StatesInstance smi) => smi.emoteKAnim).ToggleThought(Db.Get().Thoughts.Unhappy, null).PlayAnims((StatesInstance smi) => smi.emoteAnims, (StatesInstance smi) => smi.mode)
				.OnAnimQueueComplete(null);
		}
	}

	private Func<StatusItem> getStatusItem;

	public StressEmoteChore(IStateMachineTarget target, ChoreType chore_type, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode play_mode, Func<StatusItem> get_status_item)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		AddPrecondition(ChorePreconditions.instance.IsMoving, null);
		AddPrecondition(ChorePreconditions.instance.IsOffLadder, null);
		AddPrecondition(ChorePreconditions.instance.NotInTube, null);
		AddPrecondition(ChorePreconditions.instance.IsAwake, null);
		getStatusItem = get_status_item;
		base.smi = new StatesInstance(this, target.gameObject, emote_kanim, emote_anims, play_mode);
	}

	protected override StatusItem GetStatusItem()
	{
		return (getStatusItem == null) ? base.GetStatusItem() : getStatusItem();
	}

	public override string ToString()
	{
		if (base.smi.emoteKAnim.IsValid)
		{
			return "StressEmoteChore<" + base.smi.emoteKAnim + ">";
		}
		return "StressEmoteChore<" + base.smi.emoteAnims[0] + ">";
	}
}
