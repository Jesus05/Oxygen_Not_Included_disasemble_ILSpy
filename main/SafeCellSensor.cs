public class SafeCellSensor : Sensor
{
	private MinionBrain brain;

	private Navigator navigator;

	private KPrefabID prefabid;

	private int cell = Grid.InvalidCell;

	public SafeCellSensor(Sensors sensors)
		: base(sensors)
	{
		navigator = GetComponent<Navigator>();
		brain = GetComponent<MinionBrain>();
		prefabid = GetComponent<KPrefabID>();
	}

	public override void Update()
	{
		if (!prefabid.HasTag(GameTags.Idle))
		{
			cell = Grid.InvalidCell;
		}
		else
		{
			(brain.GetComponent<Navigator>().GetCurrentAbilities() as MinionPathFinderAbilities).SetIdleNavMaskEnabled(true);
			SafeCellQuery safeCellQuery = PathFinderQueries.safeCellQuery.Reset(brain);
			navigator.RunQuery(safeCellQuery);
			(brain.GetComponent<Navigator>().GetCurrentAbilities() as MinionPathFinderAbilities).SetIdleNavMaskEnabled(false);
			bool flag = HasSafeCell();
			cell = safeCellQuery.GetResultCell();
			if (cell == Grid.PosToCell(navigator))
			{
				cell = Grid.InvalidCell;
			}
			bool flag2 = HasSafeCell();
			if (flag2 != flag)
			{
				if (flag2)
				{
					sensors.Trigger(982561777, null);
				}
				else
				{
					sensors.Trigger(506919987, null);
				}
			}
		}
	}

	public int GetCell()
	{
		return cell;
	}

	public bool HasSafeCell()
	{
		return cell != Grid.InvalidCell && cell != Grid.PosToCell(sensors);
	}
}
