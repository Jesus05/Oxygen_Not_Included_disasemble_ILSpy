using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using UnityEngine;

public class ReportManager : KMonoBehaviour
{
	public delegate string FormattingFn(float v);

	public enum ReportType
	{
		OxygenCreated,
		CaloriesCreated,
		StressDelta,
		EnergyCreated,
		EnergyWasted,
		LevelUp,
		TravelTime,
		IdleTime,
		DiseaseAdded,
		ToiletIncident,
		ContaminatedOxygenFlatulence,
		ContaminatedOxygenToilet,
		ContaminatedOxygenSublimation,
		DiseaseStatus,
		TimeSpent,
		ChoreStatus
	}

	public struct ReportGroup
	{
		public FormattingFn formatfn;

		public string stringKey;

		public string positiveTooltip;

		public string negativeTooltip;

		public bool reportIfZero;

		public int group;

		public ReportEntry.Order posNoteOrder;

		public ReportEntry.Order negNoteOrder;

		public ReportGroup(FormattingFn formatfn, bool reportIfZero, int group, string stringKey, string positiveTooltip, string negativeTooltip, ReportEntry.Order pos_note_order = ReportEntry.Order.Unordered, ReportEntry.Order neg_note_order = ReportEntry.Order.Unordered)
		{
			this.formatfn = ((formatfn == null) ? ((FormattingFn)((float v) => v.ToString())) : formatfn);
			this.stringKey = stringKey;
			this.positiveTooltip = positiveTooltip;
			this.negativeTooltip = negativeTooltip;
			this.reportIfZero = reportIfZero;
			this.group = group;
			posNoteOrder = pos_note_order;
			negNoteOrder = neg_note_order;
		}
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public class ReportEntry
	{
		public struct Note
		{
			public float value;

			public string note;

			public Note(float value, string note)
			{
				this.value = value;
				this.note = note;
			}
		}

		public enum Order
		{
			Unordered,
			Ascending,
			Descending
		}

		[Serialize]
		public int noteStorageId;

		[Serialize]
		public int gameHash = -1;

		[Serialize]
		public ReportType reportType;

		[Serialize]
		public string context;

		[Serialize]
		public float accumulate;

		[Serialize]
		public float accPositive;

		[Serialize]
		public float accNegative;

		[Serialize]
		public ArrayRef<ReportEntry> contextEntries;

		public float Positive => accPositive;

		public float Negative => accNegative;

		public float Net => accPositive + accNegative;

		public ReportEntry(ReportType reportType, int note_storage_id, string context)
		{
			this.reportType = reportType;
			this.context = context;
			accumulate = 0f;
			accPositive = 0f;
			accNegative = 0f;
			noteStorageId = note_storage_id;
		}

		[OnDeserializing]
		private void OnDeserialize()
		{
			contextEntries.Clear();
		}

		public void IterateNotes(Action<Note> callback)
		{
			Instance.noteStorage.IterateNotes(noteStorageId, callback);
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			if (gameHash != -1)
			{
				reportType = (ReportType)gameHash;
				gameHash = -1;
			}
		}

		public void AddData(NoteStorage note_storage, float value, string note = null, string dataContext = null)
		{
			AddActualData(note_storage, value, note);
			if (dataContext != null)
			{
				ReportEntry reportEntry = null;
				for (int i = 0; i < contextEntries.Count; i++)
				{
					if (contextEntries[i].context == dataContext)
					{
						reportEntry = contextEntries[i];
						break;
					}
				}
				if (reportEntry == null)
				{
					reportEntry = new ReportEntry(reportType, note_storage.GetNewNoteId(), dataContext);
					contextEntries.Add(reportEntry);
				}
				reportEntry.AddActualData(note_storage, value, note);
			}
		}

		private void AddActualData(NoteStorage note_storage, float value, string note = null)
		{
			accumulate += value;
			if (value > 0f)
			{
				accPositive += value;
			}
			else
			{
				accNegative += value;
			}
			if (note != null)
			{
				note_storage.Add(noteStorageId, value, note);
			}
		}

		public bool HasContextEntries()
		{
			return contextEntries.Count > 0;
		}
	}

	public class DailyReport
	{
		[Serialize]
		public int day;

		[Serialize]
		public List<ReportEntry> reportEntries = new List<ReportEntry>();

		private NoteStorage noteStorage => Instance.noteStorage;

		public DailyReport(ReportManager manager)
		{
			foreach (KeyValuePair<ReportType, ReportGroup> reportGroup in manager.ReportGroups)
			{
				reportEntries.Add(new ReportEntry(reportGroup.Key, noteStorage.GetNewNoteId(), null));
			}
		}

		public ReportEntry GetEntry(ReportType reportType)
		{
			for (int i = 0; i < reportEntries.Count; i++)
			{
				ReportEntry reportEntry = reportEntries[i];
				if (reportEntry.reportType == reportType)
				{
					return reportEntry;
				}
			}
			ReportEntry reportEntry2 = new ReportEntry(reportType, noteStorage.GetNewNoteId(), null);
			reportEntries.Add(reportEntry2);
			return reportEntry2;
		}

		public void AddData(ReportType reportType, float value, string note = null, string context = null)
		{
			ReportEntry entry = GetEntry(reportType);
			entry.AddData(noteStorage, value, note, context);
		}
	}

	public class NoteStorage
	{
		private class StringTable
		{
			private Dictionary<int, string> strings = new Dictionary<int, string>();

			public int AddString(string str)
			{
				HashedString hashedString = new HashedString(str);
				strings[hashedString.HashValue] = str;
				return hashedString.HashValue;
			}

			public string GetStringByHash(int hash)
			{
				string value = "";
				strings.TryGetValue(hash, out value);
				return value;
			}

			public void Serialize(BinaryWriter writer)
			{
				writer.Write(strings.Count);
				foreach (KeyValuePair<int, string> @string in strings)
				{
					writer.Write(@string.Value);
				}
			}

			public void Deserialize(BinaryReader reader)
			{
				int num = reader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					string str = reader.ReadString();
					AddString(str);
				}
			}
		}

		private class NoteEntries
		{
			[StructLayout(LayoutKind.Explicit)]
			public struct NoteEntry
			{
				[FieldOffset(0)]
				public int reportEntryId;

				[FieldOffset(4)]
				public int noteHash;

				[FieldOffset(8)]
				public float value;

				public NoteEntry(int report_entry_id, int note_hash, float value)
				{
					reportEntryId = report_entry_id;
					noteHash = note_hash;
					this.value = value;
				}

				public bool Matches(int report_entry_id, int note_hash, float value)
				{
					if (report_entry_id == reportEntryId)
					{
						if (note_hash == noteHash)
						{
							if (value > 0f == this.value > 0f)
							{
								return true;
							}
							return false;
						}
						return false;
					}
					return false;
				}
			}

			[StructLayout(LayoutKind.Explicit)]
			public struct NoteEntryArray
			{
				[FieldOffset(0)]
				public byte[] bytes;

				[FieldOffset(0)]
				public NoteEntry[] structs;

				public int SizeInStructs => bytes.Length / Marshal.SizeOf(typeof(NoteEntry));

				public int StructSizeInBytes => Marshal.SizeOf(typeof(NoteEntry));

				public NoteEntryArray(int size_in_structs)
				{
					int num = size_in_structs * Marshal.SizeOf(typeof(NoteEntry));
					structs = null;
					bytes = new byte[num];
				}

				public void Resize(int size_in_structs)
				{
					byte[] array = bytes;
					bytes = new byte[size_in_structs * Marshal.SizeOf(typeof(NoteEntry))];
					Buffer.BlockCopy(array, 0, bytes, 0, array.Length);
				}
			}

			private struct NoteStorageBlock
			{
				private int entryCount;

				private NoteEntryArray entries;

				public NoteStorageBlock(int capacity)
				{
					entries = new NoteEntryArray(capacity);
					entryCount = 0;
				}

				public void Add(int report_entry_id, float value, int note_id)
				{
					bool flag = false;
					for (int i = 0; i < entryCount; i++)
					{
						NoteEntry noteEntry = entries.structs[i];
						if (noteEntry.Matches(report_entry_id, note_id, value))
						{
							noteEntry.value += value;
							entries.structs[i] = noteEntry;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						if (entries.SizeInStructs <= entryCount)
						{
							entries.Resize(entries.SizeInStructs * 2);
						}
						entries.structs[entryCount++] = new NoteEntry(report_entry_id, note_id, value);
					}
				}

				public void IterateNotes(StringTable string_table, int report_entry_id, Action<ReportEntry.Note> callback)
				{
					for (int i = 0; i < entryCount; i++)
					{
						NoteEntry noteEntry = entries.structs[i];
						if (noteEntry.reportEntryId == report_entry_id)
						{
							string stringByHash = string_table.GetStringByHash(noteEntry.noteHash);
							ReportEntry.Note obj = new ReportEntry.Note(noteEntry.value, stringByHash);
							callback(obj);
						}
					}
				}

				public void Serialize(BinaryWriter writer)
				{
					writer.Write(entryCount);
					writer.Write(entries.bytes, 0, entries.StructSizeInBytes * entryCount);
				}

				public void Deserialize(BinaryReader reader)
				{
					entryCount = reader.ReadInt32();
					entries.bytes = reader.ReadBytes(entries.StructSizeInBytes * entryCount);
				}
			}

			private const int REPORT_IDS_PER_BLOCK = 100;

			private List<NoteStorageBlock> storageBlocks = new List<NoteStorageBlock>();

			public void Add(int report_entry_id, float value, int note_id)
			{
				int num = ReportEntryIdToStorageBlockIdx(report_entry_id);
				while (num >= storageBlocks.Count)
				{
					int capacity = 32;
					storageBlocks.Add(new NoteStorageBlock(capacity));
				}
				NoteStorageBlock value2 = storageBlocks[num];
				value2.Add(report_entry_id, value, note_id);
				storageBlocks[num] = value2;
			}

			public void Serialize(BinaryWriter writer)
			{
				writer.Write(storageBlocks.Count);
				foreach (NoteStorageBlock storageBlock in storageBlocks)
				{
					storageBlock.Serialize(writer);
				}
			}

			public void Deserialize(BinaryReader reader)
			{
				int num = reader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					NoteStorageBlock item = default(NoteStorageBlock);
					item.Deserialize(reader);
					storageBlocks.Add(item);
				}
			}

			private int ReportEntryIdToStorageBlockIdx(int report_entry_id)
			{
				return report_entry_id / 100;
			}

			public void IterateNotes(StringTable string_table, int report_entry_id, Action<ReportEntry.Note> callback)
			{
				int num = ReportEntryIdToStorageBlockIdx(report_entry_id);
				if (num < storageBlocks.Count)
				{
					storageBlocks[num].IterateNotes(string_table, report_entry_id, callback);
				}
			}
		}

		private const int SERIALIZATION_VERSION = 5;

		private int nextNoteId;

		private NoteEntries noteEntries;

		private StringTable stringTable;

		public NoteStorage()
		{
			noteEntries = new NoteEntries();
			stringTable = new StringTable();
		}

		public void Add(int report_entry_id, float value, string note)
		{
			int note_id = stringTable.AddString(note);
			noteEntries.Add(report_entry_id, value, note_id);
		}

		public int GetNewNoteId()
		{
			return ++nextNoteId;
		}

		public void IterateNotes(int report_entry_id, Action<ReportEntry.Note> callback)
		{
			noteEntries.IterateNotes(stringTable, report_entry_id, callback);
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(5);
			writer.Write(nextNoteId);
			stringTable.Serialize(writer);
			noteEntries.Serialize(writer);
		}

		public void Deserialize(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 5)
			{
				nextNoteId = reader.ReadInt32();
				stringTable.Deserialize(reader);
				noteEntries.Deserialize(reader);
			}
		}
	}

	[MyCmpAdd]
	private Notifier notifier;

	private NoteStorage noteStorage;

	public Dictionary<ReportType, ReportGroup> ReportGroups = new Dictionary<ReportType, ReportGroup>
	{
		{
			ReportType.CaloriesCreated,
			new ReportGroup((float v) => GameUtil.GetFormattedCalories(v, GameUtil.TimeSlice.None, true), true, 1, UI.ENDOFDAYREPORT.CALORIES_CREATED.NAME, UI.ENDOFDAYREPORT.CALORIES_CREATED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CALORIES_CREATED.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.StressDelta,
			new ReportGroup((float v) => GameUtil.GetFormattedPercent(v, GameUtil.TimeSlice.None), true, 1, UI.ENDOFDAYREPORT.STRESS_DELTA.NAME, UI.ENDOFDAYREPORT.STRESS_DELTA.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.STRESS_DELTA.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.DiseaseAdded,
			new ReportGroup(null, false, 1, UI.ENDOFDAYREPORT.DISEASE_ADDED.NAME, UI.ENDOFDAYREPORT.DISEASE_ADDED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.DISEASE_ADDED.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.DiseaseStatus,
			new ReportGroup((float v) => GameUtil.GetFormattedDiseaseAmount((int)v), true, 1, UI.ENDOFDAYREPORT.DISEASE_STATUS.NAME, UI.ENDOFDAYREPORT.DISEASE_STATUS.TOOLTIP, "", ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.TimeSpent,
			new ReportGroup((float v) => GameUtil.GetFormattedTime(v), true, 1, UI.ENDOFDAYREPORT.TIME_SPENT.NAME, UI.ENDOFDAYREPORT.TIME_SPENT.POSITIVE_TOOLTIP, "", ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.TravelTime,
			new ReportGroup((float v) => GameUtil.GetFormattedTime(v), true, 1, UI.ENDOFDAYREPORT.TRAVEL_TIME.NAME, UI.ENDOFDAYREPORT.TRAVEL_TIME.POSITIVE_TOOLTIP, "", ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.IdleTime,
			new ReportGroup((float v) => GameUtil.GetFormattedTime(v), true, 2, UI.ENDOFDAYREPORT.IDLE_TIME.NAME, UI.ENDOFDAYREPORT.IDLE_TIME.POSITIVE_TOOLTIP, "", ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.ChoreStatus,
			new ReportGroup(null, true, 1, UI.ENDOFDAYREPORT.CHORE_STATUS.NAME, UI.ENDOFDAYREPORT.CHORE_STATUS.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CHORE_STATUS.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.OxygenCreated,
			new ReportGroup((float v) => GameUtil.GetFormattedMass(v, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), true, 2, UI.ENDOFDAYREPORT.OXYGEN_CREATED.NAME, UI.ENDOFDAYREPORT.OXYGEN_CREATED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.OXYGEN_CREATED.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.EnergyCreated,
			new ReportGroup(GameUtil.GetFormattedRoundedJoules, true, 2, UI.ENDOFDAYREPORT.ENERGY_USAGE.NAME, UI.ENDOFDAYREPORT.ENERGY_USAGE.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.ENERGY_USAGE.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.EnergyWasted,
			new ReportGroup(GameUtil.GetFormattedRoundedJoules, true, 2, UI.ENDOFDAYREPORT.ENERGY_WASTED.NAME, UI.ENDOFDAYREPORT.ENERGY_WASTED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.ENERGY_WASTED.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.LevelUp,
			new ReportGroup(null, false, 2, UI.ENDOFDAYREPORT.LEVEL_UP.NAME, UI.ENDOFDAYREPORT.LEVEL_UP.TOOLTIP, "", ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.ToiletIncident,
			new ReportGroup(null, false, 2, UI.ENDOFDAYREPORT.TOILET_INCIDENT.NAME, UI.ENDOFDAYREPORT.TOILET_INCIDENT.TOOLTIP, "", ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.ContaminatedOxygenToilet,
			new ReportGroup((float v) => GameUtil.GetFormattedMass(v, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), false, 2, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.NAME, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.ContaminatedOxygenSublimation,
			new ReportGroup((float v) => GameUtil.GetFormattedMass(v, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), false, 2, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.NAME, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		}
	};

	[Serialize]
	private List<DailyReport> dailyReports = new List<DailyReport>();

	[Serialize]
	private DailyReport todaysReport;

	[Serialize]
	private byte[] noteStorageBytes;

	[CompilerGenerated]
	private static FormattingFn _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static FormattingFn _003C_003Ef__mg_0024cache1;

	public List<DailyReport> reports => dailyReports;

	public static ReportManager Instance
	{
		get;
		private set;
	}

	public DailyReport TodaysReport => todaysReport;

	public DailyReport YesterdaysReport
	{
		get
		{
			if (dailyReports.Count > 1)
			{
				return dailyReports[dailyReports.Count - 1];
			}
			return null;
		}
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		Subscribe(Game.Instance.gameObject, -1917495436, OnSaveGameReady);
		noteStorage = new NoteStorage();
	}

	protected override void OnCleanUp()
	{
		Instance = null;
	}

	[OnSerializing]
	private void OnSerializing()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter writer = new BinaryWriter(memoryStream);
		noteStorage.Serialize(writer);
		noteStorageBytes = memoryStream.GetBuffer();
	}

	[OnSerialized]
	private void OnSerialized()
	{
		noteStorageBytes = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (noteStorageBytes != null)
		{
			noteStorage.Deserialize(new BinaryReader(new MemoryStream(noteStorageBytes)));
			noteStorageBytes = null;
		}
	}

	private void OnSaveGameReady(object data)
	{
		Subscribe(GameClock.Instance.gameObject, -722330267, OnNightTime);
		if (todaysReport == null)
		{
			todaysReport = new DailyReport(this);
			todaysReport.day = GameUtil.GetCurrentCycle();
		}
	}

	public void ReportValue(ReportType reportType, float value, string note = null, string context = null)
	{
		TodaysReport.AddData(reportType, value, note, context);
	}

	private void OnNightTime(object data)
	{
		dailyReports.Add(todaysReport);
		int day = todaysReport.day;
		Notification notification = new Notification(string.Format(UI.ENDOFDAYREPORT.NOTIFICATION_TITLE, day), NotificationType.Good, HashedString.Invalid, (List<Notification> n, object d) => string.Format(UI.ENDOFDAYREPORT.NOTIFICATION_TOOLTIP, day), null, true, 0f, delegate
		{
			ManagementMenu.Instance.OpenReports(day);
		}, null);
		if ((UnityEngine.Object)notifier == (UnityEngine.Object)null)
		{
			Debug.LogError("Cant notify, null notifier", null);
		}
		else
		{
			notifier.Add(notification, "");
		}
		todaysReport = new DailyReport(this);
		todaysReport.day = GameUtil.GetCurrentCycle() + 1;
		foreach (Chore chore in GlobalChoreProvider.Instance.chores)
		{
			if (chore.addToDailyReport)
			{
				ReportValue(ReportType.ChoreStatus, 1f, chore.choreType.Name, GameUtil.GetChoreName(chore, chore.target));
			}
		}
	}

	public DailyReport FindReport(int day)
	{
		foreach (DailyReport dailyReport in dailyReports)
		{
			if (dailyReport.day == day)
			{
				return dailyReport;
			}
		}
		if (todaysReport.day != day)
		{
			return null;
		}
		return todaysReport;
	}
}
