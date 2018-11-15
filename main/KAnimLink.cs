using System;
using UnityEngine;

public class KAnimLink
{
	private KAnimControllerBase master;

	private KAnimControllerBase slave;

	public KAnimLink(KAnimControllerBase master, KAnimControllerBase slave)
	{
		this.slave = slave;
		this.master = master;
		Register();
	}

	private void Register()
	{
		master.OnOverlayColourChanged += OnOverlayColourChanged;
		KAnimControllerBase kAnimControllerBase = master;
		kAnimControllerBase.OnTintChanged = (Action<Color>)Delegate.Combine(kAnimControllerBase.OnTintChanged, new Action<Color>(OnTintColourChanged));
		KAnimControllerBase kAnimControllerBase2 = master;
		kAnimControllerBase2.OnHighlightChanged = (Action<Color>)Delegate.Combine(kAnimControllerBase2.OnHighlightChanged, new Action<Color>(OnHighlightColourChanged));
		KAnimControllerBase kAnimControllerBase3 = master;
		KAnimControllerBase kAnimControllerBase4 = slave;
		kAnimControllerBase3.onLayerChanged += kAnimControllerBase4.SetLayer;
	}

	public void Unregister()
	{
		if ((UnityEngine.Object)master != (UnityEngine.Object)null)
		{
			master.OnOverlayColourChanged -= OnOverlayColourChanged;
			KAnimControllerBase kAnimControllerBase = master;
			kAnimControllerBase.OnTintChanged = (Action<Color>)Delegate.Remove(kAnimControllerBase.OnTintChanged, new Action<Color>(OnTintColourChanged));
			KAnimControllerBase kAnimControllerBase2 = master;
			kAnimControllerBase2.OnHighlightChanged = (Action<Color>)Delegate.Remove(kAnimControllerBase2.OnHighlightChanged, new Action<Color>(OnHighlightColourChanged));
			if ((UnityEngine.Object)slave != (UnityEngine.Object)null)
			{
				KAnimControllerBase kAnimControllerBase3 = master;
				KAnimControllerBase kAnimControllerBase4 = slave;
				kAnimControllerBase3.onLayerChanged -= kAnimControllerBase4.SetLayer;
			}
		}
	}

	private void OnOverlayColourChanged(Color32 c)
	{
		if ((UnityEngine.Object)slave != (UnityEngine.Object)null)
		{
			slave.OverlayColour = c;
		}
	}

	private void OnTintColourChanged(Color c)
	{
		if ((UnityEngine.Object)slave != (UnityEngine.Object)null)
		{
			slave.TintColour = c;
		}
	}

	private void OnHighlightColourChanged(Color c)
	{
		if ((UnityEngine.Object)slave != (UnityEngine.Object)null)
		{
			slave.HighlightColour = c;
		}
	}
}
