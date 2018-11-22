using NodeEditorFramework.Utilities;
using System;
using System.Linq;
using UnityEngine;

namespace NodeEditorFramework
{
	[Serializable]
	public class NodeKnob : ScriptableObject
	{
		public Node body;

		[NonSerialized]
		protected internal Texture2D knobTexture;

		public NodeSide side;

		public float sidePosition = 0f;

		public float sideOffset = 0f;

		protected virtual GUIStyle defaultLabelStyle => GUI.skin.label;

		protected virtual NodeSide defaultSide => NodeSide.Right;

		protected void InitBase(Node nodeBody, NodeSide nodeSide, float nodeSidePosition, string knobName)
		{
			body = nodeBody;
			side = nodeSide;
			sidePosition = nodeSidePosition;
			base.name = knobName;
			nodeBody.nodeKnobs.Add(this);
			ReloadKnobTexture();
		}

		public virtual void Delete()
		{
			body.nodeKnobs.Remove(this);
			UnityEngine.Object.DestroyImmediate(this, true);
		}

		internal void Check()
		{
			if (side == (NodeSide)0)
			{
				side = defaultSide;
			}
			if ((UnityEngine.Object)knobTexture == (UnityEngine.Object)null)
			{
				ReloadKnobTexture();
			}
		}

		protected void ReloadKnobTexture()
		{
			ReloadTexture();
			if ((UnityEngine.Object)knobTexture == (UnityEngine.Object)null)
			{
				throw new UnityException("Knob texture of " + base.name + " could not be loaded!");
			}
			if (side != defaultSide)
			{
				ResourceManager.SetDefaultResourcePath(NodeEditor.editorPath + "Resources/");
				int rotationStepsAntiCW = getRotationStepsAntiCW(defaultSide, side);
				ResourceManager.MemoryTexture memoryTexture = ResourceManager.FindInMemory(knobTexture);
				if (memoryTexture != null)
				{
					string[] array = new string[memoryTexture.modifications.Length + 1];
					memoryTexture.modifications.CopyTo(array, 0);
					array[array.Length - 1] = "Rotation:" + rotationStepsAntiCW;
					Texture2D texture = ResourceManager.GetTexture(memoryTexture.path, array);
					if ((UnityEngine.Object)texture != (UnityEngine.Object)null)
					{
						knobTexture = texture;
					}
					else
					{
						knobTexture = RTEditorGUI.RotateTextureCCW(knobTexture, rotationStepsAntiCW);
						ResourceManager.AddTextureToMemory(memoryTexture.path, knobTexture, array.ToArray());
					}
				}
				else
				{
					knobTexture = RTEditorGUI.RotateTextureCCW(knobTexture, rotationStepsAntiCW);
				}
			}
		}

		protected virtual void ReloadTexture()
		{
			knobTexture = RTEditorGUI.ColorToTex(1, Color.red);
		}

		public virtual ScriptableObject[] GetScriptableObjects()
		{
			return new ScriptableObject[0];
		}

		protected internal virtual void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSerializableObject)
		{
		}

		public virtual void DrawKnob()
		{
			Rect gUIKnob = GetGUIKnob();
			GUI.DrawTexture(gUIKnob, knobTexture);
		}

		public void DisplayLayout()
		{
			DisplayLayout(new GUIContent(base.name), defaultLabelStyle);
		}

		public void DisplayLayout(GUIStyle style)
		{
			DisplayLayout(new GUIContent(base.name), style);
		}

		public void DisplayLayout(GUIContent content)
		{
			DisplayLayout(content, defaultLabelStyle);
		}

		public void DisplayLayout(GUIContent content, GUIStyle style)
		{
			GUILayout.Label(content, style);
			if (Event.current.type == EventType.Repaint)
			{
				SetPosition();
			}
		}

		public void SetPosition(float position, NodeSide nodeSide)
		{
			if (side != nodeSide)
			{
				side = nodeSide;
				ReloadKnobTexture();
			}
			SetPosition(position);
		}

		public void SetPosition(float position)
		{
			sidePosition = position;
		}

		public void SetPosition()
		{
			Vector2 vector = GUILayoutUtility.GetLastRect().center + body.contentOffset;
			sidePosition = ((side != NodeSide.Bottom && side != NodeSide.Top) ? vector.y : vector.x);
		}

		public Rect GetGUIKnob()
		{
			Rect canvasSpaceKnob = GetCanvasSpaceKnob();
			canvasSpaceKnob.position += NodeEditor.curEditorState.zoomPanAdjust + NodeEditor.curEditorState.panOffset;
			return canvasSpaceKnob;
		}

		public Rect GetCanvasSpaceKnob()
		{
			Check();
			Vector2 knobSize = new Vector2((float)(knobTexture.width / knobTexture.height * NodeEditorGUI.knobSize), (float)(knobTexture.height / knobTexture.width * NodeEditorGUI.knobSize));
			Vector2 knobCenter = GetKnobCenter(knobSize);
			return new Rect(knobCenter.x - knobSize.x / 2f, knobCenter.y - knobSize.y / 2f, knobSize.x, knobSize.y);
		}

		private Vector2 GetKnobCenter(Vector2 knobSize)
		{
			if (side != NodeSide.Left)
			{
				if (side != NodeSide.Right)
				{
					if (side != NodeSide.Bottom)
					{
						return body.rect.position + new Vector2(sidePosition, 0f - sideOffset - knobSize.y / 2f);
					}
					return body.rect.position + new Vector2(sidePosition, sideOffset + knobSize.y / 2f + body.rect.height);
				}
				return body.rect.position + new Vector2(sideOffset + knobSize.x / 2f + body.rect.width, sidePosition);
			}
			return body.rect.position + new Vector2(0f - sideOffset - knobSize.x / 2f, sidePosition);
		}

		public Vector2 GetDirection()
		{
			return (side == NodeSide.Right) ? Vector2.right : ((side == NodeSide.Bottom) ? Vector2.up : ((side != NodeSide.Top) ? Vector2.left : Vector2.down));
		}

		private static int getRotationStepsAntiCW(NodeSide sideA, NodeSide sideB)
		{
			return sideB - sideA + ((sideA > sideB) ? 4 : 0);
		}

		public virtual Node GetNodeAcrossConnection()
		{
			return null;
		}
	}
}
