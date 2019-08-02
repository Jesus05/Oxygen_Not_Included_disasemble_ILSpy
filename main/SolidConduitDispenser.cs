using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidConduitDispenser : KMonoBehaviour, ISaveLoadable
{
	[SerializeField]
	public SimHashes[] elementFilter;

	[SerializeField]
	public bool invertElementFilter;

	[SerializeField]
	public bool alwaysDispense;

	private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	public Storage storage;

	private HandleVector<int>.Handle partitionerEntry;

	private int utilityCell = -1;

	private bool dispensing;

	private int round_robin_index;

	private const float MaxMass = 20f;

	public SolidConduitFlow.ConduitContents ConduitContents => GetConduitFlow().GetContents(utilityCell);

	public bool IsDispensing => dispensing;

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[utilityCell, 20];
			return (Object)gameObject != (Object)null && (Object)gameObject.GetComponent<BuildingComplete>() != (Object)null;
		}
	}

	public SolidConduitFlow GetConduitFlow()
	{
		return Game.Instance.solidConduitFlow;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		utilityCell = GetComponent<Building>().GetUtilityOutputCell();
		ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[20];
		partitionerEntry = GameScenePartitioner.Instance.Add("SolidConduitConsumer.OnSpawn", base.gameObject, utilityCell, layer, OnConduitConnectionChanged);
		GetConduitFlow().AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Last);
		OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		GetConduitFlow().RemoveConduitUpdater(ConduitUpdate);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void OnConduitConnectionChanged(object data)
	{
		dispensing = (dispensing && IsConnected);
		Trigger(-2094018600, IsConnected);
	}

	private void ConduitUpdate(float dt)
	{
		bool flag = false;
		operational.SetFlag(outputConduitFlag, IsConnected);
		if (operational.IsOperational || alwaysDispense)
		{
			SolidConduitFlow conduitFlow = GetConduitFlow();
			if (conduitFlow.HasConduit(utilityCell) && conduitFlow.IsConduitEmpty(utilityCell))
			{
				Pickupable pickupable = FindSuitableItem();
				if ((bool)pickupable)
				{
					if (pickupable.PrimaryElement.Mass > 20f)
					{
						pickupable = pickupable.Take(20f);
					}
					conduitFlow.AddPickupable(utilityCell, pickupable);
					flag = true;
				}
			}
		}
		storage.storageNetworkID = GetConnectedNetworkID();
		dispensing = flag;
	}

	private Pickupable FindSuitableItem()
	{
		List<GameObject> items = storage.items;
		if (items.Count < 1)
		{
			return null;
		}
		round_robin_index %= items.Count;
		GameObject gameObject = items[round_robin_index];
		round_robin_index++;
		return (!(bool)gameObject) ? null : gameObject.GetComponent<Pickupable>();
	}

	private int GetConnectedNetworkID()
	{
		GameObject gameObject = Grid.Objects[utilityCell, 20];
		SolidConduit solidConduit = (!((Object)gameObject != (Object)null)) ? null : gameObject.GetComponent<SolidConduit>();
		return ((!((Object)solidConduit != (Object)null)) ? null : solidConduit.GetNetwork())?.id ?? (-1);
	}
}
