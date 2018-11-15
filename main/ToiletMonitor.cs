public class ToiletMonitor : GameStateMachine<ToiletMonitor, ToiletMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private ToiletSensor toiletSensor;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			toiletSensor = GetComponent<Sensors>().GetSensor<ToiletSensor>();
		}

		public void RefreshStatusItem()
		{
			StatusItem status_item = null;
			if (!toiletSensor.AreThereAnyToilets())
			{
				status_item = Db.Get().DuplicantStatusItems.NoToilets;
			}
			else if (!toiletSensor.AreThereAnyUsableToilets())
			{
				status_item = Db.Get().DuplicantStatusItems.NoUsableToilets;
			}
			else if (toiletSensor.GetNearestUsableToilet() == null)
			{
				status_item = Db.Get().DuplicantStatusItems.ToiletUnreachable;
			}
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Toilet, status_item, null);
		}

		public void ClearStatusItem()
		{
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Toilet, null, null);
		}
	}

	public State satisfied;

	public State unsatisfied;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.EventHandler(GameHashes.ToiletSensorChanged, delegate(Instance smi)
		{
			smi.RefreshStatusItem();
		}).Exit("ClearStatusItem", delegate(Instance smi)
		{
			smi.ClearStatusItem();
		});
	}
}
