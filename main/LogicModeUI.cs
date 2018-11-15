using UnityEngine;

public class LogicModeUI : ScriptableObject
{
	[Header("Base Assets")]
	public Sprite inputSprite;

	public Sprite outputSprite;

	public Sprite resetSprite;

	public GameObject prefab;

	[Header("Colouring")]
	public Color32 colourOn = new Color32(0, byte.MaxValue, 0, 0);

	public Color32 colourOff = new Color32(byte.MaxValue, 0, 0, 0);

	public Color32 colourDisconnected = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
}
