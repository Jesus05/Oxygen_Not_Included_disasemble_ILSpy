using LibNoiseDotNet.Graphics.Tools.Noise.Renderer;
using System;
using System.IO;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Writer
{
	public class Heightmap16RawWriter : AbstractWriter
	{
		protected Heightmap16 _heightmap;

		public Heightmap16 Heightmap
		{
			get
			{
				return _heightmap;
			}
			set
			{
				_heightmap = value;
			}
		}

		public override void WriteFile()
		{
			if (_heightmap == null)
			{
				throw new ArgumentException("An heightmap must be provided");
			}
			OpenFile();
			ushort[] array = _heightmap.Share();
			try
			{
				for (int i = 0; i < array.Length; i++)
				{
					_writer.Write(array[i]);
				}
			}
			catch (Exception innerException)
			{
				throw new IOException("Unknown IO exception", innerException);
			}
			CloseFile();
		}
	}
}
