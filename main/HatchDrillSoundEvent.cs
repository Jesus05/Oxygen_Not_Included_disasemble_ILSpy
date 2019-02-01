using FMOD.Studio;
using UnityEngine;

public class HatchDrillSoundEvent : SoundEvent
{
	public HatchDrillSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, true, true, min_interval, false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		int cell = Grid.PosToCell(position);
		int cell2 = Grid.CellBelow(cell);
		float value = (float)GetAudioCategory(cell2);
		EventInstance instance = SoundEvent.BeginOneShot(base.sound, position);
		instance.setParameterValue("material_ID", value);
		SoundEvent.EndOneShot(instance);
	}

	private static int GetAudioCategory(int cell)
	{
		if (Grid.IsValidCell(cell))
		{
			Element element = Grid.Element[cell];
			if (element.id != SimHashes.Dirt)
			{
				if (!element.HasTag(GameTags.IceOre))
				{
					if (element.id != SimHashes.CrushedIce)
					{
						if (element.id != SimHashes.DirtyIce)
						{
							if (!Grid.Foundation[cell])
							{
								if (element.id != SimHashes.OxyRock)
								{
									if (element.id != SimHashes.PhosphateNodules && element.id != SimHashes.Phosphorus && element.id != SimHashes.Phosphorite)
									{
										if (!element.HasTag(GameTags.Metal))
										{
											if (!element.HasTag(GameTags.RefinedMetal))
											{
												if (element.id != SimHashes.Sand)
												{
													if (element.id != SimHashes.Clay)
													{
														if (element.id != SimHashes.Algae)
														{
															if (element.id != SimHashes.SlimeMold)
															{
																return 7;
															}
															return 11;
														}
														return 10;
													}
													return 9;
												}
												return 8;
											}
											return 6;
										}
										return 5;
									}
									return 4;
								}
								return 3;
							}
							return 2;
						}
						return 13;
					}
					return 12;
				}
				return 1;
			}
			return 0;
		}
		return 7;
	}
}
