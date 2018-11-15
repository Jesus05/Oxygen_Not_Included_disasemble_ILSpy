using System;
using UnityEngine;

namespace OverlayModes
{
	public class ColorHighlightCondition
	{
		public Func<KMonoBehaviour, Color> highlight_color;

		public Func<KMonoBehaviour, bool> highlight_condition;

		public ColorHighlightCondition(Func<KMonoBehaviour, Color> highlight_color, Func<KMonoBehaviour, bool> highlight_condition)
		{
			this.highlight_color = highlight_color;
			this.highlight_condition = highlight_condition;
		}
	}
}
