using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScheduleScreenColumnEntry : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IEventSystemHandler
{
	public Image image;

	public System.Action onLeftClick;

	public void OnPointerEnter(PointerEventData event_data)
	{
		RunCallbacks();
	}

	private void RunCallbacks()
	{
		if (Input.GetMouseButton(0) && onLeftClick != null)
		{
			onLeftClick();
		}
	}

	public void OnPointerDown(PointerEventData event_data)
	{
		RunCallbacks();
	}
}
