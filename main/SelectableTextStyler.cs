using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableTextStyler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
{
	public enum State
	{
		Normal
	}

	public enum HoverState
	{
		Normal,
		Hovered
	}

	[SerializeField]
	private LocText target;

	[SerializeField]
	private State state;

	[SerializeField]
	private TextStyleSetting normalNormal;

	[SerializeField]
	private TextStyleSetting normalHovered;

	private void Start()
	{
		SetState(state, HoverState.Normal);
	}

	private void SetState(State state, HoverState hover_state)
	{
		if (state == State.Normal)
		{
			switch (hover_state)
			{
			case HoverState.Normal:
				target.textStyleSetting = normalNormal;
				break;
			case HoverState.Hovered:
				target.textStyleSetting = normalHovered;
				break;
			}
		}
		target.ApplySettings();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		SetState(state, HoverState.Hovered);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		SetState(state, HoverState.Normal);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		SetState(state, HoverState.Normal);
	}
}
