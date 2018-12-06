using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ComplexFabricator : KMonoBehaviour, ISim200ms
{
	protected enum SubAnim
	{
		Queued,
		Full,
		On,
		Use
	}

	public enum ResultState
	{
		Normal,
		Hot,
		Melted
	}

	[Serializable]
	public class UserOrder
	{
		public ComplexRecipe recipe;

		public Tag Result => recipe.results[0].material;

		public Sprite Icon => recipe.GetUIIcon();

		public Color IconColor => recipe.GetUIColor();

		public UserOrder(ComplexRecipe recipe, bool infinite = false)
		{
			this.recipe = recipe;
		}

		public bool CheckMaterialRequirements(WorldInventory worldInventory, Storage storage)
		{
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				if (worldInventory.GetAmount(recipeElement.material) + storage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
				{
					return false;
				}
			}
			return true;
		}
	}

	public class MachineOrder
	{
		public UserOrder parentOrder;

		public FetchList2 fetchList;

		public Chore chore;

		public bool underway = false;

		public void Cancel()
		{
			if (chore != null)
			{
				chore.Cancel("Fabrication cancelled");
				chore = null;
			}
			if (fetchList != null)
			{
				fetchList.Cancel("Fabrication cancelled");
				fetchList = null;
			}
		}
	}

	[Serializable]
	public struct OrderSaveData
	{
		public string id;

		public bool infinite;

		public OrderSaveData(string id, bool infinite)
		{
			this.id = id;
			this.infinite = infinite;
		}
	}

	public bool duplicantOperated = true;

	protected ComplexFabricatorWorkable workable;

	public Action<UserOrder> OnUserOrderCancelledOrComplete;

	public Action<MachineOrder> OnCreateMachineOrder;

	public Action<MachineOrder> OnMachineOrderCancelledOrComplete;

	[SerializeField]
	public HashedString fetchChoreTypeIdHash = Db.Get().ChoreTypes.MachineFetch.IdHash;

	[SerializeField]
	public ResultState resultState = ResultState.Normal;

	[SerializeField]
	public bool storeProduced = false;

	public ComplexFabricatorSideScreen.StyleSetting sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;

	public bool labelByResult = true;

	public Vector3 outputOffset = Vector3.zero;

	private const int MaxPrefetchCount = 3;

	protected ChoreType choreType;

	protected Tag[] choreTags;

	public static int QUEUE_INFINITE_COUNT = 6;

	[Serialize]
	private Dictionary<string, int> recipeQueueCounts = new Dictionary<string, int>();

	[Serialize]
	public bool clearUserOrderOnComplete;

	protected List<UserOrder> userOrders = new List<UserOrder>();

	protected List<MachineOrder> machineOrders = new List<MachineOrder>();

	[Serialize]
	private int currentOrderIdx;

	private bool isCancellingOrder = false;

	private float orderProgress = 0f;

	private ComplexRecipe[] possible_recipes_cache;

	private SchedulerHandle ingredientSearchHandle;

	[SerializeField]
	public Storage inStorage;

	[SerializeField]
	public Storage buildStorage;

	[SerializeField]
	public Storage outStorage;

	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	[MyCmpGet]
	private OutputPoint outputPoint;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpAdd]
	private ComplexFabricatorSM fabricatorSM;

	private MeterController outputVisualizer;

	private ProgressBar progressBar;

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnDroppedAllDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnDroppedAll(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnCopySettings(data);
	});

	public ComplexFabricatorWorkable GetWorkable
	{
		get
		{
			if (!((UnityEngine.Object)workable != (UnityEngine.Object)null))
			{
				workable = GetComponent<ComplexFabricatorWorkable>();
				return workable;
			}
			return workable;
		}
	}

	public int CurrentOrderIdx => currentOrderIdx;

	public MachineOrder CurrentMachineOrder => (machineOrders.Count <= 0) ? null : machineOrders[0];

	public bool IsSearchHandleActive => ingredientSearchHandle.IsValid;

	public int NumOrders => userOrders.Count;

	public bool WaitingForWorker => machineOrders.Count > 0 && machineOrders[0].fetchList != null && machineOrders[0].fetchList.IsComplete;

	public bool HasWorker => !duplicantOperated || (UnityEngine.Object)workable.worker != (UnityEngine.Object)null;

	public List<UserOrder> GetUserOrders()
	{
		return userOrders;
	}

	public List<MachineOrder> GetMachineOrders()
	{
		return machineOrders;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		choreType = Db.Get().ChoreTypes.Fabricate;
		choreTags = new Tag[1]
		{
			GameTags.ChoreTypes.Fabricating
		};
		Subscribe(-1957399615, OnDroppedAllDelegate);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		if (duplicantOperated)
		{
			GetWorkable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
			GetWorkable.AttributeConvertor = Db.Get().AttributeConverters.MachinerySpeed;
			GetWorkable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		}
		Components.ComplexFabricators.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (duplicantOperated)
		{
			workable = GetComponent<ComplexFabricatorWorkable>();
		}
		InitRecipeQueueCount();
		RefreshUserOrdersFromQueueCounts();
		buildStorage.Transfer(inStorage, true, true);
		UpdateMachineOrders(true);
		Subscribe(-905833192, OnCopySettingsDelegate);
		Subscribe(-1697596308, OnStorageChanged);
	}

	private void OnStorageChanged(object data = null)
	{
		if (machineOrders.Count > 0 && (machineOrders[0].chore != null || (!duplicantOperated && machineOrders[0].underway)) && (!((UnityEngine.Object)workable != (UnityEngine.Object)null) || !(workable.WorkTimeRemaining < 0f)) && !(orderProgress >= machineOrders[0].parentOrder.recipe.time))
		{
			ComplexRecipe.RecipeElement[] ingredients = machineOrders[0].parentOrder.recipe.ingredients;
			int num = 0;
			while (true)
			{
				if (num >= ingredients.Length)
				{
					return;
				}
				ComplexRecipe.RecipeElement recipeElement = ingredients[num];
				if (buildStorage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
				{
					break;
				}
				num++;
			}
			CancelAllMachineOrders();
			UpdateMachineOrders(false);
		}
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
		{
			ComplexFabricator component = gameObject.GetComponent<ComplexFabricator>();
			if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
			{
				string[] array = recipeQueueCounts.Keys.ToArray();
				foreach (string key in array)
				{
					if (component.recipeQueueCounts.ContainsKey(key))
					{
						recipeQueueCounts[key] = component.recipeQueueCounts[key];
					}
					else
					{
						recipeQueueCounts[key] = 0;
					}
				}
				RefreshUserOrdersFromQueueCounts();
				UpdateMachineOrders(false);
			}
		}
	}

	protected override void OnCleanUp()
	{
		ingredientSearchHandle.ClearScheduler();
		foreach (UserOrder userOrder in userOrders)
		{
			CancelMachineOrdersByUserOrder(userOrder);
		}
		Components.ComplexFabricators.Remove(this);
		base.OnCleanUp();
	}

	public ComplexRecipe[] GetRecipes()
	{
		if (possible_recipes_cache == null)
		{
			KPrefabID component = GetComponent<KPrefabID>();
			Tag prefabTag = component.PrefabTag;
			List<ComplexRecipe> recipes = ComplexRecipeManager.Get().recipes;
			List<ComplexRecipe> list = new List<ComplexRecipe>();
			foreach (ComplexRecipe item in recipes)
			{
				foreach (Tag fabricator in item.fabricators)
				{
					if (fabricator == prefabTag)
					{
						list.Add(item);
					}
				}
			}
			possible_recipes_cache = list.ToArray();
		}
		return possible_recipes_cache;
	}

	private void InitRecipeQueueCount()
	{
		ComplexRecipe[] recipes = GetRecipes();
		foreach (ComplexRecipe complexRecipe in recipes)
		{
			bool flag = false;
			foreach (string key in recipeQueueCounts.Keys)
			{
				if (key == complexRecipe.id)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				recipeQueueCounts.Add(complexRecipe.id, 0);
			}
		}
	}

	private void RefreshUserOrdersFromQueueCounts()
	{
		UserOrder prevCurrentOrder = null;
		if (userOrders != null && currentOrderIdx != -1 && currentOrderIdx < userOrders.Count)
		{
			prevCurrentOrder = userOrders[currentOrderIdx];
		}
		userOrders.Clear();
		int num = 0;
		foreach (KeyValuePair<string, int> recipeQueueCount in recipeQueueCounts)
		{
			if (recipeQueueCount.Value > 0)
			{
				num++;
				UserOrder item = new UserOrder(ComplexRecipeManager.Get().GetRecipe(recipeQueueCount.Key), true);
				userOrders.Add(item);
			}
		}
		if (prevCurrentOrder != null)
		{
			int num2 = userOrders.FindIndex((UserOrder match) => match.recipe == prevCurrentOrder.recipe);
			if (num2 != -1)
			{
				userOrders = ShiftListLeft(userOrders, num2);
			}
		}
	}

	public List<T> ShiftListLeft<T>(List<T> list, int shiftBy)
	{
		if (list.Count > shiftBy)
		{
			List<T> range = list.GetRange(shiftBy, list.Count - shiftBy);
			range.AddRange((IEnumerable<T>)list.GetRange(0, shiftBy));
			return range;
		}
		return list;
	}

	public int GetRecipeQueueCount(ComplexRecipe recipe)
	{
		return recipeQueueCounts[recipe.id];
	}

	public void IncrementRecipeQueueCount(ComplexRecipe recipe)
	{
		bool flag = true;
		if (machineOrders.Count == 0)
		{
			flag = true;
		}
		UserOrder userOrder = userOrders.Find((UserOrder match) => match.recipe == recipe);
		if (userOrder != null && GetUserOrderIndex(userOrder) == currentOrderIdx && GetRecipeQueueCount(recipe) == QUEUE_INFINITE_COUNT)
		{
			CancelMachineOrdersByUserOrder(userOrder);
		}
		Dictionary<string, int> dictionary;
		string id;
		(dictionary = recipeQueueCounts)[id = recipe.id] = dictionary[id] + 1;
		if (recipeQueueCounts[recipe.id] > QUEUE_INFINITE_COUNT)
		{
			recipeQueueCounts[recipe.id] = 0;
			flag = true;
		}
		RefreshUserOrdersFromQueueCounts();
		if (flag)
		{
			UpdateMachineOrders(false);
		}
	}

	public void DecrementRecipeQueueCount(ComplexRecipe recipe, bool respectInfinite = true)
	{
		bool flag = false;
		UserOrder userOrder = userOrders.Find((UserOrder match) => match.recipe == recipe);
		if (userOrder != null && GetRecipeQueueCount(recipe) == 1)
		{
			if (GetUserOrderIndex(userOrder) == currentOrderIdx)
			{
				flag = true;
			}
			CancelMachineOrdersByUserOrder(userOrder);
		}
		if (!respectInfinite || recipeQueueCounts[recipe.id] != QUEUE_INFINITE_COUNT)
		{
			Dictionary<string, int> dictionary;
			string id;
			(dictionary = recipeQueueCounts)[id = recipe.id] = dictionary[id] - 1;
			if (recipeQueueCounts[recipe.id] < 0)
			{
				recipeQueueCounts[recipe.id] = QUEUE_INFINITE_COUNT;
				flag = true;
			}
		}
		RefreshUserOrdersFromQueueCounts();
		if (flag)
		{
			UpdateMachineOrders(false);
		}
	}

	private int GetTotalQueuedCount()
	{
		int num = 0;
		foreach (KeyValuePair<string, int> recipeQueueCount in recipeQueueCounts)
		{
			num += recipeQueueCount.Value;
		}
		return num;
	}

	private List<UserOrder> GetNextMachineOrders()
	{
		List<UserOrder> list = new List<UserOrder>();
		bool flag = false;
		int num = 0;
		while (list.Count < Mathf.Min(3, GetTotalQueuedCount()))
		{
			int index = (currentOrderIdx + num) % userOrders.Count;
			UserOrder userOrder = userOrders[index];
			if (userOrder.CheckMaterialRequirements(WorldInventory.Instance, inStorage) || userOrder.CheckMaterialRequirements(WorldInventory.Instance, buildStorage))
			{
				list.Add(userOrder);
			}
			else
			{
				flag = true;
			}
			num++;
			if (num > userOrders.Count * 3)
			{
				break;
			}
		}
		if (flag)
		{
			ScheduleCheckWorldInventory();
		}
		return list;
	}

	private bool CheckIngredientsInStorage(ComplexRecipe recipe)
	{
		ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			if (inStorage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
			{
				return false;
			}
		}
		return true;
	}

	private void ClearInvalidMachineOrders(List<UserOrder> nextMachineOrderSources)
	{
		int num = -1;
		for (int i = 0; i < machineOrders.Count; i++)
		{
			if (nextMachineOrderSources.Count <= i)
			{
				num = i;
				break;
			}
			if (machineOrders[i].parentOrder.recipe != nextMachineOrderSources[i].recipe)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			for (int num2 = machineOrders.Count - 1; num2 >= num; num2--)
			{
				if (num2 == 0 && machineOrders[0].chore != null)
				{
					buildStorage.Transfer(inStorage, true, true);
				}
				OnMachineOrderCancelledOrComplete(machineOrders[num2]);
				machineOrders[num2].Cancel();
				machineOrders.RemoveAt(num2);
			}
		}
	}

	private void AddValidMachineOrders(List<UserOrder> nextMachineOrderSources)
	{
		for (int i = machineOrders.Count; i < nextMachineOrderSources.Count; i++)
		{
			MachineOrder machineOrder = new MachineOrder();
			machineOrder.parentOrder = nextMachineOrderSources[i];
			machineOrders.Add(machineOrder);
			OnCreateMachineOrder(machineOrder);
		}
	}

	private void RefreshMachineOrderList()
	{
		List<UserOrder> nextMachineOrders = GetNextMachineOrders();
		ClearInvalidMachineOrders(nextMachineOrders);
		AddValidMachineOrders(nextMachineOrders);
	}

	protected void UpdateMachineOrders(bool force_update = false)
	{
		if (force_update || operational.IsOperational)
		{
			RefreshMachineOrderList();
			if (machineOrders.Count > 0)
			{
				if (!machineOrders[0].underway && machineOrders[0].parentOrder.CheckMaterialRequirements(WorldInventory.Instance, inStorage))
				{
					if (duplicantOperated)
					{
						machineOrders[0].underway = true;
					}
					else if (!operational.IsActive && !machineOrders[0].underway && CheckIngredientsInStorage(machineOrders[0].parentOrder.recipe))
					{
						TransferCurrentRecipeIngredientsForBuild();
						machineOrders[0].underway = true;
					}
				}
				if (duplicantOperated && machineOrders[0].underway && machineOrders[0].chore == null && CheckIngredientsInStorage(machineOrders[0].parentOrder.recipe))
				{
					TransferCurrentRecipeIngredientsForBuild();
					workable.CreateOrder(machineOrders[0], choreType, choreTags);
					workable.ResetWorkTime();
				}
				Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
				for (int i = 0; i < machineOrders.Count; i++)
				{
					MachineOrder machineOrder = machineOrders[i];
					if (machineOrder.chore == null)
					{
						UserOrder parentOrder = machineOrder.parentOrder;
						ComplexRecipe.RecipeElement[] ingredients = parentOrder.recipe.ingredients;
						ComplexRecipe.RecipeElement[] array = ingredients;
						foreach (ComplexRecipe.RecipeElement recipeElement in array)
						{
							dictionary[recipeElement.material] = inStorage.GetMassAvailable(recipeElement.material);
						}
					}
				}
				ChoreType byHash = Db.Get().ChoreTypes.GetByHash(fetchChoreTypeIdHash);
				Dictionary<Tag, float> dictionary2 = new Dictionary<Tag, float>();
				for (int k = 0; k < machineOrders.Count; k++)
				{
					MachineOrder machineOrder2 = machineOrders[k];
					if (machineOrder2.chore == null && (!machineOrder2.underway || duplicantOperated))
					{
						UserOrder parentOrder2 = machineOrder2.parentOrder;
						ComplexRecipe.RecipeElement[] ingredients2 = parentOrder2.recipe.ingredients;
						bool flag = true;
						ComplexRecipe.RecipeElement[] array2 = ingredients2;
						foreach (ComplexRecipe.RecipeElement recipeElement2 in array2)
						{
							if (dictionary[recipeElement2.material] < recipeElement2.amount)
							{
								if (dictionary2.ContainsKey(recipeElement2.material))
								{
									Dictionary<Tag, float> dictionary3;
									Tag material;
									(dictionary3 = dictionary2)[material = recipeElement2.material] = dictionary3[material] + (recipeElement2.amount - dictionary[recipeElement2.material]);
								}
								else
								{
									dictionary2.Add(recipeElement2.material, recipeElement2.amount - dictionary[recipeElement2.material]);
								}
								dictionary[recipeElement2.material] = 0f;
								flag = false;
							}
							else
							{
								Dictionary<Tag, float> dictionary3;
								Tag material2;
								(dictionary3 = dictionary)[material2 = recipeElement2.material] = dictionary3[material2] - recipeElement2.amount;
							}
						}
						int priorityMod = -k;
						if (machineOrder2.fetchList == null && !flag)
						{
							machineOrder2.fetchList = new FetchList2(inStorage, byHash, choreTags);
							machineOrder2.fetchList.ShowStatusItem = false;
							machineOrder2.fetchList.SetPriorityMod(priorityMod);
							ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[dictionary2.Count];
							int num = 0;
							foreach (Tag item in dictionary2.Keys.ToList())
							{
								float a = 0f;
								ComplexRecipe.RecipeElement[] ingredients3 = machineOrder2.parentOrder.recipe.ingredients;
								foreach (ComplexRecipe.RecipeElement recipeElement3 in ingredients3)
								{
									if (recipeElement3.material == item)
									{
										a = recipeElement3.amount;
										break;
									}
								}
								float num2 = Mathf.Min(a, dictionary2[item]);
								if (num2 != 0f)
								{
									array3[num] = new ComplexRecipe.RecipeElement(item, num2);
									Dictionary<Tag, float> dictionary3;
									Tag key;
									(dictionary3 = dictionary2)[key = item] = dictionary3[key] - num2;
								}
								num++;
							}
							AddIngredientsToFetchList(array3, machineOrder2.fetchList);
							machineOrder2.fetchList.Submit(OnFetchComplete, false);
						}
						else if (machineOrder2.fetchList != null)
						{
							machineOrder2.fetchList.SetPriorityMod(priorityMod);
						}
					}
				}
			}
			Trigger(1721324763, this);
			if (machineOrders.Count > 0)
			{
				SetCurrentUserOrderByMachineOrder(machineOrders[0]);
			}
		}
	}

	protected virtual void TransferCurrentRecipeIngredientsForBuild()
	{
		ComplexRecipe.RecipeElement[] ingredients = machineOrders[0].parentOrder.recipe.ingredients;
		ComplexRecipe.RecipeElement[] array = ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in array)
		{
			while (buildStorage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
			{
				inStorage.Transfer(buildStorage, recipeElement.material, recipeElement.amount, false, true);
				if (inStorage.GetAmountAvailable(recipeElement.material) <= 0f)
				{
					break;
				}
			}
		}
	}

	protected virtual bool HasIngredients(MachineOrder order, Storage storage)
	{
		ComplexRecipe.RecipeElement[] ingredients = order.parentOrder.recipe.ingredients;
		bool result = true;
		ComplexRecipe.RecipeElement[] array = ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in array)
		{
			float massAvailable = storage.GetMassAvailable(recipeElement.material);
			if (massAvailable < recipeElement.amount)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public void Sim200ms(float dt)
	{
		if (!duplicantOperated)
		{
			if (!operational.IsActive && machineOrders.Count > 0 && machineOrders[0].underway)
			{
				StartWork();
			}
			if (operational.IsActive ? true : false)
			{
				orderProgress += dt / machineOrders[0].parentOrder.recipe.time;
				if (orderProgress >= 1f)
				{
					machineOrders[0].underway = false;
					SetOperationalInactive();
					OnCompleteMachineOrder();
					orderProgress = 0f;
				}
			}
		}
	}

	private void CancelMachineOrder(MachineOrder order)
	{
		OnMachineOrderCancelledOrComplete(order);
		order.Cancel();
		machineOrders.Remove(order);
		if (!duplicantOperated && order.underway)
		{
			buildStorage.Transfer(inStorage, true, true);
			SetOperationalInactive();
			orderProgress = 0f;
		}
	}

	private void CancelMachineOrdersByUserOrder(UserOrder order)
	{
		isCancellingOrder = true;
		for (int num = machineOrders.Count - 1; num >= 0; num--)
		{
			MachineOrder machineOrder = machineOrders[num];
			if (machineOrder.parentOrder.recipe == order.recipe)
			{
				CancelMachineOrder(machineOrders[num]);
			}
		}
		if (OnUserOrderCancelledOrComplete != null)
		{
			OnUserOrderCancelledOrComplete(order);
		}
		isCancellingOrder = false;
	}

	private void CancelAllMachineOrders()
	{
		buildStorage.Transfer(inStorage, true, true);
		while (machineOrders.Count > 0)
		{
			MachineOrder machineOrder = machineOrders[0];
			machineOrder.Cancel();
			if (machineOrders.Count > 0 && machineOrders[0] == machineOrder)
			{
				OnMachineOrderCancelledOrComplete(machineOrders[0]);
				machineOrders.RemoveAt(0);
			}
		}
	}

	protected virtual List<GameObject> SpawnOrderProduct(UserOrder completed_order)
	{
		List<GameObject> list = new List<GameObject>();
		SimUtil.DiseaseInfo diseaseInfo = default(SimUtil.DiseaseInfo);
		diseaseInfo.count = 0;
		diseaseInfo.idx = 0;
		float num = 0f;
		float num2 = 0f;
		ComplexRecipe.RecipeElement[] ingredients = completed_order.recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			num2 += recipeElement.amount;
		}
		ComplexRecipe.RecipeElement[] ingredients2 = completed_order.recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement2 in ingredients2)
		{
			float num3 = recipeElement2.amount / num2;
			buildStorage.ConsumeAndGetDisease(recipeElement2.material, recipeElement2.amount, out SimUtil.DiseaseInfo disease_info, out float aggregate_temperature);
			if (disease_info.count > diseaseInfo.count)
			{
				diseaseInfo = disease_info;
			}
			num += aggregate_temperature * num3;
		}
		ComplexRecipe.RecipeElement[] results = completed_order.recipe.results;
		foreach (ComplexRecipe.RecipeElement recipeElement3 in results)
		{
			GameObject gameObject = buildStorage.FindFirst(recipeElement3.material);
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				Edible component = gameObject.GetComponent<Edible>();
				if ((bool)component)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, 0f - component.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.CRAFTED_USED, "{0}", component.GetProperName()), UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
				}
			}
			switch (resultState)
			{
			case ResultState.Normal:
			case ResultState.Hot:
			{
				GameObject prefab = Assets.GetPrefab(recipeElement3.material);
				GameObject gameObject2 = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore, null, 0);
				int cell = Grid.PosToCell(this);
				gameObject2.transform.SetPosition(Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore) + outputOffset);
				PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
				component2.Units = recipeElement3.amount;
				component2.Temperature = num;
				gameObject2.SetActive(true);
				float num4 = recipeElement3.amount / completed_order.recipe.TotalResultUnits();
				component2.AddDisease(diseaseInfo.idx, Mathf.RoundToInt((float)diseaseInfo.count * num4), "ComplexFabricator.CompleteOrder");
				gameObject2.GetComponent<KMonoBehaviour>().Trigger(748399584, null);
				list.Add(gameObject2);
				if (storeProduced)
				{
					outStorage.Store(gameObject2, false, false, true, false);
				}
				break;
			}
			case ResultState.Melted:
				if (storeProduced)
				{
					float temperature = ElementLoader.GetElement(recipeElement3.material).lowTemp + (ElementLoader.GetElement(recipeElement3.material).highTemp - ElementLoader.GetElement(recipeElement3.material).lowTemp) / 2f;
					outStorage.AddLiquid(ElementLoader.GetElementID(recipeElement3.material), recipeElement3.amount, temperature, 0, 0, false, true);
				}
				break;
			}
			if (list.Count > 0)
			{
				SymbolOverrideController component3 = GetComponent<SymbolOverrideController>();
				if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
				{
					KBatchedAnimController component4 = list[0].GetComponent<KBatchedAnimController>();
					KAnim.Build build = component4.AnimFiles[0].GetData().build;
					KAnim.Build.Symbol symbol = build.GetSymbol(build.name);
					if (symbol != null)
					{
						component3.TryRemoveSymbolOverride("output_tracker", 0);
						component3.AddSymbolOverride("output_tracker", symbol, 0);
					}
					else
					{
						Debug.LogWarning(component3.name + " is missing symbol " + build.name, null);
					}
				}
			}
		}
		return list;
	}

	private void PollInventory(object data = null)
	{
		bool flag = false;
		for (int i = 0; i < Mathf.Min(3, GetTotalQueuedCount()); i++)
		{
			int index = (currentOrderIdx + i) % userOrders.Count;
			UserOrder userOrder = userOrders[index];
			bool flag2 = false;
			foreach (MachineOrder machineOrder in machineOrders)
			{
				if (machineOrder.parentOrder.recipe == userOrder.recipe)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2 && userOrder.CheckMaterialRequirements(WorldInventory.Instance, inStorage))
			{
				flag = true;
				break;
			}
		}
		StopCheckWorldInventory();
		if (flag)
		{
			UpdateMachineOrders(false);
		}
		else
		{
			ScheduleCheckWorldInventory();
		}
	}

	private void ScheduleCheckWorldInventory()
	{
		if (!ingredientSearchHandle.IsValid)
		{
			ingredientSearchHandle = GameScheduler.Instance.Schedule("Idle ComplexFabricator look for ingredients", 4f, PollInventory, null, null);
		}
	}

	private void StopCheckWorldInventory()
	{
		if (ingredientSearchHandle.IsValid)
		{
			ingredientSearchHandle.ClearScheduler();
		}
	}

	public void SetCurrentUserOrderByMachineOrder(MachineOrder nextMachineOrder)
	{
		if (nextMachineOrder == null)
		{
			currentOrderIdx = 0;
		}
		else
		{
			currentOrderIdx = GetUserOrderIndex(nextMachineOrder.parentOrder);
		}
	}

	private void AddIngredientsToFetchList(ComplexRecipe.RecipeElement[] ingredients, FetchList2 fetchList)
	{
		if (fetchList == null || ingredients == null || ingredients.Length == 0)
		{
			Debug.LogError("Invalid parameters received for the fetch list.", null);
		}
		else
		{
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				if (recipeElement != null && recipeElement.amount > 0f)
				{
					Tag material = recipeElement.material;
					float amount = recipeElement.amount;
					fetchList.Add(material, null, null, amount, FetchOrder2.OperationalRequirement.None);
				}
			}
		}
	}

	private void StartWork()
	{
		GetComponent<Operational>().SetActive(true, false);
		ShowProgressBar(true);
	}

	public void ShowProgressBar(bool show)
	{
		if (show)
		{
			progressBar = ProgressBar.CreateProgressBar(GetComponent<Building>(), () => orderProgress);
		}
		else if ((UnityEngine.Object)progressBar != (UnityEngine.Object)null)
		{
			progressBar.gameObject.DeleteObject();
			progressBar = null;
		}
	}

	private void SetOperationalInactive()
	{
		GetComponent<Operational>().SetActive(false, false);
		ShowProgressBar(false);
	}

	private void OnFetchComplete()
	{
		UpdateMachineOrders(false);
	}

	private bool CanFabricate(UserOrder order, Storage storage)
	{
		ComplexRecipe.RecipeElement[] ingredients = order.recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			if (storage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
			{
				return false;
			}
		}
		return true;
	}

	public virtual List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		ComplexRecipe[] recipes = GetRecipes();
		if (recipes.Length > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.BUILDINGEFFECTS.PROCESSES, UI.BUILDINGEFFECTS.TOOLTIPS.PROCESSES, Descriptor.DescriptorType.Effect);
			list.Add(item);
		}
		ComplexRecipe[] array = recipes;
		foreach (ComplexRecipe complexRecipe in array)
		{
			string text = "";
			string text2 = "";
			ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				text = text + "• " + string.Format(UI.BUILDINGEFFECTS.PROCESSEDITEM, "", recipeElement.material.ProperName());
				text2 += string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.PROCESSEDITEM, string.Join(", ", (from r in complexRecipe.results
				select r.material.ProperName()).ToArray()));
			}
			Descriptor item2 = new Descriptor(text, text2, Descriptor.DescriptorType.Effect, false);
			item2.IncreaseIndent();
			list.Add(item2);
		}
		return list;
	}

	public void OnCompleteMachineOrder()
	{
		if (!isCancellingOrder)
		{
			if (machineOrders.Count <= 0)
			{
				Debug.LogWarning("Somehow we tried to complete an order when there was no orders to complete. Need more info on how to reproduce this for a proper fix.", null);
			}
			else
			{
				SpawnOrderProduct(machineOrders[0].parentOrder);
				buildStorage.Transfer(outStorage, true, true);
				OnMachineOrderCancelledOrComplete(machineOrders[0]);
				int userOrderIndex = GetUserOrderIndex(machineOrders[0].parentOrder);
				machineOrders.RemoveAt(0);
				DecrementRecipeQueueCount(userOrders[userOrderIndex].recipe, true);
				SetCurrentUserOrderByMachineOrder((machineOrders.Count <= 0) ? null : machineOrders[0]);
				UpdateMachineOrders(false);
				ShowProgressBar(false);
			}
		}
	}

	private int GetUserOrderIndex(UserOrder order)
	{
		for (int i = 0; i < userOrders.Count; i++)
		{
			if (userOrders[i].recipe == order.recipe)
			{
				return i;
			}
		}
		Debug.LogError("Could not find user order index", null);
		return -1;
	}

	private void OnDroppedAll(object data)
	{
	}

	private void OnOperationalChanged(object data)
	{
		bool flag = (bool)data;
	}

	public virtual List<Descriptor> AdditionalEffectsForRecipe(ComplexRecipe recipe)
	{
		return new List<Descriptor>();
	}

	public string GetConversationTopic()
	{
		if (machineOrders.Count <= 0)
		{
			return null;
		}
		UserOrder parentOrder = machineOrders[0].parentOrder;
		ComplexRecipe recipe = parentOrder.recipe;
		return recipe.results[0].material.Name;
	}
}
