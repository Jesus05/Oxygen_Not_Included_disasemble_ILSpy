using KSerialization;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CO2 : KMonoBehaviour
{
	[NonSerialized]
	[Serialize]
	public Vector3 velocity = Vector3.zero;

	[NonSerialized]
	[Serialize]
	public float mass;

	[NonSerialized]
	[Serialize]
	public float temperature;

	[NonSerialized]
	[Serialize]
	public float lifetimeRemaining;

	public void StartLoop()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.Play("exhale_pre", KAnim.PlayMode.Once, 1f, 0f);
		component.Play("exhale_loop", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void TriggerDestroy()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.Play("exhale_pst", KAnim.PlayMode.Once, 1f, 0f);
	}
}
