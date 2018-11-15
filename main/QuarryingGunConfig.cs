using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class QuarryingGunConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add("IronOre", 200f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.DIGGING, (float)TUNING.EQUIPMENT.TOOLS.QUARRYINGGUN_DIG, STRINGS.EQUIPMENT.PREFABS.QUARRYING_GUN.NAME, false, false, true));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("QuarryingGun", TUNING.EQUIPMENT.TOOLS.TOOLSLOT, TUNING.EQUIPMENT.TOOLS.TOOLFABRICATOR, (float)TUNING.EQUIPMENT.TOOLS.QUARRYINGGUN_FABTIME, SimHashes.IronOre, dictionary, (float)TUNING.EQUIPMENT.TOOLS.QUARRYINGGUN_MASS, "constructor_gun_kanim", string.Empty, string.Empty, 4, list, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, null, null);
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.QUARRYING_GUN.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
	}
}
