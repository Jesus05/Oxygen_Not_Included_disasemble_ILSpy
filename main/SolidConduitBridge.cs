public class SolidConduitBridge : KMonoBehaviour
{
	[MyCmpGet]
	private Operational operational;

	private int inputCell;

	private int outputCell;

	private bool dispensing;

	public bool IsDispensing => dispensing;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = GetComponent<Building>();
		inputCell = component.GetUtilityInputCell();
		outputCell = component.GetUtilityOutputCell();
		SolidConduit.GetFlowManager().AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Default);
	}

	protected override void OnCleanUp()
	{
		SolidConduit.GetFlowManager().RemoveConduitUpdater(ConduitUpdate);
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		dispensing = false;
		if (!(bool)operational || operational.IsOperational)
		{
			SolidConduitFlow flowManager = SolidConduit.GetFlowManager();
			if (flowManager.HasConduit(inputCell) && flowManager.HasConduit(outputCell) && flowManager.IsConduitFull(inputCell) && flowManager.IsConduitEmpty(outputCell))
			{
				Pickupable pickupable = flowManager.RemovePickupable(inputCell);
				if ((bool)pickupable)
				{
					flowManager.AddPickupable(outputCell, pickupable);
					dispensing = true;
				}
			}
		}
	}
}
