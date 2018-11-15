using UnityEngine;

public class EmoteMonitor : GameStateMachine<EmoteMonitor, EmoteMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void OnStartChore(object o)
		{
			Chore chore = (Chore)o;
			if (chore.SatisfiesUrge(Db.Get().Urges.Emote))
			{
				GoTo(base.sm.satisfied);
			}
		}
	}

	public State satisfied;

	public State ready;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = true;
		satisfied.ScheduleGoTo((float)Random.Range(30, 90), ready);
		ready.ToggleUrge(Db.Get().Urges.Emote).EventHandler(GameHashes.BeginChore, delegate(Instance smi, object o)
		{
			smi.OnStartChore(o);
		});
	}
}
