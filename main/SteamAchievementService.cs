using Steamworks;
using System.Diagnostics;
using UnityEngine;

public class SteamAchievementService : MonoBehaviour
{
	private Callback<UserStatsReceived_t> cbUserStatsReceived;

	private Callback<UserStatsStored_t> cbUserStatsStored;

	private Callback<UserAchievementStored_t> cbUserAchievementStored;

	private bool setupComplete;

	private static SteamAchievementService instance;

	public static SteamAchievementService Instance => instance;

	public static void Initialize()
	{
		if ((Object)instance == (Object)null)
		{
			Debug.Log("Initialising Achievement Service");
			GameObject gameObject = GameObject.Find("/SteamManager");
			instance = gameObject.GetComponent<SteamAchievementService>();
			if ((Object)instance == (Object)null)
			{
				instance = gameObject.AddComponent<SteamAchievementService>();
			}
		}
	}

	public void Awake()
	{
		setupComplete = false;
		Debug.Assert((Object)instance == (Object)null);
		instance = this;
	}

	private void OnDestroy()
	{
		Debug.Assert((Object)instance == (Object)this);
		instance = null;
	}

	private void Update()
	{
		if (SteamManager.Initialized && !((Object)Game.Instance != (Object)null) && !setupComplete && DistributionPlatform.Initialized)
		{
			Setup();
		}
	}

	private void Setup()
	{
		cbUserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		cbUserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
		cbUserAchievementStored = Callback<UserAchievementStored_t>.Create(OnUserAchievementStored);
		setupComplete = true;
		Debug.Log("Achievement Service setup complete");
		RefreshStats();
	}

	private void RefreshStats()
	{
		bool flag = SteamUserStats.RequestCurrentStats();
		Debug.LogFormat("RequestCurrentStats {0}", flag);
	}

	private void OnUserStatsReceived(UserStatsReceived_t data)
	{
		if (data.m_eResult != EResult.k_EResultOK)
		{
			DebugUtil.LogWarningArgs("OnUserStatsReceived", data.m_eResult, data.m_steamIDUser);
		}
		else
		{
			DebugUtil.LogArgs("OnUserStatsReceived", data.m_eResult, data.m_steamIDUser);
			string[] aCHIEVEMENT_IDS = Achievements.ACHIEVEMENT_IDS;
			foreach (string text in aCHIEVEMENT_IDS)
			{
				bool pbAchieved;
				bool userAchievement = SteamUserStats.GetUserAchievement(data.m_steamIDUser, text, out pbAchieved);
				Debug.LogFormat("{0} {1} {2}", text, userAchievement, (!pbAchieved) ? "locked" : "ACHIEVED");
				if (userAchievement)
				{
					string achievementDisplayAttribute = SteamUserStats.GetAchievementDisplayAttribute(text, "name");
					string achievementDisplayAttribute2 = SteamUserStats.GetAchievementDisplayAttribute(text, "desc");
					bool flag = SteamUserStats.GetAchievementDisplayAttribute(text, "hidden") == "1";
					Debug.LogFormat("   {0}{1}: {2}", (!flag) ? "" : "[hidden] ", achievementDisplayAttribute, achievementDisplayAttribute2);
				}
			}
		}
	}

	private void OnUserStatsStored(UserStatsStored_t data)
	{
		if (data.m_eResult != EResult.k_EResultOK)
		{
			DebugUtil.LogWarningArgs("OnUserStatsStored", data.m_eResult);
		}
		else
		{
			DebugUtil.LogArgs("OnUserStatsStored", data.m_eResult);
		}
	}

	private void OnUserAchievementStored(UserAchievementStored_t data)
	{
		Debug.LogFormat("OnUserAchievementStored {0}/{1} - {2}", data.m_nCurProgress, data.m_nMaxProgress, data.m_rgchAchievementName);
	}

	public void Unlock(string achievement_id)
	{
		bool flag = SteamUserStats.SetAchievement(achievement_id);
		Debug.LogFormat("SetAchievement {0} {1}", achievement_id, flag);
		bool flag2 = SteamUserStats.StoreStats();
		Debug.LogFormat("StoreStats {0}", flag2);
	}

	[Conditional("UNITY_EDITOR")]
	[ContextMenu("Reset All Achievements")]
	private void ResetAllAchievements()
	{
		bool flag = SteamUserStats.ResetAllStats(true);
		Debug.LogFormat("ResetAllStats {0}", flag);
		if (flag)
		{
			RefreshStats();
		}
	}

	[Conditional("UNITY_EDITOR")]
	[ContextMenu("Unlock Test Achievement")]
	private void UnlockTestAchievement()
	{
		if (Achievements.ACHIEVEMENT_IDS.Length > 0)
		{
			string achievement_id = Achievements.ACHIEVEMENT_IDS[0];
			Unlock(achievement_id);
		}
	}
}
