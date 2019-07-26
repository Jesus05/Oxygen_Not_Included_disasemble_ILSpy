using Newtonsoft.Json;
using ProcGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public class Unlocks : KMonoBehaviour
{
	private const int FILE_IO_RETRY_ATTEMPTS = 5;

	private List<string> unlocked = new List<string>();

	public Dictionary<string, string[]> lockCollections = new Dictionary<string, string[]>
	{
		{
			"emails",
			new string[21]
			{
				"email_thermodynamiclaws",
				"email_security2",
				"email_pens2",
				"email_atomiconrecruitment",
				"email_devonsblog",
				"email_researchgiant",
				"email_thejanitor",
				"email_newemployee",
				"email_timeoffapproved",
				"email_security3",
				"email_preliminarycalculations",
				"email_hollandsdog",
				"email_temporalbowupdate",
				"email_retemporalbowupdate",
				"email_memorychip",
				"email_arthistoryrequest",
				"email_AIcontrol",
				"email_AIcontrol2",
				"email_friendlyemail",
				"email_AIcontrol3",
				"email_AIcontrol4"
			}
		},
		{
			"journals",
			new string[29]
			{
				"journal_timesarrowthoughts",
				"journal_A046_1",
				"journal_B835_1",
				"journal_sunflowerseeds",
				"journal_B327_1",
				"journal_B556_1",
				"journal_employeeprocessing",
				"journal_B327_2",
				"journal_A046_2",
				"journal_elliesbirthday1",
				"journal_B835_2",
				"journal_ants",
				"journal_pipedream",
				"journal_B556_2",
				"journal_movedrats",
				"journal_B835_3",
				"journal_A046_3",
				"journal_B556_3",
				"journal_B327_3",
				"journal_B835_4",
				"journal_cleanup",
				"journal_A046_4",
				"journal_B327_4",
				"journal_revisitednumbers",
				"journal_B556_4",
				"journal_B835_5",
				"journal_elliesbirthday2",
				"journal_revisitednumbers2",
				"journal_timemusings"
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
			new string[6]
			{
				"misc_newsecurity",
				"misc_mailroometiquette",
				"misc_unattendedcultures",
				"misc_politerequest",
				"misc_casualfriday",
				"misc_dishbot"
			}
		}
	};

	public Dictionary<int, string> cycleLocked = new Dictionary<int, string>
	{
		{
			0,
			"log1"
		},
		{
			3,
			"log2"
		},
		{
			15,
			"log3"
		},
		{
			1000,
			"log4"
		},
		{
			1500,
			"log4b"
		},
		{
			2000,
			"log5"
		},
		{
			2500,
			"log5b"
		},
		{
			3000,
			"log6"
		},
		{
			3500,
			"log6b"
		},
		{
			4000,
			"log7"
		},
		{
			4001,
			"log8"
		}
	};

	private static string UnlocksFilename => System.IO.Path.Combine(Util.RootFolder(), "unlocks.json");

	protected override void OnPrefabInit()
	{
		LoadUnlocks();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UnlockCycleCodexes();
		GameClock.Instance.Subscribe(631075836, OnNewDay);
		Game.Instance.Subscribe(-1056989049, OnLaunchRocket);
		Game.Instance.Subscribe(282337316, OnDuplicantDied);
		Game.Instance.Subscribe(-818188514, OnDiscoveredSpace);
		Components.LiveMinionIdentities.OnAdd += OnNewDupe;
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
				DebugUtil.DevAssertArgs(false, "Found null/empty string in Unlocks collection: ", collectionID);
			}
			else if (!IsUnlocked(text))
			{
				Unlock(text);
				return text;
			}
		}
		return null;
	}

	private void UnlockCycleCodexes()
	{
		foreach (KeyValuePair<int, string> item in cycleLocked)
		{
			if (GameClock.Instance.GetCycle() + 1 >= item.Key)
			{
				Unlock(item.Value);
			}
		}
	}

	private void OnNewDay(object data)
	{
		UnlockCycleCodexes();
	}

	private void OnLaunchRocket(object data)
	{
		Unlock("surfacebreach");
		Unlock("firstrocketlaunch");
	}

	private void OnDuplicantDied(object data)
	{
		Unlock("duplicantdeath");
		if (Components.LiveMinionIdentities.Count == 1)
		{
			Unlock("onedupeleft");
		}
	}

	private void OnNewDupe(MinionIdentity minion_identity)
	{
		if (Components.LiveMinionIdentities.Count >= 35)
		{
			Unlock("fulldupecolony");
		}
	}

	private void OnDiscoveredSpace(object data)
	{
		Unlock("surfacebreach");
	}

	public void Sim4000ms(float dt)
	{
		int x = -2147483648;
		int num = -2147483648;
		int x2 = 2147483647;
		int num2 = 2147483647;
		foreach (MinionIdentity item in Components.MinionIdentities.Items)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
			{
				int cell = Grid.PosToCell(item);
				if (Grid.IsValidCell(cell))
				{
					Grid.CellToXY(cell, out int x3, out int y);
					if (y > num)
					{
						num = y;
						x = x3;
					}
					if (y < num2)
					{
						x2 = x3;
						num2 = y;
					}
				}
			}
		}
		if (num != -2147483648)
		{
			int num3 = num;
			for (int i = 0; i < 30; i++)
			{
				num3++;
				int cell2 = Grid.XYToCell(x, num3);
				if (!Grid.IsValidCell(cell2))
				{
					break;
				}
				SubWorld.ZoneType subWorldZoneType = World.Instance.zoneRenderData.GetSubWorldZoneType(cell2);
				if (subWorldZoneType == SubWorld.ZoneType.Space)
				{
					Unlock("nearingsurface");
					break;
				}
			}
		}
		if (num2 != 2147483647)
		{
			int num4 = num2;
			int num5 = 0;
			while (true)
			{
				if (num5 >= 30)
				{
					return;
				}
				num4--;
				int num6 = Grid.XYToCell(x2, num4);
				if (!Grid.IsValidCell(num6))
				{
					return;
				}
				SubWorld.ZoneType subWorldZoneType2 = World.Instance.zoneRenderData.GetSubWorldZoneType(num6);
				if (subWorldZoneType2 == SubWorld.ZoneType.ToxicJungle && Grid.Element[num6].id == SimHashes.Magma)
				{
					break;
				}
				num5++;
			}
			Unlock("nearingmagma");
		}
	}
}
