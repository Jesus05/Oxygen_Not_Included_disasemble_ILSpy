using System.Collections.Generic;

public class AutoDisinfectableManager : KMonoBehaviour, ISim1000ms
{
	private List<AutoDisinfectable> autoDisinfectables = new List<AutoDisinfectable>();

	public static AutoDisinfectableManager Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	public void AddAutoDisinfectable(AutoDisinfectable auto_disinfectable)
	{
		autoDisinfectables.Add(auto_disinfectable);
	}

	public void RemoveAutoDisinfectable(AutoDisinfectable auto_disinfectable)
	{
		auto_disinfectable.CancelChore();
		autoDisinfectables.Remove(auto_disinfectable);
	}

	public void Sim1000ms(float dt)
	{
		for (int i = 0; i < autoDisinfectables.Count; i++)
		{
			autoDisinfectables[i].RefreshChore();
		}
	}
}
