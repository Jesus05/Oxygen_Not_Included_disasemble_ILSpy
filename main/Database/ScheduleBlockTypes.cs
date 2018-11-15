using STRINGS;
using UnityEngine;

namespace Database
{
	public class ScheduleBlockTypes : ResourceSet<ScheduleBlockType>
	{
		public ScheduleBlockType Sleep;

		public ScheduleBlockType Eat;

		public ScheduleBlockType Work;

		public ScheduleBlockType Hygiene;

		public ScheduleBlockType Recreation;

		public ScheduleBlockTypes(ResourceSet parent)
			: base("ScheduleBlockTypes", parent)
		{
			Sleep = Add(new ScheduleBlockType("Sleep", this, UI.SCHEDULEBLOCKTYPES.SLEEP.NAME, UI.SCHEDULEBLOCKTYPES.SLEEP.DESCRIPTION, new Color(0.9843137f, 0.992156863f, 0.270588249f)));
			Eat = Add(new ScheduleBlockType("Eat", this, UI.SCHEDULEBLOCKTYPES.EAT.NAME, UI.SCHEDULEBLOCKTYPES.EAT.DESCRIPTION, new Color(0.807843149f, 0.5294118f, 0.113725491f)));
			Work = Add(new ScheduleBlockType("Work", this, UI.SCHEDULEBLOCKTYPES.WORK.NAME, UI.SCHEDULEBLOCKTYPES.WORK.DESCRIPTION, new Color(0.9372549f, 0.129411772f, 0.129411772f)));
			Hygiene = Add(new ScheduleBlockType("Hygiene", this, UI.SCHEDULEBLOCKTYPES.HYGIENE.NAME, UI.SCHEDULEBLOCKTYPES.HYGIENE.DESCRIPTION, new Color(0.458823532f, 0.1764706f, 0.345098048f)));
			Recreation = Add(new ScheduleBlockType("Recreation", this, UI.SCHEDULEBLOCKTYPES.RECREATION.NAME, UI.SCHEDULEBLOCKTYPES.RECREATION.DESCRIPTION, new Color(0.458823532f, 0.372549027f, 0.1882353f)));
		}
	}
}
