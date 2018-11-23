using System;
using UnityEngine;

public class GravityComponents : KGameObjectComponentManager<GravityComponent>
{
	public class Tuning : TuningData<Tuning>
	{
		public float maxVelocity;

		public float maxVelocityInLiquid;
	}

	private const float Acceleration = -9.8f;

	public HandleVector<int>.Handle Add(GameObject go, Vector2 initial_velocity, System.Action on_landed = null)
	{
		return Add(go, new GravityComponent(go.transform, on_landed, initial_velocity));
	}

	public override void FixedUpdate(float dt)
	{
		Tuning tuning = TuningData<Tuning>.Get();
		float num = tuning.maxVelocity * tuning.maxVelocity;
		for (int i = 0; i < data.Count; i++)
		{
			GravityComponent value = data[i];
			if (!(value.elapsedTime < 0f) && !((UnityEngine.Object)value.transform == (UnityEngine.Object)null))
			{
				Vector3 position = value.transform.GetPosition();
				Vector2 vector = position;
				Vector2 vector2 = new Vector2(value.velocity.x, value.velocity.y + -9.8f * dt);
				float sqrMagnitude = vector2.sqrMagnitude;
				if (sqrMagnitude > num)
				{
					vector2 *= tuning.maxVelocity / Mathf.Sqrt(sqrMagnitude);
				}
				int num2 = Grid.PosToCell(vector);
				bool flag = Grid.IsVisiblyInLiquid(vector + new Vector2(0f, value.radius));
				if (flag)
				{
					flag = true;
					float num3 = (float)(value.transform.GetInstanceID() % 1000) / 1000f * 0.25f;
					float num4 = tuning.maxVelocityInLiquid + num3 * tuning.maxVelocityInLiquid;
					if (sqrMagnitude > num4 * num4)
					{
						float num5 = Mathf.Sqrt(sqrMagnitude);
						vector2 = vector2 / num5 * Mathf.Lerp(num5, num3, dt * (5f + 5f * num3));
					}
				}
				value.velocity = vector2;
				value.elapsedTime += dt;
				Vector2 vector3 = vector + vector2 * dt;
				Vector2 pos = vector3;
				pos.y -= value.radius;
				bool flag2 = Grid.IsVisiblyInLiquid(vector3 + new Vector2(0f, value.radius));
				if (!flag && flag2)
				{
					KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("splash_step_kanim", new Vector3(vector3.x, vector3.y, 0f) + new Vector3(-0.38f, 0.75f, -0.1f), null, false, Grid.SceneLayer.FXFront, false);
					kBatchedAnimController.Play("fx1", KAnim.PlayMode.Once, 1f, 0f);
					kBatchedAnimController.destroyOnAnimComplete = true;
				}
				int num6 = Grid.PosToCell(pos);
				if (Grid.IsValidCell(num6))
				{
					if (vector2.sqrMagnitude > 0.2f && Grid.IsValidCell(num2) && !Grid.Element[num2].IsLiquid && Grid.Element[num6].IsLiquid)
					{
						AmbienceType ambience = Grid.Element[num6].substance.GetAmbience();
						if (ambience != AmbienceType.None)
						{
							string text = Sounds.Instance.OreSplashSoundsMigrated[(int)ambience];
							if ((UnityEngine.Object)CameraController.Instance != (UnityEngine.Object)null && CameraController.Instance.IsAudibleSound(vector3, text))
							{
								SoundEvent.PlayOneShot(text, vector3);
							}
						}
					}
					if (Grid.Solid[num6] || (Grid.LiquidPumpFloor[num6] && (UnityEngine.Object)value.transform.GetComponent<MinionIdentity>() != (UnityEngine.Object)null))
					{
						Vector3 vector4 = Grid.CellToPosCBC(Grid.CellAbove(num6), Grid.SceneLayer.Move);
						vector3.y = vector4.y + value.radius;
						value.velocity.x = 0f;
						value.elapsedTime = -1f;
						value.transform.SetPosition(new Vector3(vector3.x, vector3.y, position.z));
						data[i] = value;
						value.transform.gameObject.Trigger(1188683690, vector2);
						if (value.onLanded != null)
						{
							value.onLanded();
						}
					}
					else
					{
						Vector2 pos2 = vector3;
						pos2.x -= value.radius;
						int num7 = Grid.PosToCell(pos2);
						if (Grid.IsValidCell(num7) && Grid.Solid[num7])
						{
							vector3.x = Mathf.Floor(vector3.x - value.radius) + (1f + value.radius);
							value.velocity.x = -0.1f * value.velocity.x;
							data[i] = value;
						}
						else
						{
							Vector3 pos3 = vector3;
							pos3.x += value.radius;
							int num8 = Grid.PosToCell(pos3);
							if (Grid.IsValidCell(num8) && Grid.Solid[num8])
							{
								vector3.x = Mathf.Floor(vector3.x + value.radius) - value.radius;
								value.velocity.x = -0.1f * value.velocity.x;
								data[i] = value;
							}
						}
						value.transform.SetPosition(new Vector3(vector3.x, vector3.y, position.z));
						data[i] = value;
					}
				}
				else
				{
					value.transform.SetPosition(new Vector3(vector3.x, vector3.y, position.z));
					data[i] = value;
				}
			}
		}
	}
}
