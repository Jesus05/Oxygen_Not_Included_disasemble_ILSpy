using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Spacecraft
{
	public enum MissionState
	{
		Grounded,
		Launching,
		Underway,
		WaitingToLand,
		Landing,
		Destroyed
	}

	[Serialize]
	public int id = -1;

	[Serialize]
	public string rocketName = UI.STARMAP.DEFAULT_NAME;

	[Serialize]
	public int moduleCount = 0;

	[Serialize]
	public Ref<LaunchConditionManager> refLaunchConditions = new Ref<LaunchConditionManager>();

	[Serialize]
	public MissionState state;

	[Serialize]
	private float missionElapsed = 0f;

	[Serialize]
	private float missionDuration = 0f;

	public LaunchConditionManager launchConditions
	{
		get
		{
			return refLaunchConditions.Get();
		}
		set
		{
			refLaunchConditions.Set(value);
		}
	}

	public Spacecraft(LaunchConditionManager launchConditions)
	{
		this.launchConditions = launchConditions;
	}

	public void SetRocketName(string newName)
	{
		rocketName = newName;
		UpdateNameOnRocketModules();
	}

	public string GetRocketName()
	{
		return rocketName;
	}

	public void UpdateNameOnRocketModules()
	{
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(launchConditions.GetComponent<AttachableBuilding>()))
		{
			RocketModule component = item.GetComponent<RocketModule>();
			if ((Object)component != (Object)null)
			{
				component.SetParentRocketName(rocketName);
			}
		}
	}

	public bool HasInvalidID()
	{
		return id == -1;
	}

	public void SetID(int id)
	{
		this.id = id;
	}

	public void SetState(MissionState state)
	{
		this.state = state;
	}

	public void BeginMission(SpaceDestination destination)
	{
		missionElapsed = 0f;
		missionDuration = (float)destination.OneBasedDistance * ROCKETRY.MISSION_DURATION_SCALE;
		SetState(MissionState.Launching);
	}

	public void ForceComplete()
	{
		missionElapsed = missionDuration;
	}

	public void ProgressMission(float deltaTime)
	{
		if (state == MissionState.Underway)
		{
			missionElapsed += deltaTime;
			if (missionElapsed > missionDuration)
			{
				CompleteMission();
			}
		}
	}

	public float GetTimeLeft()
	{
		return missionDuration - missionElapsed;
	}

	public float GetDuration()
	{
		return missionDuration;
	}

	private void CompleteMission()
	{
		SpacecraftManager.instance.PushReadyToLandNotification(this);
		SetState(MissionState.WaitingToLand);
		Land();
	}

	private void Land()
	{
		launchConditions.Trigger(1366341636, SpacecraftManager.instance.GetSpacecraftDestination(id));
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(launchConditions.GetComponent<AttachableBuilding>()))
		{
			if ((Object)item != (Object)launchConditions.gameObject)
			{
				item.Trigger(1366341636, SpacecraftManager.instance.GetSpacecraftDestination(id));
			}
		}
	}
}
