using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitDispenser : KMonoBehaviour, ISaveLoadable
{
	[SerializeField]
	public ConduitType conduitType;

	[SerializeField]
	public SimHashes[] elementFilter = null;

	[SerializeField]
	public bool invertElementFilter = false;

	[SerializeField]
	public bool alwaysDispense = false;

	private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	public Storage storage;

	private HandleVector<int>.Handle partitionerEntry;

	private int utilityCell = -1;

	private int elementOutputOffset = 0;

	public ConduitType TypeOfConduit => conduitType;

	public ConduitFlow.ConduitContents ConduitContents => GetConduitManager().GetContents(utilityCell);

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[utilityCell, (conduitType != ConduitType.Gas) ? 16 : 12];
			return (Object)gameObject != (Object)null && (Object)gameObject.GetComponent<BuildingComplete>() != (Object)null;
		}
	}

	public void SetConduitData(ConduitType type)
	{
		conduitType = type;
	}

	public ConduitFlow GetConduitManager()
	{
		switch (conduitType)
		{
		case ConduitType.Gas:
			return Game.Instance.gasConduitFlow;
		case ConduitType.Liquid:
			return Game.Instance.liquidConduitFlow;
		default:
			return null;
		}
	}

	private void OnConduitConnectionChanged(object data)
	{
		Trigger(-2094018600, IsConnected);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		utilityCell = GetComponent<Building>().GetUtilityOutputCell();
		ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(conduitType != ConduitType.Gas) ? 16 : 12];
		partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, utilityCell, layer, OnConduitConnectionChanged);
		GetConduitManager().AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Last);
		OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		GetConduitManager().RemoveConduitUpdater(ConduitUpdate);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		operational.SetFlag(outputConduitFlag, IsConnected);
		if (operational.IsOperational || alwaysDispense)
		{
			PrimaryElement primaryElement = FindSuitableElement();
			if ((Object)primaryElement != (Object)null)
			{
				primaryElement.KeepZeroMassObject = true;
				ConduitFlow conduitManager = GetConduitManager();
				float num = conduitManager.AddElement(utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
				if (num > 0f)
				{
					float num2 = num / primaryElement.Mass;
					int num3 = (int)(num2 * (float)primaryElement.DiseaseCount);
					primaryElement.ModifyDiseaseCount(-num3, "ConduitDispenser.ConduitUpdate");
					primaryElement.Mass -= num;
					Trigger(-1697596308, primaryElement.gameObject);
				}
			}
		}
	}

	private PrimaryElement FindSuitableElement()
	{
		List<GameObject> items = storage.items;
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			int index = (i + elementOutputOffset) % count;
			PrimaryElement component = items[index].GetComponent<PrimaryElement>();
			if ((Object)component != (Object)null && component.Mass > 0f && ((conduitType != ConduitType.Liquid) ? component.Element.IsGas : component.Element.IsLiquid) && (elementFilter == null || elementFilter.Length == 0 || (!invertElementFilter && IsFilteredElement(component.ElementID)) || (invertElementFilter && !IsFilteredElement(component.ElementID))))
			{
				elementOutputOffset = (elementOutputOffset + 1) % count;
				return component;
			}
		}
		return null;
	}

	private bool IsFilteredElement(SimHashes element)
	{
		for (int i = 0; i != elementFilter.Length; i++)
		{
			if (elementFilter[i] == element)
			{
				return true;
			}
		}
		return false;
	}
}
