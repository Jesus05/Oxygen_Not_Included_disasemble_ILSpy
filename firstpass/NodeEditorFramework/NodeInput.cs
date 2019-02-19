using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeEditorFramework
{
	public class NodeInput : NodeKnob
	{
		public NodeOutput connection;

		[FormerlySerializedAs("type")]
		public string typeID;

		private TypeData _typeData;

		protected override NodeSide defaultSide => NodeSide.Left;

		internal TypeData typeData
		{
			get
			{
				CheckType();
				return _typeData;
			}
		}

		public bool IsValueNull => !((UnityEngine.Object)connection != (UnityEngine.Object)null) || connection.IsValueNull;

		public static NodeInput Create(Node nodeBody, string inputName, string inputType)
		{
			return Create(nodeBody, inputName, inputType, NodeSide.Left, 20f);
		}

		public static NodeInput Create(Node nodeBody, string inputName, string inputType, NodeSide nodeSide)
		{
			return Create(nodeBody, inputName, inputType, nodeSide, 20f);
		}

		public static NodeInput Create(Node nodeBody, string inputName, string inputType, NodeSide nodeSide, float sidePosition)
		{
			NodeInput nodeInput = ScriptableObject.CreateInstance<NodeInput>();
			nodeInput.typeID = inputType;
			nodeInput.InitBase(nodeBody, nodeSide, sidePosition, inputName);
			nodeBody.Inputs.Add(nodeInput);
			return nodeInput;
		}

		public override void Delete()
		{
			RemoveConnection();
			body.Inputs.Remove(this);
			base.Delete();
		}

		protected internal override void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSerializableObject)
		{
			connection = (replaceSerializableObject(connection) as NodeOutput);
		}

		protected override void ReloadTexture()
		{
			CheckType();
			knobTexture = typeData.InKnobTex;
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
			return (!((UnityEngine.Object)connection != (UnityEngine.Object)null)) ? null : connection.GetValue();
		}

		public object GetValue(Type type)
		{
			return (!((UnityEngine.Object)connection != (UnityEngine.Object)null)) ? null : connection.GetValue(type);
		}

		public void SetValue(object value)
		{
			if ((UnityEngine.Object)connection != (UnityEngine.Object)null)
			{
				connection.SetValue(value);
			}
		}

		public T GetValue<T>()
		{
			return (!((UnityEngine.Object)connection != (UnityEngine.Object)null)) ? NodeOutput.GetDefault<T>() : connection.GetValue<T>();
		}

		public void SetValue<T>(T value)
		{
			if ((UnityEngine.Object)connection != (UnityEngine.Object)null)
			{
				connection.SetValue(value);
			}
		}

		public bool TryApplyConnection(NodeOutput output)
		{
			if (CanApplyConnection(output))
			{
				ApplyConnection(output);
				return true;
			}
			return false;
		}

		public bool CanApplyConnection(NodeOutput output)
		{
			if ((UnityEngine.Object)output == (UnityEngine.Object)null || (UnityEngine.Object)body == (UnityEngine.Object)output.body || (UnityEngine.Object)connection == (UnityEngine.Object)output || !typeData.Type.IsAssignableFrom(output.typeData.Type))
			{
				return false;
			}
			if (output.body.isChildOf(body) && !output.body.allowsLoopRecursion(body))
			{
				Debug.LogWarning("Cannot apply connection: Recursion detected!", null);
				return false;
			}
			return true;
		}

		public void ApplyConnection(NodeOutput output)
		{
			if (!((UnityEngine.Object)output == (UnityEngine.Object)null))
			{
				if ((UnityEngine.Object)connection != (UnityEngine.Object)null)
				{
					NodeEditorCallbacks.IssueOnRemoveConnection(this);
					connection.connections.Remove(this);
				}
				connection = output;
				output.connections.Add(this);
				if (!output.body.calculated)
				{
					NodeEditor.RecalculateFrom(output.body);
				}
				else
				{
					NodeEditor.RecalculateFrom(body);
				}
				output.body.OnAddOutputConnection(output);
				body.OnAddInputConnection(this);
				NodeEditorCallbacks.IssueOnAddConnection(this);
			}
		}

		public void RemoveConnection()
		{
			if (!((UnityEngine.Object)connection == (UnityEngine.Object)null))
			{
				NodeEditorCallbacks.IssueOnRemoveConnection(this);
				connection.connections.Remove(this);
				connection = null;
				NodeEditor.RecalculateFrom(body);
			}
		}

		public override Node GetNodeAcrossConnection()
		{
			return (!((UnityEngine.Object)connection != (UnityEngine.Object)null)) ? null : connection.body;
		}
	}
}
