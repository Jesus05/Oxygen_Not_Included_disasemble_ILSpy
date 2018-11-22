using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeFilterableSideScreen : SideScreenContent
{
	private struct TagOrderInfo
	{
		public Tag tag;

		public string strippedName;
	}

	[SerializeField]
	private MultiToggle allCheckBox;

	[SerializeField]
	private KToggle onlyAllowTransportItemsCheckBox;

	[SerializeField]
	private GameObject onlyallowTransportItemsRow;

	[SerializeField]
	private TreeFilterableSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	[SerializeField]
	private TreeFilterableSideScreenElement elementPrefab;

	private GameObject target;

	private bool visualDirty = false;

	private KImage onlyAllowTransportItemsImg;

	public UIPool<TreeFilterableSideScreenElement> elementPool;

	private UIPool<TreeFilterableSideScreenRow> rowPool;

	private TreeFilterable targetFilterable;

	private Dictionary<Tag, TreeFilterableSideScreenRow> tagRowMap = new Dictionary<Tag, TreeFilterableSideScreenRow>();

	private Storage storage;

	public bool IsStorage => (UnityEngine.Object)storage != (UnityEngine.Object)null;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		rowPool = new UIPool<TreeFilterableSideScreenRow>(rowPrefab);
		elementPool = new UIPool<TreeFilterableSideScreenElement>(elementPrefab);
		MultiToggle multiToggle = allCheckBox;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			switch (GetAllCheckboxState())
			{
			case TreeFilterableSideScreenRow.State.On:
				SetAllCheckboxState(TreeFilterableSideScreenRow.State.Off);
				break;
			case TreeFilterableSideScreenRow.State.Off:
			case TreeFilterableSideScreenRow.State.Mixed:
				SetAllCheckboxState(TreeFilterableSideScreenRow.State.On);
				break;
			}
		});
		onlyAllowTransportItemsImg = onlyAllowTransportItemsCheckBox.gameObject.GetComponentInChildrenOnly<KImage>();
		onlyAllowTransportItemsCheckBox.onClick += OnlyAllowTransportItemsClicked;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		allCheckBox.transform.parent.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ALLBUTTONTOOLTIP);
		onlyAllowTransportItemsCheckBox.transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ONLYALLOWTRANSPORTITEMSBUTTONTOOLTIP);
	}

	private void UpdateAllCheckBoxVisualState()
	{
		switch (GetAllCheckboxState())
		{
		case TreeFilterableSideScreenRow.State.Off:
			allCheckBox.ChangeState(0);
			break;
		case TreeFilterableSideScreenRow.State.Mixed:
			allCheckBox.ChangeState(1);
			break;
		case TreeFilterableSideScreenRow.State.On:
			allCheckBox.ChangeState(2);
			break;
		}
		visualDirty = false;
	}

	public void Update()
	{
		foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> item in tagRowMap)
		{
			if (item.Value.visualDirty)
			{
				item.Value.UpdateCheckBoxVisualState();
				visualDirty = true;
			}
		}
		if (visualDirty)
		{
			UpdateAllCheckBoxVisualState();
		}
	}

	private void OnlyAllowTransportItemsClicked()
	{
		storage.SetOnlyFetchMarkedItems(!storage.GetOnlyFetchMarkedItems());
	}

	private TreeFilterableSideScreenRow.State GetAllCheckboxState()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		using (Dictionary<Tag, TreeFilterableSideScreenRow>.Enumerator enumerator = tagRowMap.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current.Value.GetState())
				{
				case TreeFilterableSideScreenRow.State.Mixed:
					flag3 = true;
					break;
				case TreeFilterableSideScreenRow.State.On:
					flag = true;
					break;
				case TreeFilterableSideScreenRow.State.Off:
					flag2 = true;
					break;
				}
			}
		}
		if (!flag3)
		{
			if (flag && !flag2)
			{
				return TreeFilterableSideScreenRow.State.On;
			}
			if (!flag && flag2)
			{
				return TreeFilterableSideScreenRow.State.Off;
			}
			if (flag && flag2)
			{
				return TreeFilterableSideScreenRow.State.Mixed;
			}
			return TreeFilterableSideScreenRow.State.Off;
		}
		return TreeFilterableSideScreenRow.State.Mixed;
	}

	private void SetAllCheckboxState(TreeFilterableSideScreenRow.State newState)
	{
		switch (newState)
		{
		case TreeFilterableSideScreenRow.State.Off:
			foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> item in tagRowMap)
			{
				item.Value.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.Off);
			}
			break;
		case TreeFilterableSideScreenRow.State.On:
			foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> item2 in tagRowMap)
			{
				item2.Value.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.On);
			}
			break;
		}
		visualDirty = true;
	}

	public bool GetElementTagAcceptedState(Tag t)
	{
		return targetFilterable.ContainsTag(t);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (UnityEngine.Object)target.GetComponent<TreeFilterable>() != (UnityEngine.Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		this.target = target;
		if ((UnityEngine.Object)target == (UnityEngine.Object)null)
		{
			Debug.LogError("The target object provided was null", null);
		}
		else
		{
			targetFilterable = target.GetComponent<TreeFilterable>();
			if ((UnityEngine.Object)targetFilterable == (UnityEngine.Object)null)
			{
				Debug.LogError("The target provided does not have a Tree Filterable component", null);
			}
			else if (!targetFilterable.showUserMenu)
			{
				DetailsScreen.Instance.DeactivateSideContent();
			}
			else if (IsStorage && !storage.showInUI)
			{
				DetailsScreen.Instance.DeactivateSideContent();
			}
			else
			{
				storage = targetFilterable.GetComponent<Storage>();
				storage.Subscribe(644822890, OnOnlyFetchMarkedItemsSettingChanged);
				OnOnlyFetchMarkedItemsSettingChanged(null);
				CreateCategories();
			}
		}
	}

	private void OnOnlyFetchMarkedItemsSettingChanged(object data)
	{
		if (storage.allowSettingOnlyFetchMarkedItems)
		{
			onlyallowTransportItemsRow.SetActive(true);
			onlyAllowTransportItemsCheckBox.isOn = storage.GetOnlyFetchMarkedItems();
			onlyAllowTransportItemsImg.enabled = storage.GetOnlyFetchMarkedItems();
		}
		else
		{
			onlyallowTransportItemsRow.SetActive(false);
		}
	}

	public bool IsTagAllowed(Tag tag)
	{
		return targetFilterable.AcceptedTags.Contains(tag);
	}

	public void AddTag(Tag tag)
	{
		if (!((UnityEngine.Object)targetFilterable == (UnityEngine.Object)null))
		{
			targetFilterable.AddTagToFilter(tag);
		}
	}

	public void RemoveTag(Tag tag)
	{
		if (!((UnityEngine.Object)targetFilterable == (UnityEngine.Object)null))
		{
			targetFilterable.RemoveTagFromFilter(tag);
		}
	}

	private List<TagOrderInfo> GetTagsSortedAlphabetically(ICollection<Tag> tags)
	{
		List<TagOrderInfo> list = new List<TagOrderInfo>();
		foreach (Tag tag in tags)
		{
			list.Add(new TagOrderInfo
			{
				tag = tag,
				strippedName = UI.StripLinkFormatting(tag.ProperName())
			});
		}
		list.Sort((TagOrderInfo a, TagOrderInfo b) => a.strippedName.CompareTo(b.strippedName));
		return list;
	}

	private TreeFilterableSideScreenRow AddRow(Tag rowTag)
	{
		TreeFilterableSideScreenRow freeElement = rowPool.GetFreeElement(rowGroup, true);
		freeElement.Parent = this;
		tagRowMap.Add(rowTag, freeElement);
		Dictionary<Tag, bool> dictionary = new Dictionary<Tag, bool>();
		List<TagOrderInfo> tagsSortedAlphabetically = GetTagsSortedAlphabetically(WorldInventory.Instance.GetDiscoveredResourcesFromTag(rowTag));
		foreach (TagOrderInfo item in tagsSortedAlphabetically)
		{
			TagOrderInfo current = item;
			dictionary.Add(current.tag, targetFilterable.ContainsTag(current.tag) || targetFilterable.ContainsTag(rowTag));
		}
		freeElement.SetElement(rowTag, targetFilterable.ContainsTag(rowTag), dictionary);
		freeElement.transform.SetAsLastSibling();
		return freeElement;
	}

	public float GetAmountInStorage(Tag tag)
	{
		if (IsStorage)
		{
			return storage.GetMassAvailable(tag);
		}
		return 0f;
	}

	private void CreateCategories()
	{
		if (storage.storageFilters != null && storage.storageFilters.Count >= 1)
		{
			bool flag = (UnityEngine.Object)target.GetComponent<CreatureDeliveryPoint>() != (UnityEngine.Object)null;
			List<TagOrderInfo> tagsSortedAlphabetically = GetTagsSortedAlphabetically(storage.storageFilters);
			foreach (TagOrderInfo item in tagsSortedAlphabetically)
			{
				TagOrderInfo current = item;
				Tag tag = current.tag;
				if (flag || WorldInventory.Instance.IsDiscovered(tag))
				{
					AddRow(tag);
				}
			}
			visualDirty = true;
		}
		else
		{
			Output.LogError("If you're filtering, your storage filter should have the filters set on it");
		}
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if ((UnityEngine.Object)storage != (UnityEngine.Object)null)
		{
			storage.Unsubscribe(644822890, OnOnlyFetchMarkedItemsSettingChanged);
		}
		rowPool.ClearAll();
		elementPool.ClearAll();
		tagRowMap.Clear();
	}
}
