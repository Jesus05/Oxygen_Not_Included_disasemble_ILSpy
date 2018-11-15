using NodeEditorFramework.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTreeNode : Resource
{
	public class Edge
	{
		public enum EdgeType
		{
			PolyLineEdge,
			QuadCurveEdge,
			ArcEdge,
			SplineEdge,
			BezierEdge,
			GenericEdge
		}

		public Vector2f sourceOffset = new Vector2f(0, 0);

		public Vector2f targetOffset = new Vector2f(0, 0);

		public EdgeType edgeType
		{
			get;
			private set;
		}

		public ResourceTreeNode source
		{
			get;
			private set;
		}

		public ResourceTreeNode target
		{
			get;
			private set;
		}

		public List<Vector2> SrcTarget
		{
			get
			{
				List<Vector2> list = new List<Vector2>();
				list.Add(SourcePos());
				list.Add(TargetPos());
				return list;
			}
		}

		public List<Vector2> path
		{
			get;
			private set;
		}

		public Edge(ResourceTreeNode source, ResourceTreeNode target, EdgeType edgeType)
		{
			this.edgeType = edgeType;
			this.source = source;
			this.target = target;
			path = null;
		}

		private Vector2 SourcePos()
		{
			return source.center + (Vector2)sourceOffset;
		}

		private Vector2 TargetPos()
		{
			return target.center + (Vector2)targetOffset;
		}

		public void AddToPath(Vector2f point)
		{
			if (path == null)
			{
				path = new List<Vector2>();
			}
			path.Add(point);
		}

		public void Render(Rect rect, float width, Color colour)
		{
			EdgeType edgeType = this.edgeType;
			if (edgeType == EdgeType.GenericEdge)
			{
				goto IL_0013;
			}
			goto IL_0013;
			IL_0013:
			RTEditorGUI.DrawLine(rect, SourcePos(), TargetPos(), colour, null, width);
		}
	}

	public float nodeX;

	public float nodeY;

	public float width;

	public float height;

	public List<ResourceTreeNode> references = new List<ResourceTreeNode>();

	public List<Edge> edges = new List<Edge>();

	public Vector2 position => new Vector2(nodeX, nodeY);

	public Vector2 center => position + new Vector2(width / 2f, (0f - height) / 2f);
}
