using System;
using UnityEngine;

public class VisibleAreaUpdater
{
	private GridVisibleArea VisibleArea;

	private Action<int> OutsideViewFirstTimeCallback;

	private Action<int> InsideViewFirstTimeCallback;

	private Action<int> InsideViewSecondTimeCallback;

	private Action<int> InsideViewRepeatCallback;

	private Action<int> UpdateCallback;

	private string Name;

	public VisibleAreaUpdater(Action<int> outside_view_first_time_cb, Action<int> inside_view_first_time_cb, Action<int> inside_view_second_time_cb, Action<int> inside_view_repeat_cb, string name)
	{
		OutsideViewFirstTimeCallback = outside_view_first_time_cb;
		InsideViewFirstTimeCallback = inside_view_first_time_cb;
		InsideViewSecondTimeCallback = inside_view_second_time_cb;
		UpdateCallback = InternalUpdateCell;
		Name = name;
	}

	public void Update()
	{
		if ((UnityEngine.Object)CameraController.Instance != (UnityEngine.Object)null && VisibleArea == null)
		{
			VisibleArea = CameraController.Instance.VisibleArea;
			VisibleArea.AddCallback(Name, OnVisibleAreaUpdate);
			VisibleArea.Run(InsideViewFirstTimeCallback);
			VisibleArea.Run(InsideViewRepeatCallback);
		}
	}

	private void OnVisibleAreaUpdate()
	{
		if (VisibleArea != null)
		{
			VisibleArea.Run(OutsideViewFirstTimeCallback, InsideViewFirstTimeCallback, InsideViewSecondTimeCallback);
		}
	}

	private void InternalUpdateCell(int cell)
	{
		OutsideViewFirstTimeCallback(cell);
		InsideViewFirstTimeCallback(cell);
	}

	public void UpdateCell(int cell)
	{
		if (VisibleArea != null)
		{
			VisibleArea.RunIfVisible(cell, UpdateCallback);
		}
	}
}
