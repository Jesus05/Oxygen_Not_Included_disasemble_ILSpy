using KSerialization;
using System.Collections.Generic;
using UnityEngine;

public class ConversationMonitor : GameStateMachine<ConversationMonitor, ConversationMonitor.Instance>
{
	public class Def : BaseDef
	{
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public new class Instance : GameInstance
	{
		[Serialize]
		private Queue<string> recentTopics;

		[Serialize]
		private List<string> favouriteTopics;

		private List<string> personalTopics;

		private static readonly List<string> randomTopics = new List<string>
		{
			"Headquarters"
		};

		public Instance(IStateMachineTarget master, Def def)
			: base(master)
		{
			recentTopics = new Queue<string>();
			favouriteTopics = new List<string>
			{
				randomTopics[Random.Range(0, randomTopics.Count)]
			};
			personalTopics = new List<string>();
		}

		public string GetATopic()
		{
			int max = recentTopics.Count + favouriteTopics.Count * 2 + personalTopics.Count;
			int num = Random.Range(0, max);
			if (num < recentTopics.Count)
			{
				return recentTopics.Dequeue();
			}
			num -= recentTopics.Count;
			if (num < favouriteTopics.Count)
			{
				return favouriteTopics[num];
			}
			num -= favouriteTopics.Count;
			if (num < favouriteTopics.Count)
			{
				return favouriteTopics[num];
			}
			num -= favouriteTopics.Count;
			if (num < personalTopics.Count)
			{
				return personalTopics[num];
			}
			return string.Empty;
		}

		public void OnTopicDiscovered(object data)
		{
			string item = (string)data;
			if (!recentTopics.Contains(item))
			{
				recentTopics.Enqueue(item);
				if (recentTopics.Count > 5)
				{
					string topic = recentTopics.Dequeue();
					TryMakeFavouriteTopic(topic);
				}
			}
		}

		public void OnTopicDiscussed(object data)
		{
			string data2 = (string)data;
			if (Random.value < 0.333333343f)
			{
				OnTopicDiscovered(data2);
			}
		}

		private void TryMakeFavouriteTopic(string topic)
		{
			if (Random.value < 0.0333333351f)
			{
				if (favouriteTopics.Count < 5)
				{
					favouriteTopics.Add(topic);
				}
				else
				{
					favouriteTopics[Random.Range(0, favouriteTopics.Count)] = topic;
				}
			}
		}
	}

	private const int MAX_RECENT_TOPICS = 5;

	private const int MAX_FAVOURITE_TOPICS = 5;

	private const float FAVOURITE_CHANCE = 0.0333333351f;

	private const float LEARN_CHANCE = 0.333333343f;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.EventHandler(GameHashes.TopicDiscussed, delegate(Instance smi, object obj)
		{
			smi.OnTopicDiscussed(obj);
		}).EventHandler(GameHashes.TopicDiscovered, delegate(Instance smi, object obj)
		{
			smi.OnTopicDiscovered(obj);
		});
	}
}
