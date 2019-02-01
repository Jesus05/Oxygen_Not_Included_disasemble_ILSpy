using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization
{
	public sealed class TagMappings
	{
		private readonly IDictionary<string, Type> mappings;

		public TagMappings()
		{
			mappings = new Dictionary<string, Type>();
		}

		public TagMappings(IDictionary<string, Type> mappings)
		{
			this.mappings = new Dictionary<string, Type>(mappings);
		}

		public void Add(string tag, Type mapping)
		{
			mappings.Add(tag, mapping);
		}

		internal Type GetMapping(string tag)
		{
			if (!mappings.TryGetValue(tag, out Type value))
			{
				return null;
			}
			return value;
		}
	}
}
