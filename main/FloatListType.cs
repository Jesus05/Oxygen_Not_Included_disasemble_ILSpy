using NodeEditorFramework;
using ProcGen.Noise;
using System;
using UnityEngine;

public class FloatListType : IConnectionTypeDeclaration
{
	public string Identifier => "FloatList";

	public Type Type => typeof(FloatList);

	public Color Color => Color.blue;

	public string InKnobTex => "Textures/In_Knob.png";

	public string OutKnobTex => "Textures/Out_Knob.png";
}
