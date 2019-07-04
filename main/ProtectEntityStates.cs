using STRINGS;

public class ProtectEntityStates : GameStateMachine<ProtectEntityStates, ProtectEntityStates.Instance, IStateMachineTarget, ProtectEntityStates.Def>
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
		State state = root.Enter("SetTarget", delegate(Instance smi)
		{
			target.Set(smi.GetSMI<EntityThreatMonitor.Instance>().MainThreat, smi);
		});
		string name = CREATURES.STATUSITEMS.PROTECTINGENTITY.NAME;
		string tooltip = CREATURES.STATUSITEMS.PROTECTINGENTITY.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
		protectEntity.DoNothing();
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
			ProtectEntityStates protectEntityStates = this;
			smi.Play("slap_pre", KAnim.PlayMode.Once);
			smi.Queue("slap", KAnim.PlayMode.Once);
			smi.Queue("slap_pst", KAnim.PlayMode.Once);
			smi.Schedule(0.5f, delegate
			{
				smi.GetComponent<Weapon>().AttackTarget(protectEntityStates.target.Get(smi));
			}, null);
		}).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Defend, false);
	}
}
