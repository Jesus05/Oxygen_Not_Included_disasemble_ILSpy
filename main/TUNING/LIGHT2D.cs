using UnityEngine;

namespace TUNING
{
	public class LIGHT2D
	{
		public const int SUNLIGHT_MAX_DEFAULT = 80000;

		public static readonly Color LIGHT_YELLOW = new Color(0.57f, 0.55f, 0.44f, 1f);

		public static readonly Color LIGHT_OVERLAY = new Color(0.56f, 0.56f, 0.56f, 1f);

		public static readonly Vector2 DEFAULT_DIRECTION = new Vector2(0f, -1f);

		public const int FLOORLAMP_LUX = 1000;

		public const float FLOORLAMP_RANGE = 4f;

		public const float FLOORLAMP_ANGLE = 0f;

		public const LightShape FLOORLAMP_SHAPE = LightShape.Circle;

		public static readonly Color FLOORLAMP_COLOR = LIGHT_YELLOW;

		public static readonly Color FLOORLAMP_OVERLAYCOLOR = LIGHT_OVERLAY;

		public static readonly Vector2 FLOORLAMP_OFFSET = new Vector2(0.05f, 1.5f);

		public static readonly Vector2 FLOORLAMP_DIRECTION = DEFAULT_DIRECTION;

		public const float CEILINGLIGHT_RANGE = 8f;

		public const float CEILINGLIGHT_ANGLE = 2.6f;

		public const LightShape CEILINGLIGHT_SHAPE = LightShape.Cone;

		public static readonly Color CEILINGLIGHT_COLOR = LIGHT_YELLOW;

		public static readonly Color CEILINGLIGHT_OVERLAYCOLOR = LIGHT_OVERLAY;

		public static readonly Vector2 CEILINGLIGHT_OFFSET = new Vector2(0.05f, 0.65f);

		public static readonly Vector2 CEILINGLIGHT_DIRECTION = DEFAULT_DIRECTION;

		public const int CEILINGLIGHT_LUX = 1800;

		public static readonly Color LIGHT_PREVIEW_COLOR = LIGHT_YELLOW;

		public const float HEADQUARTERS_RANGE = 5f;

		public const LightShape HEADQUARTERS_SHAPE = LightShape.Circle;

		public static readonly Color HEADQUARTERS_COLOR = LIGHT_YELLOW;

		public static readonly Color HEADQUARTERS_OVERLAYCOLOR = LIGHT_OVERLAY;

		public static readonly Vector2 HEADQUARTERS_OFFSET = new Vector2(0.5f, 3f);

		public const float WALLLIGHT_RANGE = 4f;

		public const float WALLLIGHT_ANGLE = 0f;

		public const LightShape WALLLIGHT_SHAPE = LightShape.Circle;

		public static readonly Color WALLLIGHT_COLOR = LIGHT_YELLOW;

		public static readonly Color WALLLIGHT_OVERLAYCOLOR = LIGHT_OVERLAY;

		public static readonly Vector2 WALLLIGHT_OFFSET = new Vector2(0f, 0.5f);

		public static readonly Vector2 WALLLIGHT_DIRECTION = DEFAULT_DIRECTION;

		public const float LIGHTBUG_RANGE = 5f;

		public const float LIGHTBUG_ANGLE = 0f;

		public const LightShape LIGHTBUG_SHAPE = LightShape.Circle;

		public const int LIGHTBUG_LUX = 1800;

		public static readonly Color LIGHTBUG_COLOR = LIGHT_YELLOW;

		public static readonly Color LIGHTBUG_OVERLAYCOLOR = LIGHT_OVERLAY;

		public static readonly Color LIGHTBUG_COLOR_ORANGE = new Color(0.5686275f, 0.482352942f, 0.4392157f, 1f);

		public static readonly Color LIGHTBUG_COLOR_PURPLE = new Color(0.490196079f, 0.4392157f, 0.5686275f, 1f);

		public static readonly Color LIGHTBUG_COLOR_PINK = new Color(0.5686275f, 0.4392157f, 0.5686275f, 1f);

		public static readonly Color LIGHTBUG_COLOR_BLUE = new Color(0.4392157f, 0.4862745f, 0.5686275f, 1f);

		public static readonly Color LIGHTBUG_COLOR_CRYSTAL = new Color(0.5137255f, 0.6666667f, 0.6666667f, 1f);

		public static readonly Vector2 LIGHTBUG_OFFSET = new Vector2(0.05f, 0.25f);

		public static readonly Vector2 LIGHTBUG_DIRECTION = DEFAULT_DIRECTION;
	}
}
