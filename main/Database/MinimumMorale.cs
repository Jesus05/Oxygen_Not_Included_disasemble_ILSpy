using Klei.AI;
using STRINGS;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Database
{
	public class MinimumMorale : VictoryColonyAchievementRequirement
	{
		private int minimumMorale;

		public MinimumMorale(int minimumMorale = 16)
		{
			this.minimumMorale = minimumMorale;
		}

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_MORALE, minimumMorale);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_MORALE_DESCRIPTION, minimumMorale);
		}

		public override bool Success()
		{
			bool flag = true;
			IEnumerator enumerator = Components.MinionAssignablesProxy.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MinionAssignablesProxy minionAssignablesProxy = (MinionAssignablesProxy)enumerator.Current;
					GameObject targetGameObject = minionAssignablesProxy.GetTargetGameObject();
					if ((UnityEngine.Object)targetGameObject != (UnityEngine.Object)null && !targetGameObject.HasTag(GameTags.Dead))
					{
						AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(targetGameObject.GetComponent<MinionModifiers>());
						flag = (attributeInstance != null && attributeInstance.GetTotalValue() >= (float)minimumMorale && flag);
					}
				}
				return flag;
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(minimumMorale);
		}

		public override void Deserialize(IReader reader)
		{
			minimumMorale = reader.ReadInt32();
		}
	}
}
