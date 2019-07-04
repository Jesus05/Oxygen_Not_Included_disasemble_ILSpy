using Klei.AI;
using STRINGS;

namespace Database
{
	public class ChoreGroups : ResourceSet<ChoreGroup>
	{
		public ChoreGroup Build;

		public ChoreGroup Basekeeping;

		public ChoreGroup Cook;

		public ChoreGroup Art;

		public ChoreGroup Dig;

		public ChoreGroup Research;

		public ChoreGroup Farming;

		public ChoreGroup Ranching;

		public ChoreGroup Hauling;

		public ChoreGroup Storage;

		public ChoreGroup MachineOperating;

		public ChoreGroup MedicalAid;

		public ChoreGroup Combat;

		public ChoreGroup LifeSupport;

		public ChoreGroup Toggle;

		public ChoreGroups(ResourceSet parent)
			: base("ChoreGroups", parent)
		{
			Combat = Add("Combat", DUPLICANTS.CHOREGROUPS.COMBAT.NAME, Db.Get().Attributes.Digging, 5);
			LifeSupport = Add("LifeSupport", DUPLICANTS.CHOREGROUPS.LIFESUPPORT.NAME, Db.Get().Attributes.LifeSupport, 5);
			Toggle = Add("Toggle", DUPLICANTS.CHOREGROUPS.TOGGLE.NAME, Db.Get().Attributes.Toggle, 5);
			MedicalAid = Add("MedicalAid", DUPLICANTS.CHOREGROUPS.MEDICALAID.NAME, Db.Get().Attributes.Caring, 4);
			Basekeeping = Add("Basekeeping", DUPLICANTS.CHOREGROUPS.BASEKEEPING.NAME, Db.Get().Attributes.Strength, 4);
			Cook = Add("Cook", DUPLICANTS.CHOREGROUPS.COOK.NAME, Db.Get().Attributes.Cooking, 3);
			Art = Add("Art", DUPLICANTS.CHOREGROUPS.ART.NAME, Db.Get().Attributes.Art, 3);
			Research = Add("Research", DUPLICANTS.CHOREGROUPS.RESEARCH.NAME, Db.Get().Attributes.Learning, 3);
			MachineOperating = Add("MachineOperating", DUPLICANTS.CHOREGROUPS.MACHINEOPERATING.NAME, Db.Get().Attributes.Machinery, 3);
			Farming = Add("Farming", DUPLICANTS.CHOREGROUPS.FARMING.NAME, Db.Get().Attributes.Botanist, 3);
			Ranching = Add("Ranching", DUPLICANTS.CHOREGROUPS.RANCHING.NAME, Db.Get().Attributes.Ranching, 3);
			Build = Add("Build", DUPLICANTS.CHOREGROUPS.BUILD.NAME, Db.Get().Attributes.Construction, 2);
			Dig = Add("Dig", DUPLICANTS.CHOREGROUPS.DIG.NAME, Db.Get().Attributes.Digging, 2);
			Hauling = Add("Hauling", DUPLICANTS.CHOREGROUPS.HAULING.NAME, Db.Get().Attributes.Strength, 1);
			Storage = Add("Storage", DUPLICANTS.CHOREGROUPS.STORAGE.NAME, Db.Get().Attributes.Strength, 1);
			Debug.Assert(true);
		}

		private ChoreGroup Add(string id, string name, Attribute attribute, int default_personal_priority)
		{
			ChoreGroup choreGroup = new ChoreGroup(id, name, attribute, default_personal_priority);
			Add(choreGroup);
			return choreGroup;
		}

		public ChoreGroup FindByHash(HashedString id)
		{
			ChoreGroup result = null;
			foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
			{
				if (resource.IdHash == id)
				{
					result = resource;
					break;
				}
			}
			return result;
		}
	}
}
