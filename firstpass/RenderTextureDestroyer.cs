using System.Collections.Generic;
using UnityEngine;

public class RenderTextureDestroyer : KMonoBehaviour
{
	public static RenderTextureDestroyer Instance;

	public List<RenderTexture> queued = new List<RenderTexture>();

	public List<RenderTexture> finished = new List<RenderTexture>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	public void Add(RenderTexture render_texture)
	{
		queued.Add(render_texture);
	}

	private void LateUpdate()
	{
		foreach (RenderTexture item in finished)
		{
			Object.Destroy(item);
		}
		finished.Clear();
		finished.AddRange(queued);
		queued.Clear();
	}
}
