using System;
using UnityEngine;

public class MeterController
{
	public GameObject gameObject;

	private KAnimLink link;

	public KBatchedAnimController meterController
	{
		get;
		private set;
	}

	public MeterController(KMonoBehaviour target, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, params string[] symbols_to_hide)
	{
		string[] array = new string[symbols_to_hide.Length + 1];
		Array.Copy(symbols_to_hide, array, symbols_to_hide.Length);
		array[array.Length - 1] = "meter_target";
		KBatchedAnimController component = target.GetComponent<KBatchedAnimController>();
		Initialize(component, "meter_target", "meter", front_back, user_specified_render_layer, Vector3.zero, array);
	}

	public MeterController(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, params string[] symbols_to_hide)
	{
		Initialize(building_controller, meter_target, meter_animation, front_back, user_specified_render_layer, Vector3.zero, symbols_to_hide);
	}

	public MeterController(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, Vector3 tracker_offset, params string[] symbols_to_hide)
	{
		Initialize(building_controller, meter_target, meter_animation, front_back, user_specified_render_layer, tracker_offset, symbols_to_hide);
	}

	public MeterController(KAnimControllerBase building_controller, KBatchedAnimController meter_controller, params string[] symbol_names)
	{
		if (!((UnityEngine.Object)meter_controller == (UnityEngine.Object)null))
		{
			meterController = meter_controller;
			link = new KAnimLink(building_controller, meter_controller);
			for (int i = 0; i < symbol_names.Length; i++)
			{
				building_controller.SetSymbolVisiblity(symbol_names[i], false);
			}
			KBatchedAnimTracker component = meterController.GetComponent<KBatchedAnimTracker>();
			component.symbol = new HashedString(symbol_names[0]);
		}
	}

	private void Initialize(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, Vector3 tracker_offset, params string[] symbols_to_hide)
	{
		string name = building_controller.name + "." + meter_animation;
		GameObject gameObject = UnityEngine.Object.Instantiate(Assets.GetPrefab(MeterConfig.ID));
		gameObject.name = name;
		gameObject.SetActive(false);
		gameObject.transform.parent = building_controller.transform;
		this.gameObject = gameObject;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.PrefabTag = new Tag(name);
		Vector3 position = building_controller.transform.GetPosition();
		switch (front_back)
		{
		case Meter.Offset.Behind:
		{
			Vector3 position3 = building_controller.transform.GetPosition();
			position.z = position3.z + 0.1f;
			break;
		}
		case Meter.Offset.Infront:
		{
			Vector3 position2 = building_controller.transform.GetPosition();
			position.z = position2.z - 0.1f;
			break;
		}
		case Meter.Offset.UserSpecified:
			position.z = Grid.GetLayerZ(user_specified_render_layer);
			break;
		}
		gameObject.transform.SetPosition(position);
		KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
		component2.AnimFiles = new KAnimFile[1]
		{
			building_controller.AnimFiles[0]
		};
		component2.initialAnim = meter_animation;
		component2.fgLayer = Grid.SceneLayer.NoLayer;
		component2.initialMode = KAnim.PlayMode.Paused;
		component2.isMovable = true;
		component2.FlipX = building_controller.FlipX;
		component2.FlipY = building_controller.FlipY;
		if (front_back == Meter.Offset.UserSpecified)
		{
			component2.sceneLayer = user_specified_render_layer;
		}
		meterController = component2;
		KBatchedAnimTracker component3 = gameObject.GetComponent<KBatchedAnimTracker>();
		component3.offset = tracker_offset;
		component3.symbol = new HashedString(meter_target);
		gameObject.SetActive(true);
		building_controller.SetSymbolVisiblity(meter_target, false);
		if (symbols_to_hide != null)
		{
			for (int i = 0; i < symbols_to_hide.Length; i++)
			{
				building_controller.SetSymbolVisiblity(symbols_to_hide[i], false);
			}
		}
		link = new KAnimLink(building_controller, component2);
	}

	public void SetPositionPercent(float percent_full)
	{
		if (!((UnityEngine.Object)meterController == (UnityEngine.Object)null))
		{
			meterController.SetPositionPercent(percent_full);
		}
	}

	public void SetSymbolTint(KAnimHashedString symbol, Color32 colour)
	{
		if ((UnityEngine.Object)meterController != (UnityEngine.Object)null)
		{
			meterController.SetSymbolTint(symbol, colour);
		}
	}

	public void SetRotation(float rot)
	{
		if (!((UnityEngine.Object)meterController == (UnityEngine.Object)null))
		{
			meterController.Rotation = rot;
		}
	}
}
