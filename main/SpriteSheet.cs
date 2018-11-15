using System;
using UnityEngine;

[Serializable]
public struct SpriteSheet
{
	public string name;

	public int numFrames;

	public int numXFrames;

	public Vector2 uvFrameSize;

	public int renderLayer;

	public Material material;

	public Texture2D texture;
}
