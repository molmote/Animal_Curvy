using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SendPacketInfo
{
	public SendPacketInfo() {}
	
	public  delegate void OnSentAndReceivedPacketDelegate(object JSONPacketInfo);
	public  OnSentAndReceivedPacketDelegate _OnSentCallback;
	public  OnSentAndReceivedPacketDelegate _OnReceivedCallback;
	public  string 						    _SendAPI;
}

public class SendGameInitPacketInfo : SendPacketInfo
{
	public  string _id;
}
