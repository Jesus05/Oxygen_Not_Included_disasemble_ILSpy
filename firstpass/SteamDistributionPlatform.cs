using Steamworks;
using System;
using UnityEngine;

internal class SteamDistributionPlatform : MonoBehaviour, DistributionPlatform.Implementation
{
	public class SteamUserId : DistributionPlatform.UserId
	{
		private CSteamID mSteamId;

		public SteamUserId(CSteamID id)
		{
			mSteamId = id;
		}

		public override string ToString()
		{
			return mSteamId.ToString();
		}

		public override ulong ToInt64()
		{
			return mSteamId.m_SteamID;
		}
	}

	public class SteamUser : DistributionPlatform.User
	{
		private SteamUserId mId;

		private string mName;

		public override DistributionPlatform.UserId Id => mId;

		public override string Name => mName;

		public SteamUser(CSteamID id, string name)
		{
			mId = new SteamUserId(id);
			mName = name;
		}
	}

	private SteamUser mLocalUser;

	public bool Initialized => SteamManager.Initialized;

	public string Name => "Steam";

	public string Platform => "Steam";

	public string AccountLoginEndpoint => "/login/LoginViaSteam";

	public string MetricsClientKey => "2Ehpf6QcWdCXV8eqbbiJBkrqD6xc8waX";

	public string MetricsUserIDField => "SteamUserID";

	public DistributionPlatform.User LocalUser
	{
		get
		{
			if (mLocalUser == null)
			{
				InitializeLocalUser();
			}
			return mLocalUser;
		}
	}

	public string ApplyWordFilter(string text)
	{
		return text;
	}

	public void GetAuthTicket(DistributionPlatform.AuthTicketHandler handler)
	{
		uint pcbTicket = 0u;
		byte[] array = new byte[2048];
		Steamworks.SteamUser.GetAuthSessionTicket(array, array.Length, out pcbTicket);
		byte[] array2 = new byte[pcbTicket];
		if (0 < pcbTicket)
		{
			Array.Copy(array, array2, pcbTicket);
		}
		handler(array2);
	}

	private void InitializeLocalUser()
	{
		if (SteamManager.Initialized)
		{
			CSteamID steamID = Steamworks.SteamUser.GetSteamID();
			string personaName = SteamFriends.GetPersonaName();
			mLocalUser = new SteamUser(steamID, personaName);
		}
	}
}
