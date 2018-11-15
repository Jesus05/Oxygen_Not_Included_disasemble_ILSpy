using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexScreen : KScreen
{
	public enum PlanCategory
	{
		Home,
		Tips,
		MyLog,
		Emails,
		Journals,
		ResearchNotes,
		Creatures,
		Plants,
		Food,
		Tech,
		Diseases,
		Roles,
		Buildings,
		Elements
	}

	private string activeEntryID;

	private Dictionary<CodexWidget.ContentType, UIGameObjectPool> ContentUIPools = new Dictionary<CodexWidget.ContentType, UIGameObjectPool>();

	private Dictionary<CodexWidget.ContentType, GameObject> ContentPrefabs = new Dictionary<CodexWidget.ContentType, GameObject>();

	private List<GameObject> categoryHeaders = new List<GameObject>();

	private Dictionary<CodexEntry, GameObject> entryButtons = new Dictionary<CodexEntry, GameObject>();

	private UIGameObjectPool contentContainerPool;

	[SerializeField]
	private KScrollRect displayScrollRect;

	[SerializeField]
	private RectTransform scrollContentPane;

	private bool editingSearch;

	private List<string> history = new List<string>();

	[Header("Hierarchy")]
	[SerializeField]
	private Transform navigatorContent;

	[SerializeField]
	private Transform displayPane;

	[SerializeField]
	private Transform contentContainers;

	[SerializeField]
	private Transform widgetPool;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private TMP_InputField searchInputField;

	[SerializeField]
	private KButton clearSearchButton;

	[SerializeField]
	private LocText backButton;

	[SerializeField]
	private LocText currentLocationText;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject prefabNavigatorEntry;

	[SerializeField]
	private GameObject prefabCategoryHeader;

	[SerializeField]
	private GameObject prefabContentContainer;

	[SerializeField]
	private GameObject prefabTextWidget;

	[SerializeField]
	private GameObject prefabImageWidget;

	[SerializeField]
	private GameObject prefabDividerLineWidget;

	[SerializeField]
	private GameObject prefabSpacer;

	[SerializeField]
	private GameObject prefabLargeSpacer;

	[SerializeField]
	private GameObject prefabLabelWithIcon;

	[SerializeField]
	private GameObject prefabContentLocked;

	[Header("Text Styles")]
	[SerializeField]
	private TextStyleSetting textStyleTitle;

	[SerializeField]
	private TextStyleSetting textStyleSubtitle;

	[SerializeField]
	private TextStyleSetting textStyleBody;

	private List<CodexEntry> searchResults = new List<CodexEntry>();

	private Coroutine scrollToTargetRoutine;

	protected override void OnActivate()
	{
		ConsumeMouseScroll = true;
		base.OnActivate();
		closeButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		clearSearchButton.onClick += delegate
		{
			searchInputField.text = string.Empty;
		};
		if (string.IsNullOrEmpty(activeEntryID))
		{
			ChangeArticle("HOME", false);
		}
		searchInputField.onValueChanged.AddListener(delegate(string value)
		{
			FilterSearch(value);
		});
		TMP_InputField tMP_InputField = searchInputField;
		tMP_InputField.onFocus = (System.Action)Delegate.Combine(tMP_InputField.onFocus, (System.Action)delegate
		{
			editingSearch = true;
		});
		searchInputField.onEndEdit.AddListener(delegate
		{
			editingSearch = false;
		});
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (editingSearch)
		{
			e.Consumed = true;
		}
		base.OnKeyDown(e);
	}

	public override float GetSortKey()
	{
		return 10000f;
	}

	private void Init()
	{
		SetupPrefabs();
		PopulatePools();
		CategorizeEntries();
		FilterSearch(string.Empty);
		Game.Instance.Subscribe(1594320620, delegate
		{
			FilterSearch(searchInputField.text);
			if (!string.IsNullOrEmpty(activeEntryID))
			{
				ChangeArticle(activeEntryID, false);
			}
		});
	}

	private void SetupPrefabs()
	{
		contentContainerPool = new UIGameObjectPool(prefabContentContainer);
		contentContainerPool.disabledElementParent = widgetPool;
		for (int i = 0; i < 7; i++)
		{
			switch (i)
			{
			case 0:
				ContentPrefabs[(CodexWidget.ContentType)i] = prefabTextWidget;
				break;
			case 1:
				ContentPrefabs[(CodexWidget.ContentType)i] = prefabImageWidget;
				break;
			case 2:
				ContentPrefabs[(CodexWidget.ContentType)i] = prefabDividerLineWidget;
				break;
			case 3:
				ContentPrefabs[(CodexWidget.ContentType)i] = prefabSpacer;
				break;
			case 4:
				ContentPrefabs[(CodexWidget.ContentType)i] = prefabLabelWithIcon;
				break;
			case 5:
				ContentPrefabs[(CodexWidget.ContentType)i] = prefabContentLocked;
				break;
			case 6:
				ContentPrefabs[(CodexWidget.ContentType)i] = prefabLargeSpacer;
				break;
			}
		}
	}

	private List<CodexEntry> FilterSearch(string input)
	{
		searchResults.Clear();
		input = input.ToLower();
		foreach (KeyValuePair<string, CodexEntry> entry in CodexCache.entries)
		{
			if (input == string.Empty)
			{
				if (!entry.Value.searchOnly)
				{
					searchResults.Add(entry.Value);
				}
			}
			else if (input == entry.Value.name.ToLower() || input.Contains(entry.Value.name.ToLower()) || entry.Value.name.ToLower().Contains(input))
			{
				searchResults.Add(entry.Value);
			}
			else
			{
				foreach (SubEntry subEntry in entry.Value.subEntries)
				{
					if (input == subEntry.name.ToLower() || input.Contains(subEntry.name.ToLower()) || subEntry.name.ToLower().Contains(input))
					{
						searchResults.Add(entry.Value);
					}
				}
			}
		}
		FilterEntries(input != string.Empty);
		return searchResults;
	}

	private bool HasUnlockedCategoryEntries(string entryID)
	{
		foreach (ContentContainer contentContainer in CodexCache.entries[entryID].contentContainers)
		{
			if (string.IsNullOrEmpty(contentContainer.lockID) || !Game.Instance.unlocks.IsLocked(contentContainer.lockID))
			{
				return true;
			}
		}
		return false;
	}

	private void FilterEntries(bool allowOpenCategories = true)
	{
		foreach (KeyValuePair<CodexEntry, GameObject> entryButton in entryButtons)
		{
			entryButton.Value.SetActive(searchResults.Contains(entryButton.Key) && HasUnlockedCategoryEntries(entryButton.Key.id));
		}
		foreach (GameObject categoryHeader in categoryHeaders)
		{
			bool flag = false;
			Transform transform = categoryHeader.transform.Find("Content");
			for (int i = 0; i < transform.childCount; i++)
			{
				if (transform.GetChild(i).gameObject.activeSelf)
				{
					flag = true;
				}
			}
			categoryHeader.SetActive(flag);
			if (allowOpenCategories)
			{
				if (flag)
				{
					ToggleCategoryOpen(categoryHeader, true);
				}
			}
			else
			{
				ToggleCategoryOpen(categoryHeader, false);
			}
		}
	}

	private void ToggleCategoryOpen(GameObject header, bool open)
	{
		MultiToggle reference = header.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("ExpandToggle");
		reference.ChangeState(open ? 1 : 0);
		header.GetComponent<HierarchyReferences>().GetReference("Content").gameObject.SetActive(open);
	}

	private void PopulatePools()
	{
		for (int i = 0; i < 7; i++)
		{
			ContentUIPools[(CodexWidget.ContentType)i] = new UIGameObjectPool(ContentPrefabs[(CodexWidget.ContentType)i]);
			ContentUIPools[(CodexWidget.ContentType)i].disabledElementParent = widgetPool;
		}
	}

	private GameObject NewCategoryHeader(KeyValuePair<string, CodexEntry> entryKVP, Dictionary<string, GameObject> categories)
	{
		if (entryKVP.Value.category == string.Empty)
		{
			entryKVP.Value.category = "Root";
		}
		GameObject categoryHeader = Util.KInstantiateUI(prefabCategoryHeader, navigatorContent.gameObject, true);
		GameObject categoryContent = categoryHeader.GetComponent<HierarchyReferences>().GetReference("Content").gameObject;
		categories.Add(entryKVP.Value.category, categoryContent);
		LocText reference = categoryHeader.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
		if (CodexCache.entries.ContainsKey(entryKVP.Value.category))
		{
			reference.text = CodexCache.entries[entryKVP.Value.category].name;
		}
		else
		{
			reference.text = Strings.Get("STRINGS.UI.CODEX.CATEGORYNAMES." + entryKVP.Value.category.ToUpper());
		}
		categoryHeaders.Add(categoryHeader);
		categoryContent.SetActive(false);
		MultiToggle reference2 = categoryHeader.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("ExpandToggle");
		reference2.onClick = delegate
		{
			ToggleCategoryOpen(categoryHeader, !categoryContent.activeSelf);
		};
		return categoryHeader;
	}

	private void CategorizeEntries()
	{
		string empty = string.Empty;
		GameObject gameObject = navigatorContent.gameObject;
		Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
		foreach (KeyValuePair<string, CodexEntry> entry in CodexCache.entries)
		{
			empty = entry.Value.category;
			if (empty == string.Empty || empty == "Root")
			{
				empty = "Root";
			}
			if (!dictionary.ContainsKey(empty))
			{
				NewCategoryHeader(entry, dictionary);
			}
			GameObject gameObject2 = Util.KInstantiateUI(prefabNavigatorEntry, dictionary[empty], true);
			string id = entry.Key;
			gameObject2.GetComponent<KButton>().onClick += delegate
			{
				ChangeArticle(id, false);
			};
			if (string.IsNullOrEmpty(entry.Value.name))
			{
				entry.Value.name = Strings.Get(entry.Value.title);
			}
			gameObject2.GetComponentInChildren<LocText>().text = entry.Value.name;
			entryButtons.Add(entry.Value, gameObject2);
		}
		foreach (KeyValuePair<string, CodexEntry> entry2 in CodexCache.entries)
		{
			if (CodexCache.entries.ContainsKey(entry2.Value.category) && CodexCache.entries.ContainsKey(CodexCache.entries[entry2.Value.category].category))
			{
				entry2.Value.searchOnly = true;
			}
		}
		List<KeyValuePair<string, GameObject>> list = new List<KeyValuePair<string, GameObject>>();
		foreach (KeyValuePair<string, GameObject> item in dictionary)
		{
			list.Add(item);
		}
		list.Sort((KeyValuePair<string, GameObject> a, KeyValuePair<string, GameObject> b) => string.Compare(a.Value.name, b.Value.name));
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Value.transform.parent.SetSiblingIndex(i);
		}
		SetupCategory(dictionary, "PLANTS");
		SetupCategory(dictionary, "CREATURES");
		SetupCategory(dictionary, "NOTICES");
		SetupCategory(dictionary, "RESEARCHNOTES");
		SetupCategory(dictionary, "JOURNALS");
		SetupCategory(dictionary, "EMAILS");
		SetupCategory(dictionary, "MYLOG");
		SetupCategory(dictionary, "TIPS");
		SetupCategory(dictionary, "Root");
	}

	private static void SetupCategory(Dictionary<string, GameObject> categories, string category_name)
	{
		if (categories.ContainsKey(category_name))
		{
			categories[category_name].transform.parent.SetAsFirstSibling();
		}
	}

	public void ChangeArticle(string id, bool playClickSound = false)
	{
		if (playClickSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
		}
		if (contentContainerPool == null)
		{
			Init();
		}
		SubEntry subEntry = null;
		if (!CodexCache.entries.ContainsKey(id))
		{
			subEntry = null;
			subEntry = CodexCache.FindSubEntry(id);
			if (subEntry != null && !subEntry.disabled)
			{
				id = subEntry.parentEntryID.ToUpper();
			}
		}
		CodexWidget codexWidget = null;
		CodexCache.entries[id].GetFirstWidget();
		RectTransform rectTransform = null;
		if (subEntry != null)
		{
			foreach (ContentContainer contentContainer2 in CodexCache.entries[id].contentContainers)
			{
				if (contentContainer2 == subEntry.contentContainers[0])
				{
					codexWidget = contentContainer2.content[0];
					break;
				}
			}
		}
		if (!CodexCache.entries.ContainsKey(id) || CodexCache.entries[id].disabled)
		{
			id = "PAGENOTFOUND";
		}
		int num = 0;
		string text = string.Empty;
		while (contentContainers.transform.childCount > 0)
		{
			while (!string.IsNullOrEmpty(text) && CodexCache.entries[activeEntryID].contentContainers[num].lockID == text)
			{
				num++;
			}
			GameObject gameObject = contentContainers.transform.GetChild(0).gameObject;
			int num2 = 0;
			while (gameObject.transform.childCount > 0)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(0).gameObject;
				CodexWidget.ContentType key;
				if (gameObject2.name == "PrefabContentLocked")
				{
					text = CodexCache.entries[activeEntryID].contentContainers[num].lockID;
					key = CodexWidget.ContentType.ContentLockedIndicator;
				}
				else
				{
					key = CodexCache.entries[activeEntryID].contentContainers[num].content[num2].type;
				}
				ContentUIPools[key].ClearElement(gameObject2);
				num2++;
			}
			contentContainerPool.ClearElement(contentContainers.transform.GetChild(0).gameObject);
			num++;
		}
		bool flag = CodexCache.entries[id] is CategoryEntry;
		activeEntryID = id;
		if (CodexCache.entries[id].contentContainers == null)
		{
			CodexCache.entries[id].contentContainers = new List<ContentContainer>();
		}
		bool flag2 = false;
		string a = string.Empty;
		for (int i = 0; i < CodexCache.entries[id].contentContainers.Count; i++)
		{
			ContentContainer contentContainer = CodexCache.entries[id].contentContainers[i];
			if (!string.IsNullOrEmpty(contentContainer.lockID) && Game.Instance.unlocks.IsLocked(contentContainer.lockID))
			{
				if (a != contentContainer.lockID)
				{
					GameObject gameObject3 = contentContainerPool.GetFreeElement(contentContainers.gameObject, true).gameObject;
					ConfigureContentContainer(contentContainer, gameObject3, flag && flag2);
					a = contentContainer.lockID;
					GameObject gameObject4 = ContentUIPools[CodexWidget.ContentType.ContentLockedIndicator].GetFreeElement(gameObject3, true).gameObject;
				}
			}
			else
			{
				GameObject gameObject3 = contentContainerPool.GetFreeElement(contentContainers.gameObject, true).gameObject;
				ConfigureContentContainer(contentContainer, gameObject3, flag && flag2);
				flag2 = !flag2;
				if (contentContainer.content != null)
				{
					foreach (CodexWidget item in contentContainer.content)
					{
						GameObject gameObject5 = ContentUIPools[item.type].GetFreeElement(gameObject3, true).gameObject;
						ConfigureContentWidget(item, gameObject5);
						if (item == codexWidget)
						{
							rectTransform = gameObject5.rectTransform();
						}
					}
				}
			}
		}
		string text2 = string.Empty;
		string text3 = id;
		int num3 = 0;
		while (text3 != CodexCache.FormatLinkID("HOME") && num3 < 10)
		{
			num3++;
			if (text3 != null)
			{
				text2 = ((!(text3 != id)) ? text2.Insert(0, CodexCache.entries[text3].name) : text2.Insert(0, CodexCache.entries[text3].name + " > "));
				text3 = CodexCache.entries[text3].parentId;
			}
			else
			{
				text3 = CodexCache.entries[CodexCache.FormatLinkID("HOME")].id;
				text2 = text2.Insert(0, CodexCache.entries[text3].name + " > ");
			}
		}
		currentLocationText.text = ((!(text2 == string.Empty)) ? text2 : CodexCache.entries["HOME"].name);
		if (history.Count == 0)
		{
			history.Add(activeEntryID);
		}
		else if (history[history.Count - 1] != activeEntryID)
		{
			if (history.Count > 1 && history[history.Count - 2] == activeEntryID)
			{
				history.RemoveAt(history.Count - 1);
			}
			else
			{
				history.Add(activeEntryID);
			}
		}
		if (history.Count > 1)
		{
			backButton.text = UI.FormatAsLink(string.Format(UI.CODEX.BACK_BUTTON, UI.StripLinkFormatting(CodexCache.entries[history[history.Count - 2]].name)), CodexCache.entries[history[history.Count - 2]].id);
		}
		else
		{
			backButton.text = UI.StripLinkFormatting(GameUtil.ColourizeString(Color.grey, string.Format(UI.CODEX.BACK_BUTTON, CodexCache.entries["HOME"].name)));
		}
		if ((UnityEngine.Object)rectTransform != (UnityEngine.Object)null)
		{
			if (scrollToTargetRoutine != null)
			{
				StopCoroutine(scrollToTargetRoutine);
			}
			scrollToTargetRoutine = StartCoroutine(ScrollToTarget(rectTransform));
		}
		else
		{
			displayScrollRect.content.SetLocalPosition(Vector3.zero);
		}
	}

	private IEnumerator ScrollToTarget(RectTransform targetWidgetTransform)
	{
		yield return (object)0;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private void ConfigureContentContainer(ContentContainer container, GameObject containerGameObject, bool bgColor = false)
	{
		LayoutGroup component = containerGameObject.GetComponent<LayoutGroup>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		if (!Game.Instance.unlocks.IsLocked(container.lockID))
		{
			switch (container.contentLayout)
			{
			case ContentContainer.ContentLayout.Horizontal:
			{
				component = containerGameObject.AddComponent<HorizontalLayoutGroup>();
				component.childAlignment = TextAnchor.MiddleLeft;
				HorizontalOrVerticalLayoutGroup obj2 = component as HorizontalOrVerticalLayoutGroup;
				bool childForceExpandHeight = (component as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = false;
				obj2.childForceExpandHeight = childForceExpandHeight;
				(component as HorizontalOrVerticalLayoutGroup).spacing = 8f;
				break;
			}
			case ContentContainer.ContentLayout.Vertical:
			{
				component = containerGameObject.AddComponent<VerticalLayoutGroup>();
				HorizontalOrVerticalLayoutGroup obj = component as HorizontalOrVerticalLayoutGroup;
				bool childForceExpandHeight = (component as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = false;
				obj.childForceExpandHeight = childForceExpandHeight;
				(component as HorizontalOrVerticalLayoutGroup).spacing = 8f;
				break;
			}
			case ContentContainer.ContentLayout.Grid:
				component = containerGameObject.AddComponent<GridLayoutGroup>();
				(component as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedColumnCount;
				(component as GridLayoutGroup).constraintCount = 3;
				(component as GridLayoutGroup).cellSize = new Vector2(170f, 32f);
				break;
			}
		}
		else
		{
			component = containerGameObject.AddComponent<VerticalLayoutGroup>();
			HorizontalOrVerticalLayoutGroup obj3 = component as HorizontalOrVerticalLayoutGroup;
			bool childForceExpandHeight = (component as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = false;
			obj3.childForceExpandHeight = childForceExpandHeight;
			(component as HorizontalOrVerticalLayoutGroup).spacing = 8f;
		}
	}

	private void ConfigureContentWidget(CodexWidget content, GameObject contentGameObject)
	{
		string value;
		switch (content.type)
		{
		case CodexWidget.ContentType.ContentLockedIndicator:
			break;
		case CodexWidget.ContentType.Text:
		{
			LocText component6 = contentGameObject.GetComponent<LocText>();
			component6.gameObject.SetActive(true);
			content.properties.TryGetValue("style", out value);
			if (value == "title")
			{
				component6.textStyleSetting = textStyleTitle;
				component6.AllowLinks = false;
			}
			else if (value == "subtitle")
			{
				component6.textStyleSetting = textStyleSubtitle;
				component6.AllowLinks = false;
			}
			else
			{
				component6.textStyleSetting = textStyleBody;
				component6.AllowLinks = true;
			}
			if (content.properties.ContainsKey("stringKey"))
			{
				content.properties.TryGetValue("stringKey", out value);
				component6.text = Strings.Get(value);
			}
			else if (content.properties.ContainsKey("string"))
			{
				content.properties.TryGetValue("string", out value);
				component6.text = value;
			}
			component6.ApplySettings();
			ConfigurePreferredLayout(content, contentGameObject);
			break;
		}
		case CodexWidget.ContentType.Image:
			if (content.properties.ContainsKey("spriteName"))
			{
				content.properties.TryGetValue("spriteName", out value);
				Image component2 = contentGameObject.GetComponent<Image>();
				component2.sprite = Assets.GetSprite(value);
				component2.color = Color.white;
			}
			else if (content.properties.ContainsKey("batchedAnimPrefabSourceID"))
			{
				Image component3 = contentGameObject.GetComponent<Image>();
				component3.sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab(content.properties["batchedAnimPrefabSourceID"]).GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false);
				component3.color = Color.white;
			}
			else if (content.objectProperties.ContainsKey("coloredSprite"))
			{
				Tuple<Sprite, Color> tuple = (Tuple<Sprite, Color>)content.objectProperties["coloredSprite"];
				Image component4 = contentGameObject.GetComponent<Image>();
				if (tuple != null)
				{
					component4.sprite = tuple.first;
					component4.color = tuple.second;
				}
				else
				{
					component4.sprite = null;
					component4.color = Color.clear;
				}
			}
			else
			{
				Image component5 = contentGameObject.GetComponent<Image>();
				component5.sprite = (Sprite)content.objectProperties["sprite"];
				component5.color = Color.white;
			}
			ConfigurePreferredLayout(content, contentGameObject);
			break;
		case CodexWidget.ContentType.DividerLine:
		{
			LayoutElement component = contentGameObject.GetComponent<LayoutElement>();
			Vector2 sizeDelta = displayPane.rectTransform().sizeDelta;
			component.minWidth = sizeDelta.x - 64f;
			break;
		}
		case CodexWidget.ContentType.Spacer:
			ConfigurePreferredLayout(content, contentGameObject);
			break;
		case CodexWidget.ContentType.LabelWithIcon:
			ConfigureLabelWithIcon(content, contentGameObject);
			break;
		}
	}

	private void ConfigureLabelWithIcon(CodexWidget content, GameObject contentGameObject)
	{
		LocText componentInChildren = contentGameObject.GetComponentInChildren<LocText>();
		componentInChildren.textStyleSetting = textStyleBody;
		componentInChildren.AllowLinks = true;
		string value = string.Empty;
		if (content.properties.ContainsKey("stringKey"))
		{
			content.properties.TryGetValue("stringKey", out value);
			componentInChildren.text = Strings.Get(value);
		}
		else if (content.properties.ContainsKey("string"))
		{
			content.properties.TryGetValue("string", out value);
			componentInChildren.text = value;
		}
		componentInChildren.ApplySettings();
		string value2 = string.Empty;
		if (content.properties.ContainsKey("spriteName"))
		{
			content.properties.TryGetValue("spriteName", out value2);
			Image component = contentGameObject.GetComponent<Image>();
			component.sprite = Assets.GetSprite(value2);
			component.color = Color.white;
		}
		else if (content.properties.ContainsKey("batchedAnimPrefabSourceID"))
		{
			Image componentInChildren2 = contentGameObject.GetComponentInChildren<Image>();
			componentInChildren2.sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab(content.properties["batchedAnimPrefabSourceID"]).GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false);
			componentInChildren2.color = Color.white;
		}
		else if (content.objectProperties.ContainsKey("coloredSprite"))
		{
			Tuple<Sprite, Color> tuple = (Tuple<Sprite, Color>)content.objectProperties["coloredSprite"];
			Image componentInChildren3 = contentGameObject.GetComponentInChildren<Image>();
			if (tuple != null)
			{
				componentInChildren3.sprite = tuple.first;
				componentInChildren3.color = tuple.second;
			}
			else
			{
				componentInChildren3.sprite = null;
				componentInChildren3.color = Color.clear;
			}
		}
		else
		{
			Image componentInChildren4 = contentGameObject.GetComponentInChildren<Image>();
			componentInChildren4.sprite = (Sprite)content.objectProperties["sprite"];
			componentInChildren4.color = Color.white;
		}
	}

	private void ConfigurePreferredLayout(CodexWidget content, GameObject contentGameObject)
	{
		LayoutElement component = contentGameObject.GetComponent<LayoutElement>();
		if (content.properties.ContainsKey("preferredHeight"))
		{
			component.preferredHeight = (float)Convert.ToInt32(content.properties["preferredHeight"]);
		}
		else
		{
			component.preferredHeight = -1f;
		}
		if (content.properties.ContainsKey("preferredWidth"))
		{
			component.preferredWidth = (float)Convert.ToInt32(content.properties["preferredWidth"]);
		}
		else
		{
			component.preferredWidth = -1f;
		}
	}
}
