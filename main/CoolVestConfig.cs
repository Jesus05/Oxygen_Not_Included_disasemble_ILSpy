using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TUNING;
using UnityEngine;

public class CoolVestConfig : IEquipmentConfig
{
	public const string ID = "Cool_Vest";

	public static ComplexRecipe recipe;

	[CompilerGenerated]
	private static Action<Equippable> _003C_003Ef__mg_0024cache0;

	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add("BasicFabric", (float)TUNING.EQUIPMENT.VESTS.COOL_VEST_MASS);
		ClothingWearer.ClothingInfo clothingInfo = ClothingWearer.ClothingInfo.COOL_CLOTHING;
		List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Cool_Vest", TUNING.EQUIPMENT.CLOTHING.SLOT, SimHashes.Carbon, (float)TUNING.EQUIPMENT.VESTS.COOL_VEST_MASS, TUNING.EQUIPMENT.VESTS.COOL_VEST_ICON0, TUNING.EQUIPMENT.VESTS.SNAPON0, TUNING.EQUIPMENT.VESTS.COOL_VEST_ANIM0, 4, attributeModifiers, TUNING.EQUIPMENT.VESTS.SNAPON1, true, EntityTemplates.CollisionShape.RECTANGLE, 0.75f, 0.4f, null, null);
		Descriptor item = new Descriptor($"{DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME}: {GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.COOL_CLOTHING.conductivityMod)}", $"{DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME}: {GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.COOL_CLOTHING.conductivityMod)}", Descriptor.DescriptorType.Effect, false);
		Descriptor item2 = new Descriptor($"{DUPLICANTS.ATTRIBUTES.DECOR.NAME}: {ClothingWearer.ClothingInfo.COOL_CLOTHING.decorMod}", $"{DUPLICANTS.ATTRIBUTES.DECOR.NAME}: {ClothingWearer.ClothingInfo.COOL_CLOTHING.decorMod}", Descriptor.DescriptorType.Effect, false);
		equipmentDef.additionalDescriptors.Add(item);
		equipmentDef.additionalDescriptors.Add(item2);
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			OnEquipVest(eq, clothingInfo);
		};
		equipmentDef.OnUnequipCallBack = OnUnequipVest;
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.COOL_VEST.RECIPE_DESC;
		return equipmentDef;
	}

	public static void OnEquipVest(Equippable eq, ClothingWearer.ClothingInfo clothingInfo)
	{
		if (!((UnityEngine.Object)eq == (UnityEngine.Object)null) && eq.assignee != null)
		{
			Ownables soleOwner = eq.assignee.GetSoleOwner();
			if (!((UnityEngine.Object)soleOwner == (UnityEngine.Object)null))
			{
				MinionAssignablesProxy component = soleOwner.GetComponent<MinionAssignablesProxy>();
				ClothingWearer component2 = (component.target as KMonoBehaviour).GetComponent<ClothingWearer>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
				{
					component2.ChangeClothes(clothingInfo);
				}
				else
				{
					Debug.LogWarning("Clothing item cannot be equipped to assignee because they lack ClothingWearer component", null);
				}
			}
		}
	}

	public static void OnUnequipVest(Equippable eq)
	{
		if ((UnityEngine.Object)eq != (UnityEngine.Object)null && eq.assignee != null)
		{
			Ownables soleOwner = eq.assignee.GetSoleOwner();
			if ((UnityEngine.Object)soleOwner != (UnityEngine.Object)null)
			{
				ClothingWearer component = soleOwner.GetComponent<ClothingWearer>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					component.ChangeToDefaultClothes();
				}
			}
		}
	}

	public static void SetupVest(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes);
		Equippable equippable = go.GetComponent<Equippable>();
		if ((UnityEngine.Object)equippable == (UnityEngine.Object)null)
		{
			equippable = go.AddComponent<Equippable>();
		}
		equippable.SetQuality(QualityLevel.Poor);
		go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
	}

	public void DoPostConfigure(GameObject go)
	{
		SetupVest(go);
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.PedestalDisplayable);
	}
}
