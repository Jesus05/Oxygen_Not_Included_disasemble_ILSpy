using System;
using UnityEngine;

public class OreSizeVisualizerComponents : KGameObjectComponentManager<OreSizeVisualizerData>
{
	private struct MassTier
	{
		public HashedString animName;

		public float massRequired;

		public float colliderRadius;
	}

	private static readonly MassTier[] MassTiers = new MassTier[3]
	{
		new MassTier
		{
			animName = (HashedString)"idle1",
			massRequired = 50f,
			colliderRadius = 0.15f
		},
		new MassTier
		{
			animName = (HashedString)"idle2",
			massRequired = 600f,
			colliderRadius = 0.2f
		},
		new MassTier
		{
			animName = (HashedString)"idle3",
			massRequired = 3.40282347E+38f,
			colliderRadius = 0.25f
		}
	};

	public HandleVector<int>.Handle Add(GameObject go)
	{
		HandleVector<int>.Handle handle = Add(go, new OreSizeVisualizerData(go));
		OnPrefabInit(handle);
		return handle;
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle handle)
	{
		Action<object> action = delegate(object ev_data)
		{
			OnMassChanged(handle, ev_data);
		};
		OreSizeVisualizerData data = GetData(handle);
		data.onMassChangedCB = action;
		data.primaryElement.Subscribe(-2064133523, action);
		data.primaryElement.Subscribe(1335436905, action);
		SetData(handle, data);
	}

	protected override void OnSpawn(HandleVector<int>.Handle handle)
	{
		OreSizeVisualizerData data = GetData(handle);
		OnMassChanged(handle, data.primaryElement.GetComponent<Pickupable>());
	}

	protected override void OnCleanUp(HandleVector<int>.Handle handle)
	{
		OreSizeVisualizerData data = GetData(handle);
		if ((UnityEngine.Object)data.primaryElement != (UnityEngine.Object)null)
		{
			Action<object> onMassChangedCB = data.onMassChangedCB;
			data.primaryElement.Unsubscribe(-2064133523, onMassChangedCB);
			data.primaryElement.Unsubscribe(1335436905, onMassChangedCB);
		}
	}

	private static void OnMassChanged(HandleVector<int>.Handle handle, object other_data)
	{
		OreSizeVisualizerData data = GameComps.OreSizeVisualizers.GetData(handle);
		PrimaryElement primaryElement = data.primaryElement;
		float num = primaryElement.Mass;
		if (other_data != null)
		{
			Pickupable pickupable = (Pickupable)other_data;
			PrimaryElement component = pickupable.GetComponent<PrimaryElement>();
			num += component.Mass;
		}
		MassTier massTier = default(MassTier);
		for (int i = 0; i < MassTiers.Length; i++)
		{
			if (num <= MassTiers[i].massRequired)
			{
				massTier = MassTiers[i];
				break;
			}
		}
		KBatchedAnimController component2 = primaryElement.GetComponent<KBatchedAnimController>();
		component2.Play(massTier.animName, KAnim.PlayMode.Once, 1f, 0f);
		KCircleCollider2D component3 = primaryElement.GetComponent<KCircleCollider2D>();
		if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
		{
			component3.radius = massTier.colliderRadius;
		}
		primaryElement.Trigger(1807976145, null);
	}
}
