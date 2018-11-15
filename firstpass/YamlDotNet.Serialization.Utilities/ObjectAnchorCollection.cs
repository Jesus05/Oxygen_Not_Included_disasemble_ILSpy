using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.Utilities
{
	internal sealed class ObjectAnchorCollection
	{
		private readonly IDictionary<string, object> objectsByAnchor = new Dictionary<string, object>();

		private readonly IDictionary<object, string> anchorsByObject = new Dictionary<object, string>();

		public object this[string anchor]
		{
			get
			{
				if (objectsByAnchor.TryGetValue(anchor, out object value))
				{
					return value;
				}
				throw new AnchorNotFoundException(string.Format(CultureInfo.InvariantCulture, "The anchor '{0}' does not exists", anchor));
			}
		}

		public void Add(string anchor, object @object)
		{
			objectsByAnchor.Add(anchor, @object);
			if (@object != null)
			{
				anchorsByObject.Add(@object, anchor);
			}
		}

		public bool TryGetAnchor(object @object, out string anchor)
		{
			return anchorsByObject.TryGetValue(@object, out anchor);
		}
	}
}
