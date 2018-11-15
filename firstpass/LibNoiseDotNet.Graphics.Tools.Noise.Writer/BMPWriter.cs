using LibNoiseDotNet.Graphics.Tools.Noise.Renderer;
using System;
using System.IO;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Writer
{
	public class BMPWriter : AbstractWriter
	{
		public const int BMP_HEADER_SIZE = 54;

		protected Image _image;

		public Image Image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
			}
		}

		public override void WriteFile()
		{
			if (_image == null)
			{
				throw new ArgumentException("An image map must be provided");
			}
			int width = _image.Width;
			int height = _image.Height;
			int num = CalcWidthByteCount(width);
			int num2 = num * height;
			byte[] array = new byte[num];
			OpenFile();
			byte[] buffer = new byte[4];
			byte[] buffer2 = new byte[2]
			{
				66,
				77
			};
			try
			{
				_writer.Write(buffer2);
				_writer.Write(Libnoise.UnpackLittleUint32(num2 + 54, ref buffer));
				_writer.Write(Libnoise.UnpackLittleUint32(0, ref buffer));
				_writer.Write(Libnoise.UnpackLittleUint32(54, ref buffer));
				_writer.Write(Libnoise.UnpackLittleUint32(40, ref buffer));
				_writer.Write(Libnoise.UnpackLittleUint32(width, ref buffer));
				_writer.Write(Libnoise.UnpackLittleUint32(height, ref buffer));
				_writer.Write(Libnoise.UnpackLittleUint16(1, ref buffer2));
				_writer.Write(Libnoise.UnpackLittleUint16(24, ref buffer2));
				_writer.Write(Libnoise.UnpackLittleUint32(0, ref buffer));
				_writer.Write(Libnoise.UnpackLittleUint32(num2, ref buffer));
				_writer.Write(Libnoise.UnpackLittleUint32(2834, ref buffer));
				_writer.Write(Libnoise.UnpackLittleUint32(2834, ref buffer));
				_writer.Write(Libnoise.UnpackLittleUint32(0, ref buffer));
				_writer.Write(buffer);
				for (int i = 0; i < height; i++)
				{
					int num3 = 0;
					Array.Clear(array, 0, array.Length);
					for (int j = 0; j < width; j++)
					{
						Color value = _image.GetValue(j, i);
						array[num3++] = value.Blue;
						array[num3++] = value.Green;
						array[num3++] = value.Red;
					}
					_writer.Write(array);
				}
			}
			catch (Exception innerException)
			{
				throw new IOException("Unknown IO exception", innerException);
			}
			CloseFile();
		}

		protected int CalcWidthByteCount(int width)
		{
			return (width * 3 + 3) & -4;
		}
	}
}
