using System;
using System.Collections.Generic;

namespace Database
{
	public class ColonyAchievement : Resource
	{
		public string description;

		public bool isVictoryCondition;

		public string messageTitle;

		public string messageBody;

		public string shortVideoName;

		public string loopVideoName;

		public List<ColonyAchievementRequirement> requirementChecklist = new List<ColonyAchievementRequirement>();

		public Action<KMonoBehaviour> victorySequence;

		public string victoryNISSnapshot
		{
			get;
			private set;
		}

		public ColonyAchievement(string Id, string Name, string description, bool isVictoryCondition, List<ColonyAchievementRequirement> requirementChecklist, string messageTitle = "", string messageBody = "", string videoDataName = "", string victoryLoopVideo = "", Action<KMonoBehaviour> VictorySequence = null, string victorySnapshot = "")
			: base(Id, Name)
		{
			base.Id = Id;
			base.Name = Name;
			this.description = description;
			this.isVictoryCondition = isVictoryCondition;
			this.requirementChecklist = requirementChecklist;
			this.messageTitle = messageTitle;
			this.messageBody = messageBody;
			shortVideoName = videoDataName;
			loopVideoName = victoryLoopVideo;
			victorySequence = VictorySequence;
			victoryNISSnapshot = ((!string.IsNullOrEmpty(victorySnapshot)) ? victorySnapshot : AudioMixerSnapshots.Get().VictoryNISGenericSnapshot);
		}
	}
}
