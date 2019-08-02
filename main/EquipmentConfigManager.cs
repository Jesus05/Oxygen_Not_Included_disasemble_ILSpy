using System.Collections.Generic;
using UnityEngine;

public class EquipmentConfigManager : KMonoBehaviour
{
	public static EquipmentConfigManager Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	public void RegisterEquipment(IEquipmentConfig config)
	{
		EquipmentDef equipmentDef = config.CreateEquipmentDef();
		string id = equipmentDef.Id;
		string name = equipmentDef.Name;
		string recipeDescription = equipmentDef.RecipeDescription;
		float mass = equipmentDef.Mass;
		bool unitMass = true;
		KAnimFile anim = equipmentDef.Anim;
		string initialAnim = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Ore;
		EntityTemplates.CollisionShape collisionShape = equipmentDef.CollisionShape;
		float width = equipmentDef.width;
		float height = equipmentDef.height;
		bool isPickupable = true;
		SimHashes outputElement = equipmentDef.OutputElement;
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, recipeDescription, mass, unitMass, anim, initialAnim, sceneLayer, collisionShape, width, height, isPickupable, 0, outputElement, null);
		Equippable equippable = gameObject.AddComponent<Equippable>();
		equippable.def = equipmentDef;
		Debug.Assert((Object)equippable.def != (Object)null);
		equippable.slotID = equipmentDef.Slot;
		Debug.Assert(equippable.slot != null);
		config.DoPostConfigure(gameObject);
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
	}

	private void LoadRecipe(EquipmentDef def, Equippable equippable)
	{
		string id = def.Id;
		string recipeDescription = def.RecipeDescription;
		Recipe recipe = new Recipe(id, 1f, (SimHashes)0, null, recipeDescription, 0);
		recipe.SetFabricator(def.FabricatorId, def.FabricationTime);
		recipe.TechUnlock = def.RecipeTechUnlock;
		foreach (KeyValuePair<string, float> item in def.InputElementMassMap)
		{
			recipe.AddIngredient(new Recipe.Ingredient(item.Key, item.Value));
		}
	}
}
