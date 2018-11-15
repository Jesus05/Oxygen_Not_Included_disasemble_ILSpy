using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct CREATESOUNDEXINFO
	{
		public int cbsize;

		public uint length;

		public uint fileoffset;

		public int numchannels;

		public int defaultfrequency;

		public SOUND_FORMAT format;

		public uint decodebuffersize;

		public int initialsubsound;

		public int numsubsounds;

		public IntPtr inclusionlist;

		public int inclusionlistnum;

		public IntPtr pcmreadcallback_handle;

		public IntPtr pcmsetposcallback_handle;

		public IntPtr nonblockcallback_handle;

		public IntPtr dlsname;

		public IntPtr encryptionkey;

		public int maxpolyphony;

		public IntPtr userdata;

		public SOUND_TYPE suggestedsoundtype;

		public IntPtr fileuseropen_handle;

		public IntPtr fileuserclose_handle;

		public IntPtr fileuserread_handle;

		public IntPtr fileuserseek_handle;

		public IntPtr fileuserasyncread_handle;

		public IntPtr fileuserasynccancel_handle;

		public IntPtr fileuserdata;

		public int filebuffersize;

		public CHANNELORDER channelorder;

		public CHANNELMASK channelmask;

		public IntPtr initialsoundgroup;

		public uint initialseekposition;

		public TIMEUNIT initialseekpostype;

		public int ignoresetfilesystem;

		public uint audioqueuepolicy;

		public uint minmidigranularity;

		public int nonblockthreadid;

		public IntPtr fsbguid;

		public SOUND_PCMREADCALLBACK pcmreadcallback
		{
			set
			{
				pcmreadcallback_handle = ((value != null) ? Marshal.GetFunctionPointerForDelegate(value) : IntPtr.Zero);
			}
		}

		public SOUND_PCMSETPOSCALLBACK pcmsetposcallback
		{
			set
			{
				pcmsetposcallback_handle = ((value != null) ? Marshal.GetFunctionPointerForDelegate(value) : IntPtr.Zero);
			}
		}

		public SOUND_NONBLOCKCALLBACK nonblockcallback
		{
			set
			{
				nonblockcallback_handle = ((value != null) ? Marshal.GetFunctionPointerForDelegate(value) : IntPtr.Zero);
			}
		}

		public FILE_OPENCALLBACK fileuseropen
		{
			set
			{
				fileuseropen_handle = ((value != null) ? Marshal.GetFunctionPointerForDelegate(value) : IntPtr.Zero);
			}
		}

		public FILE_CLOSECALLBACK fileuserclose
		{
			set
			{
				fileuserclose_handle = ((value != null) ? Marshal.GetFunctionPointerForDelegate(value) : IntPtr.Zero);
			}
		}

		public FILE_READCALLBACK fileuserread
		{
			set
			{
				fileuserread_handle = ((value != null) ? Marshal.GetFunctionPointerForDelegate(value) : IntPtr.Zero);
			}
		}

		public FILE_SEEKCALLBACK fileuserseek
		{
			set
			{
				fileuserseek_handle = ((value != null) ? Marshal.GetFunctionPointerForDelegate(value) : IntPtr.Zero);
			}
		}

		public FILE_ASYNCREADCALLBACK fileuserasyncread
		{
			set
			{
				fileuserasyncread_handle = ((value != null) ? Marshal.GetFunctionPointerForDelegate(value) : IntPtr.Zero);
			}
		}

		public FILE_ASYNCCANCELCALLBACK fileuserasynccancel
		{
			set
			{
				fileuserasynccancel_handle = ((value != null) ? Marshal.GetFunctionPointerForDelegate(value) : IntPtr.Zero);
			}
		}
	}
}
