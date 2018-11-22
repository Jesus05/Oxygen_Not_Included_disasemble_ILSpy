using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

public class ProfilerBase
{
	private bool initialised = false;

	private int idx = 0;

	protected StreamWriter proFile = null;

	private Stopwatch sw = null;

	private Stack<string> regionStack = new Stack<string>();

	private StringBuilder sb = new StringBuilder();

	private string category = "GAME";

	private string filePrefix = null;

	public ProfilerBase(string file_prefix)
	{
		filePrefix = file_prefix;
	}

	protected bool IsRecording()
	{
		return proFile != null;
	}

	public void Init()
	{
		proFile = null;
	}

	public void Finalise()
	{
		if (IsRecording())
		{
			StopRecording();
		}
	}

	public void ToggleRecording(string category = "GAME")
	{
		this.category = "G";
		if (!initialised)
		{
			initialised = true;
			Init();
		}
		if (IsRecording())
		{
			StopRecording();
		}
		else
		{
			StartRecording();
		}
	}

	public void StartRecording()
	{
		regionStack.Clear();
		proFile = new StreamWriter(filePrefix + idx.ToString() + ".json");
		idx++;
		sw = Stopwatch.StartNew();
		if (proFile != null)
		{
			proFile.WriteLine("{\"traceEvents\":[");
		}
	}

	public void StopRecording()
	{
		if (proFile != null)
		{
			WriteLine("end", "B", "},");
			WriteLine("end", "E", "}]}");
			proFile.Close();
			proFile = null;
		}
	}

	protected void Push(string region_name, string file, uint line)
	{
		if (IsRecording())
		{
			regionStack.Push(region_name);
			if (proFile != null)
			{
				WriteLine(region_name, "B", "},");
			}
		}
	}

	protected void Pop()
	{
		if (IsRecording() && regionStack.Count != 0)
		{
			WriteLine(regionStack.Pop(), "E", "},");
		}
	}

	protected void WriteLine(string region_name, string ph, string suffix)
	{
		if (proFile != null)
		{
			sb.Append("{\"cat\":\"");
			sb.Append(category);
			sb.Append("\",\"name\":\"");
			sb.Append(region_name);
			sb.Append("\",\"pid\":0");
			sb.Append(",\"ts\":");
			long elapsedTicks = sw.ElapsedTicks;
			long frequency = Stopwatch.Frequency;
			long num = elapsedTicks * 1000000 / frequency;
			sb.Append(num.ToString());
			sb.Append(",\"ph\":\"");
			sb.Append(ph);
			sb.Append("\"");
			sb.Append(suffix);
			proFile.WriteLine(sb.ToString());
			sb.Length = 0;
		}
	}
}
