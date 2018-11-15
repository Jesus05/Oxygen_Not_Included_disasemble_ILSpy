using System;

namespace NodeEditorFramework
{
	public class NodeCanvasTypeAttribute : Attribute
	{
		public string Name;

		public NodeCanvasTypeAttribute(string displayName)
		{
			Name = displayName;
		}
	}
}
