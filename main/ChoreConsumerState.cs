using Klei.AI;
using UnityEngine;

public class ChoreConsumerState
{
	public KPrefabID prefabid;

	public GameObject gameObject;

	public ChoreConsumer consumer;

	public ChoreProvider choreProvider;

	public Navigator navigator;

	public Ownable ownable;

	public Assignables assignables;

	public MinionResume resume;

	public ChoreDriver choreDriver;

	public Schedulable schedulable;

	public Traits traits;

	public Equipment equipment;

	public Storage storage;

	public ConsumableConsumer consumableConsumer;

	public Worker worker;

	public SolidTransferArm solidTransferArm;

	public bool hasSolidTransferArm;

	public ScheduleBlock scheduleBlock;

	public ChoreConsumerState(ChoreConsumer consumer)
	{
		this.consumer = consumer;
		navigator = consumer.GetComponent<Navigator>();
		prefabid = consumer.GetComponent<KPrefabID>();
		ownable = consumer.GetComponent<Ownable>();
		gameObject = consumer.gameObject;
		assignables = consumer.GetComponent<Assignables>();
		solidTransferArm = consumer.GetComponent<SolidTransferArm>();
		hasSolidTransferArm = ((Object)solidTransferArm != (Object)null);
		resume = consumer.GetComponent<MinionResume>();
		choreDriver = consumer.GetComponent<ChoreDriver>();
		schedulable = consumer.GetComponent<Schedulable>();
		traits = consumer.GetComponent<Traits>();
		choreProvider = consumer.GetComponent<ChoreProvider>();
		equipment = consumer.GetComponent<Equipment>();
		storage = consumer.GetComponent<Storage>();
		consumableConsumer = consumer.GetComponent<ConsumableConsumer>();
		worker = consumer.GetComponent<Worker>();
		if ((Object)schedulable != (Object)null)
		{
			int blockIdx = Schedule.GetBlockIdx();
			scheduleBlock = schedulable.GetSchedule().GetBlock(blockIdx);
		}
	}

	public void Refresh()
	{
		if ((Object)schedulable != (Object)null)
		{
			int blockIdx = Schedule.GetBlockIdx();
			scheduleBlock = schedulable.GetSchedule().GetBlock(blockIdx);
		}
	}
}
