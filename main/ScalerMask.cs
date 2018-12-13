using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScalerMask : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public RectTransform SourceTransform;

	private RectTransform _thisTransform;

	private LayoutElement _thisLayoutElement;

	public GameObject hoverIndicator;

	public bool hoverLock;

	private bool grandparentIsHovered;

	private bool isHovered;

	private bool queuedSizeUpdate = true;

	public float topPadding;

	public float bottomPadding;

	private RectTransform ThisTransform
	{
		get
		{
			if ((UnityEngine.Object)_thisTransform == (UnityEngine.Object)null)
			{
				_thisTransform = GetComponent<RectTransform>();
			}
			return _thisTransform;
		}
	}

	private LayoutElement ThisLayoutElement
	{
		get
		{
			if ((UnityEngine.Object)_thisLayoutElement == (UnityEngine.Object)null)
			{
				_thisLayoutElement = GetComponent<LayoutElement>();
			}
			return _thisLayoutElement;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		DetailsScreen componentInParent = GetComponentInParent<DetailsScreen>();
		if ((bool)componentInParent)
		{
			DetailsScreen detailsScreen = componentInParent;
			detailsScreen.pointerEnterActions = (KScreen.PointerEnterActions)Delegate.Combine(detailsScreen.pointerEnterActions, new KScreen.PointerEnterActions(OnPointerEnterGrandparent));
			DetailsScreen detailsScreen2 = componentInParent;
			detailsScreen2.pointerExitActions = (KScreen.PointerExitActions)Delegate.Combine(detailsScreen2.pointerExitActions, new KScreen.PointerExitActions(OnPointerExitGrandparent));
		}
	}

	protected override void OnCleanUp()
	{
		DetailsScreen componentInParent = GetComponentInParent<DetailsScreen>();
		if ((bool)componentInParent)
		{
			DetailsScreen detailsScreen = componentInParent;
			detailsScreen.pointerEnterActions = (KScreen.PointerEnterActions)Delegate.Remove(detailsScreen.pointerEnterActions, new KScreen.PointerEnterActions(OnPointerEnterGrandparent));
			DetailsScreen detailsScreen2 = componentInParent;
			detailsScreen2.pointerExitActions = (KScreen.PointerExitActions)Delegate.Remove(detailsScreen2.pointerExitActions, new KScreen.PointerExitActions(OnPointerExitGrandparent));
		}
		base.OnCleanUp();
	}

	private void Update()
	{
		if ((UnityEngine.Object)SourceTransform != (UnityEngine.Object)null)
		{
			SourceTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ThisTransform.rect.width);
		}
		if ((UnityEngine.Object)SourceTransform != (UnityEngine.Object)null && (!hoverLock || !grandparentIsHovered || isHovered || queuedSizeUpdate))
		{
			ThisLayoutElement.minHeight = SourceTransform.rect.height + topPadding + bottomPadding;
			SourceTransform.anchoredPosition = new Vector2(0f, 0f - topPadding);
			queuedSizeUpdate = false;
		}
		if ((UnityEngine.Object)hoverIndicator != (UnityEngine.Object)null)
		{
			if ((UnityEngine.Object)SourceTransform != (UnityEngine.Object)null && SourceTransform.rect.height > ThisTransform.rect.height)
			{
				hoverIndicator.SetActive(true);
			}
			else
			{
				hoverIndicator.SetActive(false);
			}
		}
	}

	public void UpdateSize()
	{
		queuedSizeUpdate = true;
	}

	public void OnPointerEnterGrandparent(PointerEventData eventData)
	{
		grandparentIsHovered = true;
	}

	public void OnPointerExitGrandparent(PointerEventData eventData)
	{
		grandparentIsHovered = false;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isHovered = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovered = false;
	}
}
