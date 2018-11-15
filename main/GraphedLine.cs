using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

[Serializable]
public class GraphedLine : KMonoBehaviour
{
	public UILineRenderer line_renderer;

	public LineLayer layer;

	private Vector2[] points;

	public void SetPoints(Vector2[] points)
	{
		this.points = points;
		UpdatePoints();
	}

	private void UpdatePoints()
	{
		Vector2[] array = new Vector2[points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = layer.graph.GetRelativePosition(points[i]);
		}
		line_renderer.Points = array;
	}
}
