using UnityEngine;

[AddComponentMenu("")]
public class FMOD_StudioEventEmitter : MonoBehaviour
{
	[Header("This component is obsolete. Use FMODUnity.StudioEventEmitter instead")]
	public FMODAsset asset;

	public string path = string.Empty;

	public bool startEventOnAwake = true;
}
