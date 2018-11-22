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

		public ChoreGroup Operating;

		public ChoreGroup MedicalAid;

		public ChoreGroup Combat;

		public ChoreGroup LifeSupport;

		public ChoreGroup Toggle;

		public ChoreGroups(ResourceSet parent)
			: base("ChoreGroups", parent)
		{
			Combat = Add("Combat", DUPLICANTS.CHOREGROUPS.COMBAT.NAME, "Digging", 5);
			LifeSupport = Add("LifeSupport", DUPLICANTS.CHOREGROUPS.LIFESUPPORT.NAME, "LifeSupport", 5);
			Toggle = Add("Toggle", DUPLICANTS.CHOREGROUPS.TOGGLE.NAME, "Toggle", 5);
			MedicalAid = Add("MedicalAid", DUPLICANTS.CHOREGROUPS.MEDICALAID.NAME, "Caring", 4);
			Basekeeping = Add("Basekeeping", DUPLICANTS.CHOREGROUPS.BASEKEEPING.NAME, "Athletics", 4);
			Cook = Add("Cook", DUPLICANTS.CHOREGROUPS.COOK.NAME, "Cooking", 3);
			Art = Add("Art", DUPLICANTS.CHOREGROUPS.ART.NAME, "Art", 3);
			Research = Add("Research", DUPLICANTS.CHOREGROUPS.RESEARCH.NAME, "Learning", 3);
			Operating = Add("MachineOperating", DUPLICANTS.CHOREGROUPS.MACHINEOPERATING.NAME, "Machinery", 3);
			Farming = Add("Farming", DUPLICANTS.CHOREGROUPS.FARMING.NAME, "Botanist", 3);
			Ranching = Add("Ranching", DUPLICANTS.CHOREGROUPS.RANCHING.NAME, "Ranching", 3);
			Build = Add("Build", DUPLICANTS.CHOREGROUPS.BUILD.NAME, "Construction", 2);
			Dig = Add("Dig", DUPLICANTS.CHOREGROUPS.DIG.NAME, "Digging", 2);
			Hauling = Add("Hauling", DUPLICANTS.CHOREGROUPS.HAULING.NAME, "Athletics", 1);
			Storage = Add("Storage", DUPLICANTS.CHOREGROUPS.STORAGE.NAME, "Athletics", 1);
		}

		private ChoreGroup Add(string id, string name, string attribute, int default_personal_priority)
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
