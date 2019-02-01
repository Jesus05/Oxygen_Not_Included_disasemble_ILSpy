using System;

namespace NodeEditorFramework
{
	public class NodeAttribute : Attribute
	{
		public Type[] typeOfNodeCanvas = null;

		public bool hide
		{
			get;
			private set;
		}

		public string contextText
		{
			get;
			private set;
		}

		public NodeAttribute(bool HideNode, string ReplacedContextText, Type[] nodeCanvasTypes = null)
		{
			hide = HideNode;
			contextText = ReplacedContextText;
			typeOfNodeCanvas = (nodeCanvasTypes ?? new Type[1]
			{
				typeof(NodeCanvas)
			});
		}
	}
}
