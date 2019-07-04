#define ENABLE_PROFILER
using System;
using UnityEngine;
using UnityEngine.Profiling;

public class GarbageCollectionProfiler : MonoBehaviour
{
	private class Test
	{
	}

	private class StringTest : Test
	{
		private string _String;
	}

	private class ObjectTest : Test
	{
		private object _Object;
	}

	private class DelegateTest : Test
	{
		private System.Action _Delegate;
	}

	private class DelegateWithSingleHandler : Test
	{
		private System.Action _Delegate;

		public DelegateWithSingleHandler()
		{
			_Delegate = (System.Action)Delegate.Combine(_Delegate, new System.Action(DoNothing));
		}

		private void DoNothing()
		{
		}
	}

	public int _ObjectCount = 100000;

	private Test[] _Items;

	private void Update()
	{
		if (_Items == null || _Items.Length != _ObjectCount)
		{
			_Items = new Test[_ObjectCount];
			for (int i = 0; i < _ObjectCount; i++)
			{
				_Items[i] = new DelegateWithSingleHandler();
			}
		}
		Profiler.BeginSample("GCCollect");
		GC.Collect();
		Profiler.EndSample();
	}
}
