using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConversationManager : KMonoBehaviour, ISim200ms
{
	public class Tuning : TuningData<Tuning>
	{
		public float cyclesBeforeFirstConversation;

		public float maxDistance;

		public int maxDupesPerConvo;

		public float minionCooldownTime;

		public float speakTime;

		public float delayBetweenUtterances;

		public float delayBeforeStart;

		public int maxUtterances;
	}

	public class StartedTalkingEvent
	{
		public GameObject talker;

		public string anim;
	}

	private List<Conversation> activeSetups;

	private Dictionary<MinionIdentity, float> lastConvoTimeByMinion;

	private Dictionary<MinionIdentity, Conversation> setupsByMinion = new Dictionary<MinionIdentity, Conversation>();

	private List<Type> convoTypes = new List<Type>
	{
		typeof(RecentThingConversation),
		typeof(AmountStateConversation),
		typeof(CurrentJobConversation)
	};

	private static readonly List<Tag> invalidConvoTags = new List<Tag>
	{
		GameTags.Asleep,
		GameTags.HoldingBreath,
		GameTags.Dead
	};

	protected override void OnPrefabInit()
	{
		activeSetups = new List<Conversation>();
		lastConvoTimeByMinion = new Dictionary<MinionIdentity, float>();
	}

	public void Sim200ms(float dt)
	{
		for (int num = activeSetups.Count - 1; num >= 0; num--)
		{
			Conversation conversation = activeSetups[num];
			for (int num2 = conversation.minions.Count - 1; num2 >= 0; num2--)
			{
				if (!ValidMinionTags(conversation.minions[num2]) || !MinionCloseEnoughToConvo(conversation.minions[num2], conversation))
				{
					conversation.minions.RemoveAt(num2);
				}
				else
				{
					setupsByMinion[conversation.minions[num2]] = conversation;
				}
			}
			if (conversation.minions.Count <= 1)
			{
				activeSetups.RemoveAt(num);
			}
			else
			{
				bool flag = true;
				if (conversation.numUtterances == 0 && GameClock.Instance.GetTime() > conversation.lastTalkedTime + TuningData<Tuning>.Get().delayBeforeStart)
				{
					MinionIdentity minionIdentity = conversation.minions[UnityEngine.Random.Range(0, conversation.minions.Count)];
					conversation.conversationType.NewTarget(minionIdentity);
					flag = DoTalking(conversation, minionIdentity);
				}
				else if (conversation.numUtterances > 0 && conversation.numUtterances < TuningData<Tuning>.Get().maxUtterances && GameClock.Instance.GetTime() > conversation.lastTalkedTime + TuningData<Tuning>.Get().speakTime + TuningData<Tuning>.Get().delayBetweenUtterances)
				{
					int num3 = conversation.minions.IndexOf(conversation.lastTalked);
					int index = (num3 + UnityEngine.Random.Range(1, conversation.minions.Count)) % conversation.minions.Count;
					MinionIdentity new_speaker = conversation.minions[index];
					flag = DoTalking(conversation, new_speaker);
				}
				else if (conversation.numUtterances >= TuningData<Tuning>.Get().maxUtterances)
				{
					flag = false;
				}
				if (!flag)
				{
					activeSetups.RemoveAt(num);
				}
			}
		}
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			if (ValidMinionTags(item) && !setupsByMinion.ContainsKey(item) && !MinionOnCooldown(item))
			{
				foreach (MinionIdentity item2 in Components.LiveMinionIdentities.Items)
				{
					if (!((UnityEngine.Object)item2 == (UnityEngine.Object)item) && ValidMinionTags(item2))
					{
						if (setupsByMinion.ContainsKey(item2))
						{
							Conversation conversation2 = setupsByMinion[item2];
							if (conversation2.minions.Count < TuningData<Tuning>.Get().maxDupesPerConvo)
							{
								Vector3 centroid = GetCentroid(conversation2);
								float magnitude = (centroid - item.transform.GetPosition()).magnitude;
								if (magnitude < TuningData<Tuning>.Get().maxDistance * 0.5f)
								{
									conversation2.minions.Add(item);
									setupsByMinion[item] = conversation2;
									break;
								}
							}
						}
						else if (!MinionOnCooldown(item2))
						{
							float magnitude2 = (item2.transform.GetPosition() - item.transform.GetPosition()).magnitude;
							if (magnitude2 < TuningData<Tuning>.Get().maxDistance)
							{
								Conversation conversation3 = new Conversation();
								conversation3.minions.Add(item);
								conversation3.minions.Add(item2);
								Type type = convoTypes[UnityEngine.Random.Range(0, convoTypes.Count)];
								conversation3.conversationType = (ConversationType)Activator.CreateInstance(type);
								conversation3.lastTalkedTime = GameClock.Instance.GetTime();
								activeSetups.Add(conversation3);
								setupsByMinion[item] = conversation3;
								setupsByMinion[item2] = conversation3;
								break;
							}
						}
					}
				}
			}
		}
		setupsByMinion.Clear();
	}

	private bool DoTalking(Conversation setup, MinionIdentity new_speaker)
	{
		if ((UnityEngine.Object)setup.lastTalked != (UnityEngine.Object)null)
		{
			setup.lastTalked.Trigger(25860745, setup.lastTalked.gameObject);
		}
		Conversation.Topic nextTopic = setup.conversationType.GetNextTopic(new_speaker, setup.lastTopic);
		if (nextTopic == null || nextTopic.mode == Conversation.ModeType.End || nextTopic.mode == Conversation.ModeType.Segue)
		{
			return false;
		}
		Thought thoughtForTopic = GetThoughtForTopic(setup, nextTopic);
		if (thoughtForTopic == null)
		{
			return false;
		}
		setup.lastTopic = nextTopic;
		setup.lastTalked = new_speaker;
		setup.lastTalkedTime = GameClock.Instance.GetTime();
		lastConvoTimeByMinion[setup.lastTalked] = GameClock.Instance.GetTime();
		ThoughtGraph.Instance sMI = setup.lastTalked.GetSMI<ThoughtGraph.Instance>();
		sMI.AddThought(thoughtForTopic);
		Effects component = setup.lastTalked.GetComponent<Effects>();
		component.Add("GoodConversation", true);
		Conversation.Mode mode = Conversation.Topic.Modes[(int)nextTopic.mode];
		StartedTalkingEvent startedTalkingEvent = new StartedTalkingEvent();
		startedTalkingEvent.talker = new_speaker.gameObject;
		startedTalkingEvent.anim = mode.anim;
		StartedTalkingEvent data = startedTalkingEvent;
		foreach (MinionIdentity minion in setup.minions)
		{
			DebugUtil.DevAssert(minion, "minion in setup.minions was null");
			if ((bool)minion)
			{
				minion.Trigger(-594200555, data);
			}
		}
		setup.numUtterances++;
		return true;
	}

	private Vector3 GetCentroid(Conversation setup)
	{
		Vector3 a = Vector3.zero;
		foreach (MinionIdentity minion in setup.minions)
		{
			if (!((UnityEngine.Object)minion == (UnityEngine.Object)null))
			{
				a += minion.transform.GetPosition();
			}
		}
		return a / (float)setup.minions.Count;
	}

	private Thought GetThoughtForTopic(Conversation setup, Conversation.Topic topic)
	{
		DebugUtil.DevAssert(!string.IsNullOrEmpty(topic.topic));
		if (string.IsNullOrEmpty(topic.topic))
		{
			return null;
		}
		Sprite sprite = setup.conversationType.GetSprite(topic.topic);
		if ((UnityEngine.Object)sprite != (UnityEngine.Object)null)
		{
			Conversation.Mode mode = Conversation.Topic.Modes[(int)topic.mode];
			return new Thought("Topic_" + topic.topic, null, sprite, mode.icon, mode.voice, "bubble_chatter", mode.mouth, DUPLICANTS.THOUGHTS.CONVERSATION.TOOLTIP, true, TuningData<Tuning>.Get().speakTime);
		}
		DebugUtil.DevAssert((UnityEngine.Object)sprite != (UnityEngine.Object)null, "Couldn't find a sprite for conversation topic:", topic.topic);
		return null;
	}

	private bool ValidMinionTags(MinionIdentity minion)
	{
		if ((UnityEngine.Object)minion == (UnityEngine.Object)null)
		{
			return false;
		}
		KPrefabID component = minion.GetComponent<KPrefabID>();
		return !component.HasAnyTags(invalidConvoTags);
	}

	private bool MinionCloseEnoughToConvo(MinionIdentity minion, Conversation setup)
	{
		Vector3 centroid = GetCentroid(setup);
		float magnitude = (centroid - minion.transform.GetPosition()).magnitude;
		return magnitude < TuningData<Tuning>.Get().maxDistance * 0.5f;
	}

	private bool MinionOnCooldown(MinionIdentity minion)
	{
		KPrefabID component = minion.GetComponent<KPrefabID>();
		return !component.HasTag(GameTags.AlwaysConverse) && ((lastConvoTimeByMinion.ContainsKey(minion) && GameClock.Instance.GetTime() < lastConvoTimeByMinion[minion] + TuningData<Tuning>.Get().minionCooldownTime) || GameClock.Instance.GetTime() / 600f < TuningData<Tuning>.Get().cyclesBeforeFirstConversation);
	}
}
