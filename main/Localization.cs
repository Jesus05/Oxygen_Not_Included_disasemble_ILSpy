using ArabicSupport;
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
		new Locale(Language.Unspecified, Direction.LeftToRight, string.Empty, "RobotoCondensed-Regular")
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

	private const string start_link_token = "<link";

	private const string end_link_token = "</link";

	public static TMP_FontAsset FontAsset => sFontAsset;

	public static bool IsRightToLeft => sLocale != null && sLocale.IsRightToLeft;

	public static void Initialize(bool dontCheckSteam = false)
	{
		Output.Log("Localization.Initialize!");
		switch ((SelectedLanguageType)Enum.Parse(typeof(SelectedLanguageType), KPlayerPrefs.GetString(SELECTED_LANGUAGE_TYPE_KEY, 0.ToString()), true))
		{
		case SelectedLanguageType.Preinstalled:
		{
			Output.Log("Localization Initialize... Preinstalled localization");
			string @string = KPlayerPrefs.GetString(SELECTED_LANGUAGE_CODE_KEY, string.Empty);
			Output.Log(" -> ", @string);
			LoadPreinstalledTranslation(@string);
			return;
		}
		case SelectedLanguageType.UGC:
			if (!dontCheckSteam && SteamManager.Initialized && LanguageOptionsScreen.HasInstalledLanguage())
			{
				Output.Log("Localization Initialize... SteamUGCService");
				PublishedFileId_t item = PublishedFileId_t.Invalid;
				LanguageOptionsScreen.LoadTranslation(ref item);
				if (item != PublishedFileId_t.Invalid)
				{
					Output.Log(" -> Loaded steamworks file id: ", item.ToString());
				}
				else
				{
					Output.Log(" -> Failed to load steamworks file id: ", item.ToString());
				}
				return;
			}
			break;
		}
		Output.Log("Initialize... Local mod localization");
		string modLocalizationFilePath = GetModLocalizationFilePath();
		Output.Log(" -> ", modLocalizationFilePath);
		LoadLocalTranslationFile(SelectedLanguageType.None, modLocalizationFilePath);
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
		bool flag = false;
		if (lines != null && lines.Length > 0)
		{
			sLocale = GetLocale(lines);
			Output.Log(" -> Locale is now ", sLocale.ToString());
			flag = LoadTranslation(lines, false);
			if (flag)
			{
				currentFontName = GetFontName(lines);
				SwapToLocalizedFont(currentFontName);
			}
		}
		return flag;
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
			Output.LogWarning(ex);
			return false;
		}
	}

	public static Dictionary<string, string> LoadStringsFile(string path, bool isTemplate)
	{
		string[] lines = File.ReadAllLines(path, Encoding.UTF8);
		return ExtractTranslatedStrings(lines, isTemplate);
	}

	private static Dictionary<string, string> ExtractTranslatedStrings(string[] lines, bool isTemplate = false)
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
		if (!all_lines[idx].StartsWith(key))
		{
			return null;
		}
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
		string text3 = string.Empty;
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

	private static void OverloadStrings(Dictionary<string, string> translated_strings)
	{
		Assembly assembly = Assembly.GetAssembly(typeof(UI));
		IEnumerable<Type> source = from t in assembly.GetTypes()
		where t.IsClass && t.Namespace == "STRINGS" && !t.IsNested
		select t;
		string parameter_errors = string.Empty;
		string link_errors = string.Empty;
		string link_count_errors = string.Empty;
		List<Type> list = source.ToList();
		foreach (Type item in list)
		{
			string path = "STRINGS." + item.Name;
			OverloadStrings(translated_strings, path, item, ref parameter_errors, ref link_errors, ref link_count_errors);
		}
		if (!string.IsNullOrEmpty(parameter_errors))
		{
			Output.Log("TRANSLATION ERROR! The following have missing or mismatched parameters:\n" + parameter_errors);
		}
		if (!string.IsNullOrEmpty(link_errors))
		{
			Output.Log("TRANSLATION ERROR! The following have mismatched <link> tags:\n" + link_errors);
		}
		if (!string.IsNullOrEmpty(link_count_errors))
		{
			Output.Log("TRANSLATION ERROR! The following do not have the same amount of <link> tags as the english string which can cause nested link errors:\n" + link_count_errors);
		}
	}

	private static void OverloadStrings(Dictionary<string, string> translated_strings, string path, Type t, ref string parameter_errors, ref string link_errors, ref string link_count_errors)
	{
		FieldInfo[] fields = t.GetFields();
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
					if (AreParametersPreserved(locString.text, value))
					{
						if (HasSameOrLessLinkCountAsEnglish(locString.text, value))
						{
							if (HasMatchingLinkTags(value, 0))
							{
								fieldInfo.SetValue(null, value2);
							}
							else
							{
								link_errors = link_errors + "\t" + text + "\n";
							}
						}
						else
						{
							link_count_errors = link_count_errors + "\t" + text + "\n";
						}
					}
					else
					{
						parameter_errors = parameter_errors + "\t" + text + "\n";
					}
				}
			}
		}
		Type[] nestedTypes = t.GetNestedTypes();
		Type[] array2 = nestedTypes;
		foreach (Type type in array2)
		{
			string path2 = path + "." + type.Name;
			OverloadStrings(translated_strings, path2, type, ref parameter_errors, ref link_errors, ref link_count_errors);
		}
	}

	public static string GetDefaultLocalizationFilePath()
	{
		return Path.Combine(Application.streamingAssetsPath, "Mods/strings_template.pot");
	}

	public static string GetModLocalizationFilePath()
	{
		return Path.Combine(Application.streamingAssetsPath, "Mods/strings.po");
	}

	public static string GetPreinstalledLocalizationFilePath(string code)
	{
		string path = "Mods/strings_preinstalled_" + code + ".po";
		return Path.Combine(Application.streamingAssetsPath, path);
	}

	public static string GetPreinstalledLocalizationTitle(string code)
	{
		return Strings.Get("STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.PREINSTALLED_LANGUAGES." + code.ToUpper());
	}

	public static Texture2D GetPreinstalledLocalizationImage(string code)
	{
		string path = Path.Combine(Application.streamingAssetsPath, "Mods/preinstalled_icon_" + code + ".png");
		if (File.Exists(path))
		{
			byte[] data = File.ReadAllBytes(path);
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(data);
			return texture2D;
		}
		return null;
	}

	public static void SetLocale(Locale locale)
	{
		sLocale = locale;
		Output.Log(" -> Locale is now ", sLocale.ToString());
	}

	private static string GetFontParam(string line)
	{
		string result = null;
		if (line.StartsWith("\"Font:"))
		{
			result = line.Substring("\"Font:".Length).Trim();
			result = result.Replace("\\n", string.Empty);
			result = result.Replace("\"", string.Empty);
		}
		return result;
	}

	private static string GetLanguageCode(string line)
	{
		string result = null;
		if (line.StartsWith("\"Language:"))
		{
			result = line.Substring("\"Language:".Length).Trim();
			result = result.Replace("\\n", string.Empty);
			result = result.Replace("\"", string.Empty);
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
				return locale;
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
		if (text != null && locale.Code == string.Empty)
		{
			locale.SetCode(text);
		}
		return locale;
	}

	public static TMP_FontAsset GetFontForLocale(string code)
	{
		Locale locale = GetLocaleForCode(code);
		if (locale == null)
		{
			locale = GetDefaultLocale();
		}
		return Resources.Load<TMP_FontAsset>(locale.FontName);
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
				return new Locale(locale);
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
				return locale.FontName;
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

	public static void SwapToLocalizedFont(string fontname)
	{
		if (!string.IsNullOrEmpty(fontname))
		{
			TMP_FontAsset tMP_FontAsset = Resources.Load<TMP_FontAsset>(fontname);
			if ((UnityEngine.Object)tMP_FontAsset != (UnityEngine.Object)null)
			{
				sFontAsset = tMP_FontAsset;
				TextStyleSetting[] array = Resources.FindObjectsOfTypeAll<TextStyleSetting>();
				foreach (TextStyleSetting textStyleSetting in array)
				{
					if ((UnityEngine.Object)textStyleSetting != (UnityEngine.Object)null)
					{
						textStyleSetting.sdfFont = tMP_FontAsset;
					}
				}
				bool isRightToLeft = IsRightToLeft;
				LocText[] array2 = Resources.FindObjectsOfTypeAll<LocText>();
				foreach (LocText locText in array2)
				{
					if ((UnityEngine.Object)locText != (UnityEngine.Object)null)
					{
						locText.SwapFont(tMP_FontAsset, isRightToLeft);
					}
				}
			}
			else
			{
				Console.WriteLine("LOCALIZATION ERROR! Font [" + fontname + "] not found");
			}
		}
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
		if (num2 < num)
		{
			return false;
		}
		int num3 = str.IndexOf("<link", num + 1);
		if (num >= 0 && num3 != -1 && num3 < num2)
		{
			return false;
		}
		return HasMatchingLinkTags(str, num2 + 1);
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
						return false;
					}
				}
				return result;
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
		Output.Log(" -> Clearing selected language! Either it didn't load correct or returning to english by menu.");
		sFontAsset = null;
		sLocale = null;
		KPlayerPrefs.SetString(SELECTED_LANGUAGE_TYPE_KEY, 0.ToString());
		KPlayerPrefs.SetString(SELECTED_LANGUAGE_CODE_KEY, string.Empty);
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
		string text = string.Empty;
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
		if (sLocale != null && text != null && text != string.Empty && sLocale.Lang == Language.Arabic)
		{
			return ReverseText(ArabicFixer.Fix(text));
		}
		return text;
	}
}
