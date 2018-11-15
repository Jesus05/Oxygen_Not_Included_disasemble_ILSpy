using KSerialization.Converters;

namespace ProcGen
{
	public class FeatureConverter
	{
		public enum Shape
		{
			Circle,
			Oval,
			Blob,
			Square,
			Rectangle,
			Line
		}

		[StringEnumConverter]
		public Shape shape
		{
			get;
			private set;
		}

		public MinMax size
		{
			get;
			private set;
		}

		public MinMax density
		{
			get;
			private set;
		}

		public ChangeFloats mass
		{
			get;
			private set;
		}

		public ChangeFloats temperature
		{
			get;
			private set;
		}
	}
}
