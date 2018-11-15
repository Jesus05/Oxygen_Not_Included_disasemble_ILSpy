using UnityEngine;

public class LadderSoundEvent : SoundEvent
{
	public LadderSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame, false, false, (float)SoundEvent.IGNORE_INTERVAL, true)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		int cell = Grid.PosToCell(position);
		GameObject gameObject = null;
		BuildingDef buildingDef = null;
		if (Grid.IsValidCell(cell))
		{
			gameObject = Grid.Objects[cell, 1];
			if ((Object)gameObject != (Object)null && (Object)gameObject.GetComponent<Ladder>() != (Object)null)
			{
				Building component = gameObject.GetComponent<BuildingComplete>();
				if ((Object)component != (Object)null)
				{
					buildingDef = component.Def;
				}
			}
		}
		if ((Object)buildingDef != (Object)null)
		{
			string name = (!(buildingDef.PrefabID == "LadderFast")) ? base.name : StringFormatter.Combine(base.name, "_Plastic");
			string sound = GlobalAssets.GetSound(name, false);
			if (sound != null)
			{
				SoundEvent.PlayOneShot(sound, position);
			}
		}
	}
}
