using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ComplexFabricator : KMonoBehaviour, IEffectDescriptor, IHasBuildQueue, ISim200ms
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
	public class UserOrder : IBuildQueueOrder
	{
		public ComplexRecipe recipe;

		private Dictionary<Tag, float> materialRequirements = new Dictionary<Tag, float>();

		public Tag Result => recipe.results[0].material;

		public Sprite Icon => recipe.GetUIIcon();

		public Color IconColor => recipe.GetUIColor();

		public UserOrder(ComplexRecipe recipe, bool infinite = false)
		{
			this.recipe = recipe;
		}

		public Dictionary<Tag, float> CheckMaterialRequirements()
		{
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				float amount = WorldInventory.Instance.GetAmount(recipeElement.material);
				dictionary[recipeElement.material] = recipeElement.amount - amount;
			}
			return dictionary;
		}

		public Dictionary<Tag, float> GetMaterialRequirements()
		{
			materialRequirements.Clear();
			ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				materialRequirements.Add(recipeElement.material, recipeElement.amount);
			}
			return materialRequirements;
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

	public Action<UserOrder> OnCreateOrder;

	public Action<UserOrder> OnUserOrderCancelledOrComplete;

	public Action<MachineOrder> OnCreateMachineOrder;

	public Action<MachineOrder> OnMachineOrderCancelledOrComplete;

	[SerializeField]
	public HashedString fetchChoreTypeIdHash = Db.Get().ChoreTypes.MachineFetch.IdHash;

	[SerializeField]
	public ResultState resultState = ResultState.Normal;

	[SerializeField]
	public bool storeProduced = false;

	public ComplexFabricatorSideScreen.StyleSetting sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.GridResult;

	public bool labelByResult = true;

	public Vector3 outputOffset = Vector3.zero;

	private const int MaxPrefetchCount = 3;

	protected ChoreType choreType;

	protected Tag[] choreTags;

	[Serialize]
	public Dictionary<ComplexRecipe, int> recipeQueueCounts = new Dictionary<ComplexRecipe, int>();

	[Serialize]
	public bool clearUserOrderOnComplete;

	[Serialize]
	private List<OrderSaveData> savedOrders;

	protected List<UserOrder> userOrders = new List<UserOrder>();

	protected List<MachineOrder> machineOrders = new List<MachineOrder>();

	[Serialize]
	private int currentOrderIdx;

	private bool isCancellingOrder = false;

	private float orderProgress = 0f;

	private List<Guid> missingIngredientStatusItems = new List<Guid>();

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

	public static int QUEUE_INFINITE_COUNT = 6;

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnDroppedAllDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnDroppedAll(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public int MAX_NUM_ORDERS => GetRecipes().Length;

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

	public List<IBuildQueueOrder> Orders => userOrders.ConvertAll((Converter<UserOrder, IBuildQueueOrder>)((UserOrder o) => o));

	public bool WaitingForWorker => machineOrders.Count > 0 && machineOrders[0].fetchList != null && machineOrders[0].fetchList.IsComplete;

	public bool HasWorker => !duplicantOperated || (UnityEngine.Object)workable.worker != (UnityEngine.Object)null;

	public int MaxOrders => MAX_NUM_ORDERS;

	public List<UserOrder> GetUserOrders()
	{
		return userOrders;
	}

	public List<MachineOrder> GetMachineOrders()
	{
		return machineOrders;
	}

	[OnSerializing]
	internal void OnSerializingMethod()
	{
		savedOrders = new List<OrderSaveData>();
		for (int i = 0; i < userOrders.Count; i++)
		{
			UserOrder userOrder = userOrders[i];
			savedOrders.Add(new OrderSaveData(userOrder.recipe.id, true));
		}
	}

	[OnDeserializing]
	internal void OnDeserializingMethod()
	{
		savedOrders = new List<OrderSaveData>();
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

	public ComplexRecipe[] GetRecipes()
	{
		Tag b = GetComponent<KPrefabID>().PrefabID();
		List<ComplexRecipe> recipes = ComplexRecipeManager.Get().recipes;
		List<ComplexRecipe> list = new List<ComplexRecipe>();
		foreach (ComplexRecipe item in recipes)
		{
			foreach (Tag fabricator in item.fabricators)
			{
				if (fabricator == b)
				{
					list.Add(item);
				}
			}
		}
		return list.ToArray();
	}

	private void ClearQueueCount()
	{
		recipeQueueCounts.Clear();
		ComplexRecipe[] recipes = GetRecipes();
		foreach (ComplexRecipe key in recipes)
		{
			recipeQueueCounts.Add(key, 0);
		}
	}

	private void ConfigUserOrdersForQueueCount()
	{
		ClearQueueCount();
		userOrders.Clear();
		foreach (KeyValuePair<ComplexRecipe, int> recipeQueueCount in recipeQueueCounts)
		{
			UserOrder userOrder = new UserOrder(recipeQueueCount.Key, true);
			if (OnCreateOrder != null)
			{
				OnCreateOrder(userOrder);
			}
			userOrders.Add(userOrder);
		}
	}

	private void ReloadSavedQueue()
	{
		userOrders.Clear();
		buildStorage.Transfer(inStorage, true, true);
		if (savedOrders != null)
		{
			bool flag = true;
			foreach (OrderSaveData savedOrder in savedOrders)
			{
				OrderSaveData current = savedOrder;
				ComplexRecipeManager complexRecipeManager = ComplexRecipeManager.Get();
				ComplexRecipe complexRecipe = complexRecipeManager.GetRecipe(current.id);
				if (complexRecipe == null)
				{
					complexRecipe = complexRecipeManager.GetObsoleteRecipe(current.id);
				}
				if (complexRecipe != null)
				{
					UserOrder userOrder = new UserOrder(complexRecipe, current.infinite);
					if (OnCreateOrder != null)
					{
						OnCreateOrder(userOrder);
					}
					userOrders.Add(userOrder);
					if (flag && duplicantOperated)
					{
						workable.SetWorkTime(complexRecipe.time);
						flag = false;
					}
				}
			}
			savedOrders = null;
		}
	}

	public int GetRecipeQueueCount(ComplexRecipe recipe)
	{
		return recipeQueueCounts[recipe];
	}

	public void IncrementRecipeQueueCount(ComplexRecipe recipe)
	{
		Dictionary<ComplexRecipe, int> dictionary;
		ComplexRecipe key;
		(dictionary = recipeQueueCounts)[key = recipe] = dictionary[key] + 1;
		if (recipeQueueCounts[recipe] > QUEUE_INFINITE_COUNT)
		{
			recipeQueueCounts[recipe] = QUEUE_INFINITE_COUNT;
		}
		UpdateOrderQueue(false);
	}

	public void DecrementRecipeQueueCount(ComplexRecipe recipe, bool respectInfinite = true)
	{
		if (!respectInfinite || recipeQueueCounts[recipe] != QUEUE_INFINITE_COUNT)
		{
			Dictionary<ComplexRecipe, int> dictionary;
			ComplexRecipe key;
			(dictionary = recipeQueueCounts)[key = recipe] = dictionary[key] - 1;
			if (recipeQueueCounts[recipe] < 0)
			{
				recipeQueueCounts[recipe] = 0;
			}
		}
		UpdateOrderQueue(false);
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
		ReloadSavedQueue();
		ConfigUserOrdersForQueueCount();
		buildStorage.Transfer(inStorage, true, true);
		UpdateOrderQueue(true);
	}

	protected void UpdateOrderQueue(bool force_update = false)
	{
		UpdateMissingIngredientStatusItems();
		if (force_update || operational.IsOperational)
		{
			bool flag = false;
			int num = 0;
			while (!flag && num < userOrders.Count)
			{
				int num2 = -1;
				for (int i = 0; i < machineOrders.Count; i++)
				{
					if (machineOrders[i].parentOrder != userOrders[(i + currentOrderIdx) % userOrders.Count])
					{
						num2 = i;
						break;
					}
					if (recipeQueueCounts[userOrders[(i + currentOrderIdx) % userOrders.Count].recipe] == 0)
					{
						num2 = i;
						break;
					}
				}
				if (num2 != -1)
				{
					for (int num3 = machineOrders.Count - 1; num3 >= num2; num3--)
					{
						if (num3 == 0 && machineOrders[0].chore != null)
						{
							buildStorage.Transfer(inStorage, true, true);
						}
						OnMachineOrderCancelledOrComplete(machineOrders[num3]);
						machineOrders[num3].Cancel();
						machineOrders.RemoveAt(num3);
					}
				}
				if (userOrders != null && userOrders.Count > 0)
				{
					int count = machineOrders.Count;
					int num4 = 0;
					int num5 = 0;
					int num6 = 0;
					foreach (KeyValuePair<ComplexRecipe, int> recipeQueueCount in recipeQueueCounts)
					{
						num6 += recipeQueueCount.Value;
					}
					while (machineOrders.Count < Mathf.Min(3, num6) && num5 < userOrders.Count)
					{
						int index = (currentOrderIdx + count + num4) % userOrders.Count;
						UserOrder userOrder = userOrders[index];
						bool flag2 = false;
						if (recipeQueueCounts[userOrder.recipe] == 0)
						{
							flag2 = true;
						}
						else
						{
							foreach (KeyValuePair<Tag, float> item in userOrder.CheckMaterialRequirements())
							{
								if (WorldInventory.Instance.GetAmount(item.Key) < item.Value && inStorage.GetAmountAvailable(item.Key) < item.Value)
								{
									flag2 = true;
									break;
								}
							}
						}
						if (!flag2)
						{
							MachineOrder machineOrder = new MachineOrder();
							machineOrder.parentOrder = userOrder;
							machineOrders.Add(machineOrder);
							OnCreateMachineOrder(machineOrder);
							num5 = 0;
						}
						else
						{
							num5++;
						}
						num4++;
					}
				}
				if (machineOrders.Count > 0)
				{
					if ((duplicantOperated && machineOrders[0].chore == null) || (!duplicantOperated && !machineOrders[0].underway))
					{
						ComplexRecipe.RecipeElement[] ingredients = machineOrders[0].parentOrder.recipe.ingredients;
						bool flag3 = true;
						ComplexRecipe.RecipeElement[] array = ingredients;
						foreach (ComplexRecipe.RecipeElement recipeElement in array)
						{
							if (inStorage.GetMassAvailable(recipeElement.material) < recipeElement.amount && buildStorage.GetMassAvailable(recipeElement.material) < recipeElement.amount)
							{
								flag3 = false;
								break;
							}
						}
						if (flag3)
						{
							flag = true;
						}
						else
						{
							currentOrderIdx += 1 % userOrders.Count;
							num++;
						}
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					num++;
				}
			}
			if (!flag)
			{
				if (!ingredientSearchHandle.IsValid)
				{
					ingredientSearchHandle = GameScheduler.Instance.Schedule("Idle ComplexFabricator look for ingredients", 4f, CheckWorldInventoryForIngredients, null, null);
				}
			}
			else
			{
				ingredientSearchHandle.ClearScheduler();
			}
			if (machineOrders.Count > 0)
			{
				if (((duplicantOperated && machineOrders[0].chore == null) || (!duplicantOperated && !machineOrders[0].underway)) && HasIngredients(machineOrders[0], inStorage))
				{
					TransferCurrentRecipeIngredientsForBuild();
					if (duplicantOperated)
					{
						workable.CreateOrder(machineOrders[0], choreType, choreTags);
					}
					else if (!operational.IsActive && !machineOrders[0].underway)
					{
						machineOrders[0].underway = true;
					}
				}
				Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
				for (int k = 0; k < machineOrders.Count; k++)
				{
					MachineOrder machineOrder2 = machineOrders[k];
					if (machineOrder2.chore == null)
					{
						UserOrder parentOrder = machineOrder2.parentOrder;
						ComplexRecipe.RecipeElement[] ingredients2 = parentOrder.recipe.ingredients;
						ComplexRecipe.RecipeElement[] array2 = ingredients2;
						foreach (ComplexRecipe.RecipeElement recipeElement2 in array2)
						{
							dictionary[recipeElement2.material] = inStorage.GetMassAvailable(recipeElement2.material);
						}
					}
				}
				ChoreType byHash = Db.Get().ChoreTypes.GetByHash(fetchChoreTypeIdHash);
				Dictionary<Tag, float> dictionary2 = new Dictionary<Tag, float>();
				for (int m = 0; m < machineOrders.Count; m++)
				{
					MachineOrder machineOrder3 = machineOrders[m];
					if (machineOrder3.chore == null && !machineOrder3.underway)
					{
						UserOrder parentOrder2 = machineOrder3.parentOrder;
						ComplexRecipe.RecipeElement[] ingredients3 = parentOrder2.recipe.ingredients;
						bool flag4 = true;
						ComplexRecipe.RecipeElement[] array3 = ingredients3;
						foreach (ComplexRecipe.RecipeElement recipeElement3 in array3)
						{
							if (dictionary[recipeElement3.material] < recipeElement3.amount)
							{
								if (dictionary2.ContainsKey(recipeElement3.material))
								{
									Dictionary<Tag, float> dictionary3;
									Tag material;
									(dictionary3 = dictionary2)[material = recipeElement3.material] = dictionary3[material] + (recipeElement3.amount - dictionary[recipeElement3.material]);
								}
								else
								{
									dictionary2.Add(recipeElement3.material, recipeElement3.amount - dictionary[recipeElement3.material]);
								}
								dictionary[recipeElement3.material] = 0f;
								flag4 = false;
							}
							else
							{
								Dictionary<Tag, float> dictionary3;
								Tag material2;
								(dictionary3 = dictionary)[material2 = recipeElement3.material] = dictionary3[material2] - recipeElement3.amount;
							}
						}
						int priorityMod = -m;
						if (machineOrder3.fetchList == null && !flag4)
						{
							machineOrder3.fetchList = new FetchList2(inStorage, byHash, choreTags);
							machineOrder3.fetchList.ShowStatusItem = false;
							machineOrder3.fetchList.SetPriorityMod(priorityMod);
							ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[dictionary2.Count];
							int num7 = 0;
							foreach (Tag item2 in dictionary2.Keys.ToList())
							{
								float a = 0f;
								ComplexRecipe.RecipeElement[] ingredients4 = machineOrder3.parentOrder.recipe.ingredients;
								foreach (ComplexRecipe.RecipeElement recipeElement4 in ingredients4)
								{
									if (recipeElement4.material == item2)
									{
										a = recipeElement4.amount;
										break;
									}
								}
								float num9 = Mathf.Min(a, dictionary2[item2]);
								if (num9 != 0f)
								{
									array4[num7] = new ComplexRecipe.RecipeElement(item2, num9);
									Dictionary<Tag, float> dictionary3;
									Tag key;
									(dictionary3 = dictionary2)[key = item2] = dictionary3[key] - num9;
								}
								num7++;
							}
							AddIngredientsToFetchList(array4, machineOrder3.fetchList);
							machineOrder3.fetchList.Submit(OnFetchComplete, false);
						}
						else if (machineOrder3.fetchList != null)
						{
							machineOrder3.fetchList.SetPriorityMod(priorityMod);
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
			inStorage.Transfer(buildStorage, recipeElement.material, recipeElement.amount, false, true);
		}
	}

	protected virtual bool HasIngredients(MachineOrder order, Storage storage)
	{
		ComplexRecipe.RecipeElement[] ingredients = order.parentOrder.recipe.ingredients;
		bool result = true;
		ComplexRecipe.RecipeElement[] array = ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in array)
		{
			if (storage.GetMassAvailable(recipeElement.material) < recipeElement.amount)
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
			if (operational.IsActive)
			{
				bool flag = true;
				if (!HasIngredients(machineOrders[0], buildStorage))
				{
					flag = false;
					orderProgress = 0f;
					for (int num = machineOrders.Count - 1; num >= 0; num--)
					{
						CancelMachineOrder(machineOrders[num]);
					}
					SetOperationalInactive();
					UpdateOrderQueue(false);
				}
				if (flag)
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
	}

	public void CreateUserOrder(ComplexRecipe recipe, bool isInfinite, string soundPath)
	{
		if (DebugHandler.InstantBuildMode)
		{
			UserOrder userOrder = new UserOrder(recipe, false);
			if (OnCreateOrder != null)
			{
				OnCreateOrder(userOrder);
			}
			SpawnOrderProduct(userOrder);
		}
		else
		{
			if (userOrders.Count < MAX_NUM_ORDERS)
			{
				KFMOD.PlayOneShot(soundPath);
				UserOrder userOrder2 = new UserOrder(recipe, isInfinite);
				if (OnCreateOrder != null)
				{
					OnCreateOrder(userOrder2);
				}
				userOrders.Add(userOrder2);
				UpdateOrderQueue(false);
			}
			else
			{
				UISounds.PlaySound(UISounds.Sound.Negative);
			}
			UpdateOrderQueue(false);
		}
	}

	private void CancelMachineOrder(MachineOrder order)
	{
		OnMachineOrderCancelledOrComplete(order);
		order.Cancel();
		machineOrders.Remove(order);
		if (order.chore != null || order.underway)
		{
			buildStorage.Transfer(inStorage, true, true);
		}
	}

	private void CancelMachineOrdersByUserOrder(UserOrder order)
	{
		isCancellingOrder = true;
		for (int num = machineOrders.Count - 1; num >= 0; num--)
		{
			MachineOrder machineOrder = machineOrders[num];
			if (machineOrder.parentOrder == order)
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
		ComplexRecipe.RecipeElement[] results = completed_order.recipe.results;
		foreach (ComplexRecipe.RecipeElement recipeElement in results)
		{
			GameObject gameObject = buildStorage.FindFirst(recipeElement.material);
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				Edible component = gameObject.GetComponent<Edible>();
				if ((bool)component)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, 0f - component.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.CRAFTED_USED, "{0}", component.GetProperName()), UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
				}
			}
			SimUtil.DiseaseInfo diseaseInfo = default(SimUtil.DiseaseInfo);
			diseaseInfo.count = 0;
			diseaseInfo.idx = 0;
			float num = 0f;
			float num2 = 0f;
			ComplexRecipe.RecipeElement[] ingredients = completed_order.recipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement2 in ingredients)
			{
				num2 += recipeElement2.amount;
			}
			ComplexRecipe.RecipeElement[] ingredients2 = completed_order.recipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement3 in ingredients2)
			{
				float num3 = recipeElement3.amount / num2;
				buildStorage.ConsumeAndGetDisease(recipeElement3.material, recipeElement3.amount, out SimUtil.DiseaseInfo disease_info, out float aggregate_temperature);
				if (disease_info.count > diseaseInfo.count)
				{
					diseaseInfo = disease_info;
				}
				num += aggregate_temperature * num3;
			}
			switch (resultState)
			{
			case ResultState.Normal:
			case ResultState.Hot:
			{
				GameObject prefab = Assets.GetPrefab(recipeElement.material);
				GameObject gameObject2 = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore, null, 0);
				int cell = Grid.PosToCell(this);
				gameObject2.transform.SetPosition(Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore) + outputOffset);
				PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
				component2.Units = recipeElement.amount;
				component2.Temperature = num;
				gameObject2.SetActive(true);
				float num4 = recipeElement.amount / completed_order.recipe.TotalResultUnits();
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
					float temperature = ElementLoader.GetElement(recipeElement.material).lowTemp + (ElementLoader.GetElement(recipeElement.material).highTemp - ElementLoader.GetElement(recipeElement.material).lowTemp) / 2f;
					outStorage.AddLiquid(ElementLoader.GetElementID(recipeElement.material), recipeElement.amount, temperature, 0, 0, false, true);
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

	private void CheckWorldInventoryForIngredients(object data)
	{
		ingredientSearchHandle.ClearScheduler();
		if (userOrders.Count != 0)
		{
			for (int i = 0; i < userOrders.Count; i++)
			{
				UserOrder userOrder = userOrders[(i + currentOrderIdx) % userOrders.Count];
				bool flag = true;
				ComplexRecipe.RecipeElement[] ingredients = userOrder.recipe.ingredients;
				foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
				{
					if (WorldInventory.Instance.GetAmount(recipeElement.material) < recipeElement.amount)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					SetCurrentUserOrderByIndex((i + currentOrderIdx) % userOrders.Count);
					return;
				}
			}
			ingredientSearchHandle = GameScheduler.Instance.Schedule("Idle ComplexFabricator look for ingredients", 4f, CheckWorldInventoryForIngredients, null, null);
		}
	}

	private void UpdateMissingIngredientStatusItems()
	{
		KSelectable component = base.gameObject.GetComponent<KSelectable>();
		for (int num = missingIngredientStatusItems.Count - 1; num >= 0; num--)
		{
			component.RemoveStatusItem(missingIngredientStatusItems[num], true);
		}
		List<ComplexRecipe> list = new List<ComplexRecipe>();
		list.Clear();
		foreach (UserOrder userOrder in userOrders)
		{
			if (!list.Contains(userOrder.recipe) && recipeQueueCounts[userOrder.recipe] != 0)
			{
				ComplexRecipe.RecipeElement[] ingredients = userOrder.recipe.ingredients;
				foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
				{
					if (WorldInventory.Instance.GetAmount(recipeElement.material) < recipeElement.amount && buildStorage.GetAmountAvailable(recipeElement.material) < recipeElement.amount && inStorage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
					{
						list.Add(userOrder.recipe);
						break;
					}
				}
			}
		}
		foreach (ComplexRecipe item in list)
		{
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			ComplexRecipe.RecipeElement[] ingredients2 = item.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement2 in ingredients2)
			{
				if (WorldInventory.Instance.GetAmount(recipeElement2.material) < recipeElement2.amount && buildStorage.GetAmountAvailable(recipeElement2.material) < recipeElement2.amount && inStorage.GetAmountAvailable(recipeElement2.material) < recipeElement2.amount)
				{
					dictionary.Add(recipeElement2.material, recipeElement2.amount);
				}
			}
			missingIngredientStatusItems.Add(component.AddStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailable, dictionary));
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
			currentOrderIdx = userOrders.IndexOf(nextMachineOrder.parentOrder);
		}
	}

	public void SetCurrentUserOrderByIndex(int userOrderIndex)
	{
		if (userOrderIndex >= userOrders.Count)
		{
			Debug.LogError("User order index is out of range: " + userOrderIndex + " / " + userOrders.Count, null);
		}
		if (currentOrderIdx != userOrderIndex)
		{
			buildStorage.Transfer(inStorage, true, true);
			SetOperationalInactive();
			currentOrderIdx = userOrderIndex;
			UpdateOrderQueue(false);
			if (duplicantOperated)
			{
				workable.ResetWorkTime();
			}
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
		SetCurrentUserOrderByMachineOrder(machineOrders[0]);
		UpdateOrderQueue(false);
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
		UpdateOrderQueue(false);
	}

	public virtual void CancelUserOrder(int idx)
	{
		if (currentOrderIdx == idx)
		{
			buildStorage.Transfer(inStorage, true, true);
			if (duplicantOperated)
			{
				workable.ResetWorkTime();
			}
			else
			{
				orderProgress = 0f;
				SetOperationalInactive();
			}
		}
		else if (idx < currentOrderIdx)
		{
			currentOrderIdx--;
		}
		UserOrder order = userOrders[idx];
		CancelMachineOrdersByUserOrder(order);
		userOrders.RemoveAt(idx);
		if (userOrders.Count == 0)
		{
			CancelAllMachineOrders();
		}
		UpdateOrderQueue(false);
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
		ComplexRecipe[] recipes2 = GetRecipes();
		foreach (ComplexRecipe complexRecipe in recipes2)
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
				int num = userOrders.IndexOf(machineOrders[0].parentOrder);
				machineOrders.RemoveAt(0);
				DecrementRecipeQueueCount(userOrders[num].recipe, true);
				if (clearUserOrderOnComplete)
				{
					CancelUserOrder(num);
				}
				SetCurrentUserOrderByMachineOrder((machineOrders.Count <= 0) ? null : machineOrders[0]);
				UpdateOrderQueue(false);
				ShowProgressBar(false);
			}
		}
	}

	private void OnDroppedAll(object data)
	{
		UpdateOrderQueue(false);
	}

	private void OnOperationalChanged(object data)
	{
		if ((bool)data)
		{
			UpdateOrderQueue(false);
		}
		else if (userOrders.Count > 0)
		{
			CancelAllMachineOrders();
		}
	}

	public virtual List<Descriptor> AdditionalEffectsForRecipe(ComplexRecipe recipe)
	{
		return new List<Descriptor>();
	}
}
