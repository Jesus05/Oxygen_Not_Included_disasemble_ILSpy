using System;
using System.IO;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Writer
{
	public abstract class AbstractWriter
	{
		protected string _filename;

		protected BinaryWriter _writer;

		public string Filename
		{
			get
			{
				return _filename;
			}
			set
			{
				_filename = value;
			}
		}

		public abstract void WriteFile();

		protected void OpenFile()
		{
			if (_writer == null)
			{
				if (File.Exists(_filename))
				{
					try
					{
						File.Delete(_filename);
					}
					catch (Exception innerException)
					{
						throw new IOException("Unable to delete destination file", innerException);
					}
				}
				BufferedStream output;
				try
				{
					output = new BufferedStream(new FileStream(_filename, FileMode.Create));
				}
				catch (Exception innerException2)
				{
					throw new IOException("Unable to create destination file", innerException2);
				}
				_writer = new BinaryWriter(output);
			}
		}

		protected void CloseFile()
		{
			try
			{
				_writer.Flush();
				_writer.Close();
				_writer = null;
			}
			catch (Exception innerException)
			{
				throw new IOException("Unable to release stream", innerException);
			}
		}
	}
}
