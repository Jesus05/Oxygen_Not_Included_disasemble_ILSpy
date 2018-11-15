using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class AquaSuitConfig : IEquipmentConfig
{
	public const string ID = "Aqua_Suit";

	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add(1832607973.ToString(), 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)TUNING.EQUIPMENT.SUITS.AQUASUIT_INSULATION, STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)TUNING.EQUIPMENT.SUITS.AQUASUIT_ATHLETICS, STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.MAX_UNDERWATER_TRAVELCOST, (float)TUNING.EQUIPMENT.SUITS.AQUASUIT_UNDERWATER_TRAVELCOST, STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false, true));
		string id = "Aqua_Suit";
		string sLOT = TUNING.EQUIPMENT.SUITS.SLOT;
		string fABRICATOR = TUNING.EQUIPMENT.SUITS.FABRICATOR;
		float fabricationTime = (float)TUNING.EQUIPMENT.SUITS.AQUASUIT_FABTIME;
		SimHashes outputElement = SimHashes.Water;
		Dictionary<string, float> inputElementMassMap = dictionary;
		float mass = (float)TUNING.EQUIPMENT.SUITS.AQUASUIT_MASS;
		string anim = "suit_water_slow_kanim";
		string sNAPON = TUNING.EQUIPMENT.SUITS.SNAPON;
		string buildOverride = "body_water_slow_kanim";
		int buildOverridePriority = 6;
		List<AttributeModifier> attributeModifiers = list;
		Tag[] additional_tags = new Tag[1]
		{
			GameTags.Suit
		};
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(id, sLOT, fABRICATOR, fabricationTime, outputElement, inputElementMassMap, mass, anim, sNAPON, buildOverride, buildOverridePriority, attributeModifiers, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, additional_tags, null);
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.underwaterSupport = true;
		suitTank.element = "Oxygen";
		suitTank.amount = 11f;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes);
	}
}
