using System;
using UnityEngine;

namespace NodeEditorFramework
{
	public interface IConnectionTypeDeclaration
	{
		string Identifier
		{
			get;
		}

		Type Type
		{
			get;
		}

		Color Color
		{
			get;
		}

		string InKnobTex
		{
			get;
		}

		string OutKnobTex
		{
			get;
		}
	}
}
