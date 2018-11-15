using KSerialization.Converters;

namespace ProcGen
{
	public struct ChangeFloats
	{
		public enum ChangeType
		{
			NoChange,
			OverrideRange,
			OverrideSet,
			TakeNoiseVal
		}

		[StringEnumConverter]
		public ChangeType change
		{
			get;
			private set;
		}

		public MinMax value
		{
			get;
			private set;
		}
	}
}
