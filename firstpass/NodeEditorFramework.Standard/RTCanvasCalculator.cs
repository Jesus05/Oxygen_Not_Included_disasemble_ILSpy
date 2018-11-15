using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeEditorFramework.Standard
{
	public class RTCanvasCalculator : MonoBehaviour
	{
		public string canvasPath;

		public NodeCanvas canvas
		{
			get;
			private set;
		}

		private void Start()
		{
			LoadCanvas(canvasPath);
		}

		public void AssureCanvas()
		{
			if ((Object)canvas == (Object)null)
			{
				LoadCanvas(canvasPath);
				if ((Object)canvas == (Object)null)
				{
					throw new UnityException("No canvas specified to calculate on " + base.name + "!");
				}
			}
		}

		public void LoadCanvas(string path)
		{
			canvasPath = path;
			if (!string.IsNullOrEmpty(canvasPath))
			{
				canvas = NodeEditorSaveManager.LoadNodeCanvas(canvasPath, true);
				CalculateCanvas();
			}
			else
			{
				canvas = null;
			}
		}

		public void CalculateCanvas()
		{
			AssureCanvas();
			NodeEditor.RecalculateAll(canvas);
			DebugOutputResults();
		}

		private void DebugOutputResults()
		{
			AssureCanvas();
			List<Node> outputNodes = getOutputNodes();
			foreach (Node item in outputNodes)
			{
				string text = "(OUT) " + item.name + ": ";
				if (item.Outputs.Count == 0)
				{
					foreach (NodeInput input in item.Inputs)
					{
						string text2 = text;
						text = text2 + input.typeID + " " + ((!input.IsValueNull) ? input.GetValue().ToString() : "NULL") + "; ";
					}
				}
				else
				{
					foreach (NodeOutput output in item.Outputs)
					{
						string text2 = text;
						text = text2 + output.typeID + " " + ((!output.IsValueNull) ? output.GetValue().ToString() : "NULL") + "; ";
					}
				}
				Debug.Log(text, null);
			}
		}

		public List<Node> getInputNodes()
		{
			AssureCanvas();
			return (from node in canvas.nodes
			where (node.Inputs.Count == 0 && node.Outputs.Count != 0) || node.Inputs.TrueForAll((NodeInput input) => (Object)input.connection == (Object)null)
			select node).ToList();
		}

		public List<Node> getOutputNodes()
		{
			AssureCanvas();
			return (from node in canvas.nodes
			where (node.Outputs.Count == 0 && node.Inputs.Count != 0) || node.Outputs.TrueForAll((NodeOutput output) => output.connections.Count == 0)
			select node).ToList();
		}
	}
}
