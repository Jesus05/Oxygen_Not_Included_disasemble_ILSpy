using Database;
using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public static class CodexEntryGenerator
{
	public static Dictionary<string, CodexEntry> GenerateBuildingEntries()
	{
		string str = "BUILD_CATEGORY_";
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (PlanScreen.PlanInfo item in TUNING.BUILDINGS.PLANORDER)
		{
			PlanScreen.PlanInfo current = item;
			string text = HashCache.Get().Get(current.category);
			string text2 = CodexCache.FormatLinkID(str + text);
			Dictionary<string, CodexEntry> dictionary2 = new Dictionary<string, CodexEntry>();
			for (int i = 0; i < (current.data as IList<string>).Count; i++)
			{
				BuildingDef buildingDef = Assets.GetBuildingDef((current.data as IList<string>)[i]);
				List<ContentContainer> list = new List<ContentContainer>();
				GenerateTitleContainers(buildingDef.Name, list);
				GenerateImageContainers(buildingDef.GetUISprite("ui", false), list);
				GenerateBuildingDescriptionContainers(buildingDef, list);
				GenerateFabricatorContainers(buildingDef.BuildingComplete, list);
				GenerateReceptacleContainers(buildingDef.BuildingComplete, list);
				CodexEntry codexEntry = new CodexEntry(text2, list, Strings.Get("STRINGS.BUILDINGS.PREFABS." + (current.data as IList<string>)[i].ToUpper() + ".NAME"));
				codexEntry.icon = buildingDef.GetUISprite("ui", false);
				codexEntry.parentId = text2;
				CodexCache.AddEntry((current.data as IList<string>)[i], codexEntry, null);
				dictionary2.Add(codexEntry.id, codexEntry);
			}
			CategoryEntry categoryEntry = GenerateCategoryEntry(CodexCache.FormatLinkID(text2), Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text.ToUpper() + ".NAME"), dictionary2, null);
			categoryEntry.parentId = "BUILDINGS";
			categoryEntry.category = "BUILDINGS";
			categoryEntry.icon = Assets.GetSprite(PlanScreen.IconNameMap[text]);
			dictionary.Add(text2, categoryEntry);
		}
		PopulateCategoryEntries(dictionary);
		return dictionary;
	}

	public static void GeneratePageNotFound()
	{
		List<ContentContainer> list = new List<ContentContainer>();
		ContentContainer contentContainer = new ContentContainer();
		contentContainer.content.Add(new CodexText(CODEX.PAGENOTFOUND.TITLE, CodexTextStyle.Title));
		contentContainer.content.Add(new CodexText(CODEX.PAGENOTFOUND.SUBTITLE, CodexTextStyle.Subtitle));
		contentContainer.content.Add(new CodexDividerLine());
		contentContainer.content.Add(new CodexImage(312, 312, Assets.GetSprite("outhouseMessage")));
		list.Add(contentContainer);
		CodexEntry codexEntry = new CodexEntry("ROOT", list, CODEX.PAGENOTFOUND.TITLE);
		codexEntry.searchOnly = true;
		CodexCache.AddEntry("PageNotFound", codexEntry, null);
	}

	public static Dictionary<string, CodexEntry> GenerateCreatureEntries()
	{
		Dictionary<string, CodexEntry> results = new Dictionary<string, CodexEntry>();
		List<GameObject> brains = Assets.GetPrefabsWithComponent<CreatureBrain>();
		Action<Tag, string> action = delegate(Tag speciesTag, string name)
		{
			CodexEntry codexEntry = new CodexEntry("CREATURES", new List<ContentContainer>
			{
				new ContentContainer(new List<ICodexWidget>
				{
					new CodexSpacer(),
					new CodexSpacer()
				}, ContentContainer.ContentLayout.Vertical)
			}, name)
			{
				parentId = "CREATURES"
			};
			CodexCache.AddEntry(speciesTag.ToString(), codexEntry, null);
			results.Add(speciesTag.ToString(), codexEntry);
			foreach (GameObject item2 in brains)
			{
				if (item2.GetDef<BabyMonitor.Def>() == null)
				{
					Sprite sprite = null;
					Sprite sprite2 = null;
					GameObject gameObject = Assets.TryGetPrefab(item2.PrefabID() + "Baby");
					if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
					{
						sprite2 = Def.GetUISprite(gameObject, "ui", false).first;
					}
					CreatureBrain component = item2.GetComponent<CreatureBrain>();
					if (!(component.species != speciesTag))
					{
						List<ContentContainer> list = new List<ContentContainer>();
						string symbolPrefix = component.symbolPrefix;
						sprite = Def.GetUISprite(item2, symbolPrefix + "ui", false).first;
						if ((bool)sprite2)
						{
							GenerateImageContainers(new Sprite[2]
							{
								sprite,
								sprite2
							}, list, ContentContainer.ContentLayout.Horizontal);
						}
						else
						{
							GenerateImageContainers(sprite, list);
						}
						GenerateCreatureDescriptionContainers(item2, list);
						SubEntry item = new SubEntry(component.PrefabID().ToString(), speciesTag.ToString(), list, component.GetProperName())
						{
							icon = sprite,
							iconColor = Color.white
						};
						codexEntry.subEntries.Add(item);
					}
				}
			}
		};
		action(GameTags.Creatures.Species.PuftSpecies, STRINGS.CREATURES.FAMILY.PUFT);
		action(GameTags.Creatures.Species.PacuSpecies, STRINGS.CREATURES.FAMILY.PACU);
		action(GameTags.Creatures.Species.OilFloaterSpecies, STRINGS.CREATURES.FAMILY.OILFLOATER);
		action(GameTags.Creatures.Species.LightBugSpecies, STRINGS.CREATURES.FAMILY.LIGHTBUG);
		action(GameTags.Creatures.Species.HatchSpecies, STRINGS.CREATURES.FAMILY.HATCH);
		action(GameTags.Creatures.Species.GlomSpecies, STRINGS.CREATURES.FAMILY.GLOM);
		action(GameTags.Creatures.Species.DreckoSpecies, STRINGS.CREATURES.FAMILY.DRECKO);
		action(GameTags.Creatures.Species.MooSpecies, STRINGS.CREATURES.FAMILY.MOO);
		action(GameTags.Creatures.Species.MoleSpecies, STRINGS.CREATURES.FAMILY.MOLE);
		action(GameTags.Creatures.Species.SquirrelSpecies, STRINGS.CREATURES.FAMILY.SQUIRREL);
		action(GameTags.Creatures.Species.CrabSpecies, STRINGS.CREATURES.FAMILY.CRAB);
		return results;
	}

	public static Dictionary<string, CodexEntry> GeneratePlantEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Harvestable>();
		prefabsWithComponent.AddRange(Assets.GetPrefabsWithComponent<WiltCondition>());
		foreach (GameObject item in prefabsWithComponent)
		{
			if (!dictionary.ContainsKey(item.PrefabID().ToString()))
			{
				List<ContentContainer> list = new List<ContentContainer>();
				Sprite first = Def.GetUISprite(item, "ui", false).first;
				GenerateImageContainers(first, list);
				GeneratePlantDescriptionContainers(item, list);
				CodexEntry codexEntry = new CodexEntry("PLANTS", list, item.GetProperName());
				codexEntry.parentId = "PLANTS";
				codexEntry.icon = first;
				CodexCache.AddEntry(item.PrefabID().ToString(), codexEntry, null);
				dictionary.Add(item.PrefabID().ToString(), codexEntry);
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateFoodEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (EdiblesManager.FoodInfo item in FOOD.FOOD_TYPES_LIST)
		{
			if (!Assets.GetPrefab(item.Id).HasTag(GameTags.IncubatableEgg))
			{
				List<ContentContainer> list = new List<ContentContainer>();
				GenerateTitleContainers(item.Name, list);
				Sprite first = Def.GetUISprite(item.ConsumableId, "ui", false).first;
				GenerateImageContainers(first, list);
				GenerateFoodDescriptionContainers(item, list);
				GenerateRecipeContainers(item.ConsumableId.ToTag(), list);
				GenerateUsedInRecipeContainers(item.ConsumableId.ToTag(), list);
				CodexEntry codexEntry = new CodexEntry("FOOD", list, item.Name);
				codexEntry.icon = first;
				codexEntry.parentId = "FOOD";
				CodexCache.AddEntry(item.Id, codexEntry, null);
				dictionary.Add(item.Id, codexEntry);
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateTechEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (Tech resource in Db.Get().Techs.resources)
		{
			List<ContentContainer> list = new List<ContentContainer>();
			GenerateTitleContainers(resource.Name, list);
			GenerateTechDescriptionContainers(resource, list);
			GeneratePrerequisiteTechContainers(resource, list);
			GenerateUnlockContainers(resource, list);
			CodexEntry codexEntry = new CodexEntry("TECH", list, resource.Name);
			TechItem techItem = resource.unlockedItems[0];
			if (techItem == null)
			{
				DebugUtil.LogErrorArgs("Unknown tech:", resource.Name);
			}
			codexEntry.icon = techItem.getUISprite("ui", false);
			codexEntry.parentId = "TECH";
			CodexCache.AddEntry(resource.Id, codexEntry, null);
			dictionary.Add(resource.Id, codexEntry);
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateRoleEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (Skill resource in Db.Get().Skills.resources)
		{
			List<ContentContainer> list = new List<ContentContainer>();
			Sprite sprite = null;
			sprite = Assets.GetSprite(resource.hat);
			GenerateTitleContainers(resource.Name, list);
			GenerateImageContainers(sprite, list);
			GenerateGenericDescriptionContainers(resource.description, list);
			GenerateSkillRequirementsAndPerksContainers(resource, list);
			GenerateRelatedSkillContainers(resource, list);
			CodexEntry codexEntry = new CodexEntry("ROLES", list, resource.Name);
			codexEntry.parentId = "ROLES";
			codexEntry.icon = sprite;
			CodexCache.AddEntry(resource.Id, codexEntry, null);
			dictionary.Add(resource.Id, codexEntry);
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateGeyserEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Geyser>();
		if (prefabsWithComponent != null)
		{
			foreach (GameObject item2 in prefabsWithComponent)
			{
				if (!item2.GetComponent<KPrefabID>().HasTag(GameTags.DeprecatedContent))
				{
					List<ContentContainer> list = new List<ContentContainer>();
					GenerateTitleContainers(item2.GetProperName(), list);
					Sprite first = Def.GetUISprite(item2, "ui", false).first;
					GenerateImageContainers(first, list);
					List<ICodexWidget> list2 = new List<ICodexWidget>();
					string text = item2.PrefabID().ToString();
					text = text.Remove(0, 14).ToUpper();
					list2.Add(new CodexText(Strings.Get("STRINGS.CREATURES.SPECIES.GEYSER." + text + ".DESC"), CodexTextStyle.Body));
					list2.Add(new CodexText(UI.CODEX.GEYSERS.DESC, CodexTextStyle.Body));
					ContentContainer item = new ContentContainer(list2, ContentContainer.ContentLayout.Vertical);
					list.Add(item);
					CodexEntry codexEntry = new CodexEntry("GEYSERS", list, item2.GetProperName());
					codexEntry.icon = first;
					codexEntry.parentId = "GEYSERS";
					codexEntry.id = item2.PrefabID().ToString();
					CodexCache.AddEntry(codexEntry.id, codexEntry, null);
					dictionary.Add(codexEntry.id, codexEntry);
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateEquipmentEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Equippable>();
		if (prefabsWithComponent != null)
		{
			foreach (GameObject item2 in prefabsWithComponent)
			{
				bool flag = false;
				Equippable component = item2.GetComponent<Equippable>();
				if (component.def.AdditionalTags != null)
				{
					Tag[] additionalTags = component.def.AdditionalTags;
					foreach (Tag a in additionalTags)
					{
						if (a == GameTags.DeprecatedContent)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					List<ContentContainer> list = new List<ContentContainer>();
					GenerateTitleContainers(item2.GetProperName(), list);
					Sprite first = Def.GetUISprite(item2, "ui", false).first;
					GenerateImageContainers(first, list);
					List<ICodexWidget> list2 = new List<ICodexWidget>();
					string text = item2.PrefabID().ToString();
					list2.Add(new CodexText(Strings.Get("STRINGS.EQUIPMENT.PREFABS." + text.ToUpper() + ".DESC"), CodexTextStyle.Body));
					ContentContainer item = new ContentContainer(list2, ContentContainer.ContentLayout.Vertical);
					list.Add(item);
					CodexEntry codexEntry = new CodexEntry("EQUIPMENT", list, item2.GetProperName());
					codexEntry.icon = first;
					codexEntry.parentId = "EQUIPMENT";
					codexEntry.id = item2.PrefabID().ToString();
					CodexCache.AddEntry(codexEntry.id, codexEntry, null);
					dictionary.Add(codexEntry.id, codexEntry);
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateElementEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary2 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary3 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary4 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary5 = new Dictionary<string, CodexEntry>();
		string text = CodexCache.FormatLinkID("ELEMENTS");
		string text2 = CodexCache.FormatLinkID("ELEMENTS_SOLID");
		string text3 = CodexCache.FormatLinkID("ELEMENTS_LIQUID");
		string text4 = CodexCache.FormatLinkID("ELEMENTS_GAS");
		string text5 = CodexCache.FormatLinkID("ELEMENTS_OTHER");
		Action<Element, List<ContentContainer>> action = delegate(Element element, List<ContentContainer> containers)
		{
			if (element.highTempTransition != null || element.lowTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexText(CODEX.HEADERS.ELEMENTTRANSITIONS, CodexTextStyle.Subtitle),
					new CodexDividerLine()
				}, ContentContainer.ContentLayout.Vertical));
			}
			if (element.highTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexImage(32, 32, Def.GetUISprite(element.highTempTransition, "ui", false)),
					new CodexText((element.highTempTransition == null) ? "" : (element.highTempTransition.name + " (" + element.highTempTransition.GetStateString() + ")  (" + GameUtil.GetFormattedTemperature(element.highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false) + ")"), CodexTextStyle.Body)
				}, ContentContainer.ContentLayout.Horizontal));
			}
			if (element.lowTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexImage(32, 32, Def.GetUISprite(element.lowTempTransition, "ui", false)),
					new CodexText((element.lowTempTransition == null) ? "" : (element.lowTempTransition.name + " (" + element.lowTempTransition.GetStateString() + ")  (" + GameUtil.GetFormattedTemperature(element.lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false) + ")"), CodexTextStyle.Body)
				}, ContentContainer.ContentLayout.Horizontal));
			}
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(element.FullDescription(true), CodexTextStyle.Body),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical));
		};
		string text6;
		foreach (Element element in ElementLoader.elements)
		{
			if (!element.disabled)
			{
				List<ContentContainer> list = new List<ContentContainer>();
				string name = element.name + " (" + element.GetStateString() + ")";
				GenerateTitleContainers(name, list);
				Tuple<Sprite, Color> uISprite = Def.GetUISprite(element, "ui", false);
				GenerateImageContainers(new Tuple<Sprite, Color>[1]
				{
					Def.GetUISprite(element, "ui", false)
				}, list, ContentContainer.ContentLayout.Horizontal);
				action(element, list);
				text6 = element.id.ToString();
				string text7;
				Dictionary<string, CodexEntry> dictionary6;
				if (element.IsSolid)
				{
					text7 = text2;
					dictionary6 = dictionary2;
				}
				else if (element.IsLiquid)
				{
					text7 = text3;
					dictionary6 = dictionary3;
				}
				else if (element.IsGas)
				{
					text7 = text4;
					dictionary6 = dictionary4;
				}
				else
				{
					text7 = text5;
					dictionary6 = dictionary5;
				}
				CodexEntry codexEntry = new CodexEntry(text7, list, name);
				codexEntry.parentId = text7;
				codexEntry.icon = uISprite.first;
				codexEntry.iconColor = uISprite.second;
				CodexCache.AddEntry(text6, codexEntry, null);
				dictionary6.Add(text6, codexEntry);
			}
		}
		text6 = text2;
		CodexEntry codexEntry2 = GenerateCategoryEntry(text6, UI.CODEX.CATEGORYNAMES.ELEMENTSSOLID, dictionary2, null);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text6, codexEntry2);
		text6 = text3;
		codexEntry2 = GenerateCategoryEntry(text6, UI.CODEX.CATEGORYNAMES.ELEMENTSLIQUID, dictionary3, null);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text6, codexEntry2);
		text6 = text4;
		codexEntry2 = GenerateCategoryEntry(text6, UI.CODEX.CATEGORYNAMES.ELEMENTSGAS, dictionary4, null);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text6, codexEntry2);
		text6 = text5;
		codexEntry2 = GenerateCategoryEntry(text6, UI.CODEX.CATEGORYNAMES.ELEMENTSOTHER, dictionary5, Assets.GetSprite("overlay_heatflow"));
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text6, codexEntry2);
		PopulateCategoryEntries(dictionary);
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateDiseaseEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (Disease resource in Db.Get().Diseases.resources)
		{
			if (!resource.Disabled)
			{
				List<ContentContainer> list = new List<ContentContainer>();
				GenerateTitleContainers(resource.Name, list);
				GenerateDiseaseDescriptionContainers(resource, list);
				CodexEntry codexEntry = new CodexEntry("DISEASE", list, resource.Name);
				codexEntry.parentId = "DISEASE";
				dictionary.Add(resource.Id, codexEntry);
				CodexCache.AddEntry(resource.Id, codexEntry, null);
			}
		}
		return dictionary;
	}

	public static CategoryEntry GenerateCategoryEntry(string id, string name, Dictionary<string, CodexEntry> entries, Sprite icon = null)
	{
		List<ContentContainer> list = new List<ContentContainer>();
		GenerateTitleContainers(name, list);
		List<CodexEntry> list2 = new List<CodexEntry>();
		foreach (KeyValuePair<string, CodexEntry> entry in entries)
		{
			list2.Add(entry.Value);
			if ((UnityEngine.Object)icon == (UnityEngine.Object)null)
			{
				icon = entry.Value.icon;
			}
		}
		CategoryEntry categoryEntry = new CategoryEntry("Root", list, name, list2);
		categoryEntry.icon = icon;
		CodexCache.AddEntry(id, categoryEntry, null);
		return categoryEntry;
	}

	public static Dictionary<string, CodexEntry> GenerateTutorialNotificationEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		for (int i = 0; i < 20; i++)
		{
			TutorialMessage tutorialMessage = (TutorialMessage)Tutorial.Instance.TutorialMessage((Tutorial.TutorialMessages)i, false);
			List<ContentContainer> list = new List<ContentContainer>();
			GenerateTitleContainers(tutorialMessage.GetTitle(), list);
			if (!string.IsNullOrEmpty(tutorialMessage.videoClipId))
			{
				CodexVideo codexVideo = new CodexVideo();
				codexVideo.videoName = tutorialMessage.videoClipId;
				codexVideo.overlayName = tutorialMessage.videoOverlayName;
				codexVideo.overlayTexts = new List<string>
				{
					tutorialMessage.videoTitleText,
					VIDEOS.TUTORIAL_HEADER
				};
				list.Add(new ContentContainer(new List<ICodexWidget>
				{
					codexVideo
				}, ContentContainer.ContentLayout.Vertical));
			}
			list.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexText(tutorialMessage.GetMessageBody(), CodexTextStyle.Body)
			}, ContentContainer.ContentLayout.Vertical));
			CodexEntry codexEntry = new CodexEntry("Tips", list, UI.FormatAsLink(tutorialMessage.GetTitle(), "tutorial_tips_" + i));
			CodexCache.AddEntry("tutorial_tips_" + i, codexEntry, null);
			dictionary.Add(codexEntry.id, codexEntry);
		}
		return dictionary;
	}

	public static void PopulateCategoryEntries(Dictionary<string, CodexEntry> categoryEntries)
	{
		List<CategoryEntry> list = new List<CategoryEntry>();
		foreach (KeyValuePair<string, CodexEntry> categoryEntry in categoryEntries)
		{
			list.Add(categoryEntry.Value as CategoryEntry);
		}
		PopulateCategoryEntries(list);
	}

	public static void PopulateCategoryEntries(List<CategoryEntry> categoryEntries)
	{
		foreach (CategoryEntry categoryEntry in categoryEntries)
		{
			List<ContentContainer> contentContainers = categoryEntry.contentContainers;
			List<CodexEntry> list = new List<CodexEntry>();
			foreach (CodexEntry item in categoryEntry.entriesInCategory)
			{
				list.Add(item);
			}
			list.Sort((CodexEntry a, CodexEntry b) => UI.StripLinkFormatting(a.name).CompareTo(UI.StripLinkFormatting(b.name)));
			foreach (CodexEntry item2 in list)
			{
				ContentContainer contentContainer = new ContentContainer(new List<ICodexWidget>(), ContentContainer.ContentLayout.Horizontal);
				if ((UnityEngine.Object)item2.icon != (UnityEngine.Object)null)
				{
					contentContainer.content.Add(new CodexImage(48, 48, item2.icon, item2.iconColor));
				}
				contentContainer.content.Add(new CodexText(item2.name, CodexTextStyle.Body));
				contentContainers.Add(contentContainer);
			}
		}
	}

	private static void GenerateTitleContainers(string name, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexText(name, CodexTextStyle.Title));
		list.Add(new CodexDividerLine());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GeneratePrerequisiteTechContainers(Tech tech, List<ContentContainer> containers)
	{
		if (tech.requiredTech != null && tech.requiredTech.Count != 0)
		{
			List<ICodexWidget> list = new List<ICodexWidget>();
			list.Add(new CodexText(CODEX.HEADERS.PREREQUISITE_TECH, CodexTextStyle.Subtitle));
			list.Add(new CodexDividerLine());
			list.Add(new CodexSpacer());
			foreach (Tech item in tech.requiredTech)
			{
				list.Add(new CodexText(item.Name, CodexTextStyle.Body));
			}
			list.Add(new CodexSpacer());
			containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		}
	}

	private static void GenerateSkillRequirementsAndPerksContainers(Skill skill, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText item = new CodexText(CODEX.HEADERS.ROLE_PERKS, CodexTextStyle.Subtitle);
		CodexText item2 = new CodexText(CODEX.HEADERS.ROLE_PERKS_DESC, CodexTextStyle.Body);
		list.Add(item);
		list.Add(new CodexDividerLine());
		list.Add(item2);
		list.Add(new CodexSpacer());
		foreach (SkillPerk perk in skill.perks)
		{
			CodexText item3 = new CodexText(perk.Name, CodexTextStyle.Body);
			list.Add(item3);
		}
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		list.Add(new CodexSpacer());
	}

	private static void GenerateRelatedSkillContainers(Skill skill, List<ContentContainer> containers)
	{
		bool flag = false;
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText item = new CodexText(CODEX.HEADERS.PREREQUISITE_ROLES, CodexTextStyle.Subtitle);
		list.Add(item);
		list.Add(new CodexDividerLine());
		list.Add(new CodexSpacer());
		foreach (string priorSkill in skill.priorSkills)
		{
			CodexText item2 = new CodexText(Db.Get().Skills.Get(priorSkill).Name, CodexTextStyle.Body);
			list.Add(item2);
			flag = true;
		}
		if (flag)
		{
			list.Add(new CodexSpacer());
			containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		}
		bool flag2 = false;
		List<ICodexWidget> list2 = new List<ICodexWidget>();
		CodexText item3 = new CodexText(CODEX.HEADERS.UNLOCK_ROLES, CodexTextStyle.Subtitle);
		CodexText item4 = new CodexText(CODEX.HEADERS.UNLOCK_ROLES_DESC, CodexTextStyle.Body);
		list2.Add(item3);
		list2.Add(new CodexDividerLine());
		list2.Add(item4);
		list2.Add(new CodexSpacer());
		foreach (Skill resource in Db.Get().Skills.resources)
		{
			foreach (string priorSkill2 in resource.priorSkills)
			{
				if (priorSkill2 == skill.Id)
				{
					CodexText item5 = new CodexText(resource.Name, CodexTextStyle.Body);
					list2.Add(item5);
					flag2 = true;
				}
			}
		}
		if (flag2)
		{
			list2.Add(new CodexSpacer());
			containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Vertical));
		}
	}

	private static void GenerateUnlockContainers(Tech tech, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText item = new CodexText(CODEX.HEADERS.TECH_UNLOCKS, CodexTextStyle.Subtitle);
		list.Add(item);
		list.Add(new CodexDividerLine());
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		foreach (TechItem unlockedItem in tech.unlockedItems)
		{
			List<ICodexWidget> list2 = new List<ICodexWidget>();
			CodexImage item2 = new CodexImage(64, 64, unlockedItem.getUISprite("ui", false));
			list2.Add(item2);
			CodexText item3 = new CodexText(unlockedItem.Name, CodexTextStyle.Body);
			list2.Add(item3);
			containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Horizontal));
		}
	}

	private static void GenerateRecipeContainers(Tag prefabID, List<ContentContainer> containers)
	{
		Recipe recipe = null;
		foreach (Recipe recipe2 in RecipeManager.Get().recipes)
		{
			if (recipe2.Result == prefabID)
			{
				recipe = recipe2;
				break;
			}
		}
		if (recipe != null)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexText(CODEX.HEADERS.RECIPE, CodexTextStyle.Subtitle),
				new CodexSpacer(),
				new CodexDividerLine()
			}, ContentContainer.ContentLayout.Vertical));
			Func<Recipe, List<ContentContainer>> func = delegate(Recipe rec)
			{
				List<ContentContainer> list = new List<ContentContainer>();
				foreach (Recipe.Ingredient ingredient in rec.Ingredients)
				{
					GameObject prefab = Assets.GetPrefab(ingredient.tag);
					if ((UnityEngine.Object)prefab != (UnityEngine.Object)null)
					{
						list.Add(new ContentContainer(new List<ICodexWidget>
						{
							new CodexImage(64, 64, Def.GetUISprite(prefab, "ui", false)),
							new CodexText(string.Format(UI.CODEX.RECIPE_ITEM, Assets.GetPrefab(ingredient.tag).GetProperName(), ingredient.amount, (ElementLoader.GetElement(ingredient.tag) != null) ? UI.UNITSUFFIXES.MASS.KILOGRAM.text : ""), CodexTextStyle.Body)
						}, ContentContainer.ContentLayout.Horizontal));
					}
				}
				return list;
			};
			containers.AddRange(func(recipe));
			GameObject gameObject = (recipe.fabricators != null) ? Assets.GetPrefab(recipe.fabricators[0]) : null;
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexText(UI.CODEX.RECIPE_FABRICATOR_HEADER, CodexTextStyle.Subtitle),
					new CodexDividerLine()
				}, ContentContainer.ContentLayout.Vertical));
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexImage(64, 64, Def.GetUISpriteFromMultiObjectAnim(gameObject.GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false, "")),
					new CodexText(string.Format(UI.CODEX.RECIPE_FABRICATOR, recipe.FabricationTime, gameObject.GetProperName()), CodexTextStyle.Body)
				}, ContentContainer.ContentLayout.Horizontal));
			}
		}
	}

	private static void GenerateUsedInRecipeContainers(Tag prefabID, List<ContentContainer> containers)
	{
		List<Recipe> list = new List<Recipe>();
		foreach (Recipe recipe in RecipeManager.Get().recipes)
		{
			foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
			{
				if (ingredient.tag == prefabID)
				{
					list.Add(recipe);
				}
			}
		}
		if (list.Count != 0)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexText(CODEX.HEADERS.USED_IN_RECIPES, CodexTextStyle.Subtitle),
				new CodexSpacer(),
				new CodexDividerLine()
			}, ContentContainer.ContentLayout.Vertical));
			foreach (Recipe item in list)
			{
				GameObject prefab = Assets.GetPrefab(item.Result);
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexImage(64, 64, Def.GetUISprite(prefab, "ui", false)),
					new CodexText(prefab.GetProperName(), CodexTextStyle.Body)
				}, ContentContainer.ContentLayout.Horizontal));
			}
		}
	}

	private static void GeneratePlantDescriptionContainers(GameObject plant, List<ContentContainer> containers)
	{
		SeedProducer component = plant.GetComponent<SeedProducer>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			GameObject prefab = Assets.GetPrefab(component.seedInfo.seedId);
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexText(CODEX.HEADERS.GROWNFROMSEED, CodexTextStyle.Subtitle),
				new CodexDividerLine()
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexImage(48, 48, Def.GetUISprite(prefab, "ui", false)),
				new CodexText(prefab.GetProperName(), CodexTextStyle.Body)
			}, ContentContainer.ContentLayout.Horizontal));
		}
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexSpacer());
		list.Add(new CodexText(UI.CODEX.DETAILS, CodexTextStyle.Subtitle));
		list.Add(new CodexDividerLine());
		InfoDescription component2 = Assets.GetPrefab(plant.PrefabID()).GetComponent<InfoDescription>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			list.Add(new CodexText(component2.description, CodexTextStyle.Body));
		}
		string text = "";
		List<Descriptor> plantRequirementDescriptors = GameUtil.GetPlantRequirementDescriptors(plant);
		if (plantRequirementDescriptors.Count > 0)
		{
			string str = text;
			Descriptor descriptor = plantRequirementDescriptors[0];
			text = str + descriptor.text;
			for (int i = 1; i < plantRequirementDescriptors.Count; i++)
			{
				string str2 = text;
				Descriptor descriptor2 = plantRequirementDescriptors[i];
				text = str2 + "\n    • " + descriptor2.text;
			}
			list.Add(new CodexText(text, CodexTextStyle.Body));
			list.Add(new CodexSpacer());
		}
		text = "";
		List<Descriptor> plantEffectDescriptors = GameUtil.GetPlantEffectDescriptors(plant);
		if (plantEffectDescriptors.Count > 0)
		{
			string str3 = text;
			Descriptor descriptor3 = plantEffectDescriptors[0];
			text = str3 + descriptor3.text;
			for (int j = 1; j < plantEffectDescriptors.Count; j++)
			{
				string str4 = text;
				Descriptor descriptor4 = plantEffectDescriptors[j];
				text = str4 + "\n    • " + descriptor4.text;
			}
			CodexText item = new CodexText(text, CodexTextStyle.Body);
			list.Add(item);
			list.Add(new CodexSpacer());
		}
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static ICodexWidget GetIconWidget(object entity)
	{
		return new CodexImage(32, 32, Def.GetUISprite(entity, "ui", false));
	}

	private static void GenerateCreatureDescriptionContainers(GameObject creature, List<ContentContainer> containers)
	{
		CreatureCalorieMonitor.Def def = creature.GetDef<CreatureCalorieMonitor.Def>();
		if (def != null)
		{
			List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag((creature.PrefabID() + "Egg").ToTag());
			if (prefabsWithTag != null && prefabsWithTag.Count > 0)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexText(CODEX.HEADERS.HATCHESFROMEGG, CodexTextStyle.Subtitle),
					new CodexDividerLine()
				}, ContentContainer.ContentLayout.Vertical));
				foreach (GameObject item2 in prefabsWithTag)
				{
					containers.Add(new ContentContainer(new List<ICodexWidget>
					{
						GetIconWidget(item2),
						new CodexText(item2.GetProperName(), CodexTextStyle.Body)
					}, ContentContainer.ContentLayout.Horizontal));
				}
			}
			TemperatureVulnerable component = creature.GetComponent<TemperatureVulnerable>();
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.COMFORTRANGE, CodexTextStyle.Subtitle),
				new CodexDividerLine(),
				new CodexText("    • " + string.Format(CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.COMFORT_RANGE, GameUtil.GetFormattedTemperature(component.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false) + " - " + GameUtil.GetFormattedTemperature(component.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), CodexTextStyle.Body),
				new CodexText("    • " + string.Format(CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.NON_LETHAL_RANGE, GameUtil.GetFormattedTemperature(component.internalTemperatureLethal_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false) + " - " + GameUtil.GetFormattedTemperature(component.internalTemperatureLethal_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), CodexTextStyle.Body),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical));
			List<Tag> list = new List<Tag>();
			if (def.diet.infos.Length > 0)
			{
				if (list.Count == 0)
				{
					containers.Add(new ContentContainer(new List<ICodexWidget>
					{
						new CodexText(CODEX.HEADERS.DIET, CodexTextStyle.Subtitle),
						new CodexDividerLine()
					}, ContentContainer.ContentLayout.Vertical));
				}
				ContentContainer contentContainer = new ContentContainer();
				contentContainer.contentLayout = ContentContainer.ContentLayout.Grid;
				contentContainer.content = new List<ICodexWidget>();
				Diet.Info[] infos = def.diet.infos;
				foreach (Diet.Info info in infos)
				{
					if (info.consumedTags.Count != 0)
					{
						foreach (Tag consumedTag in info.consumedTags)
						{
							Element element = ElementLoader.FindElementByHash(ElementLoader.GetElementID(consumedTag));
							GameObject gameObject = null;
							if (element.id == SimHashes.Vacuum || element.id == SimHashes.Void)
							{
								gameObject = Assets.GetPrefab(consumedTag);
								if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
								{
									continue;
								}
							}
							if (element != null && (UnityEngine.Object)gameObject == (UnityEngine.Object)null)
							{
								if (!list.Contains(element.tag))
								{
									list.Add(element.tag);
									contentContainer.content.Add(new CodexLabelWithIcon("    " + element.name, CodexTextStyle.Body, Def.GetUISprite(element.substance, "ui", false)));
								}
							}
							else if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && !list.Contains(gameObject.PrefabID()))
							{
								list.Add(gameObject.PrefabID());
								contentContainer.content.Add(new CodexLabelWithIcon("    " + gameObject.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject, "ui", false)));
							}
						}
					}
				}
				containers.Add(contentContainer);
			}
			bool flag = false;
			if (def.diet != null)
			{
				Diet.Info[] infos2 = def.diet.infos;
				foreach (Diet.Info info2 in infos2)
				{
					if (info2.producedElement != (Tag)null)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					ContentContainer contentContainer2 = new ContentContainer();
					contentContainer2.contentLayout = ContentContainer.ContentLayout.Grid;
					contentContainer2.content = new List<ICodexWidget>();
					List<ICodexWidget> list2 = new List<ICodexWidget>();
					list2.Add(new CodexText(CODEX.HEADERS.PRODUCES, CodexTextStyle.Subtitle));
					list2.Add(new CodexDividerLine());
					ContentContainer item = new ContentContainer(list2, ContentContainer.ContentLayout.Vertical);
					containers.Add(item);
					List<Tag> list3 = new List<Tag>();
					for (int k = 0; k < def.diet.infos.Length; k++)
					{
						if (def.diet.infos[k].producedElement != Tag.Invalid && !list3.Contains(def.diet.infos[k].producedElement))
						{
							list3.Add(def.diet.infos[k].producedElement);
							contentContainer2.content.Add(new CodexLabelWithIcon("• " + def.diet.infos[k].producedElement.ProperName(), CodexTextStyle.Body, Def.GetUISprite(def.diet.infos[k].producedElement, "ui", false)));
						}
					}
					containers.Add(contentContainer2);
				}
			}
		}
	}

	private static void GenerateDiseaseDescriptionContainers(Disease disease, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexSpacer());
		foreach (Descriptor quantitativeDescriptor in disease.GetQuantitativeDescriptors())
		{
			Descriptor current = quantitativeDescriptor;
			list.Add(new CodexText(current.text, CodexTextStyle.Body));
		}
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateFoodDescriptionContainers(EdiblesManager.FoodInfo food, List<ContentContainer> containers)
	{
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(food.Description, CodexTextStyle.Body),
			new CodexSpacer(),
			new CodexText(string.Format(UI.CODEX.FOOD.QUALITY, GameUtil.GetFormattedFoodQuality(food.Quality)), CodexTextStyle.Body),
			new CodexText(string.Format(UI.CODEX.FOOD.CALORIES, GameUtil.GetFormattedCalories(food.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), CodexTextStyle.Body),
			new CodexSpacer(),
			new CodexText((!food.CanRot) ? UI.CODEX.FOOD.NON_PERISHABLE.ToString() : string.Format(UI.CODEX.FOOD.SPOILPROPERTIES, GameUtil.GetFormattedTemperature(food.PreserveTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedCycles(food.SpoilTime, "F1")), CodexTextStyle.Body),
			new CodexSpacer()
		}, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateTechDescriptionContainers(Tech tech, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText item = new CodexText(Strings.Get("STRINGS.RESEARCH.TECHS." + tech.Id.ToUpper() + ".DESC"), CodexTextStyle.Body);
		list.Add(item);
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateGenericDescriptionContainers(string description, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText item = new CodexText(description, CodexTextStyle.Body);
		list.Add(item);
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateBuildingDescriptionContainers(BuildingDef def, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexText(Strings.Get("STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".EFFECT"), CodexTextStyle.Body));
		list.Add(new CodexText(Strings.Get("STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".DESC"), CodexTextStyle.Body));
		Tech tech = Db.Get().TechItems.LookupGroupForID(def.PrefabID);
		if (tech != null)
		{
			list.Add(new CodexText(string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, tech.Name), CodexTextStyle.Body));
		}
		list.Add(new CodexSpacer());
		List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(def);
		List<Descriptor> effectDescriptors = GameUtil.GetEffectDescriptors(allDescriptors);
		if (effectDescriptors.Count > 0)
		{
			list.Add(new CodexText(CODEX.HEADERS.BUILDINGEFFECTS, CodexTextStyle.Subtitle));
			list.Add(new CodexDividerLine());
			foreach (Descriptor item in effectDescriptors)
			{
				Descriptor current = item;
				list.Add(new CodexText(current.text, CodexTextStyle.Body));
			}
			list.Add(new CodexSpacer());
		}
		List<Descriptor> requirementDescriptors = GameUtil.GetRequirementDescriptors(allDescriptors);
		if (requirementDescriptors.Count > 0)
		{
			list.Add(new CodexText(CODEX.HEADERS.BUILDINGREQUIREMENTS, CodexTextStyle.Subtitle));
			list.Add(new CodexDividerLine());
			foreach (Descriptor item2 in requirementDescriptors)
			{
				Descriptor current2 = item2;
				list.Add(new CodexText(current2.text, CodexTextStyle.Body));
			}
			list.Add(new CodexSpacer());
		}
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateImageContainers(Sprite[] sprites, List<ContentContainer> containers, ContentContainer.ContentLayout layout)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		foreach (Sprite sprite in sprites)
		{
			if (!((UnityEngine.Object)sprite == (UnityEngine.Object)null))
			{
				CodexImage item = new CodexImage(128, 128, sprite);
				list.Add(item);
			}
		}
		containers.Add(new ContentContainer(list, layout));
	}

	private static void GenerateImageContainers(Tuple<Sprite, Color>[] sprites, List<ContentContainer> containers, ContentContainer.ContentLayout layout)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		foreach (Tuple<Sprite, Color> tuple in sprites)
		{
			if (tuple != null)
			{
				CodexImage item = new CodexImage(128, 128, tuple);
				list.Add(item);
			}
		}
		containers.Add(new ContentContainer(list, layout));
	}

	private static void GenerateImageContainers(Sprite sprite, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexImage item = new CodexImage(128, 128, sprite);
		list.Add(item);
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	public static void CreateUnlockablesContentContainer(SubEntry subentry)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexText(CODEX.HEADERS.SECTION_UNLOCKABLES, CodexTextStyle.Subtitle));
		list.Add(new CodexDividerLine());
		ContentContainer contentContainer = new ContentContainer(list, ContentContainer.ContentLayout.Vertical);
		contentContainer.showBeforeGeneratedContent = false;
		subentry.lockedContentContainer = contentContainer;
	}

	private static void GenerateFabricatorContainers(GameObject entity, List<ContentContainer> containers)
	{
		ComplexFabricator component = entity.GetComponent<ComplexFabricator>();
		if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
		{
			List<ICodexWidget> list = new List<ICodexWidget>();
			list.Add(new CodexText(Strings.Get("STRINGS.CODEX.HEADERS.FABRICATIONS"), CodexTextStyle.Subtitle));
			list.Add(new CodexDividerLine());
			containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
			ComplexRecipe[] recipes = component.GetRecipes();
			foreach (ComplexRecipe complexRecipe in recipes)
			{
				GameObject prefab = Assets.GetPrefab(complexRecipe.results[0].material);
				List<ICodexWidget> list2 = new List<ICodexWidget>();
				CodexImage item = new CodexImage(64, 64, Def.GetUISprite(prefab, "ui", false));
				list2.Add(item);
				CodexText item2 = new CodexText(prefab.GetProperName(), CodexTextStyle.Body);
				list2.Add(item2);
				containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Horizontal));
			}
		}
	}

	private static void GenerateReceptacleContainers(GameObject entity, List<ContentContainer> containers)
	{
		SingleEntityReceptacle plot = entity.GetComponent<SingleEntityReceptacle>();
		if (!((UnityEngine.Object)plot == (UnityEngine.Object)null))
		{
			List<ICodexWidget> list = new List<ICodexWidget>();
			list.Add(new CodexText(Strings.Get("STRINGS.CODEX.HEADERS.RECEPTACLE"), CodexTextStyle.Subtitle));
			list.Add(new CodexDividerLine());
			containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
			Tag[] possibleDepositObjectTags = plot.possibleDepositObjectTags;
			foreach (Tag tag in possibleDepositObjectTags)
			{
				List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(tag);
				if ((UnityEngine.Object)plot.rotatable == (UnityEngine.Object)null)
				{
					prefabsWithTag.RemoveAll(delegate(GameObject go)
					{
						IReceptacleDirection component = go.GetComponent<IReceptacleDirection>();
						return component != null && component.Direction != plot.Direction;
					});
				}
				foreach (GameObject item in prefabsWithTag)
				{
					List<ICodexWidget> list2 = new List<ICodexWidget>();
					list2.Add(new CodexImage(64, 64, Def.GetUISprite(item, "ui", false).first));
					list2.Add(new CodexText(item.GetProperName(), CodexTextStyle.Body));
					containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Horizontal));
				}
			}
		}
	}
}
