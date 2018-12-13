using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Storage : Workable, ISaveLoadableDetails, IEffectDescriptor
{
	public enum StoredItemModifier
	{
		Insulate,
		Hide,
		Seal,
		Preserve
	}

	public enum FetchCategory
	{
		Building,
		GeneralStorage,
		StorageSweepOnly
	}

	public enum FXPrefix
	{
		Delivered,
		PickedUp
	}

	private struct StoredItemModifierInfo
	{
		public StoredItemModifier modifier;

		public Action<GameObject, bool, bool> toggleState;

		public StoredItemModifierInfo(StoredItemModifier modifier, Action<GameObject, bool, bool> toggle_state)
		{
			this.modifier = modifier;
			toggleState = toggle_state;
		}
	}

	public bool allowItemRemoval;

	public bool onlyTransferFromLowerPriority;

	public bool allowSublimation = true;

	public float capacityKg = 20000f;

	public bool showInUI = true;

	public bool showDescriptor;

	public bool allowUIItemRemoval;

	public bool doDiseaseTransfer = true;

	public List<Tag> storageFilters;

	public bool useGunForDelivery = true;

	public bool sendOnStoreOnSpawn;

	public FetchCategory fetchCategory;

	public int storageNetworkID = -1;

	public float storageFullMargin;

	public FXPrefix fxPrefix;

	public List<GameObject> items = new List<GameObject>();

	[MyCmpGet]
	public Prioritizable prioritizable;

	[MyCmpGet]
	public Automatable automatable;

	[MyCmpGet]
	protected PrimaryElement primaryElement;

	public bool dropOnLoad;

	protected float maxKGPerItem = 3.40282347E+38f;

	private bool endOfLife;

	public bool allowSettingOnlyFetchMarkedItems = true;

	[Serialize]
	private bool onlyFetchMarkedItems;

	private static readonly List<StoredItemModifierInfo> StoredItemModifierHandlers = new List<StoredItemModifierInfo>
	{
		new StoredItemModifierInfo(StoredItemModifier.Hide, MakeItemInvisible),
		new StoredItemModifierInfo(StoredItemModifier.Insulate, MakeItemTemperatureInsulated),
		new StoredItemModifierInfo(StoredItemModifier.Seal, MakeItemSealed),
		new StoredItemModifierInfo(StoredItemModifier.Preserve, MakeItemPreserved)
	};

	[SerializeField]
	private List<StoredItemModifier> defaultStoredItemModifers = new List<StoredItemModifier>
	{
		StoredItemModifier.Hide
	};

	public static readonly List<StoredItemModifier> StandardSealedStorage = new List<StoredItemModifier>
	{
		StoredItemModifier.Hide,
		StoredItemModifier.Seal
	};

	public static readonly List<StoredItemModifier> StandardFabricatorStorage = new List<StoredItemModifier>
	{
		StoredItemModifier.Hide,
		StoredItemModifier.Preserve
	};

	private static readonly EventSystem.IntraObjectHandler<Storage> OnDeathDelegate = new EventSystem.IntraObjectHandler<Storage>(delegate(Storage component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Storage> OnQueueDestroyObjectDelegate = new EventSystem.IntraObjectHandler<Storage>(delegate(Storage component, object data)
	{
		component.OnQueueDestroyObject(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Storage> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Storage>(delegate(Storage component, object data)
	{
		component.OnCopySettings(data);
	});

	[CompilerGenerated]
	private static Action<GameObject, bool, bool> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<GameObject, bool, bool> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Action<GameObject, bool, bool> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Action<GameObject, bool, bool> _003C_003Ef__mg_0024cache3;

	public bool ShouldOnlyTransferFromLowerPriority => onlyTransferFromLowerPriority || allowItemRemoval;

	public GameObject this[int idx]
	{
		get
		{
			return items[idx];
		}
	}

	public int Count => items.Count;

	public int masterPriority
	{
		get
		{
			int result;
			if ((UnityEngine.Object)prioritizable != (UnityEngine.Object)null)
			{
				PrioritySetting masterPriority = prioritizable.GetMasterPriority();
				result = masterPriority.priority_value;
			}
			else
			{
				result = 10;
			}
			return result;
		}
	}

	public event System.Action OnStorageIncreased;

	protected Storage()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		showProgressBar = false;
		faceTargetWhenWorking = true;
	}

	public void SetDefaultStoredItemModifiers(List<StoredItemModifier> modifiers)
	{
		defaultStoredItemModifers = modifiers;
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		if (useGunForDelivery && worker.usesMultiTool)
		{
			AnimInfo anim = base.GetAnim(worker);
			anim.smi = new MultitoolController.Instance(this, worker, "store", Assets.GetPrefab(EffectConfigs.OreAbsorbId));
			return anim;
		}
		return base.GetAnim(worker);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(1623392196, OnDeathDelegate);
		Subscribe(1502190696, OnQueueDestroyObjectDelegate);
		Subscribe(-905833192, OnCopySettingsDelegate);
		workerStatusItem = Db.Get().DuplicantStatusItems.Storing;
		resetProgressOnStop = true;
		synchronizeAnims = false;
		workingPstComplete = HashedString.Invalid;
		workingPstFailed = HashedString.Invalid;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (!allowSettingOnlyFetchMarkedItems)
		{
			onlyFetchMarkedItems = false;
		}
		UpdateFetchCategory();
	}

	protected override void OnSpawn()
	{
		SetWorkTime(1.5f);
		foreach (GameObject item in items)
		{
			ApplyStoredItemModifiers(item, true, true);
			if (sendOnStoreOnSpawn)
			{
				item.Trigger(856640610, this);
			}
		}
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetSymbolVisiblity("sweep", onlyFetchMarkedItems);
		}
		Prioritizable component2 = GetComponent<Prioritizable>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			Prioritizable obj = component2;
			obj.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(obj.onPriorityChanged, new Action<PrioritySetting>(OnPriorityChanged));
		}
		UpdateFetchCategory();
	}

	public GameObject Store(GameObject go, bool hide_popups = false, bool block_events = false, bool do_disease_transfer = true, bool is_deserializing = false)
	{
		if ((UnityEngine.Object)go == (UnityEngine.Object)null)
		{
			return null;
		}
		GameObject result = go;
		Pickupable component = go.GetComponent<Pickupable>();
		if (!hide_popups && (UnityEngine.Object)PopFXManager.Instance != (UnityEngine.Object)null)
		{
			LocString loc_string;
			Transform transform;
			if (fxPrefix == FXPrefix.Delivered)
			{
				loc_string = UI.DELIVERED;
				transform = base.transform;
			}
			else
			{
				loc_string = UI.PICKEDUP;
				transform = go.transform;
			}
			string text = Assets.IsTagCountable(go.PrefabID()) ? string.Format(loc_string, (int)component.TotalAmount, go.GetProperName()) : string.Format(loc_string, GameUtil.GetFormattedMass(component.TotalAmount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), go.GetProperName());
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, text, transform, 1.5f, false);
		}
		go.transform.parent = base.transform;
		Vector3 position = Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Move);
		Vector3 position2 = go.transform.GetPosition();
		position.z = position2.z;
		go.transform.SetPosition(position);
		if (!block_events && do_disease_transfer)
		{
			TransferDiseaseWithObject(go);
		}
		if (!is_deserializing)
		{
			foreach (GameObject item in items)
			{
				if ((UnityEngine.Object)item != (UnityEngine.Object)null && (UnityEngine.Object)component != (UnityEngine.Object)null && item.GetComponent<Pickupable>().TryAbsorb(component, hide_popups, true))
				{
					if (!block_events)
					{
						Trigger(-1697596308, go);
						Trigger(-778359855, null);
						if (this.OnStorageIncreased != null)
						{
							this.OnStorageIncreased();
						}
					}
					ApplyStoredItemModifiers(go, true, false);
					result = item;
					go = null;
					break;
				}
			}
		}
		if ((UnityEngine.Object)go != (UnityEngine.Object)null)
		{
			items.Add(go);
			if (!is_deserializing)
			{
				ApplyStoredItemModifiers(go, true, false);
			}
			if (!block_events)
			{
				go.Trigger(856640610, this);
				Trigger(-1697596308, go);
				Trigger(-778359855, null);
				if (this.OnStorageIncreased != null)
				{
					this.OnStorageIncreased();
				}
			}
		}
		return result;
	}

	public PrimaryElement AddLiquid(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, bool keep_zero_mass = false, bool do_disease_transfer = true)
	{
		if (mass <= 0f)
		{
			return null;
		}
		PrimaryElement primaryElement = FindPrimaryElement(element);
		if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
		{
			float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, mass);
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			primaryElement.Mass += mass;
			primaryElement.Temperature = finalTemperature;
			primaryElement.AddDisease(disease_idx, disease_count, "Storage.AddLiquid");
			Trigger(-1697596308, primaryElement.gameObject);
		}
		else
		{
			SubstanceChunk substanceChunk = LiquidSourceManager.Instance.CreateChunk(element, mass, temperature, disease_idx, disease_count, base.transform.GetPosition());
			primaryElement = substanceChunk.GetComponent<PrimaryElement>();
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			GameObject gameObject = substanceChunk.gameObject;
			bool hide_popups = true;
			bool do_disease_transfer2 = do_disease_transfer;
			Store(gameObject, hide_popups, false, do_disease_transfer2, false);
		}
		return primaryElement;
	}

	public PrimaryElement AddGasChunk(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, bool keep_zero_mass, bool do_disease_transfer = true)
	{
		if (mass <= 0f)
		{
			return null;
		}
		PrimaryElement primaryElement = FindPrimaryElement(element);
		if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
		{
			float mass2 = primaryElement.Mass;
			float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, mass2, temperature, mass);
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			primaryElement.SetMassTemperature(mass2 + mass, finalTemperature);
			primaryElement.AddDisease(disease_idx, disease_count, "Storage.AddGasChunk");
			Trigger(-1697596308, primaryElement.gameObject);
		}
		else
		{
			SubstanceChunk substanceChunk = GasSourceManager.Instance.CreateChunk(element, mass, temperature, disease_idx, disease_count, base.transform.GetPosition());
			primaryElement = substanceChunk.GetComponent<PrimaryElement>();
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			GameObject gameObject = substanceChunk.gameObject;
			bool hide_popups = true;
			bool do_disease_transfer2 = do_disease_transfer;
			Store(gameObject, hide_popups, false, do_disease_transfer2, false);
		}
		return primaryElement;
	}

	public void Transfer(Storage target, bool block_events = false, bool hide_popups = false)
	{
		while (items.Count > 0)
		{
			Transfer(items[0], target, block_events, hide_popups);
		}
	}

	public float Transfer(Storage dest_storage, Tag tag, float amount, bool block_events = false, bool hide_popups = false)
	{
		GameObject gameObject = FindFirst(tag);
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (amount < component.Units)
			{
				Pickupable component2 = gameObject.GetComponent<Pickupable>();
				Pickupable pickupable = component2.Take(amount);
				dest_storage.Store(pickupable.gameObject, hide_popups, block_events, true, false);
				if (!block_events)
				{
					Trigger(-1697596308, component2.gameObject);
				}
			}
			else
			{
				Transfer(gameObject, dest_storage, block_events, hide_popups);
			}
			return amount;
		}
		return 0f;
	}

	public bool Transfer(GameObject go, Storage target, bool block_events = false, bool hide_popups = false)
	{
		items.RemoveAll((GameObject it) => (UnityEngine.Object)it == (UnityEngine.Object)null);
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			if ((UnityEngine.Object)items[i] == (UnityEngine.Object)go)
			{
				items.RemoveAt(i);
				ApplyStoredItemModifiers(go, false, false);
				target.Store(go, hide_popups, block_events, true, false);
				if (!block_events)
				{
					Trigger(-1697596308, go);
				}
				return true;
			}
		}
		return false;
	}

	public void DropAll(bool empty_containers = false)
	{
		while (items.Count > 0)
		{
			GameObject gameObject = items[0];
			TransferDiseaseWithObject(gameObject);
			items.RemoveAt(0);
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				gameObject.Trigger(1228788923, this);
				bool flag = false;
				if (empty_containers)
				{
					Dumpable component = gameObject.GetComponent<Dumpable>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null && gameObject.GetComponent<PrimaryElement>().Element.IsGas)
					{
						component.Dump();
						flag = true;
					}
				}
				if (!flag)
				{
					Vector3 position = Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore);
					gameObject.transform.SetPosition(position);
					KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
					if ((bool)component2)
					{
						component2.SetSceneLayer(Grid.SceneLayer.Ore);
					}
					MakeWorldActive(gameObject);
				}
			}
		}
	}

	public List<GameObject> Drop(Tag t)
	{
		ListPool<GameObject, Storage>.PooledList pooledList = ListPool<GameObject, Storage>.Allocate();
		Find(t, pooledList);
		foreach (GameObject item in pooledList)
		{
			Drop(item);
		}
		pooledList.Recycle();
		return pooledList;
	}

	public GameObject Drop(GameObject go)
	{
		if ((UnityEngine.Object)go != (UnityEngine.Object)null)
		{
			int count = items.Count;
			for (int i = 0; i < count; i++)
			{
				if ((UnityEngine.Object)go == (UnityEngine.Object)items[i])
				{
					items[i] = items[count - 1];
					items.RemoveAt(count - 1);
					TransferDiseaseWithObject(go);
					go.Trigger(1228788923, this);
					MakeWorldActive(go);
					break;
				}
			}
		}
		return go;
	}

	public void RenotifyAll()
	{
		items.RemoveAll((GameObject it) => (UnityEngine.Object)it == (UnityEngine.Object)null);
		foreach (GameObject item in items)
		{
			item.Trigger(856640610, this);
		}
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole("Hauler", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
		resume.AddExperienceIfRole(MaterialsManager.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
	}

	private void TransferDiseaseWithObject(GameObject obj)
	{
		if (!((UnityEngine.Object)obj == (UnityEngine.Object)null) && doDiseaseTransfer && !((UnityEngine.Object)primaryElement == (UnityEngine.Object)null))
		{
			PrimaryElement component = obj.GetComponent<PrimaryElement>();
			if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
			{
				SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
				invalid.idx = component.DiseaseIdx;
				invalid.count = (int)((float)component.DiseaseCount * 0.05f);
				SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid;
				invalid2.idx = primaryElement.DiseaseIdx;
				invalid2.count = (int)((float)primaryElement.DiseaseCount * 0.05f);
				component.ModifyDiseaseCount(-invalid.count, "Storage.TransferDiseaseWithObject");
				primaryElement.ModifyDiseaseCount(-invalid2.count, "Storage.TransferDiseaseWithObject");
				if (invalid.count > 0)
				{
					primaryElement.AddDisease(invalid.idx, invalid.count, "Storage.TransferDiseaseWithObject");
				}
				if (invalid2.count > 0)
				{
					component.AddDisease(invalid2.idx, invalid2.count, "Storage.TransferDiseaseWithObject");
				}
			}
		}
	}

	private void MakeWorldActive(GameObject go)
	{
		go.transform.parent = null;
		Trigger(-1697596308, go);
		go.Trigger(856640610, null);
		ApplyStoredItemModifiers(go, false, false);
		if ((UnityEngine.Object)go != (UnityEngine.Object)null)
		{
			PrimaryElement component = go.GetComponent<PrimaryElement>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.KeepZeroMassObject)
			{
				component.KeepZeroMassObject = false;
				if (component.Mass <= 0f)
				{
					Util.KDestroyGameObject(go);
				}
			}
		}
	}

	public List<GameObject> Find(Tag tag, List<GameObject> result)
	{
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null) && gameObject.HasTag(tag))
			{
				result.Add(gameObject);
			}
		}
		return result;
	}

	public GameObject FindFirst(Tag tag)
	{
		GameObject result = null;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null) && gameObject.HasTag(tag))
			{
				result = gameObject;
				break;
			}
		}
		return result;
	}

	public PrimaryElement FindFirstWithMass(Tag tag)
	{
		PrimaryElement result = null;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null) && gameObject.HasTag(tag))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (component.Mass > 0f)
				{
					result = component;
					break;
				}
			}
		}
		return result;
	}

	public List<Tag> GetAllTagsInStorage()
	{
		List<Tag> list = new List<Tag>();
		for (int i = 0; i < items.Count; i++)
		{
			GameObject go = items[i];
			if (!list.Contains(go.PrefabID()))
			{
				list.Add(go.PrefabID());
			}
		}
		return list;
	}

	public GameObject Find(int ID)
	{
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (ID == gameObject.PrefabID().GetHashCode())
			{
				return gameObject;
			}
		}
		return null;
	}

	public void ConsumeAllIgnoringDisease()
	{
		for (int num = items.Count - 1; num >= 0; num--)
		{
			ConsumeIgnoringDisease(items[num]);
		}
	}

	public void ConsumeAndGetDisease(Tag tag, float amount, out SimUtil.DiseaseInfo disease_info, out float aggregate_temperature)
	{
		DebugUtil.Assert(tag.IsValid);
		disease_info = SimUtil.DiseaseInfo.Invalid;
		List<GameObject> list = null;
		aggregate_temperature = 0f;
		float num = 0f;
		bool flag = false;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null) && gameObject.HasTag(tag))
			{
				flag = true;
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				float num2 = Math.Min(component.Units, amount);
				aggregate_temperature = SimUtil.CalculateFinalTemperature(num, aggregate_temperature, num2, component.Temperature);
				SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(component, num2 / component.Units);
				disease_info = SimUtil.CalculateFinalDiseaseInfo(disease_info, percentOfDisease);
				component.Units -= num2;
				component.ModifyDiseaseCount(-percentOfDisease.count, "Storage.ConsumeAndGetDisease");
				if (component.Units <= 0f && !component.KeepZeroMassObject)
				{
					if (list == null)
					{
						list = new List<GameObject>();
					}
					list.Add(gameObject);
				}
				amount -= num2;
				num += num2;
				Trigger(-1697596308, gameObject);
				if (amount <= 0f)
				{
					break;
				}
			}
		}
		if (!flag)
		{
			aggregate_temperature = GetComponent<PrimaryElement>().Temperature;
		}
		if (list != null)
		{
			for (int j = 0; j < list.Count; j++)
			{
				items.Remove(list[j]);
				Util.KDestroyGameObject(list[j]);
			}
		}
	}

	public void ConsumeAndGetDisease(Recipe.Ingredient ingredient, out SimUtil.DiseaseInfo disease_info, out float temperature)
	{
		ConsumeAndGetDisease(ingredient.tag, ingredient.amount, out disease_info, out temperature);
	}

	public void ConsumeIgnoringDisease(Tag tag, float amount)
	{
		ConsumeAndGetDisease(tag, amount, out SimUtil.DiseaseInfo _, out float _);
	}

	public void ConsumeIgnoringDisease(GameObject item_go)
	{
		if (items.Contains(item_go))
		{
			PrimaryElement component = item_go.GetComponent<PrimaryElement>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.KeepZeroMassObject)
			{
				component.Units = 0f;
				component.ModifyDiseaseCount(-component.DiseaseCount, "consume item");
				Trigger(-1697596308, item_go);
			}
			else
			{
				items.Remove(item_go);
				Trigger(-1697596308, item_go);
				item_go.DeleteObject();
			}
		}
	}

	public GameObject Drop(int ID)
	{
		return Drop(Find(ID));
	}

	private void OnDeath(object data)
	{
		DropAll(true);
	}

	public bool IsFull()
	{
		return RemainingCapacity() <= 0f;
	}

	public bool IsEmpty()
	{
		return items.Count == 0;
	}

	public float Capacity()
	{
		return capacityKg;
	}

	public bool IsEndOfLife()
	{
		return endOfLife;
	}

	public float MassStored()
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			if (!((UnityEngine.Object)items[i] == (UnityEngine.Object)null))
			{
				PrimaryElement component = items[i].GetComponent<PrimaryElement>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					num += component.Units * component.MassPerUnit;
				}
			}
		}
		return (float)Mathf.RoundToInt(num * 1000f) / 1000f;
	}

	public float UnitsStored()
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			if (!((UnityEngine.Object)items[i] == (UnityEngine.Object)null))
			{
				PrimaryElement component = items[i].GetComponent<PrimaryElement>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					num += component.Units;
				}
			}
		}
		return (float)Mathf.RoundToInt(num * 1000f) / 1000f;
	}

	public bool Has(Tag tag)
	{
		bool result = false;
		foreach (GameObject item in items)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			if (component.HasTag(tag) && component.Mass > 0f)
			{
				return true;
			}
		}
		return result;
	}

	public PrimaryElement AddToPrimaryElement(SimHashes element, float additional_mass, float temperature)
	{
		PrimaryElement primaryElement = FindPrimaryElement(element);
		if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
		{
			float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, additional_mass);
			primaryElement.Mass += additional_mass;
			primaryElement.Temperature = finalTemperature;
		}
		return primaryElement;
	}

	public PrimaryElement FindPrimaryElement(SimHashes element)
	{
		PrimaryElement result = null;
		foreach (GameObject item in items)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (component.ElementID == element)
				{
					return component;
				}
			}
		}
		return result;
	}

	public float RemainingCapacity()
	{
		return capacityKg - MassStored();
	}

	public bool GetOnlyFetchMarkedItems()
	{
		return onlyFetchMarkedItems;
	}

	public void SetOnlyFetchMarkedItems(bool is_set)
	{
		if (is_set != onlyFetchMarkedItems)
		{
			onlyFetchMarkedItems = is_set;
			UpdateFetchCategory();
			Trigger(644822890, null);
			GetComponent<KBatchedAnimController>().SetSymbolVisiblity("sweep", is_set);
		}
	}

	private void UpdateFetchCategory()
	{
		if (fetchCategory != 0)
		{
			fetchCategory = ((!onlyFetchMarkedItems) ? FetchCategory.GeneralStorage : FetchCategory.StorageSweepOnly);
		}
	}

	protected override void OnCleanUp()
	{
		if (items.Count != 0)
		{
			Debug.LogWarning("Storage for [" + base.gameObject.name + "] is being destroyed but it still contains items!", base.gameObject);
		}
		base.OnCleanUp();
	}

	private void OnQueueDestroyObject(object data)
	{
		endOfLife = true;
		DropAll(true);
		OnCleanUp();
	}

	public void Remove(GameObject go)
	{
		items.Remove(go);
		TransferDiseaseWithObject(go);
		Trigger(-1697596308, go);
		ApplyStoredItemModifiers(go, false, false);
	}

	public float GetAmountAvailable(Tag tag)
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && gameObject.HasTag(tag))
			{
				num += gameObject.GetComponent<PrimaryElement>().Units;
			}
		}
		return num;
	}

	public float GetUnitsAvailable(Tag tag)
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && gameObject.HasTag(tag))
			{
				num += gameObject.GetComponent<PrimaryElement>().Units;
			}
		}
		return num;
	}

	public float GetMassAvailable(Tag tag)
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && gameObject.HasTag(tag))
			{
				num += gameObject.GetComponent<PrimaryElement>().Mass;
			}
		}
		return num;
	}

	public float GetMassAvailable(SimHashes element)
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (component.ElementID == element)
				{
					num += component.Mass;
				}
			}
		}
		return num;
	}

	public bool IsMaterialOnStorage(Tag tag, ref float amount)
	{
		foreach (GameObject item in items)
		{
			if ((UnityEngine.Object)item != (UnityEngine.Object)null)
			{
				Pickupable component = item.GetComponent<Pickupable>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					KPrefabID component2 = component.GetComponent<KPrefabID>();
					if (component2.HasTag(tag))
					{
						amount = component.TotalAmount;
						return true;
					}
				}
			}
		}
		return false;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (showDescriptor)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.STORAGECAPACITY, GameUtil.GetFormattedMass(Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STORAGECAPACITY, GameUtil.GetFormattedMass(Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Effect);
			list.Add(item);
		}
		return list;
	}

	private static void MakeItemTemperatureInsulated(GameObject go, bool is_stored, bool is_initializing)
	{
		SimTemperatureTransfer component = go.GetComponent<SimTemperatureTransfer>();
		if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
		{
			component.enabled = !is_stored;
		}
	}

	private static void MakeItemInvisible(GameObject go, bool is_stored, bool is_initializing)
	{
		if (!is_initializing)
		{
			bool flag = !is_stored;
			KAnimControllerBase component = go.GetComponent<KAnimControllerBase>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.enabled != flag)
			{
				component.enabled = flag;
			}
			KSelectable component2 = go.GetComponent<KSelectable>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.enabled != flag)
			{
				component2.enabled = flag;
			}
		}
	}

	private static void MakeItemSealed(GameObject go, bool is_stored, bool is_initializing)
	{
		if ((UnityEngine.Object)go != (UnityEngine.Object)null)
		{
			if (is_stored)
			{
				go.GetComponent<KPrefabID>().AddTag(GameTags.Sealed);
			}
			else
			{
				go.GetComponent<KPrefabID>().RemoveTag(GameTags.Sealed);
			}
		}
	}

	private static void MakeItemPreserved(GameObject go, bool is_stored, bool is_initializing)
	{
		if ((UnityEngine.Object)go != (UnityEngine.Object)null)
		{
			if (is_stored)
			{
				go.GetComponent<KPrefabID>().AddTag(GameTags.Preserved);
			}
			else
			{
				go.GetComponent<KPrefabID>().RemoveTag(GameTags.Preserved);
			}
		}
	}

	private void ApplyStoredItemModifiers(GameObject go, bool is_stored, bool is_initializing)
	{
		List<StoredItemModifier> list = defaultStoredItemModifers;
		for (int i = 0; i < list.Count; i++)
		{
			StoredItemModifier storedItemModifier = list[i];
			for (int j = 0; j < StoredItemModifierHandlers.Count; j++)
			{
				StoredItemModifierInfo storedItemModifierInfo = StoredItemModifierHandlers[j];
				if (storedItemModifierInfo.modifier == storedItemModifier)
				{
					storedItemModifierInfo.toggleState(go, is_stored, is_initializing);
					break;
				}
			}
		}
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		Storage component = gameObject.GetComponent<Storage>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			SetOnlyFetchMarkedItems(component.onlyFetchMarkedItems);
		}
	}

	private void OnPriorityChanged(PrioritySetting priority)
	{
		foreach (GameObject item in items)
		{
			item.Trigger(-1626373771, this);
		}
	}

	private bool ShouldSaveItem(GameObject go)
	{
		bool result = false;
		if ((UnityEngine.Object)go != (UnityEngine.Object)null && (UnityEngine.Object)go.GetComponent<SaveLoadRoot>() != (UnityEngine.Object)null)
		{
			PrimaryElement component = go.GetComponent<PrimaryElement>();
			if (component.Mass > 0f)
			{
				result = true;
			}
		}
		return result;
	}

	public void Serialize(BinaryWriter writer)
	{
		int num = 0;
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			if (ShouldSaveItem(items[i]))
			{
				num++;
			}
		}
		writer.Write(num);
		if (num != 0 && items != null && items.Count > 0)
		{
			for (int j = 0; j < items.Count; j++)
			{
				GameObject gameObject = items[j];
				if (ShouldSaveItem(gameObject))
				{
					SaveLoadRoot component = gameObject.GetComponent<SaveLoadRoot>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						string name = gameObject.GetComponent<KPrefabID>().GetSaveLoadTag().Name;
						writer.WriteKleiString(name);
						component.Save(writer);
					}
					else
					{
						Output.LogWithObj(gameObject, "Tried to save obj in storage but obj has no SaveLoadRoot");
					}
				}
			}
		}
	}

	public void Deserialize(IReader reader)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		ClearItems();
		int num4 = reader.ReadInt32();
		items = new List<GameObject>(num4);
		for (int i = 0; i < num4; i++)
		{
			float realtimeSinceStartup2 = Time.realtimeSinceStartup;
			string tag_string = reader.ReadKleiString();
			Tag tag = TagManager.Create(tag_string);
			SaveLoadRoot saveLoadRoot = SaveLoadRoot.Load(tag, reader);
			num += Time.realtimeSinceStartup - realtimeSinceStartup2;
			if ((UnityEngine.Object)saveLoadRoot != (UnityEngine.Object)null)
			{
				KBatchedAnimController component = saveLoadRoot.GetComponent<KBatchedAnimController>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					component.enabled = false;
				}
				saveLoadRoot.SetRegistered(false);
				float realtimeSinceStartup3 = Time.realtimeSinceStartup;
				GameObject gameObject = Store(saveLoadRoot.gameObject, true, true, false, true);
				num2 += Time.realtimeSinceStartup - realtimeSinceStartup3;
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					float realtimeSinceStartup4 = Time.realtimeSinceStartup;
					gameObject.GetComponent<Pickupable>().OnStore(this);
					num3 += Time.realtimeSinceStartup - realtimeSinceStartup4;
					if (dropOnLoad)
					{
						Drop(saveLoadRoot.gameObject);
					}
				}
			}
			else
			{
				Output.LogWarningWithObj(base.gameObject, "Tried to deserialize " + tag.ToString() + " into storage but failed");
			}
		}
	}

	private void ClearItems()
	{
		foreach (GameObject item in items)
		{
			item.DeleteObject();
		}
		items.Clear();
	}
}
