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

			public float count
			{
				get;
				private set;
			}

			public Entry(Tag tag, float count)
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

		public float defaultCount
		{
			get;
			private set;
		}

		public float timeInSeconds
		{
			get;
			set;
		}

		public Scenario(string name, float default_count, Func<Transformation, bool> filter)
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

		public float GetCount(Tag tag)
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

		public bool timeInvariant
		{
			get;
			private set;
		}

		public Transformation(Tag tag, Type type, float time_in_seconds, bool timeInvariant = false)
		{
			this.tag = tag;
			this.type = type;
			timeInSeconds = time_in_seconds;
			this.timeInvariant = timeInvariant;
		}

		public void AddDelta(Delta delta)
		{
			Debug.Assert(delta.resource != null);
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

	public Resource fixedCaloriesResource;

	public Resource.Type massResourceType;

	public Resource.Type heatResourceType;

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
		heatResourceType = new Resource.Type("Heat Energy", "kdtu");
		energyResourceType = new Resource.Type("Energy", "joules");
		timeResourceType = new Resource.Type("Time", "seconds");
		attributeResourceType = new Resource.Type("Attribute", "units");
		caloriesResourceType = new Resource.Type("Calories", "kcal");
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
		heatResource = CreateResource(TagManager.Create("Heat"), heatResourceType);
		duplicantTimeResource = CreateResource(TagManager.Create("DupeTime"), timeResourceType);
		caloriesResource = CreateResource(new Tag(Db.Get().Amounts.Calories.deltaAttribute.Id), caloriesResourceType);
		fixedCaloriesResource = CreateResource(new Tag(Db.Get().Amounts.Calories.Id), caloriesResourceType);
		foreach (Element element in ElementLoader.elements)
		{
			CreateResource(element);
		}
		List<Tag> list = new List<Tag>
		{
			GameTags.CombustibleLiquid,
			GameTags.CombustibleGas,
			GameTags.CombustibleSolid
		};
		foreach (Tag item in list)
		{
			CreateResource(item, massResourceType);
		}
		foreach (EdiblesManager.FoodInfo item2 in FOOD.FOOD_TYPES_LIST)
		{
			CreateResource(item2.Id.ToTag(), amountResourceType);
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
		Transformation transformation = new Transformation(TagManager.Create("Duplicant"), dupeTransformationType, 1f, false);
		transformation.AddDelta(new Transformation.Delta(GetResource(GameTags.Oxygen), -0.1f));
		transformation.AddDelta(new Transformation.Delta(GetResource(GameTags.CarbonDioxide), 0.1f * Assets.GetPrefab(MinionConfig.ID).GetComponent<OxygenBreather>().O2toCO2conversion));
		transformation.AddDelta(new Transformation.Delta(duplicantTimeResource, 0.875f));
		transformation.AddDelta(new Transformation.Delta(caloriesResource, -1.66666675f));
		transformation.AddDelta(new Transformation.Delta(CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), amountResourceType), 0.166666672f));
		transformations.Add(transformation);
		Transformation transformation2 = new Transformation(TagManager.Create("Electrolysis"), referenceTransformationType, 1f, false);
		transformation2.AddDelta(new Transformation.Delta(GetResource(GameTags.Oxygen), 1.77777779f));
		transformation2.AddDelta(new Transformation.Delta(GetResource(GameTags.Hydrogen), 0.222222224f));
		transformation2.AddDelta(new Transformation.Delta(GetResource(GameTags.Water), -2f));
		transformations.Add(transformation2);
		Transformation transformation3 = new Transformation(TagManager.Create("MethaneCombustion"), referenceTransformationType, 1f, false);
		transformation3.AddDelta(new Transformation.Delta(GetResource(GameTags.Methane), -1f));
		transformation3.AddDelta(new Transformation.Delta(GetResource(GameTags.Oxygen), -4f));
		transformation3.AddDelta(new Transformation.Delta(GetResource(GameTags.CarbonDioxide), 2.75f));
		transformation3.AddDelta(new Transformation.Delta(GetResource(GameTags.Water), 2.25f));
		transformations.Add(transformation3);
		Transformation transformation4 = new Transformation(TagManager.Create("CoalCombustion"), referenceTransformationType, 1f, false);
		transformation4.AddDelta(new Transformation.Delta(GetResource(GameTags.Carbon), -1f));
		transformation4.AddDelta(new Transformation.Delta(GetResource(GameTags.Oxygen), -2.66666675f));
		transformation4.AddDelta(new Transformation.Delta(GetResource(GameTags.CarbonDioxide), 3.66666675f));
		transformations.Add(transformation4);
	}

	private static void WriteProduct(StreamWriter o, string a, string b)
	{
		o.Write("\"=PRODUCT(" + a + ", " + b + ")\"");
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
				foreach (Transformation.Delta delta4 in transformation3.deltas)
				{
					if (!used_resources.Contains(delta4.resource))
					{
						used_resources.Add(delta4.resource);
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
				if (!transformation2.timeInvariant)
				{
					o.Write(",\"" + transformation2.timeInSeconds.ToString("0.00") + "\"");
				}
				else
				{
					o.Write(",\"invariant\"");
				}
				string a = str + num3.ToString();
				float num5 = 0f;
				bool flag = false;
				foreach (Resource item3 in used_resources)
				{
					Transformation.Delta delta = null;
					foreach (Transformation.Delta delta5 in transformation2.deltas)
					{
						if (delta5.resource.tag == item3.tag)
						{
							delta = delta5;
							break;
						}
					}
					o.Write(",");
					if (delta != null && delta.amount != 0f)
					{
						if (delta.resource.type == massResourceType)
						{
							flag = true;
							num5 += delta.amount;
						}
						if (!transformation2.timeInvariant)
						{
							WriteProduct(o, a, (delta.amount / transformation2.timeInSeconds).ToString("0.00000"), text);
						}
						else
						{
							WriteProduct(o, a, delta.amount.ToString("0.00000"));
						}
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
					Transformation.Delta delta2 = transformation2.GetDelta(item4.input);
					Transformation.Delta delta3 = transformation2.GetDelta(item4.output);
					if (delta3 != null && delta2 != null && delta2.amount < 0f && (delta3.amount > 0f || item4.allowNegativeOutput))
					{
						o.Write(delta3.amount / Mathf.Abs(delta2.amount));
					}
				}
				o.Write("\n");
			}
		}
		int num6 = 4;
		for (int k = 0; k < num; k++)
		{
			if (k >= num6 && k < num6 + used_resources.Count)
			{
				string text2 = ((ushort)(65 + k % 26)).ToString();
				int num7 = Mathf.FloorToInt((float)k / 26f);
				if (num7 > 0)
				{
					text2 = ((ushort)(65 + num7 - 1)).ToString() + text2;
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
		Transformation transformation = new Transformation(new Tag(effect.Id), effectTransformationType, 1f, false);
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
		Building component = prefab_id.GetComponent<Building>();
		ElementConverter component2 = prefab_id.GetComponent<ElementConverter>();
		EnergyConsumer component3 = prefab_id.GetComponent<EnergyConsumer>();
		ElementConsumer component4 = prefab_id.GetComponent<ElementConsumer>();
		BuildingElementEmitter component5 = prefab_id.GetComponent<BuildingElementEmitter>();
		Generator component6 = prefab_id.GetComponent<Generator>();
		EnergyGenerator component7 = prefab_id.GetComponent<EnergyGenerator>();
		ManualGenerator component8 = prefab_id.GetComponent<ManualGenerator>();
		ManualDeliveryKG[] components = prefab_id.GetComponents<ManualDeliveryKG>();
		StateMachineController component9 = prefab_id.GetComponent<StateMachineController>();
		Edible component10 = prefab_id.GetComponent<Edible>();
		Crop component11 = prefab_id.GetComponent<Crop>();
		Uprootable component12 = prefab_id.GetComponent<Uprootable>();
		ComplexRecipe complexRecipe = ComplexRecipeManager.Get().recipes.Find((ComplexRecipe r) => r.FirstResult == prefab_id.PrefabTag);
		List<FertilizationMonitor.Def> list = null;
		List<IrrigationMonitor.Def> list2 = null;
		GeyserConfigurator component13 = prefab_id.GetComponent<GeyserConfigurator>();
		Toilet component14 = prefab_id.GetComponent<Toilet>();
		FlushToilet component15 = prefab_id.GetComponent<FlushToilet>();
		RelaxationPoint component16 = prefab_id.GetComponent<RelaxationPoint>();
		CreatureCalorieMonitor.Def def = prefab_id.gameObject.GetDef<CreatureCalorieMonitor.Def>();
		if ((UnityEngine.Object)component9 != (UnityEngine.Object)null)
		{
			list = component9.GetDefs<FertilizationMonitor.Def>();
			list2 = component9.GetDefs<IrrigationMonitor.Def>();
		}
		Transformation transformation = null;
		float time_in_seconds = 1f;
		if ((UnityEngine.Object)component10 != (UnityEngine.Object)null)
		{
			transformation = new Transformation(tag, foodTransformationType, time_in_seconds, complexRecipe != null);
		}
		else if ((UnityEngine.Object)component2 != (UnityEngine.Object)null || (UnityEngine.Object)component3 != (UnityEngine.Object)null || (UnityEngine.Object)component4 != (UnityEngine.Object)null || (UnityEngine.Object)component5 != (UnityEngine.Object)null || (UnityEngine.Object)component6 != (UnityEngine.Object)null || (UnityEngine.Object)component7 != (UnityEngine.Object)null || (UnityEngine.Object)component12 != (UnityEngine.Object)null || (UnityEngine.Object)component13 != (UnityEngine.Object)null || (UnityEngine.Object)component14 != (UnityEngine.Object)null || (UnityEngine.Object)component15 != (UnityEngine.Object)null || (UnityEngine.Object)component16 != (UnityEngine.Object)null || def != null)
		{
			if ((UnityEngine.Object)component12 != (UnityEngine.Object)null || (UnityEngine.Object)component11 != (UnityEngine.Object)null)
			{
				if ((UnityEngine.Object)component11 != (UnityEngine.Object)null)
				{
					time_in_seconds = component11.cropVal.cropDuration;
				}
				transformation = new Transformation(tag, plantTransformationType, time_in_seconds, false);
			}
			else if (def != null)
			{
				transformation = new Transformation(tag, creatureTransformationType, time_in_seconds, false);
			}
			else if ((UnityEngine.Object)component13 != (UnityEngine.Object)null)
			{
				GeyserConfigurator.GeyserInstanceConfiguration geyserInstanceConfiguration = new GeyserConfigurator.GeyserInstanceConfiguration();
				geyserInstanceConfiguration.typeId = component13.presetType;
				geyserInstanceConfiguration.rateRoll = 0.5f;
				geyserInstanceConfiguration.iterationLengthRoll = 0.5f;
				geyserInstanceConfiguration.iterationPercentRoll = 0.5f;
				geyserInstanceConfiguration.yearLengthRoll = 0.5f;
				geyserInstanceConfiguration.yearPercentRoll = 0.5f;
				GeyserConfigurator.GeyserInstanceConfiguration geyserInstanceConfiguration2 = geyserInstanceConfiguration;
				if (tag.Name.Contains("_ActiveOnly"))
				{
					float iterationLength = geyserInstanceConfiguration2.GetIterationLength();
					transformation = new Transformation(tag, geyserActivePeriodTransformationType, iterationLength, false);
				}
				else
				{
					float yearLength = geyserInstanceConfiguration2.GetYearLength();
					transformation = new Transformation(tag, geyserLifetimeTransformationType, yearLength, false);
				}
			}
			else
			{
				if ((UnityEngine.Object)component14 != (UnityEngine.Object)null || (UnityEngine.Object)component15 != (UnityEngine.Object)null)
				{
					time_in_seconds = 600f;
				}
				transformation = new Transformation(tag, buildingTransformationType, time_in_seconds, false);
			}
		}
		if (transformation != null)
		{
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.consumedElements != null)
			{
				ElementConverter.ConsumedElement[] consumedElements = component2.consumedElements;
				for (int i = 0; i < consumedElements.Length; i++)
				{
					ElementConverter.ConsumedElement consumedElement = consumedElements[i];
					Resource resource = CreateResource(consumedElement.tag, massResourceType);
					transformation.AddDelta(new Transformation.Delta(resource, 0f - consumedElement.massConsumptionRate));
				}
				if (component2.outputElements != null)
				{
					ElementConverter.OutputElement[] outputElements = component2.outputElements;
					for (int j = 0; j < outputElements.Length; j++)
					{
						ElementConverter.OutputElement outputElement = outputElements[j];
						Element element = ElementLoader.FindElementByHash(outputElement.elementHash);
						Resource resource2 = CreateResource(element.tag, massResourceType);
						transformation.AddDelta(new Transformation.Delta(resource2, outputElement.massGenerationRate));
					}
				}
			}
			if ((UnityEngine.Object)component4 != (UnityEngine.Object)null && (UnityEngine.Object)component7 == (UnityEngine.Object)null && ((UnityEngine.Object)component2 == (UnityEngine.Object)null || (UnityEngine.Object)prefab_id.GetComponent<AlgaeHabitat>() != (UnityEngine.Object)null))
			{
				Resource resource3 = GetResource(ElementLoader.FindElementByHash(component4.elementToConsume).tag);
				transformation.AddDelta(new Transformation.Delta(resource3, 0f - component4.consumptionRate));
			}
			if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
			{
				transformation.AddDelta(new Transformation.Delta(energyResource, 0f - component3.WattsNeededWhenActive));
			}
			if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
			{
				transformation.AddDelta(new Transformation.Delta(GetResource(component5.element), component5.emitRate));
			}
			if ((UnityEngine.Object)component6 != (UnityEngine.Object)null)
			{
				transformation.AddDelta(new Transformation.Delta(energyResource, component6.GetComponent<Building>().Def.GeneratorWattageRating));
			}
			if ((UnityEngine.Object)component7 != (UnityEngine.Object)null)
			{
				if (component7.formula.inputs != null)
				{
					EnergyGenerator.InputItem[] inputs = component7.formula.inputs;
					for (int k = 0; k < inputs.Length; k++)
					{
						EnergyGenerator.InputItem inputItem = inputs[k];
						transformation.AddDelta(new Transformation.Delta(GetResource(inputItem.tag), 0f - inputItem.consumptionRate));
					}
				}
				if (component7.formula.outputs != null)
				{
					EnergyGenerator.OutputItem[] outputs = component7.formula.outputs;
					for (int l = 0; l < outputs.Length; l++)
					{
						EnergyGenerator.OutputItem outputItem = outputs[l];
						transformation.AddDelta(new Transformation.Delta(GetResource(outputItem.element), outputItem.creationRate));
					}
				}
			}
			if ((bool)component)
			{
				BuildingDef def2 = component.Def;
				transformation.AddDelta(new Transformation.Delta(heatResource, def2.SelfHeatKilowattsWhenActive + def2.ExhaustKilowattsWhenActive));
			}
			if ((bool)component8)
			{
				transformation.AddDelta(new Transformation.Delta(duplicantTimeResource, -1f));
			}
			if ((bool)component10)
			{
				EdiblesManager.FoodInfo foodInfo = component10.FoodInfo;
				transformation.AddDelta(new Transformation.Delta(fixedCaloriesResource, foodInfo.CaloriesPerUnit * 0.001f));
				ComplexRecipe complexRecipe2 = ComplexRecipeManager.Get().recipes.Find((ComplexRecipe a) => a.FirstResult == tag);
				if (complexRecipe2 == null)
				{
					goto IL_0708;
				}
			}
			goto IL_0708;
		}
		goto IL_0cd2;
		IL_0708:
		if ((UnityEngine.Object)component11 != (UnityEngine.Object)null)
		{
			Resource resource4 = CreateResource(TagManager.Create(component11.cropVal.cropId), amountResourceType);
			float num2 = (float)component11.cropVal.numProduced;
			transformation.AddDelta(new Transformation.Delta(resource4, num2));
			GameObject prefab = Assets.GetPrefab(new Tag(component11.cropVal.cropId));
			if ((UnityEngine.Object)prefab != (UnityEngine.Object)null)
			{
				Edible component17 = prefab.GetComponent<Edible>();
				if ((UnityEngine.Object)component17 != (UnityEngine.Object)null)
				{
					transformation.AddDelta(new Transformation.Delta(caloriesResource, component17.FoodInfo.CaloriesPerUnit * num2 * 0.001f));
				}
			}
		}
		if (complexRecipe != null)
		{
			ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				CreateResource(recipeElement.material, amountResourceType);
				transformation.AddDelta(new Transformation.Delta(GetResource(recipeElement.material), 0f - recipeElement.amount));
			}
			ComplexRecipe.RecipeElement[] results = complexRecipe.results;
			foreach (ComplexRecipe.RecipeElement recipeElement2 in results)
			{
				CreateResource(recipeElement2.material, amountResourceType);
				transformation.AddDelta(new Transformation.Delta(GetResource(recipeElement2.material), recipeElement2.amount));
			}
		}
		if (components != null)
		{
			for (int num3 = 0; num3 < components.Length; num3++)
			{
				transformation.AddDelta(new Transformation.Delta(duplicantTimeResource, -0.1f * transformation.timeInSeconds));
			}
		}
		if (list != null && list.Count > 0)
		{
			foreach (FertilizationMonitor.Def item in list)
			{
				PlantElementAbsorber.ConsumeInfo[] consumedElements2 = item.consumedElements;
				for (int num4 = 0; num4 < consumedElements2.Length; num4++)
				{
					PlantElementAbsorber.ConsumeInfo consumeInfo = consumedElements2[num4];
					Resource resource5 = CreateResource(consumeInfo.tag, massResourceType);
					transformation.AddDelta(new Transformation.Delta(resource5, (0f - consumeInfo.massConsumptionRate) * transformation.timeInSeconds));
				}
			}
		}
		if (list2 != null && list2.Count > 0)
		{
			foreach (IrrigationMonitor.Def item2 in list2)
			{
				PlantElementAbsorber.ConsumeInfo[] consumedElements3 = item2.consumedElements;
				for (int num5 = 0; num5 < consumedElements3.Length; num5++)
				{
					PlantElementAbsorber.ConsumeInfo consumeInfo2 = consumedElements3[num5];
					Resource resource6 = CreateResource(consumeInfo2.tag, massResourceType);
					transformation.AddDelta(new Transformation.Delta(resource6, (0f - consumeInfo2.massConsumptionRate) * transformation.timeInSeconds));
				}
			}
		}
		if ((UnityEngine.Object)component13 != (UnityEngine.Object)null)
		{
			GeyserConfigurator.GeyserInstanceConfiguration geyserInstanceConfiguration = new GeyserConfigurator.GeyserInstanceConfiguration();
			geyserInstanceConfiguration.typeId = component13.presetType;
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
		if ((UnityEngine.Object)component14 != (UnityEngine.Object)null)
		{
			transformation.AddDelta(new Transformation.Delta(CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), amountResourceType), -0.166666672f));
			transformation.AddDelta(new Transformation.Delta(GetResource(SimHashes.Dirt), 0f - component14.solidWastePerUse.mass));
			transformation.AddDelta(new Transformation.Delta(GetResource(component14.solidWastePerUse.elementID), component14.solidWastePerUse.mass));
		}
		if ((UnityEngine.Object)component15 != (UnityEngine.Object)null)
		{
			transformation.AddDelta(new Transformation.Delta(CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), amountResourceType), -0.166666672f));
			transformation.AddDelta(new Transformation.Delta(GetResource(SimHashes.Water), 0f - component15.massConsumedPerUse));
			transformation.AddDelta(new Transformation.Delta(GetResource(SimHashes.DirtyWater), component15.massEmittedPerUse));
		}
		if ((UnityEngine.Object)component16 != (UnityEngine.Object)null)
		{
			Effect effect = component16.CreateEffect();
			foreach (AttributeModifier selfModifier in effect.SelfModifiers)
			{
				Resource resource7 = CreateResource(new Tag(selfModifier.AttributeId), attributeResourceType);
				transformation.AddDelta(new Transformation.Delta(resource7, selfModifier.Value));
			}
		}
		if (def != null)
		{
			CollectDietTransformations(prefab_id);
		}
		transformations.Add(transformation);
		goto IL_0cd2;
		IL_0cd2:
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
				Transformation transformation = new Transformation(new Tag(prefab_id.PrefabTag.Name + "Diet" + consumedTag.Name), creatureTransformationType, time_in_seconds, false);
				transformation.AddDelta(new Transformation.Delta(CreateResource(consumedTag, massResourceType), 0f - num3));
				transformation.AddDelta(new Transformation.Delta(CreateResource(new Tag(info.producedElement.ToString()), massResourceType), amount));
				transformation.AddDelta(new Transformation.Delta(caloriesResource, num));
				transformations.Add(transformation);
			}
		}
	}

	private static void CollectDietScenarios(List<Scenario> scenarios)
	{
		Scenario scenario = new Scenario("diets/all", 0f, null);
		foreach (KPrefabID prefab in Assets.Prefabs)
		{
			CreatureCalorieMonitor.Def def = prefab.gameObject.GetDef<CreatureCalorieMonitor.Def>();
			if (def != null)
			{
				Scenario scenario2 = new Scenario("diets/" + prefab.name, 0f, null);
				Diet.Info[] infos = def.diet.infos;
				foreach (Diet.Info info in infos)
				{
					foreach (Tag consumedTag in info.consumedTags)
					{
						Tag tag = prefab.PrefabTag.Name + "Diet" + consumedTag.Name;
						scenario2.AddEntry(new Scenario.Entry(tag, 1f));
						scenario.AddEntry(new Scenario.Entry(tag, 1f));
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
		DebugUtil.LogErrorArgs("Found a tag without a matching resource!", tag);
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
		Scenario item = new Scenario("default", 1f, (Transformation t) => true);
		list.Add(item);
		Scenario item2 = new Scenario("all_buildings", 1f, (Transformation t) => t.type == details.buildingTransformationType);
		list.Add(item2);
		Scenario item3 = new Scenario("all_plants", 1f, (Transformation t) => t.type == details.plantTransformationType);
		list.Add(item3);
		Scenario item4 = new Scenario("all_creatures", 1f, (Transformation t) => t.type == details.creatureTransformationType);
		list.Add(item4);
		Scenario item5 = new Scenario("all_stress", 1f, (Transformation t) => t.GetDelta(details.GetResource(new Tag(Db.Get().Amounts.Stress.deltaAttribute.Id))) != null);
		list.Add(item5);
		Scenario item6 = new Scenario("all_foods", 1f, (Transformation t) => t.type == details.foodTransformationType);
		list.Add(item6);
		Scenario item7 = new Scenario("geysers/geysers_active_period_only", 1f, (Transformation t) => t.type == details.geyserActivePeriodTransformationType);
		list.Add(item7);
		Scenario item8 = new Scenario("geysers/geysers_whole_lifetime", 1f, (Transformation t) => t.type == details.geyserLifetimeTransformationType);
		list.Add(item8);
		Scenario scenario = new Scenario("oxygen/algae_distillery", 0f, null);
		scenario.AddEntry(new Scenario.Entry(TagManager.Create("AlgaeDistillery"), 3f));
		scenario.AddEntry(new Scenario.Entry(TagManager.Create("AlgaeHabitat"), 22f));
		scenario.AddEntry(new Scenario.Entry(TagManager.Create("Duplicant"), 9f));
		scenario.AddEntry(new Scenario.Entry(TagManager.Create("WaterPurifier"), 1f));
		list.Add(scenario);
		Scenario scenario2 = new Scenario("oxygen/algae_habitat_electrolyzer", 0f, null);
		scenario2.AddEntry(new Scenario.Entry("AlgaeHabitat", 1f));
		scenario2.AddEntry(new Scenario.Entry("Duplicant", 1f));
		scenario2.AddEntry(new Scenario.Entry("Electrolyzer", 1f));
		list.Add(scenario2);
		Scenario scenario3 = new Scenario("oxygen/electrolyzer", 0f, null);
		scenario3.AddEntry(new Scenario.Entry(TagManager.Create("Electrolyzer"), 1f));
		scenario3.AddEntry(new Scenario.Entry(TagManager.Create("LiquidPump"), 1f));
		scenario3.AddEntry(new Scenario.Entry(TagManager.Create("Duplicant"), 9f));
		scenario3.AddEntry(new Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1f));
		scenario3.AddEntry(new Scenario.Entry(TagManager.Create("GasPump"), 1f));
		list.Add(scenario3);
		Scenario scenario4 = new Scenario("purifiers/methane_generator", 0f, null);
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("MethaneGenerator"), 1f));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("FertilizerMaker"), 3f));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("Electrolyzer"), 1f));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("GasPump"), 1f));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("LiquidPump"), 2f));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1f));
		scenario4.AddEntry(new Scenario.Entry(TagManager.Create("PrickleFlower"), 0f));
		list.Add(scenario4);
		Scenario scenario5 = new Scenario("purifiers/water_purifier", 0f, null);
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("WaterPurifier"), 1f));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("Compost"), 2f));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("Electrolyzer"), 1f));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("LiquidPump"), 2f));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("GasPump"), 1f));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1f));
		scenario5.AddEntry(new Scenario.Entry(TagManager.Create("PrickleFlower"), 29f));
		list.Add(scenario5);
		Scenario scenario6 = new Scenario("energy/petroleum_generator", 0f, null);
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("PetroleumGenerator"), 1f));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("OilRefinery"), 1f));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("WaterPurifier"), 1f));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("LiquidPump"), 1f));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("GasPump"), 1f));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("CO2Scrubber"), 1f));
		scenario6.AddEntry(new Scenario.Entry(TagManager.Create("MethaneGenerator"), 1f));
		list.Add(scenario6);
		Scenario scenario7 = new Scenario("energy/coal_generator", 0f, (Transformation t) => t.tag.Name.Contains("Hatch"));
		scenario7.AddEntry(new Scenario.Entry("Generator", 1f));
		list.Add(scenario7);
		Scenario scenario8 = new Scenario("waste/outhouse", 0f, null);
		scenario8.AddEntry(new Scenario.Entry(TagManager.Create("Outhouse"), 1f));
		scenario8.AddEntry(new Scenario.Entry(TagManager.Create("Compost"), 1f));
		list.Add(scenario8);
		Scenario scenario9 = new Scenario("stress/massage_table", 0f, null);
		scenario9.AddEntry(new Scenario.Entry(TagManager.Create("MassageTable"), 1f));
		scenario9.AddEntry(new Scenario.Entry(TagManager.Create("ManualGenerator"), 1f));
		list.Add(scenario9);
		Scenario scenario10 = new Scenario("waste/flush_toilet", 0f, null);
		scenario10.AddEntry(new Scenario.Entry(TagManager.Create("FlushToilet"), 1f));
		scenario10.AddEntry(new Scenario.Entry(TagManager.Create("WaterPurifier"), 1f));
		scenario10.AddEntry(new Scenario.Entry(TagManager.Create("LiquidPump"), 1f));
		scenario10.AddEntry(new Scenario.Entry(TagManager.Create("FertilizerMaker"), 1f));
		list.Add(scenario10);
		CollectDietScenarios(list);
		foreach (Transformation transformation in details.transformations)
		{
			Transformation transformation_iter = transformation;
			Scenario item9 = new Scenario("transformations/" + transformation.tag.Name, 1f, (Transformation t) => transformation_iter == t);
			list.Add(item9);
		}
		foreach (Transformation transformation2 in details.transformations)
		{
			Scenario scenario11 = new Scenario("transformation_groups/" + transformation2.tag.Name, 0f, null);
			scenario11.AddEntry(new Scenario.Entry(transformation2.tag, 1f));
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
								scenario11.AddEntry(new Scenario.Entry(transformation3.tag, 0f));
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
			Scenario scenario12 = new Scenario("food/" + item10.Id, 0f, null);
			Tag tag2 = TagManager.Create(item10.Id);
			scenario12.AddEntry(new Scenario.Entry(tag2, 1f));
			scenario12.AddEntry(new Scenario.Entry(TagManager.Create("Duplicant"), 1f));
			List<Tag> list2 = new List<Tag>();
			list2.Add(tag2);
			while (list2.Count > 0)
			{
				Tag tag = list2[0];
				list2.RemoveAt(0);
				ComplexRecipe complexRecipe = ComplexRecipeManager.Get().recipes.Find((ComplexRecipe a) => a.FirstResult == tag);
				if (complexRecipe != null)
				{
					ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
					foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
					{
						scenario12.AddEntry(new Scenario.Entry(recipeElement.material, 1f));
						list2.Add(recipeElement.material);
					}
				}
				foreach (KPrefabID prefab in Assets.Prefabs)
				{
					Crop component = prefab.GetComponent<Crop>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.cropVal.cropId == tag.Name)
					{
						scenario12.AddEntry(new Scenario.Entry(prefab.PrefabTag, 1f));
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
