using System.IO;

namespace Database
{
	public class RevealAsteriod : ColonyAchievementRequirement
	{
		private float percentToReveal;

		public RevealAsteriod(float percentToReveal)
		{
			this.percentToReveal = percentToReveal;
		}

		public override bool Success()
		{
			float num = 0f;
			for (int i = 0; i < Grid.Visible.Length; i++)
			{
				if (Grid.Visible[i] > 0)
				{
					num += 1f;
				}
			}
			return num / (float)Grid.Visible.Length > percentToReveal;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(percentToReveal);
		}

		public override void Deserialize(IReader reader)
		{
			percentToReveal = reader.ReadSingle();
		}
	}
}
