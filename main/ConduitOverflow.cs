using UnityEngine;

public class ConduitOverflow : KMonoBehaviour, ISecondaryOutput
{
	[SerializeField]
	public ConduitPortInfo portInfo;

	private int inputCell;

	private int outputCell;

	private FlowUtilityNetwork.NetworkItem secondaryOutput;

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
		secondaryOutput = new FlowUtilityNetwork.NetworkItem(portInfo.conduitType, Endpoint.Sink, cell2, base.gameObject);
		networkManager.AddToNetworks(secondaryOutput.Cell, secondaryOutput, true);
	}

	protected override void OnCleanUp()
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(portInfo.conduitType);
		networkManager.RemoveFromNetworks(secondaryOutput.Cell, secondaryOutput, true);
		Conduit.GetFlowManager(portInfo.conduitType).RemoveConduitUpdater(ConduitUpdate);
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(portInfo.conduitType);
		if (flowManager.HasConduit(inputCell))
		{
			ConduitFlow.ConduitContents contents = flowManager.GetContents(inputCell);
			if (!(contents.mass <= 0f))
			{
				int cell = outputCell;
				ConduitFlow.ConduitContents contents2 = flowManager.GetContents(cell);
				if (contents2.mass > 0f)
				{
					cell = secondaryOutput.Cell;
					contents2 = flowManager.GetContents(cell);
				}
				if (contents2.mass <= 0f)
				{
					float num = flowManager.AddElement(cell, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
					if (num > 0f)
					{
						flowManager.RemoveElement(inputCell, num);
					}
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
