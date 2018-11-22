using UnityEngine;

[SkipSaveFileSerialization]
public class Fashionable : StateMachineComponent<Fashionable.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Fashionable, object>.GameInstance
	{
		public StatesInstance(Fashionable master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Fashionable>
	{
		public State satisfied;

		public State suffering;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = satisfied;
			root.EventHandler(GameHashes.EquippedItemEquipper, delegate(StatesInstance smi)
			{
				if (smi.master.IsUncomfortable())
				{
					smi.GoTo(suffering);
				}
				else
				{
					smi.GoTo(satisfied);
				}
			}).EventHandler(GameHashes.UnequippedItemEquipper, delegate(StatesInstance smi)
			{
				if (smi.master.IsUncomfortable())
				{
					smi.GoTo(suffering);
				}
				else
				{
					smi.GoTo(satisfied);
				}
			});
			suffering.AddEffect("UnfashionableClothing").ToggleExpression(Db.Get().Expressions.Uncomfortable, null);
			satisfied.DoNothing();
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		ClothingWearer component = GetComponent<ClothingWearer>();
		if (!((Object)component != (Object)null))
		{
			return false;
		}
		return component.currentClothing.decorMod <= 0;
	}
}
