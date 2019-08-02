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
			bool flag = HasSafeCell();
			RunSafeCellQuery(false);
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

	public void RunSafeCellQuery(bool avoid_light)
	{
		MinionPathFinderAbilities minionPathFinderAbilities = (MinionPathFinderAbilities)navigator.GetCurrentAbilities();
		minionPathFinderAbilities.SetIdleNavMaskEnabled(true);
		SafeCellQuery safeCellQuery = PathFinderQueries.safeCellQuery.Reset(brain, avoid_light);
		navigator.RunQuery(safeCellQuery);
		minionPathFinderAbilities.SetIdleNavMaskEnabled(false);
		cell = safeCellQuery.GetResultCell();
		if (cell == Grid.PosToCell(navigator))
		{
			cell = Grid.InvalidCell;
		}
	}

	public int GetSensorCell()
	{
		return cell;
	}

	public int GetCellQuery()
	{
		if (cell == Grid.InvalidCell)
		{
			RunSafeCellQuery(false);
		}
		return cell;
	}

	public int GetSleepCellQuery()
	{
		if (cell == Grid.InvalidCell)
		{
			RunSafeCellQuery(true);
		}
		return cell;
	}

	public bool HasSafeCell()
	{
		return cell != Grid.InvalidCell && cell != Grid.PosToCell(sensors);
	}
}
