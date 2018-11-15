using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonLock : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IEventSystemHandler
{
	public GameObject target;

	public void OnPointerClick(PointerEventData eventData)
	{
		target.SendMessage("ToggleLock", SendMessageOptions.DontRequireReceiver);
	}

	public void OnDrag(PointerEventData eventData)
	{
		target.SendMessage("OnDrag", SendMessageOptions.DontRequireReceiver);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		target.SendMessage("Lock", true, SendMessageOptions.DontRequireReceiver);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		target.SendMessage("Lock", false, SendMessageOptions.DontRequireReceiver);
	}
}
