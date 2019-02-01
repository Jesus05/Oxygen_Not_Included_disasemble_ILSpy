using System.Collections.Generic;
using System.IO;

public class CodeWriter
{
	private List<string> Lines = new List<string>();

	private string Path;

	private int Indent;

	public CodeWriter(string path)
	{
		Path = path;
	}

	public void Comment(string text)
	{
		Lines.Add("// " + text);
	}

	public void BeginPartialClass(string class_name, string parent_name = null)
	{
		string text = "public partial class " + class_name;
		if (parent_name != null)
		{
			text = text + " : " + parent_name;
		}
		Line(text);
		Line("{");
		Indent++;
	}

	public void BeginClass(string class_name, string parent_name = null)
	{
		string text = "public class " + class_name;
		if (parent_name != null)
		{
			text = text + " : " + parent_name;
		}
		Line(text);
		Line("{");
		Indent++;
	}

	public void EndClass()
	{
		Indent--;
		Line("}");
	}

	public void BeginNameSpace(string name)
	{
		Line("namespace " + name);
		Line("{");
		Indent++;
	}

	public void EndNameSpace()
	{
		Indent--;
		Line("}");
	}

	public void BeginArrayStructureInitialization(string name)
	{
		Line("new " + name);
		Line("{");
		Indent++;
	}

	public void EndArrayStructureInitialization(bool last_item)
	{
		Indent--;
		if (!last_item)
		{
			Line("},");
		}
		else
		{
			Line("}");
		}
	}

	public void BeginArraArrayInitialization(string array_type, string array_name)
	{
		Line(array_name + " = new " + array_type + "[]");
		Line("{");
		Indent++;
	}

	public void EndArrayArrayInitialization(bool last_item)
	{
		Indent--;
		if (last_item)
		{
			Line("}");
		}
		else
		{
			Line("},");
		}
	}

	public void BeginConstructor(string name)
	{
		Line("public " + name + "()");
		Line("{");
		Indent++;
	}

	public void EndConstructor()
	{
		Indent--;
		Line("}");
	}

	public void BeginArrayAssignment(string array_type, string array_name)
	{
		Line(array_name + " = new " + array_type + "[]");
		Line("{");
		Indent++;
	}

	public void EndArrayAssignment()
	{
		Indent--;
		Line("};");
	}

	public void FieldAssignment(string field_name, string value)
	{
		Line(field_name + " = " + value + ";");
	}

	public void BeginStructureDelegateFieldInitializer(string name)
	{
		Line(name + "=delegate()");
		Line("{");
		Indent++;
	}

	public void EndStructureDelegateFieldInitializer()
	{
		Indent--;
		Line("},");
	}

	public void BeginIf(string condition)
	{
		Line("if(" + condition + ")");
		Line("{");
		Indent++;
	}

	public void BeginElseIf(string condition)
	{
		Indent--;
		Line("}");
		Line("else if(" + condition + ")");
		Line("{");
		Indent++;
	}

	public void EndIf()
	{
		Indent--;
		Line("}");
	}

	public void BeginFunctionDeclaration(string name, string parameter, string return_type)
	{
		Line("public " + return_type + " " + name + "(" + parameter + ")");
		Line("{");
		Indent++;
	}

	public void BeginFunctionDeclaration(string name, string return_type)
	{
		Line("public " + return_type + " " + name + "()");
		Line("{");
		Indent++;
	}

	public void EndFunctionDeclaration()
	{
		Indent--;
		Line("}");
	}

	private void InternalNamedParameter(string name, string value, bool last_parameter)
	{
		string str = "";
		if (!last_parameter)
		{
			str = ",";
		}
		Line(name + ":" + value + str);
	}

	public void NamedParameterBool(string name, bool value, bool last_parameter = false)
	{
		InternalNamedParameter(name, value.ToString().ToLower(), last_parameter);
	}

	public void NamedParameterInt(string name, int value, bool last_parameter = false)
	{
		InternalNamedParameter(name, value.ToString(), last_parameter);
	}

	public void NamedParameterFloat(string name, float value, bool last_parameter = false)
	{
		InternalNamedParameter(name, value.ToString() + "f", last_parameter);
	}

	public void NamedParameterString(string name, string value, bool last_parameter = false)
	{
		InternalNamedParameter(name, value, last_parameter);
	}

	public void BeginFunctionCall(string name)
	{
		Line(name);
		Line("(");
		Indent++;
	}

	public void EndFunctionCall()
	{
		Indent--;
		Line(");");
	}

	public void FunctionCall(string function_name, params string[] parameters)
	{
		string str = function_name + "(";
		for (int i = 0; i < parameters.Length; i++)
		{
			str += parameters[i];
			if (i != parameters.Length - 1)
			{
				str += ", ";
			}
		}
		Line(str + ");");
	}

	public void StructureFieldInitializer(string field, string value)
	{
		Line(field + " = " + value + ",");
	}

	public void StructureArrayFieldInitializer(string field, string field_type, params string[] values)
	{
		string str = field + " = new " + field_type + "[]{ ";
		for (int i = 0; i < values.Length; i++)
		{
			str += values[i];
			if (i < values.Length - 1)
			{
				str += ", ";
			}
		}
		str += " },";
		Line(str);
	}

	public void Line(string text = "")
	{
		for (int i = 0; i < Indent; i++)
		{
			text = "\t" + text;
		}
		Lines.Add(text);
	}

	public void Flush()
	{
		File.WriteAllLines(Path, Lines.ToArray());
	}
}
