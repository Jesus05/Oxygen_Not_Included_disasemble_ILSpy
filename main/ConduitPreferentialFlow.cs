using UnityEngine;

public class ConduitPreferentialFlow : KMonoBehaviour, ISecondaryInput
{
	[SerializeField]
	public ConduitPortInfo portInfo;

	private int inputCell;

	private int outputCell;

	private FlowUtilityNetwork.NetworkItem secondaryInput;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = GetComponent<Building>();
		inputCell = component.GetUtilityInputCell();
		outputCell = component.GetUtilityOutputCell();
		int cell = Grid.PosToCell(base.transform.GetPosition());
		CellOffset rotatedOffset = component.GetRotatedOffset(portInfo.offset);
		int cell2 = Grid.OffsetCell(cell, rotatedOffset);
		Conduit.GetFlowManager(portInfo.conduitType).AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Default);
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(portInfo.conduitType);
		secondaryInput = new FlowUtilityNetwork.NetworkItem(portInfo.conduitType, Endpoint.Sink, cell2, base.gameObject);
		networkManager.AddToNetworks(secondaryInput.Cell, secondaryInput, true);
	}

	protected override void OnCleanUp()
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(portInfo.conduitType);
		networkManager.RemoveFromNetworks(secondaryInput.Cell, secondaryInput, true);
		Conduit.GetFlowManager(portInfo.conduitType).RemoveConduitUpdater(ConduitUpdate);
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(portInfo.conduitType);
		if (flowManager.HasConduit(outputCell))
		{
			int cell = inputCell;
			ConduitFlow.ConduitContents contents = flowManager.GetContents(cell);
			if (contents.mass <= 0f)
			{
				cell = secondaryInput.Cell;
				contents = flowManager.GetContents(cell);
			}
			if (contents.mass > 0f)
			{
				float num = flowManager.AddElement(outputCell, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
				if (num > 0f)
				{
					flowManager.RemoveElement(cell, num);
				}
			}
		}
	}

	public ConduitType GetSecondaryConduitType()
	{
		return portInfo.conduitType;
	}

	public CellOffset GetSecondaryConduitOffset()
	{
		return portInfo.offset;
	}
}
