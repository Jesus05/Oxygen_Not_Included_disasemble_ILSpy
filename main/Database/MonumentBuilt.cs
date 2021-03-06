using STRINGS;
using System;
using System.Collections;
using System.IO;

namespace Database
{
	public class MonumentBuilt : VictoryColonyAchievementRequirement
	{
		public override string Name()
		{
			return COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.BUILT_MONUMENT;
		}

		public override string Description()
		{
			return COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.BUILT_MONUMENT_DESCRIPTION;
		}

		public override bool Success()
		{
			IEnumerator enumerator = Components.MonumentParts.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MonumentPart monumentPart = (MonumentPart)enumerator.Current;
					if (monumentPart.IsMonumentCompleted())
					{
						Game.Instance.unlocks.Unlock("thriving");
						return true;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return false;
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override string GetProgress(bool complete)
		{
			return Name();
		}
	}
}
