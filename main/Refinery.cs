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
public class Refinery : KMonoBehaviour, IEffectDescriptor, IHasBuildQueue, ISim200ms
{
	[Serializable]
	public class UserOrder : IBuildQueueOrder
	{
		public ComplexRecipe recipe;

		public bool infinite;

		private Dictionary<Tag, float> materialRequirements = new Dictionary<Tag, float>();

		public Tag Result => recipe.results[0].material;

		public Sprite Icon => recipe.GetUIIcon();

		public Color IconColor => recipe.GetUIColor();

		public bool Infinite => infinite;

		public UserOrder(ComplexRecipe recipe, bool infinite = false)
		{
			this.recipe = recipe;
			this.infinite = infinite;
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

		public bool underway;

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

		public void Complete()
		{
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

	private ProgressBar progressBar;

	protected RefineryWorkable workable;

	public bool duplicantOperated = true;

	private bool isCancellingOrder;

	public bool labelByResult = true;

	public RefinerySideScreen.StyleSetting sideScreenStyle;

	public Action<UserOrder> OnCreateOrder;

	public Action<UserOrder> OnOrderCancelledOrComplete;

	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	[MyCmpGet]
	private OutputPoint outputPoint;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpAdd]
	private RefinerySM refinerySM;

	[SerializeField]
	public Storage inStorage;

	[SerializeField]
	public Storage buildStorage;

	[SerializeField]
	public Storage outStorage;

	public Vector3 outputOffset = Vector3.zero;

	protected List<UserOrder> userOrders = new List<UserOrder>();

	protected List<MachineOrder> machineOrders = new List<MachineOrder>();

	private const int MaxPrefetchCount = 3;

	private MeterController outputVisualizer;

	[SerializeField]
	public ResultState resultState;

	[SerializeField]
	public bool storeProduced;

	[Serialize]
	private List<OrderSaveData> savedOrders;

	protected ChoreType choreType;

	protected Tag[] choreTags;

	private static readonly EventSystem.IntraObjectHandler<Refinery> OnDroppedAllDelegate = new EventSystem.IntraObjectHandler<Refinery>(delegate(Refinery component, object data)
	{
		component.OnDroppedAll(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Refinery> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Refinery>(delegate(Refinery component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Refinery> OnBuildingUpgradedDelegate = new EventSystem.IntraObjectHandler<Refinery>(delegate(Refinery component, object data)
	{
		component.OnBuildingUpgraded(data);
	});

	private float orderProgress;

	public RefineryWorkable GetWorkable
	{
		get
		{
			if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
			{
				return workable;
			}
			workable = GetComponent<RefineryWorkable>();
			return workable;
		}
	}

	public int NumOrders => userOrders.Count;

	public List<IBuildQueueOrder> Orders => userOrders.ConvertAll((Converter<UserOrder, IBuildQueueOrder>)((UserOrder o) => o));

	public bool WaitingForWorker => machineOrders.Count > 0 && machineOrders[0].fetchList != null && machineOrders[0].fetchList.IsComplete;

	public bool HasWorker => !duplicantOperated || (UnityEngine.Object)workable.worker != (UnityEngine.Object)null;

	public MachineOrder CurrentMachineOrder => (machineOrders.Count <= 0) ? null : machineOrders[0];

	public List<MachineOrder> GetMachineOrders => machineOrders;

	[OnSerializing]
	internal void OnSerializingMethod()
	{
		savedOrders = new List<OrderSaveData>();
		for (int i = 0; i < userOrders.Count; i++)
		{
			UserOrder userOrder = userOrders[i];
			savedOrders.Add(new OrderSaveData(userOrder.recipe.id, userOrder.infinite));
		}
	}

	[OnDeserializing]
	internal void OnDeserializingMethod()
	{
		savedOrders = new List<OrderSaveData>();
	}

	public string GetConversationTopic()
	{
		if (machineOrders.Count > 0)
		{
			UserOrder parentOrder = machineOrders[0].parentOrder;
			ComplexRecipe recipe = parentOrder.recipe;
			return recipe.results[0].material.Name;
		}
		return null;
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
		Components.Refineries.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (duplicantOperated)
		{
			workable = GetComponent<RefineryWorkable>();
		}
		Subscribe(-235298596, OnBuildingUpgradedDelegate);
		ReloadSavedQueue();
		buildStorage.Transfer(inStorage, true, true);
		UpdateOrderQueue(true);
	}

	private void Cancel(UserOrder order)
	{
		isCancellingOrder = true;
		for (int num = machineOrders.Count - 1; num >= 0; num--)
		{
			MachineOrder machineOrder = machineOrders[num];
			if (machineOrder.parentOrder == order)
			{
				machineOrder.Cancel();
				machineOrders.RemoveAt(num);
				if (machineOrder.chore != null || machineOrder.underway)
				{
					buildStorage.Transfer(inStorage, true, true);
				}
			}
		}
		if (OnOrderCancelledOrComplete != null)
		{
			OnOrderCancelledOrComplete(order);
		}
		isCancellingOrder = false;
	}

	protected override void OnCleanUp()
	{
		foreach (UserOrder userOrder in userOrders)
		{
			Cancel(userOrder);
		}
		Components.Refineries.Remove(this);
		base.OnCleanUp();
	}

	protected virtual void ProduceSolidProducts()
	{
	}

	protected virtual List<GameObject> CompleteOrder(UserOrder completed_order)
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
				component2.AddDisease(diseaseInfo.idx, Mathf.RoundToInt((float)diseaseInfo.count * num4), "Refinery.CompleteOrder");
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
		if (!completed_order.infinite && OnOrderCancelledOrComplete != null)
		{
			OnOrderCancelledOrComplete(completed_order);
		}
		return list;
	}

	public void CreateOrder(ComplexRecipe recipe, bool isInfinite, string soundPath)
	{
		if (DebugHandler.InstantBuildMode)
		{
			UserOrder userOrder = new UserOrder(recipe, false);
			if (OnCreateOrder != null)
			{
				OnCreateOrder(userOrder);
			}
			CompleteOrder(userOrder);
		}
		else if (userOrders.Count < 6)
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
	}

	private void UpdateOrderQueue(bool force_update = false)
	{
		if (force_update || operational.IsOperational)
		{
			int num = 0;
			while (num < userOrders.Count && machineOrders.Count < 3)
			{
				UserOrder userOrder = userOrders[num];
				if (!AlreadyMachineQueued(userOrder) || userOrder.infinite)
				{
					MachineOrder machineOrder = new MachineOrder();
					machineOrder.parentOrder = userOrder;
					machineOrders.Add(machineOrder);
				}
				if (!userOrder.infinite)
				{
					num++;
				}
			}
			if (machineOrders.Count > 0)
			{
				MachineOrder machineOrder2 = machineOrders[0];
				if (machineOrder2.chore == null)
				{
					ComplexRecipe recipe = machineOrder2.parentOrder.recipe;
					bool flag = true;
					ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
					foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
					{
						if (inStorage.GetUnitsAvailable(recipeElement.material) < recipeElement.amount)
						{
							flag = false;
						}
					}
					if (flag)
					{
						machineOrder2.underway = true;
						ComplexRecipe.RecipeElement[] ingredients2 = recipe.ingredients;
						foreach (ComplexRecipe.RecipeElement recipeElement2 in ingredients2)
						{
							inStorage.Transfer(buildStorage, recipeElement2.material, recipeElement2.amount, false, true);
						}
						if (duplicantOperated)
						{
							workable.CreateOrder(machineOrder2, choreType, choreTags);
						}
						OnBuildQueued(machineOrder2);
					}
				}
				Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
				for (int k = 0; k < machineOrders.Count; k++)
				{
					MachineOrder machineOrder3 = machineOrders[k];
					if (machineOrder3.chore == null)
					{
						UserOrder parentOrder = machineOrder3.parentOrder;
						ComplexRecipe recipe2 = parentOrder.recipe;
						ComplexRecipe.RecipeElement[] ingredients3 = recipe2.ingredients;
						foreach (ComplexRecipe.RecipeElement recipeElement3 in ingredients3)
						{
							dictionary[recipeElement3.material] = inStorage.GetUnitsAvailable(recipeElement3.material);
						}
					}
				}
				for (int m = 0; m < machineOrders.Count; m++)
				{
					MachineOrder machineOrder4 = machineOrders[m];
					if (machineOrder4.chore == null)
					{
						UserOrder parentOrder2 = machineOrder4.parentOrder;
						ComplexRecipe recipe3 = parentOrder2.recipe;
						List<KeyValuePair<Tag, float>> list = new List<KeyValuePair<Tag, float>>();
						ComplexRecipe.RecipeElement[] ingredients4 = recipe3.ingredients;
						foreach (ComplexRecipe.RecipeElement recipeElement4 in ingredients4)
						{
							float num2;
							if (dictionary[recipeElement4.material] < recipeElement4.amount)
							{
								num2 = recipeElement4.amount - dictionary[recipeElement4.material];
								dictionary[recipeElement4.material] = 0f;
							}
							else
							{
								Dictionary<Tag, float> dictionary2;
								Tag material;
								(dictionary2 = dictionary)[material = recipeElement4.material] = dictionary2[material] - recipeElement4.amount;
								num2 = 0f;
							}
							if (num2 > 0f)
							{
								list.Add(new KeyValuePair<Tag, float>(recipeElement4.material, num2));
							}
						}
						int priorityMod = -m;
						if (machineOrder4.fetchList == null && list.Count > 0)
						{
							machineOrder4.fetchList = new FetchList2(inStorage, Db.Get().ChoreTypes.MachineFetch, choreTags);
							machineOrder4.fetchList.ShowStatusItem = false;
							machineOrder4.fetchList.SetPriorityMod(priorityMod);
							foreach (KeyValuePair<Tag, float> item in list)
							{
								FetchList2 fetchList = machineOrder4.fetchList;
								Tag key = item.Key;
								float value = item.Value;
								fetchList.Add(key, null, null, value, FetchOrder2.OperationalRequirement.None);
							}
							machineOrder4.fetchList.Submit(OnFetchComplete, false);
						}
						else if (machineOrder4.fetchList != null)
						{
							machineOrder4.fetchList.SetPriorityMod(priorityMod);
						}
					}
				}
				try
				{
					if (machineOrder2.chore == null && machineOrder2.fetchList != null)
					{
						machineOrder2.fetchList.ShowStatusItem = true;
					}
				}
				catch
				{
					Debug.Log("!", null);
				}
			}
			Trigger(1721324763, this);
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

	private void StopWork()
	{
		GetComponent<Operational>().SetActive(false, false);
		ShowProgressBar(false);
	}

	private void OnFetchComplete()
	{
		UpdateOrderQueue(false);
	}

	private bool AlreadyMachineQueued(UserOrder user_order)
	{
		bool result = false;
		foreach (MachineOrder machineOrder in machineOrders)
		{
			if (machineOrder.parentOrder == user_order)
			{
				return true;
			}
		}
		return result;
	}

	public virtual void CancelOrder(int idx)
	{
		if (idx == 0 && duplicantOperated)
		{
			workable.OnCancelOrder();
		}
		if (idx < userOrders.Count)
		{
			UserOrder order = userOrders[idx];
			Cancel(order);
			userOrders.RemoveAt(idx);
			operational.SetActive(false, false);
		}
		if (userOrders.Count == 0)
		{
			CancelAll();
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

	private bool CanFabricate(UserOrder order)
	{
		return CanFabricate(order, inStorage);
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
			string text = string.Empty;
			string text2 = string.Empty;
			ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				text = text + "• " + string.Format(UI.BUILDINGEFFECTS.PROCESSEDITEM, string.Empty, recipeElement.material.ProperName());
				text2 += string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.PROCESSEDITEM, string.Join(", ", (from r in complexRecipe.results
				select r.material.ProperName()).ToArray()));
			}
			Descriptor item2 = new Descriptor(text, text2, Descriptor.DescriptorType.Effect, false);
			item2.IncreaseIndent();
			list.Add(item2);
		}
		return list;
	}

	public void OnCompleteWork()
	{
		if (!isCancellingOrder)
		{
			if (machineOrders.Count <= 0)
			{
				Debug.LogWarning("Somehow we tried to complete an order when there was no orders to complete. Need more info on how to reproduce this for a proper fix.", null);
			}
			else
			{
				MachineOrder machineOrder = machineOrders[0];
				machineOrder.Complete();
				if (!machineOrder.parentOrder.infinite)
				{
					userOrders.RemoveAt(0);
				}
				machineOrders.RemoveAt(0);
				operational.SetActive(false, false);
				CompleteOrder(machineOrder.parentOrder);
				buildStorage.Transfer(inStorage, false, false);
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
			CancelAll();
		}
	}

	private void OnBuildingUpgraded(object data)
	{
		for (int num = userOrders.Count - 1; num >= 0; num--)
		{
			CancelOrder(num);
		}
		CancelAll();
	}

	private void CancelAll()
	{
		buildStorage.Transfer(inStorage, true, true);
		while (machineOrders.Count > 0)
		{
			MachineOrder machineOrder = machineOrders[0];
			machineOrder.Cancel();
			if (machineOrders.Count > 0 && machineOrders[0] == machineOrder)
			{
				machineOrders.RemoveAt(0);
			}
		}
	}

	protected virtual void OnBuildQueued(MachineOrder order)
	{
	}

	public virtual List<Descriptor> AdditionalEffectsForRecipe(ComplexRecipe recipe)
	{
		return new List<Descriptor>();
	}

	public void Sim200ms(float dt)
	{
		if (!duplicantOperated)
		{
			if (machineOrders.Count > 0 && (machineOrders[0].fetchList == null || machineOrders[0].fetchList.IsComplete || machineOrders[0].underway))
			{
				if (!operational.IsActive)
				{
					machineOrders[0].underway = true;
					StartWork();
				}
				orderProgress += dt / machineOrders[0].parentOrder.recipe.time;
				if (orderProgress >= 1f)
				{
					machineOrders[0].underway = false;
					if (machineOrders.Count == 1)
					{
						StopWork();
					}
					OnCompleteWork();
					orderProgress = 0f;
				}
			}
			else if (!operational.IsActive && machineOrders.Count > 0)
			{
				if (buildStorage.MassStored() > 0f)
				{
					buildStorage.Transfer(inStorage, false, false);
				}
				UpdateOrderQueue(true);
			}
		}
	}
}
