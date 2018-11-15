using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Bank Loader")]
	public class StudioBankLoader : MonoBehaviour
	{
		public LoaderGameEvent LoadEvent;

		public LoaderGameEvent UnloadEvent;

		[BankRef]
		public List<string> Banks;

		public string CollisionTag;

		public bool PreloadSamples;

		private bool isQuitting;

		private void HandleGameEvent(LoaderGameEvent gameEvent)
		{
			if (LoadEvent == gameEvent)
			{
				Load();
			}
			if (UnloadEvent == gameEvent)
			{
				Unload();
			}
		}

		private void Start()
		{
			RuntimeUtils.EnforceLibraryOrder();
			HandleGameEvent(LoaderGameEvent.ObjectStart);
		}

		private void OnApplicationQuit()
		{
			isQuitting = true;
		}

		private void OnDestroy()
		{
			if (!isQuitting)
			{
				HandleGameEvent(LoaderGameEvent.ObjectDestroy);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
			{
				HandleGameEvent(LoaderGameEvent.TriggerEnter);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
			{
				HandleGameEvent(LoaderGameEvent.TriggerExit);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
			{
				HandleGameEvent(LoaderGameEvent.TriggerEnter2D);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
			{
				HandleGameEvent(LoaderGameEvent.TriggerExit2D);
			}
		}

		public void Load()
		{
			foreach (string bank in Banks)
			{
				try
				{
					RuntimeManager.LoadBank(bank, PreloadSamples);
				}
				catch (BankLoadException exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}
			RuntimeManager.WaitForAllLoads();
		}

		public void Unload()
		{
			foreach (string bank in Banks)
			{
				RuntimeManager.UnloadBank(bank);
			}
		}
	}
}
