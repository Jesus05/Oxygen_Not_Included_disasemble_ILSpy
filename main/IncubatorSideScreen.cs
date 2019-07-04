using UnityEngine;

public class IncubatorSideScreen : ReceptacleSideScreen
{
	public DescriptorPanel RequirementsDescriptorPanel;

	public DescriptorPanel HarvestDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

	public MultiToggle continuousToggle;

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<EggIncubator>() != (Object)null;
	}

	protected override void SetResultDescriptions(GameObject go)
	{
		string text = "";
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
		return Def.GetUISprite(prefab, "ui", false).first;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		EggIncubator incubator = target.GetComponent<EggIncubator>();
		continuousToggle.ChangeState((!incubator.autoReplaceEntity) ? 1 : 0);
		continuousToggle.onClick = delegate
		{
			incubator.autoReplaceEntity = !incubator.autoReplaceEntity;
			continuousToggle.ChangeState((!incubator.autoReplaceEntity) ? 1 : 0);
		};
	}
}
