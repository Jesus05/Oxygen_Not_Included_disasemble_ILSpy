using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BoringMachineConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add("Iron", 50f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.DIGGING, (float)TUNING.EQUIPMENT.TOOLS.BORINGMACHINE_DIG, STRINGS.EQUIPMENT.PREFABS.BORING_MACHINE.NAME, false, false, true));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("BoringMachine", TUNING.EQUIPMENT.TOOLS.TOOLSLOT, TUNING.EQUIPMENT.TOOLS.TOOLFABRICATOR, (float)TUNING.EQUIPMENT.TOOLS.BORINGMACHINE_FABTIME, SimHashes.Iron, dictionary, (float)TUNING.EQUIPMENT.TOOLS.BORINGMACHINE_MASS, TUNING.EQUIPMENT.TOOLS.TOOL_ANIM, string.Empty, string.Empty, 4, list, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, null, null);
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.BORING_MACHINE.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
	}
}
