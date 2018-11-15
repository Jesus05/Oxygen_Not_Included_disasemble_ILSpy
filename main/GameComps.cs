using System;
using System.Collections.Generic;
using System.Reflection;

public class GameComps : KComponents
{
	public static GravityComponents Gravities;

	public static FallerComponents Fallers;

	public static InfraredVisualizerComponents InfraredVisualizers;

	public static ElementSplitterComponents ElementSplitters;

	public static OreSizeVisualizerComponents OreSizeVisualizers;

	public static StructureTemperatureComponents StructureTemperatures;

	public static DiseaseContainers DiseaseContainers;

	public static RequiresFoundation RequiresFoundations;

	public static WhiteBoard WhiteBoards;

	private static Dictionary<Type, IKComponentManager> kcomponentManagers = new Dictionary<Type, IKComponentManager>();

	public GameComps()
	{
		FieldInfo[] fields = typeof(GameComps).GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			object obj = Activator.CreateInstance(fieldInfo.FieldType);
			fieldInfo.SetValue(null, obj);
			Add(obj as IComponentManager);
			if (obj is IKComponentManager)
			{
				IKComponentManager inst = obj as IKComponentManager;
				AddKComponentManager(fieldInfo.FieldType, inst);
			}
		}
	}

	public new void Clear()
	{
		FieldInfo[] fields = typeof(GameComps).GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			(fieldInfo.GetValue(null) as IComponentManager)?.Clear();
		}
	}

	public static void AddKComponentManager(Type kcomponent, IKComponentManager inst)
	{
		kcomponentManagers[kcomponent] = inst;
	}

	public static IKComponentManager GetKComponentManager(Type kcomponent_type)
	{
		return kcomponentManagers[kcomponent_type];
	}
}
