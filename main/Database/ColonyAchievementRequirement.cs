using System.IO;

namespace Database
{
	public abstract class ColonyAchievementRequirement
	{
		public abstract string Name();

		public abstract string Description();

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
	}
}
