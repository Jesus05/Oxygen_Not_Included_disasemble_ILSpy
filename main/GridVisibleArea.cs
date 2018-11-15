using System;
using System.Collections.Generic;
using UnityEngine;

public class GridVisibleArea
{
	public struct Callback
	{
		public System.Action OnUpdate;

		public string Name;
	}

	private GridArea[] Areas = new GridArea[3];

	private List<Callback> Callbacks = new List<Callback>();

	public GridArea CurrentArea => Areas[0];

	public GridArea PreviousArea => Areas[1];

	public GridArea PreviousPreviousArea => Areas[2];

	public void Update()
	{
		Areas[2] = Areas[1];
		Areas[1] = Areas[0];
		Areas[0] = GetVisibleArea();
		foreach (Callback callback in Callbacks)
		{
			Callback current = callback;
			current.OnUpdate();
		}
	}

	public void AddCallback(string name, System.Action on_update)
	{
		Callback callback = default(Callback);
		callback.Name = name;
		callback.OnUpdate = on_update;
		Callback item = callback;
		Callbacks.Add(item);
	}

	public void Run(Action<int> in_view)
	{
		if (in_view != null)
		{
			CurrentArea.Run(in_view);
		}
	}

	public void Run(Action<int> outside_view, Action<int> inside_view, Action<int> inside_view_second_time)
	{
		if (outside_view != null)
		{
			PreviousArea.RunOnDifference(CurrentArea, outside_view);
		}
		if (inside_view != null)
		{
			CurrentArea.RunOnDifference(PreviousArea, inside_view);
		}
		if (inside_view_second_time != null)
		{
			PreviousArea.RunOnDifference(PreviousPreviousArea, inside_view_second_time);
		}
	}

	public void RunIfVisible(int cell, Action<int> action)
	{
		CurrentArea.RunIfInside(cell, action);
	}

	public static GridArea GetVisibleArea()
	{
		GridArea result = default(GridArea);
		if ((UnityEngine.Object)Camera.main != (UnityEngine.Object)null)
		{
			Camera main = Camera.main;
			Vector3 position = Camera.main.transform.GetPosition();
			Vector3 vector = main.ViewportToWorldPoint(new Vector3(1f, 1f, position.z));
			Camera main2 = Camera.main;
			Vector3 position2 = Camera.main.transform.GetPosition();
			Vector3 vector2 = main2.ViewportToWorldPoint(new Vector3(0f, 0f, position2.z));
			result.SetExtents(Math.Max((int)(vector2.x - 0.5f), 0), Math.Max((int)(vector2.y - 0.5f), 0), Math.Min((int)(vector.x + 1.5f), Grid.WidthInCells), Math.Min((int)(vector.y + 1.5f), Grid.HeightInCells));
		}
		return result;
	}
}
