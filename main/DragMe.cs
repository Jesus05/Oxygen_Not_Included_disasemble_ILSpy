using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragMe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	public interface IDragListener
	{
		void OnBeginDrag(Vector2 position);

		void OnEndDrag(Vector2 position);
	}

	public bool dragOnSurfaces = true;

	private GameObject m_DraggingIcon;

	private RectTransform m_DraggingPlane;

	public IDragListener listener;

	public void OnBeginDrag(PointerEventData eventData)
	{
		Canvas canvas = FindInParents<Canvas>(base.gameObject);
		if (!((Object)canvas == (Object)null))
		{
			m_DraggingIcon = Object.Instantiate(base.gameObject);
			GraphicRaycaster component = m_DraggingIcon.GetComponent<GraphicRaycaster>();
			if ((Object)component != (Object)null)
			{
				component.enabled = false;
			}
			m_DraggingIcon.name = "dragObj";
			m_DraggingIcon.transform.SetParent(canvas.transform, false);
			m_DraggingIcon.transform.SetAsLastSibling();
			RectTransform component2 = m_DraggingIcon.GetComponent<RectTransform>();
			component2.pivot = Vector2.zero;
			if (dragOnSurfaces)
			{
				m_DraggingPlane = (base.transform as RectTransform);
			}
			else
			{
				m_DraggingPlane = (canvas.transform as RectTransform);
			}
			SetDraggedPosition(eventData);
			listener.OnBeginDrag(eventData.position);
		}
	}

	public void OnDrag(PointerEventData data)
	{
		if ((Object)m_DraggingIcon != (Object)null)
		{
			SetDraggedPosition(data);
		}
	}

	private void SetDraggedPosition(PointerEventData data)
	{
		if (dragOnSurfaces && (Object)data.pointerEnter != (Object)null && (Object)(data.pointerEnter.transform as RectTransform) != (Object)null)
		{
			m_DraggingPlane = (data.pointerEnter.transform as RectTransform);
		}
		RectTransform component = m_DraggingIcon.GetComponent<RectTransform>();
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out Vector3 worldPoint))
		{
			component.position = worldPoint;
			component.rotation = m_DraggingPlane.rotation;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		listener.OnEndDrag(eventData.position);
		if ((Object)m_DraggingIcon != (Object)null)
		{
			Object.Destroy(m_DraggingIcon);
		}
	}

	public static T FindInParents<T>(GameObject go) where T : Component
	{
		if (!((Object)go == (Object)null))
		{
			T val = (T)null;
			Transform parent = go.transform.parent;
			while ((Object)parent != (Object)null && (Object)val == (Object)null)
			{
				val = parent.gameObject.GetComponent<T>();
				parent = parent.parent;
			}
			return val;
		}
		return (T)null;
	}
}
