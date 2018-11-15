using KSerialization.Converters;

namespace ProcGen
{
	public class Mob : SampleDescriber
	{
		public enum Location
		{
			Floor,
			Ceiling,
			Air,
			BackWall,
			NearWater,
			NearLiquid,
			Solid,
			Water,
			ShallowLiquid,
			Surface
		}

		public MinMax units
		{
			get;
			private set;
		}

		public string prefabName
		{
			get;
			private set;
		}

		public int width
		{
			get;
			private set;
		}

		public int height
		{
			get;
			private set;
		}

		[StringEnumConverter]
		public Location location
		{
			get;
			private set;
		}

		public Mob()
		{
		}

		public Mob(Location location)
		{
			this.location = location;
		}
	}
}
