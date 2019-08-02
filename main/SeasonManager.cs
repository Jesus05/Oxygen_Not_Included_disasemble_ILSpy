using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SeasonManager : KMonoBehaviour, ISim200ms
{
	private struct BombardmentInfo
	{
		public string prefab;

		public float weight;
	}

	private struct Season
	{
		public string name;

		public int durationInCycles;

		public MathUtil.MinMax secondsBombardmentOff;

		public MathUtil.MinMax secondsBombardmentOn;

		public MathUtil.MinMax secondsBetweenBombardments;

		public bool meteorBackground;

		public BombardmentInfo[] bombardmentInfo;
	}

	[Serialize]
	private int currentSeasonIndex = 2147483647;

	[Serialize]
	private int currentSeasonsCyclesElapsed = 2147483647;

	[Serialize]
	private float bombardmentPeriodRemaining;

	[Serialize]
	private bool bombardmentOn;

	[Serialize]
	private float secondsUntilNextBombardment;

	private GameObject activeMeteorBackground;

	private const string SEASONNAME_DEFAULT = "Default";

	private const string SEASONNAME_METEORSHOWER_IRON = "MeteorShowerIron";

	private const string SEASONNAME_METEORSHOWER_GOLD = "MeteorShowerGold";

	private const string SEASONNAME_METEORSHOWER_COPPER = "MeteorShowerCopper";

	private Dictionary<string, Season> seasons = new Dictionary<string, Season>
	{
		{
			"Default",
			new Season
			{
				durationInCycles = 4
			}
		},
		{
			"MeteorShowerIron",
			new Season
			{
				durationInCycles = 10,
				secondsBombardmentOff = new MathUtil.MinMax(300f, 1200f),
				secondsBombardmentOn = new MathUtil.MinMax(100f, 400f),
				secondsBetweenBombardments = new MathUtil.MinMax(1f, 1.5f),
				meteorBackground = true,
				bombardmentInfo = new BombardmentInfo[3]
				{
					new BombardmentInfo
					{
						prefab = IronCometConfig.ID,
						weight = 1f
					},
					new BombardmentInfo
					{
						prefab = RockCometConfig.ID,
						weight = 2f
					},
					new BombardmentInfo
					{
						prefab = DustCometConfig.ID,
						weight = 5f
					}
				}
			}
		},
		{
			"MeteorShowerGold",
			new Season
			{
				durationInCycles = 5,
				secondsBombardmentOff = new MathUtil.MinMax(800f, 1200f),
				secondsBombardmentOn = new MathUtil.MinMax(50f, 100f),
				secondsBetweenBombardments = new MathUtil.MinMax(0.3f, 0.5f),
				meteorBackground = true,
				bombardmentInfo = new BombardmentInfo[3]
				{
					new BombardmentInfo
					{
						prefab = GoldCometConfig.ID,
						weight = 2f
					},
					new BombardmentInfo
					{
						prefab = RockCometConfig.ID,
						weight = 0.5f
					},
					new BombardmentInfo
					{
						prefab = DustCometConfig.ID,
						weight = 5f
					}
				}
			}
		},
		{
			"MeteorShowerCopper",
			new Season
			{
				durationInCycles = 7,
				secondsBombardmentOff = new MathUtil.MinMax(300f, 1200f),
				secondsBombardmentOn = new MathUtil.MinMax(100f, 400f),
				secondsBetweenBombardments = new MathUtil.MinMax(4f, 6.5f),
				meteorBackground = true,
				bombardmentInfo = new BombardmentInfo[2]
				{
					new BombardmentInfo
					{
						prefab = CopperCometConfig.ID,
						weight = 1f
					},
					new BombardmentInfo
					{
						prefab = RockCometConfig.ID,
						weight = 1f
					}
				}
			}
		}
	};

	private string[] SeasonLoop = new string[6]
	{
		"Default",
		"MeteorShowerIron",
		"Default",
		"MeteorShowerCopper",
		"Default",
		"MeteorShowerGold"
	};

	private static readonly EventSystem.IntraObjectHandler<SeasonManager> OnNewDayDelegate = new EventSystem.IntraObjectHandler<SeasonManager>(delegate(SeasonManager component, object data)
	{
		component.OnNewDay(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(631075836, OnNewDayDelegate);
		if (currentSeasonIndex >= SeasonLoop.Length)
		{
			currentSeasonIndex = SeasonLoop.Length - 1;
			currentSeasonsCyclesElapsed = 2147483647;
		}
		UpdateState();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Unsubscribe(631075836, OnNewDayDelegate, false);
	}

	private void OnNewDay(object data)
	{
		currentSeasonsCyclesElapsed++;
		UpdateState();
	}

	private void UpdateState()
	{
		Season season = seasons[SeasonLoop[currentSeasonIndex]];
		if (currentSeasonsCyclesElapsed >= season.durationInCycles)
		{
			currentSeasonIndex = (currentSeasonIndex + 1) % SeasonLoop.Length;
			ResetSeasonProgress();
		}
	}

	private void ResetSeasonProgress()
	{
		Season season = seasons[SeasonLoop[currentSeasonIndex]];
		currentSeasonsCyclesElapsed = 0;
		bombardmentOn = false;
		bombardmentPeriodRemaining = season.secondsBombardmentOff.Get();
		secondsUntilNextBombardment = season.secondsBetweenBombardments.Get();
	}

	public void Sim200ms(float dt)
	{
		Season season = seasons[SeasonLoop[currentSeasonIndex]];
		bombardmentPeriodRemaining -= dt;
		if (bombardmentPeriodRemaining <= 0f)
		{
			float num = bombardmentPeriodRemaining;
			bombardmentOn = !bombardmentOn;
			bombardmentPeriodRemaining = ((!bombardmentOn) ? season.secondsBombardmentOff.Get() : season.secondsBombardmentOn.Get());
			if (bombardmentPeriodRemaining != 0f)
			{
				bombardmentPeriodRemaining += num;
			}
		}
		if (bombardmentOn && season.bombardmentInfo != null && season.bombardmentInfo.Length > 0)
		{
			if ((UnityEngine.Object)activeMeteorBackground == (UnityEngine.Object)null)
			{
				activeMeteorBackground = Util.KInstantiate(EffectPrefabs.Instance.MeteorBackground, null, null);
				activeMeteorBackground.transform.SetPosition(new Vector3(125f, 435f, 25f));
				activeMeteorBackground.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
			}
			secondsUntilNextBombardment -= dt;
			if (secondsUntilNextBombardment <= 0f)
			{
				float num2 = secondsUntilNextBombardment;
				DoBombardment(season.bombardmentInfo);
				secondsUntilNextBombardment = season.secondsBetweenBombardments.Get();
				if (secondsUntilNextBombardment != 0f)
				{
					secondsUntilNextBombardment += num2;
				}
			}
		}
		else if ((UnityEngine.Object)activeMeteorBackground != (UnityEngine.Object)null)
		{
			ParticleSystem component = activeMeteorBackground.GetComponent<ParticleSystem>();
			component.Stop();
			if (!component.IsAlive())
			{
				UnityEngine.Object.Destroy(activeMeteorBackground);
				activeMeteorBackground = null;
			}
		}
	}

	private void DoBombardment(BombardmentInfo[] bombardment_info)
	{
		float num = 0f;
		for (int i = 0; i < bombardment_info.Length; i++)
		{
			BombardmentInfo bombardmentInfo = bombardment_info[i];
			num += bombardmentInfo.weight;
		}
		num = UnityEngine.Random.Range(0f, num);
		BombardmentInfo bombardmentInfo2 = bombardment_info[0];
		int num2 = 0;
		while (num - bombardmentInfo2.weight > 0f)
		{
			num -= bombardmentInfo2.weight;
			bombardmentInfo2 = bombardment_info[++num2];
		}
		Game.Instance.Trigger(-84771526, null);
		SpawnBombard(bombardmentInfo2.prefab);
	}

	private GameObject SpawnBombard(string prefab)
	{
		GameObject gameObject = Util.KInstantiate(position: new Vector3(UnityEngine.Random.value * (float)Grid.WidthInCells, 1.2f * (float)Grid.HeightInCells, Grid.GetLayerZ(Grid.SceneLayer.FXFront)), original: Assets.GetPrefab(prefab), rotation: Quaternion.identity, parent: null, name: null, initialize_id: true, gameLayer: 0);
		gameObject.SetActive(true);
		return gameObject;
	}

	public bool CurrentSeasonHasBombardment()
	{
		Season season = seasons[SeasonLoop[currentSeasonIndex]];
		return season.bombardmentInfo != null && season.bombardmentInfo.Length > 0;
	}

	public float TimeUntilNextBombardment()
	{
		return (!CurrentSeasonHasBombardment()) ? 3.40282347E+38f : ((!bombardmentOn) ? bombardmentPeriodRemaining : 0f);
	}

	public float GetBombardmentDuration()
	{
		if (CurrentSeasonHasBombardment())
		{
			Season season = seasons[SeasonLoop[currentSeasonIndex]];
			return (!bombardmentOn) ? season.secondsBombardmentOn.Get() : 0f;
		}
		return 0f;
	}

	public void ForceBeginMeteorSeasonWithShower()
	{
		for (int i = 0; i < SeasonLoop.Length; i++)
		{
			if (SeasonLoop[i] == "MeteorShowerIron")
			{
				currentSeasonIndex = i;
			}
		}
		ResetSeasonProgress();
		Season season = seasons[SeasonLoop[currentSeasonIndex]];
		bombardmentOn = true;
		bombardmentPeriodRemaining = season.secondsBombardmentOn.Get();
	}

	[ContextMenu("Bombard")]
	public void Debug_Bombardment()
	{
		Season season = seasons[SeasonLoop[currentSeasonIndex]];
		BombardmentInfo[] bombardmentInfo = season.bombardmentInfo;
		DoBombardment(bombardmentInfo);
	}

	[ContextMenu("Force Shower")]
	public void Debug_ForceShower()
	{
		currentSeasonIndex = Array.IndexOf(SeasonLoop, "MeteorShowerIron");
		ResetSeasonProgress();
		bombardmentOn = true;
		bombardmentPeriodRemaining = 3.40282347E+38f;
		secondsUntilNextBombardment = 0f;
	}

	public void DrawDebugger()
	{
	}
}
