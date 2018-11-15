using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class TreeFilterable : KMonoBehaviour, ISaveLoadable
{
	[MyCmpReq]
	private Storage storage;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	public bool showUserMenu = true;

	[SerializeField]
	[Serialize]
	private List<Tag> acceptedTags = new List<Tag>();

	public Action<Tag[]> OnFilterChanged;

	private static readonly EventSystem.IntraObjectHandler<TreeFilterable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<TreeFilterable>(delegate(TreeFilterable component, object data)
	{
		component.OnCopySettings(data);
	});

	public List<Tag> AcceptedTags => acceptedTags;

	private void OnDiscover(Tag category_tag, Tag tag)
	{
		if (storage.storageFilters.Contains(category_tag))
		{
			bool flag = false;
			if (WorldInventory.Instance.GetDiscoveredResourcesFromTag(category_tag).Count <= 1)
			{
				foreach (Tag storageFilter in storage.storageFilters)
				{
					if (!(storageFilter == category_tag) && WorldInventory.Instance.IsDiscovered(storageFilter))
					{
						flag = true;
						foreach (Tag item in WorldInventory.Instance.GetDiscoveredResourcesFromTag(storageFilter))
						{
							if (!acceptedTags.Contains(item))
							{
								return;
							}
						}
					}
				}
				if (!flag)
				{
					return;
				}
			}
			foreach (Tag item2 in WorldInventory.Instance.GetDiscoveredResourcesFromTag(category_tag))
			{
				if (!(item2 == tag) && !acceptedTags.Contains(item2))
				{
					return;
				}
			}
			AddTagToFilter(tag);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		WorldInventory.Instance.OnDiscover += OnDiscover;
		if ((UnityEngine.Object)storage != (UnityEngine.Object)null)
		{
			List<Tag> list = new List<Tag>();
			list.AddRange(acceptedTags);
			list.AddRange(storage.GetAllTagsInStorage());
			UpdateFilters(list.Distinct().ToList());
		}
		if (OnFilterChanged != null)
		{
			OnFilterChanged(acceptedTags.ToArray());
		}
		RemoveIncorrectAcceptedTags();
	}

	private void RemoveIncorrectAcceptedTags()
	{
		List<Tag> list = new List<Tag>();
		foreach (Tag acceptedTag in acceptedTags)
		{
			bool flag = false;
			foreach (Tag storageFilter in storage.storageFilters)
			{
				if (WorldInventory.Instance.GetDiscoveredResourcesFromTag(storageFilter).Contains(acceptedTag))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(acceptedTag);
			}
		}
		foreach (Tag item in list)
		{
			RemoveTagFromFilter(item);
		}
	}

	protected override void OnCleanUp()
	{
		WorldInventory.Instance.OnDiscover -= OnDiscover;
		base.OnCleanUp();
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		TreeFilterable component = gameObject.GetComponent<TreeFilterable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			UpdateFilters(component.GetTags());
		}
	}

	public Tag[] GetTags()
	{
		return acceptedTags.ToArray();
	}

	public bool ContainsTag(Tag t)
	{
		return acceptedTags.Contains(t);
	}

	public void AddTagToFilter(Tag t)
	{
		if (!ContainsTag(t))
		{
			List<Tag> list = new List<Tag>(acceptedTags);
			list.Add(t);
			UpdateFilters(list);
		}
	}

	public void RemoveTagFromFilter(Tag t)
	{
		if (ContainsTag(t))
		{
			List<Tag> list = new List<Tag>(acceptedTags);
			list.Remove(t);
			UpdateFilters(list);
		}
	}

	public void UpdateFilters(IList<Tag> filters)
	{
		acceptedTags.Clear();
		acceptedTags.AddRange(filters);
		if (OnFilterChanged != null)
		{
			OnFilterChanged(acceptedTags.ToArray());
		}
		if ((UnityEngine.Object)storage != (UnityEngine.Object)null && storage.items != null)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject item in storage.items)
			{
				if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
				{
					KPrefabID component = item.GetComponent<KPrefabID>();
					bool flag = false;
					foreach (Tag acceptedTag in acceptedTags)
					{
						if (component.Tags.Contains(acceptedTag))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(item);
					}
				}
			}
			foreach (GameObject item2 in list)
			{
				storage.Drop(item2);
			}
		}
	}

	public string GetTagsAsStatus(int maxDisplays = 6)
	{
		string text = "Tags:\n";
		List<Tag> list = new List<Tag>(acceptedTags);
		list.Intersect(storage.storageFilters);
		for (int i = 0; i < Mathf.Min(list.Count, maxDisplays); i++)
		{
			text += list[i].ProperName();
			if (i < Mathf.Min(list.Count, maxDisplays) - 1)
			{
				text += "\n";
			}
			if (i == maxDisplays - 1 && list.Count > maxDisplays)
			{
				text += "\n...";
				break;
			}
		}
		if (base.tag.Length == 0)
		{
			text = "No tags selected";
		}
		return text;
	}
}
