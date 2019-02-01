using Ionic.Zip;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SteamUGCService : MonoBehaviour
{
	public interface IUGCEventHandler
	{
		void OnUGCItemInstalled(ItemInstalled_t pCallback);

		void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback);

		void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback);

		void OnUGCItemDownloaded(DownloadItemResult_t pCallback);

		void OnUGCRefresh();
	}

	public class Subscribed
	{
		public string title
		{
			get;
			private set;
		}

		public string description
		{
			get;
			private set;
		}

		public PublishedFileId_t fileId
		{
			get;
			private set;
		}

		public ulong lastUpdateTime
		{
			get;
			private set;
		}

		public Subscribed(SteamUGCDetails_t item)
		{
			title = item.m_rgchTitle;
			description = item.m_rgchDescription;
			fileId = item.m_nPublishedFileId;
			lastUpdateTime = item.m_rtimeUpdated;
		}
	}

	private UGCQueryHandle_t m_UGCQueryHandle;

	protected Callback<RemoteStoragePublishedFileUpdated_t> m_ItemUpdated;

	protected Callback<RemoteStoragePublishedFileUnsubscribed_t> m_ItemUnsubscribed;

	protected Callback<ItemInstalled_t> m_ItemInstalled;

	protected Callback<DownloadItemResult_t> m_DownloadItemResult;

	protected Dictionary<UGCHandle_t, CallResult<RemoteStorageDownloadUGCResult_t>> m_DownloadPreviewResult = new Dictionary<UGCHandle_t, CallResult<RemoteStorageDownloadUGCResult_t>>();

	private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryDetailsCompletedCallResult;

	private bool listPending = false;

	private List<PublishedFileId_t> subscribed = null;

	private SteamUGCDetails_t[] details = null;

	private Dictionary<PublishedFileId_t, Texture2D> previewImages = new Dictionary<PublishedFileId_t, Texture2D>();

	private Dictionary<UGCHandle_t, PublishedFileId_t> previews = new Dictionary<UGCHandle_t, PublishedFileId_t>();

	private bool doClearList = false;

	private static PublishedFileId_t waitingForDownload = PublishedFileId_t.Invalid;

	private bool setupComplete = false;

	private static Dictionary<PublishedFileId_t, int> getBytesRetryCount = new Dictionary<PublishedFileId_t, int>();

	private static readonly string[] previewFileNames = new string[4]
	{
		"preview.png",
		"preview.png",
		".png",
		".jpg"
	};

	public List<IUGCEventHandler> ugcEventHandlers = new List<IUGCEventHandler>();

	private static SteamUGCService instance;

	private static readonly int MAX_FILE_RETRY_COUNT = 3;

	public uint numSubscriptions
	{
		get;
		private set;
	}

	public static SteamUGCService Instance => instance;

	public static void Initialize()
	{
		if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
		{
			Debug.Log("Initialising UGC Service", null);
			GameObject gameObject = GameObject.Find("/SteamManager");
			instance = gameObject.GetComponent<SteamUGCService>();
			if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
			{
				instance = gameObject.AddComponent<SteamUGCService>();
			}
		}
	}

	public void DoDownload()
	{
	}

	public List<Subscribed> GetSubscribed(string required_tag)
	{
		List<Subscribed> list = new List<Subscribed>();
		if (details != null && subscribed != null)
		{
			for (int i = 0; i < details.Length; i++)
			{
				SteamUGCDetails_t item = details[i];
				string[] array = item.m_rgchTags.Split(',');
				if (Array.IndexOf(array, required_tag) >= 0)
				{
					PublishedFileId_t nPublishedFileId = item.m_nPublishedFileId;
					if (subscribed.Contains(nPublishedFileId))
					{
						list.Add(new Subscribed(item));
					}
				}
			}
		}
		return list;
	}

	public Subscribed GetSubscribed(PublishedFileId_t id)
	{
		Subscribed result = null;
		if (subscribed != null)
		{
			for (int i = 0; i < details.Length; i++)
			{
				SteamUGCDetails_t item = details[i];
				if (item.m_nPublishedFileId == id)
				{
					result = new Subscribed(item);
					break;
				}
			}
		}
		return result;
	}

	public bool IsSubscribedTo(PublishedFileId_t id)
	{
		bool result = false;
		if (subscribed != null)
		{
			foreach (PublishedFileId_t item in subscribed)
			{
				PublishedFileId_t current = item;
				if (current.m_PublishedFileId == id.m_PublishedFileId)
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	public Texture2D GetPreviewImage(PublishedFileId_t item)
	{
		if (!previewImages.ContainsKey(item))
		{
			return null;
		}
		return previewImages[item];
	}

	public void Awake()
	{
		setupComplete = false;
		instance = this;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void Update()
	{
		if (SteamManager.Initialized && !((UnityEngine.Object)Game.Instance != (UnityEngine.Object)null))
		{
			if (!setupComplete && (UnityEngine.Object)Global.Instance != (UnityEngine.Object)null && DistributionPlatform.Initialized)
			{
				Setup();
			}
			if (doClearList)
			{
				doClearList = false;
				ClearLists();
			}
			GetSubscribedDetails();
			numSubscriptions = SteamUGC.GetNumSubscribedItems();
			UpdateSubscription(numSubscriptions);
			if (details != null)
			{
				for (int i = 0; i < details.Length; i++)
				{
					PublishedFileId_t nPublishedFileId = details[i].m_nPublishedFileId;
					if (subscribed != null && subscribed.Contains(nPublishedFileId))
					{
						EItemState itemState = (EItemState)SteamUGC.GetItemState(nPublishedFileId);
						bool flag = ((itemState & EItemState.k_EItemStateInstalled) != EItemState.k_EItemStateInstalled) | ((itemState & EItemState.k_EItemStateNeedsUpdate) == EItemState.k_EItemStateNeedsUpdate);
						bool flag2 = ((itemState & EItemState.k_EItemStateDownloading) == EItemState.k_EItemStateDownloading) | ((itemState & EItemState.k_EItemStateDownloadPending) == EItemState.k_EItemStateDownloadPending);
						if (flag && !flag2)
						{
							SteamUGC.DownloadItem(nPublishedFileId, false);
						}
						else if ((itemState & EItemState.k_EItemStateInstalled) == EItemState.k_EItemStateInstalled && !previewImages.ContainsKey(nPublishedFileId))
						{
							System.DateTime lastModified;
							byte[] bytesFromZip = GetBytesFromZip(nPublishedFileId, previewFileNames, out lastModified, false);
							if (bytesFromZip != null)
							{
								Texture2D texture2D = new Texture2D(2, 2);
								texture2D.LoadImage(bytesFromZip);
								previewImages.Add(nPublishedFileId, texture2D);
								doClearList = true;
							}
							if (getBytesRetryCount.ContainsKey(nPublishedFileId) && getBytesRetryCount[nPublishedFileId] > 3)
							{
								previewImages.Add(nPublishedFileId, null);
							}
						}
					}
				}
			}
		}
	}

	private void Setup()
	{
		m_ItemInstalled = Callback<ItemInstalled_t>.Create(OnItemInstalled);
		m_DownloadItemResult = Callback<DownloadItemResult_t>.Create(OnDownloadItemResult);
		m_ItemUnsubscribed = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create(OnItemUnsubscribed);
		m_ItemUpdated = Callback<RemoteStoragePublishedFileUpdated_t>.Create(OnItemUpdated);
		OnSteamUGCQueryDetailsCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryDetailsCompleted);
		doClearList = true;
		setupComplete = true;
		Debug.Log("UGC Service setup complete..", null);
	}

	public static bool DoDownloadItem(PublishedFileId_t item)
	{
		if (!(waitingForDownload == item))
		{
			if (!(waitingForDownload != PublishedFileId_t.Invalid))
			{
				if (!getBytesRetryCount.ContainsKey(item))
				{
					getBytesRetryCount.Add(item, 0);
				}
				if (getBytesRetryCount[item] <= MAX_FILE_RETRY_COUNT)
				{
					if (SteamUGC.DownloadItem(item, true))
					{
						Dictionary<PublishedFileId_t, int> dictionary;
						PublishedFileId_t key;
						(dictionary = getBytesRetryCount)[key = item] = dictionary[key] + 1;
						waitingForDownload = item;
						return true;
					}
					Debug.Log("SteamUGC.DownloadItem returned false for [" + item + "]", null);
					return false;
				}
				Debug.Log("Max retry count reached for [" + item + "]", null);
				return false;
			}
			Debug.Log("We are waiting for [" + waitingForDownload + "] to download, cant download [" + item + "] now", null);
			return false;
		}
		Debug.Log("We are waiting for [" + item + "] to download", null);
		return false;
	}

	private void GetSubscribedDetails()
	{
		if (!listPending)
		{
			uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
			if (numSubscribedItems != 0 && (subscribed == null || numSubscribedItems != subscribed.Count))
			{
				PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
				SteamUGC.GetSubscribedItems(array, (uint)array.Length);
				subscribed = new List<PublishedFileId_t>(array);
				listPending = true;
				m_UGCQueryHandle = SteamUGC.CreateQueryUGCDetailsRequest(array, (uint)array.Length);
				SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(m_UGCQueryHandle);
				OnSteamUGCQueryDetailsCompletedCallResult.Set(hAPICall, null);
			}
		}
	}

	public SteamUGCDetails_t GetDetails(PublishedFileId_t item)
	{
		SteamUGCDetails_t result = default(SteamUGCDetails_t);
		if (details != null)
		{
			SteamUGCDetails_t[] array = details;
			for (int i = 0; i < array.Length; i++)
			{
				SteamUGCDetails_t steamUGCDetails_t = array[i];
				if (steamUGCDetails_t.m_nPublishedFileId == item)
				{
					result = steamUGCDetails_t;
					break;
				}
			}
			return result;
		}
		return result;
	}

	private void ClearLists()
	{
		listPending = false;
		subscribed = null;
		details = null;
	}

	private void UpdateSubscription(uint num)
	{
		if (num != 0 && (subscribed == null || num != subscribed.Count))
		{
			PublishedFileId_t[] array = new PublishedFileId_t[num];
			SteamUGC.GetSubscribedItems(array, (uint)array.Length);
			subscribed = new List<PublishedFileId_t>(array);
		}
	}

	private void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			details = new SteamUGCDetails_t[pCallback.m_unNumResultsReturned];
			for (uint num = 0u; num < pCallback.m_unNumResultsReturned; num++)
			{
				SteamUGC.GetQueryUGCResult(m_UGCQueryHandle, num, out details[num]);
			}
			uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
			UpdateSubscription(numSubscribedItems);
			foreach (IUGCEventHandler ugcEventHandler in ugcEventHandlers)
			{
				ugcEventHandler.OnUGCRefresh();
			}
			listPending = false;
		}
		else
		{
			Debug.Log("[SteamUGCQueryCompleted] - handle: " + pCallback.m_handle + " -- Result: " + pCallback.m_eResult + " -- NUm results: " + pCallback.m_unNumResultsReturned + " --Total Matching: " + pCallback.m_unTotalMatchingResults + " -- cached: " + pCallback.m_bCachedData, null);
		}
		SteamUGC.ReleaseQueryUGCRequest(m_UGCQueryHandle);
	}

	private void OnSteamUGCQueryDetailsCompleted(SteamUGCQueryCompleted_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			details = new SteamUGCDetails_t[pCallback.m_unNumResultsReturned];
			for (uint num = 0u; num < pCallback.m_unNumResultsReturned; num++)
			{
				SteamUGC.GetQueryUGCResult(m_UGCQueryHandle, num, out details[num]);
				if (!previews.ContainsKey(details[num].m_hPreviewFile))
				{
					previews.Add(details[num].m_hPreviewFile, details[num].m_nPublishedFileId);
					AddPreviewToDownloadQueue(details[num].m_hPreviewFile);
				}
			}
			foreach (IUGCEventHandler ugcEventHandler in ugcEventHandlers)
			{
				ugcEventHandler.OnUGCRefresh();
			}
			listPending = false;
		}
		else if (pCallback.m_eResult == EResult.k_EResultBusy)
		{
			Debug.Log("[OnSteamUGCQueryDetailsCompleted] - handle: " + pCallback.m_handle + " -- Result: " + pCallback.m_eResult + " Resending", null);
			listPending = false;
		}
		else
		{
			Debug.Log("[OnSteamUGCQueryDetailsCompleted] - handle: " + pCallback.m_handle + " -- Result: " + pCallback.m_eResult + " -- NUm results: " + pCallback.m_unNumResultsReturned + " --Total Matching: " + pCallback.m_unTotalMatchingResults + " -- cached: " + pCallback.m_bCachedData, null);
		}
		SteamUGC.ReleaseQueryUGCRequest(m_UGCQueryHandle);
	}

	private void AddPreviewToDownloadQueue(UGCHandle_t next)
	{
		CallResult<RemoteStorageDownloadUGCResult_t> callResult = CallResult<RemoteStorageDownloadUGCResult_t>.Create(OnDownloadPreviewResult);
		SteamAPICall_t hAPICall = SteamRemoteStorage.UGCDownload(next, 0u);
		callResult.Set(hAPICall, null);
		m_DownloadPreviewResult.Add(next, callResult);
	}

	private void OnDownloadPreviewResult(RemoteStorageDownloadUGCResult_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult == EResult.k_EResultOK && !previewImages.ContainsKey(previews[pCallback.m_hFile]))
		{
			byte[] array = new byte[pCallback.m_nSizeInBytes];
			SteamRemoteStorage.UGCRead(pCallback.m_hFile, array, array.Length, 0u, EUGCReadAction.k_EUGCRead_ContinueReadingUntilFinished);
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(array);
			previewImages.Add(previews[pCallback.m_hFile], texture2D);
		}
		m_DownloadPreviewResult.Remove(pCallback.m_hFile);
		if (m_DownloadPreviewResult.Count == 0)
		{
			doClearList = true;
		}
	}

	private void OnItemInstalled(ItemInstalled_t pCallback)
	{
		foreach (IUGCEventHandler ugcEventHandler in ugcEventHandlers)
		{
			ugcEventHandler.OnUGCItemInstalled(pCallback);
		}
		doClearList = true;
	}

	private void OnItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
	{
		foreach (IUGCEventHandler ugcEventHandler in ugcEventHandlers)
		{
			ugcEventHandler.OnUGCItemUpdated(pCallback);
		}
		doClearList = true;
	}

	private void OnItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
	{
		foreach (IUGCEventHandler ugcEventHandler in ugcEventHandlers)
		{
			ugcEventHandler.OnUGCItemUnsubscribed(pCallback);
		}
		doClearList = true;
	}

	private void OnDownloadItemResult(DownloadItemResult_t pCallback)
	{
		if (waitingForDownload == pCallback.m_nPublishedFileId)
		{
			Debug.Log("Download complete for waitingForDownload [" + waitingForDownload + "]", null);
			waitingForDownload = PublishedFileId_t.Invalid;
		}
		foreach (IUGCEventHandler ugcEventHandler in ugcEventHandlers)
		{
			ugcEventHandler.OnUGCItemDownloaded(pCallback);
		}
		doClearList = true;
	}

	public static byte[] GetBytesFromZip(PublishedFileId_t item, string[] filesToExtract, out System.DateTime lastModified, bool getFirstMatch = false)
	{
		byte[] result = null;
		lastModified = System.DateTime.MinValue;
		SteamUGC.GetItemInstallInfo(item, out ulong _, out string pchFolder, 1024u, out uint _);
		try
		{
			lastModified = File.GetLastWriteTimeUtc(pchFolder);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZipFile zipFile = ZipFile.Read(pchFolder))
				{
					ZipEntry zipEntry = null;
					foreach (string file in filesToExtract)
					{
						if (file.Length > 4)
						{
							if (zipFile.ContainsEntry(file))
							{
								zipEntry = zipFile[file];
							}
						}
						else
						{
							zipEntry = zipFile.Entries.First((ZipEntry x) => x.FileName.EndsWith(file));
						}
						if (zipEntry != null)
						{
							break;
						}
					}
					zipEntry?.Extract(memoryStream);
				}
				memoryStream.Flush();
				result = memoryStream.ToArray();
			}
		}
		catch (Exception)
		{
		}
		return result;
	}
}
