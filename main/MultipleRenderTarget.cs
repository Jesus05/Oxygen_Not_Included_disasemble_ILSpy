using System;
using System.Collections;
using UnityEngine;

public class MultipleRenderTarget : MonoBehaviour
{
	private MultipleRenderTargetProxy renderProxy;

	private FullScreenQuad quad;

	public bool isFrontEnd = false;

	public event Action<Camera> onSetupComplete;

	private void Start()
	{
		StartCoroutine(SetupProxy());
	}

	private IEnumerator SetupProxy()
	{
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private void OnPreCull()
	{
		if ((UnityEngine.Object)renderProxy != (UnityEngine.Object)null)
		{
			quad.Draw(renderProxy.Textures[0]);
		}
	}

	public void ToggleColouredOverlayView(bool enabled)
	{
		if ((UnityEngine.Object)renderProxy != (UnityEngine.Object)null)
		{
			renderProxy.ToggleColouredOverlayView(enabled);
		}
	}
}
