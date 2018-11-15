using UnityEngine;

public class IdleCellSensor : Sensor
{
	private MinionBrain brain;

	private Navigator navigator;

	private int cell;

	public IdleCellSensor(Sensors sensors)
		: base(sensors)
	{
		navigator = GetComponent<Navigator>();
		brain = GetComponent<MinionBrain>();
	}

	public override void Update()
	{
		IdleCellQuery idleCellQuery = PathFinderQueries.idleCellQuery.Reset(brain, Random.Range(30, 60));
		(brain.GetComponent<Navigator>().GetCurrentAbilities() as MinionPathFinderAbilities).SetIdleNavMaskEnabled(true);
		navigator.RunQuery(idleCellQuery);
		(brain.GetComponent<Navigator>().GetCurrentAbilities() as MinionPathFinderAbilities).SetIdleNavMaskEnabled(false);
		cell = idleCellQuery.GetResultCell();
	}

	public int GetCell()
	{
		return cell;
	}
}
