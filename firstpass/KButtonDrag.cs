using System;
using UnityEngine.EventSystems;

public class KButtonDrag : KButton, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	public event System.Action onBeginDrag;

	public event System.Action onDrag;

	public event System.Action onEndDrag;

	public void ClearOnDragEvents()
	{
		this.onBeginDrag = null;
		this.onDrag = null;
		this.onEndDrag = null;
	}

	public void OnBeginDrag(PointerEventData data)
	{
		this.onBeginDrag.Signal();
	}

	public void OnDrag(PointerEventData data)
	{
		this.onDrag.Signal();
	}

	public void OnEndDrag(PointerEventData data)
	{
		this.onEndDrag.Signal();
	}
}
