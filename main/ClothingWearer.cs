using Klei.AI;
using KSerialization;
using STRINGS;

public class ClothingWearer : KMonoBehaviour
{
	public class ClothingInfo
	{
		[Serialize]
		public string name = string.Empty;

		[Serialize]
		public int decorMod;

		[Serialize]
		public float conductivityMod;

		[Serialize]
		public float homeostasisEfficiencyMultiplier;

		public static readonly ClothingInfo UGLY_CLOTHING = new ClothingInfo(EQUIPMENT.PREFABS.COOL_VEST.GENERICNAME, -30, 0.0025f, -1.25f);

		public static readonly ClothingInfo BASIC_CLOTHING = new ClothingInfo(EQUIPMENT.PREFABS.COOL_VEST.GENERICNAME, -5, 0.0025f, -1.25f);

		public static readonly ClothingInfo WARM_CLOTHING = new ClothingInfo(EQUIPMENT.PREFABS.WARM_VEST.NAME, -10, 0.01f, -1.25f);

		public static readonly ClothingInfo COOL_CLOTHING = new ClothingInfo(EQUIPMENT.PREFABS.COOL_VEST.NAME, -10, 0.0005f, 0f);

		public static readonly ClothingInfo FANCY_CLOTHING = new ClothingInfo(EQUIPMENT.PREFABS.FUNKY_VEST.NAME, 30, 0.0025f, -1.25f);

		public ClothingInfo(string _name, int _decor, float _temperature, float _homeostasisEfficiencyMultiplier)
		{
			name = _name;
			decorMod = _decor;
			conductivityMod = _temperature;
			homeostasisEfficiencyMultiplier = _homeostasisEfficiencyMultiplier;
		}
	}

	private DecorProvider decorProvider;

	private AttributeModifier decorModifier;

	private AttributeModifier conductivityModifier;

	[Serialize]
	public ClothingInfo currentClothing;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		decorProvider = GetComponent<DecorProvider>();
		if (decorModifier == null)
		{
			decorModifier = new AttributeModifier("Decor", 0f, DUPLICANTS.MODIFIERS.CLOTHING.NAME, false, false, false);
		}
		if (conductivityModifier == null)
		{
			AttributeInstance attributeInstance = base.gameObject.GetAttributes().Get("ThermalConductivityBarrier");
			conductivityModifier = new AttributeModifier("ThermalConductivityBarrier", ClothingInfo.BASIC_CLOTHING.conductivityMod, DUPLICANTS.MODIFIERS.CLOTHING.NAME, false, false, false);
			attributeInstance.Add(conductivityModifier);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		decorProvider.decor.Add(decorModifier);
		decorProvider.decorRadius.Add(new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, 3f, null, false, false, true));
		decorProvider.overrideName = string.Format(UI.OVERLAYS.DECOR.CLOTHING, base.gameObject.GetProperName());
		if (currentClothing == null)
		{
			ChangeToDefaultClothes();
		}
		else
		{
			ChangeClothes(currentClothing);
		}
	}

	public void ChangeClothes(ClothingInfo clothingInfo)
	{
		decorProvider.baseRadius = 3f;
		currentClothing = clothingInfo;
		conductivityModifier.Description = clothingInfo.name;
		conductivityModifier.SetValue(currentClothing.conductivityMod);
		decorModifier.SetValue((float)currentClothing.decorMod);
	}

	public void ChangeToDefaultClothes()
	{
		ChangeClothes(new ClothingInfo(ClothingInfo.BASIC_CLOTHING.name, ClothingInfo.BASIC_CLOTHING.decorMod, ClothingInfo.BASIC_CLOTHING.conductivityMod, ClothingInfo.BASIC_CLOTHING.homeostasisEfficiencyMultiplier));
	}
}
