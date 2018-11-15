using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HoverTextConfiguration : KMonoBehaviour
{
	[Serializable]
	public struct TextStylePair
	{
		public TextStyleSetting Standard;

		public TextStyleSetting Selected;
	}

	[Serializable]
	public struct ValuePropertyTextStyles
	{
		public TextStylePair Property;

		public TextStylePair Property_Decimal;

		public TextStylePair Property_Unit;
	}

	public TextStyleSetting[] HoverTextStyleSettings;

	public string ToolNameStringKey = string.Empty;

	public string ActionStringKey = string.Empty;

	[HideInInspector]
	public string ActionName = string.Empty;

	[HideInInspector]
	public string ToolName;

	protected string backStr;

	public TextStyleSetting ToolTitleTextStyle;

	public TextStylePair Styles_Title;

	public TextStylePair Styles_BodyText;

	public TextStylePair Styles_Instruction;

	public ValuePropertyTextStyles Styles_Values;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ConfigureHoverScreen();
	}

	protected virtual void ConfigureTitle(HoverTextScreen screen)
	{
		if (string.IsNullOrEmpty(ToolName))
		{
			ToolName = Strings.Get(ToolNameStringKey).String.ToUpper();
		}
	}

	protected void DrawTitle(HoverTextScreen screen, HoverTextDrawer drawer)
	{
		drawer.DrawText(ToolName, ToolTitleTextStyle);
	}

	protected void DrawInstructions(HoverTextScreen screen, HoverTextDrawer drawer)
	{
		TextStyleSetting standard = Styles_Instruction.Standard;
		drawer.NewLine(26);
		drawer.DrawIcon(screen.GetSprite("icon_mouse_left"), 20);
		drawer.DrawText(ActionName, standard);
		drawer.AddIndent(8);
		drawer.DrawIcon(screen.GetSprite("icon_mouse_right"), 20);
		drawer.DrawText(backStr, standard);
	}

	public virtual void ConfigureHoverScreen()
	{
		if (!string.IsNullOrEmpty(ActionStringKey))
		{
			ActionName = Strings.Get(ActionStringKey);
		}
		HoverTextScreen instance = HoverTextScreen.Instance;
		ConfigureTitle(instance);
		backStr = UI.TOOLS.GENERIC.BACK.ToString().ToUpper();
	}

	public virtual void UpdateHoverElements(List<KSelectable> hover_objects)
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		DrawTitle(instance, hoverTextDrawer);
		DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}
}
