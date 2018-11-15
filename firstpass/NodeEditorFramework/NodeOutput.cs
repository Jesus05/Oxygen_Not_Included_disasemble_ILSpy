using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeEditorFramework
{
	public class NodeOutput : NodeKnob
	{
		private static GUIStyle _defaultStyle;

		public List<NodeInput> connections = new List<NodeInput>();

		[FormerlySerializedAs("type")]
		public string typeID;

		private TypeData _typeData;

		[NonSerialized]
		private object value;

		public bool calculationBlockade;

		protected override NodeSide defaultSide => NodeSide.Right;

		protected override GUIStyle defaultLabelStyle
		{
			get
			{
				if (_defaultStyle == null)
				{
					_defaultStyle = new GUIStyle(GUI.skin.label);
					_defaultStyle.alignment = TextAnchor.MiddleRight;
				}
				return _defaultStyle;
			}
		}

		internal TypeData typeData
		{
			get
			{
				CheckType();
				return _typeData;
			}
		}

		public bool IsValueNull => value == null;

		public static NodeOutput Create(Node nodeBody, string outputName, string outputType)
		{
			return Create(nodeBody, outputName, outputType, NodeSide.Right, 20f);
		}

		public static NodeOutput Create(Node nodeBody, string outputName, string outputType, NodeSide nodeSide)
		{
			return Create(nodeBody, outputName, outputType, nodeSide, 20f);
		}

		public static NodeOutput Create(Node nodeBody, string outputName, string outputType, NodeSide nodeSide, float sidePosition)
		{
			NodeOutput nodeOutput = ScriptableObject.CreateInstance<NodeOutput>();
			nodeOutput.typeID = outputType;
			nodeOutput.InitBase(nodeBody, nodeSide, sidePosition, outputName);
			nodeBody.Outputs.Add(nodeOutput);
			return nodeOutput;
		}

		public override void Delete()
		{
			while (connections.Count > 0)
			{
				connections[0].RemoveConnection();
			}
			body.Outputs.Remove(this);
			base.Delete();
		}

		protected internal override void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSerializableObject)
		{
			for (int i = 0; i < connections.Count; i++)
			{
				connections[i] = (replaceSerializableObject(connections[i]) as NodeInput);
			}
		}

		protected override void ReloadTexture()
		{
			CheckType();
			knobTexture = typeData.OutKnobTex;
		}

		private void CheckType()
		{
			if (_typeData == null || !_typeData.isValid())
			{
				_typeData = ConnectionTypes.GetTypeData(typeID);
			}
			if (_typeData == null || !_typeData.isValid())
			{
				ConnectionTypes.FetchTypes();
				_typeData = ConnectionTypes.GetTypeData(typeID);
				if (_typeData == null || !_typeData.isValid())
				{
					throw new UnityException("Could not find type " + typeID + "!");
				}
			}
		}

		public object GetValue()
		{
			return value;
		}

		public object GetValue(Type type)
		{
			if (type == null)
			{
				throw new UnityException("Trying to get value of " + base.name + " with null type!");
			}
			CheckType();
			if (type.IsAssignableFrom(typeData.Type))
			{
				return value;
			}
			Debug.LogError("Trying to GetValue<" + type.FullName + "> for Output Type: " + typeData.Type.FullName, null);
			return null;
		}

		public void SetValue(object Value)
		{
			CheckType();
			if (Value == null || typeData.Type.IsAssignableFrom(Value.GetType()))
			{
				value = Value;
			}
			else
			{
				Debug.LogError("Trying to SetValue of type " + Value.GetType().FullName + " for Output Type: " + typeData.Type.FullName, null);
			}
		}

		public T GetValue<T>()
		{
			CheckType();
			if (typeof(T).IsAssignableFrom(typeData.Type))
			{
				return (T)(value ?? (value = GetDefault<T>()));
			}
			Debug.LogError("Trying to GetValue<" + typeof(T).FullName + "> for Output Type: " + typeData.Type.FullName, null);
			return GetDefault<T>();
		}

		public void SetValue<T>(T Value)
		{
			CheckType();
			if (typeData.Type.IsAssignableFrom(typeof(T)))
			{
				value = Value;
			}
			else
			{
				Debug.LogError("Trying to SetValue<" + typeof(T).FullName + "> for Output Type: " + typeData.Type.FullName, null);
			}
		}

		public void ResetValue()
		{
			value = null;
		}

		public static T GetDefault<T>()
		{
			if (typeof(T).GetConstructor(Type.EmptyTypes) != null)
			{
				return Activator.CreateInstance<T>();
			}
			return default(T);
		}

		public static object GetDefault(Type type)
		{
			if (type.GetConstructor(Type.EmptyTypes) != null)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}

		public override Node GetNodeAcrossConnection()
		{
			return (connections.Count <= 0) ? null : connections[0].body;
		}
	}
}
