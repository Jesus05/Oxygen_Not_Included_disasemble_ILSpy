using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

[Serializable]
public class GraphedLine : KMonoBehaviour
{
	public UILineRenderer line_renderer;

	public LineLayer layer;

	private Vector2[] points = new Vector2[0];

	[SerializeField]
	private GameObject highlightPoint;

	public int PointCount => points.Length;

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

	public Vector2 GetClosestDataToPointOnXAxis(Vector2 toPoint)
	{
		float x = toPoint.x;
		Vector2 sizeDelta = layer.graph.rectTransform().sizeDelta;
		float num = x / sizeDelta.x;
		float num2 = layer.graph.axis_x.min_value + layer.graph.axis_x.range * num;
		Vector2 result = Vector2.zero;
		Vector2[] array = points;
		for (int i = 0; i < array.Length; i++)
		{
			Vector2 vector = array[i];
			if (Mathf.Abs(vector.x - num2) < Mathf.Abs(result.x - num2))
			{
				result = vector;
			}
		}
		return result;
	}

	public void HidePointHighlight()
	{
		highlightPoint.SetActive(false);
	}

	public void SetPointHighlight(Vector2 point)
	{
		highlightPoint.SetActive(true);
		Vector2 relativePosition = layer.graph.GetRelativePosition(point);
		RectTransform transform = highlightPoint.rectTransform();
		float x = relativePosition.x;
		Vector2 sizeDelta = layer.graph.rectTransform().sizeDelta;
		float num = x * sizeDelta.x;
		Vector2 sizeDelta2 = layer.graph.rectTransform().sizeDelta;
		float x2 = num - sizeDelta2.x / 2f;
		float y = relativePosition.y;
		Vector2 sizeDelta3 = layer.graph.rectTransform().sizeDelta;
		float num2 = y * sizeDelta3.y;
		Vector2 sizeDelta4 = layer.graph.rectTransform().sizeDelta;
		transform.SetLocalPosition(new Vector2(x2, num2 - sizeDelta4.y / 2f));
		ToolTip component = layer.graph.GetComponent<ToolTip>();
		component.ClearMultiStringTooltip();
		ToolTip toolTip = component;
		Vector3 localPosition = highlightPoint.rectTransform().localPosition;
		toolTip.tooltipPositionOffset = new Vector2(localPosition.x, layer.graph.rectTransform().rect.height / 2f - 12f);
		component.SetSimpleTooltip(layer.graph.axis_x.name + " " + point.x + ", " + point.y + " " + layer.graph.axis_y.name);
		ToolTipScreen.Instance.SetToolTip(component);
	}
}
