using System;
using UnityEngine;
using WebSocketSharp;

[RequireComponent(typeof(ColyseusClient))]
public class NetworkManager : MonoBehaviour
{
	public bool onlineMode = true;
	ColyseusClient client;

	public void Initialize(){
		
		if (onlineMode){
			client = GetComponent<ColyseusClient>();

			// Initialize takes a callback, which here is an anonymous delegate
			StartCoroutine(client.Initialize( (msg) => Debug.Log(msg) ));
		}
	}
}