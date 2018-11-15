using KSerialization.Converters;

namespace ProcGen
{
	public class SampleDescriber
	{
		public enum PointSelectionMethod
		{
			RandomPoints,
			Centroid
		}

		public class Override
		{
			public float? massOverride
			{
				get;
				protected set;
			}

			public float? massMultiplier
			{
				get;
				protected set;
			}

			public float? temperatureOverride
			{
				get;
				protected set;
			}

			public float? temperatureMultiplier
			{
				get;
				protected set;
			}

			public string diseaseOverride
			{
				get;
				protected set;
			}

			public int? diseaseAmountOverride
			{
				get;
				protected set;
			}

			public Override()
			{
			}

			public Override(float? massOverride, float? massMultiplier, float? temperatureOverride, float? temperatureMultiplier, string diseaseOverride, int? diseaseAmountOverride)
			{
				this.massOverride = massOverride;
				this.massMultiplier = massMultiplier;
				this.temperatureOverride = temperatureOverride;
				this.temperatureMultiplier = temperatureMultiplier;
				this.diseaseOverride = diseaseOverride;
				this.diseaseAmountOverride = diseaseAmountOverride;
			}
		}

		public string name
		{
			get;
			set;
		}

		[StringEnumConverter]
		public PointSelectionMethod selectMethod
		{
			get;
			protected set;
		}

		public MinMax density
		{
			get;
			protected set;
		}

		public float avoidRadius
		{
			get;
			protected set;
		}

		[StringEnumConverter]
		public PointGenerator.SampleBehaviour sampleBehaviour
		{
			get;
			protected set;
		}

		public bool doAvoidPoints
		{
			get;
			protected set;
		}

		public bool dontRelaxChildren
		{
			get;
			protected set;
		}

		public MinMax blobSize
		{
			get;
			protected set;
		}

		public SampleDescriber()
		{
			doAvoidPoints = true;
			dontRelaxChildren = false;
		}
	}
}
