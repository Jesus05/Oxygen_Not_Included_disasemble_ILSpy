using NodeEditorFramework.Utilities;
using System;
using UnityEngine;

namespace NodeEditorFramework
{
	public class TypeData
	{
		private IConnectionTypeDeclaration declaration;

		public string Identifier
		{
			get;
			private set;
		}

		public Type Type
		{
			get;
			private set;
		}

		public Color Color
		{
			get;
			private set;
		}

		public Texture2D InKnobTex
		{
			get;
			private set;
		}

		public Texture2D OutKnobTex
		{
			get;
			private set;
		}

		internal TypeData(IConnectionTypeDeclaration typeDecl)
		{
			Identifier = typeDecl.Identifier;
			declaration = typeDecl;
			Type = declaration.Type;
			Color = declaration.Color;
			InKnobTex = ResourceManager.GetTintedTexture(declaration.InKnobTex, Color);
			OutKnobTex = ResourceManager.GetTintedTexture(declaration.OutKnobTex, Color);
			if (!isValid())
			{
				throw new DataMisalignedException("Type Declaration " + typeDecl.Identifier + " contains invalid data!");
			}
		}

		public TypeData(Type type)
		{
			Identifier = type.Name;
			declaration = null;
			Type = type;
			Color = Color.white;
			int hashCode = type.GetHashCode();
			byte[] bytes = BitConverter.GetBytes(hashCode);
			Color = new Color(Mathf.Pow((float)(int)bytes[0] / 255f, 0.5f), Mathf.Pow((float)(int)bytes[1] / 255f, 0.5f), Mathf.Pow((float)(int)bytes[2] / 255f, 0.5f));
			InKnobTex = ResourceManager.GetTintedTexture("Textures/In_Knob.png", Color);
			OutKnobTex = ResourceManager.GetTintedTexture("Textures/Out_Knob.png", Color);
		}

		public bool isValid()
		{
			return Type != null && (UnityEngine.Object)InKnobTex != (UnityEngine.Object)null && (UnityEngine.Object)OutKnobTex != (UnityEngine.Object)null;
		}
	}
}
