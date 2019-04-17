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
			Mining = Add(new SkillGroup("Mining", Db.Get().ChoreGroups.Dig.Id, DUPLICANTS.CHOREGROUPS.DIG.NAME));
			Mining.relevantAttributes = new List<string>
			{
				Db.Get().Attributes.Digging.Id
			};
			Mining.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Dig.Id
			};
			Building = Add(new SkillGroup("Building", Db.Get().ChoreGroups.Build.Id, DUPLICANTS.CHOREGROUPS.BUILD.NAME));
			Building.relevantAttributes = new List<string>
			{
				Db.Get().Attributes.Construction.Id
			};
			Building.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Build.Id
			};
			Farming = Add(new SkillGroup("Farming", Db.Get().ChoreGroups.Farming.Id, DUPLICANTS.CHOREGROUPS.FARMING.NAME));
			Farming.relevantAttributes = new List<string>
			{
				Db.Get().Attributes.Botanist.Id
			};
			Farming.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Farming.Id
			};
			Ranching = Add(new SkillGroup("Ranching", Db.Get().ChoreGroups.Ranching.Id, DUPLICANTS.CHOREGROUPS.RANCHING.NAME));
			Ranching.relevantAttributes = new List<string>
			{
				Db.Get().Attributes.Ranching.Id
			};
			Ranching.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Ranching.Id
			};
			Cooking = Add(new SkillGroup("Cooking", Db.Get().ChoreGroups.Cook.Id, DUPLICANTS.CHOREGROUPS.COOK.NAME));
			Cooking.relevantAttributes = new List<string>
			{
				Db.Get().Attributes.Cooking.Id
			};
			Cooking.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Cook.Id
			};
			Art = Add(new SkillGroup("Art", Db.Get().ChoreGroups.Art.Id, DUPLICANTS.CHOREGROUPS.ART.NAME));
			Art.relevantAttributes = new List<string>
			{
				Db.Get().Attributes.Art.Id
			};
			Art.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Art.Id
			};
			Research = Add(new SkillGroup("Research", Db.Get().ChoreGroups.Research.Id, DUPLICANTS.CHOREGROUPS.RESEARCH.NAME));
			Research.relevantAttributes = new List<string>
			{
				Db.Get().Attributes.Learning.Id
			};
			Research.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Research.Id
			};
			Suits = Add(new SkillGroup("Suits", string.Empty, DUPLICANTS.ROLES.GROUPS.SUITS));
			Suits.relevantAttributes = new List<string>
			{
				Db.Get().Attributes.Athletics.Id
			};
			Suits.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Hauling.Id
			};
			Hauling = Add(new SkillGroup("Hauling", Db.Get().ChoreGroups.Hauling.Id, DUPLICANTS.CHOREGROUPS.HAULING.NAME));
			Hauling.relevantAttributes = new List<string>
			{
				Db.Get().Attributes.Athletics.Id
			};
			Hauling.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Hauling.Id
			};
			Technicals = Add(new SkillGroup("Technicals", Db.Get().ChoreGroups.MachineOperating.Id, DUPLICANTS.CHOREGROUPS.MACHINEOPERATING.NAME));
			Technicals.relevantAttributes = new List<string>
			{
				Db.Get().Attributes.Machinery.Id
			};
			Technicals.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.MachineOperating.Id
			};
			MedicalAid = Add(new SkillGroup("MedicalAid", Db.Get().ChoreGroups.MedicalAid.Id, DUPLICANTS.CHOREGROUPS.MEDICALAID.NAME));
			Basekeeping = Add(new SkillGroup("Basekeeping", Db.Get().ChoreGroups.Basekeeping.Id, DUPLICANTS.CHOREGROUPS.BASEKEEPING.NAME));
			Basekeeping.relevantAttributes = new List<string>
			{
				Db.Get().Attributes.Athletics.Id
			};
			Basekeeping.requiredChoreGroups = new List<string>
			{
				Db.Get().ChoreGroups.Basekeeping.Id
			};
		}
	}
}
