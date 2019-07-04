using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class ColonyAchievement : Resource
	{
		public string description;

		public bool isVictoryCondition;

		public string messageTitle;

		public string messageBody;

		public string comicDataName;

		public string videoDataName;

		public List<ColonyAchievementRequirement> requirementChecklist = new List<ColonyAchievementRequirement>();

		public Func<GameObject> GetSuccessTargetEntity;

		public string victoryNISSnapshot
		{
			get;
			private set;
		}

		public ColonyAchievement(string Id, string Name, string description, bool isVictoryCondition, List<ColonyAchievementRequirement> requirementChecklist, string messageTitle = "", string messageBody = "", string comicDataName = null, string videoDataName = "", Func<GameObject> GetSuccessTargetEntity = null, string victorySnapshot = "")
			: base(Id, Name)
		{
			base.Id = Id;
			base.Name = Name;
			this.description = description;
			this.isVictoryCondition = isVictoryCondition;
			this.requirementChecklist = requirementChecklist;
			this.messageTitle = messageTitle;
			this.messageBody = messageBody;
			this.comicDataName = comicDataName;
			this.videoDataName = videoDataName;
			this.GetSuccessTargetEntity = GetSuccessTargetEntity;
			victoryNISSnapshot = ((!string.IsNullOrEmpty(victorySnapshot)) ? victorySnapshot : AudioMixerSnapshots.Get().VictoryNISGenericSnapshot);
		}
	}
}
