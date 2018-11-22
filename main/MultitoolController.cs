using System;
using System.Collections.Generic;
using UnityEngine;

public class MultitoolController : GameStateMachine<MultitoolController, MultitoolController.Instance, Worker>
{
	public new class Instance : GameInstance
	{
		private GameObject hitEffectPrefab;

		private GameObject hitEffect;

		private string[] anims;

		private bool inPlace;

		public Instance(Workable workable, Worker worker, HashedString context, GameObject hit_effect)
			: base(worker)
		{
			hitEffectPrefab = hit_effect;
			worker.GetComponent<AnimEventHandler>().SetContext(context);
			base.sm.worker.Set(worker, base.smi);
			base.sm.workable.Set(workable, base.smi);
			anims = GetAnimationStrings(workable, worker, "dig");
		}

		public void PlayPre()
		{
			base.sm.worker.Get<KAnimControllerBase>(base.smi).Play(anims[0], KAnim.PlayMode.Once, 1f, 0f);
		}

		public void PlayLoop()
		{
			KAnimControllerBase kAnimControllerBase = base.sm.worker.Get<KAnimControllerBase>(base.smi);
			if (kAnimControllerBase.currentAnim != anims[1])
			{
				base.sm.worker.Get<KAnimControllerBase>(base.smi).Play(anims[1], KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public void PlayPost()
		{
			KAnimControllerBase kAnimControllerBase = base.sm.worker.Get<KAnimControllerBase>(base.smi);
			if (kAnimControllerBase.currentAnim != anims[2])
			{
				base.sm.worker.Get<KAnimControllerBase>(base.smi).Play(anims[2], KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		public void UpdateHitEffectTarget()
		{
			if (!((UnityEngine.Object)hitEffect == (UnityEngine.Object)null))
			{
				Workable workable = base.sm.workable.Get<Workable>(base.smi);
				Worker worker = base.sm.worker.Get<Worker>(base.smi);
				AnimEventHandler component = worker.GetComponent<AnimEventHandler>();
				Vector3 targetPoint = workable.GetTargetPoint();
				worker.GetComponent<Facing>().Face(workable.transform.GetPosition());
				anims = GetAnimationStrings(workable, worker, "dig");
				PlayLoop();
				component.SetTargetPos(targetPoint);
				component.UpdateWorkTarget(workable.GetTargetPoint());
				hitEffect.transform.SetPosition(targetPoint);
			}
		}

		public void CreateHitEffect()
		{
			Worker worker = base.sm.worker.Get<Worker>(base.smi);
			Workable workable = base.sm.workable.Get<Workable>(base.smi);
			if (!((UnityEngine.Object)worker == (UnityEngine.Object)null) && !((UnityEngine.Object)workable == (UnityEngine.Object)null))
			{
				if (Grid.PosToCell(workable) != Grid.PosToCell(worker))
				{
					worker.Trigger(-673283254, null);
				}
				Diggable diggable = workable as Diggable;
				if ((bool)diggable)
				{
					Element targetElement = diggable.GetTargetElement();
					worker.Trigger(-1762453998, targetElement);
				}
				if (!((UnityEngine.Object)hitEffectPrefab == (UnityEngine.Object)null))
				{
					if ((UnityEngine.Object)hitEffect != (UnityEngine.Object)null)
					{
						DestroyHitEffect();
					}
					AnimEventHandler component = worker.GetComponent<AnimEventHandler>();
					Vector3 targetPoint = workable.GetTargetPoint();
					component.SetTargetPos(targetPoint);
					hitEffect = GameUtil.KInstantiate(hitEffectPrefab, targetPoint, Grid.SceneLayer.FXFront2, null, 0);
					KBatchedAnimController component2 = hitEffect.GetComponent<KBatchedAnimController>();
					hitEffect.SetActive(true);
					component2.sceneLayer = Grid.SceneLayer.FXFront2;
					component2.enabled = false;
					component2.enabled = true;
					component.UpdateWorkTarget(workable.GetTargetPoint());
				}
			}
		}

		public void DestroyHitEffect()
		{
			Worker worker = base.sm.worker.Get<Worker>(base.smi);
			if ((UnityEngine.Object)worker != (UnityEngine.Object)null)
			{
				worker.Trigger(-1559999068, null);
				worker.Trigger(939543986, null);
			}
			if (!((UnityEngine.Object)hitEffectPrefab == (UnityEngine.Object)null) && !((UnityEngine.Object)hitEffect == (UnityEngine.Object)null))
			{
				hitEffect.DeleteObject();
			}
		}
	}

	private enum DigDirection
	{
		dig_down,
		dig_up
	}

	public State pre;

	public State loop;

	public State pst;

	public TargetParameter worker;

	public TargetParameter workable;

	private static readonly string[][][] ANIM_BASE = new string[5][][]
	{
		new string[4][]
		{
			new string[3]
			{
				"{verb}_dn_pre",
				"{verb}_dn_loop",
				"{verb}_dn_pst"
			},
			new string[3]
			{
				"ladder_{verb}_dn_pre",
				"ladder_{verb}_dn_loop",
				"ladder_{verb}_dn_pst"
			},
			new string[3]
			{
				"pole_{verb}_dn_pre",
				"pole_{verb}_dn_loop",
				"pole_{verb}_dn_pst"
			},
			new string[3]
			{
				"jetpack_{verb}_dn_pre",
				"jetpack_{verb}_dn_loop",
				"jetpack_{verb}_dn_pst"
			}
		},
		new string[4][]
		{
			new string[3]
			{
				"{verb}_diag_dn_pre",
				"{verb}_diag_dn_loop",
				"{verb}_diag_dn_pst"
			},
			new string[3]
			{
				"ladder_{verb}_diag_dn_pre",
				"ladder_{verb}_loop_diag_dn",
				"ladder_{verb}_diag_dn_pst"
			},
			new string[3]
			{
				"pole_{verb}_diag_dn_pre",
				"pole_{verb}_loop_diag_dn",
				"pole_{verb}_diag_dn_pst"
			},
			new string[3]
			{
				"jetpack_{verb}_diag_dn_pre",
				"jetpack_{verb}_diag_dn_loop",
				"jetpack_{verb}_diag_dn_pst"
			}
		},
		new string[4][]
		{
			new string[3]
			{
				"{verb}_fwd_pre",
				"{verb}_fwd_loop",
				"{verb}_fwd_pst"
			},
			new string[3]
			{
				"ladder_{verb}_pre",
				"ladder_{verb}_loop",
				"ladder_{verb}_pst"
			},
			new string[3]
			{
				"pole_{verb}_pre",
				"pole_{verb}_loop",
				"pole_{verb}_pst"
			},
			new string[3]
			{
				"jetpack_{verb}_fwd_pre",
				"jetpack_{verb}_fwd_loop",
				"jetpack_{verb}_fwd_pst"
			}
		},
		new string[4][]
		{
			new string[3]
			{
				"{verb}_diag_up_pre",
				"{verb}_diag_up_loop",
				"{verb}_diag_up_pst"
			},
			new string[3]
			{
				"ladder_{verb}_diag_up_pre",
				"ladder_{verb}_loop_diag_up",
				"ladder_{verb}_diag_up_pst"
			},
			new string[3]
			{
				"pole_{verb}_diag_up_pre",
				"pole_{verb}_loop_diag_up",
				"pole_{verb}_diag_up_pst"
			},
			new string[3]
			{
				"jetpack_{verb}_diag_up_pre",
				"jetpack_{verb}_diag_up_loop",
				"jetpack_{verb}_diag_up_pst"
			}
		},
		new string[4][]
		{
			new string[3]
			{
				"{verb}_up_pre",
				"{verb}_up_loop",
				"{verb}_up_pst"
			},
			new string[3]
			{
				"ladder_{verb}_up_pre",
				"ladder_{verb}_up_loop",
				"ladder_{verb}_up_pst"
			},
			new string[3]
			{
				"pole_{verb}_up_pre",
				"pole_{verb}_up_loop",
				"pole_{verb}_up_pst"
			},
			new string[3]
			{
				"jetpack_{verb}_up_pre",
				"jetpack_{verb}_up_loop",
				"jetpack_{verb}_up_pst"
			}
		}
	};

	private static Dictionary<string, string[][][]> TOOL_ANIM_SETS = new Dictionary<string, string[][][]>();

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = pre;
		Target(worker);
		root.ToggleSnapOn("dig");
		pre.Enter(delegate(Instance smi)
		{
			smi.PlayPre();
			worker.Get<Facing>(smi).Face(workable.Get(smi).transform.GetPosition());
		}).OnAnimQueueComplete(loop);
		loop.Enter("PlayLoop", delegate(Instance smi)
		{
			smi.PlayLoop();
		}).Enter("CreateHitEffect", delegate(Instance smi)
		{
			smi.CreateHitEffect();
		}).Exit("DestroyHitEffect", delegate(Instance smi)
		{
			smi.DestroyHitEffect();
		})
			.EventTransition(GameHashes.WorkerPlayPostAnim, pst, null);
		pst.Enter("PlayPost", delegate(Instance smi)
		{
			smi.PlayPost();
		});
	}

	public static string[] GetAnimationStrings(Workable workable, Worker worker, string toolString = "dig")
	{
		if (!TOOL_ANIM_SETS.TryGetValue(toolString, out string[][][] value))
		{
			value = new string[ANIM_BASE.Length][][];
			TOOL_ANIM_SETS[toolString] = value;
			for (int i = 0; i < value.Length; i++)
			{
				string[][] array = ANIM_BASE[i];
				string[][] array2 = value[i] = new string[array.Length][];
				for (int j = 0; j < array2.Length; j++)
				{
					string[] array3 = array[j];
					string[] array4 = array2[j] = new string[array3.Length];
					for (int k = 0; k < array4.Length; k++)
					{
						array4[k] = array3[k].Replace("{verb}", toolString);
					}
				}
			}
		}
		Vector3 target = Vector3.zero;
		Vector3 source = Vector3.zero;
		GetTargetPoints(workable, worker, out source, out target);
		Vector2 normalized = new Vector2(target.x - source.x, target.y - source.y).normalized;
		float num = Vector2.Angle(new Vector2(0f, -1f), normalized);
		float num2 = Mathf.Lerp(0f, 1f, num / 180f);
		int num3 = value.Length;
		int val = (int)(num2 * (float)num3);
		val = Math.Min(val, num3 - 1);
		NavType currentNavType = worker.GetComponent<Navigator>().CurrentNavType;
		int num4 = 0;
		switch (currentNavType)
		{
		case NavType.Ladder:
			num4 = 1;
			break;
		case NavType.Pole:
			num4 = 2;
			break;
		case NavType.Hover:
			num4 = 3;
			break;
		}
		return value[val][num4];
	}

	private static void GetTargetPoints(Workable workable, Worker worker, out Vector3 source, out Vector3 target)
	{
		target = workable.GetTargetPoint();
		source = worker.transform.GetPosition();
		source.y += 0.7f;
	}
}
