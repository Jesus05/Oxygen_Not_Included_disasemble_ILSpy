using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class Edible : Workable, IGameObjectEffectDescriptor
{
	public class EdibleStartWorkInfo : Worker.StartWorkInfo
	{
		public float amount
		{
			get;
			private set;
		}

		public EdibleStartWorkInfo(Workable workable, float amount)
			: base(workable)
		{
			this.amount = amount;
		}
	}

	public string FoodID;

	private EdiblesManager.FoodInfo foodInfo;

	private float consumptionTime = float.NaN;

	public float unitsConsumed = float.NaN;

	public float caloriesConsumed = float.NaN;

	private AttributeModifier caloriesModifier = new AttributeModifier("CaloriesDelta", 50000f, DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, false, false, true);

	private static readonly EventSystem.IntraObjectHandler<Edible> OnCraftDelegate = new EventSystem.IntraObjectHandler<Edible>(delegate(Edible component, object data)
	{
		component.OnCraft(data);
	});

	private static readonly HashedString[] normalWorkAnims = new HashedString[2]
	{
		"working_pre",
		"working_loop"
	};

	private static readonly HashedString[] hatWorkAnims = new HashedString[2]
	{
		"hat_pre",
		"working_loop"
	};

	private static readonly HashedString normalWorkPstAnim = "working_pst";

	private static readonly HashedString hatWorkPstAnim = "hat_pst";

	private static Dictionary<int, string> qualityEffects = new Dictionary<int, string>
	{
		{
			-1,
			"EdibleMinus3"
		},
		{
			0,
			"EdibleMinus2"
		},
		{
			1,
			"EdibleMinus1"
		},
		{
			2,
			"Edible0"
		},
		{
			3,
			"Edible1"
		},
		{
			4,
			"Edible2"
		},
		{
			5,
			"Edible3"
		}
	};

	public float Units
	{
		get
		{
			return GetComponent<PrimaryElement>().Units;
		}
		set
		{
			GetComponent<PrimaryElement>().Units = value;
		}
	}

	public float Calories
	{
		get
		{
			return Units * foodInfo.CaloriesPerUnit;
		}
		set
		{
			Units = value / foodInfo.CaloriesPerUnit;
		}
	}

	public EdiblesManager.FoodInfo FoodInfo
	{
		get
		{
			return foodInfo;
		}
		set
		{
			foodInfo = value;
			FoodID = foodInfo.Id;
		}
	}

	public bool isBeingConsumed
	{
		get;
		private set;
	}

	private Edible()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
		showProgressBar = false;
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		shouldTransferDiseaseWithWorker = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (foodInfo == null)
		{
			if (FoodID == null)
			{
				Debug.LogError("No food FoodID");
			}
			foodInfo = Game.Instance.ediblesManager.GetFoodInfo(FoodID);
		}
		GetComponent<KPrefabID>().AddTag(GameTags.Edible);
		Subscribe(748399584, OnCraftDelegate);
		Subscribe(1272413801, OnCraftDelegate);
		workerStatusItem = Db.Get().DuplicantStatusItems.Eating;
		synchronizeAnims = false;
		Components.Edibles.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.Edible, this);
	}

	public override HashedString[] GetWorkAnims(Worker worker)
	{
		MinionResume component = worker.GetComponent<MinionResume>();
		if ((Object)component != (Object)null && component.CurrentHat != null)
		{
			return hatWorkAnims;
		}
		return normalWorkAnims;
	}

	public override HashedString GetWorkPstAnim(Worker worker, bool successfully_completed)
	{
		MinionResume component = worker.GetComponent<MinionResume>();
		if ((Object)component != (Object)null && component.CurrentHat != null)
		{
			return hatWorkPstAnim;
		}
		return normalWorkPstAnim;
	}

	private void OnCraft(object data)
	{
		RationTracker.Get().RegisterCaloriesProduced(Calories);
	}

	public float GetFeedingTime(Worker worker)
	{
		float num = Calories * 2E-05f;
		if ((Object)worker != (Object)null)
		{
			BingeEatChore.StatesInstance sMI = worker.GetSMI<BingeEatChore.StatesInstance>();
			if (sMI != null && sMI.IsBingeEating())
			{
				num /= 2f;
			}
		}
		return num;
	}

	protected override void OnStartWork(Worker worker)
	{
		SetWorkTime(GetFeedingTime(worker));
		worker.GetAttributes().Add(caloriesModifier);
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.AddTag(GameTags.AlwaysConverse);
		StartConsuming();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		consumptionTime += dt;
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetAttributes().Remove(caloriesModifier);
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.RemoveTag(GameTags.AlwaysConverse);
		StopConsuming(worker);
	}

	private void StartConsuming()
	{
		DebugUtil.DevAssert(!isBeingConsumed, "Can't StartConsuming()...we've already started");
		isBeingConsumed = true;
		consumptionTime = 0f;
		base.worker.Trigger(1406130139, this);
	}

	private void StopConsuming(Worker worker)
	{
		DebugUtil.DevAssert(isBeingConsumed, "StopConsuming() called without StartConsuming()");
		isBeingConsumed = false;
		if (float.IsNaN(consumptionTime))
		{
			DebugUtil.DevAssert(false, "consumptionTime NaN in StopConsuming()");
		}
		else
		{
			PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
			if ((Object)component != (Object)null && component.DiseaseCount > 0)
			{
				new EmoteChore(worker.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_react_contaminated_food_kanim", new HashedString[1]
				{
					"react"
				}, null);
			}
			float num = Mathf.Clamp01(consumptionTime / GetFeedingTime(worker));
			unitsConsumed = Units * num;
			if (float.IsNaN(unitsConsumed))
			{
				KCrashReporter.Assert(false, "Why is unitsConsumed NaN?");
				unitsConsumed = Units;
			}
			caloriesConsumed = unitsConsumed * foodInfo.CaloriesPerUnit;
			Units -= unitsConsumed;
			for (int i = 0; i < foodInfo.Effects.Count; i++)
			{
				worker.GetComponent<Effects>().Add(foodInfo.Effects[i], true);
			}
			ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, 0f - caloriesConsumed, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.EATEN, "{0}", this.GetProperName()), worker.GetProperName());
			AddQualityEffects(worker);
			worker.Trigger(1121894420, this);
			Trigger(-10536414, worker.gameObject);
			unitsConsumed = float.NaN;
			caloriesConsumed = float.NaN;
			consumptionTime = float.NaN;
			if (Units <= 0f)
			{
				base.gameObject.DeleteObject();
			}
		}
	}

	public static string GetEffectForFoodQuality(int qualityLevel)
	{
		qualityLevel = Mathf.Clamp(qualityLevel, -1, 5);
		return qualityEffects[qualityLevel];
	}

	private void AddQualityEffects(Worker worker)
	{
		Attributes attributes = worker.GetAttributes();
		AttributeInstance attributeInstance = attributes.Add(Db.Get().Attributes.FoodExpectation);
		float totalValue = attributeInstance.GetTotalValue();
		int num = Mathf.RoundToInt(totalValue);
		int qualityLevel = FoodInfo.Quality + num;
		Effects component = worker.GetComponent<Effects>();
		component.Add(GetEffectForFoodQuality(qualityLevel), true);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Edibles.Remove(this);
	}

	public int GetQuality()
	{
		return foodInfo.Quality;
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories(foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.CALORIES, GameUtil.GetFormattedCalories(foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.Information, false));
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(foodInfo.Quality)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(foodInfo.Quality)), Descriptor.DescriptorType.Effect, false));
		foreach (string effect in foodInfo.Effects)
		{
			list.Add(new Descriptor(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + effect.ToUpper() + ".NAME"), Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + effect.ToUpper() + ".DESCRIPTION"), Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}
}
