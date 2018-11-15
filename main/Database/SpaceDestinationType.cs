using System.Collections.Generic;
using System.Diagnostics;

namespace Database
{
	[DebuggerDisplay("{Id}")]
	public class SpaceDestinationType : Resource
	{
		public string typeName;

		public string description;

		public int iconSize = 128;

		public string spriteName;

		public Dictionary<SimHashes, MathUtil.MinMax> elementTable;

		public Dictionary<string, int> recoverableEntities;

		public SpaceDestinationType(string id, ResourceSet parent, string name, string description, int iconSize, string spriteName, Dictionary<SimHashes, MathUtil.MinMax> elementTable, Dictionary<string, int> recoverableEntities = null)
			: base(id, parent, name)
		{
			typeName = name;
			this.description = description;
			this.iconSize = iconSize;
			this.spriteName = spriteName;
			this.elementTable = elementTable;
			this.recoverableEntities = recoverableEntities;
		}
	}
}
