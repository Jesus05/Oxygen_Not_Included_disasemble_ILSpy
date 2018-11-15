using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextLinkHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	private static TextLinkHandler hoveredText;

	[MyCmpGet]
	private LocText text;

	private bool hoverLink;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && this.text.AllowLinks)
		{
			int num = TMP_TextUtilities.FindIntersectingLink(this.text, KInputManager.GetMousePos(), null);
			if (num != -1)
			{
				string text = CodexCache.FormatLinkID(this.text.textInfo.linkInfo[num].GetLinkID());
				if (!CodexCache.entries.ContainsKey(text))
				{
					SubEntry subEntry = CodexCache.FindSubEntry(text);
					if (subEntry == null || subEntry.disabled)
					{
						text = "PAGENOTFOUND";
					}
				}
				else if (CodexCache.entries[text].disabled)
				{
					text = "PAGENOTFOUND";
				}
				if (!ManagementMenu.Instance.codexScreen.gameObject.activeInHierarchy)
				{
					ManagementMenu.Instance.ToggleCodex();
				}
				ManagementMenu.Instance.codexScreen.ChangeArticle(text, true);
			}
		}
	}

	private void Update()
	{
		CheckMouseOver();
		if ((Object)hoveredText == (Object)this && text.AllowLinks)
		{
			PlayerController.Instance.ActiveTool.SetLinkCursor(hoverLink);
		}
	}

	private void OnEnable()
	{
		CheckMouseOver();
	}

	private void OnDisable()
	{
		ClearState();
	}

	private void Awake()
	{
		text = GetComponent<LocText>();
		if (text.AllowLinks && !text.raycastTarget)
		{
			text.raycastTarget = true;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		SetMouseOver();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ClearState();
	}

	private void ClearState()
	{
		if (!((Object)this == (Object)null) && !Equals(null) && (Object)hoveredText == (Object)this)
		{
			if (hoverLink && (Object)PlayerController.Instance != (Object)null && (Object)PlayerController.Instance.ActiveTool != (Object)null)
			{
				PlayerController.Instance.ActiveTool.SetLinkCursor(false);
			}
			hoveredText = null;
			hoverLink = false;
		}
	}

	public void CheckMouseOver()
	{
		if (!((Object)text == (Object)null))
		{
			if (TMP_TextUtilities.FindIntersectingLink(text, KInputManager.GetMousePos(), null) != -1)
			{
				SetMouseOver();
				hoverLink = true;
			}
			else if ((Object)hoveredText == (Object)this)
			{
				hoverLink = false;
			}
		}
	}

	private void SetMouseOver()
	{
		if ((Object)hoveredText != (Object)null && (Object)hoveredText != (Object)this)
		{
			hoveredText.hoverLink = false;
		}
		hoveredText = this;
	}
}
