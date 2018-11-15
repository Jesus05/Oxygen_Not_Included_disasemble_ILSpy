using System;
using System.Diagnostics;
using UnityEngine;

[Serializable]
[DebuggerDisplay("{name}")]
public class TintedSprite : ISerializationCallbackReceiver
{
	[ReadOnly]
	public string name;

	public Sprite sprite;

	public Color color;

	public void OnAfterDeserialize()
	{
	}

	public void OnBeforeSerialize()
	{
		if ((UnityEngine.Object)sprite != (UnityEngine.Object)null)
		{
			name = sprite.name;
		}
	}
}
