using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantablePlot : SingleEntityReceptacle, ISaveLoadable, IEffectDescriptor, IGameObjectEffectDescriptor
{
	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	[Serialize]
	private Ref<KPrefabID> plantRef;

	public Vector3 occupyingObjectVisualOffset = Vector3.zero;

	public Grid.SceneLayer plantLayer = Grid.SceneLayer.BuildingBack;

	private EntityPreview plantPreview;

	[SerializeField]
	private bool accepts_fertilizer;

	[SerializeField]
	private bool accepts_irrigation = true;

	[SerializeField]
	public bool has_liquid_pipe_input;

	private static readonly EventSystem.IntraObjectHandler<PlantablePlot> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<PlantablePlot>(delegate(PlantablePlot component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PlantablePlot> OnUpdateRoomDelegate = new EventSystem.IntraObjectHandler<PlantablePlot>(delegate(PlantablePlot component, object data)
	{
		if ((UnityEngine.Object)component.plantRef.Get() != (UnityEngine.Object)null)
		{
			component.plantRef.Get().Trigger(144050788, data);
		}
	});

	public KPrefabID plant
	{
		get
		{
			return plantRef.Get();
		}
		set
		{
			plantRef.Set(value);
		}
	}

	public bool ValidPlant => (UnityEngine.Object)plantPreview == (UnityEngine.Object)null || plantPreview.Valid;

	public bool AcceptsFertilizer => accepts_fertilizer;

	public bool AcceptsIrrigation => accepts_irrigation;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		statusItemNeed = Db.Get().BuildingStatusItems.NeedSeed;
		statusItemNoneAvailable = Db.Get().BuildingStatusItems.NoAvailableSeed;
		statusItemAwaitingDelivery = Db.Get().BuildingStatusItems.AwaitingSeedDelivery;
		plantRef = new Ref<KPrefabID>();
		destroyEntityOnDeposit = true;
		Subscribe(-905833192, OnCopySettingsDelegate);
		Subscribe(144050788, OnUpdateRoomDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		PlantablePlot component = gameObject.GetComponent<PlantablePlot>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			if ((UnityEngine.Object)base.occupyingObject == (UnityEngine.Object)null && (requestedEntityTag != component.requestedEntityTag || (UnityEngine.Object)component.occupyingObject != (UnityEngine.Object)null))
			{
				Tag entityTag = component.requestedEntityTag;
				if ((UnityEngine.Object)component.occupyingObject != (UnityEngine.Object)null)
				{
					SeedProducer component2 = component.occupyingObject.GetComponent<SeedProducer>();
					if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
					{
						entityTag = TagManager.Create(component2.seedInfo.seedId);
					}
				}
				CancelActiveRequest();
				CreateOrder(entityTag);
			}
			if ((UnityEngine.Object)base.occupyingObject != (UnityEngine.Object)null)
			{
				Prioritizable component3 = GetComponent<Prioritizable>();
				if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
				{
					Prioritizable component4 = base.occupyingObject.GetComponent<Prioritizable>();
					if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
					{
						component4.SetMasterPriority(component3.GetMasterPriority());
					}
				}
			}
		}
	}

	public override void CreateOrder(Tag entityTag)
	{
		SetPreview(entityTag, false);
		if (ValidPlant)
		{
			base.CreateOrder(entityTag);
		}
		else
		{
			SetPreview(Tag.Invalid, false);
		}
	}

	private void SyncPriority(PrioritySetting priority)
	{
		Prioritizable component = GetComponent<Prioritizable>();
		if (!object.Equals(component.GetMasterPriority(), priority))
		{
			component.SetMasterPriority(priority);
		}
		if ((UnityEngine.Object)base.occupyingObject != (UnityEngine.Object)null)
		{
			Prioritizable component2 = base.occupyingObject.GetComponent<Prioritizable>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && !object.Equals(component2.GetMasterPriority(), priority))
			{
				component2.SetMasterPriority(component.GetMasterPriority());
			}
		}
	}

	protected override void OnSpawn()
	{
		if ((UnityEngine.Object)plant != (UnityEngine.Object)null)
		{
			RegisterWithPlant(plant.gameObject);
		}
		base.OnSpawn();
		autoReplaceEntity = false;
		Components.PlantablePlots.Add(this);
		Prioritizable component = GetComponent<Prioritizable>();
		Prioritizable prioritizable = component;
		prioritizable.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(prioritizable.onPriorityChanged, new Action<PrioritySetting>(SyncPriority));
	}

	public void SetFertilizationFlags(bool fertilizer, bool liquid_piping)
	{
		accepts_fertilizer = fertilizer;
		has_liquid_pipe_input = liquid_piping;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if ((UnityEngine.Object)plantPreview != (UnityEngine.Object)null)
		{
			Util.KDestroyGameObject(plantPreview.gameObject);
		}
		if ((bool)base.occupyingObject)
		{
			base.occupyingObject.Trigger(-216549700, null);
		}
		Components.PlantablePlots.Remove(this);
	}

	public override GameObject SpawnOccupyingObject(GameObject depositedEntity)
	{
		PlantableSeed component = depositedEntity.GetComponent<PlantableSeed>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			Debug.LogError("Planted seed " + depositedEntity.gameObject.name + " is missing PlantableSeed component");
			return null;
		}
		Vector3 position = Grid.CellToPosCBC(Grid.PosToCell(this), plantLayer);
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(component.PlantID), position, plantLayer, null, 0);
		gameObject.SetActive(true);
		KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
		plantRef.Set(component2);
		RegisterWithPlant(gameObject);
		UprootedMonitor component3 = gameObject.GetComponent<UprootedMonitor>();
		if ((bool)component3)
		{
			component3.canBeUprooted = false;
		}
		autoReplaceEntity = false;
		Prioritizable component4 = GetComponent<Prioritizable>();
		if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
		{
			Prioritizable component5 = gameObject.GetComponent<Prioritizable>();
			if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
			{
				component5.SetMasterPriority(component4.GetMasterPriority());
				Prioritizable prioritizable = component5;
				prioritizable.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(prioritizable.onPriorityChanged, new Action<PrioritySetting>(SyncPriority));
			}
		}
		return gameObject;
	}

	protected override void PositionOccupyingObject()
	{
		base.PositionOccupyingObject();
		KBatchedAnimController component = base.occupyingObject.GetComponent<KBatchedAnimController>();
		component.SetSceneLayer(plantLayer);
		OffsetAnim(component, occupyingObjectVisualOffset);
	}

	private void RegisterWithPlant(GameObject plant)
	{
		base.occupyingObject = plant;
		plant.Trigger(1309017699, storage);
		ReceptacleMonitor component = plant.GetComponent<ReceptacleMonitor>();
		if ((bool)component)
		{
			component.smi.sm.receptacle.Set(this, component.smi);
		}
	}

	protected override void SubscribeToOccupant()
	{
		base.SubscribeToOccupant();
		if ((UnityEngine.Object)base.occupyingObject != (UnityEngine.Object)null)
		{
			Subscribe(base.occupyingObject, -216549700, OnOccupantUprooted);
		}
	}

	protected override void UnsubscribeFromOccupant()
	{
		base.UnsubscribeFromOccupant();
		if ((UnityEngine.Object)base.occupyingObject != (UnityEngine.Object)null)
		{
			Unsubscribe(base.occupyingObject, -216549700, OnOccupantUprooted);
		}
	}

	private void OnOccupantUprooted(object data)
	{
		autoReplaceEntity = false;
		requestedEntityTag = Tag.Invalid;
	}

	public override void OrderRemoveOccupant()
	{
		if (!((UnityEngine.Object)base.Occupant == (UnityEngine.Object)null))
		{
			Uprootable component = base.Occupant.GetComponent<Uprootable>();
			if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
			{
				component.MarkForUproot();
			}
		}
	}

	public override void SetPreview(Tag entityTag, bool solid = false)
	{
		PlantableSeed plantableSeed = null;
		if (entityTag.IsValid)
		{
			GameObject prefab = Assets.GetPrefab(entityTag);
			if ((UnityEngine.Object)prefab == (UnityEngine.Object)null)
			{
				DebugUtil.LogWarningArgs(base.gameObject, "Planter tried previewing a tag with no asset! If this was the 'Empty' tag, ignore it, that will go away in new save games. Otherwise... Eh? Tag was: ", entityTag);
				return;
			}
			plantableSeed = prefab.GetComponent<PlantableSeed>();
		}
		if ((UnityEngine.Object)plantPreview != (UnityEngine.Object)null)
		{
			KPrefabID component = plantPreview.GetComponent<KPrefabID>();
			if ((UnityEngine.Object)plantableSeed != (UnityEngine.Object)null && (UnityEngine.Object)component != (UnityEngine.Object)null && component.PrefabTag == plantableSeed.PreviewID)
			{
				return;
			}
			plantPreview.gameObject.Unsubscribe(-1820564715, OnValidChanged);
			Util.KDestroyGameObject(plantPreview.gameObject);
		}
		if ((UnityEngine.Object)plantableSeed != (UnityEngine.Object)null)
		{
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(plantableSeed.PreviewID), Grid.SceneLayer.Front, null, 0);
			plantPreview = gameObject.GetComponent<EntityPreview>();
			gameObject.transform.SetPosition(Vector3.zero);
			gameObject.transform.SetParent(base.gameObject.transform, false);
			gameObject.transform.SetLocalPosition(Vector3.zero);
			if ((UnityEngine.Object)rotatable != (UnityEngine.Object)null)
			{
				if (plantableSeed.direction == ReceptacleDirection.Top)
				{
					gameObject.transform.SetLocalPosition(occupyingObjectRelativePosition);
				}
				else if (plantableSeed.direction == ReceptacleDirection.Side)
				{
					gameObject.transform.SetLocalPosition(Rotatable.GetRotatedOffset(occupyingObjectRelativePosition, Orientation.R90));
				}
				else
				{
					gameObject.transform.SetLocalPosition(Rotatable.GetRotatedOffset(occupyingObjectRelativePosition, Orientation.R180));
				}
			}
			else
			{
				gameObject.transform.SetLocalPosition(occupyingObjectRelativePosition);
			}
			KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
			OffsetAnim(component2, occupyingObjectVisualOffset);
			gameObject.SetActive(true);
			gameObject.Subscribe(-1820564715, OnValidChanged);
			if (solid)
			{
				plantPreview.SetSolid();
			}
			plantPreview.UpdateValidity();
		}
	}

	private void OffsetAnim(KBatchedAnimController kanim, Vector3 offset)
	{
		if ((UnityEngine.Object)rotatable != (UnityEngine.Object)null)
		{
			offset = rotatable.GetRotatedOffset(offset);
		}
		kanim.Offset = offset;
	}

	private void OnValidChanged(object obj)
	{
		Trigger(-1820564715, obj);
		if (!plantPreview.Valid && base.GetActiveRequest != null)
		{
			CancelActiveRequest();
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return GetDescriptors(def.BuildingComplete);
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.ENABLESDOMESTICGROWTH, UI.BUILDINGEFFECTS.TOOLTIPS.ENABLESDOMESTICGROWTH, Descriptor.DescriptorType.Effect);
		list.Add(item);
		return list;
	}
}
