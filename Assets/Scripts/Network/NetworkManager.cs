using System;
using UnityEngine;
using WebSocketSharp;

public enum MatchMode{none, Duel, twoVtwo};

[RequireComponent(typeof(ColyseusClient))]
public class NetworkManager : MonoBehaviour
{
	public bool onlineMode = true;
	static ColyseusClient client;

	public void Initialize(){
		
		if (onlineMode){
			client = GetComponent<ColyseusClient>();

			// Initialize takes a callback, which here is an anonymous delegate
			StartCoroutine(client.Initialize( (msg) => {
				#if debug_server
					Debug.Log(msg); 
				#endif
			}));
		}
	}

	public static void SendMessage(string msg){
		client.chat.Send(msg);
	}

	public static void JoinMatchmakingQueue(MatchMode mode = MatchMode.Duel){
		client.registrar.RequestJoinMatchmaking();
	}
}