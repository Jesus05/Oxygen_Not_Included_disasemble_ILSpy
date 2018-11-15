using System;
using System.Runtime.InteropServices;
using System.Text;

public static class RailAPI
{
	public enum Platform
	{
		TGP = 1,
		QQGame
	}

	public enum Event
	{
		AuthTicketAcquired = 13001
	}

	[Serializable]
	public struct AuthTicketResponse
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
		public string ticket;
	}

	public delegate void EventHandler(Event eventId, IntPtr data);

	[DllImport("RailAPI")]
	public static extern bool RestartAppIfNecessary(ulong gameId);

	[DllImport("RailAPI")]
	public static extern bool Initialize();

	[DllImport("RailAPI")]
	public static extern void Shutdown();

	[DllImport("RailAPI")]
	public static extern void FireEvents();

	[DllImport("RailAPI")]
	public static extern void RegisterEventHandler(Event eventId, EventHandler handler);

	[DllImport("RailAPI")]
	public static extern void UnregisterEventHandler(Event eventId, EventHandler handler);

	[DllImport("RailAPI")]
	public static extern void ApplyWordFilter(string strIn, StringBuilder strOut, ref int strOutLen);

	[DllImport("RailAPI")]
	public static extern void GetRailPlatformId(ref int idOut);

	[DllImport("RailAPI")]
	public static extern void GetLocalUserName(StringBuilder strOut, ref int strOutLen);

	[DllImport("RailAPI")]
	public static extern void GetLocalUserId(ref ulong idOut);

	[DllImport("RailAPI")]
	public static extern void RequestAuthTicket();
}
