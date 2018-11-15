using System.Collections.Generic;
using UnityEngine;

public class ConduitBridge : KMonoBehaviour, IBridgedNetworkItem
{
	[SerializeField]
	public ConduitType type;

	private int inputCell;

	private int outputCell;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		accumulator = Game.Instance.accumulators.Add("Flow", this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = GetComponent<Building>();
		inputCell = component.GetUtilityInputCell();
		outputCell = component.GetUtilityOutputCell();
		Conduit.GetFlowManager(type).AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Default);
	}

	protected override void OnCleanUp()
	{
		Conduit.GetFlowManager(type).RemoveConduitUpdater(ConduitUpdate);
		Game.Instance.accumulators.Remove(accumulator);
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(type);
		if (flowManager.HasConduit(inputCell))
		{
			ConduitFlow.ConduitContents contents = flowManager.GetContents(inputCell);
			if (contents.mass > 0f)
			{
				float num = flowManager.AddElement(outputCell, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
				if (num > 0f)
				{
					flowManager.RemoveElement(inputCell, num);
					Game.Instance.accumulators.Accumulate(accumulator, contents.mass);
				}
			}
		}
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(type);
		UtilityNetwork networkForCell = networkManager.GetNetworkForCell(inputCell);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
		networkForCell = networkManager.GetNetworkForCell(outputCell);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		bool flag = false;
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(type);
		return flag || networks.Contains(networkManager.GetNetworkForCell(inputCell)) || networks.Contains(networkManager.GetNetworkForCell(outputCell));
	}

	public int GetNetworkCell()
	{
		return inputCell;
	}
}
