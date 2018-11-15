using Klei;
using Klei.AI;
using TUNING;

public class EspressoMachineWorkable : Workable, IGameObjectEffectDescriptor, IWorkerPrioritizable
{
	[MyCmpReq]
	private Operational operational;

	public int basePriority = RELAXATION.PRIORITY.TIER5;

	private static string specificEffect = "Espresso";

	private static string trackingEffect = "RecentlyEspresso";

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_espresso_machine_kanim")
		};
		showProgressBar = true;
		resetProgressOnStop = true;
		synchronizeAnims = false;
		SetWorkTime(30f);
	}

	protected override void OnStartWork(Worker worker)
	{
		operational.SetActive(true, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage component = GetComponent<Storage>();
		component.ConsumeAndGetDisease(GameTags.Water, EspressoMachine.WATER_MASS_PER_USE, out SimUtil.DiseaseInfo disease_info, out float aggregate_temperature);
		component.ConsumeAndGetDisease(EspressoMachine.INGREDIENT_TAG, EspressoMachine.INGREDIENT_MASS_PER_USE, out SimUtil.DiseaseInfo disease_info2, out aggregate_temperature);
		ImmuneSystemMonitor.Instance sMI = worker.GetSMI<ImmuneSystemMonitor.Instance>();
		if (sMI != null)
		{
			sMI.TryInjectDisease(disease_info.idx, disease_info.count, GameTags.Water, Disease.InfectionVector.Digestion);
			sMI.TryInjectDisease(disease_info2.idx, disease_info2.count, EspressoMachine.INGREDIENT_TAG, Disease.InfectionVector.Digestion);
		}
		Effects component2 = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(specificEffect))
		{
			component2.Add(specificEffect, true);
		}
		if (!string.IsNullOrEmpty(trackingEffect))
		{
			component2.Add(trackingEffect, true);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		operational.SetActive(false, false);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(trackingEffect) && component.HasEffect(trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(specificEffect) && component.HasEffect(specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
