using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class SymbolOverrideInfoGpuData
{
	[StructLayout(LayoutKind.Explicit)]
	public struct SymbolOverrideInfo
	{
		[FieldOffset(0)]
		public float atlas;

		[FieldOffset(4)]
		public float isoverriden;

		[FieldOffset(8)]
		public float unused1;

		[FieldOffset(12)]
		public float unused2;

		[FieldOffset(16)]
		public Vector2 bboxMin;

		[FieldOffset(24)]
		public Vector2 bboxMax;

		[FieldOffset(32)]
		public Vector2 uvMin;

		[FieldOffset(40)]
		public Vector2 uvMax;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct SymbolOverrideInfoToByteConverter
	{
		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public SymbolOverrideInfo[] symbolOverrideInfos;
	}

	public const int FLOATS_PER_SYMBOL_OVERRIDE_INFO = 12;

	private SymbolOverrideInfoToByteConverter symbolOverrideInfoConverter;

	private int symbolCount;

	private SymbolOverrideInfo[] symbolOverrideInfos => symbolOverrideInfoConverter.symbolOverrideInfos;

	public int version
	{
		get;
		private set;
	}

	public SymbolOverrideInfoGpuData(int symbol_count)
	{
		symbolCount = symbol_count;
		symbolOverrideInfoConverter = new SymbolOverrideInfoToByteConverter
		{
			bytes = new byte[12 * symbol_count * 4]
		};
		for (int i = 0; i < symbol_count; i++)
		{
			symbolOverrideInfos[i].atlas = 0f;
		}
		MarkDirty();
	}

	private void MarkDirty()
	{
		version++;
	}

	public void SetSymbolOverrideInfo(int symbol_idx, KAnim.Build.SymbolFrameInstance symbol_frame_instance)
	{
		if (symbol_idx >= symbolCount)
		{
			DebugUtil.Assert(false);
		}
		SymbolOverrideInfo symbolOverrideInfo = symbolOverrideInfos[symbol_idx];
		symbolOverrideInfo.atlas = (float)symbol_frame_instance.buildImageIdx;
		symbolOverrideInfo.isoverriden = 1f;
		symbolOverrideInfo.bboxMin = symbol_frame_instance.symbolFrame.bboxMin;
		symbolOverrideInfo.bboxMax = symbol_frame_instance.symbolFrame.bboxMax;
		symbolOverrideInfo.uvMin = symbol_frame_instance.symbolFrame.uvMin;
		symbolOverrideInfo.uvMax = symbol_frame_instance.symbolFrame.uvMax;
		symbolOverrideInfos[symbol_idx] = symbolOverrideInfo;
		MarkDirty();
	}

	public void WriteToTexture(byte[] data, int data_idx, int instance_idx)
	{
		DebugUtil.Assert(instance_idx * symbolCount * 12 * 4 == data_idx);
		Buffer.BlockCopy(symbolOverrideInfoConverter.bytes, 0, data, data_idx, symbolCount * 12 * 4);
	}
}
