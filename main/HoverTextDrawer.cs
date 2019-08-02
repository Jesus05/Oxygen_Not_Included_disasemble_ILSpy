using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverTextDrawer
{
	[Serializable]
	public class Skin
	{
		public Vector2 baseOffset;

		public LocText textWidget;

		public Image iconWidget;

		public Vector2 shadowImageOffset;

		public Color shadowImageColor;

		public Image shadowBarWidget;

		public Image selectBorderWidget;

		public Vector2 shadowBarBorder;

		public Vector2 selectBorder;

		public bool drawWidgets;

		public bool enableProfiling;

		public bool enableDebugOffset;

		public bool drawInProgressHoverText;

		public Vector2 debugOffset;
	}

	private class Pool<WidgetType> where WidgetType : MonoBehaviour
	{
		public struct Entry
		{
			public WidgetType widget;

			public RectTransform rect;
		}

		private GameObject prefab;

		private RectTransform root;

		private List<Entry> entries = new List<Entry>();

		private int drawnWidgets;

		public Pool(GameObject prefab, RectTransform master_root)
		{
			this.prefab = prefab;
			GameObject gameObject = new GameObject(typeof(WidgetType).Name);
			root = gameObject.AddComponent<RectTransform>();
			root.SetParent(master_root);
			root.anchoredPosition = Vector2.zero;
			root.anchorMin = Vector2.zero;
			root.anchorMax = Vector2.one;
			root.sizeDelta = Vector2.zero;
			gameObject.AddComponent<CanvasGroup>();
		}

		public Entry Draw(Vector2 pos)
		{
			Entry entry = default(Entry);
			if (drawnWidgets < entries.Count)
			{
				entry = entries[drawnWidgets];
				if (!entry.widget.gameObject.activeSelf)
				{
					entry.widget.gameObject.SetActive(true);
				}
			}
			else
			{
				GameObject gameObject = Util.KInstantiateUI(prefab, root.gameObject, false);
				gameObject.SetActive(true);
				entry.widget = gameObject.GetComponent<WidgetType>();
				entry.rect = gameObject.GetComponent<RectTransform>();
				entries.Add(entry);
			}
			entry.rect.anchoredPosition = new Vector2(pos.x, pos.y);
			drawnWidgets++;
			return entry;
		}

		public void BeginDrawing()
		{
			drawnWidgets = 0;
		}

		public void EndDrawing()
		{
			for (int i = drawnWidgets; i < entries.Count; i++)
			{
				Entry entry = entries[i];
				if (entry.widget.gameObject.activeSelf)
				{
					Entry entry2 = entries[i];
					entry2.widget.gameObject.SetActive(false);
				}
			}
		}

		public void SetEnabled(bool enabled)
		{
			if (enabled)
			{
				root.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
			}
			else
			{
				root.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
			}
		}

		public void Cleanup()
		{
			foreach (Entry entry in entries)
			{
				Entry current = entry;
				UnityEngine.Object.Destroy(current.widget.gameObject);
			}
			entries.Clear();
		}
	}

	public Skin skin;

	private Vector2 currentPos;

	private Vector2 rootPos;

	private Vector2 shadowStartPos;

	private float maxShadowX;

	private bool firstShadowBar;

	private bool isShadowBarSelected;

	private int minLineHeight;

	private Pool<LocText> textWidgets;

	private Pool<Image> iconWidgets;

	private Pool<Image> shadowBars;

	private Pool<Image> selectBorders;

	public HoverTextDrawer(Skin skin, RectTransform parent)
	{
		shadowBars = new Pool<Image>(skin.shadowBarWidget.gameObject, parent);
		selectBorders = new Pool<Image>(skin.selectBorderWidget.gameObject, parent);
		textWidgets = new Pool<LocText>(skin.textWidget.gameObject, parent);
		iconWidgets = new Pool<Image>(skin.iconWidget.gameObject, parent);
		this.skin = skin;
	}

	public void SetEnabled(bool enabled)
	{
		shadowBars.SetEnabled(enabled);
		textWidgets.SetEnabled(enabled);
		iconWidgets.SetEnabled(enabled);
		selectBorders.SetEnabled(enabled);
	}

	public void BeginDrawing(Vector2 root_pos)
	{
		BeginSample("BeginDrawing");
		rootPos = root_pos + skin.baseOffset;
		if (skin.enableDebugOffset)
		{
			rootPos += skin.debugOffset;
		}
		currentPos = rootPos;
		textWidgets.BeginDrawing();
		iconWidgets.BeginDrawing();
		shadowBars.BeginDrawing();
		selectBorders.BeginDrawing();
		firstShadowBar = true;
		minLineHeight = 0;
		EndSample();
	}

	public void EndDrawing()
	{
		BeginSample("EndDrawing");
		shadowBars.EndDrawing();
		iconWidgets.EndDrawing();
		textWidgets.EndDrawing();
		selectBorders.EndDrawing();
		EndSample();
	}

	public void DrawText(string text, TextStyleSetting style, Color color, bool override_color = true)
	{
		if (skin.drawWidgets)
		{
			BeginSample("DrawText");
			Pool<LocText>.Entry entry = textWidgets.Draw(currentPos);
			LocText widget = entry.widget;
			Color color2 = Color.white;
			if ((UnityEngine.Object)widget.textStyleSetting != (UnityEngine.Object)style)
			{
				widget.textStyleSetting = style;
				widget.ApplySettings();
			}
			if ((UnityEngine.Object)style != (UnityEngine.Object)null)
			{
				color2 = style.textColor;
			}
			if (override_color)
			{
				color2 = color;
			}
			widget.color = color2;
			if (widget.text != text)
			{
				widget.text = text;
				widget.KForceUpdateDirty();
			}
			currentPos.x += widget.renderedWidth;
			maxShadowX = Mathf.Max(currentPos.x, maxShadowX);
			minLineHeight = (int)Mathf.Max((float)minLineHeight, widget.renderedHeight);
			EndSample();
		}
	}

	public void DrawText(string text, TextStyleSetting style)
	{
		DrawText(text, style, Color.white, false);
	}

	public void AddIndent(int width = 36)
	{
		if (skin.drawWidgets)
		{
			currentPos.x += (float)width;
		}
	}

	public void NewLine(int min_height = 26)
	{
		if (skin.drawWidgets)
		{
			currentPos.y -= (float)Math.Max(min_height, minLineHeight);
			currentPos.x = rootPos.x;
			minLineHeight = 0;
		}
	}

	public void DrawIcon(Sprite icon, int min_width = 18)
	{
		DrawIcon(icon, Color.white, min_width, 2);
	}

	public void DrawIcon(Sprite icon, Color color, int image_size = 18, int horizontal_spacing = 2)
	{
		if (skin.drawWidgets)
		{
			BeginSample("DrawIcon");
			AddIndent(horizontal_spacing);
			Pool<Image>.Entry entry = iconWidgets.Draw(currentPos + skin.shadowImageOffset);
			entry.widget.sprite = icon;
			entry.widget.color = skin.shadowImageColor;
			entry.rect.sizeDelta = new Vector2((float)image_size, (float)image_size);
			Pool<Image>.Entry entry2 = iconWidgets.Draw(currentPos);
			entry2.widget.sprite = icon;
			entry2.widget.color = color;
			entry2.rect.sizeDelta = new Vector2((float)image_size, (float)image_size);
			AddIndent(horizontal_spacing);
			currentPos.x += (float)image_size;
			maxShadowX = Mathf.Max(currentPos.x, maxShadowX);
			EndSample();
		}
	}

	public void BeginShadowBar(bool selected = false)
	{
		if (skin.drawWidgets)
		{
			if (firstShadowBar)
			{
				firstShadowBar = false;
			}
			else
			{
				NewLine(22);
			}
			isShadowBarSelected = selected;
			shadowStartPos = currentPos;
			maxShadowX = rootPos.x;
		}
	}

	public void EndShadowBar()
	{
		if (skin.drawWidgets)
		{
			NewLine(22);
			Pool<Image>.Entry entry = shadowBars.Draw(currentPos);
			entry.rect.anchoredPosition = shadowStartPos + new Vector2(0f - skin.shadowBarBorder.x, skin.shadowBarBorder.y);
			entry.rect.sizeDelta = new Vector2(maxShadowX - rootPos.x + skin.shadowBarBorder.x * 2f, shadowStartPos.y - currentPos.y + skin.shadowBarBorder.y * 2f);
			if (isShadowBarSelected)
			{
				Pool<Image>.Entry entry2 = selectBorders.Draw(currentPos);
				entry2.rect.anchoredPosition = shadowStartPos + new Vector2(0f - skin.shadowBarBorder.x - skin.selectBorder.x, skin.shadowBarBorder.y + skin.selectBorder.y);
				entry2.rect.sizeDelta = new Vector2(maxShadowX - rootPos.x + skin.shadowBarBorder.x * 2f + skin.selectBorder.x * 2f, shadowStartPos.y - currentPos.y + skin.shadowBarBorder.y * 2f + skin.selectBorder.y * 2f);
			}
		}
	}

	public void Cleanup()
	{
		shadowBars.Cleanup();
		textWidgets.Cleanup();
		iconWidgets.Cleanup();
	}

	private void BeginSample(string section_name)
	{
		if (!skin.enableProfiling)
		{
			return;
		}
	}

	private void EndSample()
	{
		if (!skin.enableProfiling)
		{
			return;
		}
	}
}
