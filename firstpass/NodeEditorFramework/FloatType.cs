using System;
using UnityEngine;

namespace NodeEditorFramework
{
	public class FloatType : IConnectionTypeDeclaration
	{
		public string Identifier => "Float";

		public Type Type => typeof(float);

		public Color Color => Color.cyan;

		public string InKnobTex => "Textures/In_Knob.png";

		public string OutKnobTex => "Textures/Out_Knob.png";
	}
}
