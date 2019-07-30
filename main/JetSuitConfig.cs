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

	private AttributeModifier expertAthleticsModifier;

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
		list.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		expertAthleticsModifier = new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)(-TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS), Db.Get().Skills.Suits1.Name, false, false, true);
		string id = "Jet_Suit";
		string sLOT = TUNING.EQUIPMENT.SUITS.SLOT;
		SimHashes outputElement = SimHashes.Steel;
		float mass = (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS;
		List<AttributeModifier> attributeModifiers = list;
		Tag[] additional_tags = new Tag[1]
		{
			GameTags.Suit
		};
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(id, sLOT, outputElement, mass, "suit_jetpack_kanim", string.Empty, "body_jetpack_kanim", 6, attributeModifiers, null, true, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, additional_tags, "JetSuit");
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC;
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("SoakingWet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("WetFeet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("PoppedEarDrums"));
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			Ownables soleOwner2 = eq.assignee.GetSoleOwner();
			if ((Object)soleOwner2 != (Object)null)
			{
				GameObject targetGameObject2 = soleOwner2.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				Navigator component4 = targetGameObject2.GetComponent<Navigator>();
				if ((Object)component4 != (Object)null)
				{
					component4.SetFlags(PathFinder.PotentialPath.Flags.HasJetPack);
				}
				MinionResume component5 = targetGameObject2.GetComponent<MinionResume>();
				if ((Object)component5 != (Object)null && component5.HasPerk(Db.Get().SkillPerks.ExosuitExpertise.Id))
				{
					targetGameObject2.GetAttributes().Get(Db.Get().Attributes.Athletics).Add(expertAthleticsModifier);
				}
				KAnimControllerBase component6 = targetGameObject2.GetComponent<KAnimControllerBase>();
				if ((bool)component6)
				{
					component6.AddAnimOverrides(Assets.GetAnim("anim_loco_hover_kanim"), 0f);
				}
			}
		};
		equipmentDef.OnUnequipCallBack = delegate(Equippable eq)
		{
			if (eq.assignee != null)
			{
				Ownables soleOwner = eq.assignee.GetSoleOwner();
				if ((bool)soleOwner)
				{
					GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
					if ((bool)targetGameObject)
					{
						targetGameObject.GetAttributes()?.Get(Db.Get().Attributes.Athletics).Remove(expertAthleticsModifier);
						Navigator component = targetGameObject.GetComponent<Navigator>();
						if ((Object)component != (Object)null)
						{
							component.ClearFlags(PathFinder.PotentialPath.Flags.HasJetPack);
						}
						KAnimControllerBase component2 = targetGameObject.GetComponent<KAnimControllerBase>();
						if ((bool)component2)
						{
							component2.RemoveAnimOverrides(Assets.GetAnim("anim_loco_hover_kanim"));
						}
						Effects component3 = targetGameObject.GetComponent<Effects>();
						if (component3.HasEffect("SoiledSuit"))
						{
							component3.Remove("SoiledSuit");
						}
					}
					eq.GetComponent<Storage>().DropAll(eq.transform.GetPosition(), true, true, default(Vector3), false);
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
		helmetController.has_jets = true;
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Clothes, false);
		component.AddTag(GameTags.PedestalDisplayable, false);
		component.AddTag(GameTags.AirtightSuit, false);
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		storage.showInUI = true;
		go.AddOrGet<AtmoSuit>();
		go.AddComponent<SuitDiseaseHandler>();
	}
}
