using System.IO;

namespace Database
{
	public abstract class ColonyAchievementRequirement
	{
		public virtual void Update()
		{
		}

		public abstract bool Success();

		public virtual bool Fail()
		{
			return false;
		}

		public abstract void Serialize(BinaryWriter writer);

		public abstract void Deserialize(IReader reader);

		public virtual string GetProgress(bool complete)
		{
			return string.Empty;
		}
	}
}
