using UnityEngine;

public class FillRenderTargetEffect : MonoBehaviour
{
	private Texture fillTexture;

	public void SetFillTexture(Texture tex)
	{
		fillTexture = tex;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(fillTexture, (RenderTexture)null);
	}
}
