using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Fabricator : Workable, IEffectDescriptor, IHasBuildQueue
{
	[Serializable]
	public class UserOrder : IBuildQueueOrder
	{
		public Recipe recipe;

		public List<Tag> orderTags;

		public bool infinite;

		private Dictionary<Tag, float> materialRequirements = new Dictionary<Tag, float>();

		public Tag Result => recipe.Result;

		public Sprite Icon => recipe.GetUIIcon();

		public Color IconColor => recipe.GetUIColor();

		public bool Infinite => infinite;

		public UserOrder(Recipe recipe, List<Tag> orderTags, Action<UserOrder> on_create_order, bool infinite = false)
		{
			this.recipe = recipe;
			this.orderTags = orderTags;
			this.infinite = infinite;
			on_create_order(this);
		}

		public void Cleanup(Action<UserOrder> on_order_cancelled_or_complete)
		{
			on_order_cancelled_or_complete(this);
		}

		public Dictionary<Tag, float> CheckMaterialRequirements()
		{
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			for (int i = 0; i < recipe.Ingredients.Count; i++)
			{
				Recipe.Ingredient ingredient = recipe.Ingredients[i];
				float amount = ingredient.amount;
				float amount2 = WorldInventory.Instance.GetAmount(ingredient.tag);
				dictionary[ingredient.tag] = amount - amount2;
			}
			return dictionary;
		}

		public Dictionary<Tag, float> GetMaterialRequirements()
		{
			materialRequirements.Clear();
			foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
			{
				materialRequirements.Add(ingredient.tag, ingredient.amount);
			}
			return materialRequirements;
		}
	}

	public class MachineOrder
	{
		public UserOrder parentOrder;

		public FetchList2 fetchList;

		public Chore chore;

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
		public string recipePrefab;

		public string[] tagNames;

		public bool infinite;

		public OrderSaveData(string recipePrefab, string[] tagNames, bool infinite)
		{
			this.recipePrefab = recipePrefab;
			this.tagNames = tagNames;
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

	private bool isCancellingOrder;

	public const int MAX_NUM_ORDERS = 6;

	public Action<UserOrder> OnCreateOrder;

	public Action<UserOrder> OnOrderCancelledOrComplete;

	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	[MyCmpGet]
	private OutputPoint outputPoint;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpAdd]
	private FabricatorSM fabricatorSM;

	[SerializeField]
	public Storage inStorage;

	[SerializeField]
	public Storage buildStorage;

	[SerializeField]
	public Storage outStorage;

	protected List<UserOrder> userOrders = new List<UserOrder>();

	protected List<MachineOrder> machineOrders = new List<MachineOrder>();

	private const int MaxPrefetchCount = 3;

	[SerializeField]
	public ResultState resultState;

	[Serialize]
	private List<OrderSaveData> savedOrders;

	public bool hideRecipesUndiscoveredIngredients;

	protected ChoreType choreType;

	[SerializeField]
	public HashedString fetchChoreTypeIdHash;

	[SerializeField]
	public Tag[] choreTags;

	private static readonly EventSystem.IntraObjectHandler<Fabricator> OnDroppedAllDelegate = new EventSystem.IntraObjectHandler<Fabricator>(delegate(Fabricator component, object data)
	{
		component.OnDroppedAll(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Fabricator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Fabricator>(delegate(Fabricator component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Fabricator> OnBuildingUpgradedDelegate = new EventSystem.IntraObjectHandler<Fabricator>(delegate(Fabricator component, object data)
	{
		component.OnBuildingUpgraded(data);
	});

	public int NumOrders => userOrders.Count;

	public List<IBuildQueueOrder> Orders => userOrders.ConvertAll((Converter<UserOrder, IBuildQueueOrder>)((UserOrder o) => o));

	public bool WaitingForWorker => machineOrders.Count > 0 && machineOrders[0].fetchList != null && machineOrders[0].fetchList.IsComplete;

	public bool HasWorker => (UnityEngine.Object)base.worker != (UnityEngine.Object)null;

	[OnSerializing]
	internal void OnSerializingMethod()
	{
		savedOrders = new List<OrderSaveData>();
		for (int i = 0; i < userOrders.Count; i++)
		{
			UserOrder userOrder = userOrders[i];
			string[] array = new string[userOrder.orderTags.Count];
			bool infinite = userOrders[i].infinite;
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = userOrder.orderTags[j].Name;
			}
			savedOrders.Add(new OrderSaveData(userOrder.recipe.Result.Name, array, infinite));
		}
	}

	[OnDeserializing]
	internal void OnDeserializingMethod()
	{
		savedOrders = new List<OrderSaveData>();
	}

	public override string GetConversationTopic()
	{
		if (machineOrders.Count > 0)
		{
			UserOrder parentOrder = machineOrders[0].parentOrder;
			Recipe recipe = parentOrder.recipe;
			return recipe.Result.Name;
		}
		return base.GetConversationTopic();
	}

	public Recipe[] GetRecipes()
	{
		string name = GetComponent<KPrefabID>().PrefabID().Name;
		List<Recipe> recipes = RecipeManager.Get().recipes;
		List<Recipe> list = new List<Recipe>();
		foreach (Recipe item in recipes)
		{
			string[] fabricators = item.fabricators;
			foreach (string text in fabricators)
			{
				if (text != null && text == name)
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
		Recipe[] recipes = GetRecipes();
		buildStorage.Transfer(inStorage, true, true);
		if (savedOrders != null)
		{
			bool flag = true;
			foreach (OrderSaveData savedOrder in savedOrders)
			{
				OrderSaveData current = savedOrder;
				string[] tagNames = current.tagNames;
				if (tagNames != null)
				{
					string recipePrefab = current.recipePrefab;
					bool flag2 = false;
					for (int i = 0; i < recipes.Length; i++)
					{
						if (recipes[i].Result.Name == recipePrefab)
						{
							List<Tag> list = new List<Tag>();
							for (int j = 0; j < tagNames.Length; j++)
							{
								list.Add(new Tag(tagNames[j]));
							}
							flag2 = true;
							userOrders.Add(new UserOrder(recipes[i], list, OnCreateOrder, current.infinite));
							if (flag)
							{
								SetWorkTime(recipes[i].FabricationTime);
								flag = false;
							}
							break;
						}
					}
					if (!flag2)
					{
						Output.LogWarning("Order failed, missing recipe [", recipePrefab, "]");
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
		if (!fetchChoreTypeIdHash.IsValid)
		{
			fetchChoreTypeIdHash = Db.Get().ChoreTypes.Fetch.IdHash;
		}
		Subscribe(-1957399615, OnDroppedAllDelegate);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		Components.Fabricators.Add(this);
		workerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-235298596, OnBuildingUpgradedDelegate);
		ReloadSavedQueue();
		buildStorage.Transfer(inStorage, true, true);
		UpdateOrderQueue(true);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (operational.IsOperational)
		{
			operational.SetActive(true, false);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		operational.SetActive(false, false);
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
				if (machineOrder.chore != null)
				{
					buildStorage.Transfer(inStorage, true, true);
				}
			}
		}
		order.Cleanup(OnOrderCancelledOrComplete);
		isCancellingOrder = false;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(MachineTechnician.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("PowerTechnician", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("MechatronicEngineer", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		foreach (UserOrder userOrder in userOrders)
		{
			Cancel(userOrder);
		}
		Components.Fabricators.Remove(this);
	}

	protected virtual GameObject CompleteOrder(UserOrder completed_order)
	{
		GameObject gameObject = completed_order.recipe.Craft(buildStorage, completed_order.orderTags);
		gameObject.transform.SetLocalPosition(base.transform.GetLocalPosition());
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		switch (resultState)
		{
		case ResultState.Normal:
		{
			int cell2 = Grid.PosToCell(this);
			gameObject.transform.SetPosition(Grid.CellToPosCCC(cell2, Grid.SceneLayer.Ore));
			break;
		}
		case ResultState.Hot:
		{
			int cell = Grid.PosToCell(this);
			component.gameObject.transform.SetPosition(Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore));
			component.Temperature = component.Element.highTemp - 10f;
			break;
		}
		case ResultState.Melted:
		{
			int outputCell = outputPoint.GetOutputCell();
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			SimMessages.AddRemoveSubstance(outputCell, component.Element.highTempTransition.id, CellEventLogger.Instance.FabricatorProduceMelted, component2.Mass, component.Element.highTempTransition.defaultValues.temperature + 10f, byte.MaxValue, 0, true, -1);
			component.gameObject.DeleteObject();
			break;
		}
		}
		if (!completed_order.infinite)
		{
			completed_order.Cleanup(OnOrderCancelledOrComplete);
		}
		return gameObject;
	}

	public override float GetWorkTime()
	{
		if (machineOrders.Count > 0)
		{
			MachineOrder machineOrder = machineOrders[0];
			workTime = machineOrder.parentOrder.recipe.FabricationTime;
			return workTime;
		}
		return -1f;
	}

	public void CreateOrder(Recipe recipe, List<Tag> tags, bool isInfinite, string soundPath)
	{
		if (DebugHandler.InstantBuildMode)
		{
			UserOrder completed_order = new UserOrder(recipe, tags, OnCreateOrder, false);
			CompleteOrder(completed_order);
		}
		else if (userOrders.Count < 6)
		{
			KFMOD.PlayOneShot(soundPath);
			UserOrder item = new UserOrder(recipe, tags, OnCreateOrder, isInfinite);
			userOrders.Add(item);
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
					Recipe.Ingredient[] allIngredients = machineOrder2.parentOrder.recipe.GetAllIngredients(machineOrder2.parentOrder.orderTags);
					bool flag = true;
					Recipe.Ingredient[] array = allIngredients;
					foreach (Recipe.Ingredient ingredient in array)
					{
						if (inStorage.GetMassAvailable(ingredient.tag) < ingredient.amount)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						machineOrder2.chore = new WorkChore<Fabricator>(choreType, this, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
						if (workTimeRemaining <= 0f)
						{
							workTimeRemaining = GetWorkTime();
						}
						Recipe.Ingredient[] array2 = allIngredients;
						foreach (Recipe.Ingredient ingredient2 in array2)
						{
							inStorage.Transfer(buildStorage, ingredient2.tag, ingredient2.amount, false, true);
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
						Recipe.Ingredient[] allIngredients2 = parentOrder.recipe.GetAllIngredients(parentOrder.orderTags);
						Recipe.Ingredient[] array3 = allIngredients2;
						foreach (Recipe.Ingredient ingredient3 in array3)
						{
							dictionary[ingredient3.tag] = inStorage.GetMassAvailable(ingredient3.tag);
						}
					}
				}
				ChoreType byHash = Db.Get().ChoreTypes.GetByHash(fetchChoreTypeIdHash);
				for (int m = 0; m < machineOrders.Count; m++)
				{
					MachineOrder machineOrder4 = machineOrders[m];
					if (machineOrder4.chore == null)
					{
						UserOrder parentOrder2 = machineOrder4.parentOrder;
						Recipe.Ingredient[] allIngredients3 = parentOrder2.recipe.GetAllIngredients(parentOrder2.orderTags);
						bool flag2 = true;
						Recipe.Ingredient[] array4 = allIngredients3;
						foreach (Recipe.Ingredient ingredient4 in array4)
						{
							if (dictionary[ingredient4.tag] < ingredient4.amount)
							{
								ingredient4.amount -= dictionary[ingredient4.tag];
								dictionary[ingredient4.tag] = 0f;
								flag2 = false;
							}
							else
							{
								Dictionary<Tag, float> dictionary2;
								Tag tag;
								(dictionary2 = dictionary)[tag = ingredient4.tag] = dictionary2[tag] - ingredient4.amount;
								ingredient4.amount = 0f;
							}
						}
						int priorityMod = -m;
						if (machineOrder4.fetchList == null && !flag2)
						{
							machineOrder4.fetchList = new FetchList2(inStorage, byHash, choreTags);
							machineOrder4.fetchList.ShowStatusItem = false;
							machineOrder4.fetchList.SetPriorityMod(priorityMod);
							AddIngredientsToFetchList(allIngredients3, machineOrder4.fetchList);
							machineOrder4.fetchList.Submit(OnFetchComplete, false);
						}
						else if (machineOrder4.fetchList != null)
						{
							machineOrder4.fetchList.SetPriorityMod(priorityMod);
						}
					}
				}
				if (machineOrder2.chore == null)
				{
					machineOrder2.fetchList.ShowStatusItem = true;
				}
			}
		}
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

	private void AddIngredientsToFetchList(Recipe.Ingredient[] ingredients, FetchList2 fetchList)
	{
		if (fetchList == null || ingredients == null || ingredients.Length == 0)
		{
			Debug.LogError("Invalid parameters received for the fetch list.", null);
		}
		else
		{
			foreach (Recipe.Ingredient ingredient in ingredients)
			{
				if (ingredient.amount > 0f)
				{
					Tag tag = ingredient.tag;
					float amount = ingredient.amount;
					fetchList.Add(tag, null, null, amount, FetchOrder2.OperationalRequirement.None);
				}
			}
		}
	}

	public virtual void CancelOrder(int idx)
	{
		if (idx == 0)
		{
			workTimeRemaining = GetWorkTime();
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
		Recipe.Ingredient[] allIngredients = order.recipe.GetAllIngredients(order.orderTags);
		Recipe.Ingredient[] array = allIngredients;
		foreach (Recipe.Ingredient ingredient in array)
		{
			float amount = 0f;
			if (!storage.IsMaterialOnStorage(ingredient.tag, ref amount))
			{
				return false;
			}
			if (amount < ingredient.amount)
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
		Recipe[] recipes = GetRecipes();
		if (recipes.Length > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.BUILDINGEFFECTS.FABRICATES, UI.BUILDINGEFFECTS.TOOLTIPS.FABRICATES, Descriptor.DescriptorType.Effect);
			list.Add(item);
		}
		Recipe[] recipes2 = GetRecipes();
		foreach (Recipe recipe in recipes2)
		{
			GameObject prefab = Assets.GetPrefab(recipe.Result);
			string keywordStyle = GameUtil.GetKeywordStyle(prefab);
			Descriptor item2 = default(Descriptor);
			item2.IncreaseIndent();
			item2.SetupDescriptor("• " + string.Format(UI.BUILDINGEFFECTS.FABRICATEDITEM, keywordStyle, recipe.Name), GameUtil.GetGameObjectEffectsTooltipString(prefab), Descriptor.DescriptorType.Effect);
			list.Add(item2);
		}
		return list;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		if (!isCancellingOrder)
		{
			base.OnCompleteWork(worker);
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
				buildStorage.Transfer(outStorage, true, true);
				UpdateOrderQueue(false);
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
		ShowProgressBar(userOrders.Count > 0);
	}

	protected virtual void OnBuildQueued(MachineOrder order)
	{
	}
}
