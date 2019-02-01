using System;
using UnityEngine;

public class EntombedChore : Chore<EntombedChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, EntombedChore, object>.GameInstance
	{
		public StatesInstance(EntombedChore master, GameObject entombable)
			: base(master)
		{
			base.sm.entombable.Set(entombable, base.smi);
		}

		public void UpdateFaceEntombed()
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			int num = Grid.CellAbove(cell);
			base.sm.isFaceEntombed.Set(Grid.IsValidCell(num) && Grid.Solid[num], base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, EntombedChore>
	{
		public BoolParameter isFaceEntombed;

		public TargetParameter entombable;

		public State entombedface;

		public State entombedbody;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = entombedbody;
			Target(entombable);
			root.ToggleAnims("anim_emotes_default_kanim", 0f).Update("IsFaceEntombed", delegate(StatesInstance smi, float dt)
			{
				smi.UpdateFaceEntombed();
			}, UpdateRate.SIM_200ms, false).ToggleStatusItem(Db.Get().DuplicantStatusItems.EntombedChore, (object)null);
			entombedface.PlayAnim("entombed_ceiling", KAnim.PlayMode.Loop).ParamTransition(isFaceEntombed, entombedbody, GameStateMachine<States, StatesInstance, EntombedChore, object>.IsFalse);
			entombedbody.PlayAnim("entombed_floor", KAnim.PlayMode.Loop).StopMoving().ParamTransition(isFaceEntombed, entombedface, GameStateMachine<States, StatesInstance, EntombedChore, object>.IsTrue);
		}
	}

	public EntombedChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Entombed, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, (Tag[])null, false)
	{
		smi = new StatesInstance(this, target.gameObject);
	}
}
