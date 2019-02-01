using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	internal static class StringHelper
	{
		public class ThreadSafeEncoding : IDisposable
		{
			private UTF8Encoding encoding = new UTF8Encoding();

			private byte[] encodedBuffer = new byte[128];

			private char[] decodedBuffer = new char[128];

			private bool inUse;

			public bool InUse()
			{
				return inUse;
			}

			public void SetInUse()
			{
				inUse = true;
			}

			private int roundUpPowerTwo(int number)
			{
				int num;
				for (num = 1; num <= number; num *= 2)
				{
				}
				return num;
			}

			public byte[] byteFromStringUTF8(string s)
			{
				if (s != null)
				{
					int num = encoding.GetMaxByteCount(s.Length) + 1;
					if (num > encodedBuffer.Length)
					{
						int num2 = encoding.GetByteCount(s) + 1;
						if (num2 > encodedBuffer.Length)
						{
							encodedBuffer = new byte[roundUpPowerTwo(num2)];
						}
					}
					int bytes = encoding.GetBytes(s, 0, s.Length, encodedBuffer, 0);
					encodedBuffer[bytes] = 0;
					return encodedBuffer;
				}
				return null;
			}

			public string stringFromNative(IntPtr nativePtr)
			{
				if (!(nativePtr == IntPtr.Zero))
				{
					int i;
					for (i = 0; Marshal.ReadByte(nativePtr, i) != 0; i++)
					{
					}
					if (i != 0)
					{
						if (i > encodedBuffer.Length)
						{
							encodedBuffer = new byte[roundUpPowerTwo(i)];
						}
						Marshal.Copy(nativePtr, encodedBuffer, 0, i);
						int maxCharCount = encoding.GetMaxCharCount(i);
						if (maxCharCount > decodedBuffer.Length)
						{
							int charCount = encoding.GetCharCount(encodedBuffer, 0, i);
							if (charCount > decodedBuffer.Length)
							{
								decodedBuffer = new char[roundUpPowerTwo(charCount)];
							}
						}
						int chars = encoding.GetChars(encodedBuffer, 0, i, decodedBuffer, 0);
						return new string(decodedBuffer, 0, chars);
					}
					return "";
				}
				return "";
			}

			public void Dispose()
			{
				lock (encoders)
				{
					inUse = false;
				}
			}
		}

		private static List<ThreadSafeEncoding> encoders = new List<ThreadSafeEncoding>(1);

		public static ThreadSafeEncoding GetFreeHelper()
		{
			lock (encoders)
			{
				ThreadSafeEncoding threadSafeEncoding = null;
				for (int i = 0; i < encoders.Count; i++)
				{
					if (!encoders[i].InUse())
					{
						threadSafeEncoding = encoders[i];
						break;
					}
				}
				if (threadSafeEncoding == null)
				{
					threadSafeEncoding = new ThreadSafeEncoding();
					encoders.Add(threadSafeEncoding);
				}
				threadSafeEncoding.SetInUse();
				return threadSafeEncoding;
			}
		}
	}
}
