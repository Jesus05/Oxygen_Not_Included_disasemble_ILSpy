using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class SpaceArtifact : KMonoBehaviour, IEffectDescriptor, IGameObjectEffectDescriptor
{
	public const string ID = "SpaceArtifact";

	[SerializeField]
	private string ui_anim;

	[SerializeField]
	private ArtifactTier artifactTier;

	public void SetArtifactTier(ArtifactTier tier)
	{
		artifactTier = tier;
	}

	public ArtifactTier GetArtifactTier()
	{
		return artifactTier;
	}

	public void SetUIAnim(string anim)
	{
		ui_anim = anim;
	}

	public string GetUIAnim()
	{
		return ui_anim;
	}

	public List<Descriptor> GetEffectDescriptions()
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = new Descriptor(string.Format("This is an artifact from space"), string.Format("This is the tooltip string"), Descriptor.DescriptorType.Information, false);
		list.Add(item);
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return GetEffectDescriptions();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return GetEffectDescriptions();
	}
}
