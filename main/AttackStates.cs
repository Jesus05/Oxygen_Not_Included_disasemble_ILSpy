using STRINGS;

public class AttackStates : GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Attack);
		}
	}

	public TargetParameter target;

	public ApproachSubState<AttackableBase> approach;

	public State attack;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = approach;
		root.Enter("SetTarget", delegate(Instance smi)
		{
			target.Set(smi.GetSMI<ThreatMonitor.Instance>().MainThreat, smi);
		});
		State state = approach.InitializeStates(masterTarget, target, attack, null, new CellOffset[5]
		{
			new CellOffset(0, 0),
			new CellOffset(1, 0),
			new CellOffset(-1, 0),
			new CellOffset(1, 1),
			new CellOffset(-1, 1)
		}, null);
		string name = CREATURES.STATUSITEMS.ATTACK_APPROACH.NAME;
		string tooltip = CREATURES.STATUSITEMS.ATTACK_APPROACH.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main);
		State state2 = attack.Enter(delegate(Instance smi)
		{
			AttackStates attackStates = this;
			smi.Play("eat_pre", KAnim.PlayMode.Once);
			smi.Queue("eat_pst", KAnim.PlayMode.Once);
			smi.Schedule(0.5f, delegate
			{
				smi.GetComponent<Weapon>().AttackTarget(attackStates.target.Get(smi));
			}, null);
		});
		tooltip = CREATURES.STATUSITEMS.ATTACK.NAME;
		name = CREATURES.STATUSITEMS.ATTACK.TOOLTIP;
		main = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(tooltip, name, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Attack, false);
	}
}
