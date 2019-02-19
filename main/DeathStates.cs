using STRINGS;
using UnityEngine;

internal class DeathStates : GameStateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>
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
		loop.ToggleStatusItem(CREATURES.STATUSITEMS.DEAD.NAME, CREATURES.STATUSITEMS.DEAD.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 63486, resolve_string_callback: null, resolve_tooltip_callback: null).Enter("EnableGravity", delegate(Instance smi)
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
