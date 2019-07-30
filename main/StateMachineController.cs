using KSerialization;
using System;
using System.Collections.Generic;
using System.IO;

[SerializationConfig(MemberSerialization.OptIn)]
public class StateMachineController : KMonoBehaviour, ISaveLoadableDetails, IStateMachineControllerHack
{
	public class CmpDef
	{
		public List<StateMachine.BaseDef> defs = new List<StateMachine.BaseDef>();
	}

	public DefHandle defHandle;

	private List<StateMachine.Instance> stateMachines = new List<StateMachine.Instance>();

	private LoggerFSSSS log = new LoggerFSSSS("StateMachineController", 35);

	private StateMachineSerializer serializer = new StateMachineSerializer();

	private static readonly EventSystem.IntraObjectHandler<StateMachineController> OnTargetDestroyedDelegate = new EventSystem.IntraObjectHandler<StateMachineController>(delegate(StateMachineController component, object data)
	{
		component.OnTargetDestroyed(data);
	});

	public CmpDef cmpdef => defHandle.Get<CmpDef>();

	public IEnumerator<StateMachine.Instance> GetEnumerator()
	{
		return stateMachines.GetEnumerator();
	}

	public void AddStateMachineInstance(StateMachine.Instance state_machine)
	{
		if (!stateMachines.Contains(state_machine))
		{
			stateMachines.Add(state_machine);
		}
	}

	public void RemoveStateMachineInstance(StateMachine.Instance state_machine)
	{
		if (!state_machine.GetStateMachine().saveHistory && !state_machine.GetStateMachine().debugSettings.saveHistory)
		{
			stateMachines.Remove(state_machine);
		}
	}

	public bool HasStateMachineInstance(StateMachine.Instance state_machine)
	{
		return stateMachines.Contains(state_machine);
	}

	public void AddDef(StateMachine.BaseDef def)
	{
		cmpdef.defs.Add(def);
	}

	public LoggerFSSSS GetLog()
	{
		return log;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		log.SetName(base.name);
		Subscribe(1969584890, OnTargetDestroyedDelegate);
		Subscribe(1502190696, OnTargetDestroyedDelegate);
	}

	private void OnTargetDestroyed(object data)
	{
		while (stateMachines.Count > 0)
		{
			StateMachine.Instance instance = stateMachines[0];
			instance.StopSM("StateMachineController.OnCleanUp");
			stateMachines.Remove(instance);
		}
	}

	protected override void OnLoadLevel()
	{
		while (stateMachines.Count > 0)
		{
			StateMachine.Instance instance = stateMachines[0];
			instance.FreeResources();
			stateMachines.Remove(instance);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		while (stateMachines.Count > 0)
		{
			StateMachine.Instance instance = stateMachines[0];
			instance.StopSM("StateMachineController.OnCleanUp");
			stateMachines.Remove(instance);
		}
	}

	public void CreateSMIS()
	{
		if (defHandle.IsValid())
		{
			foreach (StateMachine.BaseDef def in cmpdef.defs)
			{
				def.CreateSMI(this);
			}
		}
	}

	public void StartSMIS()
	{
		if (defHandle.IsValid())
		{
			foreach (StateMachine.BaseDef def in cmpdef.defs)
			{
				StateMachine.Instance sMI = GetSMI(Singleton<StateMachineManager>.Instance.CreateStateMachine(def.GetStateMachineType()).GetStateMachineInstanceType());
				if (sMI != null && !sMI.IsRunning())
				{
					sMI.StartSM();
				}
			}
		}
	}

	public void Serialize(BinaryWriter writer)
	{
		serializer.Serialize(stateMachines, writer);
	}

	public void Deserialize(IReader reader)
	{
		serializer.Deserialize(reader);
	}

	public bool Restore(StateMachine.Instance smi)
	{
		return serializer.Restore(smi);
	}

	public DefType GetDef<DefType>() where DefType : StateMachine.BaseDef
	{
		if (!defHandle.IsValid())
		{
			return (DefType)null;
		}
		foreach (StateMachine.BaseDef def in cmpdef.defs)
		{
			DefType val = def as DefType;
			if (val != null)
			{
				return val;
			}
		}
		return (DefType)null;
	}

	public List<DefType> GetDefs<DefType>() where DefType : StateMachine.BaseDef
	{
		List<DefType> list = new List<DefType>();
		if (!defHandle.IsValid())
		{
			return list;
		}
		foreach (StateMachine.BaseDef def in cmpdef.defs)
		{
			DefType val = def as DefType;
			if (val != null)
			{
				list.Add(val);
			}
		}
		return list;
	}

	public StateMachine.Instance GetSMI(Type type)
	{
		for (int i = 0; i < stateMachines.Count; i++)
		{
			StateMachine.Instance instance = stateMachines[i];
			if (type.IsAssignableFrom(instance.GetType()))
			{
				return instance;
			}
		}
		return null;
	}

	public StateMachineInstanceType GetSMI<StateMachineInstanceType>() where StateMachineInstanceType : class
	{
		return GetSMI(typeof(StateMachineInstanceType)) as StateMachineInstanceType;
	}

	public List<StateMachineInstanceType> GetAllSMI<StateMachineInstanceType>() where StateMachineInstanceType : class
	{
		List<StateMachineInstanceType> list = new List<StateMachineInstanceType>();
		foreach (StateMachine.Instance stateMachine in stateMachines)
		{
			StateMachineInstanceType val = stateMachine as StateMachineInstanceType;
			if (val != null)
			{
				list.Add(val);
			}
		}
		return list;
	}

	public List<IGameObjectEffectDescriptor> GetDescriptors()
	{
		List<IGameObjectEffectDescriptor> list = new List<IGameObjectEffectDescriptor>();
		if (!defHandle.IsValid())
		{
			return list;
		}
		foreach (StateMachine.BaseDef def in cmpdef.defs)
		{
			if (def is IGameObjectEffectDescriptor)
			{
				list.Add(def as IGameObjectEffectDescriptor);
			}
		}
		return list;
	}
}
