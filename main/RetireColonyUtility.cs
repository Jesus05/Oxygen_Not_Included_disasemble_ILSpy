using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public static class RetireColonyUtility
{
	private const int FILE_IO_RETRY_ATTEMPTS = 5;

	private static char[] invalidCharacters = "<>:\"\\/|?*.".ToCharArray();

	private static Encoding[] attempt_encodings = new Encoding[3]
	{
		new UTF8Encoding(false, true),
		new UnicodeEncoding(false, true, true),
		Encoding.ASCII
	};

	public static bool SaveColonySummaryData()
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string text = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = StripInvalidCharacters(SaveGame.Instance.BaseName);
		string text3 = Path.Combine(text, text2);
		if (!Directory.Exists(text3))
		{
			Directory.CreateDirectory(text3);
		}
		string path = Path.Combine(text3, text2 + ".json");
		MinionAssignablesProxy[] array = new MinionAssignablesProxy[Components.MinionAssignablesProxy.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Components.MinionAssignablesProxy[i];
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, ColonyAchievementStatus> achievement in SaveGame.Instance.GetComponent<ColonyAchievementTracker>().achievements)
		{
			if (achievement.Value.success)
			{
				list.Add(achievement.Key);
			}
		}
		BuildingComplete[] array2 = new BuildingComplete[Components.BuildingCompletes.Count];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = Components.BuildingCompletes[j];
		}
		RetiredColonyData value = new RetiredColonyData(SaveGame.Instance.BaseName, GameClock.Instance.GetCycle(), System.DateTime.Now.ToShortDateString(), list.ToArray(), array, array2);
		string s = JsonConvert.SerializeObject(value);
		bool flag = false;
		int num = 0;
		while (!flag && num < 5)
		{
			try
			{
				Thread.Sleep(num * 100);
				using (FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					flag = true;
					Encoding uTF = Encoding.UTF8;
					byte[] bytes = uTF.GetBytes(s);
					fileStream.Write(bytes, 0, bytes.Length);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("SaveColonySummaryData failed attempt {0}: {1}", num + 1, ex.ToString());
			}
			num++;
		}
		return flag;
	}

	public static RetiredColonyData[] LoadRetiredColonies()
	{
		List<RetiredColonyData> list = new List<RetiredColonyData>();
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string path = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		path = Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
		string[] directories = Directory.GetDirectories(path);
		foreach (string path2 in directories)
		{
			string[] files = Directory.GetFiles(path2);
			foreach (string text in files)
			{
				if (text.EndsWith(".json"))
				{
					int num = 0;
					while (true)
					{
						if (num < attempt_encodings.Length)
						{
							Encoding encoding = attempt_encodings[num];
							try
							{
								string value = File.ReadAllText(text, encoding);
								RetiredColonyData retiredColonyData = JsonConvert.DeserializeObject<RetiredColonyData>(value);
								if (retiredColonyData != null)
								{
									if (retiredColonyData.colonyName == null)
									{
										throw new Exception("data.colonyName was null");
									}
									list.Add(retiredColonyData);
								}
							}
							catch (Exception ex)
							{
								Debug.LogWarningFormat("LoadRetiredColonies failed load {0} [{1}]: {2}", encoding, text, ex.ToString());
								goto IL_010f;
							}
						}
						break;
						IL_010f:
						num++;
					}
				}
			}
		}
		return list.ToArray();
	}

	public static Sprite[] LoadColonySlideshow(string colonyName)
	{
		string path = StripInvalidCharacters(colonyName);
		string text = Path.Combine(Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), path);
		List<Sprite> list = new List<Sprite>();
		if (Directory.Exists(text))
		{
			string[] files = Directory.GetFiles(text);
			foreach (string text2 in files)
			{
				if (text2.EndsWith(".png"))
				{
					Texture2D texture2D = new Texture2D(640, 360);
					texture2D.filterMode = FilterMode.Point;
					texture2D.LoadImage(File.ReadAllBytes(text2));
					list.Add(Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2(640f, 360f)), new Vector2(0.5f, 0.5f)));
				}
			}
		}
		else
		{
			Debug.LogWarningFormat("LoadColonySlideshow path does not exist or is not directory [{0}]", text);
		}
		return list.ToArray();
	}

	public static Sprite LoadColonyPreview(string colonyName)
	{
		string path = StripInvalidCharacters(colonyName);
		string text = Path.Combine(Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), path);
		List<string> list = new List<string>();
		if (Directory.Exists(text))
		{
			string[] files = Directory.GetFiles(text);
			foreach (string text2 in files)
			{
				if (text2.EndsWith(".png"))
				{
					list.Add(text2);
				}
			}
		}
		else
		{
			Debug.LogWarningFormat("LoadColonyPreview path does not exist or is not directory [{0}]", text);
		}
		if (list.Count > 0)
		{
			Texture2D texture2D = new Texture2D(640, 360);
			texture2D.LoadImage(File.ReadAllBytes(list[list.Count - 1]));
			return Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2(640f, 360f)), new Vector2(0.5f, 0.5f));
		}
		return null;
	}

	public static string StripInvalidCharacters(string source)
	{
		char[] array = invalidCharacters;
		foreach (char oldChar in array)
		{
			source = source.Replace(oldChar, '_');
		}
		source = source.Trim();
		return source;
	}
}
