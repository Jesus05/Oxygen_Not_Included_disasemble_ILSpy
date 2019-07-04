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

	public void NewLine(Tuple<float, float>[] points, string ID = "")
	{
		Vector2[] array = new Vector2[points.Length];
		for (int i = 0; i < points.Length; i++)
		{
			array[i] = new Vector2(points[i].first, points[i].second);
		}
		NewLine(array, ID, 256, true);
	}

	public void NewLine(Vector2[] points, string ID = "", int compressDataToPointCount = 256, bool useMaxValueOnCompress = true)
	{
		GameObject gameObject = Util.KInstantiateUI(prefab_line, line_container, true);
		if (ID == "")
		{
			ID = lines.Count.ToString();
		}
		gameObject.name = $"line_{ID}";
		GraphedLine component = gameObject.GetComponent<GraphedLine>();
		if (points.Length > compressDataToPointCount)
		{
			int num = points.Length / compressDataToPointCount;
			Vector2[] array = new Vector2[compressDataToPointCount];
			for (int i = 0; i < compressDataToPointCount; i++)
			{
				if (i > 0)
				{
					float num2 = 0f;
					if (useMaxValueOnCompress)
					{
						for (int j = 0; j < num; j++)
						{
							num2 = Mathf.Max(num2, points[i * num - j].y);
						}
					}
					else
					{
						for (int k = 0; k < num; k++)
						{
							num2 += points[i * num - k].y;
						}
						num2 /= (float)num;
					}
					array[i] = new Vector2(points[i * num].x, num2);
				}
			}
			points = array;
		}
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

	private void Update()
	{
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if (!RectTransformUtility.RectangleContainsScreenPoint(component, Input.mousePosition))
		{
			for (int i = 0; i < lines.Count; i++)
			{
				lines[i].HidePointHighlight();
			}
		}
		else
		{
			Vector2 localPoint = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.gameObject.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
			localPoint += component.sizeDelta / 2f;
			for (int j = 0; j < lines.Count; j++)
			{
				if (lines[j].PointCount != 0)
				{
					Vector2 closestDataToPointOnXAxis = lines[j].GetClosestDataToPointOnXAxis(localPoint);
					lines[j].SetPointHighlight(closestDataToPointOnXAxis);
				}
			}
		}
	}
}
