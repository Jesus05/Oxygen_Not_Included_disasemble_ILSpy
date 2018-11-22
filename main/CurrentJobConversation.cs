using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class CurrentJobConversation : ConversationType
{
	public static Dictionary<Conversation.ModeType, List<Conversation.ModeType>> transitions = new Dictionary<Conversation.ModeType, List<Conversation.ModeType>>
	{
		{
			Conversation.ModeType.Query,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Statement
			}
		},
		{
			Conversation.ModeType.Satisfaction,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Agreement
			}
		},
		{
			Conversation.ModeType.Nominal,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Musing
			}
		},
		{
			Conversation.ModeType.Dissatisfaction,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Disagreement
			}
		},
		{
			Conversation.ModeType.Stressing,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Disagreement
			}
		},
		{
			Conversation.ModeType.Agreement,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Query,
				Conversation.ModeType.End
			}
		},
		{
			Conversation.ModeType.Disagreement,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Query,
				Conversation.ModeType.End
			}
		},
		{
			Conversation.ModeType.Musing,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Query,
				Conversation.ModeType.End
			}
		}
	};

	public CurrentJobConversation()
	{
		id = "CurrentJobConversation";
	}

	public override void NewTarget(MinionIdentity speaker)
	{
		target = "hows_role";
	}

	public override Conversation.Topic GetNextTopic(MinionIdentity speaker, Conversation.Topic lastTopic)
	{
		if (lastTopic != null)
		{
			List<Conversation.ModeType> list = transitions[lastTopic.mode];
			Conversation.ModeType modeType = list[Random.Range(0, list.Count)];
			if (modeType != Conversation.ModeType.Statement)
			{
				return new Conversation.Topic(target, modeType);
			}
			target = GetRoleForSpeaker(speaker);
			Conversation.ModeType modeForRole = GetModeForRole(speaker, target);
			return new Conversation.Topic(target, modeForRole);
		}
		return new Conversation.Topic(target, Conversation.ModeType.Query);
	}

	public override Sprite GetSprite(string topic)
	{
		if (!(topic == "hows_role"))
		{
			if (!RoleManager.roleHatIndex.ContainsKey(topic))
			{
				return null;
			}
			return Assets.GetSprite(RoleManager.roleHatIndex[topic]);
		}
		return Assets.GetSprite("crew_state_role");
	}

	private Conversation.ModeType GetModeForRole(MinionIdentity speaker, string roleId)
	{
		MinionResume component = speaker.GetComponent<MinionResume>();
		RoleConfig role = Game.Instance.roleManager.GetRole(roleId);
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(speaker);
		AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(speaker);
		if (!(component.AptitudeByRoleGroup[role.roleGroup] > 0f))
		{
			if (!(attributeInstance.GetTotalValue() < attributeInstance2.GetTotalValue()))
			{
				if (!(roleId == "NoRole"))
				{
					return Conversation.ModeType.Nominal;
				}
				return Conversation.ModeType.Dissatisfaction;
			}
			return Conversation.ModeType.Stressing;
		}
		return Conversation.ModeType.Satisfaction;
	}

	private string GetRoleForSpeaker(MinionIdentity speaker)
	{
		MinionResume component = speaker.GetComponent<MinionResume>();
		return component.CurrentRole;
	}
}
