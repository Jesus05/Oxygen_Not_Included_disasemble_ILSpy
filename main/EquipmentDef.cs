using Klei.AI;
using System;
using System.Collections.Generic;

public class EquipmentDef : Def
{
	public string Id;

	public string Slot;

	public string FabricatorId;

	public float FabricationTime;

	public string RecipeTechUnlock;

	public SimHashes OutputElement;

	public Dictionary<string, float> InputElementMassMap;

	public float Mass;

	public KAnimFile Anim;

	public string SnapOn;

	public string SnapOn1;

	public KAnimFile BuildOverride;

	public int BuildOverridePriority;

	public bool IsBody;

	public List<AttributeModifier> AttributeModifiers;

	public string RecipeDescription;

	public List<Effect> EffectImmunites = new List<Effect>();

	public Action<Equippable> OnEquipCallBack;

	public Action<Equippable> OnUnequipCallBack;

	public EntityTemplates.CollisionShape CollisionShape = EntityTemplates.CollisionShape.CIRCLE;

	public float width;

	public float height = 0.325f;

	public Tag[] AdditionalTags;

	public List<Descriptor> additionalDescriptors = new List<Descriptor>();

	public override string Name => Strings.Get("STRINGS.EQUIPMENT.PREFABS." + Id.ToUpper() + ".NAME");

	public string GenericName => Strings.Get("STRINGS.EQUIPMENT.PREFABS." + Id.ToUpper() + ".GENERICNAME");
}
