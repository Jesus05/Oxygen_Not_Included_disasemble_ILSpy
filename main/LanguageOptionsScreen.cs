using Klei;
using Steamworks;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageOptionsScreen : KModalScreen, SteamUGCService.IUGCEventHandler
{
	private class InstalledLanguageData : YamlIO<InstalledLanguageData>
	{
		private static readonly string FILE_NAME = "Mods/mod_installed.dat";

		public ulong PublishedFileId
		{
			get;
			set;
		}

		public long LastModified
		{
			get;
			set;
		}

		private static string FilePath()
		{
			return Path.Combine(Application.streamingAssetsPath, FILE_NAME);
		}

		public static void Set(PublishedFileId_t item, System.DateTime lastModified)
		{
			InstalledLanguageData installedLanguageData = new InstalledLanguageData();
			installedLanguageData.PublishedFileId = item.m_PublishedFileId;
			installedLanguageData.LastModified = lastModified.ToFileTimeUtc();
			installedLanguageData.Save(FilePath(), null);
		}

		public static void Get(out PublishedFileId_t item, out System.DateTime lastModified)
		{
			if (Exists())
			{
				InstalledLanguageData installedLanguageData = YamlIO<InstalledLanguageData>.LoadFile(FilePath(), null);
				if (installedLanguageData != null)
				{
					lastModified = System.DateTime.FromFileTimeUtc(installedLanguageData.LastModified);
					item = new PublishedFileId_t(installedLanguageData.PublishedFileId);
					return;
				}
			}
			lastModified = System.DateTime.MinValue;
			item = PublishedFileId_t.Invalid;
		}

		public static bool Exists()
		{
			return File.Exists(FilePath());
		}

		public static void Delete()
		{
			if (Exists())
			{
				File.Delete(FilePath());
			}
		}
	}

	private static readonly string[] poFile = new string[1]
	{
		"strings.po"
	};

	private const string KPLAYER_PREFS_LANGUAGE_KEY = "InstalledLanguage";

	public const string TAG_LANGUAGE = "language";

	public KButton textButton;

	public KButton dismissButton;

	public KButton closeButton;

	public KButton workshopButton;

	public KButton uninstallButton;

	[Space]
	public GameObject languageButtonPrefab;

	public GameObject preinstalledLanguagesTitle;

	public GameObject preinstalledLanguagesContainer;

	public GameObject ugcLanguagesTitle;

	public GameObject ugcLanguagesContainer;

	private List<GameObject> buttons = new List<GameObject>();

	private System.DateTime currentLastModified;

	public PublishedFileId_t currentLanguage
	{
		get;
		private set;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		dismissButton.onClick += delegate
		{
			Deactivate();
		};
		LocText reference = dismissButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Title");
		reference.SetText(UI.FRONTEND.OPTIONS_SCREEN.BACK);
		closeButton.onClick += delegate
		{
			Deactivate();
		};
		workshopButton.onClick += delegate
		{
			OnClickOpenWorkshop();
		};
		uninstallButton.onClick += delegate
		{
			OnClickUninstall();
		};
		RebuildScreen();
	}

	private void RebuildScreen()
	{
		foreach (GameObject button in buttons)
		{
			UnityEngine.Object.Destroy(button);
		}
		buttons.Clear();
		uninstallButton.isInteractable = (KPlayerPrefs.GetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, 0.ToString()) != 0.ToString());
		RebuildPreinstalledButtons();
		RebuildUGCButtons();
	}

	private void RebuildPreinstalledButtons()
	{
		foreach (string preinstalledLanguage in Localization.PreinstalledLanguages)
		{
			if (!(preinstalledLanguage != Localization.DEFAULT_LANGUAGE_CODE) || File.Exists(Localization.GetPreinstalledLocalizationFilePath(preinstalledLanguage)))
			{
				GameObject gameObject = Util.KInstantiateUI(languageButtonPrefab, preinstalledLanguagesContainer, false);
				gameObject.name = preinstalledLanguage + "_button";
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				LocText reference = component.GetReference<LocText>("Title");
				reference.text = Localization.GetPreinstalledLocalizationTitle(preinstalledLanguage);
				reference.enabled = false;
				reference.enabled = true;
				Texture2D preinstalledLocalizationImage = Localization.GetPreinstalledLocalizationImage(preinstalledLanguage);
				if ((UnityEngine.Object)preinstalledLocalizationImage != (UnityEngine.Object)null)
				{
					Image reference2 = component.GetReference<Image>("Image");
					reference2.sprite = Sprite.Create(preinstalledLocalizationImage, new Rect(Vector2.zero, new Vector2((float)preinstalledLocalizationImage.width, (float)preinstalledLocalizationImage.height)), Vector2.one * 0.5f);
				}
				KButton component2 = gameObject.GetComponent<KButton>();
				string _code = preinstalledLanguage;
				component2.onClick += delegate
				{
					ActivatePreinstalledLanguage(_code);
				};
				buttons.Add(gameObject);
			}
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		currentLanguage = GetInstalledFileID(out currentLastModified);
		if ((UnityEngine.Object)SteamUGCService.Instance != (UnityEngine.Object)null)
		{
			if (!SteamUGCService.Instance.IsSubscribedTo(currentLanguage))
			{
				currentLanguage = PublishedFileId_t.Invalid;
				InstallLanguageFile(currentLanguage, false);
			}
			SteamUGCService.Instance.ugcEventHandlers.Add(this);
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		if ((UnityEngine.Object)SteamUGCService.Instance != (UnityEngine.Object)null)
		{
			SteamUGCService.Instance.ugcEventHandlers.Remove(this);
		}
	}

	private void ActivatePreinstalledLanguage(string code)
	{
		Localization.LoadPreinstalledTranslation(code);
		ConfirmDialogScreen confirmDialog = GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
		{
			Application.Quit();
		}, delegate
		{
			App.LoadScene("frontend");
		}, null, null, null, null, null, null);
	}

	private ConfirmDialogScreen GetConfirmDialog()
	{
		GameObject gameObject = KScreenManager.AddChild(base.transform.parent.gameObject, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject);
		KScreen component = gameObject.GetComponent<KScreen>();
		component.Activate();
		return component.GetComponent<ConfirmDialogScreen>();
	}

	private void RebuildUGCButtons()
	{
		if (!((UnityEngine.Object)SteamUGCService.Instance == (UnityEngine.Object)null))
		{
			List<SteamUGCService.Subscribed> subs = SteamUGCService.Instance.GetSubscribed("language");
			if (subs.Count != 0)
			{
				for (int i = 0; i < subs.Count; i++)
				{
					GameObject gameObject = Util.KInstantiateUI(languageButtonPrefab, ugcLanguagesContainer, false);
					gameObject.name = subs[i].title + "_button";
					HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
					TMP_FontAsset fontForLangage = GetFontForLangage(subs[i].fileId);
					LocText reference = component.GetReference<LocText>("Title");
					reference.SetText(string.Format(UI.FRONTEND.TRANSLATIONS_SCREEN.UGC_MOD_TITLE_FORMAT, subs[i].title));
					reference.font = fontForLangage;
					Texture2D previewImage = SteamUGCService.Instance.GetPreviewImage(subs[i].fileId);
					if ((UnityEngine.Object)previewImage != (UnityEngine.Object)null)
					{
						Image reference2 = component.GetReference<Image>("Image");
						reference2.sprite = Sprite.Create(previewImage, new Rect(Vector2.zero, new Vector2((float)previewImage.width, (float)previewImage.height)), Vector2.one * 0.5f);
					}
					KButton component2 = gameObject.GetComponent<KButton>();
					int index = i;
					component2.onClick += delegate
					{
						PublishedFileId_t fileId = subs[index].fileId;
						SetCurrentLanguage(fileId);
					};
					buttons.Add(gameObject);
				}
			}
		}
	}

	private void InstallLanguage(PublishedFileId_t item)
	{
		SetCurrentLanguage(item);
		ConfirmDialogScreen confirmDialog = GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
		{
			Application.Quit();
		}, delegate
		{
			App.LoadScene("frontend");
		}, null, null, null, null, null, null);
	}

	private void Uninstall()
	{
		ConfirmDialogScreen confirmDialog = GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.ARE_YOU_SURE, delegate
		{
			Localization.ClearLanguage();
			ConfirmDialogScreen confirmDialog2 = GetConfirmDialog();
			confirmDialog2.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
			{
				Application.Quit();
			}, delegate
			{
				App.LoadScene("frontend");
			}, null, null, null, null, null, null);
		}, delegate
		{
		}, null, null, null, null, null, null);
	}

	private void OnClickUninstall()
	{
		Uninstall();
	}

	private void OnClickOpenWorkshop()
	{
		Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=language");
	}

	public void OnUGCItemInstalled(ItemInstalled_t pCallback)
	{
	}

	private ulong GetCurrentLanguage()
	{
		return (ulong)KPlayerPrefs.GetInt("InstalledLanguage");
	}

	public void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
	{
		ulong currentLanguage = GetCurrentLanguage();
		if (currentLanguage == pCallback.m_nPublishedFileId.m_PublishedFileId)
		{
			Debug.Log("Update detected for currently installed font [" + pCallback.m_nPublishedFileId + "]", null);
			SteamUGCService.DoDownloadItem(pCallback.m_nPublishedFileId);
		}
	}

	public void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
	{
		ulong currentLanguage = GetCurrentLanguage();
		if (pCallback.m_nPublishedFileId.m_PublishedFileId == currentLanguage)
		{
			Debug.Log("Unsubscribe detected for currently installed font [" + pCallback.m_nPublishedFileId + "]", null);
			CleanUpCurrentModLanguage();
		}
	}

	public void OnUGCRefresh()
	{
		RebuildScreen();
	}

	public static void CleanUpCurrentModLanguage()
	{
		PublishedFileId_t invalid = PublishedFileId_t.Invalid;
		KPlayerPrefs.SetInt("InstalledLanguage", (int)invalid.m_PublishedFileId);
		InstalledLanguageData.Delete();
		string modLocalizationFilePath = Localization.GetModLocalizationFilePath();
		if (File.Exists(modLocalizationFilePath))
		{
			File.Delete(modLocalizationFilePath);
		}
	}

	public void OnUGCItemDownloaded(DownloadItemResult_t pCallback)
	{
		ulong currentLanguage = GetCurrentLanguage();
		if (currentLanguage == pCallback.m_nPublishedFileId.m_PublishedFileId)
		{
			Debug.Log("Download complete for currently installed font [" + pCallback.m_nPublishedFileId + "] updating in background. Changes will happen next restart.", null);
			UpdateInstalledLanguage(pCallback.m_nPublishedFileId);
		}
	}

	public void SetCurrentLanguage(PublishedFileId_t item)
	{
		InstallLanguageFile(item, false);
	}

	public static bool HasInstalledLanguage()
	{
		System.DateTime lastModified;
		return GetInstalledFileID(out lastModified) != PublishedFileId_t.Invalid;
	}

	public static string GetInstalledLanguageCode(out PublishedFileId_t installed)
	{
		string result = string.Empty;
		System.DateTime lastModified;
		string languageFile = GetLanguageFile(out installed, out lastModified);
		if (languageFile != null && File.Exists(languageFile))
		{
			string[] lines = File.ReadAllLines(languageFile, Encoding.UTF8);
			Localization.Locale locale = Localization.GetLocale(lines);
			if (locale != null)
			{
				result = locale.Code;
			}
		}
		return result;
	}

	public static string GetInstalledLanguageFile(ref PublishedFileId_t item)
	{
		System.DateTime lastModified;
		return GetLanguageFile(out item, out lastModified);
	}

	public static TMP_FontAsset GetFontForLangage(PublishedFileId_t item)
	{
		System.DateTime lastModified;
		string languageFileFromSteam = GetLanguageFileFromSteam(item, out lastModified);
		if (languageFileFromSteam != null && languageFileFromSteam.Length > 0)
		{
			string[] lines = languageFileFromSteam.Split('\n');
			string fontForLocalisation = GetFontForLocalisation(lines);
			return Localization.GetFont(fontForLocalisation);
		}
		return null;
	}

	public static void LoadTranslation(ref PublishedFileId_t item)
	{
		string installedLanguageFile = GetInstalledLanguageFile(ref item);
		Localization.LoadLocalTranslationFile(Localization.SelectedLanguageType.UGC, installedLanguageFile);
	}

	private void UpdateInstalledLanguage(PublishedFileId_t item)
	{
		string languageFileFromSteam = GetLanguageFileFromSteam(item, out currentLastModified);
		if (languageFileFromSteam != null && languageFileFromSteam.Length > 0)
		{
			InstalledLanguageData.Set(item, currentLastModified);
			File.WriteAllText(Localization.GetModLocalizationFilePath(), languageFileFromSteam);
		}
		else
		{
			Debug.LogWarning("Loc file was empty.. [" + item + "]  [" + currentLastModified + "]", null);
		}
	}

	private void InstallLanguageFile(PublishedFileId_t item, bool fromDownload = false)
	{
		CleanUpCurrentModLanguage();
		if (item != PublishedFileId_t.Invalid)
		{
			UpdateInstalledLanguage(item);
		}
		PublishedFileId_t item2 = PublishedFileId_t.Invalid;
		LoadTranslation(ref item2);
		currentLanguage = item;
	}

	private static string GetFontForLocalisation(string[] lines)
	{
		return Localization.GetLocale(lines).FontName;
	}

	private static string GetLanguageFile(out PublishedFileId_t item, out System.DateTime lastModified)
	{
		InstalledLanguageData.Get(out item, out lastModified);
		if (item != PublishedFileId_t.Invalid)
		{
			string modLocalizationFilePath = Localization.GetModLocalizationFilePath();
			if (File.Exists(modLocalizationFilePath))
			{
				return modLocalizationFilePath;
			}
			Debug.LogWarning("GetLanguagFile [" + modLocalizationFilePath + "] missing for [" + item + "]", null);
		}
		return null;
	}

	private static string GetLanguageFileFromSteam(PublishedFileId_t item, out System.DateTime lastModified)
	{
		lastModified = System.DateTime.MinValue;
		if (item == PublishedFileId_t.Invalid)
		{
			Debug.LogWarning("Cant get INVALID file id from Steam", null);
			return null;
		}
		EItemState itemState = (EItemState)SteamUGC.GetItemState(item);
		if ((itemState & EItemState.k_EItemStateInstalled) == EItemState.k_EItemStateInstalled)
		{
			byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(item, poFile, out lastModified, false);
			if (bytesFromZip != null && bytesFromZip.Length > 0)
			{
				return Encoding.UTF8.GetString(bytesFromZip);
			}
			Debug.LogWarning("Empty bytes from Zip file, trying redownload", null);
			SteamUGCService.DoDownloadItem(item);
		}
		else
		{
			Debug.LogWarning("Steam says item not installed [" + itemState + "]", null);
		}
		return null;
	}

	private static PublishedFileId_t GetInstalledFileID(out System.DateTime lastModified)
	{
		InstalledLanguageData.Get(out PublishedFileId_t item, out lastModified);
		if (item == PublishedFileId_t.Invalid)
		{
			PublishedFileId_t invalid = PublishedFileId_t.Invalid;
			item = new PublishedFileId_t((uint)KPlayerPrefs.GetInt("InstalledLanguage", (int)invalid.m_PublishedFileId));
			if (item != PublishedFileId_t.Invalid)
			{
				if ((UnityEngine.Object)SteamUGCService.Instance != (UnityEngine.Object)null)
				{
					if (!SteamUGCService.Instance.IsSubscribedTo(item))
					{
						Debug.LogWarning("It doesn't look like we are subscribed..." + item, null);
					}
				}
				else
				{
					Debug.LogWarning("Cant check yet..." + item, null);
				}
			}
		}
		return item;
	}
}
