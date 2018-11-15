using FMOD.Studio;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Comet : KMonoBehaviour, ISim33ms
{
	private const SimHashes EXHAUST_ELEMENT = SimHashes.CarbonDioxide;

	private const float EXHAUST_RATE = 50f;

	public Vector2 spawnVelocity = new Vector2(12f, 15f);

	public Vector2 spawnAngle = new Vector2(-100f, -80f);

	public Vector2 massRange;

	public Vector2 temperatureRange;

	public SpawnFXHashes explosionEffectHash;

	public int splashRadius = 1;

	public int addTiles;

	public int addTilesMinHeight;

	public int addTilesMaxHeight;

	public int entityDamage = 1;

	public float totalTileDamage = 0.2f;

	private float addTileMass;

	public Vector2 elementReplaceTileTemperatureRange = new Vector2(800f, 1000f);

	public Vector2I explosionOreCount = new Vector2I(0, 0);

	private float explosionMass;

	public Vector2 explosionTemperatureRange = new Vector2(500f, 700f);

	public Vector2 explosionSpeedRange = new Vector2(8f, 14f);

	public float windowDamageMultiplier = 5f;

	public float bunkerDamageMultiplier;

	public string impactSound;

	public string flyingSound;

	public int flyingSoundID;

	private HashedString FLYING_SOUND_ID_PARAMETER = "meteorType";

	[Serialize]
	private Vector2 velocity;

	[Serialize]
	private float remainingTileDamage;

	private Vector3 previousPosition;

	private bool hasExploded;

	private LoopingSounds loopingSounds;

	private List<GameObject> damagedEntities = new List<GameObject>();

	private List<int> destroyedCells = new List<int>();

	private const float MAX_DISTANCE_TEST = 6f;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		remainingTileDamage = totalTileDamage;
		loopingSounds = base.gameObject.GetComponent<LoopingSounds>();
		flyingSound = GlobalAssets.GetSound("Meteor_LP", false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RandomizeValues();
		StartLoopingSound();
	}

	public void RandomizeValues()
	{
		float num = Random.Range(massRange.x, massRange.y);
		PrimaryElement component = GetComponent<PrimaryElement>();
		component.Mass = num;
		component.Temperature = Random.Range(temperatureRange.x, temperatureRange.y);
		float num2 = Random.Range(spawnAngle.x, spawnAngle.y);
		float f = num2 * 3.14159274f / 180f;
		float num3 = Random.Range(spawnVelocity.x, spawnVelocity.y);
		velocity = new Vector2((0f - Mathf.Cos(f)) * num3, Mathf.Sin(f) * num3);
		KBatchedAnimController component2 = GetComponent<KBatchedAnimController>();
		component2.Rotation = 0f - num2 - 90f;
		if (addTiles > 0)
		{
			float num4 = Random.Range(0.95f, 0.98f);
			explosionMass = num * (1f - num4);
			addTileMass = num * num4;
		}
		else
		{
			explosionMass = num;
			addTileMass = 0f;
		}
	}

	[ContextMenu("Explode")]
	private void Explode(Vector3 pos, int cell, int prev_cell, Element element)
	{
		PlayImpactSound(pos);
		Vector3 pos2 = pos;
		pos2.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2);
		Game.Instance.SpawnFX(explosionEffectHash, pos2, 0f);
		Substance substance = element.substance;
		int num = Random.Range(explosionOreCount.x, explosionOreCount.y + 1);
		Vector2 a = -velocity.normalized;
		Vector2 a2 = new Vector2(a.y, 0f - a.x);
		ListPool<ScenePartitionerEntry, Comet>.PooledList pooledList = ListPool<ScenePartitionerEntry, Comet>.Allocate();
		GameScenePartitioner.Instance.GatherEntries((int)pos.x - 3, (int)pos.y - 3, 6, 6, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		foreach (ScenePartitionerEntry item in pooledList)
		{
			GameObject gameObject = (item.obj as Pickupable).gameObject;
			if (!((Object)gameObject.GetComponent<MinionIdentity>() != (Object)null))
			{
				Vector2 vector = gameObject.transform.GetPosition() - pos;
				vector = vector.normalized;
				vector += new Vector2(0f, 0.55f);
				vector *= 0.5f * Random.Range(explosionSpeedRange.x, explosionSpeedRange.y);
				if (GameComps.Fallers.Has(gameObject))
				{
					GameComps.Fallers.Remove(gameObject);
				}
				if (GameComps.Gravities.Has(gameObject))
				{
					GameComps.Gravities.Remove(gameObject);
				}
				GameComps.Fallers.Add(gameObject, vector);
			}
		}
		pooledList.Recycle();
		int num2 = splashRadius + 1;
		for (int i = -num2; i <= num2; i++)
		{
			for (int j = -num2; j <= num2; j++)
			{
				int num3 = Grid.OffsetCell(cell, j, i);
				if (Grid.IsValidCell(num3) && !destroyedCells.Contains(num3))
				{
					float num4 = (1f - (float)Mathf.Abs(j) / (float)num2) * (1f - (float)Mathf.Abs(i) / (float)num2);
					if (num4 > 0f)
					{
						DamageTiles(num3, prev_cell, num4 * totalTileDamage * 0.5f);
					}
				}
			}
		}
		float mass = (num <= 0) ? 1f : (explosionMass / (float)num);
		float temperature = Random.Range(explosionTemperatureRange.x, explosionTemperatureRange.y);
		for (int k = 0; k < num; k++)
		{
			Vector2 normalized = (a + a2 * Random.Range(-1f, 1f)).normalized;
			Vector3 v = normalized * Random.Range(explosionSpeedRange.x, explosionSpeedRange.y);
			Vector3 a3 = normalized.normalized * 0.75f;
			a3 += new Vector3(0f, 0.55f, 0f);
			a3 += pos;
			GameObject go = substance.SpawnResource(a3, mass, temperature, byte.MaxValue, 0, false, false);
			if (GameComps.Fallers.Has(go))
			{
				GameComps.Fallers.Remove(go);
			}
			GameComps.Fallers.Add(go, v);
		}
		if (addTiles > 0)
		{
			int depthOfElement = GetDepthOfElement(cell, element);
			float num5 = 1f - (float)(depthOfElement - addTilesMinHeight) / (float)(addTilesMaxHeight - addTilesMinHeight);
			int num6 = Mathf.Min(addTiles, Mathf.Clamp(Mathf.RoundToInt((float)addTiles * num5), 1, addTiles));
			HashSetPool<int, Comet>.PooledHashSet pooledHashSet = HashSetPool<int, Comet>.Allocate();
			HashSetPool<int, Comet>.PooledHashSet pooledHashSet2 = HashSetPool<int, Comet>.Allocate();
			QueuePool<GameUtil.FloodFillInfo, Comet>.PooledQueue pooledQueue = QueuePool<GameUtil.FloodFillInfo, Comet>.Allocate();
			pooledQueue.Enqueue(new GameUtil.FloodFillInfo
			{
				cell = cell,
				depth = 0
			});
			pooledQueue.Enqueue(new GameUtil.FloodFillInfo
			{
				cell = prev_cell,
				depth = 0
			});
			pooledQueue.Enqueue(new GameUtil.FloodFillInfo
			{
				cell = Grid.OffsetCell(cell, new CellOffset(-1, 0)),
				depth = 0
			});
			pooledQueue.Enqueue(new GameUtil.FloodFillInfo
			{
				cell = Grid.OffsetCell(cell, new CellOffset(1, 0)),
				depth = 0
			});
			GameUtil.FloodFillConditional(pooledQueue, SpawnTilesCellTest, pooledHashSet2, pooledHashSet, 10);
			if (addTiles > 0)
			{
				Output.Log($"Comet at height: addtiles {addTiles}, height {depthOfElement}, height ratio {num5}, total added {num6}");
			}
			float mass2 = (num6 <= 0) ? 1f : (addTileMass / (float)num6);
			UnstableGroundManager component = World.Instance.GetComponent<UnstableGroundManager>();
			foreach (int item2 in pooledHashSet)
			{
				if (num6 <= 0)
				{
					break;
				}
				component.Spawn(item2, element, mass2, temperature, byte.MaxValue, 0);
				num6--;
			}
			pooledHashSet.Recycle();
			pooledHashSet2.Recycle();
			pooledQueue.Recycle();
		}
	}

	private int GetDepthOfElement(int cell, Element element)
	{
		int num = 0;
		int num2 = Grid.CellBelow(cell);
		while (Grid.IsValidCell(num2) && Grid.Element[num2] == element)
		{
			num++;
			num2 = Grid.CellBelow(num2);
		}
		return num;
	}

	private bool SpawnTilesCellTest(int cell)
	{
		return Grid.IsValidCell(cell) && !Grid.Solid[cell];
	}

	[ContextMenu("DamageTiles")]
	private float DamageTiles(int cell, int prev_cell, float input_damage)
	{
		GameObject gameObject = Grid.Objects[cell, 9];
		float num = 1f;
		bool flag = false;
		if ((Object)gameObject != (Object)null)
		{
			if (gameObject.GetComponent<KPrefabID>().HasTag(GameTags.Window))
			{
				num = windowDamageMultiplier;
			}
			else if (gameObject.GetComponent<KPrefabID>().HasTag(GameTags.Bunker))
			{
				num = bunkerDamageMultiplier;
			}
			SimCellOccupier component = gameObject.GetComponent<SimCellOccupier>();
			if ((Object)component != (Object)null && !component.doReplaceElement)
			{
				flag = true;
			}
		}
		Element element = (!flag) ? Grid.Element[cell] : gameObject.GetComponent<PrimaryElement>().Element;
		if (element.strength == 0f)
		{
			return 0f;
		}
		float num2 = input_damage * num / element.strength;
		PlayTileDamageSound(element, Grid.CellToPos(cell));
		if (num2 == 0f)
		{
			return 0f;
		}
		float num3;
		if (flag)
		{
			BuildingHP component2 = gameObject.GetComponent<BuildingHP>();
			float a = (float)component2.HitPoints / (float)component2.MaxHitPoints;
			float f = num2 * (float)component2.MaxHitPoints;
			component2.gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo
			{
				damage = Mathf.RoundToInt(f),
				source = (string)BUILDINGS.DAMAGESOURCES.COMET,
				popString = (string)UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.COMET
			});
			num3 = Mathf.Min(a, num2);
		}
		else
		{
			WorldDamage instance = WorldDamage.Instance;
			float amount = num2;
			string source_name = BUILDINGS.DAMAGESOURCES.COMET;
			num3 = instance.ApplyDamage(cell, amount, prev_cell, -1, source_name, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.COMET);
		}
		destroyedCells.Add(cell);
		float num4 = num3 / num2;
		return input_damage * (1f - num4);
	}

	private void DamageThings(Vector3 pos, int cell, int damage)
	{
		if (Grid.IsValidCell(cell))
		{
			GameObject gameObject = Grid.Objects[cell, 1];
			if ((Object)gameObject != (Object)null)
			{
				BuildingHP component = gameObject.GetComponent<BuildingHP>();
				Building component2 = gameObject.GetComponent<Building>();
				if ((Object)component != (Object)null && !damagedEntities.Contains(gameObject))
				{
					KPrefabID component3 = gameObject.GetComponent<KPrefabID>();
					float f = (!component3.HasTag(GameTags.Bunker)) ? ((float)damage) : ((float)damage * bunkerDamageMultiplier);
					if ((Object)component2 != (Object)null && (Object)component2.Def != (Object)null)
					{
						PlayBuildingDamageSound(component2.Def, Grid.CellToPos(cell));
					}
					component.gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo
					{
						damage = Mathf.RoundToInt(f),
						source = (string)BUILDINGS.DAMAGESOURCES.COMET,
						popString = (string)UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.COMET
					});
					damagedEntities.Add(gameObject);
				}
			}
			ListPool<ScenePartitionerEntry, Comet>.PooledList pooledList = ListPool<ScenePartitionerEntry, Comet>.Allocate();
			GameScenePartitioner.Instance.GatherEntries((int)pos.x, (int)pos.y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
			foreach (ScenePartitionerEntry item in pooledList)
			{
				Pickupable pickupable = item.obj as Pickupable;
				Health component4 = pickupable.GetComponent<Health>();
				if ((Object)component4 != (Object)null && !damagedEntities.Contains(pickupable.gameObject))
				{
					KPrefabID component5 = pickupable.GetComponent<KPrefabID>();
					float amount = (!component5.HasTag(GameTags.Bunker)) ? ((float)damage) : ((float)damage * bunkerDamageMultiplier);
					component4.Damage(amount);
					damagedEntities.Add(pickupable.gameObject);
				}
			}
			pooledList.Recycle();
		}
	}

	private float GetDistanceFromImpact()
	{
		float num = velocity.x / velocity.y;
		Vector3 position = base.transform.GetPosition();
		float num2 = 0f;
		while (num2 > -6f)
		{
			num2 -= 1f;
			float num3 = Mathf.Ceil(position.y + num2) - 0.2f;
			num2 = num3 - position.y;
			float x = num2 * num;
			Vector3 b = new Vector3(x, num2, 0f);
			int num4 = Grid.PosToCell(position + b);
			if (Grid.IsValidCell(num4) && Grid.Solid[num4])
			{
				return b.magnitude;
			}
		}
		return 6f;
	}

	public float GetSoundDistance()
	{
		return GetDistanceFromImpact();
	}

	private void PlayTileDamageSound(Element element, Vector3 pos)
	{
		string text = element.substance.GetMiningBreakSound();
		if (text == null)
		{
			text = (element.HasTag(GameTags.RefinedMetal) ? "RefinedMetal" : ((!element.HasTag(GameTags.Metal)) ? "Rock" : "RawMetal"));
		}
		text = "MeteorDamage_" + text;
		text = GlobalAssets.GetSound(text, false);
		if ((bool)CameraController.Instance && CameraController.Instance.IsAudibleSound(pos, text))
		{
			KFMOD.PlayOneShot(text, CameraController.Instance.GetVerticallyScaledPosition(pos));
		}
	}

	private void PlayBuildingDamageSound(BuildingDef def, Vector3 pos)
	{
		if ((Object)def != (Object)null)
		{
			string name = StringFormatter.Combine("MeteorDamage_Building_", def.AudioCategory);
			string sound = GlobalAssets.GetSound(name, false);
			if (sound == null)
			{
				name = "MeteorDamage_Building_Metal";
				sound = GlobalAssets.GetSound(name, false);
			}
			if (sound != null && (bool)CameraController.Instance && CameraController.Instance.IsAudibleSound(pos, sound))
			{
				KFMOD.PlayOneShot(sound, CameraController.Instance.GetVerticallyScaledPosition(pos));
			}
		}
	}

	public void Sim33ms(float dt)
	{
		if (!hasExploded)
		{
			Vector2 vector = new Vector2((float)Grid.WidthInCells, (float)Grid.HeightInCells) * -0.1f;
			Vector2 vector2 = new Vector2((float)Grid.WidthInCells, (float)Grid.HeightInCells) * 1.1f;
			Vector3 position = base.transform.GetPosition();
			Vector3 vector3 = position + new Vector3(velocity.x * dt, velocity.y * dt, 0f);
			int num = Grid.PosToCell(vector3);
			loopingSounds.UpdateVelocity(flyingSound, vector3 - position);
			Element element = ElementLoader.FindElementByHash(SimHashes.CarbonDioxide);
			if (Grid.IsValidCell(num) && !Grid.Solid[num])
			{
				SimMessages.EmitMass(num, element.idx, dt * 50f, element.defaultValues.temperature, 0, 0, -1);
			}
			if (vector3.x < vector.x || vector2.x < vector3.x || vector3.y < vector.y)
			{
				Util.KDestroyGameObject(base.gameObject);
			}
			int num2 = Grid.PosToCell(this);
			int num3 = Grid.PosToCell(previousPosition);
			if (num2 != num3)
			{
				if (Grid.IsValidCell(num2) && Grid.Solid[num2])
				{
					PrimaryElement component = GetComponent<PrimaryElement>();
					remainingTileDamage = DamageTiles(num2, num3, remainingTileDamage);
					if (remainingTileDamage <= 0f)
					{
						Explode(position, num2, num3, component.Element);
						hasExploded = true;
						Util.KDestroyGameObject(base.gameObject);
						return;
					}
				}
				else
				{
					DamageThings(position, num2, entityDamage);
				}
			}
			previousPosition = position;
			base.transform.SetPosition(vector3);
		}
	}

	private void PlayImpactSound(Vector3 pos)
	{
		if (impactSound == null)
		{
			impactSound = "Meteor_Large_Impact";
		}
		loopingSounds.StopSound(flyingSound);
		string sound = GlobalAssets.GetSound(impactSound, false);
		if (CameraController.Instance.IsAudibleSound(pos, sound))
		{
			EventInstance instance = KFMOD.BeginOneShot(sound, pos);
			instance.setParameterValue("userVolume_SFX", KPlayerPrefs.GetFloat("Volume_SFX"));
			KFMOD.EndOneShot(instance);
		}
	}

	private void StartLoopingSound()
	{
		loopingSounds.StartSound(flyingSound);
		loopingSounds.UpdateFirstParameter(flyingSound, FLYING_SOUND_ID_PARAMETER, (float)flyingSoundID);
	}
}
