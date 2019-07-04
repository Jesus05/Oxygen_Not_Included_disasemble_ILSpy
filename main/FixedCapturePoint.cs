using KSerialization;
using STRINGS;
using System;
using UnityEngine;

public class FixedCapturePoint : GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>
{
	public class Def : BaseDef
	{
		public Func<GameObject, Instance, bool> isCreatureEligibleToBeCapturedCb;

		public Func<Instance, int> getTargetCapturePoint = delegate(Instance smi)
		{
			int num = Grid.PosToCell(smi);
			Navigator component = smi.targetCapturable.GetComponent<Navigator>();
			if (Grid.IsValidCell(num - 1) && component.CanReach(num - 1))
			{
				return num - 1;
			}
			if (Grid.IsValidCell(num + 1) && component.CanReach(num + 1))
			{
				return num + 1;
			}
			return num;
		};
	}

	public class OperationalState : State
	{
		public State manual;

		public State automated;
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public new class Instance : GameInstance, ICheckboxControl
	{
		private struct CapturableIterator : GameScenePartitioner.Iterator
		{
			private CavityInfo captureCavityInfo;

			private int captureCell;

			private Instance capturePoint;

			public FixedCapturableMonitor.Instance result
			{
				get;
				private set;
			}

			public CapturableIterator(Instance capture_point, CavityInfo capture_cavity_info, int capture_cell)
			{
				capturePoint = capture_point;
				captureCavityInfo = capture_cavity_info;
				captureCell = capture_cell;
				result = null;
			}

			public void Iterate(object target_obj)
			{
				KMonoBehaviour kMonoBehaviour = target_obj as KMonoBehaviour;
				if (!((UnityEngine.Object)kMonoBehaviour == (UnityEngine.Object)null))
				{
					FixedCapturableMonitor.Instance sMI = kMonoBehaviour.GetSMI<FixedCapturableMonitor.Instance>();
					if (sMI != null && CanCapturableBeCapturedAtCapturePoint(sMI, capturePoint, captureCavityInfo, captureCell))
					{
						result = sMI;
					}
				}
			}

			public void Cleanup()
			{
			}
		}

		string ICheckboxControl.CheckboxTitleKey
		{
			get
			{
				StringKey key = UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.TITLE.key;
				return key.String;
			}
		}

		string ICheckboxControl.CheckboxLabel
		{
			get
			{
				return UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.AUTOWRANGLE;
			}
		}

		string ICheckboxControl.CheckboxTooltip
		{
			get
			{
				return UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.AUTOWRANGLE_TOOLTIP;
			}
		}

		public FixedCapturableMonitor.Instance targetCapturable
		{
			get;
			private set;
		}

		public bool shouldCreatureGoGetCaptured
		{
			get;
			private set;
		}

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			Subscribe(-905833192, OnCopySettings);
		}

		private void OnCopySettings(object data)
		{
			GameObject gameObject = (GameObject)data;
			if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
			{
				Instance sMI = gameObject.GetSMI<Instance>();
				if (sMI != null)
				{
					base.sm.automated.Set(base.sm.automated.Get(sMI), this);
				}
			}
		}

		public Chore CreateChore()
		{
			FindFixedCapturable();
			return new FixedCaptureChore(GetComponent<KPrefabID>());
		}

		public bool IsCreatureAvailableForFixedCapture()
		{
			if (targetCapturable.IsNullOrStopped())
			{
				return false;
			}
			int num = Grid.PosToCell(base.transform.GetPosition());
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			return CanCapturableBeCapturedAtCapturePoint(targetCapturable, this, cavityForCell, num);
		}

		public void SetRancherIsAvailableForCapturing()
		{
			shouldCreatureGoGetCaptured = true;
		}

		public void ClearRancherIsAvailableForCapturing()
		{
			shouldCreatureGoGetCaptured = false;
		}

		private static bool CanCapturableBeCapturedAtCapturePoint(FixedCapturableMonitor.Instance capturable, Instance capture_point, CavityInfo capture_cavity_info, int capture_cell)
		{
			if (capturable.IsRunning())
			{
				if (!capturable.HasTag(GameTags.Creatures.Bagged))
				{
					if (capturable.targetCapturePoint != capture_point && !capturable.targetCapturePoint.IsNullOrStopped())
					{
						return false;
					}
					if (capture_point.def.isCreatureEligibleToBeCapturedCb != null && !capture_point.def.isCreatureEligibleToBeCapturedCb(capturable.gameObject, capture_point))
					{
						return false;
					}
					if (capturable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<FixedCaptureStates>())
					{
						int cell = Grid.PosToCell(capturable.transform.GetPosition());
						CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
						if (cavityForCell != null && cavityForCell == capture_cavity_info)
						{
							int navigationCost = capturable.GetComponent<Navigator>().GetNavigationCost(capture_cell);
							if (navigationCost != -1)
							{
								TreeFilterable component = capture_point.GetComponent<TreeFilterable>();
								IUserControlledCapacity component2 = capture_point.GetComponent<IUserControlledCapacity>();
								if (component.ContainsTag(capturable.GetComponent<KPrefabID>().PrefabTag) && component2.AmountStored <= component2.UserMaxCapacity)
								{
									return false;
								}
								return true;
							}
							return false;
						}
						return false;
					}
					return false;
				}
				return false;
			}
			return false;
		}

		public void FindFixedCapturable()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			if (cavityForCell == null)
			{
				ResetCapturePoint();
			}
			else
			{
				if (!targetCapturable.IsNullOrStopped() && !CanCapturableBeCapturedAtCapturePoint(targetCapturable, this, cavityForCell, num))
				{
					ResetCapturePoint();
				}
				if (targetCapturable.IsNullOrStopped())
				{
					CapturableIterator iterator = new CapturableIterator(this, cavityForCell, num);
					GameScenePartitioner.Instance.Iterate(cavityForCell.minX, cavityForCell.minY, cavityForCell.maxX - cavityForCell.minX + 1, cavityForCell.maxY - cavityForCell.minY + 1, GameScenePartitioner.Instance.collisionLayer, ref iterator);
					iterator.Cleanup();
					targetCapturable = iterator.result;
					if (!targetCapturable.IsNullOrStopped())
					{
						targetCapturable.targetCapturePoint = this;
					}
				}
			}
		}

		public void ResetCapturePoint()
		{
			Trigger(643180843, null);
			if (!targetCapturable.IsNullOrStopped())
			{
				targetCapturable.targetCapturePoint = null;
				targetCapturable.Trigger(1034952693, null);
				targetCapturable = null;
			}
		}

		bool ICheckboxControl.GetCheckboxValue()
		{
			return base.sm.automated.Get(this);
		}

		void ICheckboxControl.SetCheckboxValue(bool value)
		{
			base.sm.automated.Set(value, this);
		}
	}

	private BoolParameter automated;

	public State unoperational;

	public OperationalState operational;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = operational;
		base.serializable = true;
		unoperational.TagTransition(GameTags.Operational, operational, false);
		operational.DefaultState(operational.manual).TagTransition(GameTags.Operational, unoperational, true);
		operational.manual.ParamTransition(automated, operational.automated, GameStateMachine<FixedCapturePoint, Instance, IStateMachineTarget, Def>.IsTrue);
		operational.automated.ParamTransition(automated, operational.manual, GameStateMachine<FixedCapturePoint, Instance, IStateMachineTarget, Def>.IsFalse).ToggleChore((Instance smi) => smi.CreateChore(), unoperational, unoperational).Update("FindFixedCapturable", delegate(Instance smi, float dt)
		{
			smi.FindFixedCapturable();
		}, UpdateRate.SIM_1000ms, false);
	}
}
