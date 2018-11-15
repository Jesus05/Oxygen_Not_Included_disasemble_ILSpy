using UnityEngine;

public class ClosestEdibleSensor : Sensor
{
	private static Tag[] edibleTag = new Tag[1]
	{
		GameTags.Edible
	};

	private Edible edible;

	private Worker worker;

	private bool hasEdible;

	public bool edibleInReachButNotPermitted;

	public ClosestEdibleSensor(Sensors sensors)
		: base(sensors)
	{
		worker = GetComponent<Worker>();
	}

	public override void Update()
	{
		Pickupable pickupable = Game.Instance.fetchManager.FindEdibleFetchTarget(worker, GetComponent<Storage>(), new TagBits(edibleTag), default(TagBits), new TagBits(GetComponent<ConsumableConsumer>().forbiddenTags), 0f);
		bool flag = edibleInReachButNotPermitted;
		Edible x = null;
		bool flag2 = false;
		if ((Object)pickupable != (Object)null)
		{
			x = pickupable.GetComponent<Edible>();
			flag2 = true;
			flag = false;
		}
		else
		{
			Pickupable x2 = Game.Instance.fetchManager.FindFetchTarget(worker, GetComponent<Storage>(), new TagBits(edibleTag), default(TagBits), default(TagBits), 0f);
			flag = ((Object)x2 != (Object)null);
		}
		if ((Object)x != (Object)edible || hasEdible != flag2)
		{
			edible = x;
			hasEdible = flag2;
			edibleInReachButNotPermitted = flag;
			Trigger(86328522, edible);
		}
	}

	public Edible GetEdible()
	{
		return edible;
	}
}
