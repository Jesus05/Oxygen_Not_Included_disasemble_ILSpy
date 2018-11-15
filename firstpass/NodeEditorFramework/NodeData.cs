using System;

namespace NodeEditorFramework
{
	public struct NodeData
	{
		public string adress;

		public Type[] typeOfNodeCanvas;

		public NodeData(string name, Type[] types)
		{
			adress = name;
			typeOfNodeCanvas = types;
		}
	}
}
