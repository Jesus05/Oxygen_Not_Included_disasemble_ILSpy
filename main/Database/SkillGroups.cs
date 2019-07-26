using Klei.AI;
using STRINGS;
using System.Collections.Generic;

namespace Database
{
	public class SkillGroups : ResourceSet<SkillGroup>
	{
		public SkillGroup Mining;

		public SkillGroup Building;

		public SkillGroup Farming;

		public SkillGroup Ranching;

		public SkillGroup Cooking;

		public SkillGroup Art;

		public SkillGroup Research;

		public SkillGroup Suits;

		public SkillGroup Hauling;

		public SkillGroup Technicals;

		public SkillGroup MedicalAid;

		public SkillGroup Basekeeping;

		public SkillGroups(ResourceSet parent)
			: base("SkillGroups", parent)
		{
			Mining = Add(new SkillGroup("Mining", Db.Get().ChoreGroups.Dig.Id, DUPLICANTS.CHOREGROUPS.DIG.NAME, "icon_errand_dig", "icon_archetype_dig"));
			Mining.relevantAttributes = new List<Attribute>
			{
				Db.Get().ChoreGroups.Dig.attribute
			};
			Mining.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Dig.Id
			};
			Building = Add(new SkillGroup("Building", Db.Get().ChoreGroups.Build.Id, DUPLICANTS.CHOREGROUPS.BUILD.NAME, "status_item_pending_repair", "icon_archetype_build"));
			Building.relevantAttributes = new List<Attribute>
			{
				Db.Get().ChoreGroups.Build.attribute
			};
			Building.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Build.Id
			};
			Farming = Add(new SkillGroup("Farming", Db.Get().ChoreGroups.Farming.Id, DUPLICANTS.CHOREGROUPS.FARMING.NAME, "icon_errand_farm", "icon_archetype_farm"));
			Farming.relevantAttributes = new List<Attribute>
			{
				Db.Get().ChoreGroups.Farming.attribute
			};
			Farming.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Farming.Id
			};
			Ranching = Add(new SkillGroup("Ranching", Db.Get().ChoreGroups.Ranching.Id, DUPLICANTS.CHOREGROUPS.RANCHING.NAME, "icon_errand_ranch", "icon_archetype_ranch"));
			Ranching.relevantAttributes = new List<Attribute>
			{
				Db.Get().ChoreGroups.Ranching.attribute
			};
			Ranching.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Ranching.Id
			};
			Cooking = Add(new SkillGroup("Cooking", Db.Get().ChoreGroups.Cook.Id, DUPLICANTS.CHOREGROUPS.COOK.NAME, "icon_errand_cook", "icon_archetype_cook"));
			Cooking.relevantAttributes = new List<Attribute>
			{
				Db.Get().ChoreGroups.Cook.attribute
			};
			Cooking.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Cook.Id
			};
			Art = Add(new SkillGroup("Art", Db.Get().ChoreGroups.Art.Id, DUPLICANTS.CHOREGROUPS.ART.NAME, "icon_errand_art", "icon_archetype_art"));
			Art.relevantAttributes = new List<Attribute>
			{
				Db.Get().ChoreGroups.Art.attribute
			};
			Art.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Art.Id
			};
			Research = Add(new SkillGroup("Research", Db.Get().ChoreGroups.Research.Id, DUPLICANTS.CHOREGROUPS.RESEARCH.NAME, "icon_errand_research", "icon_archetype_research"));
			Research.relevantAttributes = new List<Attribute>
			{
				Db.Get().ChoreGroups.Research.attribute
			};
			Research.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Research.Id
			};
			Suits = Add(new SkillGroup("Suits", "", DUPLICANTS.ROLES.GROUPS.SUITS, "suit_overlay_icon", "icon_archetype_astronaut"));
			Suits.relevantAttributes = new List<Attribute>
			{
				Db.Get().Attributes.Athletics
			};
			Suits.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Hauling.Id
			};
			Hauling = Add(new SkillGroup("Hauling", Db.Get().ChoreGroups.Hauling.Id, DUPLICANTS.CHOREGROUPS.HAULING.NAME, "icon_errand_supply", "icon_archetype_storage"));
			Hauling.relevantAttributes = new List<Attribute>
			{
				Db.Get().ChoreGroups.Hauling.attribute
			};
			Hauling.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Hauling.Id
			};
			Technicals = Add(new SkillGroup("Technicals", Db.Get().ChoreGroups.MachineOperating.Id, DUPLICANTS.CHOREGROUPS.MACHINEOPERATING.NAME, "icon_errand_operate", "icon_archetype_operate"));
			Technicals.relevantAttributes = new List<Attribute>
			{
				Db.Get().ChoreGroups.MachineOperating.attribute
			};
			Technicals.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.MachineOperating.Id
			};
			MedicalAid = Add(new SkillGroup("MedicalAid", Db.Get().ChoreGroups.MedicalAid.Id, DUPLICANTS.CHOREGROUPS.MEDICALAID.NAME, "icon_errand_care", "icon_archetype_care"));
			MedicalAid.relevantAttributes = new List<Attribute>
			{
				Db.Get().ChoreGroups.MedicalAid.attribute
			};
			Basekeeping = Add(new SkillGroup("Basekeeping", Db.Get().ChoreGroups.Basekeeping.Id, DUPLICANTS.CHOREGROUPS.BASEKEEPING.NAME, "icon_errand_tidy", "icon_archetype_tidy"));
			Basekeeping.relevantAttributes = new List<Attribute>
			{
				Db.Get().ChoreGroups.Basekeeping.attribute
			};
			Basekeeping.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Basekeeping.Id
			};
		}
	}
}
