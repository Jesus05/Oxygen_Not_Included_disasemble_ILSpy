using System.Runtime.InteropServices;

public struct TextureRegion
{
	[StructLayout(LayoutKind.Explicit)]
	public struct ByteToFloatConverter
	{
		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public float[] floats;
	}

	public int x;

	public int y;

	public int bytesPerPixel;

	public byte[] bytes;

	public int width;

	public TexturePage page;

	public TextureBuffer buffer;

	public ByteToFloatConverter floatConverter;

	public TextureRegion(int x, int y, TexturePage page, TextureBuffer buffer)
	{
		this.x = x;
		this.y = y;
		this.page = page;
		this.buffer = buffer;
		width = page.width;
		bytesPerPixel = TextureUtil.GetBytesPerPixel(page.format);
		bytes = page.bytes;
		floatConverter = new ByteToFloatConverter
		{
			bytes = page.bytes
		};
	}

	private int GetByteIdx(int x, int y)
	{
		int num = x - this.x;
		int num2 = y - this.y;
		return (num + num2 * width) * bytesPerPixel;
	}

	public void SetBytes(int x, int y, byte b0)
	{
		int byteIdx = GetByteIdx(x, y);
		bytes[byteIdx] = b0;
	}

	public void SetBytes(int x, int y, byte b0, byte b1)
	{
		int byteIdx = GetByteIdx(x, y);
		bytes[byteIdx] = b0;
		bytes[byteIdx + 1] = b1;
	}

	public void SetBytes(int x, int y, byte b0, byte b1, byte b2)
	{
		int byteIdx = GetByteIdx(x, y);
		bytes[byteIdx] = b0;
		bytes[byteIdx + 1] = b1;
		bytes[byteIdx + 2] = b2;
	}

	public void SetBytes(int x, int y, byte b0, byte b1, byte b2, byte b3)
	{
		int byteIdx = GetByteIdx(x, y);
		bytes[byteIdx] = b0;
		bytes[byteIdx + 1] = b1;
		bytes[byteIdx + 2] = b2;
		bytes[byteIdx + 3] = b3;
	}

	public void SetBytes(int x, int y, float v0, float v1)
	{
		int byteIdx = GetByteIdx(x, y);
		int num = byteIdx / 4;
		floatConverter.floats[num] = v0;
		floatConverter.floats[num + 1] = v1;
	}

	public void Unlock()
	{
		buffer.Unlock(this);
	}
}
