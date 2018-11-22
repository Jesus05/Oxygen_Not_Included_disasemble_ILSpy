using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class TemperatureSuitConfig : IEquipmentConfig
{
	public const string ID = "Temperature_Suit";

	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add(873952427.ToString(), 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_INSULATION, STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_ATHLETICS, STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.NAME, false, false, true));
		string id = "Temperature_Suit";
		string sLOT = TUNING.EQUIPMENT.SUITS.SLOT;
		SimHashes outputElement = SimHashes.Water;
		float mass = (float)TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_MASS;
		string aNIM = TUNING.EQUIPMENT.SUITS.ANIM;
		string sNAPON = TUNING.EQUIPMENT.SUITS.SNAPON;
		string buildOverride = "body_oxygen_kanim";
		int buildOverridePriority = 6;
		List<AttributeModifier> attributeModifiers = list;
		Tag[] additional_tags = new Tag[1]
		{
			GameTags.Suit
		};
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(id, sLOT, outputElement, mass, aNIM, sNAPON, buildOverride, buildOverridePriority, attributeModifiers, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, additional_tags, null);
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Water";
		suitTank.amount = 100f;
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.PedestalDisplayable);
	}
}
