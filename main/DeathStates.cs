using STRINGS;
using UnityEngine;

public class DeathStates : GameStateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Die);
		}

		public void EnableGravityIfNecessary()
		{
			if (HasTag(GameTags.Creatures.Flyer))
			{
				GameComps.Gravities.Add(base.smi.gameObject, Vector2.zero, null);
			}
		}

		public void DisableGravity()
		{
			if (GameComps.Gravities.Has(base.smi.gameObject))
			{
				GameComps.Gravities.Remove(base.smi.gameObject);
			}
		}
	}

	private State loop;

	private State pst;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = loop;
		State state = loop;
		string name = CREATURES.STATUSITEMS.DEAD.NAME;
		string tooltip = CREATURES.STATUSITEMS.DEAD.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main).Enter("EnableGravity", delegate(Instance smi)
		{
			smi.EnableGravityIfNecessary();
		}).PlayAnim("Death")
			.OnAnimQueueComplete(pst)
			.Exit("DisableGravity", delegate(Instance smi)
			{
				smi.DisableGravity();
			});
		pst.TriggerOnEnter(GameHashes.DeathAnimComplete, null).Enter("Butcher", delegate(Instance smi)
		{
			if ((Object)smi.gameObject.GetComponent<Butcherable>() != (Object)null)
			{
				smi.GetComponent<Butcherable>().OnButcherComplete();
			}
		}).Enter("Destroy", delegate(Instance smi)
		{
			smi.gameObject.DeleteObject();
		})
			.BehaviourComplete(GameTags.Creatures.Die, false);
	}
}
