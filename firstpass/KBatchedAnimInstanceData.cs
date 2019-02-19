using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class KBatchedAnimInstanceData
{
	[StructLayout(LayoutKind.Explicit)]
	public struct AnimInstanceData
	{
		[FieldOffset(0)]
		public float curAnimFrameIndex;

		[FieldOffset(4)]
		public float thisIndex;

		[FieldOffset(8)]
		public float currentAnimNumFrames;

		[FieldOffset(12)]
		public float currentAnimFirstFrameIdx;

		[FieldOffset(16)]
		public Matrix2x3 transformMatrix;

		[FieldOffset(40)]
		public float blend;

		[FieldOffset(44)]
		public float unused;

		[FieldOffset(48)]
		public Color highlightColour;

		[FieldOffset(64)]
		public Color tintColour;

		[FieldOffset(80)]
		public Color overlayColour;

		[FieldOffset(96)]
		public Vector4 clipParameters;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct AnimInstanceDataToByteConverter
	{
		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public AnimInstanceData[] animInstanceData;
	}

	public const int SIZE_IN_BYTES = 112;

	public const int SIZE_IN_FLOATS = 28;

	private KAnimConverter.IAnimConverter target;

	private bool isTransformOverriden;

	private AnimInstanceDataToByteConverter converter;

	public KBatchedAnimInstanceData(KAnimConverter.IAnimConverter target)
	{
		this.target = target;
		converter = new AnimInstanceDataToByteConverter
		{
			bytes = new byte[112]
		};
		AnimInstanceData animInstanceData = converter.animInstanceData[0];
		animInstanceData.tintColour = Color.white;
		animInstanceData.highlightColour = Color.black;
		animInstanceData.overlayColour = Color.white;
		converter.animInstanceData[0] = animInstanceData;
	}

	public void SetClipRadius(float x, float y, float dist_sq, bool do_clip)
	{
		converter.animInstanceData[0].clipParameters = new Vector4(x, y, dist_sq, (float)(do_clip ? 1 : 0));
	}

	public void SetBlend(float amt)
	{
		converter.animInstanceData[0].blend = amt;
	}

	public Color GetOverlayColour()
	{
		return converter.animInstanceData[0].overlayColour;
	}

	public bool SetOverlayColour(Color color)
	{
		if (color != converter.animInstanceData[0].overlayColour)
		{
			converter.animInstanceData[0].overlayColour = color;
			return true;
		}
		return false;
	}

	public Color GetTintColour()
	{
		return converter.animInstanceData[0].tintColour;
	}

	public bool SetTintColour(Color color)
	{
		if (color != converter.animInstanceData[0].tintColour)
		{
			converter.animInstanceData[0].tintColour = color;
			return true;
		}
		return false;
	}

	public Color GetHighlightcolour()
	{
		return converter.animInstanceData[0].highlightColour;
	}

	public bool SetHighlightColour(Color color)
	{
		if (color != converter.animInstanceData[0].highlightColour)
		{
			converter.animInstanceData[0].highlightColour = color;
			return true;
		}
		return false;
	}

	public void WriteToTexture(byte[] output_bytes, int output_index, int this_index)
	{
		AnimInstanceData animInstanceData = converter.animInstanceData[0];
		animInstanceData.curAnimFrameIndex = (float)target.GetCurrentFrameIndex();
		animInstanceData.thisIndex = (float)this_index;
		animInstanceData.currentAnimNumFrames = (float)(target.IsVisible() ? target.GetCurrentNumFrames() : 0);
		animInstanceData.currentAnimFirstFrameIdx = (float)target.GetFirstFrameIndex();
		if (!isTransformOverriden)
		{
			animInstanceData.transformMatrix = target.GetTransformMatrix();
		}
		converter.animInstanceData[0] = animInstanceData;
		Buffer.BlockCopy(converter.bytes, 0, output_bytes, output_index, 112);
	}

	public void SetOverrideTransformMatrix(Matrix2x3 transform_matrix)
	{
		isTransformOverriden = true;
		converter.animInstanceData[0].transformMatrix = transform_matrix;
	}

	public void ClearOverrideTransformMatrix()
	{
		isTransformOverriden = false;
	}
}
