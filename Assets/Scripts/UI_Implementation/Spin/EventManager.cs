using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Waggle
{

// http://www.willrmiller.com/?p=87
// http://wiki.unity3d.com/index.php/CSharpEventManager
public class EventManager : MonoSingleton<EventManager> 
{
	public delegate void EventDelegate<T>(T evt) where T : GameEvent;
	public delegate void EventDelegate(GameEvent evt);
	
	protected Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate>();
	protected Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();
	
	protected Queue eventQueue = new Queue();
	protected float queueTime  = UnityEngine.Time.fixedDeltaTime;
	protected bool  showLog;

	public static float QueueTime
	{
		get { return Instance.queueTime; }
		set { Instance.queueTime = value; }
	}

	public static bool ShowLog 
	{
		get { return Instance.showLog; }
		set { Instance.showLog = value; }
	}
	
	public static void Subscribe<T>(EventDelegate<T> del) where T : GameEvent
	{
		if (Instance.delegateLookup.ContainsKey(del))
		{
			return;
		}
		
		EventDelegate newDel = (e) => del((T)e);
		Instance.delegateLookup[del] = newDel;

		EventDelegate tempDel;
		if (Instance.delegates.TryGetValue(typeof(T), out tempDel))
		{
			Instance.delegates[typeof(T)] = tempDel += newDel;
		}
		else 
		{
			Instance.delegates[typeof(T)] = newDel;
		}
	}

	public static void UnSubscribe<T>(EventDelegate<T> del) where T : GameEvent
	{
		if (Instance == null) return;
		
		EventDelegate internalDel;
		if (Instance.delegateLookup.TryGetValue(del, out internalDel))
		{
			EventDelegate tempDel;
			if (Instance.delegates.TryGetValue(typeof(T), out tempDel))
			{
				tempDel -= internalDel;
				if (tempDel == null)
				{
					Instance.delegates.Remove(typeof(T));
				}
				else 
				{
					Instance.delegates[typeof(T)] = tempDel;
				}
			}

			Instance.delegateLookup.Remove(del);
		}
	}

	public static void TriggerEvent(GameEvent evt)
	{
		EventDelegate del;
		if (Instance.delegates.TryGetValue(evt.GetType(), out del))
		{
			if (ShowLog)
			{
				Debug.Log("(Event) <color=green>" + evt.GetType().Name + "</color>");
			}

			del.Invoke(evt);
		}
	}

	public static void QueueEvent(GameEvent evt)
	{
		Instance.eventQueue.Enqueue(evt);
	}

	protected void Update()
	{
		float startTime = UnityEngine.Time.time;
		while (eventQueue.Count > 0 && (UnityEngine.Time.time - startTime) < QueueTime)
		{
			GameEvent evt = (GameEvent)eventQueue.Dequeue();
			TriggerEvent(evt);
		}
	}
}

}