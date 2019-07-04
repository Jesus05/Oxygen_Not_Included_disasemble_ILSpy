using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class ResearchPointObject : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public string TypeID = "";

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Research.Instance.AddResearchPoints(TypeID, 1f);
		ResearchType researchType = Research.Instance.GetResearchType(TypeID);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Research, researchType.name, base.transform, 1.5f, false);
		Util.KDestroyGameObject(base.gameObject);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		ResearchType researchType = Research.Instance.GetResearchType(TypeID);
		list.Add(new Descriptor(string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.RESEARCHPOINT, researchType.name), string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.RESEARCHPOINT, researchType.description), Descriptor.DescriptorType.Effect, false));
		return list;
	}
}
