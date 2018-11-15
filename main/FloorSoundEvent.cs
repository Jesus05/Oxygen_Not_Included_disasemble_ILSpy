using FMOD.Studio;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{Name}")]
public class FloorSoundEvent : SoundEvent
{
	public static float IDLE_WALKING_VOLUME_REDUCTION = 0.55f;

	public FloorSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame, false, false, (float)SoundEvent.IGNORE_INTERVAL, true)
	{
		base.noiseValues = SoundEventVolumeCache.instance.GetVolume("FloorSoundEvent", sound_name);
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 pos = behaviour.GetComponent<Transform>().GetPosition();
		KBatchedAnimController component = behaviour.GetComponent<KBatchedAnimController>();
		if ((Object)component != (Object)null)
		{
			pos = component.GetPivotSymbolPosition();
		}
		int num = Grid.PosToCell(pos);
		int cell = Grid.CellBelow(num);
		string audioCategory = GetAudioCategory(cell);
		string name = StringFormatter.Combine(audioCategory, "_", base.name);
		string sound = GlobalAssets.GetSound(name, true);
		if (sound == null)
		{
			name = StringFormatter.Combine("Rock_", base.name);
			sound = GlobalAssets.GetSound(name, true);
			if (sound == null)
			{
				name = base.name;
				sound = GlobalAssets.GetSound(name, true);
			}
		}
		if (!SoundEvent.IsLowPrioritySound(sound))
		{
			pos = SoundEvent.GetCameraScaledPosition(pos);
			if (Grid.Element != null)
			{
				bool isLiquid = Grid.Element[num].IsLiquid;
				float num2 = 0f;
				if (isLiquid)
				{
					num2 = SoundUtil.GetLiquidDepth(num);
					string sound2 = GlobalAssets.GetSound("Liquid_footstep", true);
					if (sound2 != null)
					{
						FMOD.Studio.EventInstance instance = SoundEvent.BeginOneShot(sound2, pos);
						if (num2 > 0f)
						{
							instance.setParameterValue("liquidDepth", num2);
						}
						SoundEvent.EndOneShot(instance);
					}
				}
				if (sound != null)
				{
					FMOD.Studio.EventInstance instance2 = SoundEvent.BeginOneShot(sound, pos);
					if (instance2.isValid())
					{
						if (num2 > 0f)
						{
							instance2.setParameterValue("liquidDepth", num2);
						}
						if (behaviour.currentAnimFile != null && behaviour.currentAnimFile.Contains("anim_loco_walk"))
						{
							instance2.setVolume(IDLE_WALKING_VOLUME_REDUCTION);
						}
						SoundEvent.EndOneShot(instance2);
					}
				}
			}
		}
	}

	private static string GetAudioCategory(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return "Rock";
		}
		Element element = Grid.Element[cell];
		if (Grid.Foundation[cell])
		{
			BuildingDef buildingDef = null;
			GameObject gameObject = Grid.Objects[cell, 1];
			if ((Object)gameObject != (Object)null)
			{
				Building component = gameObject.GetComponent<BuildingComplete>();
				if ((Object)component != (Object)null)
				{
					buildingDef = component.Def;
				}
			}
			string result = string.Empty;
			if ((Object)buildingDef != (Object)null)
			{
				string prefabID = buildingDef.PrefabID;
				result = ((prefabID == "PlasticTile") ? "TilePlastic" : ((prefabID == "GlassTile") ? "TileGlass" : ((prefabID == "BunkerTile") ? "TileBunker" : ((!(prefabID == "MetalTile")) ? "Tile" : "TileMetal"))));
			}
			return result;
		}
		string floorEventAudioCategory = element.substance.GetFloorEventAudioCategory();
		if (floorEventAudioCategory != null)
		{
			return floorEventAudioCategory;
		}
		if (element.HasTag(GameTags.RefinedMetal))
		{
			return "RefinedMetal";
		}
		if (element.HasTag(GameTags.Metal))
		{
			return "RawMetal";
		}
		return "Rock";
	}
}
