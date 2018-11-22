using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class JetSuitConfig : IEquipmentConfig
{
	public const string ID = "Jet_Suit";

	public static ComplexRecipe recipe;

	private const PathFinder.PotentialPath.Flags suit_flags = PathFinder.PotentialPath.Flags.HasJetPack;

	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add((-899253461).ToString(), 200f);
		dictionary.Add((-486269331).ToString(), 25f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_DIGGING, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, TUNING.EQUIPMENT.SUITS.ATMOSUIT_BLADDER, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		string id = "Jet_Suit";
		string sLOT = TUNING.EQUIPMENT.SUITS.SLOT;
		SimHashes outputElement = SimHashes.Steel;
		float mass = (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS;
		List<AttributeModifier> attributeModifiers = list;
		Tag[] additional_tags = new Tag[1]
		{
			GameTags.Suit
		};
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(id, sLOT, outputElement, mass, "suit_jetpack_kanim", "", "body_jetpack_kanim", 6, attributeModifiers, null, true, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, additional_tags, "JetSuit");
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC;
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("SoakingWet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("WetFeet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("PoppedEarDrums"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("Unclean"));
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			Ownables soleOwner2 = eq.assignee.GetSoleOwner();
			if ((Object)soleOwner2 != (Object)null)
			{
				GameObject targetGameObject2 = soleOwner2.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				Navigator component2 = targetGameObject2.GetComponent<Navigator>();
				if ((Object)component2 != (Object)null)
				{
					component2.SetFlags(PathFinder.PotentialPath.Flags.HasJetPack);
				}
				MinionResume component3 = targetGameObject2.GetComponent<MinionResume>();
				if ((Object)component3 != (Object)null && component3.HasPerk(RoleManager.rolePerks.ExosuitExpertise.id))
				{
					targetGameObject2.GetAttributes().Get(Db.Get().Attributes.Athletics).Add(SuitExpert.AthleticsModifier);
				}
			}
		};
		equipmentDef.OnUnequipCallBack = delegate(Equippable eq)
		{
			if (eq.assignee != null)
			{
				Ownables soleOwner = eq.assignee.GetSoleOwner();
				GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				targetGameObject.GetAttributes()?.Get(Db.Get().Attributes.Athletics).Remove(SuitExpert.AthleticsModifier);
				Navigator component = targetGameObject.GetComponent<Navigator>();
				if ((Object)component != (Object)null)
				{
					component.ClearFlags(PathFinder.PotentialPath.Flags.HasJetPack);
				}
			}
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Jet_Suit");
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Oxygen";
		suitTank.capacity = 75f;
		go.AddComponent<JetSuitTank>();
		HelmetController helmetController = go.AddComponent<HelmetController>();
		helmetController.anim_file = "helm_jetpack_kanim";
		helmetController.has_jets = true;
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Clothes);
		component.AddTag(GameTags.PedestalDisplayable);
		go.AddComponent<SuitDiseaseHandler>();
	}
}
