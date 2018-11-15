public class SafetyChecker
{
	public struct Condition
	{
		public delegate bool Callback(int cell, int cost, Context context);

		public Callback callback
		{
			get;
			private set;
		}

		public int mask
		{
			get;
			private set;
		}

		public Condition(string id, int condition_mask, Callback condition_callback)
		{
			this = default(Condition);
			callback = condition_callback;
			mask = condition_mask;
		}
	}

	public struct Context
	{
		public Navigator navigator;

		public OxygenBreather oxygenBreather;

		public SimTemperatureTransfer temperatureTransferer;

		public PrimaryElement primaryElement;

		public MinionBrain minionBrain;

		public int cell;

		public Context(KMonoBehaviour cmp)
		{
			cell = Grid.PosToCell(cmp);
			navigator = cmp.GetComponent<Navigator>();
			oxygenBreather = cmp.GetComponent<OxygenBreather>();
			minionBrain = cmp.GetComponent<MinionBrain>();
			temperatureTransferer = cmp.GetComponent<SimTemperatureTransfer>();
			primaryElement = cmp.GetComponent<PrimaryElement>();
		}
	}

	public Condition[] conditions
	{
		get;
		private set;
	}

	public SafetyChecker(Condition[] conditions)
	{
		this.conditions = conditions;
	}

	public int GetSafetyConditions(int cell, int cost, Context context, out bool all_conditions_met)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < conditions.Length; i++)
		{
			Condition condition = conditions[i];
			if (condition.callback(cell, cost, context))
			{
				num |= condition.mask;
				num2++;
			}
		}
		all_conditions_met = (num2 == conditions.Length);
		return num;
	}
}
