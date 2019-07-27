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

	public enum DataScalingType
	{
		Average,
		Max,
		DropValues
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
		NewLine(array, ID, 128, DataScalingType.DropValues);
	}

	public void NewLine(Vector2[] points, string ID = "", int compressDataToPointCount = 128, DataScalingType compressType = DataScalingType.DropValues)
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
			Vector2[] array = new Vector2[compressDataToPointCount];
			if (compressType == DataScalingType.DropValues)
			{
				float num = (float)(points.Length - compressDataToPointCount);
				float num2 = (float)points.Length / num;
				int num3 = 0;
				float num4 = 0f;
				for (int i = 0; i < points.Length; i++)
				{
					num4 += 1f;
					if (num4 >= num2)
					{
						num4 -= num2;
					}
					else
					{
						array[num3] = points[i];
						num3++;
					}
				}
			}
			else
			{
				int num5 = points.Length / compressDataToPointCount;
				for (int j = 0; j < compressDataToPointCount; j++)
				{
					if (j > 0)
					{
						float num6 = 0f;
						switch (compressType)
						{
						case DataScalingType.Max:
							for (int l = 0; l < num5; l++)
							{
								num6 = Mathf.Max(num6, points[j * num5 - l].y);
							}
							break;
						case DataScalingType.Average:
							for (int k = 0; k < num5; k++)
							{
								num6 += points[j * num5 - k].y;
							}
							num6 /= (float)num5;
							break;
						}
						array[j] = new Vector2(points[j * num5].x, num6);
					}
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
