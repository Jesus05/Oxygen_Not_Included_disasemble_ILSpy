namespace ProcGen.Noise
{
	public class Link
	{
		public enum Type
		{
			None,
			Primitive,
			Filter,
			Transformer,
			Selector,
			Modifier,
			Combiner,
			FloatPoints,
			ControlPoints,
			Terminator
		}

		public Type type
		{
			get;
			set;
		}

		public string name
		{
			get;
			set;
		}
	}
}
