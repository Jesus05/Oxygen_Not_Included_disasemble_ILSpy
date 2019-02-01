using Klei;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using ProcGen.Noise;
using System;
using System.Collections.Generic;
using UnityEngine;

[NodeCanvasType("Noise Canvas")]
public class NoiseNodeCanvas : NodeCanvas
{
	private NoiseTreeFiles ntf;

	private TerminalNodeEditor terminator = null;

	private Rect lastRectPos;

	private Dictionary<string, PrimitiveNodeEditor> primitiveLookup = new Dictionary<string, PrimitiveNodeEditor>();

	private Dictionary<string, FilterNodeEditor> filterLookup = new Dictionary<string, FilterNodeEditor>();

	private Dictionary<string, ModifierModuleNodeEditor> modifierLookup = new Dictionary<string, ModifierModuleNodeEditor>();

	private Dictionary<string, SelectorModuleNodeEditor> selectorLookup = new Dictionary<string, SelectorModuleNodeEditor>();

	private Dictionary<string, TransformerNodeEditor> transformerLookup = new Dictionary<string, TransformerNodeEditor>();

	private Dictionary<string, CombinerModuleNodeEditor> combinerLookup = new Dictionary<string, CombinerModuleNodeEditor>();

	private Dictionary<string, FloatPointsNodeEditor> floatlistLookup = new Dictionary<string, FloatPointsNodeEditor>();

	private Dictionary<string, ControlPointsNodeEditor> ctrlpointsLookup = new Dictionary<string, ControlPointsNodeEditor>();

	[SerializeField]
	public SampleSettings settings
	{
		get;
		private set;
	}

	public static NoiseNodeCanvas CreateInstance()
	{
		NoiseNodeCanvas noiseNodeCanvas = ScriptableObject.CreateInstance<NoiseNodeCanvas>();
		noiseNodeCanvas.ntf = YamlIO<NoiseTreeFiles>.LoadFile(NoiseTreeFiles.GetPath(), null);
		return noiseNodeCanvas;
	}

	public override void UpdateSettings(string sceneCanvasName)
	{
		if (settings == null)
		{
			settings = new SampleSettings();
			settings.name = sceneCanvasName;
		}
	}

	public override string DrawAdditionalSettings(string sceneCanvasName)
	{
		return sceneCanvasName;
	}

	public override void BeforeSavingCanvas()
	{
		foreach (BaseNodeEditor node in nodes)
		{
			NoiseBase target = node.GetTarget();
			if (target != null)
			{
				target.pos = new Vector2f(node.rect.position);
			}
		}
	}

	public override void AdditionalSaveMethods(string sceneCanvasName, CompleteLoadCallback onComplete)
	{
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent("Load Yaml", "Loads the Canvas from a Yaml Save File")))
		{
			Load(sceneCanvasName, onComplete);
		}
		if (GUILayout.Button(new GUIContent("Save to Yaml", "Saves the Canvas to a Yaml file"), GUILayout.ExpandWidth(false)))
		{
			BeforeSavingCanvas();
			Tree tree = BuildTreeFromCanvas();
			if (tree != null)
			{
				tree.ClearEmptyLists();
				string treeFilePath = NoiseTreeFiles.GetTreeFilePath(sceneCanvasName);
				tree.Save(treeFilePath, null);
			}
		}
		GUILayout.EndHorizontal();
		if (ntf == null)
		{
			ntf = YamlIO<NoiseTreeFiles>.LoadFile(NoiseTreeFiles.GetPath(), null);
		}
		if (ntf != null && GUILayout.Button(new GUIContent("Load Tree", "Loads the Canvas from Trees list")))
		{
			GenericMenu genericMenu = new GenericMenu();
			foreach (string tree_file in ntf.tree_files)
			{
				genericMenu.AddItem(new GUIContent(tree_file), false, delegate(object fileName)
				{
					Load((string)fileName, onComplete);
				}, tree_file);
			}
			genericMenu.Show(lastRectPos.position, 40f);
		}
		if (Event.current.type == EventType.Repaint)
		{
			Rect lastRect = GUILayoutUtility.GetLastRect();
			lastRectPos = new Rect(lastRect.x + 2f, lastRect.yMax + 2f, lastRect.width - 4f, 0f);
		}
	}

	private void UpdateTerminator()
	{
		if ((UnityEngine.Object)terminator == (UnityEngine.Object)null)
		{
			foreach (Node node in nodes)
			{
				Type type = node.GetType();
				if (type == typeof(TerminalNodeEditor))
				{
					if ((UnityEngine.Object)terminator == (UnityEngine.Object)null)
					{
						terminator = (node as TerminalNodeEditor);
					}
					else
					{
						node.Delete();
					}
				}
			}
			if ((UnityEngine.Object)terminator == (UnityEngine.Object)null)
			{
				terminator = (TerminalNodeEditor)Node.Create("terminalNodeEditor", Vector2.zero);
			}
		}
		Vector2 position = terminator.rect.min + new Vector2(0f, -290f);
		DisplayNodeEditor displayNodeEditor = (DisplayNodeEditor)Node.Create("displayNodeEditor", position);
		displayNodeEditor.Inputs[0].ApplyConnection(terminator.Outputs[0]);
	}

	private Link GetLink(Node node)
	{
		Link link = new Link();
		Type type = node.GetType();
		if (type == typeof(PrimitiveNodeEditor))
		{
			PrimitiveNodeEditor primitiveNodeEditor = node as PrimitiveNodeEditor;
			link.name = primitiveNodeEditor.target.name;
			link.type = Link.Type.Primitive;
		}
		else if (type == typeof(FilterNodeEditor))
		{
			FilterNodeEditor filterNodeEditor = node as FilterNodeEditor;
			link.name = filterNodeEditor.target.name;
			link.type = Link.Type.Filter;
		}
		else if (type == typeof(TransformerNodeEditor))
		{
			TransformerNodeEditor transformerNodeEditor = node as TransformerNodeEditor;
			link.name = transformerNodeEditor.target.name;
			link.type = Link.Type.Transformer;
		}
		else if (type == typeof(SelectorModuleNodeEditor))
		{
			SelectorModuleNodeEditor selectorModuleNodeEditor = node as SelectorModuleNodeEditor;
			link.name = selectorModuleNodeEditor.target.name;
			link.type = Link.Type.Selector;
		}
		else if (type == typeof(ModifierModuleNodeEditor))
		{
			ModifierModuleNodeEditor modifierModuleNodeEditor = node as ModifierModuleNodeEditor;
			link.name = modifierModuleNodeEditor.target.name;
			link.type = Link.Type.Modifier;
		}
		else if (type == typeof(CombinerModuleNodeEditor))
		{
			CombinerModuleNodeEditor combinerModuleNodeEditor = node as CombinerModuleNodeEditor;
			link.name = combinerModuleNodeEditor.target.name;
			link.type = Link.Type.Combiner;
		}
		else if (type == typeof(FloatPointsNodeEditor))
		{
			FloatPointsNodeEditor floatPointsNodeEditor = node as FloatPointsNodeEditor;
			link.name = floatPointsNodeEditor.target.name;
			link.type = Link.Type.FloatPoints;
		}
		else if (type == typeof(ControlPointsNodeEditor))
		{
			ControlPointsNodeEditor controlPointsNodeEditor = node as ControlPointsNodeEditor;
			link.name = controlPointsNodeEditor.target.name;
			link.type = Link.Type.ControlPoints;
		}
		else if (type == typeof(TerminalNodeEditor))
		{
			link.name = "TERMINATOR";
			link.type = Link.Type.Terminator;
		}
		return link;
	}

	public Tree BuildTreeFromCanvas()
	{
		Tree tree = new Tree();
		tree.settings = settings;
		foreach (Node node in nodes)
		{
			Type type = node.GetType();
			if (type == typeof(PrimitiveNodeEditor))
			{
				PrimitiveNodeEditor primitiveNodeEditor = node as PrimitiveNodeEditor;
				if (primitiveNodeEditor.target.name == null || primitiveNodeEditor.target.name == "" || tree.primitives.ContainsKey(primitiveNodeEditor.target.name))
				{
					primitiveNodeEditor.target.name = "Primitive" + tree.primitives.Count;
				}
				tree.primitives.Add(primitiveNodeEditor.target.name, primitiveNodeEditor.target);
			}
			else if (type == typeof(FilterNodeEditor))
			{
				FilterNodeEditor filterNodeEditor = node as FilterNodeEditor;
				if (filterNodeEditor.target.name == null || filterNodeEditor.target.name == "" || tree.filters.ContainsKey(filterNodeEditor.target.name))
				{
					filterNodeEditor.target.name = "Filter" + tree.filters.Count;
				}
				tree.filters.Add(filterNodeEditor.target.name, filterNodeEditor.target);
			}
			else if (type == typeof(TransformerNodeEditor))
			{
				TransformerNodeEditor transformerNodeEditor = node as TransformerNodeEditor;
				if (transformerNodeEditor.target.name == null || transformerNodeEditor.target.name == "" || tree.transformers.ContainsKey(transformerNodeEditor.target.name))
				{
					transformerNodeEditor.target.name = "Transformer" + tree.transformers.Count;
				}
				tree.transformers.Add(transformerNodeEditor.target.name, transformerNodeEditor.target);
			}
			else if (type == typeof(SelectorModuleNodeEditor))
			{
				SelectorModuleNodeEditor selectorModuleNodeEditor = node as SelectorModuleNodeEditor;
				if (selectorModuleNodeEditor.target.name == null || selectorModuleNodeEditor.target.name == "" || tree.selectors.ContainsKey(selectorModuleNodeEditor.target.name))
				{
					selectorModuleNodeEditor.target.name = "Selector" + tree.selectors.Count;
				}
				tree.selectors.Add(selectorModuleNodeEditor.target.name, selectorModuleNodeEditor.target);
			}
			else if (type == typeof(ModifierModuleNodeEditor))
			{
				ModifierModuleNodeEditor modifierModuleNodeEditor = node as ModifierModuleNodeEditor;
				if (modifierModuleNodeEditor.target.name == null || modifierModuleNodeEditor.target.name == "" || tree.modifiers.ContainsKey(modifierModuleNodeEditor.target.name))
				{
					modifierModuleNodeEditor.target.name = "Modifier" + tree.modifiers.Count;
				}
				tree.modifiers.Add(modifierModuleNodeEditor.target.name, modifierModuleNodeEditor.target);
			}
			else if (type == typeof(CombinerModuleNodeEditor))
			{
				CombinerModuleNodeEditor combinerModuleNodeEditor = node as CombinerModuleNodeEditor;
				if (combinerModuleNodeEditor.target.name == null || combinerModuleNodeEditor.target.name == "" || tree.combiners.ContainsKey(combinerModuleNodeEditor.target.name))
				{
					combinerModuleNodeEditor.target.name = "Combiner" + tree.combiners.Count;
				}
				tree.combiners.Add(combinerModuleNodeEditor.target.name, combinerModuleNodeEditor.target);
			}
			else if (type == typeof(FloatPointsNodeEditor))
			{
				FloatPointsNodeEditor floatPointsNodeEditor = node as FloatPointsNodeEditor;
				if (floatPointsNodeEditor.target.name == null || floatPointsNodeEditor.target.name == "" || tree.floats.ContainsKey(floatPointsNodeEditor.target.name))
				{
					floatPointsNodeEditor.target.name = "Terrace Control" + tree.combiners.Count;
				}
				tree.floats.Add(floatPointsNodeEditor.target.name, floatPointsNodeEditor.target);
			}
			else if (type == typeof(ControlPointsNodeEditor))
			{
				ControlPointsNodeEditor controlPointsNodeEditor = node as ControlPointsNodeEditor;
				if (controlPointsNodeEditor.target.name == null || controlPointsNodeEditor.target.name == "" || tree.controlpoints.ContainsKey(controlPointsNodeEditor.target.name))
				{
					controlPointsNodeEditor.target.name = "Curve Control" + tree.combiners.Count;
				}
				tree.controlpoints.Add(controlPointsNodeEditor.target.name, controlPointsNodeEditor.target);
			}
			else if (type == typeof(TerminalNodeEditor) && (UnityEngine.Object)terminator == (UnityEngine.Object)null)
			{
				terminator = (node as TerminalNodeEditor);
			}
		}
		foreach (Node node2 in nodes)
		{
			Type type2 = node2.GetType();
			if (type2 == typeof(FilterNodeEditor))
			{
				FilterNodeEditor filterNodeEditor2 = node2 as FilterNodeEditor;
				NodeLink nodeLink = new NodeLink();
				nodeLink.target = GetLink(node2);
				if ((UnityEngine.Object)filterNodeEditor2.Inputs[0] != (UnityEngine.Object)null && (UnityEngine.Object)filterNodeEditor2.Inputs[0].connection != (UnityEngine.Object)null)
				{
					nodeLink.source0 = GetLink(filterNodeEditor2.Inputs[0].connection.body);
				}
				tree.links.Add(nodeLink);
			}
			else if (type2 == typeof(TransformerNodeEditor))
			{
				TransformerNodeEditor transformerNodeEditor2 = node2 as TransformerNodeEditor;
				NodeLink nodeLink2 = new NodeLink();
				nodeLink2.target = GetLink(node2);
				if ((UnityEngine.Object)transformerNodeEditor2.Inputs[0] != (UnityEngine.Object)null && (UnityEngine.Object)transformerNodeEditor2.Inputs[0].connection != (UnityEngine.Object)null)
				{
					nodeLink2.source0 = GetLink(transformerNodeEditor2.Inputs[0].connection.body);
				}
				if ((UnityEngine.Object)transformerNodeEditor2.Inputs[1] != (UnityEngine.Object)null && (UnityEngine.Object)transformerNodeEditor2.Inputs[1].connection != (UnityEngine.Object)null)
				{
					nodeLink2.source1 = GetLink(transformerNodeEditor2.Inputs[1].connection.body);
				}
				if ((UnityEngine.Object)transformerNodeEditor2.Inputs[2] != (UnityEngine.Object)null && (UnityEngine.Object)transformerNodeEditor2.Inputs[2].connection != (UnityEngine.Object)null)
				{
					nodeLink2.source2 = GetLink(transformerNodeEditor2.Inputs[2].connection.body);
				}
				if ((UnityEngine.Object)transformerNodeEditor2.Inputs[3] != (UnityEngine.Object)null && (UnityEngine.Object)transformerNodeEditor2.Inputs[3].connection != (UnityEngine.Object)null)
				{
					nodeLink2.source3 = GetLink(transformerNodeEditor2.Inputs[3].connection.body);
				}
				tree.links.Add(nodeLink2);
			}
			else if (type2 == typeof(SelectorModuleNodeEditor))
			{
				SelectorModuleNodeEditor selectorModuleNodeEditor2 = node2 as SelectorModuleNodeEditor;
				NodeLink nodeLink3 = new NodeLink();
				nodeLink3.target = GetLink(node2);
				if ((UnityEngine.Object)selectorModuleNodeEditor2.Inputs[0] != (UnityEngine.Object)null && (UnityEngine.Object)selectorModuleNodeEditor2.Inputs[0].connection != (UnityEngine.Object)null)
				{
					nodeLink3.source0 = GetLink(selectorModuleNodeEditor2.Inputs[0].connection.body);
				}
				if ((UnityEngine.Object)selectorModuleNodeEditor2.Inputs[1] != (UnityEngine.Object)null && (UnityEngine.Object)selectorModuleNodeEditor2.Inputs[1].connection != (UnityEngine.Object)null)
				{
					nodeLink3.source1 = GetLink(selectorModuleNodeEditor2.Inputs[1].connection.body);
				}
				if ((UnityEngine.Object)selectorModuleNodeEditor2.Inputs[2] != (UnityEngine.Object)null && (UnityEngine.Object)selectorModuleNodeEditor2.Inputs[2].connection != (UnityEngine.Object)null)
				{
					nodeLink3.source2 = GetLink(selectorModuleNodeEditor2.Inputs[2].connection.body);
				}
				tree.links.Add(nodeLink3);
			}
			else if (type2 == typeof(ModifierModuleNodeEditor))
			{
				ModifierModuleNodeEditor modifierModuleNodeEditor2 = node2 as ModifierModuleNodeEditor;
				NodeLink nodeLink4 = new NodeLink();
				nodeLink4.target = GetLink(node2);
				if ((UnityEngine.Object)modifierModuleNodeEditor2.Inputs[0] != (UnityEngine.Object)null && (UnityEngine.Object)modifierModuleNodeEditor2.Inputs[0].connection != (UnityEngine.Object)null)
				{
					nodeLink4.source0 = GetLink(modifierModuleNodeEditor2.Inputs[0].connection.body);
				}
				if ((UnityEngine.Object)modifierModuleNodeEditor2.Inputs[1] != (UnityEngine.Object)null && (UnityEngine.Object)modifierModuleNodeEditor2.Inputs[1].connection != (UnityEngine.Object)null)
				{
					nodeLink4.source1 = GetLink(modifierModuleNodeEditor2.Inputs[1].connection.body);
				}
				if ((UnityEngine.Object)modifierModuleNodeEditor2.Inputs[2] != (UnityEngine.Object)null && (UnityEngine.Object)modifierModuleNodeEditor2.Inputs[2].connection != (UnityEngine.Object)null)
				{
					nodeLink4.source2 = GetLink(modifierModuleNodeEditor2.Inputs[2].connection.body);
				}
				tree.links.Add(nodeLink4);
			}
			else if (type2 == typeof(CombinerModuleNodeEditor))
			{
				CombinerModuleNodeEditor combinerModuleNodeEditor2 = node2 as CombinerModuleNodeEditor;
				NodeLink nodeLink5 = new NodeLink();
				nodeLink5.target = GetLink(node2);
				if ((UnityEngine.Object)combinerModuleNodeEditor2.Inputs[0] != (UnityEngine.Object)null && (UnityEngine.Object)combinerModuleNodeEditor2.Inputs[0].connection != (UnityEngine.Object)null)
				{
					nodeLink5.source0 = GetLink(combinerModuleNodeEditor2.Inputs[0].connection.body);
				}
				if ((UnityEngine.Object)combinerModuleNodeEditor2.Inputs[1] != (UnityEngine.Object)null && (UnityEngine.Object)combinerModuleNodeEditor2.Inputs[1].connection != (UnityEngine.Object)null)
				{
					nodeLink5.source1 = GetLink(combinerModuleNodeEditor2.Inputs[1].connection.body);
				}
				tree.links.Add(nodeLink5);
			}
			else if (type2 == typeof(TerminalNodeEditor))
			{
				TerminalNodeEditor terminalNodeEditor = node2 as TerminalNodeEditor;
				NodeLink nodeLink6 = new NodeLink();
				nodeLink6.target = GetLink(node2);
				if ((UnityEngine.Object)terminalNodeEditor.Inputs[0] != (UnityEngine.Object)null && (UnityEngine.Object)terminalNodeEditor.Inputs[0].connection != (UnityEngine.Object)null)
				{
					nodeLink6.source0 = GetLink(terminalNodeEditor.Inputs[0].connection.body);
				}
				tree.links.Add(nodeLink6);
			}
		}
		return tree;
	}

	private NodeCanvas Load(string name, CompleteLoadCallback onComplete)
	{
		NodeCanvas nodeCanvas = null;
		string treeFilePath = NoiseTreeFiles.GetTreeFilePath(name);
		Tree tree = YamlIO<Tree>.LoadFile(treeFilePath, null);
		if (tree != null)
		{
			if (tree.settings.name == null || tree.settings.name == "")
			{
				tree.settings.name = name;
			}
			nodeCanvas = PopulateNoiseNodeEditor(tree);
		}
		onComplete(name, nodeCanvas);
		return nodeCanvas;
	}

	private Node GetNodeFromLink(Link link)
	{
		if (link != null)
		{
			switch (link.type)
			{
			case Link.Type.Primitive:
				if (primitiveLookup.ContainsKey(link.name))
				{
					return primitiveLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in primitives", null);
				break;
			case Link.Type.Filter:
				if (filterLookup.ContainsKey(link.name))
				{
					return filterLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in filters", null);
				break;
			case Link.Type.Modifier:
				if (modifierLookup.ContainsKey(link.name))
				{
					return modifierLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in modifiers", null);
				break;
			case Link.Type.Selector:
				if (selectorLookup.ContainsKey(link.name))
				{
					return selectorLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in selectors", null);
				break;
			case Link.Type.Transformer:
				if (transformerLookup.ContainsKey(link.name))
				{
					return transformerLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in transformers", null);
				break;
			case Link.Type.Combiner:
				if (combinerLookup.ContainsKey(link.name))
				{
					return combinerLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in combiners", null);
				break;
			case Link.Type.FloatPoints:
				if (floatlistLookup.ContainsKey(link.name))
				{
					return floatlistLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in float points", null);
				break;
			case Link.Type.ControlPoints:
				if (ctrlpointsLookup.ContainsKey(link.name))
				{
					return ctrlpointsLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in control points", null);
				break;
			case Link.Type.Terminator:
				if ((UnityEngine.Object)terminator == (UnityEngine.Object)null)
				{
					terminator = (TerminalNodeEditor)Node.Create("terminalNodeEditor", Vector2.zero);
					terminator.name = link.name;
				}
				return terminator;
			}
			Debug.LogError("Couldnt find link [" + link.name + "] [" + link.type.ToString() + "]", null);
			return null;
		}
		return null;
	}

	private static NoiseNodeCanvas PopulateNoiseNodeEditor(Tree tree)
	{
		NoiseNodeCanvas noiseNodeCanvas = (NoiseNodeCanvas)(NodeEditor.curNodeCanvas = CreateInstance());
		noiseNodeCanvas.Populate(tree);
		return noiseNodeCanvas;
	}

	private void Populate(Tree tree)
	{
		settings = tree.settings;
		primitiveLookup.Clear();
		foreach (KeyValuePair<string, Primitive> primitive in tree.primitives)
		{
			PrimitiveNodeEditor primitiveNodeEditor = (PrimitiveNodeEditor)Node.Create("primitiveNodeEditor", primitive.Value.pos);
			primitiveNodeEditor.name = primitive.Key;
			primitiveNodeEditor.target = primitive.Value;
			primitiveLookup.Add(primitive.Key, primitiveNodeEditor);
		}
		filterLookup.Clear();
		foreach (KeyValuePair<string, Filter> filter in tree.filters)
		{
			FilterNodeEditor filterNodeEditor = (FilterNodeEditor)Node.Create("filterNodeEditor", filter.Value.pos);
			filterNodeEditor.name = filter.Key;
			filterNodeEditor.target = filter.Value;
			filterLookup.Add(filter.Key, filterNodeEditor);
		}
		modifierLookup.Clear();
		foreach (KeyValuePair<string, ProcGen.Noise.Modifier> modifier in tree.modifiers)
		{
			ModifierModuleNodeEditor modifierModuleNodeEditor = (ModifierModuleNodeEditor)Node.Create("modifierModuleNodeEditor", modifier.Value.pos);
			modifierModuleNodeEditor.name = modifier.Key;
			modifierModuleNodeEditor.target = modifier.Value;
			modifierLookup.Add(modifier.Key, modifierModuleNodeEditor);
		}
		selectorLookup.Clear();
		foreach (KeyValuePair<string, Selector> selector in tree.selectors)
		{
			SelectorModuleNodeEditor selectorModuleNodeEditor = (SelectorModuleNodeEditor)Node.Create("selectorModuleNodeEditor", selector.Value.pos);
			selectorModuleNodeEditor.name = selector.Key;
			selectorModuleNodeEditor.target = selector.Value;
			selectorLookup.Add(selector.Key, selectorModuleNodeEditor);
		}
		transformerLookup.Clear();
		foreach (KeyValuePair<string, Transformer> transformer in tree.transformers)
		{
			TransformerNodeEditor transformerNodeEditor = (TransformerNodeEditor)Node.Create("transformerNodeEditor", transformer.Value.pos);
			transformerNodeEditor.name = transformer.Key;
			transformerNodeEditor.target = transformer.Value;
			transformerLookup.Add(transformer.Key, transformerNodeEditor);
		}
		combinerLookup.Clear();
		foreach (KeyValuePair<string, Combiner> combiner in tree.combiners)
		{
			CombinerModuleNodeEditor combinerModuleNodeEditor = (CombinerModuleNodeEditor)Node.Create("combinerModuleNodeEditor", combiner.Value.pos);
			combinerModuleNodeEditor.name = combiner.Key;
			combinerModuleNodeEditor.target = combiner.Value;
			combinerLookup.Add(combiner.Key, combinerModuleNodeEditor);
		}
		floatlistLookup.Clear();
		foreach (KeyValuePair<string, FloatList> @float in tree.floats)
		{
			FloatPointsNodeEditor floatPointsNodeEditor = (FloatPointsNodeEditor)Node.Create("floatPointsNodeEditor", @float.Value.pos);
			floatPointsNodeEditor.name = @float.Key;
			floatPointsNodeEditor.target = @float.Value;
			floatlistLookup.Add(@float.Key, floatPointsNodeEditor);
		}
		ctrlpointsLookup.Clear();
		foreach (KeyValuePair<string, ControlPointList> controlpoint in tree.controlpoints)
		{
			ControlPointsNodeEditor controlPointsNodeEditor = (ControlPointsNodeEditor)Node.Create("controlPointsNodeEditor", controlpoint.Value.pos);
			controlPointsNodeEditor.name = controlpoint.Key;
			controlPointsNodeEditor.target = controlpoint.Value;
			ctrlpointsLookup.Add(controlpoint.Key, controlPointsNodeEditor);
		}
		for (int i = 0; i < tree.links.Count; i++)
		{
			NodeLink nodeLink = tree.links[i];
			Node nodeFromLink = GetNodeFromLink(nodeLink.target);
			Node node = null;
			Node node2 = null;
			Node node3 = null;
			Node node4 = null;
			switch (nodeLink.target.type)
			{
			case Link.Type.Filter:
			case Link.Type.Terminator:
				node = GetNodeFromLink(nodeLink.source0);
				break;
			case Link.Type.Combiner:
				node = GetNodeFromLink(nodeLink.source0);
				node2 = GetNodeFromLink(nodeLink.source1);
				break;
			case Link.Type.Selector:
			case Link.Type.Modifier:
				node = GetNodeFromLink(nodeLink.source0);
				node2 = GetNodeFromLink(nodeLink.source1);
				node3 = GetNodeFromLink(nodeLink.source2);
				break;
			case Link.Type.Transformer:
				node = GetNodeFromLink(nodeLink.source0);
				node2 = GetNodeFromLink(nodeLink.source1);
				node3 = GetNodeFromLink(nodeLink.source2);
				node4 = GetNodeFromLink(nodeLink.source3);
				break;
			}
			if ((UnityEngine.Object)node != (UnityEngine.Object)null)
			{
				if (nodeFromLink.Inputs.Count == 0)
				{
					Debug.LogError("Target [" + nodeFromLink.name + "][" + nodeLink.target.type + "] doesnt have any inputs", null);
				}
				if (node.Outputs.Count == 0)
				{
					Debug.LogError("Source [" + node.name + "][" + nodeLink.source0.type + "] doesnt have any outputs", null);
				}
				nodeFromLink.Inputs[0].ApplyConnection(node.Outputs[0]);
			}
			if ((UnityEngine.Object)node2 != (UnityEngine.Object)null)
			{
				nodeFromLink.Inputs[1].ApplyConnection(node2.Outputs[0]);
			}
			if ((UnityEngine.Object)node3 != (UnityEngine.Object)null)
			{
				nodeFromLink.Inputs[2].ApplyConnection(node3.Outputs[0]);
			}
			if ((UnityEngine.Object)node4 != (UnityEngine.Object)null)
			{
				nodeFromLink.Inputs[3].ApplyConnection(node4.Outputs[0]);
			}
		}
		UpdateTerminator();
	}
}
