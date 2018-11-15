using FMODUnity;
using UnityEngine;

public class GlobalResources : ScriptableObject
{
	public Material AnimMaterial;

	public Material AnimUIMaterial;

	public Material AnimPlaceMaterial;

	public Material AnimMaterialUIDesaturated;

	public Material AnimSimpleMaterial;

	public Material AnimOverlayMaterial;

	public Texture2D WhiteTexture;

	[EventRef]
	public string ConduitOverlaySoundLiquid;

	[EventRef]
	public string ConduitOverlaySoundGas;

	[EventRef]
	public string ConduitOverlaySoundSolid;

	[EventRef]
	public string AcousticDisturbanceSound;

	[EventRef]
	public string AcousticDisturbanceBubbleSound;

	[EventRef]
	public string WallDamageLayerSound;

	public Sprite sadDupeAudio;

	public Sprite sadDupe;

	private static GlobalResources _Instance;

	public static GlobalResources Instance()
	{
		if ((Object)_Instance == (Object)null)
		{
			_Instance = Resources.Load<GlobalResources>("GlobalResources");
		}
		return _Instance;
	}
}
