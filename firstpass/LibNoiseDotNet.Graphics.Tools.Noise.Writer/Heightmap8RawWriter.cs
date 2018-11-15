using LibNoiseDotNet.Graphics.Tools.Noise.Renderer;
using System;
using System.IO;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Writer
{
	public class Heightmap8RawWriter : AbstractWriter
	{
		protected Heightmap8 _heightmap;

		public Heightmap8 Heightmap
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
			try
			{
				_writer.Write(_heightmap.Share());
			}
			catch (Exception innerException)
			{
				throw new IOException("Unknown IO exception", innerException);
			}
			CloseFile();
		}
	}
}
