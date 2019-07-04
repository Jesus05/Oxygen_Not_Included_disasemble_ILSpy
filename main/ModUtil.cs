using KMod;
using System;
using System.Collections.Generic;
using System.IO;
using TUNING;
using UnityEngine;

public static class ModUtil
{
	public static void AddBuildingToPlanScreen(HashedString category, string building_id)
	{
		int num = BUILDINGS.PLANORDER.FindIndex((PlanScreen.PlanInfo x) => x.category == category);
		if (num >= 0)
		{
			PlanScreen.PlanInfo planInfo = BUILDINGS.PLANORDER[num];
			IList<string> list = planInfo.data as IList<string>;
			list.Add(building_id);
		}
	}

	public static void AddBuildingToHotkeyBuildMenu(HashedString category, string building_id, Action hotkey)
	{
		BuildMenu.DisplayInfo info = BuildMenu.OrderedBuildings.GetInfo(category);
		if (!(info.category != category))
		{
			IList<BuildMenu.BuildingInfo> list = info.data as IList<BuildMenu.BuildingInfo>;
			list.Add(new BuildMenu.BuildingInfo(building_id, hotkey));
		}
	}

	public static KAnimFile AddKAnim(string name, TextAsset anim_file, TextAsset build_file, IList<Texture2D> textures)
	{
		KAnimFile kAnimFile = ScriptableObject.CreateInstance<KAnimFile>();
		kAnimFile.animFile = anim_file;
		kAnimFile.buildFile = build_file;
		kAnimFile.textures.AddRange(textures);
		kAnimFile.name = name;
		AnimCommandFile animCommandFile = new AnimCommandFile();
		KAnimGroupFile.GroupFile groupFile = new KAnimGroupFile.GroupFile();
		groupFile.groupID = animCommandFile.GetGroupName(kAnimFile);
		groupFile.commandDirectory = "assets/" + name;
		animCommandFile.AddGroupFile(groupFile);
		KAnimGroupFile groupFile2 = KAnimGroupFile.GetGroupFile();
		groupFile2.AddAnimFile(groupFile, animCommandFile, kAnimFile);
		Assets.ModLoadedKAnims.Add(kAnimFile);
		return kAnimFile;
	}

	public static KAnimFile AddKAnim(string name, TextAsset anim_file, TextAsset build_file, Texture2D texture)
	{
		KAnimFile kAnimFile = ScriptableObject.CreateInstance<KAnimFile>();
		kAnimFile.animFile = anim_file;
		kAnimFile.buildFile = build_file;
		kAnimFile.textures.Add(texture);
		kAnimFile.name = name;
		AnimCommandFile animCommandFile = new AnimCommandFile();
		KAnimGroupFile.GroupFile groupFile = new KAnimGroupFile.GroupFile();
		groupFile.groupID = animCommandFile.GetGroupName(kAnimFile);
		groupFile.commandDirectory = "assets/" + name;
		animCommandFile.AddGroupFile(groupFile);
		KAnimGroupFile groupFile2 = KAnimGroupFile.GetGroupFile();
		groupFile2.AddAnimFile(groupFile, animCommandFile, kAnimFile);
		Assets.ModLoadedKAnims.Add(kAnimFile);
		return kAnimFile;
	}

	public static Substance CreateSubstance(string name, Element.State state, KAnimFile kanim, Material material, Color32 colour, Color32 ui_colour, Color32 conduit_colour)
	{
		Substance substance = new Substance();
		substance.name = name;
		substance.nameTag = TagManager.Create(name);
		substance.elementID = (SimHashes)Hash.SDBMLower(name);
		substance.anim = kanim;
		substance.colour = colour;
		substance.uiColour = ui_colour;
		substance.conduitColour = conduit_colour;
		substance.material = material;
		substance.renderedByWorld = ((state & Element.State.Solid) == Element.State.Solid);
		return substance;
	}

	public static void RegisterForTranslation(Type locstring_tree_root)
	{
		Localization.RegisterForTranslation(locstring_tree_root);
		Localization.GenerateStringsTemplate(locstring_tree_root, Path.Combine(Manager.GetDirectory(), "strings_templates"));
	}
}
