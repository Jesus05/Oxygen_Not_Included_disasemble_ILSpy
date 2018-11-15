using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class GlomConfig : IEntityConfig
{
	public const string ID = "Glom";

	public const string BASE_TRAIT_ID = "GlomBaseTrait";

	public const SimHashes dirtyEmitElement = SimHashes.ContaminatedOxygen;

	public const float dirtyProbabilityPercent = 25f;

	public const float dirtyCellToTargetMass = 1f;

	public const float dirtyMassPerDirty = 0.2f;

	public const float dirtyMassReleaseOnDeath = 3f;

	public const string emitDisease = "SlimeLung";

	public const int emitDiseasePerKg = 1000;

	public GameObject CreatePrefab()
	{
		string text = STRINGS.CREATURES.SPECIES.GLOM.NAME;
		string id = "Glom";
		string name = text;
		string desc = STRINGS.CREATURES.SPECIES.GLOM.DESC;
		float mass = 25f;
		KAnimFile anim = Assets.GetAnim("glom_kanim");
		string initialAnim = "idle_loop";
		EffectorValues tIER = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.Creatures, 1, 1, tIER, default(EffectorValues), SimHashes.Creature, null, 293f);
		Trait trait = Db.Get().CreateTrait("GlomBaseTrait", text, text, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, text, false, false, true));
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.GroundBased);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, "GlomBaseTrait", "HatchNavGrid", NavType.Floor, 32, 2f, string.Empty, 0, true, true, 293.15f, 393.15f, 273.15f, 423.15f);
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		ElementDropperMonitor.Def def = gameObject.AddOrGetDef<ElementDropperMonitor.Def>();
		def.dirtyEmitElement = SimHashes.ContaminatedOxygen;
		def.dirtyProbabilityPercent = 25f;
		def.dirtyCellToTargetMass = 1f;
		def.dirtyMassPerDirty = 0.2f;
		def.dirtyMassReleaseOnDeath = 3f;
		def.emitDiseaseIdx = Db.Get().Diseases.GetIndex("SlimeLung");
		def.emitDiseasePerKg = 1000f;
		gameObject.AddOrGet<LoopingSounds>();
		LoopingSounds component2 = gameObject.GetComponent<LoopingSounds>();
		component2.updatePosition = true;
		DiseaseSourceVisualizer diseaseSourceVisualizer = gameObject.AddOrGet<DiseaseSourceVisualizer>();
		diseaseSourceVisualizer.alwaysShowDisease = "SlimeLung";
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_movement_short", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_jump", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_land", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_expel", NOISE_POLLUTION.CREATURES.TIER4);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, false);
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new TrappedStates.Def(), true).Add(new BaggedStates.Def(), true)
			.Add(new FallStates.Def(), true)
			.Add(new StunnedStates.Def(), true)
			.Add(new DrowningStates.Def(), true)
			.Add(new DebugGoToStates.Def(), true)
			.Add(new FleeStates.Def(), true)
			.Add(new DropElementStates.Def(), true)
			.Add(new IdleStates.Def(), true);
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.GlomSpecies, null);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
