using System;
using System.Collections;
using System.Collections.Generic;

public class KSplitCompactedVector<Header, Payload> : KCompactedVectorBase, ICollection, IEnumerable
{
	private struct Enumerator : IEnumerator
	{
		public struct Value
		{
			public Header header;

			public Payload payload;
		}

		private readonly List<Header>.Enumerator headerBegin;

		private readonly List<Payload>.Enumerator payloadBegin;

		private List<Header>.Enumerator headerCurrent;

		private List<Payload>.Enumerator payloadCurrent;

		public object Current
		{
			get
			{
				Value value = default(Value);
				value.header = headerCurrent.Current;
				value.payload = payloadCurrent.Current;
				return value;
			}
		}

		public Enumerator(List<Header>.Enumerator headerEnumerator, List<Payload>.Enumerator payloadEnumerator)
		{
			headerBegin = headerEnumerator;
			payloadBegin = payloadEnumerator;
			Reset();
		}

		public bool MoveNext()
		{
			return headerCurrent.MoveNext() && payloadCurrent.MoveNext();
		}

		public void Reset()
		{
			headerCurrent = headerBegin;
			payloadCurrent = payloadBegin;
		}
	}

	protected List<Header> headers;

	protected List<Payload> payloads;

	public int Count => headers.Count;

	public bool IsSynchronized
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public object SyncRoot
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public KSplitCompactedVector(int initial_count = 0)
		: base(initial_count)
	{
		headers = new List<Header>(initial_count);
		payloads = new List<Payload>(initial_count);
	}

	public HandleVector<int>.Handle Allocate(Header header, ref Payload payload)
	{
		headers.Add(header);
		payloads.Add(payload);
		return Allocate(headers.Count - 1);
	}

	public HandleVector<int>.Handle Free(HandleVector<int>.Handle handle)
	{
		int num = headers.Count - 1;
		int free_component_idx;
		bool flag = Free(handle, num, out free_component_idx);
		if (flag)
		{
			if (free_component_idx < num)
			{
				headers[free_component_idx] = headers[num];
				payloads[free_component_idx] = payloads[num];
			}
			headers.RemoveAt(num);
			payloads.RemoveAt(num);
		}
		return (!flag) ? handle : HandleVector<int>.InvalidHandle;
	}

	public void GetData(HandleVector<int>.Handle handle, out Header header, out Payload payload)
	{
		int index = ComputeIndex(handle);
		header = headers[index];
		payload = payloads[index];
	}

	public Header GetHeader(HandleVector<int>.Handle handle)
	{
		return headers[ComputeIndex(handle)];
	}

	public Payload GetPayload(HandleVector<int>.Handle handle)
	{
		return payloads[ComputeIndex(handle)];
	}

	public void SetData(HandleVector<int>.Handle handle, Header new_data0, ref Payload new_data1)
	{
		int index = ComputeIndex(handle);
		headers[index] = new_data0;
		payloads[index] = new_data1;
	}

	public void SetHeader(HandleVector<int>.Handle handle, Header new_data)
	{
		headers[ComputeIndex(handle)] = new_data;
	}

	public void SetPayload(HandleVector<int>.Handle handle, ref Payload new_data)
	{
		payloads[ComputeIndex(handle)] = new_data;
	}

	public new virtual void Clear()
	{
		base.Clear();
		headers.Clear();
		payloads.Clear();
	}

	public void GetDataLists(out List<Header> headers, out List<Payload> payloads)
	{
		headers = this.headers;
		payloads = this.payloads;
	}

	public void CopyTo(Array array, int index)
	{
		throw new NotImplementedException();
	}

	public IEnumerator GetEnumerator()
	{
		return new Enumerator(headers.GetEnumerator(), payloads.GetEnumerator());
	}
}
