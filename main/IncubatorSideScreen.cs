using UnityEngine;

public class IncubatorSideScreen : ReceptacleSideScreen
{
	public DescriptorPanel RequirementsDescriptorPanel;

	public DescriptorPanel HarvestDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<EggIncubator>() != (Object)null;
	}

	protected override void SetResultDescriptions(GameObject go)
	{
		string text = string.Empty;
		InfoDescription component = go.GetComponent<InfoDescription>();
		if ((bool)component)
		{
			text += component.description;
		}
		descriptionLabel.SetText(text);
	}

	protected override Sprite GetEntityIcon(Tag prefabTag)
	{
		GameObject prefab = Assets.GetPrefab(prefabTag);
		IncubationMonitor.Def def = prefab.GetDef<IncubationMonitor.Def>();
		string text = "ui";
		if (def != null)
		{
			GameObject prefab2 = Assets.GetPrefab(def.spawnedCreature);
			if ((bool)prefab2)
			{
				CreatureBrain component = prefab2.GetComponent<CreatureBrain>();
				if ((bool)component && !string.IsNullOrEmpty(component.symbolPrefix))
				{
					text = component.symbolPrefix + text;
				}
			}
		}
		KBatchedAnimController component2 = prefab.GetComponent<KBatchedAnimController>();
		return Def.GetUISpriteFromMultiObjectAnim(component2.AnimFiles[0], text, false);
	}
}
