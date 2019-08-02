using KSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	[DebuggerDisplay("{amount.Name} {value} ({deltaAttribute.value}/{minAttribute.value}/{maxAttribute.value})")]
	public class AmountInstance : ModifierInstance<Amount>, ISaveLoadable, ISim200ms
	{
		private struct BatchUpdateContext
		{
			public struct Result
			{
				public AmountInstance amount_instance;

				public float previous;

				public float delta;
			}

			public List<UpdateBucketWithUpdater<ISim200ms>.Entry> amount_instances;

			public float time_delta;

			public ListPool<Result, AmountInstance>.PooledList results;

			public BatchUpdateContext(List<UpdateBucketWithUpdater<ISim200ms>.Entry> amount_instances, float time_delta)
			{
				for (int i = 0; i != amount_instances.Count; i++)
				{
					UpdateBucketWithUpdater<ISim200ms>.Entry value = amount_instances[i];
					value.lastUpdateTime = 0f;
					amount_instances[i] = value;
				}
				this.amount_instances = amount_instances;
				this.time_delta = time_delta;
				results = ListPool<Result, AmountInstance>.Allocate();
				results.Capacity = this.amount_instances.Count;
			}

			public void Finish()
			{
				foreach (Result result in results)
				{
					Result current = result;
					current.amount_instance.Publish(current.delta, current.previous);
				}
				results.Recycle();
			}
		}

		private struct BatchUpdateTask : IWorkItem<BatchUpdateContext>
		{
			private int start;

			private int end;

			public BatchUpdateTask(int start, int end)
			{
				this.start = start;
				this.end = end;
			}

			public void Run(BatchUpdateContext context)
			{
				for (int i = start; i != end; i++)
				{
					UpdateBucketWithUpdater<ISim200ms>.Entry entry = context.amount_instances[i];
					AmountInstance amountInstance = (AmountInstance)entry.data;
					float num = amountInstance.GetDelta() * context.time_delta;
					if (num != 0f)
					{
						context.results.Add(new BatchUpdateContext.Result
						{
							amount_instance = amountInstance,
							previous = amountInstance.value,
							delta = num
						});
						amountInstance.SetValue(amountInstance.value + num);
					}
				}
			}
		}

		[Serialize]
		public float value;

		public AttributeInstance minAttribute;

		public AttributeInstance maxAttribute;

		public AttributeInstance deltaAttribute;

		public Action<float> OnDelta;

		public System.Action OnMaxValueReached;

		public bool hide;

		private bool _paused;

		private static WorkItemCollection<BatchUpdateTask, BatchUpdateContext> batch_update_job = new WorkItemCollection<BatchUpdateTask, BatchUpdateContext>();

		public Amount amount => modifier;

		public bool paused
		{
			get
			{
				return _paused;
			}
			set
			{
				_paused = paused;
				if (_paused)
				{
					Deactivate();
				}
				else
				{
					Activate();
				}
			}
		}

		public AmountInstance(Amount amount, GameObject game_object)
			: base(game_object, amount)
		{
			Attributes attributes = game_object.GetAttributes();
			minAttribute = attributes.Add(amount.minAttribute);
			maxAttribute = attributes.Add(amount.maxAttribute);
			deltaAttribute = attributes.Add(amount.deltaAttribute);
		}

		public float GetMin()
		{
			return minAttribute.GetTotalValue();
		}

		public float GetMax()
		{
			return maxAttribute.GetTotalValue();
		}

		public float GetDelta()
		{
			return deltaAttribute.GetTotalValue();
		}

		public float SetValue(float value)
		{
			this.value = Mathf.Min(Mathf.Max(value, GetMin()), GetMax());
			return this.value;
		}

		public void Publish(float delta, float previous_value)
		{
			if (OnDelta != null)
			{
				OnDelta(delta);
			}
			if (OnMaxValueReached != null && previous_value < GetMax() && value >= GetMax())
			{
				OnMaxValueReached();
			}
		}

		public float ApplyDelta(float delta)
		{
			float previous_value = value;
			SetValue(value + delta);
			Publish(delta, previous_value);
			return value;
		}

		public string GetValueString()
		{
			return amount.GetValueString(this);
		}

		public string GetDescription()
		{
			return amount.GetDescription(this);
		}

		public string GetTooltip()
		{
			return amount.GetTooltip(this);
		}

		public void Activate()
		{
			SimAndRenderScheduler.instance.Add(this, false);
		}

		public void Sim200ms(float dt)
		{
		}

		public static void BatchUpdate(List<UpdateBucketWithUpdater<ISim200ms>.Entry> amount_instances, float time_delta)
		{
			if (time_delta != 0f)
			{
				BatchUpdateContext shared_data = new BatchUpdateContext(amount_instances, time_delta);
				batch_update_job.Reset(shared_data);
				int num = 512;
				for (int i = 0; i < amount_instances.Count; i += num)
				{
					int num2 = i + num;
					if (amount_instances.Count < num2)
					{
						num2 = amount_instances.Count;
					}
					batch_update_job.Add(new BatchUpdateTask(i, num2));
				}
				GlobalJobManager.Run(batch_update_job);
				shared_data.Finish();
			}
		}

		public void Deactivate()
		{
			SimAndRenderScheduler.instance.Remove(this);
		}
	}
}
