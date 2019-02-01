using System;
using System.Collections.Generic;
using UnityEngine;

public class LineLayer : GraphLayer
{
	[Serializable]
	public struct LineFormat
	{
		public Color color;

		public int thickness;
	}

	[Header("Lines")]
	public LineFormat[] line_formatting;

	public GameObject prefab_line;

	public GameObject line_container;

	private List<GraphedLine> lines = new List<GraphedLine>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void NewLine(Vector2[] points, string ID = "")
	{
		GameObject gameObject = Util.KInstantiateUI(prefab_line, line_container, true);
		if (ID == "")
		{
			ID = lines.Count.ToString();
		}
		gameObject.name = $"line_{ID}";
		GraphedLine component = gameObject.GetComponent<GraphedLine>();
		component.SetPoints(points);
		component.line_renderer.color = line_formatting[lines.Count % line_formatting.Length].color;
		component.line_renderer.LineThickness = (float)line_formatting[lines.Count % line_formatting.Length].thickness;
		lines.Add(component);
	}

	public void ClearLines()
	{
		foreach (GraphedLine line in lines)
		{
			if ((UnityEngine.Object)line != (UnityEngine.Object)null && (UnityEngine.Object)line.gameObject != (UnityEngine.Object)null)
			{
				UnityEngine.Object.DestroyImmediate(line.gameObject);
			}
		}
		lines.Clear();
	}
}
