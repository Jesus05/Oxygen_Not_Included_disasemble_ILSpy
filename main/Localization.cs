using ArabicSupport;
using Klei;
using KMod;
using Steamworks;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public static class Localization
{
	public enum Language
	{
		Chinese,
		Japanese,
		Korean,
		Russian,
		Thai,
		Arabic,
		Hebrew,
		Unspecified
	}

	public enum Direction
	{
		LeftToRight,
		RightToLeft
	}

	public class Locale
	{
		private Language mLanguage;

		private string mCode;

		private string mFontName;

		private Direction mDirection;

		public Language Lang => mLanguage;

		public string Code => mCode;

		public string FontName => mFontName;

		public bool IsRightToLeft => mDirection == Direction.RightToLeft;

		public Locale(Locale other)
		{
			mLanguage = other.mLanguage;
			mDirection = other.mDirection;
			mCode = other.mCode;
			mFontName = other.mFontName;
		}

		public Locale(Language language, Direction direction, string code, string fontName)
		{
			mLanguage = language;
			mDirection = direction;
			mCode = code.ToLower();
			mFontName = fontName;
		}

		public void SetCode(string code)
		{
			mCode = code;
		}

		public bool MatchesCode(string language_code)
		{
			return language_code.ToLower().Contains(mCode);
		}

		public bool MatchesFont(string fontname)
		{
			return fontname.ToLower() == mFontName.ToLower();
		}

		public override string ToString()
		{
			return mCode + ":" + mLanguage + ":" + mDirection + ":" + mFontName;
		}
	}

	private struct Entry
	{
		public string msgctxt;

		public string msgstr;

		public bool IsPopulated => msgctxt != null && msgstr != null && msgstr.Length > 0;
	}

	public enum SelectedLanguageType
	{
		None,
		Preinstalled,
		UGC
	}

	private static TMP_FontAsset sFontAsset = null;

	private static readonly List<Locale> Locales = new List<Locale>
	{
		new Locale(Language.Chinese, Direction.LeftToRight, "zh", "NotoSansCJKsc-Regular"),
		new Locale(Language.Japanese, Direction.LeftToRight, "ja", "NotoSansCJKjp-Regular"),
		new Locale(Language.Korean, Direction.LeftToRight, "ko", "NotoSansCJKkr-Regular"),
		new Locale(Language.Russian, Direction.LeftToRight, "ru", "RobotoCondensed-Regular"),
		new Locale(Language.Thai, Direction.LeftToRight, "th", "NotoSansThai-Regular"),
		new Locale(Language.Arabic, Direction.RightToLeft, "ar", "NotoNaskhArabic-Regular"),
		new Locale(Language.Hebrew, Direction.RightToLeft, "he", "NotoSansHebrew-Regular"),
		new Locale(Language.Unspecified, Direction.LeftToRight, "", "RobotoCondensed-Regular")
	};

	private static Locale sLocale = null;

	private static string currentFontName = null;

	public static string DEFAULT_LANGUAGE_CODE = "en";

	public static readonly List<string> PreinstalledLanguages = new List<string>
	{
		DEFAULT_LANGUAGE_CODE,
		"zh_klei",
		"ko_klei",
		"ru_klei"
	};

	public static string SELECTED_LANGUAGE_TYPE_KEY = "SelectedLanguageType";

	public static string SELECTED_LANGUAGE_CODE_KEY = "SelectedLanguageCode";

	private static Dictionary<string, List<Assembly>> translatable_assemblies = new Dictionary<string, List<Assembly>>();

	public const BindingFlags non_static_data_member_fields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	private const string start_link_token = "<link";

	private const string end_link_token = "</link";

	public static TMP_FontAsset FontAsset => sFontAsset;

	public static bool IsRightToLeft => sLocale != null && sLocale.IsRightToLeft;

	private static IEnumerable<Type> CollectLocStringTreeRoots(string locstrings_namespace, Assembly assembly)
	{
		return from type in assembly.GetTypes()
		where type.IsClass && type.Namespace == locstrings_namespace && !type.IsNested
		select type;
	}

	private static Dictionary<string, object> MakeRuntimeLocStringTree(Type locstring_tree_root)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		FieldInfo[] fields = locstring_tree_root.GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType == typeof(LocString))
			{
				LocString locString = (LocString)fieldInfo.GetValue(null);
				if (locString == null)
				{
					Debug.LogError("Tried to generate LocString for " + fieldInfo.Name + " but it is null so skipping");
				}
				else
				{
					dictionary[fieldInfo.Name] = locString.text;
				}
			}
		}
		Type[] nestedTypes = locstring_tree_root.GetNestedTypes();
		Type[] array2 = nestedTypes;
		foreach (Type type in array2)
		{
			Dictionary<string, object> dictionary2 = MakeRuntimeLocStringTree(type);
			if (dictionary2.Count > 0)
			{
				dictionary[type.Name] = dictionary2;
			}
		}
		return dictionary;
	}

	private static void WriteStringsTemplate(string path, StreamWriter writer, Dictionary<string, object> runtime_locstring_tree)
	{
		List<string> list = new List<string>(runtime_locstring_tree.Keys);
		list.Sort();
		foreach (string item in list)
		{
			string text = path + '.' + item;
			object obj = runtime_locstring_tree[item];
			if (obj.GetType() != typeof(string))
			{
				WriteStringsTemplate(text, writer, obj as Dictionary<string, object>);
			}
			else
			{
				string text2 = obj as string;
				text2 = text2.Replace("\"", "\\\"");
				text2 = text2.Replace("\n", "\\n");
				writer.WriteLine("#. " + text);
				writer.WriteLine("msgctxt \"{0}\"", text);
				writer.WriteLine("msgid \"" + text2 + "\"");
				writer.WriteLine("msgstr \"\"");
				writer.WriteLine("");
			}
		}
	}

	public static void GenerateStringsTemplate(string locstrings_namespace, Assembly assembly, string output_filename, Dictionary<string, object> current_runtime_locstring_forest)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (Type item in CollectLocStringTreeRoots(locstrings_namespace, assembly))
		{
			Dictionary<string, object> dictionary2 = MakeRuntimeLocStringTree(item);
			if (dictionary2.Count > 0)
			{
				dictionary[item.Name] = dictionary2;
			}
		}
		if (current_runtime_locstring_forest != null)
		{
			dictionary.Concat(current_runtime_locstring_forest);
		}
		using (StreamWriter streamWriter = new StreamWriter(output_filename, false, new UTF8Encoding(false)))
		{
			streamWriter.WriteLine("msgid \"\"");
			streamWriter.WriteLine("msgstr \"\"");
			streamWriter.WriteLine("\"Application: Oxygen Not Included\"");
			streamWriter.WriteLine("\"POT Version: 2.0\"");
			streamWriter.WriteLine("");
			WriteStringsTemplate(locstrings_namespace, streamWriter, dictionary);
		}
		DebugUtil.LogArgs("Generated " + output_filename);
	}

	public static void GenerateStringsTemplate(Type locstring_tree_root, string output_folder)
	{
		output_folder = FileSystem.Normalize(output_folder);
		if (FileUtil.CreateDirectory(output_folder, 5))
		{
			GenerateStringsTemplate(locstring_tree_root.Namespace, Assembly.GetAssembly(locstring_tree_root), FileSystem.Normalize(Path.Combine(output_folder, $"{locstring_tree_root.Namespace.ToLower()}_template.pot")), null);
		}
	}

	public static void Initialize(bool dontCheckSteam = false)
	{
		DebugUtil.LogArgs("Localization.Initialize!");
		bool flag = false;
		switch (GetSelectedLanguageType())
		{
		case SelectedLanguageType.Preinstalled:
		{
			string selectedPreinstalledLanguageCode = GetSelectedPreinstalledLanguageCode();
			if (!string.IsNullOrEmpty(selectedPreinstalledLanguageCode))
			{
				DebugUtil.LogArgs("Localization Initialize... Preinstalled localization");
				DebugUtil.LogArgs(" -> ", selectedPreinstalledLanguageCode);
				LoadPreinstalledTranslation(selectedPreinstalledLanguageCode);
			}
			else
			{
				flag = true;
			}
			break;
		}
		case SelectedLanguageType.UGC:
			if (!dontCheckSteam && SteamManager.Initialized && LanguageOptionsScreen.HasInstalledLanguage())
			{
				DebugUtil.LogArgs("Localization Initialize... SteamUGCService");
				PublishedFileId_t item = PublishedFileId_t.Invalid;
				LanguageOptionsScreen.LoadTranslation(ref item);
				if (item != PublishedFileId_t.Invalid)
				{
					DebugUtil.LogArgs(" -> Loaded steamworks file id: ", item.ToString());
				}
				else
				{
					DebugUtil.LogArgs(" -> Failed to load steamworks file id: ", item.ToString());
				}
			}
			else
			{
				flag = true;
			}
			break;
		case SelectedLanguageType.None:
			sFontAsset = GetFont(GetDefaultLocale().FontName);
			break;
		}
		if (flag)
		{
			ClearLanguage();
		}
	}

	public static void VerifyTranslationModSubscription(GameObject context)
	{
		if (GetSelectedLanguageType() == SelectedLanguageType.UGC && SteamManager.Initialized && !LanguageOptionsScreen.HasInstalledLanguage())
		{
			PublishedFileId_t invalid = PublishedFileId_t.Invalid;
			PublishedFileId_t publishedFileId_t = new PublishedFileId_t((uint)KPlayerPrefs.GetInt("InstalledLanguage", (int)invalid.m_PublishedFileId));
			Label label = default(Label);
			label.distribution_platform = Label.DistributionPlatform.Steam;
			label.id = publishedFileId_t.ToString();
			Label rhs = label;
			string arg = UI.FRONTEND.TRANSLATIONS_SCREEN.UNKNOWN;
			foreach (Mod mod in Global.Instance.modManager.mods)
			{
				if (mod.label.Match(rhs))
				{
					arg = mod.title;
					break;
				}
			}
			ClearLanguage();
			GameObject gameObject = KScreenManager.AddChild(context, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject);
			KScreen component = gameObject.GetComponent<KScreen>();
			component.Activate();
			ConfirmDialogScreen component2 = component.GetComponent<ConfirmDialogScreen>();
			ConfirmDialogScreen confirmDialogScreen = component2;
			string title_text = UI.CONFIRMDIALOG.DIALOG_HEADER;
			string text = string.Format(UI.FRONTEND.TRANSLATIONS_SCREEN.MISSING_LANGUAGE_PACK, arg);
			string confirm_text = UI.FRONTEND.TRANSLATIONS_SCREEN.RESTART;
			System.Action on_confirm = App.instance.Restart;
			confirmDialogScreen.PopupConfirmDialog(text, on_confirm, null, null, null, title_text, confirm_text, null, null, true);
		}
	}

	public static void LoadPreinstalledTranslation(string code)
	{
		if (!string.IsNullOrEmpty(code) && code != DEFAULT_LANGUAGE_CODE)
		{
			string preinstalledLocalizationFilePath = GetPreinstalledLocalizationFilePath(code);
			if (LoadLocalTranslationFile(SelectedLanguageType.Preinstalled, preinstalledLocalizationFilePath))
			{
				KPlayerPrefs.SetString(SELECTED_LANGUAGE_CODE_KEY, code);
			}
		}
		else
		{
			ClearLanguage();
		}
	}

	public static bool LoadLocalTranslationFile(SelectedLanguageType source, string path)
	{
		if (File.Exists(path))
		{
			string[] lines = File.ReadAllLines(path, Encoding.UTF8);
			bool flag = LoadTranslationFromLines(lines);
			if (flag)
			{
				KPlayerPrefs.SetString(SELECTED_LANGUAGE_TYPE_KEY, source.ToString());
			}
			else
			{
				ClearLanguage();
			}
			return flag;
		}
		return false;
	}

	private static bool LoadTranslationFromLines(string[] lines)
	{
		if (lines != null && lines.Length > 0)
		{
			sLocale = GetLocale(lines);
			DebugUtil.LogArgs(" -> Locale is now ", sLocale.ToString());
			bool flag = LoadTranslation(lines, false);
			if (flag)
			{
				currentFontName = GetFontName(lines);
				SwapToLocalizedFont(currentFontName);
			}
			return flag;
		}
		return false;
	}

	private static bool LoadTranslation(string[] lines, bool isTemplate = false)
	{
		try
		{
			Dictionary<string, string> translated_strings = ExtractTranslatedStrings(lines, isTemplate);
			OverloadStrings(translated_strings);
			return true;
		}
		catch (Exception ex)
		{
			DebugUtil.LogWarningArgs(ex);
			return false;
		}
	}

	public static Dictionary<string, string> LoadStringsFile(string path, bool isTemplate)
	{
		string[] lines = File.ReadAllLines(path, Encoding.UTF8);
		return ExtractTranslatedStrings(lines, isTemplate);
	}

	public static Dictionary<string, string> ExtractTranslatedStrings(string[] lines, bool isTemplate = false)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Entry entry = default(Entry);
		string key = (!isTemplate) ? "msgstr" : "msgid";
		for (int i = 0; i < lines.Length; i++)
		{
			string text = lines[i];
			if (text == null || text.Length == 0)
			{
				entry = default(Entry);
			}
			else
			{
				string parameter = GetParameter("msgctxt", i, lines);
				if (parameter != null)
				{
					entry.msgctxt = parameter;
				}
				parameter = GetParameter(key, i, lines);
				if (parameter != null)
				{
					entry.msgstr = parameter;
				}
			}
			if (entry.IsPopulated)
			{
				dictionary[entry.msgctxt] = entry.msgstr;
				entry = default(Entry);
			}
		}
		return dictionary;
	}

	private static string FixupString(string result)
	{
		result = result.Replace("\\n", "\n");
		result = result.Replace("\\\"", "\"");
		result = result.Replace("<style=“", "<style=\"");
		result = result.Replace("”>", "\">");
		result = result.Replace("<color=^p", "<color=#");
		return result;
	}

	private static string GetParameter(string key, int idx, string[] all_lines)
	{
		if (all_lines[idx].StartsWith(key))
		{
			List<string> list = new List<string>();
			string text = all_lines[idx];
			text = text.Substring(key.Length + 1, text.Length - key.Length - 1);
			list.Add(text);
			for (int i = idx + 1; i < all_lines.Length; i++)
			{
				string text2 = all_lines[i];
				if (!text2.StartsWith("\""))
				{
					break;
				}
				list.Add(text2);
			}
			string text3 = "";
			foreach (string item in list)
			{
				string text4 = item;
				if (text4.EndsWith("\r"))
				{
					text4 = text4.Substring(0, text4.Length - 1);
				}
				text4 = text4.Substring(1, text4.Length - 2);
				text4 = FixupString(text4);
				text3 += text4;
			}
			return text3;
		}
		return null;
	}

	private static void AddAssembly(string locstrings_namespace, Assembly assembly)
	{
		if (!translatable_assemblies.TryGetValue(locstrings_namespace, out List<Assembly> value))
		{
			value = new List<Assembly>();
			translatable_assemblies.Add(locstrings_namespace, value);
		}
		value.Add(assembly);
	}

	public static void AddAssembly(Assembly assembly)
	{
		AddAssembly("STRINGS", assembly);
	}

	public static void RegisterForTranslation(Type locstring_tree_root)
	{
		Assembly assembly = Assembly.GetAssembly(locstring_tree_root);
		AddAssembly(locstring_tree_root.Namespace, assembly);
		string parent_path = locstring_tree_root.Namespace + '.';
		foreach (Type item in CollectLocStringTreeRoots(locstring_tree_root.Namespace, assembly))
		{
			LocString.CreateLocStringKeys(item, parent_path);
		}
	}

	public static void OverloadStrings(Dictionary<string, string> translated_strings)
	{
		string parameter_errors = "";
		string link_errors = "";
		string link_count_errors = "";
		foreach (KeyValuePair<string, List<Assembly>> translatable_assembly in translatable_assemblies)
		{
			foreach (Assembly item in translatable_assembly.Value)
			{
				foreach (Type item2 in CollectLocStringTreeRoots(translatable_assembly.Key, item))
				{
					string path = translatable_assembly.Key + "." + item2.Name;
					OverloadStrings(translated_strings, path, item2, ref parameter_errors, ref link_errors, ref link_count_errors);
				}
			}
		}
		if (!string.IsNullOrEmpty(parameter_errors))
		{
			DebugUtil.LogArgs("TRANSLATION ERROR! The following have missing or mismatched parameters:\n" + parameter_errors);
		}
		if (!string.IsNullOrEmpty(link_errors))
		{
			DebugUtil.LogArgs("TRANSLATION ERROR! The following have mismatched <link> tags:\n" + link_errors);
		}
		if (!string.IsNullOrEmpty(link_count_errors))
		{
			DebugUtil.LogArgs("TRANSLATION ERROR! The following do not have the same amount of <link> tags as the english string which can cause nested link errors:\n" + link_count_errors);
		}
	}

	public static void OverloadStrings(Dictionary<string, string> translated_strings, string path, Type locstring_hierarchy, ref string parameter_errors, ref string link_errors, ref string link_count_errors)
	{
		FieldInfo[] fields = locstring_hierarchy.GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType == typeof(LocString))
			{
				string text = path + "." + fieldInfo.Name;
				string value = null;
				if (translated_strings.TryGetValue(text, out value))
				{
					LocString locString = (LocString)fieldInfo.GetValue(null);
					LocString value2 = new LocString(value, text);
					if (!AreParametersPreserved(locString.text, value))
					{
						parameter_errors = parameter_errors + "\t" + text + "\n";
					}
					else if (!HasSameOrLessLinkCountAsEnglish(locString.text, value))
					{
						link_count_errors = link_count_errors + "\t" + text + "\n";
					}
					else if (!HasMatchingLinkTags(value, 0))
					{
						link_errors = link_errors + "\t" + text + "\n";
					}
					else
					{
						fieldInfo.SetValue(null, value2);
					}
				}
			}
		}
		Type[] nestedTypes = locstring_hierarchy.GetNestedTypes();
		Type[] array2 = nestedTypes;
		foreach (Type type in array2)
		{
			string path2 = path + "." + type.Name;
			OverloadStrings(translated_strings, path2, type, ref parameter_errors, ref link_errors, ref link_count_errors);
		}
	}

	public static string GetDefaultLocalizationFilePath()
	{
		return Path.Combine(Application.streamingAssetsPath, "strings/strings_template.pot");
	}

	public static string GetModLocalizationFilePath()
	{
		return Path.Combine(Application.streamingAssetsPath, "strings/strings.po");
	}

	public static string GetPreinstalledLocalizationFilePath(string code)
	{
		string path = "strings/strings_preinstalled_" + code + ".po";
		return Path.Combine(Application.streamingAssetsPath, path);
	}

	public static string GetPreinstalledLocalizationTitle(string code)
	{
		return Strings.Get("STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.PREINSTALLED_LANGUAGES." + code.ToUpper());
	}

	public static Texture2D GetPreinstalledLocalizationImage(string code)
	{
		string path = Path.Combine(Application.streamingAssetsPath, "strings/preinstalled_icon_" + code + ".png");
		if (!File.Exists(path))
		{
			return null;
		}
		byte[] data = File.ReadAllBytes(path);
		Texture2D texture2D = new Texture2D(2, 2);
		texture2D.LoadImage(data);
		return texture2D;
	}

	public static void SetLocale(Locale locale)
	{
		sLocale = locale;
		DebugUtil.LogArgs(" -> Locale is now ", sLocale.ToString());
	}

	private static string GetFontParam(string line)
	{
		string result = null;
		if (line.StartsWith("\"Font:"))
		{
			result = line.Substring("\"Font:".Length).Trim();
			result = result.Replace("\\n", "");
			result = result.Replace("\"", "");
		}
		return result;
	}

	public static string GetSelectedPreinstalledLanguageCode()
	{
		SelectedLanguageType selectedLanguageType = GetSelectedLanguageType();
		if (selectedLanguageType != SelectedLanguageType.Preinstalled)
		{
			return "";
		}
		return KPlayerPrefs.GetString(SELECTED_LANGUAGE_CODE_KEY);
	}

	public static SelectedLanguageType GetSelectedLanguageType()
	{
		return (SelectedLanguageType)Enum.Parse(typeof(SelectedLanguageType), KPlayerPrefs.GetString(SELECTED_LANGUAGE_TYPE_KEY, 0.ToString()), true);
	}

	private static string GetLanguageCode(string line)
	{
		string result = null;
		if (line.StartsWith("\"Language:"))
		{
			result = line.Substring("\"Language:".Length).Trim();
			result = result.Replace("\\n", "");
			result = result.Replace("\"", "");
		}
		return result;
	}

	private static Locale GetLocaleForCode(string code)
	{
		Locale result = null;
		foreach (Locale locale in Locales)
		{
			if (locale.MatchesCode(code))
			{
				result = locale;
				break;
			}
		}
		return result;
	}

	public static Locale GetLocale(string[] lines)
	{
		Locale locale = null;
		string text = null;
		if (lines != null && lines.Length > 0)
		{
			foreach (string text2 in lines)
			{
				if (text2 != null && text2.Length != 0)
				{
					text = GetLanguageCode(text2);
					if (text != null)
					{
						locale = GetLocaleForCode(text);
					}
					if (text != null)
					{
						break;
					}
				}
			}
		}
		if (locale == null)
		{
			locale = GetDefaultLocale();
		}
		if (text != null && locale.Code == "")
		{
			locale.SetCode(text);
		}
		return locale;
	}

	private static string GetFontName(string filename)
	{
		string[] lines = File.ReadAllLines(filename, Encoding.UTF8);
		return GetFontName(lines);
	}

	public static Locale GetDefaultLocale()
	{
		Locale result = null;
		foreach (Locale locale in Locales)
		{
			if (locale.Lang == Language.Unspecified)
			{
				result = new Locale(locale);
				break;
			}
		}
		return result;
	}

	public static string GetDefaultFontName()
	{
		string result = null;
		foreach (Locale locale in Locales)
		{
			if (locale.Lang == Language.Unspecified)
			{
				result = locale.FontName;
				break;
			}
		}
		return result;
	}

	public static string ValidateFontName(string fontName)
	{
		foreach (Locale locale in Locales)
		{
			if (locale.MatchesFont(fontName))
			{
				return locale.FontName;
			}
		}
		return null;
	}

	public static string GetFontName(string[] lines)
	{
		string text = null;
		foreach (string text2 in lines)
		{
			if (text2 != null && text2.Length != 0)
			{
				string fontParam = GetFontParam(text2);
				if (fontParam != null)
				{
					text = ValidateFontName(fontParam);
				}
			}
			if (text != null)
			{
				break;
			}
		}
		if (text == null)
		{
			text = ((sLocale == null) ? GetDefaultFontName() : sLocale.FontName);
		}
		return text;
	}

	public static void SwapToLocalizedFont()
	{
		SwapToLocalizedFont(currentFontName);
	}

	public static bool SwapToLocalizedFont(string fontname)
	{
		if (!string.IsNullOrEmpty(fontname))
		{
			sFontAsset = GetFont(fontname);
			TextStyleSetting[] array = Resources.FindObjectsOfTypeAll<TextStyleSetting>();
			foreach (TextStyleSetting textStyleSetting in array)
			{
				if ((UnityEngine.Object)textStyleSetting != (UnityEngine.Object)null)
				{
					textStyleSetting.sdfFont = sFontAsset;
				}
			}
			bool isRightToLeft = IsRightToLeft;
			LocText[] array2 = Resources.FindObjectsOfTypeAll<LocText>();
			foreach (LocText locText in array2)
			{
				if ((UnityEngine.Object)locText != (UnityEngine.Object)null)
				{
					locText.SwapFont(sFontAsset, isRightToLeft);
				}
			}
			return true;
		}
		return false;
	}

	private static bool SetFont(Type target_type, object target, TMP_FontAsset font, bool is_right_to_left, HashSet<MemberInfo> excluded_members)
	{
		if (target_type != null && target != null && !((UnityEngine.Object)font == (UnityEngine.Object)null))
		{
			FieldInfo[] fields = target_type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!excluded_members.Contains(fieldInfo))
				{
					if (fieldInfo.FieldType == typeof(TextStyleSetting))
					{
						((TextStyleSetting)fieldInfo.GetValue(target)).sdfFont = font;
					}
					else if (fieldInfo.FieldType == typeof(LocText))
					{
						((LocText)fieldInfo.GetValue(target)).SwapFont(font, is_right_to_left);
					}
					else if (fieldInfo.FieldType == typeof(GameObject))
					{
						Component[] components = ((GameObject)fieldInfo.GetValue(target)).GetComponents<Component>();
						foreach (Component component in components)
						{
							SetFont(component.GetType(), component, font, is_right_to_left, excluded_members);
						}
					}
					else if (fieldInfo.MemberType == MemberTypes.Field && fieldInfo.FieldType != fieldInfo.DeclaringType)
					{
						SetFont(fieldInfo.FieldType, fieldInfo.GetValue(target), font, is_right_to_left, excluded_members);
					}
				}
			}
			return true;
		}
		return false;
	}

	public static bool SetFont<T>(T target, TMP_FontAsset font, bool is_right_to_left, HashSet<MemberInfo> excluded_members)
	{
		return SetFont(typeof(T), target, font, is_right_to_left, excluded_members);
	}

	public static TMP_FontAsset GetFont(string fontname)
	{
		TMP_FontAsset[] array = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
		foreach (TMP_FontAsset tMP_FontAsset in array)
		{
			if (tMP_FontAsset.name == fontname)
			{
				return tMP_FontAsset;
			}
		}
		return null;
	}

	private static bool HasSameOrLessTokenCount(string english_string, string translated_string, string token)
	{
		int num = english_string.Split(new string[1]
		{
			token
		}, StringSplitOptions.None).Length;
		int num2 = translated_string.Split(new string[1]
		{
			token
		}, StringSplitOptions.None).Length;
		return num >= num2;
	}

	private static bool HasSameOrLessLinkCountAsEnglish(string english_string, string translated_string)
	{
		return HasSameOrLessTokenCount(english_string, translated_string, "<link") && HasSameOrLessTokenCount(english_string, translated_string, "</link");
	}

	private static bool HasMatchingLinkTags(string str, int idx = 0)
	{
		int num = str.IndexOf("<link", idx);
		int num2 = str.IndexOf("</link", idx);
		if (num == -1 && num2 == -1)
		{
			return true;
		}
		if (num == -1 && num2 != -1)
		{
			return false;
		}
		if (num != -1 && num2 == -1)
		{
			return false;
		}
		if (num2 >= num)
		{
			int num3 = str.IndexOf("<link", num + 1);
			if (num >= 0 && num3 != -1 && num3 < num2)
			{
				return false;
			}
			return HasMatchingLinkTags(str, num2 + 1);
		}
		return false;
	}

	private static bool AreParametersPreserved(string old_string, string new_string)
	{
		MatchCollection matchCollection = Regex.Matches(old_string, "{.*?}");
		MatchCollection matchCollection2 = Regex.Matches(new_string, "{.*?}");
		bool result = false;
		if (matchCollection == null && matchCollection2 == null)
		{
			result = true;
		}
		else if (matchCollection != null && matchCollection2 != null && matchCollection.Count == matchCollection2.Count)
		{
			result = true;
			IEnumerator enumerator = matchCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					string a = current.ToString();
					bool flag = false;
					IEnumerator enumerator2 = matchCollection2.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							object current2 = enumerator2.Current;
							string b = current2.ToString();
							if (a == b)
							{
								flag = true;
								break;
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator2 as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					if (!flag)
					{
						result = false;
						break;
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
		}
		return result;
	}

	public static bool HasDirtyWords(string str)
	{
		return FilterDirtyWords(str) != str;
	}

	public static string FilterDirtyWords(string str)
	{
		return DistributionPlatform.Inst.ApplyWordFilter(str);
	}

	public static string GetFileDateFormat(int format_idx)
	{
		return "{" + format_idx.ToString() + ":dd / MMM / yyyy}";
	}

	public static void ClearLanguage()
	{
		DebugUtil.LogArgs(" -> Clearing selected language! Either it didn't load correct or returning to english by menu.");
		sFontAsset = null;
		sLocale = null;
		KPlayerPrefs.SetString(SELECTED_LANGUAGE_TYPE_KEY, 0.ToString());
		KPlayerPrefs.SetString(SELECTED_LANGUAGE_CODE_KEY, "");
		SwapToLocalizedFont(GetDefaultLocale().FontName);
		string defaultLocalizationFilePath = GetDefaultLocalizationFilePath();
		if (File.Exists(defaultLocalizationFilePath))
		{
			string[] lines = File.ReadAllLines(defaultLocalizationFilePath, Encoding.UTF8);
			LoadTranslation(lines, true);
		}
		LanguageOptionsScreen.CleanUpCurrentModLanguage();
	}

	private static string ReverseText(string source)
	{
		char[] separator = new char[1]
		{
			'\n'
		};
		string[] array = source.Split(separator);
		string text = "";
		int num = 0;
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			num++;
			char[] array3 = new char[text2.Length];
			for (int j = 0; j < text2.Length; j++)
			{
				array3[array3.Length - 1 - j] = text2[j];
			}
			text += new string(array3);
			if (num < array.Length)
			{
				text += "\n";
			}
		}
		return text;
	}

	public static string Fixup(string text)
	{
		if (sLocale != null && text != null && text != "" && sLocale.Lang == Language.Arabic)
		{
			return ReverseText(ArabicFixer.Fix(text));
		}
		return text;
	}
}
