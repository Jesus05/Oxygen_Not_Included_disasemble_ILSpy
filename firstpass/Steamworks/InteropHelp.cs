using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	public class InteropHelp
	{
		public class UTF8StringHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			public UTF8StringHandle(string str)
				: base(true)
			{
				if (str == null)
				{
					SetHandle(IntPtr.Zero);
				}
				else
				{
					byte[] array = new byte[Encoding.UTF8.GetByteCount(str) + 1];
					Encoding.UTF8.GetBytes(str, 0, str.Length, array, 0);
					IntPtr intPtr = Marshal.AllocHGlobal(array.Length);
					Marshal.Copy(array, 0, intPtr, array.Length);
					SetHandle(intPtr);
				}
			}

			protected override bool ReleaseHandle()
			{
				if (!IsInvalid)
				{
					Marshal.FreeHGlobal(handle);
				}
				return true;
			}
		}

		public class SteamParamStringArray
		{
			private IntPtr[] m_Strings;

			private IntPtr m_ptrStrings;

			private IntPtr m_pSteamParamStringArray;

			public SteamParamStringArray(IList<string> strings)
			{
				if (strings == null)
				{
					m_pSteamParamStringArray = IntPtr.Zero;
				}
				else
				{
					m_Strings = new IntPtr[strings.Count];
					for (int i = 0; i < strings.Count; i++)
					{
						byte[] array = new byte[Encoding.UTF8.GetByteCount(strings[i]) + 1];
						Encoding.UTF8.GetBytes(strings[i], 0, strings[i].Length, array, 0);
						m_Strings[i] = Marshal.AllocHGlobal(array.Length);
						Marshal.Copy(array, 0, m_Strings[i], array.Length);
					}
					m_ptrStrings = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * m_Strings.Length);
					SteamParamStringArray_t steamParamStringArray_t = new SteamParamStringArray_t
					{
						m_ppStrings = m_ptrStrings,
						m_nNumStrings = m_Strings.Length
					};
					Marshal.Copy(m_Strings, 0, steamParamStringArray_t.m_ppStrings, m_Strings.Length);
					m_pSteamParamStringArray = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SteamParamStringArray_t)));
					Marshal.StructureToPtr(steamParamStringArray_t, m_pSteamParamStringArray, false);
				}
			}

			~SteamParamStringArray()
			{
				IntPtr[] strings = m_Strings;
				foreach (IntPtr hglobal in strings)
				{
					Marshal.FreeHGlobal(hglobal);
				}
				if (m_ptrStrings != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(m_ptrStrings);
				}
				if (m_pSteamParamStringArray != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(m_pSteamParamStringArray);
				}
			}

			public static implicit operator IntPtr(SteamParamStringArray that)
			{
				return that.m_pSteamParamStringArray;
			}
		}

		public static void TestIfPlatformSupported()
		{
		}

		public static void TestIfAvailableClient()
		{
			TestIfPlatformSupported();
			if (NativeMethods.SteamClient() == IntPtr.Zero)
			{
				throw new InvalidOperationException("Steamworks is not initialized.");
			}
		}

		public static void TestIfAvailableGameServer()
		{
			TestIfPlatformSupported();
			if (NativeMethods.SteamGameServerClient() == IntPtr.Zero)
			{
				throw new InvalidOperationException("Steamworks is not initialized.");
			}
		}

		public static string PtrToStringUTF8(IntPtr nativeUtf8)
		{
			if (!(nativeUtf8 == IntPtr.Zero))
			{
				int i;
				for (i = 0; Marshal.ReadByte(nativeUtf8, i) != 0; i++)
				{
				}
				if (i != 0)
				{
					byte[] array = new byte[i];
					Marshal.Copy(nativeUtf8, array, 0, array.Length);
					return Encoding.UTF8.GetString(array);
				}
				return string.Empty;
			}
			return null;
		}
	}
}
