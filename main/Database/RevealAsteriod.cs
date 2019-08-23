using STRINGS;
using System.IO;

namespace Database
{
	public class RevealAsteriod : ColonyAchievementRequirement
	{
		private float percentToReveal;

		private float amountRevealed;

		public RevealAsteriod(float percentToReveal)
		{
			this.percentToReveal = percentToReveal;
		}

		public override bool Success()
		{
			amountRevealed = 0f;
			float num = 0f;
			for (int i = 0; i < Grid.Visible.Length; i++)
			{
				if (Grid.Visible[i] > 0)
				{
					num += 1f;
				}
			}
			amountRevealed = num / (float)Grid.Visible.Length;
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

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.REVEALED, amountRevealed * 100f, percentToReveal * 100f);
		}
	}
}
