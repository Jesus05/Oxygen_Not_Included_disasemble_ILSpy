using System.Collections.Generic;
using UnityEngine;

public class AttackToolHoverTextCard : HoverTextConfiguration
{
	public override void UpdateHoverElements(List<KSelectable> hover_objects)
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		DrawTitle(instance, hoverTextDrawer);
		DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
		hoverTextDrawer.EndShadowBar();
		if (hover_objects != null)
		{
			foreach (KSelectable hover_object in hover_objects)
			{
				if ((Object)hover_object.GetComponent<AttackableBase>() != (Object)null)
				{
					hoverTextDrawer.BeginShadowBar(false);
					hoverTextDrawer.DrawText(hover_object.GetProperName().ToUpper(), Styles_Title.Standard);
					hoverTextDrawer.EndShadowBar();
					break;
				}
			}
		}
		hoverTextDrawer.EndDrawing();
	}
}
