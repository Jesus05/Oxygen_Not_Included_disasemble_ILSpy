using System;
using UnityEngine.EventSystems;

public class HoverCallback : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public Action<bool> OnHover;

	public void OnPointerEnter(PointerEventData data)
	{
		if (OnHover != null)
		{
			OnHover(true);
		}
	}

	public void OnPointerExit(PointerEventData data)
	{
		if (OnHover != null)
		{
			OnHover(false);
		}
	}
}
