using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class WorldInventory : KMonoBehaviour, ISaveLoadable
{
	[Serialize]
	private HashSet<Tag> Discovered = new HashSet<Tag>();

	[Serialize]
	private Dictionary<Tag, HashSet<Tag>> DiscoveredCategories = new Dictionary<Tag, HashSet<Tag>>();

	private Dictionary<Tag, List<Pickupable>> Inventory = new Dictionary<Tag, List<Pickupable>>();

	private MinionGroupProber Prober;

	private Dictionary<Tag, float> accessibleAmounts = new Dictionary<Tag, float>();

	private int accessibleUpdateIndex;

	private bool firstUpdate = true;

	public static WorldInventory Instance
	{
		get;
		private set;
	}

	public event Action<Tag, Tag> OnDiscover;

	protected override void OnPrefabInit()
	{
		Instance = this;
		Subscribe(Game.Instance.gameObject, -1588644844, OnAddedFetchable);
		Subscribe(Game.Instance.gameObject, -1491270284, OnRemovedFetchable);
		GameClock.Instance.Subscribe(631075836, GenerateInventoryReport);
	}

	private void GenerateInventoryReport(object data)
	{
		int num = 0;
		int num2 = 0;
		IEnumerator enumerator = Components.Brains.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				CreatureBrain creatureBrain = current as CreatureBrain;
				if ((UnityEngine.Object)creatureBrain != (UnityEngine.Object)null)
				{
					if (creatureBrain.HasTag(GameTags.Creatures.Wild))
					{
						num++;
						ReportManager.Instance.ReportValue(ReportManager.ReportType.WildCritters, 1f, creatureBrain.GetProperName(), creatureBrain.GetProperName());
					}
					else
					{
						num2++;
						ReportManager.Instance.ReportValue(ReportManager.ReportType.DomesticatedCritters, 1f, creatureBrain.GetProperName(), creatureBrain.GetProperName());
					}
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		foreach (Spacecraft item in SpacecraftManager.instance.GetSpacecraft())
		{
			if (item.state != 0 && item.state != Spacecraft.MissionState.Destroyed)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.RocketsInFlight, 1f, item.rocketName, null);
			}
		}
	}

	protected override void OnSpawn()
	{
		Prober = MinionGroupProber.Get();
		StartCoroutine(InitialRefresh());
	}

	private IEnumerator InitialRefresh()
	{
		int i = 0;
		if (i < 1)
		{
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		for (int j = 0; j < Components.Pickupables.Count; j++)
		{
			Pickupable pickupable = Components.Pickupables[j];
			if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
			{
				pickupable.GetSMI<ReachabilityMonitor.Instance>()?.UpdateReachability();
			}
		}
	}

	public bool IsReachable(Pickupable pickupable)
	{
		return Prober.IsReachable(pickupable);
	}

	public float GetTotalAmount(Tag tag)
	{
		float value = 0f;
		accessibleAmounts.TryGetValue(tag, out value);
		return value;
	}

	public List<Pickupable> GetPickupables(Tag tag)
	{
		List<Pickupable> value = null;
		Inventory.TryGetValue(tag, out value);
		return value;
	}

	public List<Tag> GetPickupableTagsFromCategoryTag(Tag t)
	{
		List<Tag> list = new List<Tag>();
		List<Pickupable> pickupables = GetPickupables(t);
		if (pickupables != null && pickupables.Count > 0)
		{
			foreach (Pickupable item in pickupables)
			{
				list.AddRange(item.KPrefabID.Tags);
			}
			return list;
		}
		return list;
	}

	public float GetAmount(Tag tag)
	{
		float totalAmount = GetTotalAmount(tag);
		totalAmount -= MaterialNeeds.Instance.GetAmount(tag);
		return Mathf.Max(totalAmount, 0f);
	}

	public void Discover(Tag tag, Tag categoryTag)
	{
		bool flag = Discovered.Add(tag);
		DiscoverCategory(categoryTag, tag);
		if (flag && this.OnDiscover != null)
		{
			this.OnDiscover(categoryTag, tag);
		}
	}

	private void DiscoverCategory(Tag category_tag, Tag item_tag)
	{
		if (!DiscoveredCategories.TryGetValue(category_tag, out HashSet<Tag> value))
		{
			value = new HashSet<Tag>();
			DiscoveredCategories[category_tag] = value;
		}
		value.Add(item_tag);
	}

	public HashSet<Tag> GetDiscovered()
	{
		return Discovered;
	}

	public bool IsDiscovered(Tag tag)
	{
		return Discovered.Contains(tag) || DiscoveredCategories.ContainsKey(tag);
	}

	public bool AnyDiscovered(ICollection<Tag> tags)
	{
		foreach (Tag tag in tags)
		{
			if (IsDiscovered(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(Recipe.Ingredient[] ingredients)
	{
		bool result = true;
		foreach (Recipe.Ingredient ingredient in ingredients)
		{
			if (GetAmount(ingredient.tag) < ingredient.amount)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public bool TryGetDiscoveredResourcesFromTag(Tag tag, out HashSet<Tag> resources)
	{
		return DiscoveredCategories.TryGetValue(tag, out resources);
	}

	public HashSet<Tag> GetDiscoveredResourcesFromTag(Tag tag)
	{
		if (DiscoveredCategories.TryGetValue(tag, out HashSet<Tag> value))
		{
			return value;
		}
		return new HashSet<Tag>();
	}

	private void Update()
	{
		int num = 0;
		Dictionary<Tag, List<Pickupable>>.Enumerator enumerator = Inventory.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<Tag, List<Pickupable>> current = enumerator.Current;
			if (num == accessibleUpdateIndex || firstUpdate)
			{
				Tag key = current.Key;
				List<Pickupable> value = current.Value;
				float num2 = 0f;
				for (int i = 0; i < value.Count; i++)
				{
					Pickupable pickupable = value[i];
					if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null && !pickupable.HasTag(GameTags.StoredPrivate))
					{
						num2 += pickupable.TotalAmount;
					}
				}
				accessibleAmounts[key] = num2;
				accessibleUpdateIndex = (accessibleUpdateIndex + 1) % Inventory.Count;
				break;
			}
			num++;
		}
		firstUpdate = false;
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
		Instance = null;
	}

	public static Tag GetCategoryForTags(HashSet<Tag> tags)
	{
		Tag invalid = Tag.Invalid;
		foreach (Tag tag in tags)
		{
			if (GameTags.AllCategories.Contains(tag))
			{
				return tag;
			}
		}
		return invalid;
	}

	public static Tag GetCategoryForEntity(KPrefabID entity)
	{
		ElementChunk component = entity.GetComponent<ElementChunk>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			PrimaryElement component2 = component.GetComponent<PrimaryElement>();
			return component2.Element.materialCategory;
		}
		return GetCategoryForTags(entity.Tags);
	}

	private void OnAddedFetchable(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (!((UnityEngine.Object)gameObject.GetComponent<Health>() != (UnityEngine.Object)null))
		{
			Pickupable component = gameObject.GetComponent<Pickupable>();
			KPrefabID component2 = component.GetComponent<KPrefabID>();
			Tag tag = component2.PrefabID();
			if (!Inventory.ContainsKey(tag))
			{
				Tag categoryForEntity = GetCategoryForEntity(component2);
				DebugUtil.DevAssertArgs(categoryForEntity.IsValid, component.name, "was found by worldinventory but doesn't have a category! Add it to the element definition.");
				Discover(tag, categoryForEntity);
			}
			foreach (Tag tag2 in component2.Tags)
			{
				if (!Inventory.TryGetValue(tag2, out List<Pickupable> value))
				{
					value = new List<Pickupable>();
					Inventory[tag2] = value;
				}
				value.Add(component);
			}
		}
	}

	private void OnRemovedFetchable(object data)
	{
		GameObject gameObject = (GameObject)data;
		Pickupable component = gameObject.GetComponent<Pickupable>();
		foreach (Tag tag in component.GetComponent<KPrefabID>().Tags)
		{
			if (Inventory.TryGetValue(tag, out List<Pickupable> value))
			{
				value.Remove(component);
			}
		}
	}

	public Dictionary<Tag, float> GetAccessibleAmounts()
	{
		return accessibleAmounts;
	}
}
