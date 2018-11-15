using System;
using UnityEngine;

public static class EntityTemplateExtensions
{
	public static DefType AddOrGetDef<DefType>(this GameObject go) where DefType : StateMachine.BaseDef
	{
		StateMachineController stateMachineController = go.AddOrGet<StateMachineController>();
		DefType val = stateMachineController.GetDef<DefType>();
		if (val == null)
		{
			val = Activator.CreateInstance<DefType>();
			stateMachineController.AddDef(val);
			val.Configure(stateMachineController.gameObject);
		}
		return val;
	}

	public static ComponentType AddOrGet<ComponentType>(this GameObject go) where ComponentType : Component
	{
		ComponentType val = go.GetComponent<ComponentType>();
		if ((UnityEngine.Object)val == (UnityEngine.Object)null)
		{
			val = go.AddComponent<ComponentType>();
		}
		KMonoBehaviour kMonoBehaviour = val as KMonoBehaviour;
		if ((UnityEngine.Object)kMonoBehaviour != (UnityEngine.Object)null)
		{
			kMonoBehaviour.CreateDef();
		}
		return val;
	}
}
