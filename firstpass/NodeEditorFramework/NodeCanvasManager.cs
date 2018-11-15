using NodeEditorFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NodeEditorFramework
{
	public class NodeCanvasManager
	{
		public static Dictionary<Type, NodeCanvasTypeData> TypeOfCanvases;

		private static Action<Type> _callBack;

		[CompilerGenerated]
		private static PopupMenu.MenuFunctionData _003C_003Ef__mg_0024cache0;

		public static void GetAllCanvasTypes()
		{
			TypeOfCanvases = new Dictionary<Type, NodeCanvasTypeData>();
			IEnumerable<Assembly> enumerable = from assembly in AppDomain.CurrentDomain.GetAssemblies()
			where assembly.FullName.Contains("Assembly")
			select assembly;
			foreach (Assembly item in enumerable)
			{
				foreach (Type item2 in from T in item.GetTypes()
				where T.IsClass && !T.IsAbstract && T.GetCustomAttributes(typeof(NodeCanvasTypeAttribute), false).Length > 0
				select T)
				{
					object[] customAttributes = item2.GetCustomAttributes(typeof(NodeCanvasTypeAttribute), false);
					NodeCanvasTypeAttribute nodeCanvasTypeAttribute = customAttributes[0] as NodeCanvasTypeAttribute;
					TypeOfCanvases.Add(item2, new NodeCanvasTypeData
					{
						CanvasType = item2,
						DisplayString = nodeCanvasTypeAttribute.Name
					});
				}
			}
		}

		private static void CreateNewCanvas(object userdata)
		{
			NodeCanvasTypeData nodeCanvasTypeData = (NodeCanvasTypeData)userdata;
			_callBack(nodeCanvasTypeData.CanvasType);
		}

		public static void PopulateMenu(ref GenericMenu menu, Action<Type> newNodeCanvas)
		{
			_callBack = newNodeCanvas;
			foreach (KeyValuePair<Type, NodeCanvasTypeData> typeOfCanvase in TypeOfCanvases)
			{
				GenericMenu obj = menu;
				NodeCanvasTypeData value = typeOfCanvase.Value;
				obj.AddItem(new GUIContent(value.DisplayString), false, CreateNewCanvas, typeOfCanvase.Value);
			}
		}
	}
}
