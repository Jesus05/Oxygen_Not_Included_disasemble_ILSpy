using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

public class Unlocks : KMonoBehaviour
{
	private const int FILE_IO_RETRY_ATTEMPTS = 5;

	private List<string> unlocked = new List<string>();

	public Dictionary<string, string[]> lockCollections = new Dictionary<string, string[]>
	{
		{
			"emails",
			new string[20]
			{
				"email_preliminarycalculations",
				"email_researchgiant",
				"email_thermodynamiclaws",
				"email_frankiesblog",
				"email_atomiconrecruitment",
				"email_thejanitor",
				"email_security2",
				"email_newemployee",
				"email_security3",
				"email_hollandsdog",
				"email_temporalbowupdate",
				"email_retemporalbowupdate",
				"email_pens",
				"email_pens2",
				"email_memorychip",
				"email_arthistoryrequest",
				"email_AIcontrol",
				"email_AIcontrol2",
				"email_AIcontrol3",
				"email_AIcontrol4"
			}
		},
		{
			"journals",
			new string[30]
			{
				"journal_sunflowerseeds",
				"journal_debrief",
				"journal_employeeprocessing",
				"journal_B835_1",
				"journal_B835_2",
				"journal_B835_3",
				"journal_B835_4",
				"journal_B835_5",
				"journal_B835_6",
				"journal_cleanup",
				"journal_pipedream",
				"journal_A046_1",
				"journal_A046_2",
				"journal_A046_3",
				"journal_A046_4",
				"journal_movedrats",
				"journal_spittingimage",
				"journal_B327_1",
				"journal_B327_2",
				"journal_B327_3",
				"journal_B327_4",
				"journal_revisitednumbers",
				"journal_ants",
				"journal_B556_1",
				"journal_B556_2",
				"journal_B556_3",
				"journal_B556_4",
				"journal_timemusings",
				"journal_timesarrowthoughts",
				"journal_magazine"
			}
		},
		{
			"researchnotes",
			new string[15]
			{
				"notes_clonedrats",
				"notes_agriculture1",
				"notes_husbandry1",
				"notes_hibiscus3",
				"notes_husbandry2",
				"notes_agriculture2",
				"notes_geneticooze",
				"notes_agriculture3",
				"notes_husbandry3",
				"notes_memoryimplantation",
				"notes_husbandry4",
				"notes_agriculture4",
				"notes_neutronium",
				"notes_firstsuccess",
				"notes_neutroniumapplications"
			}
		},
		{
			"misc",
			new string[7]
			{
				"misc_mailroometiquette",
				"misc_unattendedcultures",
				"misc_newsecurity",
				"misc_politerequest",
				"misc_casualfriday",
				"misc_bringyourkidtowork",
				"misc_dishbot"
			}
		}
	};

	private static string UnlocksFilename => Path.Combine(Util.RootFolder(), "unlocks.json");

	protected override void OnPrefabInit()
	{
		LoadUnlocks();
	}

	public bool IsUnlocked(string unlockID)
	{
		if (!string.IsNullOrEmpty(unlockID))
		{
			if (!DebugHandler.InstantBuildMode)
			{
				return unlocked.Contains(unlockID);
			}
			return true;
		}
		return false;
	}

	public void Unlock(string unlockID)
	{
		if (string.IsNullOrEmpty(unlockID))
		{
			DebugUtil.DevAssert(false, "Unlock called with null or empty string");
		}
		else if (!unlocked.Contains(unlockID))
		{
			unlocked.Add(unlockID);
			SaveUnlocks();
			Game.Instance.Trigger(1594320620, unlockID);
		}
	}

	private void SaveUnlocks()
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string s = JsonConvert.SerializeObject(unlocked);
		bool flag = false;
		int num = 0;
		while (!flag && num < 5)
		{
			try
			{
				Thread.Sleep(num * 100);
				using (FileStream fileStream = File.Open(UnlocksFilename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					flag = true;
					ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
					byte[] bytes = aSCIIEncoding.GetBytes(s);
					fileStream.Write(bytes, 0, bytes.Length);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("Failed to save Unlocks attempt {0}: {1}", num + 1, ex.ToString());
			}
			num++;
		}
	}

	public void LoadUnlocks()
	{
		unlocked.Clear();
		if (File.Exists(UnlocksFilename))
		{
			string text = "";
			bool flag = false;
			int num = 0;
			while (!flag && num < 5)
			{
				try
				{
					Thread.Sleep(num * 100);
					using (FileStream fileStream = File.Open(UnlocksFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						flag = true;
						ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
						byte[] array = new byte[fileStream.Length];
						if (fileStream.Read(array, 0, array.Length) == fileStream.Length)
						{
							text += aSCIIEncoding.GetString(array);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarningFormat("Failed to load Unlocks attempt {0}: {1}", num + 1, ex.ToString());
				}
				num++;
			}
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					string[] array2 = JsonConvert.DeserializeObject<string[]>(text);
					string[] array3 = array2;
					foreach (string text2 in array3)
					{
						if (!string.IsNullOrEmpty(text2) && !unlocked.Contains(text2))
						{
							unlocked.Add(text2);
						}
					}
				}
				catch (Exception ex2)
				{
					Debug.LogErrorFormat("Error parsing unlocks file [{0}]: {1}", UnlocksFilename, ex2.ToString());
				}
			}
		}
	}

	public string UnlockNext(string collectionID)
	{
		string[] array = lockCollections[collectionID];
		foreach (string text in array)
		{
			if (string.IsNullOrEmpty(text))
			{
				DebugUtil.DevAssert(false, "Found null/empty string in Unlocks collection: ", collectionID);
			}
			else if (!IsUnlocked(text))
			{
				Unlock(text);
				return text;
			}
		}
		return null;
	}
}
