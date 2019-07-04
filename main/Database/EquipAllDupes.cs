using KSerialization;
using STRINGS;
using System.IO;
using UnityEngine;

namespace Database
{
	public class EquipAllDupes : ColonyAchievementRequirement
	{
		private AssignableSlot equipmentSlot;

		public EquipAllDupes(AssignableSlot equipmentSlot)
		{
			this.equipmentSlot = equipmentSlot;
		}

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_ALL_DUPES, equipmentSlot.Name);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_ALL_DUPES_DESCRIPTION, equipmentSlot.Name);
		}

		public override bool Success()
		{
			foreach (MinionIdentity item in Components.MinionIdentities.Items)
			{
				Equipment equipment = item.GetEquipment();
				if ((Object)equipment != (Object)null && !equipment.IsSlotOccupied(equipmentSlot))
				{
					return false;
				}
			}
			return true;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(equipmentSlot.Id);
		}

		public override void Deserialize(IReader reader)
		{
			string id = reader.ReadKleiString();
			equipmentSlot = Db.Get().AssignableSlots.Get(id);
		}
	}
}
