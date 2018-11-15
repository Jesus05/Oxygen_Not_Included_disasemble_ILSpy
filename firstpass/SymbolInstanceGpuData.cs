using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class SymbolInstanceGpuData
{
	[StructLayout(LayoutKind.Explicit)]
	public struct SymbolInstance
	{
		[FieldOffset(0)]
		public float symbolIndex;

		[FieldOffset(4)]
		public float isVisible;

		[FieldOffset(8)]
		public float scale;

		[FieldOffset(12)]
		public float unused;

		[FieldOffset(16)]
		public Color color;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct SymbolInstanceToByteConverter
	{
		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public SymbolInstance[] symbolInstances;
	}

	public const int FLOATS_PER_SYMBOL_INSTANCE = 8;

	private SymbolInstanceToByteConverter symbolInstancesConverter;

	private int symbolCount;

	private SymbolInstance[] symbolInstances => symbolInstancesConverter.symbolInstances;

	public int version
	{
		get;
		private set;
	}

	public SymbolInstanceGpuData(int symbol_count)
	{
		symbolCount = symbol_count;
		symbolInstancesConverter = new SymbolInstanceToByteConverter
		{
			bytes = new byte[8 * symbol_count * 4]
		};
		for (int i = 0; i < symbol_count; i++)
		{
			symbolInstances[i].isVisible = 1f;
			symbolInstances[i].symbolIndex = -1f;
			symbolInstances[i].scale = 1f;
			symbolInstances[i].unused = 1f;
			symbolInstances[i].color = Color.white;
		}
		MarkDirty();
	}

	private void MarkDirty()
	{
		version++;
	}

	public void SetVisible(int symbol_idx, bool is_visible)
	{
		DebugUtil.Assert(symbol_idx < symbolCount, "Assert!", string.Empty, string.Empty);
		float num = 0f;
		if (is_visible)
		{
			num = 1f;
		}
		if (symbolInstances[symbol_idx].isVisible != num)
		{
			symbolInstances[symbol_idx].isVisible = num;
			MarkDirty();
		}
	}

	public bool IsVisible(int symbol_idx)
	{
		DebugUtil.Assert(symbol_idx < symbolCount, "Assert!", string.Empty, string.Empty);
		return symbolInstances[symbol_idx].isVisible > 0.5f;
	}

	public void SetSymbolScale(int symbol_index, float scale)
	{
		DebugUtil.Assert(symbol_index < symbolCount, "Assert!", string.Empty, string.Empty);
		if (symbolInstances[symbol_index].scale != scale)
		{
			symbolInstances[symbol_index].scale = scale;
			MarkDirty();
		}
	}

	public void SetSymbolTint(int symbol_index, Color color)
	{
		DebugUtil.Assert(symbol_index < symbolCount, "Assert!", string.Empty, string.Empty);
		if (symbolInstances[symbol_index].color != color)
		{
			symbolInstances[symbol_index].color = color;
			MarkDirty();
		}
	}

	public void WriteToTexture(byte[] data, int data_idx, int instance_idx)
	{
		Buffer.BlockCopy(symbolInstancesConverter.bytes, 0, data, data_idx, symbolCount * 8 * 4);
	}
}
