using UnityEngine;

public class DistributionPlatform : MonoBehaviour
{
	public interface Implementation
	{
		bool Initialized
		{
			get;
		}

		string Name
		{
			get;
		}

		string Platform
		{
			get;
		}

		string AccountLoginEndpoint
		{
			get;
		}

		string MetricsClientKey
		{
			get;
		}

		string MetricsUserIDField
		{
			get;
		}

		User LocalUser
		{
			get;
		}

		string ApplyWordFilter(string text);

		void GetAuthTicket(AuthTicketHandler callback);
	}

	public delegate void AuthTicketHandler(byte[] ticket);

	public abstract class UserId
	{
		public abstract ulong ToInt64();
	}

	public abstract class User
	{
		public abstract UserId Id
		{
			get;
		}

		public abstract string Name
		{
			get;
		}
	}

	private static Implementation sImpl;

	public static bool Initialized => Impl.Initialized;

	public static Implementation Inst => Impl;

	private static Implementation Impl => sImpl;

	public static void Initialize()
	{
		if (sImpl == null)
		{
			sImpl = new GameObject("DistributionPlatform").AddComponent<SteamDistributionPlatform>();
			if (!SteamManager.Initialized)
			{
				Debug.LogError("Steam not initialized in time.", null);
			}
			Output.Log("Distribution platform: " + sImpl.Platform);
		}
	}
}
