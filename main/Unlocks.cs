using Newtonsoft.Json;
using ProcGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public class Unlocks : KMonoBehaviour, ISim4000ms
{
	private const int FILE_IO_RETRY_ATTEMPTS = 5;

	public Dictionary<string, bool> locked = new Dictionary<string, bool>();

	public Dictionary<string, bool> defaultLocked = new Dictionary<string, bool>
	{
		{
			"poi_surface_facillity_1",
			true
		},
		{
			"poi_surface_facillity_2",
			true
		},
		{
			"poi_surface_facillity_3",
			true
		},
		{
			"poi_surface_facillity_4",
			true
		},
		{
			"firstrocketlaunch",
			true
		},
		{
			"duplicantdeath",
			true
		},
		{
			"onedupeleft",
			true
		},
		{
			"twentydupecolony",
			true
		},
		{
			"fulldupecolony",
			true
		},
		{
			"firstresearch",
			true
		},
		{
			"rocketryresearch",
			true
		},
		{
			"surfacebreach",
			true
		},
		{
			"nearingsurface",
			true
		},
		{
			"nearingmagma",
			true
		},
		{
			"neuralvacillator",
			true
		}
	};

	public Dictionary<string, string[]> lockCollections = new Dictionary<string, string[]>
	{
		{
			"critters",
			new string[1]
			{
				"critter_Hatch_studied"
			}
		},
		{
			"emails",
			new string[17]
			{
				"email_preliminarycalculations",
				"email_researchgiant",
				"email_frankiesblog",
				"email_atomiconrecruitment",
				"email_thejanitor",
				"email_security2",
				"email_newemployee",
				"email_security3",
				"email_hollandsdog",
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
			new string[23]
			{
				"journal_cleanup",
				"journal_employeeprocessing",
				"journal_sunflowerseeds",
				"journal_B835_1",
				"journal_B835_2",
				"journal_B835_3",
				"journal_B835_4",
				"journal_B835_5",
				"journal_B835_6",
				"journal_pipedream",
				"journal_spittingimage",
				"journal_A046_1",
				"journal_A046_2",
				"journal_A046_3",
				"journal_A046_4",
				"journal_ants",
				"journal_debrief",
				"journal_B327_1",
				"journal_B327_2",
				"journal_B327_3",
				"journal_B327_4",
				"journal_movedrats",
				"journal_revisitednumbers"
			}
		},
		{
			"researchnotes",
			new string[4]
			{
				"notes_clonedrats",
				"notes_hibiscus3",
				"notes_geneticooze",
				"notes_memoryimplantation"
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
		},
		{
			"special_set_items",
			new string[5]
			{
				"display_prop1",
				"display_prop2",
				"display_prop3",
				"pod_evacuation",
				"printingpod"
			}
		}
	};

	public Dictionary<int, string> cycleLocked = new Dictionary<int, string>
	{
		{
			3,
			"log2"
		},
		{
			10,
			"log3"
		},
		{
			15,
			"log4"
		},
		{
			20,
			"log5"
		},
		{
			30,
			"log6"
		},
		{
			35,
			"log7"
		}
	};

	private static string UnlocksFilename => System.IO.Path.Combine(Util.RootFolder(), "unlocks.json");

	protected override void OnPrefabInit()
	{
		foreach (KeyValuePair<string, string[]> lockCollection in lockCollections)
		{
			string[] value = lockCollection.Value;
			foreach (string key in value)
			{
				defaultLocked.Add(key, true);
			}
		}
		foreach (KeyValuePair<int, string> item in cycleLocked)
		{
			defaultLocked.Add(item.Value, true);
		}
		LoadLocks();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameClock.Instance.Subscribe(631075836, OnNewDay);
		Game.Instance.Subscribe(-1056989049, OnLaunchRocket);
		Game.Instance.Subscribe(282337316, OnDuplicantDied);
		Game.Instance.Subscribe(-107300940, OnResearchComplete);
		Game.Instance.Subscribe(-818188514, OnDiscoveredSpace);
		UnlockCycleCodexes();
		Components.LiveMinionIdentities.OnAdd += OnNewDupe;
	}

	public bool IsLocked(string lockID)
	{
		if (string.IsNullOrEmpty(lockID))
		{
			return false;
		}
		if (locked.ContainsKey(lockID))
		{
			return locked[lockID];
		}
		return false;
	}

	public void Unlock(string lockID)
	{
		if (locked.ContainsKey(lockID) && locked[lockID])
		{
			locked[lockID] = false;
			SaveUnlocks(locked);
			Game.Instance.Trigger(1594320620, lockID);
		}
	}

	public void UnlockOne(string[] lockIDs)
	{
		List<string> list = new List<string>();
		foreach (string text in lockIDs)
		{
			if (IsLocked(text))
			{
				list.Add(text);
			}
		}
		Unlock(list.GetRandom());
	}

	public static void SaveUnlocks(Dictionary<string, bool> locks)
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, bool> @lock in locks)
		{
			if (!@lock.Value)
			{
				list.Add(@lock.Key);
			}
		}
		string s = JsonConvert.SerializeObject(list);
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

	private void SetToDefaultLocks()
	{
		locked.Clear();
		foreach (KeyValuePair<string, bool> item in defaultLocked)
		{
			locked.Add(item.Key, item.Value);
		}
	}

	public void LoadLocks()
	{
		SetToDefaultLocks();
		if (File.Exists(UnlocksFilename))
		{
			string text = string.Empty;
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
					foreach (string key in array3)
					{
						if (locked.ContainsKey(key))
						{
							locked[key] = false;
						}
						else
						{
							locked.Add(key, false);
						}
					}
				}
				catch
				{
					Output.LogError("Error parsing", UnlocksFilename);
				}
				if (locked != null && locked.Count == 0)
				{
					return;
				}
			}
		}
	}

	public string UnlockNext(string collectionID)
	{
		string[] array = lockCollections[collectionID];
		foreach (string text in array)
		{
			if (locked[text])
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
		if (Components.LiveMinionIdentities.Count >= 20)
		{
			Unlock("twentydupecolony");
		}
		else if (Components.LiveMinionIdentities.Count >= 35)
		{
			Unlock("fulldupecolony");
		}
	}

	private void OnResearchComplete(object data)
	{
		Tech tech = (Tech)data;
		Unlock("firstresearch");
		if (tech.Id == "BasicRocketry")
		{
			Unlock("rocketryresearch");
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
