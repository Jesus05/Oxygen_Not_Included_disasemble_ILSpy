using System;
using System.IO;

namespace Database
{
	public class TuneUpGenerator : ColonyAchievementRequirement
	{
		private float numChoreseToComplete;

		public TuneUpGenerator(float numChoreseToComplete)
		{
			this.numChoreseToComplete = numChoreseToComplete;
		}

		public override bool Success()
		{
			float num = 0f;
			ReportManager.DailyReport todaysReport = ReportManager.Instance.TodaysReport;
			ReportManager.ReportEntry entry = todaysReport.GetEntry(ReportManager.ReportType.ChoreStatus);
			for (int i = 0; i < entry.contextEntries.Count; i++)
			{
				ReportManager.ReportEntry reportEntry = entry.contextEntries[i];
				if (reportEntry.context == Db.Get().ChoreTypes.PowerTinker.Name)
				{
					num += reportEntry.Negative;
				}
			}
			for (int j = 0; j < ReportManager.Instance.reports.Count; j++)
			{
				for (int k = 0; k < ReportManager.Instance.reports[j].GetEntry(ReportManager.ReportType.ChoreStatus).contextEntries.Count; k++)
				{
					ReportManager.ReportEntry reportEntry2 = ReportManager.Instance.reports[j].GetEntry(ReportManager.ReportType.ChoreStatus).contextEntries[k];
					if (reportEntry2.context == Db.Get().ChoreTypes.PowerTinker.Name)
					{
						num += reportEntry2.Negative;
					}
				}
			}
			return Math.Abs(num) >= numChoreseToComplete;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(numChoreseToComplete);
		}

		public override void Deserialize(IReader reader)
		{
			numChoreseToComplete = reader.ReadSingle();
		}
	}
}
