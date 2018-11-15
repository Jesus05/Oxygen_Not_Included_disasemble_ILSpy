public class ToiletSensor : Sensor
{
	private Navigator navigator;

	private IUsable toilet;

	private bool areThereAnyToilets;

	private bool areThereAnyUsableToilets;

	public ToiletSensor(Sensors sensors)
		: base(sensors)
	{
		navigator = GetComponent<Navigator>();
	}

	public override void Update()
	{
		IUsable usable = null;
		int num = 2147483647;
		bool flag = false;
		foreach (IUsable item in Components.Toilets.Items)
		{
			if (item.IsUsable())
			{
				flag = true;
				int navigationCost = navigator.GetNavigationCost(Grid.PosToCell(item.transform.GetPosition()));
				if (navigationCost != -1 && navigationCost < num)
				{
					usable = item;
					num = navigationCost;
				}
			}
		}
		bool flag2 = Components.Toilets.Count > 0;
		if (usable != toilet || flag2 != areThereAnyToilets || areThereAnyUsableToilets != flag)
		{
			toilet = usable;
			areThereAnyToilets = flag2;
			areThereAnyUsableToilets = flag;
			Trigger(-752545459, null);
		}
	}

	public bool AreThereAnyToilets()
	{
		return areThereAnyToilets;
	}

	public bool AreThereAnyUsableToilets()
	{
		return areThereAnyUsableToilets;
	}

	public IUsable GetNearestUsableToilet()
	{
		return toilet;
	}
}
