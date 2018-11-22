using FMOD.Studio;
using UnityEngine;

public class WallDamageSoundEvent : SoundEvent
{
	public int tile;

	public WallDamageSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, true, false, min_interval, false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 vector = default(Vector3);
		int num = 0;
		AggressiveChore.StatesInstance sMI = behaviour.controller.gameObject.GetSMI<AggressiveChore.StatesInstance>();
		if (sMI != null)
		{
			tile = sMI.sm.wallCellToBreak;
			num = GetAudioCategory(tile);
			vector = Grid.CellToPos(tile);
			EventInstance instance = SoundEvent.BeginOneShot(base.sound, vector);
			instance.setParameterValue("material_ID", (float)num);
			SoundEvent.EndOneShot(instance);
		}
	}

	private static int GetAudioCategory(int tile)
	{
		Element element = Grid.Element[tile];
		if (!Grid.Foundation[tile])
		{
			if (element.id != SimHashes.Dirt)
			{
				if (element.id != SimHashes.CrushedIce && element.id != SimHashes.Ice && element.id != SimHashes.DirtyIce)
				{
					if (element.id != SimHashes.OxyRock)
					{
						if (!element.HasTag(GameTags.Metal))
						{
							if (!element.HasTag(GameTags.RefinedMetal))
							{
								if (element.id != SimHashes.Sand)
								{
									if (element.id != SimHashes.Algae)
									{
										return 7;
									}
									return 10;
								}
								return 8;
							}
							return 6;
						}
						return 5;
					}
					return 3;
				}
				return 1;
			}
			return 0;
		}
		return 12;
	}
}
