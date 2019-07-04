using KSerialization;
using STRINGS;
using System;
using System.Runtime.Serialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElementFilter : KMonoBehaviour, ISaveLoadable, ISecondaryOutput
{
	[SerializeField]
	public ConduitPortInfo portInfo;

	[Serialize]
	private Tag filteredTag = GameTags.Water;

	private SimHashes filteredElem = SimHashes.Void;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private KSelectable selectable;

	public Filterable filterable = null;

	private Guid needsConduitStatusItemGuid;

	private Guid conduitBlockedStatusItemGuid;

	private int inputCell = -1;

	private int outputCell = -1;

	private int filteredCell = -1;

	private FlowUtilityNetwork.NetworkItem itemFilter;

	private HandleVector<int>.Handle partitionerEntry;

	private static StatusItem filterStatusItem = null;

	public SimHashes FilteredElement => filteredElem;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		filterable = GetComponent<Filterable>();
		ConduitType conduitType = portInfo.conduitType;
		if (conduitType == ConduitType.Gas && filteredTag == GameTags.Water)
		{
			filteredTag = GameTags.Oxygen;
		}
		InitializeStatusItems();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		inputCell = building.GetUtilityInputCell();
		outputCell = building.GetUtilityOutputCell();
		int cell = Grid.PosToCell(base.transform.GetPosition());
		CellOffset rotatedOffset = building.GetRotatedOffset(portInfo.offset);
		filteredCell = Grid.OffsetCell(cell, rotatedOffset);
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(portInfo.conduitType);
		itemFilter = new FlowUtilityNetwork.NetworkItem(portInfo.conduitType, Endpoint.Source, filteredCell, base.gameObject);
		networkManager.AddToNetworks(filteredCell, itemFilter, true);
		GetComponent<ConduitConsumer>().isConsuming = false;
		OnFilterChanged(ElementLoader.FindElementByHash(filteredElem).tag);
		filterable.onFilterChanged += OnFilterChanged;
		ConduitFlow flowManager = Conduit.GetFlowManager(portInfo.conduitType);
		flowManager.AddConduitUpdater(OnConduitTick, ConduitFlowPriority.Default);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, filterStatusItem, this);
		UpdateConduitExistsStatus();
		UpdateConduitBlockedStatus();
		ScenePartitionerLayer scenePartitionerLayer = null;
		switch (portInfo.conduitType)
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
		if (scenePartitionerLayer != null)
		{
			partitionerEntry = GameScenePartitioner.Instance.Add("ElementFilterConduitExists", base.gameObject, filteredCell, scenePartitionerLayer, delegate
			{
				UpdateConduitExistsStatus();
			});
		}
	}

	protected override void OnCleanUp()
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(portInfo.conduitType);
		networkManager.RemoveFromNetworks(filteredCell, itemFilter, true);
		ConduitFlow flowManager = Conduit.GetFlowManager(portInfo.conduitType);
		flowManager.RemoveConduitUpdater(OnConduitTick);
		if (partitionerEntry.IsValid() && (UnityEngine.Object)GameScenePartitioner.Instance != (UnityEngine.Object)null)
		{
			GameScenePartitioner.Instance.Free(ref partitionerEntry);
		}
		base.OnCleanUp();
	}

	private void OnConduitTick(float dt)
	{
		bool value = false;
		UpdateConduitBlockedStatus();
		if (operational.IsOperational)
		{
			ConduitFlow flowManager = Conduit.GetFlowManager(portInfo.conduitType);
			ConduitFlow.ConduitContents contents = flowManager.GetContents(inputCell);
			int num = (contents.element != filteredElem) ? outputCell : filteredCell;
			ConduitFlow.ConduitContents contents2 = flowManager.GetContents(num);
			if (contents.mass > 0f && contents2.mass <= 0f)
			{
				value = true;
				float num2 = flowManager.AddElement(num, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
				if (num2 > 0f)
				{
					flowManager.RemoveElement(inputCell, num2);
				}
			}
		}
		operational.SetActive(value, false);
	}

	private void UpdateConduitExistsStatus()
	{
		bool flag = RequireOutputs.IsConnected(filteredCell, portInfo.conduitType);
		StatusItem status_item;
		switch (portInfo.conduitType)
		{
		case ConduitType.Gas:
			status_item = Db.Get().BuildingStatusItems.NeedGasOut;
			break;
		case ConduitType.Liquid:
			status_item = Db.Get().BuildingStatusItems.NeedLiquidOut;
			break;
		case ConduitType.Solid:
			status_item = Db.Get().BuildingStatusItems.NeedSolidOut;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		bool flag2 = needsConduitStatusItemGuid != Guid.Empty;
		if (flag == flag2)
		{
			needsConduitStatusItemGuid = selectable.ToggleStatusItem(status_item, needsConduitStatusItemGuid, !flag, null);
		}
	}

	private void UpdateConduitBlockedStatus()
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(portInfo.conduitType);
		bool flag = flowManager.IsConduitEmpty(filteredCell);
		StatusItem conduitBlockedMultiples = Db.Get().BuildingStatusItems.ConduitBlockedMultiples;
		bool flag2 = conduitBlockedStatusItemGuid != Guid.Empty;
		if (flag == flag2)
		{
			conduitBlockedStatusItemGuid = selectable.ToggleStatusItem(conduitBlockedMultiples, conduitBlockedStatusItemGuid, !flag, null);
		}
	}

	private void OnFilterChanged(Tag tag)
	{
		bool on = true;
		filteredTag = tag;
		Element element = ElementLoader.GetElement(filteredTag);
		if (element != null)
		{
			filteredElem = element.id;
			on = (filteredElem == SimHashes.Void || filteredElem == SimHashes.Vacuum);
		}
		GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, on, null);
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		Element element = ElementLoader.GetElement(filteredTag);
		if (element != null)
		{
			filterable.SelectedTag = filteredTag;
		}
	}

	private void InitializeStatusItems()
	{
		if (filterStatusItem == null)
		{
			filterStatusItem = new StatusItem("Filter", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.LiquidConduits.ID, true, 129022);
			filterStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				ElementFilter elementFilter = (ElementFilter)data;
				if (elementFilter.filteredElem == SimHashes.Void)
				{
					str = string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, BUILDINGS.PREFABS.GASFILTER.ELEMENT_NOT_SPECIFIED);
				}
				else
				{
					Element element = ElementLoader.FindElementByHash(elementFilter.filteredElem);
					str = string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, element.name);
				}
				return str;
			};
			filterStatusItem.conditionalOverlayCallback = ShowInUtilityOverlay;
		}
	}

	private bool ShowInUtilityOverlay(HashedString mode, object data)
	{
		bool result = false;
		ElementFilter elementFilter = (ElementFilter)data;
		switch (elementFilter.portInfo.conduitType)
		{
		case ConduitType.Gas:
			result = (mode == OverlayModes.GasConduits.ID);
			break;
		case ConduitType.Liquid:
			result = (mode == OverlayModes.LiquidConduits.ID);
			break;
		}
		return result;
	}

	public ConduitType GetSecondaryConduitType()
	{
		return portInfo.conduitType;
	}

	public CellOffset GetSecondaryConduitOffset()
	{
		return portInfo.offset;
	}

	public int GetFilteredCell()
	{
		return filteredCell;
	}
}
