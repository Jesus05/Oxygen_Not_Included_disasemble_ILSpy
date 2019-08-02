using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeTypes
	{
		public static Dictionary<Node, NodeData> nodes;

		public static void FetchNodes()
		{
			nodes = new Dictionary<Node, NodeData>();
			IEnumerable<Assembly> enumerable = from assembly in AppDomain.CurrentDomain.GetAssemblies()
			where assembly.FullName.Contains("Assembly")
			select assembly;
			foreach (Assembly item in enumerable)
			{
				foreach (Type item2 in from T in item.GetTypes()
				where T.IsClass && !T.IsAbstract && T.IsSubclassOf(typeof(Node))
				select T)
				{
					object[] customAttributes = item2.GetCustomAttributes(typeof(NodeAttribute), false);
					NodeAttribute nodeAttribute = customAttributes[0] as NodeAttribute;
					if (nodeAttribute == null || !nodeAttribute.hide)
					{
						try
						{
							Node node = ScriptableObject.CreateInstance(item2.Name) as Node;
							node = node.Create(Vector2.zero);
							nodes.Add(node, new NodeData((nodeAttribute != null) ? nodeAttribute.contextText : node.name, nodeAttribute.typeOfNodeCanvas));
						}
						catch (Exception ex)
						{
							Debug.LogError(ex.Message + " " + item2.Name);
						}
					}
				}
			}
		}

		public static NodeData getNodeData(Node node)
		{
			return nodes[getDefaultNode(node.GetID)];
		}

		public static Node getDefaultNode(string nodeID)
		{
			return nodes.Keys.Single((Node node) => node.GetID == nodeID);
		}

		public static T getDefaultNode<T>() where T : Node
		{
			return nodes.Keys.Single((Node node) => node.GetType() == typeof(T)) as T;
		}

		public static List<Node> getCompatibleNodes(NodeOutput nodeOutput)
		{
			if ((UnityEngine.Object)nodeOutput == (UnityEngine.Object)null)
			{
				throw new ArgumentNullException("nodeOutput");
			}
			List<Node> list = new List<Node>();
			foreach (Node key in nodes.Keys)
			{
				for (int i = 0; i < key.Inputs.Count; i++)
				{
					NodeInput nodeInput = key.Inputs[i];
					if ((UnityEngine.Object)nodeInput == (UnityEngine.Object)null)
					{
						throw new UnityException("Input " + i + " is null!");
					}
					if (nodeInput.typeData.Type.IsAssignableFrom(nodeOutput.typeData.Type))
					{
						list.Add(key);
						break;
					}
				}
			}
			return list;
		}
	}
}
