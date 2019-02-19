public class CringeMonitor : GameStateMachine<CringeMonitor, CringeMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private StatusItem statusItem;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void SetCringeSourceData(object data)
		{
			string name = (string)data;
			statusItem = new StatusItem("CringeSource", name, null, string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, 63486);
		}

		public StatusItem GetStatusItem()
		{
			return statusItem;
		}
	}

	private static readonly HashedString[] CringeAnims = new HashedString[3]
	{
		"cringe_pre",
		"cringe_loop",
		"cringe_pst"
	};

	public State idle;

	public State cringe;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.EventHandler(GameHashes.Cringe, TriggerCringe);
		cringe.ToggleChore((Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, "anim_cringe_kanim", CringeAnims, smi.GetStatusItem), idle).ScheduleGoTo(3f, idle);
	}

	private void TriggerCringe(Instance smi, object data)
	{
		if (!smi.GetComponent<KPrefabID>().HasTag(GameTags.Suit))
		{
			smi.SetCringeSourceData(data);
			smi.GoTo(cringe);
		}
	}
}
