using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class RequireOutputs : KMonoBehaviour
{
	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Operational operational;

	public bool ignoreFullPipe;

	private int utilityCell;

	private ConduitType conduitType;

	private static readonly Operational.Flag outputConnectedFlag = new Operational.Flag("output_connected", Operational.Flag.Type.Requirement);

	private static readonly Operational.Flag pipesHaveRoomFlag = new Operational.Flag("pipesHaveRoom", Operational.Flag.Type.Requirement);

	private bool previouslyConnected = true;

	private bool previouslyHadRoom = true;

	private bool connected;

	private Guid hasPipeGuid;

	private Guid pipeBlockedGuid;

	private HandleVector<int>.Handle partitionerEntry;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ScenePartitionerLayer scenePartitionerLayer = null;
		Building component = GetComponent<Building>();
		utilityCell = component.GetUtilityOutputCell();
		conduitType = component.Def.OutputConduitType;
		switch (component.Def.OutputConduitType)
		{
		case ConduitType.Gas:
			scenePartitionerLayer = GameScenePartitioner.Instance.gasConduitsLayer;
			break;
		case ConduitType.Liquid:
			scenePartitionerLayer = GameScenePartitioner.Instance.liquidConduitsLayer;
			break;
		case ConduitType.Solid:
			scenePartitionerLayer = GameScenePartitioner.Instance.solidConduitsLayer;
			break;
		}
		UpdateConnectionState(true);
		UpdatePipeRoomState(true);
		if (scenePartitionerLayer != null)
		{
			partitionerEntry = GameScenePartitioner.Instance.Add("RequireOutputs", base.gameObject, utilityCell, scenePartitionerLayer, delegate
			{
				UpdateConnectionState(false);
			});
		}
		GetConduitFlow().AddConduitUpdater(UpdatePipeState, ConduitFlowPriority.First);
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		GetConduitFlow()?.RemoveConduitUpdater(UpdatePipeState);
		base.OnCleanUp();
	}

	private void UpdateConnectionState(bool force_update = false)
	{
		connected = IsConnected(utilityCell);
		if (connected != previouslyConnected || force_update)
		{
			operational.SetFlag(outputConnectedFlag, connected);
			previouslyConnected = connected;
			StatusItem status_item = null;
			switch (conduitType)
			{
			case ConduitType.Liquid:
				status_item = Db.Get().BuildingStatusItems.NeedLiquidOut;
				break;
			case ConduitType.Gas:
				status_item = Db.Get().BuildingStatusItems.NeedGasOut;
				break;
			case ConduitType.Solid:
				status_item = Db.Get().BuildingStatusItems.NeedSolidOut;
				break;
			}
			hasPipeGuid = selectable.ToggleStatusItem(status_item, hasPipeGuid, !connected, this);
		}
	}

	private bool OutputPipeIsEmpty()
	{
		if (ignoreFullPipe)
		{
			return true;
		}
		bool result = true;
		if (connected)
		{
			IConduitFlow conduitFlow = GetConduitFlow();
			result = conduitFlow.IsConduitEmpty(utilityCell);
		}
		return result;
	}

	private void UpdatePipeState(float dt)
	{
		UpdatePipeRoomState(false);
	}

	private void UpdatePipeRoomState(bool force_update = false)
	{
		bool flag = OutputPipeIsEmpty();
		if (flag != previouslyHadRoom || force_update)
		{
			operational.SetFlag(pipesHaveRoomFlag, flag);
			previouslyHadRoom = flag;
			StatusItem conduitBlockedMultiples = Db.Get().BuildingStatusItems.ConduitBlockedMultiples;
			pipeBlockedGuid = selectable.ToggleStatusItem(conduitBlockedMultiples, pipeBlockedGuid, !flag, null);
		}
	}

	private IConduitFlow GetConduitFlow()
	{
		switch (conduitType)
		{
		case ConduitType.Gas:
			return Game.Instance.gasConduitFlow;
		case ConduitType.Liquid:
			return Game.Instance.liquidConduitFlow;
		case ConduitType.Solid:
			return Game.Instance.solidConduitFlow;
		default:
			Debug.LogWarning("GetConduitFlow() called with unexpected conduitType: " + conduitType.ToString());
			return null;
		}
	}

	private bool IsConnected(int cell)
	{
		return IsConnected(cell, conduitType);
	}

	public static bool IsConnected(int cell, ConduitType conduitType)
	{
		ObjectLayer layer = ObjectLayer.NumLayers;
		switch (conduitType)
		{
		case ConduitType.Gas:
			layer = ObjectLayer.GasConduit;
			break;
		case ConduitType.Liquid:
			layer = ObjectLayer.LiquidConduit;
			break;
		case ConduitType.Solid:
			layer = ObjectLayer.SolidConduit;
			break;
		}
		GameObject gameObject = Grid.Objects[cell, (int)layer];
		return (UnityEngine.Object)gameObject != (UnityEngine.Object)null && (UnityEngine.Object)gameObject.GetComponent<BuildingComplete>() != (UnityEngine.Object)null;
	}
}
