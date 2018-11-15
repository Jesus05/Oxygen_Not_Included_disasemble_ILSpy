using UnityEngine;
using UnityEngine.EventSystems;

namespace TMPro
{
	public class TMP_ScrollbarEventHandler : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler, IEventSystemHandler
	{
		public bool isSelected;

		public void OnPointerClick(PointerEventData eventData)
		{
			Debug.Log("Scrollbar click...", null);
		}

		public void OnSelect(BaseEventData eventData)
		{
			Debug.Log("Scrollbar selected", null);
			isSelected = true;
		}

		public void OnDeselect(BaseEventData eventData)
		{
			Debug.Log("Scrollbar De-Selected", null);
			isSelected = false;
		}
	}
}
