using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentTemplates
{
	public static EquipmentDef CreateEquipmentDef(string Id, string Slot, SimHashes OutputElement, float Mass, string Anim, string SnapOn, string BuildOverride, int BuildOverridePriority, List<AttributeModifier> AttributeModifiers, string SnapOn1 = null, bool IsBody = false, EntityTemplates.CollisionShape CollisionShape = EntityTemplates.CollisionShape.CIRCLE, float width = 0.325f, float height = 0.325f, Tag[] additional_tags = null, string RecipeTechUnlock = null)
	{
		EquipmentDef equipmentDef = ScriptableObject.CreateInstance<EquipmentDef>();
		equipmentDef.Id = Id;
		equipmentDef.Slot = Slot;
		equipmentDef.RecipeTechUnlock = RecipeTechUnlock;
		equipmentDef.OutputElement = OutputElement;
		equipmentDef.Mass = Mass;
		equipmentDef.Anim = Assets.GetAnim(Anim);
		equipmentDef.SnapOn = SnapOn;
		equipmentDef.SnapOn1 = SnapOn1;
		equipmentDef.BuildOverride = ((BuildOverride == null || BuildOverride.Length <= 0) ? null : Assets.GetAnim(BuildOverride));
		equipmentDef.BuildOverridePriority = BuildOverridePriority;
		equipmentDef.IsBody = IsBody;
		equipmentDef.AttributeModifiers = AttributeModifiers;
		equipmentDef.CollisionShape = CollisionShape;
		equipmentDef.width = width;
		equipmentDef.height = height;
		equipmentDef.AdditionalTags = additional_tags;
		return equipmentDef;
	}
}
