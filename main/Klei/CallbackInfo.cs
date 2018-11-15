using System;

namespace Klei
{
	public struct CallbackInfo
	{
		private HandleVector<Game.CallbackInfo>.Handle handle;

		public CallbackInfo(HandleVector<Game.CallbackInfo>.Handle h)
		{
			handle = h;
		}

		public void Release()
		{
			if (handle.IsValid())
			{
				Game.CallbackInfo item = Game.Instance.callbackManager.GetItem(handle);
				System.Action cb = item.cb;
				if (!item.manuallyRelease)
				{
					Game.Instance.callbackManager.Release(handle);
				}
				cb();
			}
		}
	}
}
