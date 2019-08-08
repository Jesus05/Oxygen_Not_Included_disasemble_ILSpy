using KSerialization;
using System.IO;

namespace Database
{
	public class CreaturePoopKGProduction : ColonyAchievementRequirement
	{
		private Tag poopElement;

		private float amountToPoop;

		public CreaturePoopKGProduction(Tag poopElement, float amountToPoop)
		{
			this.poopElement = poopElement;
			this.amountToPoop = amountToPoop;
		}

		public override bool Success()
		{
			if (Game.Instance.savedInfo.creaturePoopAmount.ContainsKey(poopElement))
			{
				return Game.Instance.savedInfo.creaturePoopAmount[poopElement] >= amountToPoop;
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(amountToPoop);
			writer.WriteKleiString(poopElement.ToString());
		}

		public override void Deserialize(IReader reader)
		{
			amountToPoop = reader.ReadSingle();
			string name = reader.ReadKleiString();
			poopElement = new Tag(name);
		}
	}
}
