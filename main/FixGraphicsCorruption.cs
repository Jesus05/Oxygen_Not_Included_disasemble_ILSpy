using UnityEngine;

public class FixGraphicsCorruption : MonoBehaviour
{
	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		Graphics.Blit(source, dest);
	}
}
