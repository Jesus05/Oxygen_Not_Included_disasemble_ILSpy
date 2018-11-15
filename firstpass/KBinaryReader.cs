using System.IO;
using System.Text;

public class KBinaryReader : BinaryReader, IReader
{
	public bool IsFinished => BaseStream.Position == BaseStream.Length;

	public int Position => (int)BaseStream.Position;

	public KBinaryReader(Stream stream)
		: base(stream)
	{
	}

	public void SkipBytes(int length)
	{
		ReadBytes(length);
	}

	public string ReadKleiString()
	{
		string result = null;
		int num = ReadInt32();
		if (num >= 0)
		{
			byte[] bytes = ReadBytes(num);
			result = Encoding.UTF8.GetString(bytes, 0, num);
		}
		return result;
	}

	public byte[] RawBytes()
	{
		long position = BaseStream.Position;
		BaseStream.Position = 0L;
		byte[] result = ReadBytes((int)BaseStream.Length);
		BaseStream.Position = position;
		return result;
	}
}
