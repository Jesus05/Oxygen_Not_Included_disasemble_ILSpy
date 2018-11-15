using UnityEngine.EventSystems;

public class HighlightOnHover : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public KImage image;

	public void OnPointerEnter(PointerEventData data)
	{
		image.ColorState = KImage.ColorSelector.Hover;
	}

	public void OnPointerExit(PointerEventData data)
	{
		image.ColorState = KImage.ColorSelector.Inactive;
	}
}
