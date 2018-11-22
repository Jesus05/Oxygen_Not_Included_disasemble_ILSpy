using Klei.AI;
using STRINGS;
using UnityEngine;

public class RoomType : Resource
{
	public enum RoomIdentificationResult
	{
		all_satisfied,
		primary_satisfied,
		primary_unsatisfied
	}

	public string tooltip
	{
		get;
		private set;
	}

	public string effect
	{
		get;
		private set;
	}

	public RoomConstraints.Constraint primary_constraint
	{
		get;
		private set;
	}

	public RoomConstraints.Constraint[] additional_constraints
	{
		get;
		private set;
	}

	public int priority
	{
		get;
		private set;
	}

	public bool single_assignee
	{
		get;
		private set;
	}

	public RoomDetails.Detail[] display_details
	{
		get;
		private set;
	}

	public bool priority_building_use
	{
		get;
		private set;
	}

	public RoomTypeCategory category
	{
		get;
		private set;
	}

	public RoomType[] upgrade_paths
	{
		get;
		private set;
	}

	public string[] effects
	{
		get;
		private set;
	}

	public RoomType(string id, string name, string tooltip, string effect, RoomTypeCategory category, RoomConstraints.Constraint primary_constraint, RoomConstraints.Constraint[] additional_constraints, RoomDetails.Detail[] display_details, int priority = 0, RoomType[] upgrade_paths = null, bool single_assignee = false, bool priority_building_use = false, string[] effects = null)
		: base(id, name)
	{
		this.tooltip = tooltip;
		this.effect = effect;
		this.category = category;
		this.primary_constraint = primary_constraint;
		this.additional_constraints = additional_constraints;
		this.display_details = display_details;
		this.priority = priority;
		this.upgrade_paths = upgrade_paths;
		this.single_assignee = single_assignee;
		this.priority_building_use = priority_building_use;
		this.effects = effects;
		if (this.upgrade_paths != null)
		{
			RoomType[] upgrade_paths2 = this.upgrade_paths;
			foreach (RoomType roomType in upgrade_paths2)
			{
			}
		}
	}

	public RoomIdentificationResult isSatisfactory(Room candidate_room)
	{
		if (primary_constraint != null && !primary_constraint.isSatisfied(candidate_room))
		{
			return RoomIdentificationResult.primary_unsatisfied;
		}
		if (this.additional_constraints != null)
		{
			RoomConstraints.Constraint[] additional_constraints = this.additional_constraints;
			foreach (RoomConstraints.Constraint constraint in additional_constraints)
			{
				if (!constraint.isSatisfied(candidate_room))
				{
					return RoomIdentificationResult.primary_satisfied;
				}
			}
		}
		return RoomIdentificationResult.all_satisfied;
	}

	public string GetCriteriaString()
	{
		string str = "<b>" + Name + "</b>\n" + tooltip + UI.HORIZONTAL_BR_RULE + ROOMS.CRITERIA.HEADER;
		if (this == Db.Get().RoomTypes.Neutral)
		{
			str = str + "\n    • " + ROOMS.CRITERIA.NEUTRAL_TYPE;
		}
		str += ((primary_constraint != null) ? ("\n    • " + primary_constraint.name) : "");
		if (this.additional_constraints != null)
		{
			RoomConstraints.Constraint[] additional_constraints = this.additional_constraints;
			foreach (RoomConstraints.Constraint constraint in additional_constraints)
			{
				str = str + "\n    • " + constraint.name;
			}
		}
		return str;
	}

	public string GetRoomEffectsString()
	{
		if (this.effects != null && this.effects.Length > 0)
		{
			string text = ROOMS.EFFECTS.HEADER;
			string[] effects = this.effects;
			foreach (string id in effects)
			{
				Effect effect = Db.Get().effects.Get(id);
				text += Effect.CreateTooltip(effect, false, "\n    • ");
			}
			return text;
		}
		return null;
	}

	public void TriggerRoomEffects(KPrefabID triggerer, Effects target)
	{
		if (primary_constraint != null && !((Object)triggerer == (Object)null) && this.effects != null && primary_constraint.building_criteria(triggerer))
		{
			string[] effects = this.effects;
			foreach (string effect_id in effects)
			{
				target.Add(effect_id, true);
			}
		}
	}
}
