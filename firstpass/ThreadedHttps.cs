using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

public class ThreadedHttps<T> where T : class, new()
{
	private class Singleton
	{
		internal static readonly T instance;

		static Singleton()
		{
			instance = new T();
		}
	}

	protected string serviceName;

	protected string CLIENT_KEY;

	protected string LIVE_ENDPOINT;

	private bool certFail;

	private const int retryCount = 3;

	protected Thread updateThread;

	protected List<byte[]> packets = new List<byte[]>();

	private EventWaitHandle _waitHandle = new AutoResetEvent(false);

	protected bool shouldQuit;

	protected bool quitOnError;

	private object _quitLock = new object();

	protected bool singleSend;

	public static T Instance => Singleton.instance;

	public bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		if (certFail)
		{
			return false;
		}
		certFail = true;
		string text = string.Empty;
		switch (sslPolicyErrors)
		{
		case SslPolicyErrors.None:
			certFail = false;
			break;
		case SslPolicyErrors.RemoteCertificateChainErrors:
			certFail = false;
			for (int i = 0; i < chain.ChainStatus.Length; i++)
			{
				string text2 = text;
				text = text2 + "[" + i + "] " + chain.ChainStatus[i].Status.ToString() + "\n";
				if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
				{
					chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
					chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
					chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
					chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
					if (!chain.Build((X509Certificate2)certificate))
					{
						certFail = true;
					}
				}
			}
			break;
		default:
			certFail = true;
			break;
		}
		if (certFail)
		{
			X509Certificate2 x509Certificate = new X509Certificate2(certificate);
			Debug.LogWarning(serviceName + ": " + sslPolicyErrors.ToString() + "\n" + text + "\n" + x509Certificate.ToString(), null);
		}
		return !certFail;
	}

	public void Start()
	{
		if (updateThread != null)
		{
			End();
		}
		if (!certFail)
		{
			packets = new List<byte[]>();
			shouldQuit = false;
			updateThread = new Thread(SendData);
			updateThread.Start();
		}
	}

	public void End()
	{
		Quit();
		if (updateThread != null)
		{
			if (!updateThread.Join(TimeSpan.FromSeconds(2.0)))
			{
				updateThread.Abort();
			}
			updateThread = null;
		}
	}

	protected virtual void OnReplyRecieved(WebResponse response)
	{
	}

	protected string Send(byte[] byteArray, bool isForce = false)
	{
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(RemoteCertificateValidationCallback));
		string text = string.Empty;
		int num = 0;
		while (true)
		{
			try
			{
				string requestUriString = "https://" + LIVE_ENDPOINT;
				Stream stream = null;
				WebResponse webResponse = null;
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
				httpWebRequest.AllowAutoRedirect = false;
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
				httpWebRequest.ContentLength = byteArray.Length;
				try
				{
					stream = httpWebRequest.GetRequestStream();
				}
				catch (WebException ex)
				{
					string message = ex.Message;
					text = DateTime.Now.ToLongTimeString() + " " + serviceName + ": Exception getting Request Stream:" + message;
					Debug.LogWarning(text, null);
					throw;
				}
				try
				{
					stream.Write(byteArray, 0, byteArray.Length);
				}
				catch (WebException ex2)
				{
					string message2 = ex2.Message;
					text = DateTime.Now.ToLongTimeString() + " " + serviceName + ": Exception writing data to Stream:" + message2;
					Debug.LogWarning(text, null);
					throw;
				}
				stream.Close();
				try
				{
					webResponse = httpWebRequest.GetResponse();
				}
				catch (WebException ex3)
				{
					string message3 = ex3.Message;
					WebResponse response = ex3.Response;
					if (response != null)
					{
						using (Stream stream2 = response.GetResponseStream())
						{
							StreamReader streamReader = new StreamReader(stream2);
							text = streamReader.ReadToEnd();
						}
					}
					else
					{
						text = " -- we.Response is NULL";
					}
					text = DateTime.Now.ToLongTimeString() + " " + serviceName + ": Exception getting response:" + message3 + text;
					Debug.LogWarning(text, null);
					throw;
				}
				text = ((HttpWebResponse)webResponse).StatusDescription;
				if (text != "OK")
				{
					stream = webResponse.GetResponseStream();
					StreamReader streamReader2 = new StreamReader(stream);
					string text2 = streamReader2.ReadToEnd();
					streamReader2.Close();
					stream.Close();
					text = string.Empty + serviceName + ": Server Responded with Status: [" + text + "] Response: " + text2;
				}
				else
				{
					OnReplyRecieved(webResponse);
				}
				webResponse.Close();
			}
			catch (Exception ex4)
			{
				if (shouldQuit)
				{
					continue;
				}
				if (certFail)
				{
					Debug.LogWarning(serviceName + ": Cert fail, quitting", null);
					try
					{
						OnReplyRecieved(null);
					}
					catch
					{
					}
					QuitOnError();
				}
				else
				{
					num++;
					if (num > 3)
					{
						text = DateTime.Now.ToLongTimeString() + " " + serviceName + ": Max Retries (" + 3 + ") reached. Disabling " + serviceName + "...";
						Debug.LogWarning(text, null);
						try
						{
							OnReplyRecieved(null);
						}
						catch
						{
						}
						QuitOnError();
					}
					else
					{
						string message4 = ex4.Message;
						string stackTrace = ex4.StackTrace;
						TimeSpan timeout = TimeSpan.FromSeconds(Math.Pow(2.0, (double)(num + 3)));
						text = DateTime.Now.ToLongTimeString() + " " + serviceName + ": Exception (retrying in " + timeout.TotalSeconds + " seconds): " + message4 + "\n" + stackTrace;
						Debug.LogWarning(text, null);
						if (!isForce)
						{
							Thread.Sleep(timeout);
							continue;
						}
						Debug.LogWarning(ex4.StackTrace, null);
					}
				}
			}
			break;
		}
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Remove(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(RemoteCertificateValidationCallback));
		return text;
	}

	protected bool ShouldQuit()
	{
		bool flag = false;
		lock (_quitLock)
		{
			return (shouldQuit && packets.Count == 0) || quitOnError;
		}
	}

	protected void QuitOnError()
	{
		lock (_quitLock)
		{
			quitOnError = true;
			shouldQuit = true;
		}
	}

	protected void Quit()
	{
		lock (_quitLock)
		{
			shouldQuit = true;
		}
	}

	protected byte[] GetPacket()
	{
		byte[] result = null;
		lock (packets)
		{
			if (packets.Count <= 0)
			{
				return result;
			}
			result = packets[0];
			packets.RemoveAt(0);
			return result;
		}
	}

	protected void PutPacket(byte[] packet, bool infront = false)
	{
		lock (packets)
		{
			if (infront)
			{
				packets.Insert(0, packet);
			}
			else
			{
				packets.Add(packet);
				_waitHandle.Set();
			}
		}
	}

	public void ForceSendData()
	{
		byte[] packet = GetPacket();
		while (true)
		{
			if (packet == null)
			{
				return;
			}
			if (Send(packet, true) != "OK")
			{
				break;
			}
			packet = GetPacket();
		}
		PutPacket(packet, true);
	}

	protected void SendData()
	{
		while (!ShouldQuit())
		{
			byte[] packet = GetPacket();
			if (packet != null)
			{
				if (Send(packet, false) != "OK")
				{
					PutPacket(packet, true);
				}
			}
			else
			{
				_waitHandle.WaitOne();
			}
			if (singleSend)
			{
				break;
			}
		}
	}
}
