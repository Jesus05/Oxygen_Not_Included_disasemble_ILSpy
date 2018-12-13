using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class MedicinalPill : Workable, IGameObjectEffectDescriptor, IConsumableUIItem
{
	public MedicineInfo info;

	public string ConsumableId => this.PrefabID().Name;

	public string ConsumableName => this.GetProperName();

	public int MajorOrder => (int)(info.medicineType + 1000);

	public int MinorOrder => 0;

	public bool Display => true;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SetWorkTime(10f);
		showProgressBar = false;
		synchronizeAnims = false;
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, null);
		CreateChore();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		EffectInstance effectInstance = component.Get(info.effect);
		if (effectInstance != null)
		{
			effectInstance.timeRemaining = effectInstance.effect.duration;
		}
		else
		{
			component.Add(info.effect, true);
		}
		base.gameObject.DeleteObject();
	}

	private void CreateChore()
	{
		new TakeMedicineChore(this);
	}

	public bool CanBeTakenBy(GameObject consumer)
	{
		Effects component = consumer.GetComponent<Effects>();
		if ((Object)component == (Object)null || component.HasEffect(info.effect))
		{
			return false;
		}
		if (info.medicineType == MedicineInfo.MedicineType.Booster)
		{
			AmountInstance amountInstance = Db.Get().Amounts.ImmuneLevel.Lookup(consumer);
			return amountInstance != null && amountInstance.value < amountInstance.GetMax();
		}
		Diseases diseases = consumer.GetDiseases();
		if (info.medicineType == MedicineInfo.MedicineType.CureAny && diseases.Count > 0)
		{
			return true;
		}
		foreach (DiseaseInstance item in diseases)
		{
			if (info.curedDiseases.Contains(item.modifier.Id))
			{
				return true;
			}
		}
		return false;
	}

	public List<Descriptor> EffectDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		switch (info.medicineType)
		{
		case MedicineInfo.MedicineType.Booster:
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.BOOSTER), string.Format(DUPLICANTS.DISEASES.MEDICINE.BOOSTER_TOOLTIP), Descriptor.DescriptorType.Effect, false));
			break;
		case MedicineInfo.MedicineType.CureAny:
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_ANY), string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_ANY_TOOLTIP), Descriptor.DescriptorType.Effect, false));
			break;
		case MedicineInfo.MedicineType.CureSpecific:
		{
			List<string> list2 = new List<string>();
			foreach (string curedDisease in info.curedDiseases)
			{
				list2.Add(Strings.Get("STRINGS.DUPLICANTS.DISEASES." + curedDisease.ToUpper() + ".NAME"));
			}
			string arg = string.Join(",", list2.ToArray());
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES, arg), string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_TOOLTIP, arg), Descriptor.DescriptorType.Effect, false));
			break;
		}
		}
		Effect effect = Db.Get().effects.Get(info.effect);
		list.Add(new Descriptor(string.Format(DUPLICANTS.MODIFIERS.MEDICINE_GENERICPILL.EFFECT_DESC, effect.Name), string.Format("{0}\n{1}", effect.description, Effect.CreateTooltip(effect, true, "\n")), Descriptor.DescriptorType.Effect, false));
		return list;
	}

	public new List<Descriptor> GetDescriptors(GameObject go)
	{
		return EffectDescriptors(go);
	}
}
