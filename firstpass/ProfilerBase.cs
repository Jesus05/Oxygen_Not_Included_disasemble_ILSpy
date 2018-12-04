using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

public class ProfilerBase
{
	protected struct Thread
	{
		public Stack<string> regionStack;

		public StringBuilder sb;

		public int id;

		public Thread(int id)
		{
			regionStack = new Stack<string>();
			sb = new StringBuilder();
			this.id = id;
		}

		public void Reset()
		{
			regionStack.Clear();
			sb.Length = 0;
		}

		public void WriteLine(string category, string region_name, Stopwatch sw, string ph, string suffix)
		{
			ProfilerBase.WriteLine(sb, category, region_name, id, sw, ph, suffix);
		}
	}

	private bool initialised = false;

	private int idx = 0;

	protected StreamWriter proFile = null;

	private string category = "GAME";

	private string filePrefix = null;

	private Dictionary<int, Thread> threads;

	public Stopwatch sw;

	public ProfilerBase(string file_prefix)
	{
		filePrefix = file_prefix;
		threads = new Dictionary<int, Thread>();
		sw = new Stopwatch();
	}

	public static void WriteLine(StringBuilder sb, string category, string region_name, int tid, Stopwatch sw, string ph, string suffix)
	{
		sb.Append("{\"cat\":\"").Append(category).Append("\"");
		sb.Append(",\"name\":\"").Append(region_name).Append("\"");
		sb.Append(",\"pid\":0");
		sb.Append(",\"tid\":").Append(tid);
		long elapsedTicks = sw.ElapsedTicks;
		long frequency = Stopwatch.Frequency;
		long value = elapsedTicks * 1000000 / frequency;
		sb.Append(",\"ts\":").Append(value);
		sb.Append(",\"ph\":\"").Append(ph).Append("\"");
		sb.Append(suffix);
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
		foreach (KeyValuePair<int, Thread> thread in threads)
		{
			thread.Value.Reset();
		}
		proFile = new StreamWriter(filePrefix + idx.ToString() + ".json");
		idx++;
		if (proFile != null)
		{
			proFile.WriteLine("{\"traceEvents\":[");
		}
		sw.Start();
	}

	public void StopRecording()
	{
		sw.Stop();
		if (proFile != null)
		{
			foreach (KeyValuePair<int, Thread> thread2 in threads)
			{
				StreamWriter streamWriter = proFile;
				Thread value = thread2.Value;
				streamWriter.Write(value.sb.ToString());
				thread2.Value.Reset();
			}
			Thread thread = ManifestThread();
			thread.WriteLine(category, "end", sw, "B", "},");
			thread.WriteLine(category, "end", sw, "E", "}]}");
			proFile.Write(thread.sb.ToString());
			thread.Reset();
			proFile.Close();
			proFile = null;
		}
	}

	protected Thread ManifestThread()
	{
		if (!threads.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out Thread value))
		{
			value = new Thread(System.Threading.Thread.CurrentThread.ManagedThreadId);
			lock (this)
			{
				threads.Add(System.Threading.Thread.CurrentThread.ManagedThreadId, value);
			}
		}
		return value;
	}

	[Conditional("KPROFILER_VALIDATE_REGION_NAME")]
	private void ValidateRegionName(string region_name)
	{
		DebugUtil.Assert(!region_name.Contains("\""));
		region_name = "InvalidRegionName";
	}

	protected void Push(string region_name, string file, uint line)
	{
		if (IsRecording())
		{
			Thread thread = ManifestThread();
			thread.regionStack.Push(region_name);
			thread.WriteLine(category, region_name, sw, "B", "},");
		}
	}

	protected void Pop()
	{
		Thread thread = ManifestThread();
		if (IsRecording() && thread.regionStack.Count != 0)
		{
			thread.WriteLine(category, thread.regionStack.Pop(), sw, "E", "},");
		}
	}
}
