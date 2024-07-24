using UnityEngine;
using System.Collections;

public class NetworkRequestManager 
{

	private static NetworkRequestManager _instance = new NetworkRequestManager();
	public static NetworkRequestManager instance
	{
		get { return _instance; } 
	}

	public void RequestGameInit(string id,
								SendPacketInfo.OnSentAndReceivedPacketDelegate onSentCallback,
								SendPacketInfo.OnSentAndReceivedPacketDelegate onReceivedCallback)
	{
		SendGameInitPacketInfo sendPacket = new SendGameInitPacketInfo();
		sendPacket._OnSentCallback 		  = onSentCallback;
		sendPacket._OnReceivedCallback 	  = onReceivedCallback;
		sendPacket._SendAPI 			  = NetworkAPIInfo.GAME_INIT;
		sendPacket._id 					  = id;

		NetworkCommunicationManager.instance.Request(sendPacket);
	}
}
