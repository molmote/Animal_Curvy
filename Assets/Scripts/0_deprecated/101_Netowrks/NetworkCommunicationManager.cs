using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NetworkCommunicationManager : MonoBehaviour 
{
	[SerializeField]
	private bool 				  _isLocal;

	private Queue<SendPacketInfo> _requestedPacketData;

	private static NetworkCommunicationManager _instance = null;
	public static NetworkCommunicationManager instance
	{
		get
		{
			if(_instance == null)
			{
				GameObject networkObj = new GameObject();
				networkObj.name 	  = "NetworkCommunicationManager";
				_instance = networkObj.AddComponent<NetworkCommunicationManager>();
			}

			return _instance;
		}
	}

	public void Request(SendPacketInfo sendPacketInfo)
	{
		_requestedPacketData.Enqueue(sendPacketInfo);
	}

	private IEnumerator Send(SendPacketInfo sendPacketInfo)
	{
		if(this._isLocal)
		{
			Loader.LoadTextAsset("Network/RecordPackets");
		}
		else
		{

		}

		yield break;
	}

	public void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void Start()
	{
		this.StartCoroutine(UpdateCoroutine());
	}

	public void OnEnable()
	{
		this.StartCoroutine(UpdateCoroutine());
	}

	private IEnumerator UpdateCoroutine()
	{
		while(enabled)
		{
			if(_requestedPacketData.Count > 0)
			{
				SendPacketInfo sendPacketInfo = _requestedPacketData.Dequeue();
				yield return this.StartCoroutine(Send(sendPacketInfo));
			}

			
			yield return null;
		}
	}
}
