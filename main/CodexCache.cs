using Klei;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CodexCache
{
	private static string baseEntryPath;

	public static Dictionary<string, CodexEntry> entries;

	private static Dictionary<string, List<string>> unlockedEntryLookup;

	private static List<Tuple<string, Type>> widgetTagMappings;

	public static string FormatLinkID(string linkID)
	{
		linkID = linkID.ToUpper();
		linkID = linkID.Replace("_", string.Empty);
		return linkID;
	}

	public static void Init()
	{
		entries = new Dictionary<string, CodexEntry>();
		unlockedEntryLookup = new Dictionary<string, List<string>>();
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		if (widgetTagMappings == null)
		{
			List<Tuple<string, Type>> list = new List<Tuple<string, Type>>();
			list.Add(new Tuple<string, Type>("!CodexText", typeof(CodexText)));
			list.Add(new Tuple<string, Type>("!CodexImage", typeof(CodexImage)));
			list.Add(new Tuple<string, Type>("!CodexDividerLine", typeof(CodexDividerLine)));
			list.Add(new Tuple<string, Type>("!CodexSpacer", typeof(CodexSpacer)));
			list.Add(new Tuple<string, Type>("!CodexLabelWithIcon", typeof(CodexLabelWithIcon)));
			list.Add(new Tuple<string, Type>("!CodexContentLockedIndicator", typeof(CodexContentLockedIndicator)));
			list.Add(new Tuple<string, Type>("!CodexLargeSpacer", typeof(CodexLargeSpacer)));
			widgetTagMappings = list;
		}
		string text = FormatLinkID("creatures");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.CREATURES, CodexEntryGenerator.GenerateCreatureEntries(), Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab("Hatch").GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false)));
		text = FormatLinkID("plants");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.PLANTS, CodexEntryGenerator.GeneratePlantEntries(), Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab("PrickleFlower").GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false)));
		text = FormatLinkID("food");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.FOOD, CodexEntryGenerator.GenerateFoodEntries(), Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab("CookedMeat").GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false)));
		text = FormatLinkID("buildings");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.BUILDINGS, CodexEntryGenerator.GenerateBuildingEntries(), Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab("Generator").GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false)));
		text = FormatLinkID("tech");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.TECH, CodexEntryGenerator.GenerateTechEntries(), Assets.GetSprite("hud_research")));
		text = FormatLinkID("roles");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.ROLES, CodexEntryGenerator.GenerateRoleEntries(), Assets.GetSprite("hat_role_mining2")));
		text = FormatLinkID("disease");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.DISEASE, CodexEntryGenerator.GenerateDiseaseEntries(), Assets.GetSprite("overlay_disease")));
		text = FormatLinkID("elements");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.ELEMENTS, CodexEntryGenerator.GenerateElementEntries(), null));
		text = FormatLinkID("geysers");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.GEYSERS, CodexEntryGenerator.GenerateGeyserEntries(), null));
		text = FormatLinkID("equipment");
		dictionary.Add(text, CodexEntryGenerator.GenerateCategoryEntry(text, UI.CODEX.CATEGORYNAMES.EQUIPMENT, CodexEntryGenerator.GenerateEquipmentEntries(), null));
		CategoryEntry item = CodexEntryGenerator.GenerateCategoryEntry(FormatLinkID("HOME"), UI.CODEX.CATEGORYNAMES.ROOT, dictionary, null);
		CodexEntryGenerator.GeneratePageNotFound();
		List<CategoryEntry> list2 = new List<CategoryEntry>();
		foreach (KeyValuePair<string, CodexEntry> item2 in dictionary)
		{
			list2.Add(item2.Value as CategoryEntry);
		}
		CollectYAMLEntries(list2);
		CollectYAMLSubEntries(list2);
		CheckUnlockableContent();
		list2.Add(item);
		foreach (KeyValuePair<string, CodexEntry> entry in entries)
		{
			if (entry.Value.subEntries.Count > 0)
			{
				entry.Value.subEntries.Sort((SubEntry a, SubEntry b) => a.layoutPriority.CompareTo(b.layoutPriority));
				if ((UnityEngine.Object)entry.Value.icon == (UnityEngine.Object)null)
				{
					entry.Value.icon = entry.Value.subEntries[0].icon;
					entry.Value.iconColor = entry.Value.subEntries[0].iconColor;
				}
				int num = 0;
				foreach (SubEntry subEntry in entry.Value.subEntries)
				{
					if (subEntry.lockID != null && !Game.Instance.unlocks.IsUnlocked(subEntry.lockID))
					{
						num++;
					}
				}
				List<ICodexWidget> list3 = new List<ICodexWidget>();
				list3.Add(new CodexSpacer());
				list3.Add(new CodexText((string)CODEX.HEADERS.SUBENTRIES + " (" + (entry.Value.subEntries.Count - num) + "/" + entry.Value.subEntries.Count + ")", CodexTextStyle.Subtitle));
				foreach (SubEntry subEntry2 in entry.Value.subEntries)
				{
					if (subEntry2.lockID != null && !Game.Instance.unlocks.IsUnlocked(subEntry2.lockID))
					{
						list3.Add(new CodexText(UI.FormatAsLink(CODEX.HEADERS.CONTENTLOCKED, UI.ExtractLinkID(subEntry2.name)), CodexTextStyle.Body));
					}
					else
					{
						list3.Add(new CodexText(subEntry2.name, CodexTextStyle.Body));
					}
				}
				list3.Add(new CodexSpacer());
				entry.Value.contentContainers.Insert(entry.Value.customContentLength, new ContentContainer(list3, ContentContainer.ContentLayout.Vertical));
			}
			for (int i = 0; i < entry.Value.subEntries.Count; i++)
			{
				entry.Value.contentContainers.AddRange(entry.Value.subEntries[i].contentContainers);
			}
		}
		CodexEntryGenerator.PopulateCategoryEntries(list2);
	}

	public static SubEntry FindSubEntry(string id)
	{
		foreach (KeyValuePair<string, CodexEntry> entry in entries)
		{
			foreach (SubEntry subEntry in entry.Value.subEntries)
			{
				if (subEntry.id.ToUpper() == id.ToUpper())
				{
					return subEntry;
				}
			}
		}
		return null;
	}

	private static void CheckUnlockableContent()
	{
		foreach (KeyValuePair<string, CodexEntry> entry in entries)
		{
			foreach (SubEntry subEntry in entry.Value.subEntries)
			{
				if (subEntry.lockedContentContainer != null)
				{
					subEntry.lockedContentContainer.content.Clear();
					subEntry.contentContainers.Remove(subEntry.lockedContentContainer);
				}
			}
		}
	}

	private static void CollectYAMLEntries(List<CategoryEntry> categories)
	{
		baseEntryPath = Application.streamingAssetsPath + "/codex";
		List<CodexEntry> list = CollectEntries(string.Empty);
		foreach (CodexEntry item in list)
		{
			if (item != null && item.id != null && item.contentContainers != null)
			{
				if (entries.ContainsKey(FormatLinkID(item.id)))
				{
					MergeEntry(item.id, item);
				}
				else
				{
					AddEntry(item.id, item, categories);
				}
			}
		}
		string[] directories = Directory.GetDirectories(baseEntryPath);
		foreach (string path in directories)
		{
			List<CodexEntry> list2 = CollectEntries(Path.GetFileNameWithoutExtension(path));
			foreach (CodexEntry item2 in list2)
			{
				if (item2 != null && item2.id != null && item2.contentContainers != null)
				{
					if (entries.ContainsKey(FormatLinkID(item2.id)))
					{
						MergeEntry(item2.id, item2);
					}
					else
					{
						AddEntry(item2.id, item2, categories);
					}
				}
			}
		}
	}

	private static void CollectYAMLSubEntries(List<CategoryEntry> categories)
	{
		baseEntryPath = Application.streamingAssetsPath + "/codex";
		List<SubEntry> list = CollectSubEntries(string.Empty);
		foreach (SubEntry item in list)
		{
			if (item.parentEntryID != null && item.id != null)
			{
				if (entries.ContainsKey(item.parentEntryID.ToUpper()))
				{
					SubEntry subEntry = entries[item.parentEntryID.ToUpper()].subEntries.Find((SubEntry match) => match.id == item.id);
					if (!string.IsNullOrEmpty(item.lockID))
					{
						foreach (ContentContainer contentContainer in item.contentContainers)
						{
							contentContainer.lockID = item.lockID;
						}
					}
					if (subEntry != null)
					{
						if (!string.IsNullOrEmpty(item.lockID))
						{
							foreach (ContentContainer contentContainer2 in subEntry.contentContainers)
							{
								contentContainer2.lockID = item.lockID;
							}
							subEntry.lockID = item.lockID;
						}
						for (int i = 0; i < item.contentContainers.Count; i++)
						{
							if (!string.IsNullOrEmpty(item.contentContainers[i].lockID))
							{
								int num = subEntry.contentContainers.IndexOf(subEntry.lockedContentContainer);
								subEntry.contentContainers.Insert(num + 1, item.contentContainers[i]);
							}
							else if (item.contentContainers[i].showBeforeGeneratedContent)
							{
								subEntry.contentContainers.Insert(0, item.contentContainers[i]);
							}
							else
							{
								subEntry.contentContainers.Add(item.contentContainers[i]);
							}
						}
						subEntry.contentContainers.Add(new ContentContainer(new List<ICodexWidget>
						{
							new CodexLargeSpacer()
						}, ContentContainer.ContentLayout.Vertical));
						subEntry.layoutPriority = item.layoutPriority;
					}
					else
					{
						entries[item.parentEntryID.ToUpper()].subEntries.Add(item);
					}
				}
				else
				{
					Debug.LogWarningFormat("Codex SubEntry {0} cannot find parent codex entry with id {1}", item.name, item.parentEntryID);
				}
			}
		}
	}

	private static void AddLockLookup(string lockId, string articleId)
	{
		if (!unlockedEntryLookup.ContainsKey(lockId))
		{
			unlockedEntryLookup[lockId] = new List<string>();
		}
		unlockedEntryLookup[lockId].Add(articleId);
	}

	public static string GetEntryForLock(string lockId)
	{
		if (unlockedEntryLookup.ContainsKey(lockId) && unlockedEntryLookup[lockId].Count > 0)
		{
			return unlockedEntryLookup[lockId][0];
		}
		return null;
	}

	public static void AddEntry(string id, CodexEntry entry, List<CategoryEntry> categoryEntries = null)
	{
		id = FormatLinkID(id);
		if (entries.ContainsKey(id))
		{
			Debug.LogError("Tried to add " + id + " to the Codex screen multiple times");
		}
		entries.Add(id, entry);
		entry.id = id;
		if (entry.name == null)
		{
			entry.name = Strings.Get(entry.title);
		}
		if (!string.IsNullOrEmpty(entry.iconPrefabID))
		{
			try
			{
				entry.icon = Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab(entry.iconPrefabID).GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false);
			}
			catch
			{
				Debug.LogWarningFormat("Unable to get icon for prefabID {0}", entry.iconPrefabID);
			}
		}
		if (categoryEntries != null)
		{
			CodexEntry codexEntry = categoryEntries.Find((CategoryEntry group) => group.id == entry.parentId);
			if (codexEntry != null)
			{
				(codexEntry as CategoryEntry).entriesInCategory.Add(entry);
			}
		}
		foreach (ContentContainer contentContainer in entry.contentContainers)
		{
			if (contentContainer.lockID != null)
			{
				AddLockLookup(contentContainer.lockID, entry.id);
			}
		}
	}

	public static void AddSubEntry(string id, SubEntry entry)
	{
	}

	public static void MergeSubEntry(string id, SubEntry entry)
	{
	}

	public static void MergeEntry(string id, CodexEntry entry)
	{
		id = FormatLinkID(entry.id);
		entry.id = id;
		CodexEntry codexEntry = entries[id];
		codexEntry.customContentLength = entry.contentContainers.Count;
		for (int num = entry.contentContainers.Count - 1; num >= 0; num--)
		{
			codexEntry.contentContainers.Insert(0, entry.contentContainers[num]);
		}
		if (entry.disabled)
		{
			codexEntry.disabled = entry.disabled;
		}
		foreach (ContentContainer contentContainer in entry.contentContainers)
		{
			if (contentContainer.lockID != null)
			{
				AddLockLookup(contentContainer.lockID, entry.id);
			}
		}
	}

	public static void Clear()
	{
		entries = null;
		baseEntryPath = null;
	}

	public static string GetEntryPath()
	{
		return baseEntryPath;
	}

	public static CodexEntry GetTemplate(string templatePath)
	{
		if (!entries.ContainsKey(templatePath))
		{
			entries.Add(templatePath, null);
		}
		if (entries[templatePath] == null)
		{
			string text = Path.Combine(baseEntryPath, templatePath);
			CodexEntry codexEntry = YamlIO<CodexEntry>.LoadFile(text + ".yaml", widgetTagMappings);
			if (codexEntry == null)
			{
				Debug.LogWarning("Missing template [" + text + ".yaml]");
			}
			entries[templatePath] = codexEntry;
		}
		return entries[templatePath];
	}

	public static List<CodexEntry> CollectEntries(string folder)
	{
		List<CodexEntry> list = new List<CodexEntry>();
		string path = (!(folder == string.Empty)) ? Path.Combine(baseEntryPath, folder) : baseEntryPath;
		string[] array = new string[0];
		try
		{
			array = Directory.GetFiles(path, "*.yaml");
		}
		catch (UnauthorizedAccessException obj)
		{
			Debug.LogWarning(obj);
		}
		string category = folder.ToUpper();
		string[] array2 = array;
		foreach (string filename in array2)
		{
			CodexEntry codexEntry = YamlIO<CodexEntry>.LoadFile(filename, widgetTagMappings);
			if (codexEntry != null)
			{
				codexEntry.category = category;
				list.Add(codexEntry);
			}
		}
		list.Sort((CodexEntry x, CodexEntry y) => x.title.CompareTo(y.title));
		return list;
	}

	public static List<SubEntry> CollectSubEntries(string folder)
	{
		List<SubEntry> list = new List<SubEntry>();
		string path = (!(folder == string.Empty)) ? Path.Combine(baseEntryPath, folder) : baseEntryPath;
		string[] array = new string[0];
		try
		{
			array = Directory.GetFiles(path, "*.yaml", SearchOption.AllDirectories);
		}
		catch (UnauthorizedAccessException obj)
		{
			Debug.LogWarning(obj);
		}
		string[] array2 = array;
		foreach (string filename in array2)
		{
			SubEntry subEntry = YamlIO<SubEntry>.LoadFile(filename, widgetTagMappings);
			if (subEntry != null)
			{
				list.Add(subEntry);
			}
		}
		list.Sort((SubEntry x, SubEntry y) => x.title.CompareTo(y.title));
		return list;
	}
}
