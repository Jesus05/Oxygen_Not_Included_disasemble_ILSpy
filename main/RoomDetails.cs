using STRINGS;
using System;
using UnityEngine;

public class RoomDetails
{
	public class Detail
	{
		public Func<Room, string> resolve_string_function;

		public Detail(Func<Room, string> resolve_string_function)
		{
			this.resolve_string_function = resolve_string_function;
		}
	}

	public static readonly Detail AVERAGE_TEMPERATURE = new Detail(delegate
	{
		float num3 = 0f;
		if (num3 == 0f)
		{
			return string.Format(ROOMS.DETAILS.AVERAGE_TEMPERATURE.NAME, UI.OVERLAYS.TEMPERATURE.EXTREMECOLD);
		}
		return string.Format(ROOMS.DETAILS.AVERAGE_TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(num3, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
	});

	public static readonly Detail AVERAGE_ATMO_MASS = new Detail(delegate
	{
		float num = 0f;
		float num2 = 0f;
		num = ((!(num2 > 0f)) ? 0f : (num / num2));
		return string.Format(ROOMS.DETAILS.AVERAGE_ATMO_MASS.NAME, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	});

	public static readonly Detail ASSIGNED_TO = new Detail(delegate(Room room)
	{
		string text = string.Empty;
		foreach (KPrefabID primaryEntity in room.GetPrimaryEntities())
		{
			if (!((UnityEngine.Object)primaryEntity == (UnityEngine.Object)null))
			{
				Assignable component = primaryEntity.GetComponent<Assignable>();
				if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
				{
					IAssignableIdentity assignee = component.assignee;
					if (assignee == null)
					{
						text += ((!(text == string.Empty)) ? ("\n<color=#BCBCBC>    • " + primaryEntity.GetProperName() + ": " + ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED) : ("<color=#BCBCBC>    • " + primaryEntity.GetProperName() + ": " + ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED));
						text += "</color>";
					}
					else
					{
						text += ((!(text == string.Empty)) ? ("\n    • " + primaryEntity.GetProperName() + ": " + assignee.GetProperName()) : ("    • " + primaryEntity.GetProperName() + ": " + assignee.GetProperName()));
					}
				}
			}
		}
		if (text == string.Empty)
		{
			text = ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED;
		}
		return string.Format(ROOMS.DETAILS.ASSIGNED_TO.NAME, text);
	});

	public static readonly Detail SIZE = new Detail((Room room) => string.Format(ROOMS.DETAILS.SIZE.NAME, room.cavity.numCells));

	public static readonly Detail BUILDING_COUNT = new Detail((Room room) => string.Format(ROOMS.DETAILS.BUILDING_COUNT.NAME, room.buildings.Count));

	public static readonly Detail CREATURE_COUNT = new Detail((Room room) => string.Format(ROOMS.DETAILS.CREATURE_COUNT.NAME, room.cavity.creatures.Count + room.cavity.eggs.Count));

	public static readonly Detail EFFECT = new Detail((Room room) => room.roomType.effect);

	public static readonly Detail EFFECTS = new Detail((Room room) => room.roomType.GetRoomEffectsString());

	public static string RoomDetailString(Room room)
	{
		string empty = string.Empty;
		empty = empty + "<b>" + ROOMS.DETAILS.HEADER + "</b>";
		RoomType roomType = room.roomType;
		Detail[] display_details = roomType.display_details;
		foreach (Detail detail in display_details)
		{
			empty = empty + "\n    • " + detail.resolve_string_function(room);
		}
		return empty;
	}
}
