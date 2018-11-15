using UnityEngine;

public class ColorStyleSetting : ScriptableObject
{
	public Color activeColor;

	public Color inactiveColor;

	public Color disabledColor;

	public Color disabledActiveColor;

	public Color hoverColor;

	public Color disabledhoverColor = Color.grey;

	public void Init(Color _color)
	{
		activeColor = _color;
		inactiveColor = _color;
		disabledColor = _color;
		disabledActiveColor = _color;
		hoverColor = _color;
		disabledhoverColor = _color;
	}
}
