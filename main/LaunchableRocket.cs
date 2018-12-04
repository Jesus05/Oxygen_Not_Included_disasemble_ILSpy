using FMOD.Studio;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LaunchableRocket : StateMachineComponent<LaunchableRocket.StatesInstance>, IEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, LaunchableRocket, object>.GameInstance
	{
		public StatesInstance(LaunchableRocket master)
			: base(master)
		{
		}

		public bool IsMissionState(Spacecraft.MissionState state)
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>());
			return spacecraftFromLaunchConditionManager.state == state;
		}

		public void SetMissionState(Spacecraft.MissionState state)
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>());
			spacecraftFromLaunchConditionManager.SetState(state);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, LaunchableRocket>
	{
		public class NotGroundedStates : State
		{
			public State launch_pre;

			public State space;

			public State launch_loop;

			public State returning;

			public State landing_loop;
		}

		public State grounded;

		public NotGroundedStates not_grounded;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = grounded;
			base.serializable = true;
			grounded.EventTransition(GameHashes.LaunchRocket, not_grounded.launch_pre, null).Enter(delegate(StatesInstance smi)
			{
				smi.master.rocketSpeed = 0f;
				foreach (GameObject part in smi.master.parts)
				{
					if (!((Object)part == (Object)null))
					{
						part.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
					}
				}
				smi.SetMissionState(Spacecraft.MissionState.Grounded);
			});
			not_grounded.ToggleTag(GameTags.RocketNotOnGround);
			not_grounded.launch_pre.Enter(delegate(StatesInstance smi)
			{
				smi.master.isLanding = false;
				smi.master.rocketSpeed = 0f;
				smi.master.parts = AttachableBuilding.GetAttachedNetwork(smi.master.GetComponent<AttachableBuilding>());
				if ((Object)smi.master.soundSpeakerObject == (Object)null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				foreach (GameObject engine in smi.master.GetEngines())
				{
					engine.Trigger(-1358394196, null);
				}
				Game.Instance.Trigger(-1056989049, this);
				foreach (GameObject part2 in smi.master.parts)
				{
					if (!((Object)part2 == (Object)null))
					{
						smi.master.takeOffLocation = Grid.PosToCell(smi.master.gameObject);
						part2.Trigger(-1056989049, null);
					}
				}
				smi.SetMissionState(Spacecraft.MissionState.Launching);
			}).ScheduleGoTo(3f, not_grounded.launch_loop);
			not_grounded.launch_loop.EventTransition(GameHashes.ReturnRocket, not_grounded.returning, null).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.isLanding = false;
				bool flag2 = true;
				float num8 = Mathf.Clamp(Mathf.Pow(smi.timeinstate / 5f, 4f), 0f, 10f);
				smi.master.rocketSpeed = num8;
				smi.master.flightAnimOffset += dt * num8;
				foreach (GameObject part3 in smi.master.parts)
				{
					if (!((Object)part3 == (Object)null))
					{
						KBatchedAnimController component5 = part3.GetComponent<KBatchedAnimController>();
						component5.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 positionIncludingOffset3 = component5.PositionIncludingOffset;
						if ((Object)smi.master.soundSpeakerObject == (Object)null)
						{
							smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
							smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
						}
						smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
						Vector2I vector2I = Grid.PosToXY(positionIncludingOffset3);
						int y2 = vector2I.y;
						Vector2I visibleSize = Singleton<KBatchedAnimUpdater>.Instance.GetVisibleSize();
						if (y2 > visibleSize.y)
						{
							part3.GetComponent<RocketModule>().OnSuspend(null);
							part3.GetComponent<KBatchedAnimController>().enabled = false;
							if ((Object)part3.gameObject != (Object)smi.master.gameObject)
							{
								part3.gameObject.SetActive(false);
							}
						}
						else
						{
							flag2 = false;
							DoWorldDamage(part3, positionIncludingOffset3);
						}
					}
				}
				if (flag2)
				{
					smi.GoTo(not_grounded.space);
				}
			}, UpdateRate.SIM_33ms, false);
			not_grounded.space.Enter(delegate(StatesInstance smi)
			{
				smi.master.rocketSpeed = 0f;
				foreach (GameObject part4 in smi.master.parts)
				{
					if (!((Object)part4 == (Object)null))
					{
						part4.GetComponent<KBatchedAnimController>().Offset = Vector3.up * smi.master.flightAnimOffset;
						part4.GetComponent<KBatchedAnimController>().enabled = false;
						if ((Object)part4.gameObject != (Object)smi.master.gameObject)
						{
							part4.gameObject.SetActive(false);
						}
					}
				}
				smi.SetMissionState(Spacecraft.MissionState.Underway);
			}).EventTransition(GameHashes.ReturnRocket, not_grounded.returning, (StatesInstance smi) => smi.IsMissionState(Spacecraft.MissionState.WaitingToLand));
			not_grounded.returning.Enter(delegate(StatesInstance smi)
			{
				smi.master.isLanding = true;
				smi.master.rocketSpeed = 0f;
				foreach (GameObject part5 in smi.master.parts)
				{
					if (!((Object)part5 == (Object)null))
					{
						part5.gameObject.SetActive(true);
					}
				}
				smi.SetMissionState(Spacecraft.MissionState.Landing);
			}).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.isLanding = true;
				KBatchedAnimController component3 = smi.master.gameObject.GetComponent<KBatchedAnimController>();
				component3.Offset = Vector3.up * smi.master.flightAnimOffset;
				Vector3 position = smi.master.gameObject.transform.position;
				float y = position.y;
				Vector3 offset = component3.Offset;
				float num4 = y + offset.y;
				Vector3 vector = Grid.CellToPos(smi.master.takeOffLocation) + Vector3.down * (Grid.CellSizeInMeters / 2f);
				float num5 = Mathf.Abs(num4 - vector.y);
				float num6 = 0f;
				float num7 = 0.5f;
				num6 = Mathf.Clamp(num7 * num5, 0f, 10f) * dt;
				smi.master.rocketSpeed = num6;
				smi.master.flightAnimOffset -= num6;
				bool flag = true;
				if ((Object)smi.master.soundSpeakerObject == (Object)null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
				foreach (GameObject part6 in smi.master.parts)
				{
					if (!((Object)part6 == (Object)null))
					{
						KBatchedAnimController component4 = part6.GetComponent<KBatchedAnimController>();
						component4.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 positionIncludingOffset2 = component4.PositionIncludingOffset;
						if (Grid.IsValidCell(Grid.PosToCell(part6)))
						{
							part6.GetComponent<KBatchedAnimController>().enabled = true;
						}
						else
						{
							flag = false;
						}
						DoWorldDamage(part6, positionIncludingOffset2);
					}
				}
				if (flag)
				{
					smi.GoTo(not_grounded.landing_loop);
				}
			}, UpdateRate.SIM_33ms, false);
			not_grounded.landing_loop.Enter(delegate(StatesInstance smi)
			{
				smi.master.isLanding = true;
				int num3 = -1;
				for (int i = 0; i < smi.master.parts.Count; i++)
				{
					GameObject gameObject = smi.master.parts[i];
					if (!((Object)gameObject == (Object)null) && (Object)gameObject != (Object)smi.master.gameObject && (Object)gameObject.GetComponent<RocketEngine>() != (Object)null)
					{
						num3 = i;
					}
				}
				if (num3 != -1)
				{
					smi.master.parts[num3].Trigger(-1358394196, null);
				}
			}).Update(delegate(StatesInstance smi, float dt)
			{
				KBatchedAnimController component = smi.master.gameObject.GetComponent<KBatchedAnimController>();
				component.Offset = Vector3.up * smi.master.flightAnimOffset;
				float flightAnimOffset = smi.master.flightAnimOffset;
				float num = 0f;
				float num2 = 0.5f;
				num = Mathf.Clamp(num2 * flightAnimOffset, 0f, 10f);
				smi.master.rocketSpeed = num;
				smi.master.flightAnimOffset -= num * dt;
				if ((Object)smi.master.soundSpeakerObject == (Object)null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
				if (num <= 0.0025f && dt != 0f)
				{
					smi.master.GetComponent<KSelectable>().IsSelectable = true;
					num = 0f;
					foreach (GameObject part7 in smi.master.parts)
					{
						if (!((Object)part7 == (Object)null))
						{
							part7.Trigger(238242047, null);
						}
					}
					smi.GoTo(grounded);
				}
				else
				{
					foreach (GameObject part8 in smi.master.parts)
					{
						if (!((Object)part8 == (Object)null))
						{
							KBatchedAnimController component2 = part8.GetComponent<KBatchedAnimController>();
							component2.Offset = Vector3.up * smi.master.flightAnimOffset;
							Vector3 positionIncludingOffset = component2.PositionIncludingOffset;
							DoWorldDamage(part8, positionIncludingOffset);
						}
					}
				}
			}, UpdateRate.SIM_33ms, false);
		}

		private static void DoWorldDamage(GameObject part, Vector3 apparentPosition)
		{
			OccupyArea component = part.GetComponent<OccupyArea>();
			component.UpdateOccupiedArea();
			CellOffset[] occupiedCellsOffsets = component.OccupiedCellsOffsets;
			foreach (CellOffset offset in occupiedCellsOffsets)
			{
				int num = Grid.OffsetCell(Grid.PosToCell(apparentPosition), offset);
				if (Grid.IsValidCell(num))
				{
					if (Grid.Solid[num])
					{
						WorldDamage instance = WorldDamage.Instance;
						int cell = num;
						float amount = 10000f;
						int src_cell = num;
						string source_name = BUILDINGS.DAMAGESOURCES.ROCKET;
						instance.ApplyDamage(cell, amount, src_cell, -1, source_name, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET);
					}
					else if (Grid.FakeFloor[num])
					{
						GameObject gameObject = Grid.Objects[num, 36];
						if ((Object)gameObject != (Object)null)
						{
							BuildingHP component2 = gameObject.GetComponent<BuildingHP>();
							if ((Object)component2 != (Object)null)
							{
								gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo
								{
									damage = component2.MaxHitPoints,
									source = (string)BUILDINGS.DAMAGESOURCES.ROCKET,
									popString = (string)UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET
								});
							}
						}
					}
				}
			}
		}
	}

	private class UpdateRocketLandingParameter : LoopingSoundParameterUpdater
	{
		private struct Entry
		{
			public RocketModule rocketModule;

			public EventInstance ev;

			public int parameterIdx;
		}

		private List<Entry> entries = new List<Entry>();

		public UpdateRocketLandingParameter()
			: base("rocketLanding")
		{
		}

		public override void Add(Sound sound)
		{
			Entry entry = default(Entry);
			entry.rocketModule = sound.transform.GetComponent<RocketModule>();
			entry.ev = sound.ev;
			entry.parameterIdx = sound.description.GetParameterIdx(base.parameter);
			Entry item = entry;
			entries.Add(item);
		}

		public override void Update(float dt)
		{
			foreach (Entry entry in entries)
			{
				Entry current = entry;
				if (!((Object)current.rocketModule == (Object)null))
				{
					LaunchConditionManager conditionManager = current.rocketModule.conditionManager;
					if (!((Object)conditionManager == (Object)null))
					{
						LaunchableRocket component = conditionManager.GetComponent<LaunchableRocket>();
						if (!((Object)component == (Object)null))
						{
							if (component.isLanding)
							{
								current.ev.setParameterValueByIndex(current.parameterIdx, 1f);
							}
							else
							{
								current.ev.setParameterValueByIndex(current.parameterIdx, 0f);
							}
						}
					}
				}
			}
		}

		public override void Remove(Sound sound)
		{
			int num = 0;
			while (true)
			{
				if (num >= entries.Count)
				{
					return;
				}
				Entry entry = entries[num];
				if (entry.ev.handle == sound.ev.handle)
				{
					break;
				}
				num++;
			}
			entries.RemoveAt(num);
		}
	}

	private class UpdateRocketSpeedParameter : LoopingSoundParameterUpdater
	{
		private struct Entry
		{
			public RocketModule rocketModule;

			public EventInstance ev;

			public int parameterIdx;
		}

		private List<Entry> entries = new List<Entry>();

		public UpdateRocketSpeedParameter()
			: base("rocketSpeed")
		{
		}

		public override void Add(Sound sound)
		{
			Entry entry = default(Entry);
			entry.rocketModule = sound.transform.GetComponent<RocketModule>();
			entry.ev = sound.ev;
			entry.parameterIdx = sound.description.GetParameterIdx(base.parameter);
			Entry item = entry;
			entries.Add(item);
		}

		public override void Update(float dt)
		{
			foreach (Entry entry in entries)
			{
				Entry current = entry;
				if (!((Object)current.rocketModule == (Object)null))
				{
					LaunchConditionManager conditionManager = current.rocketModule.conditionManager;
					if (!((Object)conditionManager == (Object)null))
					{
						LaunchableRocket component = conditionManager.GetComponent<LaunchableRocket>();
						if (!((Object)component == (Object)null))
						{
							current.ev.setParameterValueByIndex(current.parameterIdx, component.rocketSpeed);
						}
					}
				}
			}
		}

		public override void Remove(Sound sound)
		{
			int num = 0;
			while (true)
			{
				if (num >= entries.Count)
				{
					return;
				}
				Entry entry = entries[num];
				if (entry.ev.handle == sound.ev.handle)
				{
					break;
				}
				num++;
			}
			entries.RemoveAt(num);
		}
	}

	public List<GameObject> parts = new List<GameObject>();

	[Serialize]
	private int takeOffLocation;

	[Serialize]
	private float flightAnimOffset = 0f;

	private bool isLanding;

	private float rocketSpeed;

	private GameObject soundSpeakerObject;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.master.parts = AttachableBuilding.GetAttachedNetwork(base.smi.master.GetComponent<AttachableBuilding>());
		int spacecraftID = SpacecraftManager.instance.GetSpacecraftID(this);
		if (spacecraftID == -1)
		{
			Spacecraft spacecraft = new Spacecraft(GetComponent<LaunchConditionManager>());
			spacecraft.GenerateName();
			SpacecraftManager.instance.RegisterSpacecraft(spacecraft);
		}
		base.smi.StartSM();
	}

	public List<GameObject> GetEngines()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject part in parts)
		{
			if ((bool)part.GetComponent<RocketEngine>())
			{
				list.Add(part);
			}
		}
		return list;
	}

	protected override void OnCleanUp()
	{
		SpacecraftManager.instance.UnregisterSpacecraft(GetComponent<LaunchConditionManager>());
		base.OnCleanUp();
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}
}
