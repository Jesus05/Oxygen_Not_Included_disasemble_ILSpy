using Klei;
using KMod;
using Steamworks;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageOptionsScreen : KModalScreen, SteamUGCService.IClient
{
	private class InstalledLanguageData
	{
		private static readonly string FILE_NAME = "strings/mod_installed.dat";

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
			YamlIO.Save(installedLanguageData, FilePath(), null);
		}

		public static void Get(out PublishedFileId_t item, out System.DateTime lastModified)
		{
			if (Exists())
			{
				InstalledLanguageData installedLanguageData = YamlIO.LoadFile<InstalledLanguageData>(FilePath(), null, null);
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

	public const string KPLAYER_PREFS_LANGUAGE_KEY = "InstalledLanguage";

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

	private PublishedFileId_t _currentLanguage = PublishedFileId_t.Invalid;

	private System.DateTime currentLastModified;

	public PublishedFileId_t currentLanguage
	{
		get
		{
			return _currentLanguage;
		}
		private set
		{
			_currentLanguage = value;
			KPlayerPrefs.SetInt("InstalledLanguage", (int)_currentLanguage.m_PublishedFileId);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		dismissButton.onClick += Deactivate;
		LocText reference = dismissButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Title");
		reference.SetText(UI.FRONTEND.OPTIONS_SCREEN.BACK);
		closeButton.onClick += Deactivate;
		workshopButton.onClick += delegate
		{
			OnClickOpenWorkshop();
		};
		uninstallButton.onClick += delegate
		{
			OnClickUninstall();
		};
		uninstallButton.gameObject.SetActive(false);
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
				component2.onClick += delegate
				{
					ConfirmLanguageChoiceDialog((!(preinstalledLanguage != Localization.DEFAULT_LANGUAGE_CODE)) ? string.Empty : preinstalledLanguage, PublishedFileId_t.Invalid);
				};
				buttons.Add(gameObject);
			}
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Global.Instance.modManager.Sanitize(base.gameObject);
		currentLanguage = GetInstalledFileID(out currentLastModified);
		if ((UnityEngine.Object)SteamUGCService.Instance != (UnityEngine.Object)null)
		{
			if (SteamUGCService.Instance.FindMod(currentLanguage) == null)
			{
				currentLanguage = PublishedFileId_t.Invalid;
				InstallLanguageFile(currentLanguage, false);
			}
			SteamUGCService.Instance.AddClient(this);
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		if ((UnityEngine.Object)SteamUGCService.Instance != (UnityEngine.Object)null)
		{
			SteamUGCService.Instance.RemoveClient(this);
		}
	}

	private void ConfirmLanguageChoiceDialog(string[] lines, bool is_template, System.Action install_language)
	{
		Localization.Locale locale = Localization.GetLocale(lines);
		Dictionary<string, string> translated_strings = Localization.ExtractTranslatedStrings(lines, is_template);
		TMP_FontAsset font = Localization.GetFont(locale.FontName);
		ConfirmDialogScreen screen = GetConfirmDialog();
		HashSet<MemberInfo> excluded_members = new HashSet<MemberInfo>(typeof(ConfirmDialogScreen).GetMember("cancelButton", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));
		Localization.SetFont(screen, font, locale.IsRightToLeft, excluded_members);
		Func<LocString, string> func = delegate(LocString loc_string)
		{
			Dictionary<string, string> dictionary = translated_strings;
			StringKey key = loc_string.key;
			string value;
			return (!dictionary.TryGetValue(key.String, out value)) ? ((string)loc_string) : value;
		};
		screen.PopupConfirmDialog(title_text: func(UI.CONFIRMDIALOG.DIALOG_HEADER), text: func(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT), cancel_text: UI.FRONTEND.TRANSLATIONS_SCREEN.CANCEL, on_confirm: delegate
		{
			install_language();
			App.instance.Restart();
		}, on_cancel: delegate
		{
			Localization.SetFont(screen, Localization.FontAsset, Localization.IsRightToLeft, excluded_members);
		}, configurable_text: null, on_configurable_clicked: null, confirm_text: func(UI.FRONTEND.TRANSLATIONS_SCREEN.RESTART), image_sprite: null, activateBlackBackground: true);
	}

	private void ConfirmLanguageChoiceDialog(string selected_preinstalled_translation)
	{
		Localization.SelectedLanguageType selectedLanguageType = Localization.GetSelectedLanguageType();
		if (!string.IsNullOrEmpty(selected_preinstalled_translation))
		{
			string selectedPreinstalledLanguageCode = Localization.GetSelectedPreinstalledLanguageCode();
			if (selectedLanguageType == Localization.SelectedLanguageType.Preinstalled && selectedPreinstalledLanguageCode == selected_preinstalled_translation)
			{
				Deactivate();
			}
			else
			{
				string preinstalledLocalizationFilePath = Localization.GetPreinstalledLocalizationFilePath(selected_preinstalled_translation);
				string[] lines = File.ReadAllLines(preinstalledLocalizationFilePath, Encoding.UTF8);
				ConfirmLanguageChoiceDialog(lines, false, delegate
				{
					Localization.LoadPreinstalledTranslation(selected_preinstalled_translation);
				});
			}
		}
		else if (selectedLanguageType == Localization.SelectedLanguageType.None)
		{
			Deactivate();
		}
		else
		{
			string defaultLocalizationFilePath = Localization.GetDefaultLocalizationFilePath();
			string[] lines2 = File.ReadAllLines(defaultLocalizationFilePath, Encoding.UTF8);
			ConfirmLanguageChoiceDialog(lines2, true, delegate
			{
				Localization.ClearLanguage();
			});
		}
	}

	private void ConfirmLanguageChoiceDialog(string selected_preinstalled_translation, PublishedFileId_t selected_language_pack)
	{
		if (selected_language_pack != PublishedFileId_t.Invalid)
		{
			Localization.SelectedLanguageType selectedLanguageType = Localization.GetSelectedLanguageType();
			if (selectedLanguageType == Localization.SelectedLanguageType.UGC && selected_language_pack == currentLanguage)
			{
				Deactivate();
			}
			else
			{
				System.DateTime lastModified;
				string languageFile = GetLanguageFile(selected_language_pack, out lastModified);
				string[] lines = languageFile.Split('\n');
				ConfirmLanguageChoiceDialog(lines, false, delegate
				{
					SetCurrentLanguage(selected_language_pack);
				});
			}
		}
		else
		{
			ConfirmLanguageChoiceDialog(selected_preinstalled_translation);
		}
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
			foreach (Mod mod in Global.Instance.modManager.mods)
			{
				if ((mod.available_content & Content.Translation) != 0 && mod.status == Mod.Status.Installed)
				{
					GameObject gameObject = Util.KInstantiateUI(languageButtonPrefab, ugcLanguagesContainer, false);
					gameObject.name = mod.title + "_button";
					HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
					PublishedFileId_t file_id = new PublishedFileId_t(ulong.Parse(mod.label.id));
					TMP_FontAsset fontForLangage = GetFontForLangage(file_id);
					LocText reference = component.GetReference<LocText>("Title");
					reference.SetText(string.Format(UI.FRONTEND.TRANSLATIONS_SCREEN.UGC_MOD_TITLE_FORMAT, mod.title));
					reference.font = fontForLangage;
					Texture2D texture2D = SteamUGCService.Instance.FindMod(file_id)?.previewImage;
					if ((UnityEngine.Object)texture2D != (UnityEngine.Object)null)
					{
						Image reference2 = component.GetReference<Image>("Image");
						reference2.sprite = Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2((float)texture2D.width, (float)texture2D.height)), Vector2.one * 0.5f);
					}
					KButton component2 = gameObject.GetComponent<KButton>();
					component2.onClick += delegate
					{
						ConfirmLanguageChoiceDialog(string.Empty, file_id);
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
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, App.instance.Restart, Deactivate, null, null, null, null, null, null, true);
	}

	private void Uninstall()
	{
		ConfirmDialogScreen confirmDialog = GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.ARE_YOU_SURE, delegate
		{
			Localization.ClearLanguage();
			ConfirmDialogScreen confirmDialog2 = GetConfirmDialog();
			confirmDialog2.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, App.instance.Restart, Deactivate, null, null, null, null, null, null, true);
		}, delegate
		{
		}, null, null, null, null, null, null, true);
	}

	private void OnClickUninstall()
	{
		Uninstall();
	}

	private void OnClickOpenWorkshop()
	{
		Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=language");
	}

	public void UpdateMods(IEnumerable<PublishedFileId_t> added, IEnumerable<PublishedFileId_t> updated, IEnumerable<PublishedFileId_t> removed, IEnumerable<SteamUGCService.Mod> loaded_previews)
	{
		PublishedFileId_t publishedFileId_t = (PublishedFileId_t)GetCurrentLanguage();
		if (removed.Contains(publishedFileId_t))
		{
			Debug.Log("Unsubscribe detected for currently installed font [" + publishedFileId_t + "]");
			GetConfirmDialog().PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, confirm_text: UI.FRONTEND.TRANSLATIONS_SCREEN.RESTART, on_confirm: delegate
			{
				Localization.ClearLanguage();
				currentLanguage = PublishedFileId_t.Invalid;
				App.instance.Restart();
			}, on_cancel: null, configurable_text: null, on_configurable_clicked: null, title_text: null, cancel_text: null, image_sprite: null, activateBlackBackground: true);
		}
		if (updated.Contains(publishedFileId_t))
		{
			Debug.Log("Download complete for currently installed font [" + publishedFileId_t + "] updating in background. Changes will happen next restart.");
			UpdateInstalledLanguage(publishedFileId_t);
		}
		RebuildScreen();
	}

	private ulong GetCurrentLanguage()
	{
		return (ulong)KPlayerPrefs.GetInt("InstalledLanguage");
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
		string result = "";
		System.DateTime lastModified;
		string languageFilename = GetLanguageFilename(out installed, out lastModified);
		if (languageFilename != null && File.Exists(languageFilename))
		{
			string[] lines = File.ReadAllLines(languageFilename, Encoding.UTF8);
			Localization.Locale locale = Localization.GetLocale(lines);
			if (locale != null)
			{
				result = locale.Code;
			}
		}
		return result;
	}

	public static string GetInstalledLanguageFilename(ref PublishedFileId_t item)
	{
		System.DateTime lastModified;
		return GetLanguageFilename(out item, out lastModified);
	}

	public static TMP_FontAsset GetFontForLangage(PublishedFileId_t item)
	{
		System.DateTime lastModified;
		string languageFile = GetLanguageFile(item, out lastModified);
		if (languageFile != null && languageFile.Length > 0)
		{
			string[] lines = languageFile.Split('\n');
			string fontForLocalisation = GetFontForLocalisation(lines);
			return Localization.GetFont(fontForLocalisation);
		}
		return null;
	}

	public static void LoadTranslation(ref PublishedFileId_t item)
	{
		string installedLanguageFilename = GetInstalledLanguageFilename(ref item);
		Localization.LoadLocalTranslationFile(Localization.SelectedLanguageType.UGC, installedLanguageFilename);
	}

	private void UpdateInstalledLanguage(PublishedFileId_t item)
	{
		string languageFile = GetLanguageFile(item, out currentLastModified);
		if (languageFile != null && languageFile.Length > 0)
		{
			InstalledLanguageData.Set(item, currentLastModified);
			File.WriteAllText(Localization.GetModLocalizationFilePath(), languageFile);
		}
		else
		{
			Debug.LogWarning("Loc file was empty.. [" + item + "]  [" + currentLastModified + "]");
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

	private static string GetLanguageFilename(out PublishedFileId_t item, out System.DateTime lastModified)
	{
		InstalledLanguageData.Get(out item, out lastModified);
		if (item != PublishedFileId_t.Invalid)
		{
			string modLocalizationFilePath = Localization.GetModLocalizationFilePath();
			if (File.Exists(modLocalizationFilePath))
			{
				return modLocalizationFilePath;
			}
			Debug.LogWarning("GetLanguagFile [" + modLocalizationFilePath + "] missing for [" + item + "]");
		}
		return null;
	}

	private static string GetLanguageFile(PublishedFileId_t item, out System.DateTime lastModified)
	{
		lastModified = System.DateTime.MinValue;
		if (!((UnityEngine.Object)Global.Instance == (UnityEngine.Object)null) && Global.Instance.modManager != null)
		{
			string language_id = item.ToString();
			Mod mod = Global.Instance.modManager.mods.Find((Mod candidate) => candidate.label.id == language_id);
			if (!string.IsNullOrEmpty(mod.label.id))
			{
				lastModified = mod.label.time_stamp;
				string text = Path.Combine(Application.streamingAssetsPath, "strings.po");
				byte[] array = mod.file_source.GetFileSystem().ReadBytes(text);
				if (array != null)
				{
					return FileSystem.ConvertToText(array);
				}
				Debug.LogFormat("Failed to load language file from local mod installation...couldn't find {0}", text);
				return GetLanguageFileFromSteam(item, out lastModified);
			}
			Debug.LogFormat("Failed to load language file from local mod installation...mod not found.");
			return GetLanguageFileFromSteam(item, out lastModified);
		}
		Debug.LogFormat("Failed to load language file from local mod installation...too early in initialization flow.");
		return GetLanguageFileFromSteam(item, out lastModified);
	}

	private static string GetLanguageFileFromSteam(PublishedFileId_t item, out System.DateTime lastModified)
	{
		lastModified = System.DateTime.MinValue;
		if (!(item == PublishedFileId_t.Invalid))
		{
			SteamUGCService.Mod mod = SteamUGCService.Instance.FindMod(item);
			if (mod != null)
			{
				byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(item, poFile, out lastModified, false);
				if (bytesFromZip != null && bytesFromZip.Length != 0)
				{
					return Encoding.UTF8.GetString(bytesFromZip);
				}
				Debug.LogWarning("Failed to read from Steam mod installation");
				return null;
			}
			Debug.LogWarning("Mod is not in published list");
			return null;
		}
		Debug.LogWarning("Cant get INVALID file id from Steam");
		return null;
	}

	private static PublishedFileId_t GetInstalledFileID(out System.DateTime lastModified)
	{
		InstalledLanguageData.Get(out PublishedFileId_t item, out lastModified);
		if (item == PublishedFileId_t.Invalid)
		{
			PublishedFileId_t invalid = PublishedFileId_t.Invalid;
			item = new PublishedFileId_t((uint)KPlayerPrefs.GetInt("InstalledLanguage", (int)invalid.m_PublishedFileId));
		}
		if (item != PublishedFileId_t.Invalid && (UnityEngine.Object)SteamUGCService.Instance != (UnityEngine.Object)null && !SteamUGCService.Instance.IsSubscribed(item))
		{
			Debug.LogWarning("It doesn't look like we are subscribed..." + item);
			item = PublishedFileId_t.Invalid;
			return item;
		}
		return item;
	}
}
