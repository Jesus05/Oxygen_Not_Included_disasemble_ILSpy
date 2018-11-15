using Steamworks;
using System;
using System.Text;
using UnityEngine;

[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	public const uint STEAM_APPLICATION_ID = 457140u;

	private static SteamManager s_instance;

	private bool m_bInitialized;

	private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

	private static SteamManager Instance
	{
		get
		{
			if ((UnityEngine.Object)s_instance == (UnityEngine.Object)null)
			{
				return new GameObject("SteamManager").AddComponent<SteamManager>();
			}
			return s_instance;
		}
	}

	public static bool Initialized => Instance.m_bInitialized;

	private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText, null);
	}

	private void Awake()
	{
		if ((UnityEngine.Object)s_instance != (UnityEngine.Object)null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			s_instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (!Packsize.Test())
			{
				Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
			}
			if (!DllCheck.Test())
			{
				Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
			}
			try
			{
				if (SteamAPI.RestartAppIfNecessary(new AppId_t(457140u)))
				{
					Application.Quit();
					return;
				}
			}
			catch (DllNotFoundException arg)
			{
				Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + arg, this);
				Application.Quit();
				return;
			}
			m_bInitialized = SteamAPI.Init();
			if (!m_bInitialized)
			{
				return;
			}
		}
	}

	private void OnEnable()
	{
		if ((UnityEngine.Object)s_instance == (UnityEngine.Object)null)
		{
			s_instance = this;
		}
		if (m_bInitialized && m_SteamAPIWarningMessageHook == null)
		{
			m_SteamAPIWarningMessageHook = SteamAPIDebugTextHook;
			SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
		}
	}

	private void OnDestroy()
	{
		if (!((UnityEngine.Object)s_instance != (UnityEngine.Object)this))
		{
			s_instance = null;
			if (m_bInitialized)
			{
				SteamAPI.Shutdown();
			}
		}
	}

	private void Update()
	{
		if (m_bInitialized)
		{
			SteamAPI.RunCallbacks();
		}
	}
}
