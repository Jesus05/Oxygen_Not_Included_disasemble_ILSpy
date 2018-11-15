using KSerialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class AttributeLevels : KMonoBehaviour, ISaveLoadable
	{
		[Serializable]
		public struct LevelSaveLoad
		{
			public string attributeId;

			public float experience;

			public int level;
		}

		private List<AttributeLevel> levels = new List<AttributeLevel>();

		[Serialize]
		private LevelSaveLoad[] saveLoadLevels = new LevelSaveLoad[0];

		public LevelSaveLoad[] SaveLoadLevels
		{
			get
			{
				return saveLoadLevels;
			}
			set
			{
				saveLoadLevels = value;
			}
		}

		public IEnumerator<AttributeLevel> GetEnumerator()
		{
			return levels.GetEnumerator();
		}

		protected override void OnPrefabInit()
		{
			foreach (AttributeInstance attribute in this.GetAttributes())
			{
				if (attribute.Attribute.IsTrainable)
				{
					AttributeLevel attributeLevel = new AttributeLevel(attribute);
					levels.Add(attributeLevel);
					attributeLevel.Apply(this);
				}
			}
		}

		[OnSerializing]
		public void OnSerializing()
		{
			saveLoadLevels = new LevelSaveLoad[levels.Count];
			for (int i = 0; i < levels.Count; i++)
			{
				saveLoadLevels[i].attributeId = levels[i].attribute.Attribute.Id;
				saveLoadLevels[i].experience = levels[i].experience;
				saveLoadLevels[i].level = levels[i].level;
			}
		}

		[OnDeserialized]
		public void OnDeserialized()
		{
			LevelSaveLoad[] array = saveLoadLevels;
			for (int i = 0; i < array.Length; i++)
			{
				LevelSaveLoad levelSaveLoad = array[i];
				SetExperience(levelSaveLoad.attributeId, levelSaveLoad.experience);
				SetLevel(levelSaveLoad.attributeId, levelSaveLoad.level);
			}
		}

		public int GetLevel(Attribute attribute)
		{
			foreach (AttributeLevel level in levels)
			{
				if (attribute == level.attribute.Attribute)
				{
					return level.GetLevel();
				}
			}
			return 1;
		}

		public AttributeLevel GetAttributeLevel(string attribute_id)
		{
			foreach (AttributeLevel level in levels)
			{
				if (level.attribute.Attribute.Id == attribute_id)
				{
					return level;
				}
			}
			return null;
		}

		public bool AddExperience(string attribute_id, float time_spent, float multiplier)
		{
			AttributeLevel attributeLevel = GetAttributeLevel(attribute_id);
			if (attributeLevel == null)
			{
				Debug.LogWarning(attribute_id + " has no level.", null);
				return false;
			}
			time_spent *= multiplier;
			AttributeConverterInstance attributeConverterInstance = Db.Get().AttributeConverters.TrainingSpeed.Lookup(this);
			if (attributeConverterInstance != null)
			{
				float num = attributeConverterInstance.Evaluate();
				time_spent += time_spent * num;
			}
			bool result = attributeLevel.AddExperience(this, time_spent);
			attributeLevel.Apply(this);
			return result;
		}

		public void SetLevel(string attribute_id, int level)
		{
			AttributeLevel attributeLevel = GetAttributeLevel(attribute_id);
			if (attributeLevel != null)
			{
				attributeLevel.SetLevel(level);
				attributeLevel.Apply(this);
			}
		}

		public void SetExperience(string attribute_id, float experience)
		{
			AttributeLevel attributeLevel = GetAttributeLevel(attribute_id);
			if (attributeLevel != null)
			{
				attributeLevel.SetExperience(experience);
				attributeLevel.Apply(this);
			}
		}

		public float GetPercentComplete(string attribute_id)
		{
			AttributeLevel attributeLevel = GetAttributeLevel(attribute_id);
			return attributeLevel.GetPercentComplete();
		}

		public int GetMaxLevel()
		{
			int num = 0;
			using (IEnumerator<AttributeLevel> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AttributeLevel current = enumerator.Current;
					if (current.GetLevel() > num)
					{
						num = current.GetLevel();
					}
				}
				return num;
			}
		}
	}
}
