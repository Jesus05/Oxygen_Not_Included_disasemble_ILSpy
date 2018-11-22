using TMPro;
using UnityEngine;

public class TextStyleSetting : ScriptableObject
{
	public TMP_FontAsset sdfFont;

	public int fontSize;

	public Color textColor;

	public FontStyles style;

	public bool enableWordWrapping = true;

	public void Init(TMP_FontAsset _sdfFont, int _fontSize, Color _color, bool _enableWordWrapping)
	{
		sdfFont = _sdfFont;
		fontSize = _fontSize;
		textColor = _color;
		enableWordWrapping = _enableWordWrapping;
	}
}
