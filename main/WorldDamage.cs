using FMODUnity;
using STRINGS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDamage : KMonoBehaviour
{
	public KBatchedAnimController leakEffect;

	[SerializeField]
	private FMODAsset leakSound;

	[SerializeField]
	[EventRef]
	private string leakSoundMigrated;

	private List<int> queuedDigCallbackCells = new List<int>();

	private float damageAmount = 0.000833333354f;

	private const float SPAWN_DELAY = 1f;

	private Dictionary<int, float> spawnTimes = new Dictionary<int, float>();

	private List<int> expiredCells = new List<int>();

	public static WorldDamage Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	public void RestoreDamageToValue(int cell, float amount)
	{
		if (Grid.Damage[cell] > amount)
		{
			Grid.Damage[cell] = amount;
		}
	}

	public float ApplyDamage(Sim.WorldDamageInfo damage_info)
	{
		int gameCell = damage_info.gameCell;
		float amount = damageAmount;
		int damageSourceOffset = damage_info.damageSourceOffset;
		string source_name = BUILDINGS.DAMAGESOURCES.LIQUID_PRESSURE;
		return ApplyDamage(gameCell, amount, damageSourceOffset, -1, source_name, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.LIQUID_PRESSURE);
	}

	public float ApplyDamage(int cell, float amount, int src_cell, int destroy_cb_index = -1, string source_name = null, string pop_text = null)
	{
		float result = 0f;
		if (Grid.Solid[cell])
		{
			bool flag = false;
			float num = Grid.Damage[cell];
			result = Mathf.Min(amount, 1f - num);
			num += amount;
			flag = (num > 0.15f);
			if (flag)
			{
				GameObject gameObject = Grid.Objects[cell, 9];
				if ((Object)gameObject != (Object)null)
				{
					BuildingHP component = gameObject.GetComponent<BuildingHP>();
					if ((Object)component != (Object)null)
					{
						float a = (float)component.HitPoints - (1f - num) * (float)component.MaxHitPoints;
						int damage = Mathf.RoundToInt(Mathf.Max(a, 0f));
						gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo
						{
							damage = damage,
							source = source_name,
							popString = pop_text
						});
					}
				}
			}
			Grid.Damage[cell] = Mathf.Min(1f, num);
			if (Grid.Damage[cell] >= 1f)
			{
				DestroyCell(cell, destroy_cb_index);
			}
			else if (Grid.IsValidCell(src_cell) && flag)
			{
				Element element = Grid.Element[src_cell];
				if (element.IsLiquid && Grid.Mass[src_cell] > 1f)
				{
					int num2 = cell - src_cell;
					if (num2 == 1 || num2 == -1 || num2 == Grid.WidthInCells || num2 == -Grid.WidthInCells)
					{
						int num3 = cell + num2;
						if (Grid.IsValidCell(num3))
						{
							Element element2 = Grid.Element[num3];
							if (!element2.IsSolid && (!element2.IsLiquid || (element2.id == element.id && Grid.Mass[num3] <= 100f)) && (Grid.Properties[num3] & 2) == 0 && !spawnTimes.ContainsKey(num3))
							{
								spawnTimes[num3] = Time.realtimeSinceStartup;
								int idx = element.idx;
								float temperature = Grid.Temperature[src_cell];
								StartCoroutine(DelayedSpawnFX(src_cell, num3, num2, element, idx, temperature));
							}
						}
					}
				}
			}
		}
		return result;
	}

	private void ReleaseGO(GameObject go)
	{
		go.DeleteObject();
	}

	private IEnumerator DelayedSpawnFX(int src_cell, int dest_cell, int offset, Element elem, int idx, float temperature)
	{
		float random_delay = Random.value * 0.25f;
		yield return (object)new WaitForSeconds(random_delay);
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private void Update()
	{
		expiredCells.Clear();
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		foreach (KeyValuePair<int, float> spawnTime in spawnTimes)
		{
			if (realtimeSinceStartup - spawnTime.Value > 1f)
			{
				expiredCells.Add(spawnTime.Key);
			}
		}
		foreach (int expiredCell in expiredCells)
		{
			spawnTimes.Remove(expiredCell);
		}
		expiredCells.Clear();
	}

	public void DestroyCell(int cell, int cb_index = -1)
	{
		if (Grid.Solid[cell])
		{
			if (cb_index == -1)
			{
				if (!queuedDigCallbackCells.Contains(cell))
				{
					queuedDigCallbackCells.Add(cell);
					SimMessages.Dig(cell, -1);
				}
			}
			else
			{
				SimMessages.Dig(cell, cb_index);
			}
		}
	}

	public void OnSolidStateChanged(int cell)
	{
		Grid.Damage[cell] = 0f;
	}

	public void OnDigComplete(int cell, float mass, float temperature, byte element_idx, byte disease_idx, int disease_count)
	{
		if (queuedDigCallbackCells.Contains(cell))
		{
			queuedDigCallbackCells.Remove(cell);
			Vector3 vector = Grid.CellToPos(cell, CellAlignment.RandomInternal, Grid.SceneLayer.Ore);
			Element element = ElementLoader.elements[element_idx];
			Grid.Damage[cell] = 0f;
			Instance.PlaySoundForSubstance(element, vector);
			float num = mass * 0.5f;
			if (!(num <= 0f))
			{
				GameObject gameObject = element.substance.SpawnResource(vector, num, temperature, disease_idx, disease_count, false, false, false);
				Pickupable component = gameObject.GetComponent<Pickupable>();
				if ((Object)component != (Object)null && WorldInventory.Instance.IsReachable(gameObject.GetComponent<Pickupable>()))
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, Mathf.RoundToInt(num).ToString() + " " + element.name, gameObject.transform, 1.5f, false);
				}
			}
		}
	}

	private void PlaySoundForSubstance(Element element, Vector3 pos)
	{
		string text = element.substance.GetMiningBreakSound();
		if (text == null)
		{
			text = (element.HasTag(GameTags.RefinedMetal) ? "RefinedMetal" : ((!element.HasTag(GameTags.Metal)) ? "Rock" : "RawMetal"));
		}
		text = "Break_" + text;
		text = GlobalAssets.GetSound(text, false);
		if ((bool)CameraController.Instance && CameraController.Instance.IsAudibleSound(pos, text))
		{
			KFMOD.PlayOneShot(text, CameraController.Instance.GetVerticallyScaledPosition(pos));
		}
	}
}
