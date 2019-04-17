using Klei.AI;
using ProcGen;
using System;
using System.Collections.Generic;
using System.IO;
using TUNING;
using UnityEngine;

public class EconomyDetails
{
	public class Resource
	{
		public class Type
		{
			public string id
			{
				get;
				private set;
			}

			public string unit
			{
				get;
				private set;
			}

			public Type(string id, string unit)
			{
				this.id = id;
				this.unit = unit;
			}
		}

		public Tag tag
		{
			get;
			private set;
		}

		public Type type
		{
			get;
			private set;
		}

		public Resource(Tag tag, Type type)
		{
			this.tag = tag;
			this.type = type;
		}
	}

	public class BiomeTransformation
	{
		public Tag tag
		{
			get;
			private set;
		}

		public Resource resource
		{
			get;
			private set;
		}

		public float ratio
		{
			get;
			private set;
		}

		public BiomeTransformation(Tag tag, Resource resource, float ratio)
		{
			this.tag = tag;
			this.resource = resource;
			this.ratio = ratio;
		}

		public float Transform(Element element, float amount)
		{
			if (resource.tag == element.tag)
			{
				return ratio * amount;
			}
			return 0f;
		}
	}

	public class Ratio
	{
		public Resource input
		{
			get;
			private set;
		}

		public Resource output
		{
			get;
			private set;
		}

		public bool allowNegativeOutput
		{
			get;
			private set;
		}

		public Ratio(Resource input, Resource output, bool allow_negative_output)
		{
			this.input = input;
			this.output = output;
			allowNegativeOutput = allow_negative_output;
		}
	}

	public class Scenario
	{
		public class Entry
		{
			public Tag tag
			{
				get;
				private set;
			}

			public int count
			{
				get;
				private set;
			}

			public Entry(Tag tag, int count)
			{
				this.tag = tag;
				this.count = count;
			}
		}

		private Func<Transformation, bool> filter;

		private List<Entry> entries = new List<Entry>();

		public string name
		{
			get;
			private set;
		}

		public int defaultCount
		{
			get;
			private set;
		}

		public float timeInSeconds
		{
			get;
			set;
		}

		public Scenario(string name, int default_count, Func<Transformation, bool> filter)
		{
			this.name = name;
			defaultCount = default_count;
			this.filter = filter;
			timeInSeconds = 600f;
		}

		public void AddEntry(Entry entry)
		{
			entries.Add(entry);
		}

		public int GetCount(Tag tag)
		{
			foreach (Entry entry in entries)
			{
				if (entry.tag == tag)
				{
					return entry.count;
				}
			}
			return defaultCount;
		}

		public bool IncludesTransformation(Transformation transformation)
		{
			if (filter != null && filter(transformation))
			{
				return true;
			}
			foreach (Entry entry in entries)
			{
				if (entry.tag == transformation.tag)
				{
					return true;
				}
			}
			return false;
		}
	}

	public class Transformation
	{
		public class Delta
		{
			public Resource resource
			{
				get;
				private set;
			}

			public float amount
			{
				get;
				set;
			}

			public Delta(Resource resource, float amount)
			{
				this.resource = resource;
				this.amount = amount;
			}
		}

		public class Type
		{
			public string id
			{
				get;
				private set;
			}

			public Type(string id)
			{
				this.id = id;
			}
		}

		public List<Delta> deltas = new List<Delta>();

		public Tag tag
		{
			get;
			private set;
		}

		public Type type
		{
			get;
			private set;
		}

		public float timeInSeconds
		{
			get;
			private set;
		}

		public Transformation(Tag tag, Type type, float time_in_seconds)
		{
			this.tag = tag;
			this.type = type;
			timeInSeconds = time_in_seconds;
		}

		public void AddDelta(Delta delta)
		{
			deltas.Add(delta);
		}

		public Delta GetDelta(Resource resource)
		{
			foreach (Delta delta in deltas)
			{
				if (delta.resource == resource)
				{
					return delta;
				}
			}
			return null;
		}
	}

	private List<Transformation> transformations = new List<Transformation>();

	private List<Resource> resources = new List<Resource>();

	public Dictionary<Element, float> startingBiomeAmounts = new Dictionary<Element, float>();

	public int startingBiomeCellCount;

	public Resource energyResource;

	public Resource heatResource;

	public Resource duplicantTimeResource;

	public Resource caloriesResource;

	public Resource.Type massResourceType;

	public Resource.Type energyResourceType;

	public Resource.Type timeResourceType;

	public Resource.Type attributeResourceType;

	public Resource.Type caloriesResourceType;

	public Resource.Type amountResourceType;

	public Transformation.Type buildingTransformationType;

	public Transformation.Type foodTransformationType;

	public Transformation.Type plantTransformationType;

	public Transformation.Type creatureTransformationType;

	public Transformation.Type dupeTransformationType;

	public Transformation.Type referenceTransformationType;

	public Transformation.Type effectTransformationType;

	private const string GEYSER_ACTIVE_SUFFIX = "_ActiveOnly";

	public Transformation.Type geyserActivePeriodTransformationType;

	public Transformation.Type geyserLifetimeTransformationType;

	private static string debugTag = "CO2Scrubber";

	public EconomyDetails()
	{
		massResourceType = new Resource.Type("Mass", "kg");
		energyResourceType = new Resource.Type("Energy", "joules");
		timeResourceType = new Resource.Type("Time", "seconds");
		attributeResourceType = new Resource.Type("Attribute", "units");
		caloriesResourceType = new Resource.Type("Calories", "cal");
		amountResourceType = new Resource.Type("Amount", "units");
		buildingTransformationType = new Transformation.Type("Building");
		foodTransformationType = new Transformation.Type("Food");
		plantTransformationType = new Transformation.Type("Plant");
		creatureTransformationType = new Transformation.Type("Creature");
		dupeTransformationType = new Transformation.Type("Duplicant");
		referenceTransformationType = new Transformation.Type("Reference");
		effectTransformationType = new Transformation.Type("Effect");
		geyserActivePeriodTransformationType = new Transformation.Type("GeyserActivePeriod");
		geyserLifetimeTransformationType = new Transformation.Type("GeyserLifetime");
		energyResource = CreateResource(TagManager.Create("Energy"), energyResourceType);
		heatResource = CreateResource(TagManager.Create("Heat"), energyResourceType);
		duplicantTimeResource = CreateResource(TagManager.Create("DupeTime"), timeResourceType);
		caloriesResource = CreateResource(new Tag(Db.Get().Amounts.Calories.deltaAttribute.Id), amountResourceType);
		foreach (Element element in ElementLoader.elements)
		{
			CreateResource(element);
		}
		GatherStartingBiomeAmounts();
		foreach (KPrefabID prefab in Assets.Prefabs)
		{
			CreateTransformation(prefab, prefab.PrefabTag);
			if ((UnityEngine.Object)prefab.GetComponent<GeyserConfigurator>() != (UnityEngine.Object)null)
			{
				CreateTransformation(prefab, prefab.PrefabTag + "_ActiveOnly");
			}
		}
		foreach (Effect resource in Db.Get().effects.resources)
		{
			CreateTransformation(resource);
		}
		Transformation transformation = new Transformation(TagManager.Create("Duplicant"), dupeTransformationType, 1f);
		transformation.AddDelta(new Transformation.Delta(GetResource(GameTags.Oxygen), -0.1f));
		transformation.AddDelta(new Transformation.Delta(GetResource(GameTags.CarbonDioxide), 0.1f * Assets.GetPrefab(MinionConfig.ID).GetComponent<OxygenBreather>().O2toCO2conversion));
		transformation.AddDelta(new Transformation.Delta(duplicantTimeResource, 0.875f));
		transformation.AddDelta(new Transformation.Delta(caloriesResource, -1666.66663f));
		transformation.AddDelta(new Transformation.Delta(CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), amountResourceType), 0.166666672f));
		transformations.Add(transformation);
		Transformation transformation2 = new Transformation(TagManager.Create("Electrolysis"), referenceTransformationType, 1f);
		transformation2.AddDelta(new Transformation.Delta(GetResource(GameTags.Oxygen), 1.77777779f));
		transformation2.AddDelta(new Transformation.Delta(GetResource(GameTags.Hydrogen), 0.222222224f));
		transformation2.AddDelta(new Transformation.Delta(GetResource(GameTags.Water), -2f));
		transformations.Add(transformation2);
		Transformation transformation3 = new Transformation(TagManager.Create("MethaneCombustion"), referenceTransformationType, 1f);
		transformation3.AddDelta(new Transformation.Delta(GetResource(GameTags.Methane), -1f));
		transformation3.AddDelta(new Transformation.Delta(GetResource(GameTags.Oxygen), -4f));
		transformation3.AddDelta(new Transformation.Delta(GetResource(GameTags.CarbonDioxide), 2.75f));
		transformation3.AddDelta(new Transformation.Delta(GetResource(GameTags.Water), 2.25f));
		transformations.Add(transformation3);
		Transformation transformation4 = new Transformation(TagManager.Create("CoalCombustion"), referenceTransformationType, 1f);
		transformation4.AddDelta(new Transformation.Delta(GetResource(GameTags.Carbon), -1f));
		transformation4.AddDelta(new Transformation.Delta(GetResource(GameTags.Oxygen), -2.66666675f));
		transformation4.AddDelta(new Transformation.Delta(GetResource(GameTags.CarbonDioxide), 3.66666675f));
		transformations.Add(transformation4);
	}

	private static void WriteProduct(StreamWriter o, string a, string b, string c)
	{
		o.Write("\"=PRODUCT(" + a + ", " + b + ", " + c + ")\"");
	}

	public void DumpTransformations(Scenario scenario, StreamWriter o)
	{
		List<Resource> used_resources = new List<Resource>();
		foreach (Transformation transformation3 in transformations)
		{
			if (scenario.IncludesTransformation(transformation3))
			{
				foreach (Transformation.Delta delta3 in transformation3.deltas)
				{
					if (!used_resources.Contains(delta3.resource))
					{
						used_resources.Add(delta3.resource);
					}
				}
			}
		}
		used_resources.Sort((Resource x, Resource y) => x.tag.Name.CompareTo(y.tag.Name));
		List<Ratio> list = new List<Ratio>();
		list.Add(new Ratio(GetResource(GameTags.Algae), GetResource(GameTags.Oxygen), false));
		list.Add(new Ratio(energyResource, GetResource(GameTags.Oxygen), false));
		list.Add(new Ratio(GetResource(GameTags.Oxygen), energyResource, false));
		list.Add(new Ratio(GetResource(GameTags.Water), GetResource(GameTags.Oxygen), false));
		list.Add(new Ratio(GetResource(GameTags.DirtyWater), caloriesResource, false));
		list.Add(new Ratio(GetResource(GameTags.Water), caloriesResource, false));
		list.Add(new Ratio(GetResource(GameTags.Fertilizer), caloriesResource, false));
		list.Add(new Ratio(energyResource, CreateResource(new Tag(Db.Get().Amounts.Stress.deltaAttribute.Id), amountResourceType), true));
		list.RemoveAll((Ratio x) => !used_resources.Contains(x.input) || !used_resources.Contains(x.output));
		o.Write("Id");
		o.Write(",Count");
		o.Write(",Type");
		o.Write(",Time(s)");
		int num = 4;
		foreach (Resource item in used_resources)
		{
			o.Write(", " + item.tag.Name + "(" + item.type.unit + ")");
			num++;
		}
		o.Write(",MassDelta");
		foreach (Ratio item2 in list)
		{
			o.Write(", " + item2.output.tag.Name + "(" + item2.output.type.unit + ")/" + item2.input.tag.Name + "(" + item2.input.type.unit + ")");
			num++;
		}
		string str = "B";
		o.Write("\n");
		int num2 = 1;
		transformations.Sort((Transformation x, Transformation y) => x.tag.Name.CompareTo(y.tag.Name));
		for (int i = 0; i < transformations.Count; i++)
		{
			Transformation transformation = transformations[i];
			if (scenario.IncludesTransformation(transformation))
			{
				num2++;
			}
		}
		string text = "B" + (num2 + 4).ToString();
		int num3 = 1;
		for (int j = 0; j < transformations.Count; j++)
		{
			Transformation transformation2 = transformations[j];
			if (scenario.IncludesTransformation(transformation2))
			{
				if (transformation2.tag == new Tag(debugTag))
				{
					int num4 = 0;
					num4++;
				}
				num3++;
				o.Write("\"" + transformation2.tag.Name + "\"");
				o.Write("," + scenario.GetCount(transformation2.tag).ToString());
				o.Write(",\"" + transformation2.type.id + "\"");
				o.Write(",\"" + transformation2.timeInSeconds.ToString("0.00") + "\"");
				string a = str + num3.ToString();
				float num5 = 0f;
				bool flag = false;
				foreach (Resource item3 in used_resources)
				{
					float num6 = 0f;
					foreach (Transformation.Delta delta4 in transformation2.deltas)
					{
						if (delta4.resource.tag == item3.tag)
						{
							num6 += delta4.amount;
							if (delta4.resource.type == massResourceType)
							{
								flag = true;
								num5 += num6;
							}
						}
					}
					o.Write(",");
					if (num6 != 0f)
					{
						num6 /= transformation2.timeInSeconds;
						WriteProduct(o, a, num6.ToString("0.00000"), text);
					}
				}
				o.Write(",");
				if (flag)
				{
					num5 /= transformation2.timeInSeconds;
					WriteProduct(o, a, num5.ToString("0.00000"), text);
				}
				foreach (Ratio item4 in list)
				{
					o.Write(", ");
					Transformation.Delta delta = transformation2.GetDelta(item4.input);
					Transformation.Delta delta2 = transformation2.GetDelta(item4.output);
					if (delta2 != null && delta != null && delta.amount < 0f && (delta2.amount > 0f || item4.allowNegativeOutput))
					{
						o.Write(delta2.amount / Mathf.Abs(delta.amount));
					}
				}
				o.Write("\n");
			}
		}
		int num7 = 4;
		for (int k = 0; k < num; k++)
		{
			if (k >= num7 && k < num7 + used_resources.Count)
			{
				string text2 = ((ushort)(65 + k % 26)).ToString();
				int num8 = Mathf.FloorToInt((float)k / 26f);
				if (num8 > 0)
				{
					text2 = ((ushort)(65 + num8 - 1)).ToString() + text2;
				}
				o.Write("\"=SUM(" + text2 + "2: " + text2 + num2.ToString() + ")\"");
			}
			o.Write(",");
		}
		string str2 = "B" + (num2 + 5).ToString();
		o.Write("\n");
		o.Write("\nTiming:");
		o.Write("\nTimeInSeconds:," + scenario.timeInSeconds);
		o.Write("\nSecondsPerCycle:," + 600f.ToString());
		o.Write("\nCycles:,=" + text + "/" + str2);
	}

	public Resource CreateResource(Tag tag, Resource.Type resource_type)
	{
		foreach (Resource resource2 in resources)
		{
			if (resource2.tag == tag)
			{
				return resource2;
			}
		}
		Resource resource = new Resource(tag, resource_type);
		resources.Add(resource);
		return resource;
	}

	public Resource CreateResource(Element element)
	{
		return CreateResource(element.tag, massResourceType);
	}

	public Transformation CreateTransformation(Effect effect)
	{
		Transformation transformation = new Transformation(new Tag(effect.Id), effectTransformationType, 1f);
		foreach (AttributeModifier selfModifier in effect.SelfModifiers)
		{
			Resource resource = CreateResource(new Tag(selfModifier.AttributeId), attributeResourceType);
			transformation.AddDelta(new Transformation.Delta(resource, selfModifier.Value));
		}
		transformations.Add(transformation);
		return transformation;
	}

	public Transformation GetTransformation(Tag tag)
	{
		foreach (Transformation transformation in transformations)
		{
			if (transformation.tag == tag)
			{
				return transformation;
			}
		}
		return null;
	}

	public Transformation CreateTransformation(KPrefabID prefab_id, Tag tag)
	{
		if (tag == new Tag(debugTag))
		{
			int num = 0;
			num++;
		}
		ElementConverter component = prefab_id.GetComponent<ElementConverter>();
		EnergyConsumer component2 = prefab_id.GetComponent<EnergyConsumer>();
		ElementConsumer component3 = prefab_id.GetComponent<ElementConsumer>();
		BuildingElementEmitter component4 = prefab_id.GetComponent<BuildingElementEmitter>();
		Generator component5 = prefab_id.GetComponent<Generator>();
		EnergyGenerator component6 = prefab_id.GetComponent<EnergyGenerator>();
		ManualGenerator component7 = prefab_id.GetComponent<ManualGenerator>();
		ManualDeliveryKG[] components = prefab_id.GetComponents<ManualDeliveryKG>();
		StateMachineController component8 = prefab_id.GetComponent<StateMachineController>();
		Edible component9 = prefab_id.GetComponent<Edible>();
		Crop component10 = prefab_id.GetComponent<Crop>();
		Uprootable component11 = prefab_id.GetComponent<Uprootable>();
		Recipe recipe = RecipeManager.Get().recipes.Find((Recipe r) => r.Result == prefab_id.PrefabTag);
		List<FertilizationMonitor.Def> list = null;
		List<IrrigationMonitor.Def> list2 = null;
		GeyserConfigurator component12 = prefab_id.GetComponent<GeyserConfigurator>();
		Toilet component13 = prefab_id.GetComponent<Toilet>();
		FlushToilet component14 = prefab_id.GetComponent<FlushToilet>();
		RelaxationPoint component15 = prefab_id.GetComponent<RelaxationPoint>();
		CreatureCalorieMonitor.Def def = prefab_id.gameObject.GetDef<CreatureCalorieMonitor.Def>();
		if ((UnityEngine.Object)component8 != (UnityEngine.Object)null)
		{
			list = component8.GetDefs<FertilizationMonitor.Def>();
			list2 = component8.GetDefs<IrrigationMonitor.Def>();
		}
		Transformation transformation = null;
		float time_in_seconds = 1f;
		if ((UnityEngine.Object)component9 != (UnityEngine.Object)null)
		{
			transformation = new Transformation(tag, foodTransformationType, time_in_seconds);
		}
		else if ((UnityEngine.Object)component != (UnityEngine.Object)null || (UnityEngine.Object)component2 != (UnityEngine.Object)null || (UnityEngine.Object)component3 != (UnityEngine.Object)null || (UnityEngine.Object)component4 != (UnityEngine.Object)null || (UnityEngine.Object)component5 != (UnityEngine.Object)null || (UnityEngine.Object)component6 != (UnityEngine.Object)null || (UnityEngine.Object)component11 != (UnityEngine.Object)null || (UnityEngine.Object)component12 != (UnityEngine.Object)null || (UnityEngine.Object)component13 != (UnityEngine.Object)null || (UnityEngine.Object)component14 != (UnityEngine.Object)null || (UnityEngine.Object)component15 != (UnityEngine.Object)null || def != null)
		{
			if ((UnityEngine.Object)component11 != (UnityEngine.Object)null || (UnityEngine.Object)component10 != (UnityEngine.Object)null)
			{
				if ((UnityEngine.Object)component10 != (UnityEngine.Object)null)
				{
					time_in_seconds = component10.cropVal.cropDuration;
				}
				transformation = new Transformation(tag, plantTransformationType, time_in_seconds);
			}
			else if (def != null)
			{
				transformation = new Transformation(tag, creatureTransformationType, time_in_seconds);
			}
			else if ((UnityEngine.Object)component12 != (UnityEngine.Object)null)
			{
				GeyserConfigurator.GeyserInstanceConfiguration geyserInstanceConfiguration = new GeyserConfigurator.GeyserInstanceConfiguration();
				geyserInstanceConfiguration.typeId = component12.presetType;
				geyserInstanceConfiguration.rateRoll = 0.5f;
				geyserInstanceConfiguration.iterationLengthRoll = 0.5f;
				geyserInstanceConfiguration.iterationPercentRoll = 0.5f;
				geyserInstanceConfiguration.yearLengthRoll = 0.5f;
				geyserInstanceConfiguration.yearPercentRoll = 0.5f;
				GeyserConfigurator.GeyserInstanceConfiguration geyserInstanceConfiguration2 = geyserInstanceConfiguration;
				if (tag.Name.Contains("_ActiveOnly"))
				{
					float iterationLength = geyserInstanceConfiguration2.GetIterationLength();
					transformation = new Transformation(tag, geyserActivePeriodTransformationType, iterationLength);
				}
				else
				{
					float yearLength = geyserInstanceConfiguration2.GetYearLength();
					transformation = new Transformation(tag, geyserLifetimeTransformationType, yearLength);
				}
			}
			else
			{
				if ((UnityEngine.Object)component13 != (UnityEngine.Object)null || (UnityEngine.Object)component14 != (UnityEngine.Object)null)
				{
					time_in_seconds = 600f;
				}
				transformation = new Transformation(tag, buildingTransformationType, time_in_seconds);
			}
		}
		if (transformation != null)
		{
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.consumedElements != null)
			{
				ElementConverter.ConsumedElement[] consumedElements = component.consumedElements;
				for (int i = 0; i < consumedElements.Length; i++)
				{
					ElementConverter.ConsumedElement consumedElement = consumedElements[i];
					Resource resource = CreateResource(consumedElement.tag, massResourceType);
					transformation.AddDelta(new Transformation.Delta(resource, 0f - consumedElement.massConsumptionRate));
				}
				if (component.outputElements != null)
				{
					ElementConverter.OutputElement[] outputElements = component.outputElements;
					for (int j = 0; j < outputElements.Length; j++)
					{
						ElementConverter.OutputElement outputElement = outputElements[j];
						Element element = ElementLoader.FindElementByHash(outputElement.elementHash);
						Resource resource2 = CreateResource(element.tag, massResourceType);
						transformation.AddDelta(new Transformation.Delta(resource2, outputElement.massGenerationRate));
					}
				}
			}
			if ((UnityEngine.Object)component3 != (UnityEngine.Object)null && (UnityEngine.Object)component6 == (UnityEngine.Object)null && ((UnityEngine.Object)component == (UnityEngine.Object)null || (UnityEngine.Object)prefab_id.GetComponent<AlgaeHabitat>() != (UnityEngine.Object)null))
			{
				Resource resource3 = GetResource(ElementLoader.FindElementByHash(component3.elementToConsume).tag);
				transformation.AddDelta(new Transformation.Delta(resource3, 0f - component3.consumptionRate));
			}
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				transformation.AddDelta(new Transformation.Delta(energyResource, 0f - component2.WattsNeededWhenActive));
			}
			if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
			{
				transformation.AddDelta(new Transformation.Delta(GetResource(component4.element), component4.emitRate));
			}
			if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
			{
				transformation.AddDelta(new Transformation.Delta(energyResource, component5.GetComponent<Building>().Def.GeneratorWattageRating));
			}
			if ((UnityEngine.Object)component6 != (UnityEngine.Object)null)
			{
				if (component6.formula.inputs != null)
				{
					EnergyGenerator.InputItem[] inputs = component6.formula.inputs;
					for (int k = 0; k < inputs.Length; k++)
					{
						EnergyGenerator.InputItem inputItem = inputs[k];
						transformation.AddDelta(new Transformation.Delta(GetResource(inputItem.tag), 0f - inputItem.consumptionRate));
					}
				}
				if (component6.formula.outputs != null)
				{
					EnergyGenerator.OutputItem[] outputs = component6.formula.outputs;
					for (int l = 0; l < outputs.Length; l++)
					{
						EnergyGenerator.OutputItem outputItem = outputs[l];
						transformation.AddDelta(new Transformation.Delta(GetResource(outputItem.element), outputItem.creationRate));
					}
				}
			}
			if (GameComps.StructureTemperatures.Has(prefab_id.gameObject))
			{
				BuildingDef def2 = prefab_id.GetComponent<BuildingComplete>().Def;
				transformation.AddDelta(new Transformation.Delta(heatResource, def2.SelfHeatKilowattsWhenActive + def2.ExhaustKilowattsWhenActive));
			}
			if ((bool)component7)
			{
				transformation.AddDelta(new Transformation.Delta(duplicantTimeResource, -1f));
			}
			if ((bool)component9)
			{
				EdiblesManager.FoodInfo foodInfo = component9.FoodInfo;
				transformation.AddDelta(new Transformation.Delta(caloriesResource, foodInfo.CaloriesPerUnit));
			}
			if ((UnityEngine.Object)component10 != (UnityEngine.Object)null)
			{
				Resource resource4 = CreateResource(TagManager.Create(component10.cropVal.cropId), amountResourceType);
				float num2 = (float)component10.cropVal.numProduced;
				transformation.AddDelta(new Transformation.Delta(resource4, num2));
				GameObject prefab = Assets.GetPrefab(new Tag(component10.cropVal.cropId));
				if ((UnityEngine.Object)prefab != (UnityEngine.Object)null)
				{
					Edible component16 = prefab.GetComponent<Edible>();
					if ((UnityEngine.Object)component16 != (UnityEngine.Object)null)
					{
						transformation.AddDelta(new Transformation.Delta(caloriesResource, component16.FoodInfo.CaloriesPerUnit * num2));
					}
				}
			}
			if (recipe != null)
			{
				Resource resource5;
				foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
				{
					resource5 = CreateResource(ingredient.tag, amountResourceType);
					transformation.AddDelta(new Transformation.Delta(resource5, 0f - ingredient.amount));
				}
				resource5 = CreateResource(recipe.Result, amountResourceType);
				transformation.AddDelta(new Transformation.Delta(resource5, recipe.OutputUnits));
			}
			if (components != null)
			{
				for (int m = 0; m < components.Length; m++)
				{
					transformation.AddDelta(new Transformation.Delta(duplicantTimeResource, -0.1f * transformation.timeInSeconds));
				}
			}
			if (list != null && list.Count > 0)
			{
				foreach (FertilizationMonitor.Def item in list)
				{
					PlantElementAbsorber.ConsumeInfo[] consumedElements2 = item.consumedElements;
					for (int n = 0; n < consumedElements2.Length; n++)
					{
						PlantElementAbsorber.ConsumeInfo consumeInfo = consumedElements2[n];
						Resource resource6 = CreateResource(consumeInfo.tag, massResourceType);
						transformation.AddDelta(new Transformation.Delta(resource6, (0f - consumeInfo.massConsumptionRate) * transformation.timeInSeconds));
					}
				}
			}
			if (list2 != null && list2.Count > 0)
			{
				foreach (IrrigationMonitor.Def item2 in list2)
				{
					PlantElementAbsorber.ConsumeInfo[] consumedElements3 = item2.consumedElements;
					for (int num3 = 0; num3 < consumedElements3.Length; num3++)
					{
						PlantElementAbsorber.ConsumeInfo consumeInfo2 = consumedElements3[num3];
						Resource resource7 = CreateResource(consumeInfo2.tag, massResourceType);
						transformation.AddDelta(new Transformation.Delta(resource7, (0f - consumeInfo2.massConsumptionRate) * transformation.timeInSeconds));
					}
				}
			}
			if ((UnityEngine.Object)component12 != (UnityEngine.Object)null)
			{
				GeyserConfigurator.GeyserInstanceConfiguration geyserInstanceConfiguration = new GeyserConfigurator.GeyserInstanceConfiguration();
				geyserInstanceConfiguration.typeId = component12.presetType;
				geyserInstanceConfiguration.rateRoll = 0.5f;
				geyserInstanceConfiguration.iterationLengthRoll = 0.5f;
				geyserInstanceConfiguration.iterationPercentRoll = 0.5f;
				geyserInstanceConfiguration.yearLengthRoll = 0.5f;
				geyserInstanceConfiguration.yearPercentRoll = 0.5f;
				GeyserConfigurator.GeyserInstanceConfiguration geyserInstanceConfiguration3 = geyserInstanceConfiguration;
				if (tag.Name.Contains("_ActiveOnly"))
				{
					float amount = geyserInstanceConfiguration3.GetMassPerCycle() / 600f * geyserInstanceConfiguration3.GetIterationLength();
					transformation.AddDelta(new Transformation.Delta(CreateResource(geyserInstanceConfiguration3.GetElement().CreateTag(), massResourceType), amount));
				}
				else
				{
					float amount2 = geyserInstanceConfiguration3.GetMassPerCycle() / 600f * geyserInstanceConfiguration3.GetYearLength() * geyserInstanceConfiguration3.GetYearPercent();
					transformation.AddDelta(new Transformation.Delta(CreateResource(geyserInstanceConfiguration3.GetElement().CreateTag(), massResourceType), amount2));
				}
			}
			if ((UnityEngine.Object)component13 != (UnityEngine.Object)null)
			{
				transformation.AddDelta(new Transformation.Delta(CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), amountResourceType), -0.166666672f));
				transformation.AddDelta(new Transformation.Delta(GetResource(SimHashes.Dirt), 0f - component13.solidWastePerUse.mass));
				transformation.AddDelta(new Transformation.Delta(GetResource(component13.solidWastePerUse.elementID), component13.solidWastePerUse.mass));
			}
			if ((UnityEngine.Object)component14 != (UnityEngine.Object)null)
			{
				transformation.AddDelta(new Transformation.Delta(CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), amountResourceType), -0.166666672f));
				transformation.AddDelta(new Transformation.Delta(GetResource(SimHashes.Water), 0f - component14.massConsumedPerUse));
				transformation.AddDelta(new Transformation.Delta(GetResource(SimHashes.DirtyWater), component14.massEmittedPerUse));
			}
			if ((UnityEngine.Object)component15 != (UnityEngine.Object)null)
			{
				Effect effect = component15.CreateEffect();
				foreach (AttributeModifier selfModifier in effect.SelfModifiers)
				{
					Resource resource8 = CreateResource(new Tag(selfModifier.AttributeId), attributeResourceType);
					transformation.AddDelta(new Transformation.Delta(resource8, selfModifier.Value));
				}
			}
			if (def != null)
			{
				CollectDietTransformations(prefab_id);
			}
			transformations.Add(transformation);
		}
		return transformation;
	}

	private void CollectDietTransformations(KPrefabID prefab_id)
	{
		Trait trait = Db.Get().traits.Get(prefab_id.GetComponent<Modifiers>().initialTraits[0]);
		CreatureCalorieMonitor.Def def = prefab_id.gameObject.GetDef<CreatureCalorieMonitor.Def>();
		WildnessMonitor.Def def2 = prefab_id.gameObject.GetDef<WildnessMonitor.Def>();
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.AddRange(trait.SelfModifiers);
		list.AddRange(def2.tameEffect.SelfModifiers);
		float num = 0f;
		float num2 = 0f;
		foreach (AttributeModifier item in list)
		{
			if (item.AttributeId == Db.Get().Amounts.Calories.maxAttribute.Id)
			{
				num = item.Value;
			}
			if (item.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
			{
				num2 = item.Value;
			}
		}
		Diet.Info[] infos = def.diet.infos;
		foreach (Diet.Info info in infos)
		{
			foreach (Tag consumedTag in info.consumedTags)
			{
				float time_in_seconds = Mathf.Abs(num / num2);
				float num3 = num / info.caloriesPerKg;
				float amount = num3 * info.producedConversionRate;
				Transformation transformation = new Transformation(new Tag(prefab_id.PrefabTag.Name + "Diet" + consumedTag.Name), creatureTransformationType, time_in_seconds);
				transformation.AddDelta(new Transformation.Delta(CreateResource(consumedTag, massResourceType), 0f - num3));
				transformation.AddDelta(new Transformation.Delta(CreateResource(new Tag(info.producedElement.ToString()), massResourceType), amount));
				transformation.AddDelta(new Transformation.Delta(caloriesResource, num));
				transformations.Add(transformation);
			}
		}
	}

	private static void CollectDietScenarios(List<Scenario> scenarios)
	{
		Scenario scenario = new Scenario("diets/all", 0, null);
		foreach (KPrefabID prefab in Assets.Prefabs)
		{
			CreatureCalorieMonitor.Def def = prefab.gameObject.GetDef<CreatureCalorieMonitor.Def>();
			if (def != null)
			{
				Scenario scenario2 = new Scenario("diets/" + prefab.name, 0, null);
				Diet.Info[] infos = def.diet.infos;
				foreach (Diet.Info info in infos)
				{
					foreach (Tag consumedTag in info.consumedTags)
					{
						Tag tag = prefab.PrefabTag.Name + "Diet" + consumedTag.Name;
						scenario2.AddEntry(new Scenario.Entry(tag, 1));
						scenario.AddEntry(new Scenario.Entry(tag, 1));
					}
				}
				scenarios.Add(scenario2);
			}
		}
		scenarios.Add(scenario);
	}

	public void GatherStartingBiomeAmounts()
	{
		for (int i = 0; i < Grid.CellCount; i++)
		{
			if (World.Instance.zoneRenderData.worldZoneTypes[i] == SubWorld.ZoneType.Sandstone)
			{
				Element key = Grid.Element[i];
				float value = 0f;
				startingBiomeAmounts.TryGetValue(key, out value);
				startingBiomeAmounts[key] = value + Grid.Mass[i];
				startingBiomeCellCount++;
			}
		}
	}

	public Resource GetResource(SimHashes element)
	{
		return GetResource(ElementLoader.FindElementByHash(element).tag);
	}

	public Resource GetResource(Tag tag)
	{
		foreach (Resource resource in resources)
		{
			if (resource.tag == tag)
			{
				return resource;
			}
		}
		return null;
	}

	private float GetDupeBreathingPerSecond(EconomyDetails details)
	{
		Transformation transformation = details.GetTransformation(TagManager.Create("Duplicant"));
		return transformation.GetDelta(details.GetResource(GameTags.Oxygen)).amount;
	}

	private BiomeTransformation CreateBiomeTransformationFromTransformation(EconomyDetails details, Tag transformation_tag, Tag input_resource_tag, Tag output_resource_tag)
	{
		Resource resource = details.GetResource(input_resource_tag);
		Resource resource2 = details.GetResource(output_resource_tag);
		Transformation transformation = details.GetTransformation(transformation_tag);
		float num = transformation.GetDelta(resource2).amount / (0f - transformation.GetDelta(resource).amount);
		float num2 = GetDupeBreathingPerSecond(details) * 600f;
		return new BiomeTransformation((transformation_tag.Name + input_resource_tag.Name + "Cycles").ToTag(), resource, num / (0f - num2));
	}

	private static void DumpEconomyDetails()
	{
		Debug.Log("Starting Economy Details Dump...");
		EconomyDetails details = new EconomyDetails();
		List<Scenario> list = new List<Scenario>();
		Scenario item = new Scenario("default", 1, (Transformation t) => true);
		list.Add(item);
		Scenario item2 = new Scenario("all_buildings", 1, (Transformation t) => t.type == details.buildingTransformationType);
		list.Add(item2);
		Scenario item3 = new Scenario("all_plants", 1, (Transformation t) => t.type == details.plantTransformationType);
		list.Add(item3);
		Scenario item4 = new Scenario("all_creatures", 1, (Transformation t) => t.type == details.creatureTransformationType);
		list.Add(item4);
		Scenario item5 = new Scenario("all_stress", 1, (Transformation t) => t.GetDelta(details.GetResource(new Tag(Db.Get().Amounts.Stress.deltaAttribute.Id))) != null);
		list.Add(item5);
		Scenario item6 = new Scenario("all_foods", 1, (Transformation t) => t.type == details.foodTransformationType);
		list.Add(item6);
		Scenario item7 = new Scenario("geysers/geysers_active_period_only", 1, (Transformation t) => t.type == details.geyserActivePeriodTransformationType);
		list.Add(item7);
		Scenario item8 = new Scenario("geyser/geysers_whole_lifetime", 1, (Transformation t) => t.type == details.geyserLifetimeTransformationType);
		list.Add(item8);
		Scenario scenario = new Scenario("oxygen/algae_distillery", 0, null);
		scenario.AddEntry(new Scenario.Entry(TagManager.Create("AlgaeDistillery"), 3));
		scenario.AddEntry(new Scenario.Entry(TagManager.Create("AlgaeHabitat"), 22));
		scenario.AddEntry(new Scenario.Entry(TagManager.Create("Duplicant"), 9));
		scenario.AddEntry(new Scenario.Entry(TagManager.Create("WaterPurifier"), 1));
		list.Add(scenario);
		Scenario scenario2 = new Scenario("oxygen/algae_habitat_electrolyzer", 0, null);
		scenario2.AddEntry(new Scenario.Entry("AlgaeHabitat", 1));
		scenario2.AddEntry(new Scenario.Entry("Duplicant", 1));
		scenario2.AddEntry(new Scenario.Entry("Electrolyzer", 1));
		list.Add(scenario2);
		Scenario scenario3 = new Scenario("oxygen/electrolyzer", 0, null);
		scenario3.AddEntry(new Scenario.Entry(TagManager.Create("Electrolyzer"), 1));
		scenario3.AddEntry(new Scenario.Entry(TagManager.Create("LiquidPump"), 1));
		scenario3.AddEntry(new Scenario.Entry(TagManager.Create("Duplicant"), 9));
		scenario3.AddEntry(new Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1));
		scenario3.AddEntry(new Scenario.Entry(TagManager.Create("GasPump"), 1));
		list.Add(scenario3);
		Scenario scenario4 = new Scenario("purifiers/methane_generator", 0, null);
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("MethaneGenerator"), 1));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("FertilizerMaker"), 3));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("Electrolyzer"), 1));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("GasPump"), 1));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("LiquidPump"), 2));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("PrickleFlower"), 0));
		list.Add(scenario4);
		Scenario scenario5 = new Scenario("purifiers/water_purifier", 0, null);
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("WaterPurifier"), 1));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("Compost"), 2));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("Electrolyzer"), 1));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("LiquidPump"), 2));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("GasPump"), 1));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("PrickleFlower"), 29));
		list.Add(scenario5);
		Scenario scenario6 = new Scenario("energy/petroleum_generator", 0, null);
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("PetroleumGenerator"), 1));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("OilRefinery"), 1));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("WaterPurifier"), 1));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("LiquidPump"), 1));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("GasPump"), 1));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("CO2Scrubber"), 1));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("MethaneGenerator"), 1));
		list.Add(scenario6);
		Scenario scenario7 = new Scenario("energy/coal_generator", 0, (Transformation t) => t.tag.Name.Contains("Hatch"));
		scenario7.AddEntry(new Scenario.Entry("Generator", 1));
		list.Add(scenario7);
		Scenario scenario8 = new Scenario("waste/outhouse", 0, null);
		scenario8.AddEntry(new Scenario.Entry(TagManager.Create("Outhouse"), 1));
		scenario8.AddEntry(new Scenario.Entry(TagManager.Create("Compost"), 1));
		list.Add(scenario8);
		Scenario scenario9 = new Scenario("stress/massage_table", 0, null);
		scenario9.AddEntry(new Scenario.Entry(TagManager.Create("MassageTable"), 1));
		scenario9.AddEntry(new Scenario.Entry(TagManager.Create("ManualGenerator"), 1));
		list.Add(scenario9);
		Scenario scenario10 = new Scenario("waste/flush_toilet", 0, null);
		scenario10.AddEntry(new Scenario.Entry(TagManager.Create("FlushToilet"), 1));
		scenario10.AddEntry(new Scenario.Entry(TagManager.Create("WaterPurifier"), 1));
		scenario10.AddEntry(new Scenario.Entry(TagManager.Create("LiquidPump"), 1));
		scenario10.AddEntry(new Scenario.Entry(TagManager.Create("FertilizerMaker"), 1));
		list.Add(scenario10);
		CollectDietScenarios(list);
		foreach (Transformation transformation in details.transformations)
		{
			Transformation transformation_iter = transformation;
			Scenario item9 = new Scenario("transformations/" + transformation.tag.Name, 1, (Transformation t) => transformation_iter == t);
			list.Add(item9);
		}
		foreach (Transformation transformation2 in details.transformations)
		{
			Scenario scenario11 = new Scenario("transformation_groups/" + transformation2.tag.Name, 0, null);
			scenario11.AddEntry(new Scenario.Entry(transformation2.tag, 1));
			foreach (Transformation transformation3 in details.transformations)
			{
				bool flag = false;
				foreach (Transformation.Delta delta in transformation2.deltas)
				{
					if (delta.resource.type != details.energyResourceType)
					{
						foreach (Transformation.Delta delta2 in transformation3.deltas)
						{
							if (delta.resource == delta2.resource)
							{
								scenario11.AddEntry(new Scenario.Entry(transformation3.tag, 0));
								flag = true;
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
			list.Add(scenario11);
		}
		foreach (EdiblesManager.FoodInfo item10 in FOOD.FOOD_TYPES_LIST)
		{
			Scenario scenario12 = new Scenario("food/" + item10.Id, 0, null);
			Tag tag2 = TagManager.Create(item10.Id);
			scenario12.AddEntry(new Scenario.Entry(tag2, 1));
			scenario12.AddEntry(new Scenario.Entry(TagManager.Create("Duplicant"), 1));
			List<Tag> list2 = new List<Tag>();
			list2.Add(tag2);
			while (list2.Count > 0)
			{
				Tag tag = list2[0];
				list2.RemoveAt(0);
				Recipe recipe = RecipeManager.Get().recipes.Find((Recipe a) => a.Result == tag);
				if (recipe != null)
				{
					foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
					{
						scenario12.AddEntry(new Scenario.Entry(ingredient.tag, 1));
						list2.Add(ingredient.tag);
					}
				}
				foreach (KPrefabID prefab in Assets.Prefabs)
				{
					Crop component = prefab.GetComponent<Crop>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.cropVal.cropId == tag.Name)
					{
						scenario12.AddEntry(new Scenario.Entry(prefab.PrefabTag, 1));
						list2.Add(prefab.PrefabTag);
					}
				}
			}
			list.Add(scenario12);
		}
		if (!Directory.Exists("assets/Tuning/Economy"))
		{
			Directory.CreateDirectory("assets/Tuning/Economy");
		}
		foreach (Scenario item11 in list)
		{
			string path = "assets/Tuning/Economy/" + item11.name + ".csv";
			if (!Directory.Exists(System.IO.Path.GetDirectoryName(path)))
			{
				Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
			}
			using (StreamWriter o = new StreamWriter(path))
			{
				details.DumpTransformations(item11, o);
			}
		}
		float dupeBreathingPerSecond = details.GetDupeBreathingPerSecond(details);
		List<BiomeTransformation> list3 = new List<BiomeTransformation>();
		list3.Add(details.CreateBiomeTransformationFromTransformation(details, "MineralDeoxidizer".ToTag(), GameTags.Algae, GameTags.Oxygen));
		list3.Add(details.CreateBiomeTransformationFromTransformation(details, "AlgaeHabitat".ToTag(), GameTags.Algae, GameTags.Oxygen));
		list3.Add(details.CreateBiomeTransformationFromTransformation(details, "AlgaeHabitat".ToTag(), GameTags.Water, GameTags.Oxygen));
		list3.Add(details.CreateBiomeTransformationFromTransformation(details, "Electrolyzer".ToTag(), GameTags.Water, GameTags.Oxygen));
		list3.Add(new BiomeTransformation("StartingOxygenCycles".ToTag(), details.GetResource(GameTags.Oxygen), 1f / (0f - dupeBreathingPerSecond * 600f)));
		list3.Add(new BiomeTransformation("StartingOxyliteCycles".ToTag(), details.CreateResource(GameTags.OxyRock, details.massResourceType), 1f / (0f - dupeBreathingPerSecond * 600f)));
		string path2 = "assets/Tuning/Economy/biomes/starting_amounts.csv";
		if (!Directory.Exists(System.IO.Path.GetDirectoryName(path2)))
		{
			Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path2));
		}
		using (StreamWriter streamWriter = new StreamWriter(path2))
		{
			streamWriter.Write("Resource,Amount");
			foreach (BiomeTransformation item12 in list3)
			{
				streamWriter.Write("," + item12.tag);
			}
			streamWriter.Write("\n");
			streamWriter.Write("Cells, " + details.startingBiomeCellCount + "\n");
			foreach (KeyValuePair<Element, float> startingBiomeAmount in details.startingBiomeAmounts)
			{
				streamWriter.Write(startingBiomeAmount.Key.id.ToString() + ", " + startingBiomeAmount.Value.ToString());
				foreach (BiomeTransformation item13 in list3)
				{
					streamWriter.Write(",");
					float num = item13.Transform(startingBiomeAmount.Key, startingBiomeAmount.Value);
					if (num > 0f)
					{
						streamWriter.Write(num);
					}
				}
				streamWriter.Write("\n");
			}
		}
		Debug.Log("Completed economy details dump!!");
	}
}
