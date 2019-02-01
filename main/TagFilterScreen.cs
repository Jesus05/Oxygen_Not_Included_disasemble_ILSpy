using System.Collections.Generic;
using UnityEngine;

public class TagFilterScreen : SideScreenContent
{
	public class TagEntry
	{
		public string name;

		public Tag tag;

		public TagEntry[] children;
	}

	[SerializeField]
	private KTreeControl treeControl;

	private KTreeControl.UserItem rootItem;

	private TagEntry rootTag = defaultRootTag;

	private List<Tag> acceptedTags = new List<Tag>();

	private TreeFilterable targetFilterable;

	public static TagEntry defaultRootTag = new TagEntry
	{
		name = "All",
		tag = default(Tag),
		children = new TagEntry[0]
	};

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<TreeFilterable>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		if ((Object)target == (Object)null)
		{
			Debug.LogError("The target object provided was null", null);
		}
		else
		{
			targetFilterable = target.GetComponent<TreeFilterable>();
			if ((Object)targetFilterable == (Object)null)
			{
				Debug.LogError("The target provided does not have a Tree Filterable component", null);
			}
			else if (targetFilterable.showUserMenu)
			{
				Filter(targetFilterable.AcceptedTags);
				Activate();
			}
		}
	}

	protected override void OnActivate()
	{
		rootItem = BuildDisplay(rootTag);
		treeControl.SetUserItemRoot(rootItem);
		treeControl.root.opened = true;
		Filter(treeControl.root, acceptedTags, false);
	}

	public static List<Tag> GetAllTags()
	{
		List<Tag> list = new List<Tag>();
		TagEntry[] children = defaultRootTag.children;
		foreach (TagEntry tagEntry in children)
		{
			if (tagEntry.tag.IsValid)
			{
				list.Add(tagEntry.tag);
			}
		}
		return list;
	}

	private KTreeControl.UserItem BuildDisplay(TagEntry root)
	{
		KTreeControl.UserItem userItem = null;
		if (root.name != null && root.name != "")
		{
			KTreeControl.UserItem userItem2 = new KTreeControl.UserItem();
			userItem2.text = root.name;
			userItem2.userData = root.tag;
			userItem = userItem2;
			List<KTreeControl.UserItem> list = new List<KTreeControl.UserItem>();
			if (root.children != null)
			{
				TagEntry[] children = root.children;
				foreach (TagEntry root2 in children)
				{
					list.Add(BuildDisplay(root2));
				}
			}
			userItem.children = list;
		}
		return userItem;
	}

	private static KTreeControl.UserItem CreateTree(string tree_name, Tag tree_tag, IList<Element> items)
	{
		KTreeControl.UserItem userItem = new KTreeControl.UserItem();
		userItem.text = tree_name;
		userItem.userData = tree_tag;
		userItem.children = new List<KTreeControl.UserItem>();
		KTreeControl.UserItem userItem2 = userItem;
		foreach (Element item2 in items)
		{
			userItem = new KTreeControl.UserItem();
			userItem.text = item2.name;
			userItem.userData = GameTagExtensions.Create(item2.id);
			KTreeControl.UserItem item = userItem;
			userItem2.children.Add(item);
		}
		return userItem2;
	}

	public void SetRootTag(TagEntry root_tag)
	{
		rootTag = root_tag;
	}

	public void Filter(List<Tag> acceptedTags)
	{
		this.acceptedTags = acceptedTags;
	}

	private void Filter(KTreeItem root, List<Tag> acceptedTags, bool parentEnabled)
	{
		root.checkboxChecked = (parentEnabled || (root.userData != null && acceptedTags.Contains((Tag)root.userData)));
		foreach (KTreeItem child in root.children)
		{
			Filter(child, acceptedTags, root.checkboxChecked);
		}
		if (!root.checkboxChecked && root.children.Count > 0)
		{
			bool checkboxChecked = true;
			foreach (KTreeItem child2 in root.children)
			{
				if (!child2.checkboxChecked)
				{
					checkboxChecked = false;
					break;
				}
			}
			root.checkboxChecked = checkboxChecked;
		}
	}

	private void AddEnabledTags(KTreeItem root, List<Tag> tags)
	{
		bool flag = false;
		if (root.userData != null)
		{
			Tag item = (Tag)root.userData;
			if (item.IsValid && root.checkboxChecked)
			{
				flag = true;
				tags.Add(item);
			}
		}
		if (!flag)
		{
			foreach (KTreeItem child in root.children)
			{
				AddEnabledTags(child, tags);
			}
		}
	}

	private void UpdateFilters()
	{
		if ((Object)targetFilterable == (Object)null)
		{
			Debug.LogError("Cannot update the filters on a null target.", null);
		}
		else
		{
			List<Tag> list = new List<Tag>();
			AddEnabledTags(treeControl.root, list);
			targetFilterable.UpdateFilters(list);
		}
	}
}
