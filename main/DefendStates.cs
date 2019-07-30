using STRINGS;

public class DefendStates : GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Defend);
		}
	}

	public class ProtectStates : State
	{
		public ApproachSubState<AttackableBase> moveToThreat;

		public State attackThreat;
	}

	public TargetParameter target;

	public ProtectStates protectEntity;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = protectEntity.moveToThreat;
		root.Enter("SetTarget", delegate(Instance smi)
		{
			target.Set(smi.GetSMI<EggProtectionMonitor.Instance>().MainThreat, smi);
		}).ToggleStatusItem(CREATURES.STATUSITEMS.PROTECTINGENTITY.NAME, CREATURES.STATUSITEMS.PROTECTINGENTITY.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: (NotificationType)0, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 0, resolve_string_callback: null, resolve_tooltip_callback: null);
		protectEntity.moveToThreat.InitializeStates(masterTarget, target, protectEntity.attackThreat, null, new CellOffset[5]
		{
			new CellOffset(0, 0),
			new CellOffset(1, 0),
			new CellOffset(-1, 0),
			new CellOffset(1, 1),
			new CellOffset(-1, 1)
		}, null);
		protectEntity.attackThreat.Enter(delegate(Instance smi)
		{
			DefendStates defendStates = this;
			smi.Play("slap_pre", KAnim.PlayMode.Once);
			smi.Queue("slap", KAnim.PlayMode.Once);
			smi.Queue("slap_pst", KAnim.PlayMode.Once);
			smi.Schedule(0.5f, delegate
			{
				smi.GetComponent<Weapon>().AttackTarget(defendStates.target.Get(smi));
			}, null);
		}).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Defend, false);
	}
}
