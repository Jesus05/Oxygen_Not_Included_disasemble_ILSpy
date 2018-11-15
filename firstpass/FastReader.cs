using System.Text;

public class FastReader : IReader
{
	private int idx;

	private byte[] bytes;

	public bool IsFinished => bytes == null || idx == bytes.Length;

	public int Position
	{
		get
		{
			return idx;
		}
		set
		{
			idx = value;
		}
	}

	public FastReader(byte[] bytes)
	{
		this.bytes = bytes;
	}

	public unsafe byte ReadByte()
	{
		byte b = 0;
		fixed (byte* ptr = &bytes[idx])
		{
			b = *ptr;
		}
		idx++;
		return b;
	}

	public unsafe sbyte ReadSByte()
	{
		sbyte b = 0;
		fixed (byte* ptr = &bytes[idx])
		{
			b = (sbyte)(*ptr);
		}
		idx++;
		return b;
	}

	public unsafe ushort ReadUInt16()
	{
		ushort num = 0;
		fixed (byte* ptr = &bytes[idx])
		{
			num = *(ushort*)ptr;
		}
		idx += 2;
		return num;
	}

	public unsafe short ReadInt16()
	{
		short num = 0;
		fixed (byte* ptr = &bytes[idx])
		{
			num = *(short*)ptr;
		}
		idx += 2;
		return num;
	}

	public unsafe uint ReadUInt32()
	{
		uint num = 0u;
		fixed (byte* ptr = &bytes[idx])
		{
			num = *(uint*)ptr;
		}
		idx += 4;
		return num;
	}

	public unsafe int ReadInt32()
	{
		int num = 0;
		fixed (byte* ptr = &bytes[idx])
		{
			num = *(int*)ptr;
		}
		idx += 4;
		return num;
	}

	public unsafe ulong ReadUInt64()
	{
		ulong num = 0uL;
		fixed (byte* ptr = &bytes[idx])
		{
			num = (ulong)(*(long*)ptr);
		}
		idx += 8;
		return num;
	}

	public unsafe long ReadInt64()
	{
		long num = 0L;
		fixed (byte* ptr = &bytes[idx])
		{
			num = *(long*)ptr;
		}
		idx += 8;
		return num;
	}

	public unsafe float ReadSingle()
	{
		float num = 0f;
		fixed (byte* ptr = &bytes[idx])
		{
			num = *(float*)ptr;
		}
		idx += 4;
		return num;
	}

	public unsafe double ReadDouble()
	{
		double num = 0.0;
		fixed (byte* ptr = &bytes[idx])
		{
			num = *(double*)ptr;
		}
		idx += 8;
		return num;
	}

	public char[] ReadChars(int length)
	{
		char[] array = new char[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = (char)bytes[idx + i];
		}
		idx += length;
		return array;
	}

	public byte[] ReadBytes(int length)
	{
		byte[] array = new byte[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = bytes[idx + i];
		}
		idx += length;
		return array;
	}

	public string ReadKleiString()
	{
		int num = ReadInt32();
		string result = null;
		if (num >= 0)
		{
			result = Encoding.UTF8.GetString(bytes, idx, num);
			idx += num;
		}
		return result;
	}

	public void SkipBytes(int length)
	{
		idx += length;
	}

	public byte[] RawBytes()
	{
		return bytes;
	}
}
