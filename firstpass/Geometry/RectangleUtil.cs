using System;
using System.Collections.Generic;

namespace Geometry
{
	public class RectangleUtil
	{
		public static void Subtract(KRect r1, KRect r2, List<KRect> result, HorizontalEvent[] events, Strip[] strips, List<Strip> activeStrips, List<VerticalEvent> verticalEvents)
		{
			strips[0] = new Strip(r1.min.y, r1.max.y, false);
			strips[1] = new Strip(r2.min.y, r2.max.y, true);
			events[0] = new HorizontalEvent(r1.min.x, strips[0], true);
			events[1] = new HorizontalEvent(r1.max.x, strips[0], false);
			events[2] = new HorizontalEvent(r2.min.x, strips[1], true);
			events[3] = new HorizontalEvent(r2.max.x, strips[1], false);
			Array.Sort(events, (HorizontalEvent a, HorizontalEvent b) => a.x.CompareTo(b.x));
			activeStrips.Clear();
			for (int i = 0; i < events.Length; i++)
			{
				if (i > 0 && activeStrips.Count > 0)
				{
					GenerateActiveRectangles(events[i - 1].x, events[i].x, result, activeStrips, verticalEvents);
				}
				if (events[i].isStart)
				{
					activeStrips.Add(events[i].strip);
				}
				else
				{
					activeStrips.Remove(events[i].strip);
				}
			}
		}

		public static void GenerateActiveRectangles(float x0, float x1, List<KRect> result, List<Strip> activeStrips, List<VerticalEvent> verticalEvents)
		{
			verticalEvents.Clear();
			for (int i = 0; i < activeStrips.Count; i++)
			{
				verticalEvents.Add(new VerticalEvent(activeStrips[i].yMin, true, activeStrips[i].subtract));
				verticalEvents.Add(new VerticalEvent(activeStrips[i].yMax, false, activeStrips[i].subtract));
			}
			verticalEvents.Sort((VerticalEvent a, VerticalEvent b) => a.y.CompareTo(b.y));
			int num = 0;
			float num2 = float.NegativeInfinity;
			for (int j = 0; j < verticalEvents.Count; j++)
			{
				int num3 = num;
				int num4 = num;
				VerticalEvent verticalEvent = verticalEvents[j];
				int num5 = verticalEvent.isStart ? 1 : (-1);
				VerticalEvent verticalEvent2 = verticalEvents[j];
				num = num4 + num5 * ((!verticalEvent2.subtract) ? 1 : (-1));
				if (num == 1 && num3 == 0)
				{
					VerticalEvent verticalEvent3 = verticalEvents[j];
					num2 = verticalEvent3.y;
				}
				else if (num == 0 && num3 > 0)
				{
					float num6 = num2;
					VerticalEvent verticalEvent4 = verticalEvents[j];
					if (num6 != verticalEvent4.y && x0 != x1)
					{
						float y = num2;
						VerticalEvent verticalEvent5 = verticalEvents[j];
						result.Add(new KRect(x0, y, x1, verticalEvent5.y));
					}
				}
			}
		}
	}
}
