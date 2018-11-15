using System.Collections.Generic;

public class Conversation
{
	public enum ModeType
	{
		Query,
		Statement,
		Agreement,
		Disagreement,
		Musing,
		Satisfaction,
		Nominal,
		Dissatisfaction,
		Stressing,
		Segue,
		End
	}

	public class Mode
	{
		public ModeType type;

		public string voice;

		public string mouth;

		public string anim;

		public string icon;

		public bool newTopic;

		public Mode(ModeType type, string voice, string icon, string mouth, string anim, bool newTopic = false)
		{
			this.type = type;
			this.voice = voice;
			this.mouth = mouth;
			this.anim = anim;
			this.icon = icon;
			this.newTopic = newTopic;
		}
	}

	public class Topic
	{
		public static List<Mode> modeList = new List<Mode>
		{
			new Mode(ModeType.Query, "conversation_question", "mode_query", SpeechMonitor.PREFIX_HAPPY, "happy", false),
			new Mode(ModeType.Statement, "conversation_answer", "mode_statement", SpeechMonitor.PREFIX_HAPPY, "happy", false),
			new Mode(ModeType.Agreement, "conversation_answer", "mode_agreement", SpeechMonitor.PREFIX_HAPPY, "happy", false),
			new Mode(ModeType.Disagreement, "conversation_answer", "mode_disagreement", SpeechMonitor.PREFIX_SAD, "unhappy", false),
			new Mode(ModeType.Musing, "conversation_short", "mode_musing", SpeechMonitor.PREFIX_HAPPY, "happy", false),
			new Mode(ModeType.Satisfaction, "conversation_short", "mode_satisfaction", SpeechMonitor.PREFIX_HAPPY, "happy", false),
			new Mode(ModeType.Nominal, "conversation_short", "mode_nominal", SpeechMonitor.PREFIX_HAPPY, "happy", false),
			new Mode(ModeType.Dissatisfaction, "conversation_short", "mode_dissatisfaction", SpeechMonitor.PREFIX_SAD, "unhappy", false),
			new Mode(ModeType.Stressing, "conversation_short", "mode_stressing", SpeechMonitor.PREFIX_SAD, "unhappy", false),
			new Mode(ModeType.Segue, "conversation_question", "mode_segue", SpeechMonitor.PREFIX_HAPPY, "happy", true)
		};

		private static Dictionary<int, Mode> _modes;

		public string topic;

		public ModeType mode;

		public static Dictionary<int, Mode> Modes
		{
			get
			{
				if (_modes == null)
				{
					_modes = new Dictionary<int, Mode>();
					foreach (Mode mode2 in modeList)
					{
						_modes[(int)mode2.type] = mode2;
					}
				}
				return _modes;
			}
		}

		public Topic(string topic, ModeType mode)
		{
			this.topic = topic;
			this.mode = mode;
		}
	}

	public List<MinionIdentity> minions = new List<MinionIdentity>();

	public MinionIdentity lastTalked;

	public ConversationType conversationType;

	public float lastTalkedTime;

	public Topic lastTopic;

	public int numUtterances;
}
